using Godot;
using System;
using BasterBoer.Core.Time;

namespace BasterBoer.Fence
{
	/// <summary>
	/// Player interaction with fence segments.
	/// Detects when player is near a fence segment and provides:
	/// - Inspect: shows condition, age, last inspection date
	/// - Repair: restores condition to max (costs money)
	///
	/// Attach to an Area3D near fence segments or use fence chunk trigger zones.
	/// </summary>
	public partial class FenceInteraction : Area3D
	{
		[Export] public string SegmentId = "";
		[Export] public float InteractionRange = 5.0f;

		/// <summary>Emitted when player enters interaction range of a damaged fence.</summary>
		[Signal]
		public delegate void FenceInteractReadyEventHandler(string segmentId, int repairCost);

		/// <summary>Emitted when interaction completes.</summary>
		[Signal]
		public delegate void FenceInteractionDoneEventHandler(string segmentId, string action);

		private bool _playerInRange;

		public override void _Ready()
		{
			BodyEntered += OnBodyEntered;
			BodyExited += OnBodyExited;
		}

		private void OnBodyEntered(Node3D body)
		{
			if (body.IsInGroup("player") || body.Name.ToString().Contains("Boer"))
			{
				_playerInRange = true;
				CheckInteractionAvailable();
			}
		}

		private void OnBodyExited(Node3D body)
		{
			if (body.IsInGroup("player") || body.Name.ToString().Contains("Boer"))
			{
				_playerInRange = false;
			}
		}

		/// <summary>
		/// Checks if the nearby fence segment needs repair and emits signal if so.
		/// </summary>
		private void CheckInteractionAvailable()
		{
			if (string.IsNullOrEmpty(SegmentId)) return;

			var segment = FenceHealthSystem.Instance?.GetSegment(SegmentId);
			if (segment == null) return;

			if (segment.IsBreach || segment.Condition < 0.7f)
			{
				int cost = FenceHealthSystem.Instance.CalculateRepairCost(segment);
				EmitSignal(SignalName.FenceInteractReady, SegmentId, cost);
			}
		}

		/// <summary>
		/// Called when player presses interact key (E) while in range.
		/// Attempts repair if segment is damaged.
		/// </summary>
		public void OnInteract()
		{
			if (!_playerInRange || string.IsNullOrEmpty(SegmentId)) return;

			var segment = FenceHealthSystem.Instance?.GetSegment(SegmentId);
			if (segment == null) return;

			if (segment.IsBreach || segment.Condition < 0.7f)
			{
				bool success = FenceHealthSystem.Instance.RepairSegment(SegmentId);
				if (success)
				{
					int cost = FenceHealthSystem.Instance.CalculateRepairCost(segment);
					GD.Print($"[FenceInteraction] Repaired segment {SegmentId} for R{cost}");
					EmitSignal(SignalName.FenceInteractionDone, SegmentId, "repair");
				}
				else
				{
					GD.Print("[FenceInteraction] Insufficient funds for repair.");
					EmitSignal(SignalName.FenceInteractionDone, SegmentId, "insufficient_funds");
				}
			}
			else
			{
				InspectFence(segment);
				EmitSignal(SignalName.FenceInteractionDone, SegmentId, "inspect");
			}
		}

		/// <summary>
		/// Inspects a fence segment and prints information.
		/// </summary>
		private void InspectFence(FenceSegment segment)
		{
			string conditionDesc = segment.Condition switch
			{
				> 0.8f => "Good",
				> 0.5f => "Fair",
				> 0.25f => "Poor",
				_ => "BREACHED"
			};

			GD.Print($"[FenceInteraction] Inspecting segment {segment.Id}:");
			GD.Print($"  Type: {segment.Type}");
			GD.Print($"  Condition: {conditionDesc} ({segment.Condition:P0})");
			GD.Print($"  Max Condition: {segment.MaxCondition:P0}");
			GD.Print($"  Breach: {segment.IsBreach}");
			GD.Print($"  Last Inspected: {segment.LastInspected.ToDisplayString()}");
			GD.Print($"  Repair Cost: R{FenceHealthSystem.Instance.CalculateRepairCost(segment):N0}");
		}

		/// <summary>
		/// Gets current segment data for UI display.
		/// </summary>
		public FenceSegment GetCurrentSegment()
		{
			return FenceHealthSystem.Instance?.GetSegment(SegmentId);
		}

		public bool IsPlayerInRange => _playerInRange;
	}
}
