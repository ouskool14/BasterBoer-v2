using Godot;
using System;
using System.Collections.Generic;
using BasterBoer.Core;
using BasterBoer.Core.Time;
using WorldStreaming;

namespace BasterBoer.Fence
{
	/// <summary>
	/// Manages fence segment condition degradation and breach detection.
	/// Processes all segments monthly — must be fast for 1000+ segments.
	/// Uses direct array iteration (no LINQ) per ARCHITECTURE.md rules.
	/// </summary>
	public class FenceHealthSystem
	{
		private static FenceHealthSystem _instance;
		public static FenceHealthSystem Instance => _instance ??= new FenceHealthSystem();

		private readonly List<FenceSegment> _segments = new(2048);

		/// <summary>Emitted when a fence segment condition drops below breach threshold.</summary>
		public event Action<FenceSegment> OnFenceBreached;

		/// <summary>Emitted when a fence segment is repaired.</summary>
		public event Action<FenceSegment> OnFenceRepaired;

		/// <summary>Emitted when a fence segment's max condition drops too low (needs replacement).</summary>
		public event Action<FenceSegment> OnFenceNeedsReplacement;

		private FenceHealthSystem() { }

		/// <summary>Read-only access to all tracked segments.</summary>
		public IReadOnlyList<FenceSegment> Segments => _segments;

		/// <summary>
		/// Registers a fence segment for health tracking.
		/// </summary>
		public void RegisterSegment(FenceSegment segment)
		{
			_segments.Add(segment);
		}

		/// <summary>
		/// Registers multiple fence segments at once.
		/// </summary>
		public void RegisterSegments(IEnumerable<FenceSegment> segments)
		{
			_segments.AddRange(segments);
		}

		/// <summary>
		/// Gets a segment by ID.
		/// </summary>
		public FenceSegment GetSegment(string id)
		{
			for (int i = 0; i < _segments.Count; i++)
			{
				if (_segments[i].Id == id) return _segments[i];
			}
			return null;
		}

		/// <summary>
		/// Gets all segments within a chunk coordinate.
		/// Uses direct iteration for performance.
		/// </summary>
		public List<FenceSegment> GetSegmentsInChunk(ChunkCoord chunkCoord)
		{
			var result = new List<FenceSegment>();
			for (int i = 0; i < _segments.Count; i++)
			{
				if (_segments[i].ChunkId == chunkCoord)
					result.Add(_segments[i]);
			}
			return result;
		}

		/// <summary>
		/// Gets all segments that need inspection (low condition or not inspected recently).
		/// </summary>
		public List<FenceSegment> GetSegmentsNeedingInspection(GameDate currentDate)
		{
			var result = new List<FenceSegment>();
			for (int i = 0; i < _segments.Count; i++)
			{
				var seg = _segments[i];
				if (seg.IsBreach || seg.Condition < 0.5f)
				{
					result.Add(seg);
				}
			}
			return result;
		}

		/// <summary>
		/// Monthly tick — degrades all segments, checks for breaches.
		/// Must be fast: direct iteration, no allocations.
		/// </summary>
		/// <param name="currentDate">Current game date</param>
		/// <param name="hasStorm">Whether current weather includes storms</param>
		public void TickMonthly(GameDate currentDate, bool hasStorm)
		{
			for (int i = 0; i < _segments.Count; i++)
			{
				FenceSegment segment = _segments[i];

				// Natural degradation
				float degradation = segment.GetMonthlyDegradationRate();
				segment.Condition -= degradation;

				// Storm damage
				if (hasStorm)
				{
					segment.Condition -= 0.05f;
				}

				// Clamp condition
				segment.Condition = Math.Max(0f, segment.Condition);

				// Max condition degrades over years (eventual replacement needed)
				segment.MaxCondition -= 0.001f;
				segment.MaxCondition = Math.Max(0.3f, segment.MaxCondition);

				// Check for breach
				if (segment.Condition < FenceSegment.BreachThreshold && !segment.IsBreach)
				{
					segment.IsBreach = true;
					GD.Print($"[FenceHealth] BREACH detected: segment {segment.Id}");
					OnFenceBreached?.Invoke(segment);
				}

				// Check for replacement needed
				if (segment.MaxCondition < 0.4f && segment.MaxCondition >= 0.399f)
				{
					OnFenceNeedsReplacement?.Invoke(segment);
				}

				_segments[i] = segment;
			}
		}

		/// <summary>
		/// Repairs a fence segment to full condition (within max condition limit).
		/// Deducts cost from GameState.
		/// </summary>
		/// <param name="segmentId">Segment to repair</param>
		/// <returns>True if repair succeeded</returns>
		public bool RepairSegment(string segmentId)
		{
			for (int i = 0; i < _segments.Count; i++)
			{
				if (_segments[i].Id != segmentId) continue;

				FenceSegment segment = _segments[i];
				int cost = CalculateRepairCost(segment);

				// Check funds
				var gameState = GameState.Instance;
				if (gameState != null && gameState.CashBalance < cost)
				{
					GD.Print($"[FenceHealth] Cannot repair segment {segmentId}: need R{cost}, have R{gameState.CashBalance:F2}");
					return false;
				}

				// Deduct cost
				if (gameState != null && cost > 0)
				{
					gameState.CashBalance -= cost;
				}

				// Repair
				segment.Condition = segment.MaxCondition;
				segment.IsBreach = false;
				segment.LastInspected = Bootstrap.Time?.CurrentDate ?? new GameDate(2024, 1, 1);
				_segments[i] = segment;

				GD.Print($"[FenceHealth] Repaired segment {segmentId} for R{cost}");
				OnFenceRepaired?.Invoke(segment);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Inspects a fence segment (updates LastInspected date).
		/// </summary>
		public void InspectSegment(string segmentId)
		{
			for (int i = 0; i < _segments.Count; i++)
			{
				if (_segments[i].Id == segmentId)
				{
					_segments[i].LastInspected = Bootstrap.Time?.CurrentDate ?? new GameDate(2024, 1, 1);
					GD.Print($"[FenceHealth] Inspected segment {segmentId}, condition: {_segments[i].Condition:P0}");
					return;
				}
			}
		}

		/// <summary>
		/// Calculates repair cost based on how much damage needs fixing.
		/// </summary>
		public int CalculateRepairCost(FenceSegment segment)
		{
			float damageRatio = 1f - (segment.Condition / segment.MaxCondition);
			return (int)(segment.RepairCost * damageRatio);
		}

		/// <summary>
		/// Gets aggregate fence statistics.
		/// </summary>
		public FenceStats GetStats()
		{
			var stats = new FenceStats();
			stats.TotalSegments = _segments.Count;

			for (int i = 0; i < _segments.Count; i++)
			{
				var seg = _segments[i];
				if (seg.IsBreach)
					stats.BreachedSegments++;
				else if (seg.Condition < 0.5f)
					stats.DamagedSegments++;
				else
					stats.HealthySegments++;

				stats.TotalLength += seg.Length;
			}

			return stats;
		}
	}

	/// <summary>
	/// Aggregate statistics about fence health.
	/// </summary>
	public struct FenceStats
	{
		public int TotalSegments;
		public int HealthySegments;
		public int DamagedSegments;
		public int BreachedSegments;
		public float TotalLength;
	}
}
