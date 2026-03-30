using Godot;
using BasterBoer.Core.Time;
using WorldStreaming;

namespace BasterBoer.Fence
{
	/// <summary>
	/// Fence construction type. Determines durability, cost, and degradation rate.
	/// </summary>
	public enum FenceType
	{
		/// <summary>Standard farm fence — cheap, degrades faster, animals can push through.</summary>
		Standard = 0,
		/// <summary>Game fence — expensive, durable, keeps predators out.</summary>
		Game = 1,
		/// <summary>Electric fence — mid-cost, requires power, stuns animals.</summary>
		Electric = 2
	}

	/// <summary>
	/// Data model for a single fence segment between two posts.
	/// Tracks condition, breach state, and inspection history.
	/// Pure data — no Godot scene dependencies.
	/// </summary>
	public class FenceSegment
	{
		/// <summary>Unique identifier for this segment.</summary>
		public string Id;

		/// <summary>World position of the segment start point.</summary>
		public Vector3 StartPoint;

		/// <summary>World position of the segment end point.</summary>
		public Vector3 EndPoint;

		/// <summary>Construction type.</summary>
		public FenceType Type;

		/// <summary>Current condition from 0.0 (destroyed) to 1.0 (perfect).</summary>
		public float Condition;

		/// <summary>Maximum achievable condition. Degrades over years (replacement needed).</summary>
		public float MaxCondition;

		/// <summary>True if Condition is below breach threshold.</summary>
		public bool IsBreach;

		/// <summary>Last time this segment was inspected by player or staff.</summary>
		public GameDate LastInspected;

		/// <summary>Cost in ZAR to repair this segment to full condition.</summary>
		public int RepairCost;

		/// <summary>Zone IDs this segment borders (for paddock containment).</summary>
		public string[] LinkedZoneIds;

		/// <summary>Chunk coordinate this segment belongs to.</summary>
		public ChunkCoord ChunkId;

		/// <summary>Condition threshold below which a breach occurs.</summary>
		public const float BreachThreshold = 0.25f;

		/// <summary>
		/// Returns the midpoint of this fence segment.
		/// </summary>
		public Vector3 Midpoint => (StartPoint + EndPoint) * 0.5f;

		/// <summary>
		/// Returns the length of this fence segment.
		/// </summary>
		public float Length => StartPoint.DistanceTo(EndPoint);

		/// <summary>
		/// Returns degradation rate per month based on fence type.
		/// </summary>
		public float GetMonthlyDegradationRate()
		{
			return Type switch
			{
				FenceType.Standard => 0.015f,  // Degrades ~6.7% per year
				FenceType.Game => 0.005f,       // Degrades ~2% per year
				FenceType.Electric => 0.008f,   // Degrades ~3.3% per year
				_ => 0.01f
			};
		}

		/// <summary>
		/// Creates a fence segment with default values.
		/// </summary>
		public static FenceSegment Create(string id, Vector3 start, Vector3 end,
			FenceType type, ChunkCoord chunkId, GameDate installDate)
		{
			return new FenceSegment
			{
				Id = id,
				StartPoint = start,
				EndPoint = end,
				Type = type,
				Condition = 0.9f,
				MaxCondition = 1.0f,
				IsBreach = false,
				LastInspected = installDate,
				RepairCost = type switch
				{
					FenceType.Standard => 500,
					FenceType.Game => 2000,
					FenceType.Electric => 1200,
					_ => 800
				},
				LinkedZoneIds = System.Array.Empty<string>(),
				ChunkId = chunkId
			};
		}
	}
}
