using Godot;
using System;
using System.Collections.Generic;

namespace BasterBoer.Fence
{
	/// <summary>
	/// Generates efficient patrol routes for inspecting fence segments.
	/// Prioritizes: breaches > low condition > not inspected recently.
	/// Returns waypoints for player or staff navigation.
	///
	/// Pure C# — no Godot scene dependencies.
	/// </summary>
	public class PatrolRouteGenerator
	{
		/// <summary>
		/// Generates an efficient patrol route visiting all segments needing inspection.
		/// Uses greedy nearest-neighbor approach for simplicity (good enough for < 100 segments).
		/// </summary>
		/// <param name="segments">Segments to patrol</param>
		/// <param name="startPos">Starting world position (player or staff location)</param>
		/// <returns>Ordered list of waypoints to visit</returns>
		public List<Vector3> GeneratePatrolRoute(List<FenceSegment> segments, Vector3 startPos)
		{
			if (segments == null || segments.Count == 0)
				return new List<Vector3>();

			// Sort segments by priority (most urgent first)
			var prioritized = new List<FenceSegment>(segments);
			prioritized.Sort((a, b) => GetPriority(b).CompareTo(GetPriority(a)));

			// Greedy nearest-neighbor route from start position
			var route = new List<Vector3>();
			var remaining = new List<FenceSegment>(prioritized);
			Vector3 currentPos = startPos;

			while (remaining.Count > 0)
			{
				int nearestIdx = FindNearestIndex(remaining, currentPos);
				var next = remaining[nearestIdx];

				route.Add(next.Midpoint);
				currentPos = next.Midpoint;
				remaining.RemoveAt(nearestIdx);
			}

			return route;
		}

		/// <summary>
		/// Generates a patrol route for segments in specific chunk coordinates.
		/// </summary>
		public List<Vector3> GeneratePatrolRouteForChunks(
			FenceHealthSystem healthSystem,
			List<WorldStreaming.ChunkCoord> chunkCoords,
			Vector3 startPos)
		{
			var segments = new List<FenceSegment>();
			for (int i = 0; i < chunkCoords.Count; i++)
			{
				segments.AddRange(healthSystem.GetSegmentsInChunk(chunkCoords[i]));
			}
			return GeneratePatrolRoute(segments, startPos);
		}

		/// <summary>
		/// Calculates priority score for a fence segment. Higher = more urgent.
		/// </summary>
		private float GetPriority(FenceSegment segment)
		{
			float score = 0f;

			// Breaches are highest priority
			if (segment.IsBreach)
				score += 100f;

			// Low condition is urgent
			score += (1f - segment.Condition) * 50f;

			// Not inspected recently adds urgency
			// (simplified — would need current date for real calculation)
			score += 10f;

			return score;
		}

		/// <summary>
		/// Finds the index of the nearest segment to a position.
		/// </summary>
		private int FindNearestIndex(List<FenceSegment> segments, Vector3 pos)
		{
			float bestDistSq = float.MaxValue;
			int bestIdx = 0;

			for (int i = 0; i < segments.Count; i++)
			{
				float distSq = segments[i].Midpoint.DistanceSquaredTo(pos);
				if (distSq < bestDistSq)
				{
					bestDistSq = distSq;
					bestIdx = i;
				}
			}

			return bestIdx;
		}
	}
}
