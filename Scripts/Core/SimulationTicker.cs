using Godot;
using BasterBoer.Core.Time;
using BasterBoer.Core.Systems;
using BasterBoer.Fence;
using LandManagementSim.Simulation;

namespace BasterBoer.Core
{
	/// <summary>
	/// Subscribes to TimeSystem signals and dispatches simulation ticks to all
	/// managed systems. Separates heavy monthly updates from light daily updates
	/// to respect the 2ms tick budget from ARCHITECTURE.md.
	///
	/// Created and attached by Bootstrap during initialization.
	/// </summary>
	public partial class SimulationTicker : Node
	{
		/// <summary>Emitted when a full tick cycle completes (all systems updated).</summary>
		[Signal]
		public delegate void TickCycleCompletedEventHandler();

		private TimeSystem _timeSystem;
		private bool _initialized;

		public override void _Ready()
		{
			// Defer wiring to ensure TimeSystem is fully ready
			CallDeferred(MethodName.WireSignals);
		}

		/// <summary>
		/// Connects to TimeSystem events. Called deferred to guarantee TimeSystem._Ready() has fired.
		/// Also wires fence/water alerts to AlertSystem and EventLogger.
		/// </summary>
		private void WireSignals()
		{
			_timeSystem = Bootstrap.Time;

			if (_timeSystem == null)
			{
				GD.PrintErr("[SimulationTicker] TimeSystem not available — signals not connected.");
				return;
			}

			_timeSystem.OnDayPassed += HandleDayPassed;
			_timeSystem.OnMonthPassed += HandleMonthPassed;
			_timeSystem.OnSeasonChanged += HandleSeasonChanged;
			_timeSystem.OnYearPassed += HandleYearPassed;

			// Wire fence breaches to alerts
			var fenceHealth = FenceHealthSystem.Instance;
			if (fenceHealth != null)
			{
				fenceHealth.OnFenceBreached += (seg) =>
				{
					AlertSystem.Instance?.Add(AlertType.FenceBreach,
						$"Fence breach at ({seg.Midpoint.X:F0}, {seg.Midpoint.Z:F0})",
						seg.Midpoint, Fence.AlertPriority.Critical);
					EventLogger.Instance?.Log(EventCategory.Infrastructure,
						$"Fence segment {seg.Id} breached", seg.Midpoint);
				};
				fenceHealth.OnFenceRepaired += (seg) =>
				{
					EventLogger.Instance?.Log(EventCategory.Infrastructure,
						$"Fence segment {seg.Id} repaired");
				};
			}

			// Wire water events to alerts and logger
			var waterSys = WaterSystem.Instance;
			if (waterSys != null)
			{
				waterSys.OnWaterLevelCritical += (ws) =>
				{
					AlertSystem.Instance?.Add(AlertType.WaterLow,
						$"{ws.Name} low ({ws.CurrentLevel:P0})",
						ws.Position, Fence.AlertPriority.Warning);
					EventLogger.Instance?.Log(EventCategory.Infrastructure,
						$"{ws.Name} dropped below 25%", ws.Position);
				};
				waterSys.OnWaterDry += (ws) =>
				{
					AlertSystem.Instance?.Add(AlertType.WaterDry,
						$"{ws.Name} has dried up!",
						ws.Position, Fence.AlertPriority.Critical);
					EventLogger.Instance?.Log(EventCategory.Infrastructure,
						$"{ws.Name} has dried up!", ws.Position);
				};
				waterSys.OnWaterRepaired += (ws) =>
				{
					EventLogger.Instance?.Log(EventCategory.Infrastructure,
						$"{ws.Name} repaired");
				};
			}

			_initialized = true;
			GD.Print("[SimulationTicker] Signals wired to TimeSystem.");
		}

		/// <summary>
		/// Daily tick — lightweight updates that should execute quickly.
		/// Weather effects, herd movement ticks, water evaporation.
		/// </summary>
		private void HandleDayPassed(GameDate date)
		{
			// WeatherSystem day tick (it's an autoload with its own OnDayTicked method)
			var weather = WeatherSystem.Instance;
			weather?.OnDayTicked();

			// Get current weather from GameState (may have been updated by WeatherSystem above)
			var currentWeather = Bootstrap.Game?.CurrentWeather ?? WeatherState.Clear;

			// Water evaporation and rain effects (weather-dependent)
			WaterSystem.Instance?.OnDailyTick(date.Season, currentWeather);

			// Animal daily tick (resets, health effects)
			AnimalSystem.Instance?.OnDailyTick();

			// Staff daily tick (fatigue, morale drift)
			StaffSystem.Instance?.OnDailyTick();

			// Update GameState time of day
			Bootstrap.Game?.UpdateTimeOfDay(8.0f); // Reset to 8 AM each new day
		}

		/// <summary>
		/// Monthly tick — heavy simulation updates.
		/// Economy, animal aging/death/reproduction, staff salaries, water rainfall, fence degradation.
		/// </summary>
		private void HandleMonthPassed(GameDate date)
		{
			GD.Print($"[SimulationTicker] === Monthly tick: {date.ToDisplayString()} ===");

			// Economy (connects to OnMonthPassed itself, but we also trigger it explicitly)
			// EconomySystem already subscribes to OnMonthPassed in its _Ready()

			// Water rainfall and long-term dynamics
			WaterSystem.Instance?.OnMonthlyTick(date.Season, date.Month);

			// Animal aging, death, population dynamics
			AnimalSystem.Instance?.OnMonthlyTick();

			// Staff salaries, morale shifts, resignation checks
			StaffSystem.Instance?.OnMonthlyTick();

			// Fence degradation and breach checks
			bool hasStorm = Bootstrap.Game?.CurrentWeather == WeatherState.Storm;
			FenceHealthSystem.Instance?.TickMonthly(date, hasStorm);

			EmitSignal(SignalName.TickCycleCompleted);
		}

		/// <summary>
		/// Seasonal change — update weather probabilities, alert player.
		/// </summary>
		private void HandleSeasonChanged(Season newSeason, GameDate date)
		{
			GD.Print($"[SimulationTicker] Season changed to {newSeason} ({date.ToDisplayString()})");
		}

		/// <summary>
		/// Year tick — annual permits, grants, long-term decay.
		/// </summary>
		private void HandleYearPassed(int year, GameDate date)
		{
			GD.Print($"[SimulationTicker] Year {year} began.");
		}

		public override void _ExitTree()
		{
			if (_timeSystem != null && _initialized)
			{
				_timeSystem.OnDayPassed -= HandleDayPassed;
				_timeSystem.OnMonthPassed -= HandleMonthPassed;
				_timeSystem.OnSeasonChanged -= HandleSeasonChanged;
				_timeSystem.OnYearPassed -= HandleYearPassed;
			}
		}
	}
}
