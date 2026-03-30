using Godot;
using BasterBoer.Core.Time;
using BasterBoer.Core.Systems;

namespace BasterBoer.Debug
{
	/// <summary>
	/// Debug test harness for the Bootstrap system. Verifies that all systems
	/// are accessible and wired correctly at runtime.
	///
	/// Attach to any node in the scene. Toggle <see cref="Enabled"/> to disable.
	/// </summary>
	public partial class BootstrapTest : Node
	{
		/// <summary>Set to false to skip all debug checks.</summary>
		[Export] public bool Enabled = true;

		public override void _Ready()
		{
			if (!Enabled) return;

			GD.Print("=== BootstrapTest: Starting verification ===");

			// 1. Verify Bootstrap exists
			var bootstrap = Core.Bootstrap.Instance;
			if (bootstrap == null)
			{
				GD.PrintErr("[BootstrapTest] FAIL: Bootstrap.Instance is null!");
				return;
			}
			GD.Print("[BootstrapTest] PASS: /root/Bootstrap exists.");

			// 2. Verify GameState
			var game = Core.Bootstrap.Game;
			if (game == null)
			{
				GD.PrintErr("[BootstrapTest] FAIL: Bootstrap.Game is null!");
			}
			else
			{
				GD.Print($"[BootstrapTest] PASS: GameState accessible. Balance: R{game.CashBalance:F2}");
			}

			// 3. Verify TimeSystem
			var time = Core.Bootstrap.Time;
			if (time == null)
			{
				GD.PrintErr("[BootstrapTest] FAIL: Bootstrap.Time is null!");
			}
			else
			{
				GD.Print($"[BootstrapTest] PASS: TimeSystem accessible. Date: {time.CurrentDate.ToDisplayString()}");

				// Subscribe to month tick for verification
				time.OnMonthPassed += OnMonthPassed;
				GD.Print("[BootstrapTest] PASS: Subscribed to OnMonthPassed.");
			}

			// 4. Verify EconomySystem
			var econ = Core.Bootstrap.Economy;
			if (econ == null)
			{
				GD.PrintErr("[BootstrapTest] FAIL: Bootstrap.Economy is null!");
			}
			else
			{
				GD.Print("[BootstrapTest] PASS: EconomySystem accessible.");
			}

			// 5. Verify pure C# singletons
			var animals = LandManagementSim.Simulation.AnimalSystem.Instance;
			GD.Print(animals != null
				? "[BootstrapTest] PASS: AnimalSystem singleton exists."
				: "[BootstrapTest] FAIL: AnimalSystem singleton is null!");

			var staff = StaffSystem.Instance;
			GD.Print(staff != null
				? "[BootstrapTest] PASS: StaffSystem singleton exists."
				: "[BootstrapTest] FAIL: StaffSystem singleton is null!");

			var water = WaterSystem.Instance;
			GD.Print(water != null
				? "[BootstrapTest] PASS: WaterSystem singleton exists."
				: "[BootstrapTest] FAIL: WaterSystem singleton is null!");

			// 6. Verify SimulationTicker
			var ticker = Core.Bootstrap.Ticker;
			if (ticker == null)
			{
				GD.PrintErr("[BootstrapTest] FAIL: SimulationTicker is null!");
			}
			else
			{
				GD.Print("[BootstrapTest] PASS: SimulationTicker accessible.");
				ticker.TickCycleCompleted += OnTickCycleCompleted;
			}

			// 7. Verify WorldChunkStreamer (scene node, may not exist yet)
			var streamer = WorldStreaming.WorldChunkStreamer.Instance;
			if (streamer != null)
			{
				GD.Print($"[BootstrapTest] PASS: WorldChunkStreamer found. " +
					$"Active chunks: {streamer.ActiveChunkCount}");
			}
			else
			{
				GD.Print("[BootstrapTest] INFO: WorldChunkStreamer not found (may not be in scene yet).");
			}

			// 8. Verify FenceSystem (scene node)
			var fence = FenceSystem.Instance;
			if (fence != null)
			{
				GD.Print("[BootstrapTest] PASS: FenceSystem found.");
			}
			else
			{
				GD.Print("[BootstrapTest] INFO: FenceSystem not found (may not be in scene yet).");
			}

			GD.Print("=== BootstrapTest: Verification complete ===");
		}

		private void OnMonthPassed(GameDate date)
		{
			GD.Print($"[BootstrapTest] Month {date.Month}, Year {date.Year} started — systems responding.");
		}

		private void OnTickCycleCompleted()
		{
			GD.Print("[BootstrapTest] Tick cycle completed — all systems updated.");
		}

		public override void _ExitTree()
		{
			var time = Core.Bootstrap.Time;
			if (time != null)
			{
				time.OnMonthPassed -= OnMonthPassed;
			}

			var ticker = Core.Bootstrap.Ticker;
			if (ticker != null)
			{
				ticker.TickCycleCompleted -= OnTickCycleCompleted;
			}
		}
	}
}
