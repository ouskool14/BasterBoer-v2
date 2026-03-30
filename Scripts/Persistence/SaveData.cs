using System;
using System.Collections.Generic;
using Godot;

namespace BasterBoer.Persistence
{
	/// <summary>
	/// Root save data container. Holds all serializable game state.
	/// Versioned for future migration support.
	/// </summary>
	[Serializable]
	public class SaveData
	{
		// --- Metadata ---
		/// <summary>Save format version for migration.</summary>
		public int SaveVersion = 1;

		/// <summary>Unique save identifier.</summary>
		public string SaveId;

		/// <summary>When the save was created (UTC).</summary>
		public string SavedAt;

		/// <summary>Game version that created this save.</summary>
		public string GameVersion;

		// --- World generation ---
		/// <summary>World generation seed.</summary>
		public int WorldSeed;

		/// <summary>Map dimensions.</summary>
		public float MapSizeX;
		public float MapSizeZ;

		// --- Core systems ---
		/// <summary>Time/calendar state.</summary>
		public TimeData Time;

		/// <summary>Economy state.</summary>
		public EconomyData Economy;

		/// <summary>All herd data.</summary>
		public List<HerdData> Herds;

		/// <summary>All water source data.</summary>
		public List<WaterPointData> WaterPoints;

		/// <summary>Fence segment data.</summary>
		public List<FenceSegmentData> FenceSegments;

		/// <summary>Staff data.</summary>
		public List<StaffData> Staff;

		/// <summary>Weather state.</summary>
		public WeatherData Weather;

		/// <summary>Recent events for the event log.</summary>
		public List<GameEventData> RecentEvents;

		/// <summary>Player state (position, stats).</summary>
		public PlayerData Player;
	}

	/// <summary>Time system save data.</summary>
	[Serializable]
	public class TimeData
	{
		public int Day;
		public int Month;
		public int Year;
		public float TimeOfDay;
		public string CurrentSeason;
	}

	/// <summary>Economy system save data.</summary>
	[Serializable]
	public class EconomyData
	{
		public float Balance;
		public float TotalRevenue;
		public float TotalExpenses;
		public float MonthlyRevenue;
		public float MonthlyExpenses;
	}

	/// <summary>Herd save data.</summary>
	[Serializable]
	public class HerdData
	{
		public int HerdId;
		public string Species;
		public float CenterX;
		public float CenterY;
		public float CenterZ;
		public int Population;
		public float Thirst;
		public float Hunger;
		public float Fatigue;
	}

	/// <summary>Water source save data.</summary>
	[Serializable]
	public class WaterPointData
	{
		public int Id;
		public string Name;
		public string Type;
		public float PositionX;
		public float PositionY;
		public float PositionZ;
		public float Capacity;
		public float MaxCapacity;
		public bool IsOperational;
		public float Quality;
		public string Status;
	}

	/// <summary>Fence segment save data.</summary>
	[Serializable]
	public class FenceSegmentData
	{
		public string Id;
		public string Type;
		public float StartX;
		public float StartY;
		public float StartZ;
		public float EndX;
		public float EndY;
		public float EndZ;
		public float Condition;
		public float MaxCondition;
		public bool IsBreach;
		public string LastInspectedDate;
	}

	/// <summary>Staff member save data.</summary>
	[Serializable]
	public class StaffData
	{
		public int Id;
		public string Name;
		public string Role;
		public float SkillLevel;
		public float Morale;
		public float Loyalty;
		public float MonthlySalary;
		public string Status;
		public int MonthsEmployed;
		public int AssignedZoneId;
	}

	/// <summary>Weather state save data.</summary>
	[Serializable]
	public class WeatherData
	{
		public string CurrentWeather;
		public int ConsecutiveRainDays;
		public int ConsecutiveDryDays;
		public int DaysUntilWeatherChange;
	}

	/// <summary>Game event save data.</summary>
	[Serializable]
	public class GameEventData
	{
		public string Id;
		public string Category;
		public string Timestamp;
		public string Summary;
		public string Details;
	}

	/// <summary>Player save data.</summary>
	[Serializable]
	public class PlayerData
	{
		public float PositionX;
		public float PositionY;
		public float PositionZ;
		public float RotationY;
	}

	/// <summary>Summary info for save slot display.</summary>
	[Serializable]
	public class SaveSlotInfo
	{
		public string SlotName;
		public string SavedAt;
		public int Year;
		public int Month;
		public float Balance;
		public int SaveVersion;
	}
}
