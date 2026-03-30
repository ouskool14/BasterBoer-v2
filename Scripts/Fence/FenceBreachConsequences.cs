using Godot;
using System;
using System.Collections.Generic;
using BasterBoer.Core.Systems;
using LandManagementSim.Simulation;

namespace BasterBoer.Fence
{
	/// <summary>
	/// Handles consequences when fence segments breach.
	/// Subscribes to FenceHealthSystem.OnFenceBreached and applies:
	/// - Animal escape risk for herds in linked zones
	/// - Predator/poacher threat level increase
	/// - Player alerts via C# events
	///
	/// Pure simulation logic — no Godot scene dependencies.
	/// </summary>
	public class FenceBreachConsequences
	{
		private static FenceBreachConsequences _instance;
		public static FenceBreachConsequences Instance => _instance ??= new FenceBreachConsequences();

		/// <summary>Emitted when an alert should be shown to the player.</summary>
		public event Action<string, Vector3, AlertPriority> OnAlertRaised;

		/// <summary>Emitted when escape risk changes for a zone.</summary>
		public event Action<string, float> OnEscapeRiskChanged;

		// Zone threat tracking
		private readonly Dictionary<string, float> _zoneThreatLevels = new();
		private readonly Dictionary<string, float> _zoneEscapeRisk = new();

		private const float EscapeRiskPerBreach = 0.1f;
		private const float ThreatPerBreach = 0.2f;
		private const float MaxEscapeRisk = 0.8f;

		private FenceBreachConsequences() { }

		/// <summary>
		/// Initializes breach consequences by subscribing to FenceHealthSystem events.
		/// Call once during bootstrap.
		/// </summary>
		public void Initialize()
		{
			FenceHealthSystem.Instance.OnFenceBreached += OnFenceBreached;
			FenceHealthSystem.Instance.OnFenceRepaired += OnFenceRepaired;
			GD.Print("[FenceBreach] Initialized and listening for breach events.");
		}

		/// <summary>
		/// Called when a fence segment breaches.
		/// Increases escape risk and threat levels for linked zones.
		/// </summary>
		private void OnFenceBreached(FenceSegment segment)
		{
			// Consequence 1: Animals in linked zones can escape
			if (segment.LinkedZoneIds != null)
			{
				for (int i = 0; i < segment.LinkedZoneIds.Length; i++)
				{
					string zoneId = segment.LinkedZoneIds[i];
					IncreaseEscapeRisk(zoneId, EscapeRiskPerBreach);
					IncreaseZoneThreat(zoneId, ThreatPerBreach);
				}
			}

			// Consequence 2: Alert player
			string locationDesc = $"({segment.Midpoint.X:F0}, {segment.Midpoint.Z:F0})";
			OnAlertRaised?.Invoke(
				$"Fence breach at {locationDesc}",
				segment.Midpoint,
				AlertPriority.Critical
			);

			// Consequence 3: Increase general threat (predator/poacher access)
			GD.Print($"[FenceBreach] Consequences applied for segment {segment.Id}");
		}

		/// <summary>
		/// Called when a fence segment is repaired. Reduces escape risk in linked zones.
		/// </summary>
		private void OnFenceRepaired(FenceSegment segment)
		{
			if (segment.LinkedZoneIds != null)
			{
				for (int i = 0; i < segment.LinkedZoneIds.Length; i++)
				{
					string zoneId = segment.LinkedZoneIds[i];
					DecreaseEscapeRisk(zoneId, EscapeRiskPerBreach);
					DecreaseZoneThreat(zoneId, ThreatPerBreach * 0.5f);
				}
			}
		}

		/// <summary>
		/// Increases escape risk for a zone. Animals may escape when risk is high.
		/// </summary>
		private void IncreaseEscapeRisk(string zoneId, float amount)
		{
			if (!_zoneEscapeRisk.ContainsKey(zoneId))
				_zoneEscapeRisk[zoneId] = 0f;

			_zoneEscapeRisk[zoneId] = Math.Min(MaxEscapeRisk, _zoneEscapeRisk[zoneId] + amount);
			OnEscapeRiskChanged?.Invoke(zoneId, _zoneEscapeRisk[zoneId]);

			GD.Print($"[FenceBreach] Zone {zoneId} escape risk: {_zoneEscapeRisk[zoneId]:P0}");
		}

		/// <summary>
		/// Decreases escape risk for a zone.
		/// </summary>
		private void DecreaseEscapeRisk(string zoneId, float amount)
		{
			if (!_zoneEscapeRisk.ContainsKey(zoneId))
				return;

			_zoneEscapeRisk[zoneId] = Math.Max(0f, _zoneEscapeRisk[zoneId] - amount);
			OnEscapeRiskChanged?.Invoke(zoneId, _zoneEscapeRisk[zoneId]);
		}

		/// <summary>
		/// Increases threat level for a zone (predator/poacher access).
		/// </summary>
		private void IncreaseZoneThreat(string zoneId, float amount)
		{
			if (!_zoneThreatLevels.ContainsKey(zoneId))
				_zoneThreatLevels[zoneId] = 0f;

			_zoneThreatLevels[zoneId] = Math.Min(1f, _zoneThreatLevels[zoneId] + amount);
		}

		/// <summary>
		/// Decreases threat level for a zone.
		/// </summary>
		private void DecreaseZoneThreat(string zoneId, float amount)
		{
			if (!_zoneThreatLevels.ContainsKey(zoneId))
				return;

			_zoneThreatLevels[zoneId] = Math.Max(0f, _zoneThreatLevels[zoneId] - amount);
		}

		/// <summary>
		/// Gets the current escape risk for a zone.
		/// </summary>
		public float GetEscapeRisk(string zoneId)
		{
			return _zoneEscapeRisk.GetValueOrDefault(zoneId, 0f);
		}

		/// <summary>
		/// Gets the current threat level for a zone.
		/// </summary>
		public float GetZoneThreat(string zoneId)
		{
			return _zoneThreatLevels.GetValueOrDefault(zoneId, 0f);
		}

		/// <summary>
		/// Rolls escape check for herds in a zone. Animals may escape if risk is high.
		/// Called monthly by SimulationTicker.
		/// </summary>
		public void ProcessEscapeChecks(string zoneId)
		{
			float risk = GetEscapeRisk(zoneId);
			if (risk < 0.1f) return;

			// Check herds in this zone
			var animals = AnimalSystem.Instance;
			if (animals == null) return;

			// Simple escape check: each herd in zone has a chance to lose animals
			var rng = new Random();
			for (int i = 0; i < animals.Herds.Count; i++)
			{
				var herd = animals.Herds[i];
				if (herd.Animals.Length == 0) continue;

				// Roll against escape risk
				if (rng.NextDouble() < risk * 0.1f) // 10% of risk value per month
				{
					GD.Print($"[FenceBreach] Animals from herd {herd.HerdId} escaped from zone {zoneId}!");
					// TODO: Actually remove animals from herd when AnimalSystem supports it
				}
			}
		}
	}

	/// <summary>
	/// Alert priority levels for player notification.
	/// </summary>
	public enum AlertPriority
	{
		Info = 0,
		Warning = 1,
		Critical = 2
	}
}
