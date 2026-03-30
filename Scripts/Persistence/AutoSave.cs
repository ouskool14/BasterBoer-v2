using Godot;
using System;
using System.IO;
using System.Linq;

namespace BasterBoer.Persistence
{
	/// <summary>
	/// Automatic save system. Saves the game at regular intervals.
	/// Rotates auto-save slots (keeps last N saves).
	///
	/// Add to the scene tree or let Bootstrap create it.
	/// </summary>
	public partial class AutoSave : Node
	{
		[Export] public int AutoSaveIntervalMinutes = 10;
		[Export] public int MaxAutoSaves = 3;
		[Export] public bool Enabled = true;

		private float _timeSinceLastSave = 0f;

		public override void _Process(double delta)
		{
			if (!Enabled) return;
			if (SaveManager.Instance == null) return;

			_timeSinceLastSave += (float)delta;

			float intervalSeconds = AutoSaveIntervalMinutes * 60f;
			if (_timeSinceLastSave >= intervalSeconds)
			{
				PerformAutoSave();
				_timeSinceLastSave = 0f;
			}
		}

		/// <summary>
		/// Performs an auto-save with a timestamped slot name.
		/// </summary>
		private void PerformAutoSave()
		{
			var slotName = $"autosave_{DateTime.Now:yyyyMMdd_HHmmss}";
			bool success = SaveManager.Instance.SaveGame(slotName);

			if (success)
			{
				GD.Print($"[AutoSave] Saved to slot: {slotName}");
				CleanupOldAutoSaves();
			}
			else
			{
				GD.PrintErr("[AutoSave] Auto-save failed!");
			}
		}

		/// <summary>
		/// Removes old auto-saves, keeping only the most recent N.
		/// </summary>
		private void CleanupOldAutoSaves()
		{
			var slots = SaveManager.Instance.GetSaveSlots();
			var autoSaves = slots
				.Where(s => s.SlotName.StartsWith("autosave_"))
				.OrderByDescending(s => s.SavedAt)
				.ToList();

			for (int i = MaxAutoSaves; i < autoSaves.Count; i++)
			{
				SaveManager.Instance.DeleteSave(autoSaves[i].SlotName);
				GD.Print($"[AutoSave] Deleted old auto-save: {autoSaves[i].SlotName}");
			}
		}
	}
}
