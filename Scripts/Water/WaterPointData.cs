using Godot;
using BasterBoer.Core.Water;

namespace BasterBoer.Core.Systems
{
	/// <summary>
	/// Pure data class for water point serialization. Contains all state needed
	/// to save and restore a water source without any Godot scene dependencies.
	/// Used by the persistence system (Prompt 5) and network sync if needed.
	/// </summary>
	public class WaterPointData
	{
		/// <summary>Unique identifier.</summary>
		public int Id;

		/// <summary>Player-assigned name.</summary>
		public string Name;

		/// <summary>World position.</summary>
		public Vector3 Position;

		/// <summary>Source type (Dam, Borehole, Trough, River, Spring).</summary>
		public string Type;

		/// <summary>Current level (0.0 to 1.0).</summary>
		public float Capacity;

		/// <summary>Maximum capacity in cubic meters.</summary>
		public float MaxCapacity;

		/// <summary>Whether the source is operational.</summary>
		public bool IsOperational;

		/// <summary>Water quality (0.0 to 1.0).</summary>
		public float Quality;

		/// <summary>Current operational status as string.</summary>
		public string Status;

		/// <summary>Daily evaporation rate.</summary>
		public float EvaporationRate;

		/// <summary>Daily seepage rate.</summary>
		public float SeepageRate;

		/// <summary>Borehole output in liters per hour.</summary>
		public float BoreholeOutputLPH;

		/// <summary>Borehole depth in meters.</summary>
		public float BoreholeDepthM;

		/// <summary>Visual radius in meters.</summary>
		public float Radius;

		/// <summary>Repair cost in ZAR.</summary>
		public int RepairCostZAR;

		/// <summary>
		/// Creates a WaterPointData snapshot from a live WaterSource.
		/// </summary>
		public static WaterPointData FromWaterSource(WaterSource source)
		{
			return new WaterPointData
			{
				Id = source.Id,
				Name = source.Name,
				Position = source.Position,
				Type = source.Type.ToString(),
				Capacity = source.CurrentLevel,
				MaxCapacity = source.MaxCapacityM3,
				IsOperational = source.Status == WaterSourceStatus.Operational,
				Quality = source.QualityFactor,
				Status = source.Status.ToString(),
				EvaporationRate = source.EvaporationRate,
				SeepageRate = source.SeepageRate,
				BoreholeOutputLPH = source.BoreholeOutputLPH,
				BoreholeDepthM = source.BoreholeDepthM,
				Radius = source.Radius,
				RepairCostZAR = source.RepairCostZAR
			};
		}

		/// <summary>
		/// Restores a live WaterSource from this data snapshot.
		/// </summary>
		public WaterSource ToWaterSource()
		{
			var sourceType = System.Enum.TryParse<WaterSourceType>(Type, out var t)
				? t : WaterSourceType.Dam;
			var sourceStatus = System.Enum.TryParse<WaterSourceStatus>(Status, out var s)
				? s : WaterSourceStatus.Operational;

			return new WaterSource
			{
				Id = Id,
				Name = Name,
				Type = sourceType,
				Position = Position,
				CurrentLevel = Capacity,
				MaxCapacityM3 = MaxCapacity,
				Status = sourceStatus,
				QualityFactor = Quality,
				EvaporationRate = EvaporationRate,
				SeepageRate = SeepageRate,
				BoreholeOutputLPH = BoreholeOutputLPH,
				BoreholeDepthM = BoreholeDepthM,
				Radius = Radius,
				RepairCostZAR = RepairCostZAR,
				ChunkId = -1
			};
		}
	}
}
