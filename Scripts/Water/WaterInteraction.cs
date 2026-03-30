using Godot;
using BasterBoer.Core.Water;
using BasterBoer.Core.Systems;

namespace BasterBoer.Water
{
	/// <summary>
	/// Handles player interaction with water sources.
	/// Detects when player is near a water point and provides interaction options:
	/// - Inspect: shows current status (capacity, quality, flow rate)
	/// - Repair: fixes damaged water source (deducts funds via EconomySystem)
	/// - Refill: fills trough from another source
	///
	/// Attach to an Area3D child of a water point scene.
	/// </summary>
	public partial class WaterInteraction : Area3D
	{
		[Export] public int WaterSourceId = -1;
		[Export] public float InteractionRange = 5.0f;

		/// <summary>Emitted when player enters interaction range.</summary>
		[Signal]
		public delegate void PlayerInRangeEventHandler(int sourceId);

		/// <summary>Emitted when player leaves interaction range.</summary>
		[Signal]
		public delegate void PlayerLeftRangeEventHandler(int sourceId);

		/// <summary>Emitted when interaction completes (repair, refill, inspect).</summary>
		[Signal]
		public delegate void InteractionCompletedEventHandler(int sourceId, string action);

		private bool _playerInRange;
		private Node3D _playerNode;

		public override void _Ready()
		{
			// Connect area signals
			BodyEntered += OnBodyEntered;
			BodyExited += OnBodyExited;
		}

		private void OnBodyEntered(Node3D body)
		{
			// Check if it's the player (group "player" or name contains "Boer")
			if (body.IsInGroup("player") || body.Name.ToString().Contains("Boer"))
			{
				_playerInRange = true;
				_playerNode = body;
				EmitSignal(SignalName.PlayerInRange, WaterSourceId);
			}
		}

		private void OnBodyExited(Node3D body)
		{
			if (body.IsInGroup("player") || body.Name.ToString().Contains("Boer"))
			{
				_playerInRange = false;
				_playerNode = null;
				EmitSignal(SignalName.PlayerLeftRange, WaterSourceId);
			}
		}

		/// <summary>
		/// Called when player presses interact key (E) while in range.
		/// Performs the primary available action.
		/// </summary>
		public void OnInteract()
		{
			if (!_playerInRange || WaterSourceId < 0) return;

			var waterSystem = WaterSystem.Instance;
			if (waterSystem == null) return;

			var sourceOpt = waterSystem.GetWaterSource(WaterSourceId);
			if (!sourceOpt.HasValue)
			{
				GD.PrintErr($"[WaterInteraction] Water source {WaterSourceId} not found.");
				return;
			}

			var source = sourceOpt.Value;

			// Priority: Repair > Refill (trough) > Inspect
			if (source.Status == WaterSourceStatus.Damaged ||
				source.Status == WaterSourceStatus.PumpFailure)
			{
				TryRepair(source);
			}
			else if (source.Type == WaterSourceType.Trough && source.CurrentLevel < 0.5f)
			{
				GD.Print($"[WaterInteraction] Trough '{source.Name}' at {source.CurrentLevel:P0}. Refill available.");
				EmitSignal(SignalName.InteractionCompleted, WaterSourceId, "inspect_refill");
			}
			else
			{
				Inspect(source);
			}
		}

		private void TryRepair(WaterSource source)
		{
			var gameState = GameState.Instance;
			if (gameState != null && source.RepairCostZAR > 0 &&
				gameState.CashBalance < source.RepairCostZAR)
			{
				GD.Print($"[WaterInteraction] Cannot repair '{source.Name}': need R{source.RepairCostZAR:N0}, have R{gameState.CashBalance:N0}");
				EmitSignal(SignalName.InteractionCompleted, WaterSourceId, "insufficient_funds");
				return;
			}

			bool success = WaterSystem.Instance.RepairWaterSource(WaterSourceId);
			if (success)
			{
				GD.Print($"[WaterInteraction] Repaired '{source.Name}' for R{source.RepairCostZAR:N0}");
				EmitSignal(SignalName.InteractionCompleted, WaterSourceId, "repair");
			}
		}

		private void Inspect(WaterSource source)
		{
			string qualityDesc = source.QualityFactor switch
			{
				> 0.8f => "Pristine",
				> 0.6f => "Good",
				> 0.4f => "Fair",
				> 0.2f => "Poor",
				_ => "Contaminated"
			};

			GD.Print($"[WaterInteraction] Inspecting '{source.Name}':");
			GD.Print($"  Type: {source.Type}");
			GD.Print($"  Level: {source.CurrentLevel:P0}");
			GD.Print($"  Quality: {qualityDesc} ({source.QualityFactor:P0})");
			GD.Print($"  Status: {source.Status}");
			GD.Print($"  Volume: {source.CurrentVolumeM3:F1} / {source.MaxCapacityM3:F1} m³");

			EmitSignal(SignalName.InteractionCompleted, WaterSourceId, "inspect");
		}

		/// <summary>
		/// Gets current water source data for UI display.
		/// </summary>
		public WaterSource? GetCurrentSource()
		{
			return WaterSystem.Instance?.GetWaterSource(WaterSourceId);
		}

		public bool IsPlayerInRange => _playerInRange;
	}
}
