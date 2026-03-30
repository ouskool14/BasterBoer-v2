using Godot;
using System;
using System.Collections.Generic;

namespace BasterBoer.Core
{
	/// <summary>
	/// Types of alerts the player can receive.
	/// </summary>
	public enum AlertType
	{
		FenceBreach, WaterLow, WaterDry, HerdStressed, HerdSick,
		AnimalDeath, StaffIssue, PaymentDue, LoanOverdue,
		SeasonChange, DroughtWarning, EventNotification
	}

	/// <summary>
	/// A single alert entry in the player's alert queue.
	/// </summary>
	public class Alert
	{
		/// <summary>Unique identifier.</summary>
		public string Id;

		/// <summary>Alert category.</summary>
		public AlertType Type;

		/// <summary>Priority level.</summary>
		public Fence.AlertPriority Priority;

		/// <summary>Display message.</summary>
		public string Message;

		/// <summary>Optional world location (null if not location-based).</summary>
		public Vector3? WorldLocation;

		/// <summary>When the alert was created.</summary>
		public DateTime CreatedAt;

		/// <summary>If true, auto-dismiss when condition resolves.</summary>
		public bool AutoDismiss;
	}

	/// <summary>
	/// Central alert management system. Manages the player's alert queue.
	/// Emits signals when alerts are added, dismissed, or changed.
	/// Pure C# singleton — UI subscribes via events.
	/// </summary>
	public class AlertSystem
	{
		private static AlertSystem _instance;
		public static AlertSystem Instance => _instance ??= new AlertSystem();

		private readonly List<Alert> _alerts = new(32);
		private int _nextId;

		/// <summary>Emitted when a new alert is added.</summary>
		public event Action<Alert> OnAlertAdded;

		/// <summary>Emitted when an alert is dismissed.</summary>
		public event Action<string> OnAlertDismissed;

		/// <summary>Emitted when alert count changes.</summary>
		public event Action<int> OnAlertsChanged;

		private AlertSystem() { }

		/// <summary>Read-only access to current alerts.</summary>
		public IReadOnlyList<Alert> Alerts => _alerts;

		/// <summary>Current number of active alerts.</summary>
		public int Count => _alerts.Count;

		/// <summary>
		/// Adds a new alert to the queue.
		/// </summary>
		public Alert Add(AlertType type, string message, Vector3? location = null,
			Fence.AlertPriority priority = Fence.AlertPriority.Warning, bool autoDismiss = false)
		{
			var alert = new Alert
			{
				Id = $"alert_{_nextId++}",
				Type = type,
				Priority = priority,
				Message = message,
				WorldLocation = location,
				CreatedAt = DateTime.UtcNow,
				AutoDismiss = autoDismiss
			};

			_alerts.Add(alert);
			OnAlertAdded?.Invoke(alert);
			OnAlertsChanged?.Invoke(_alerts.Count);

			GD.Print($"[AlertSystem] [{priority}] {message}");
			return alert;
		}

		/// <summary>
		/// Dismisses an alert by ID.
		/// </summary>
		public bool Dismiss(string alertId)
		{
			for (int i = 0; i < _alerts.Count; i++)
			{
				if (_alerts[i].Id == alertId)
				{
					_alerts.RemoveAt(i);
					OnAlertDismissed?.Invoke(alertId);
					OnAlertsChanged?.Invoke(_alerts.Count);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Dismisses all alerts of a specific type.
		/// </summary>
		public int DismissByType(AlertType type)
		{
			int dismissed = 0;
			for (int i = _alerts.Count - 1; i >= 0; i--)
			{
				if (_alerts[i].Type == type)
				{
					string id = _alerts[i].Id;
					_alerts.RemoveAt(i);
					OnAlertDismissed?.Invoke(id);
					dismissed++;
				}
			}

			if (dismissed > 0)
				OnAlertsChanged?.Invoke(_alerts.Count);

			return dismissed;
		}

		/// <summary>
		/// Gets alerts of a specific type.
		/// </summary>
		public List<Alert> GetByType(AlertType type)
		{
			var result = new List<Alert>();
			for (int i = 0; i < _alerts.Count; i++)
			{
				if (_alerts[i].Type == type)
					result.Add(_alerts[i]);
			}
			return result;
		}

		/// <summary>
		/// Clears all alerts.
		/// </summary>
		public void Clear()
		{
			_alerts.Clear();
			OnAlertsChanged?.Invoke(0);
		}
	}
}
