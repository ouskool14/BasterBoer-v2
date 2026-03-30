using Godot;
using System;
using System.Collections.Generic;
using BasterBoer.Core.Time;

namespace BasterBoer.Core
{
	/// <summary>
	/// Event categories for filtering the event log.
	/// </summary>
	public enum EventCategory
	{
		Animal, Economy, Infrastructure, Weather, Staff, System
	}

	/// <summary>
	/// A single logged game event.
	/// </summary>
	public class GameEvent
	{
		/// <summary>Unique identifier.</summary>
		public string Id;

		/// <summary>Event category for filtering.</summary>
		public EventCategory Category;

		/// <summary>In-game timestamp.</summary>
		public GameDate Timestamp;

		/// <summary>Human-readable summary (e.g., "Sold 5 cattle for R12,500").</summary>
		public string Summary;

		/// <summary>Extended detail text.</summary>
		public string Details;

		/// <summary>Optional world location.</summary>
		public Vector3? Location;

		/// <summary>Additional metadata key-value pairs.</summary>
		public Dictionary<string, object> Metadata;
	}

	/// <summary>
	/// Records simulation events for the player's event log.
	/// Keeps last N events in memory. Older events can be archived to disk.
	/// Pure C# singleton — UI subscribes via events.
	/// </summary>
	public class EventLogger
	{
		private static EventLogger _instance;
		public static EventLogger Instance => _instance ??= new EventLogger();

		private readonly List<GameEvent> _events = new(512);
		private const int MaxMemoryEvents = 200;
		private int _nextId;

		/// <summary>Emitted when a new event is logged.</summary>
		public event Action<GameEvent> OnEventLogged;

		private EventLogger() { }

		/// <summary>Read-only access to all events (newest last).</summary>
		public IReadOnlyList<GameEvent> Events => _events;

		/// <summary>Current number of stored events.</summary>
		public int Count => _events.Count;

		/// <summary>
		/// Logs a new game event.
		/// </summary>
		/// <param name="category">Event category</param>
		/// <param name="summary">Short description</param>
		/// <param name="location">Optional world position</param>
		/// <param name="details">Optional extended details</param>
		public GameEvent Log(EventCategory category, string summary,
			Vector3? location = null, string details = null)
		{
			var evt = new GameEvent
			{
				Id = $"evt_{_nextId++}",
				Category = category,
				Timestamp = Bootstrap.Time?.CurrentDate ?? new GameDate(2024, 1, 1),
				Summary = summary,
				Details = details,
				Location = location,
				Metadata = new Dictionary<string, object>()
			};

			_events.Add(evt);
			OnEventLogged?.Invoke(evt);

			// Trim old events if exceeding limit
			if (_events.Count > MaxMemoryEvents)
			{
				_events.RemoveRange(0, _events.Count - MaxMemoryEvents / 2);
			}

			return evt;
		}

		/// <summary>
		/// Gets events filtered by category.
		/// </summary>
		public List<GameEvent> GetByCategory(EventCategory category)
		{
			var result = new List<GameEvent>();
			for (int i = _events.Count - 1; i >= 0; i--)
			{
				if (_events[i].Category == category)
					result.Add(_events[i]);
			}
			return result;
		}

		/// <summary>
		/// Gets events grouped by month (most recent first).
		/// </summary>
		public List<(GameDate Month, List<GameEvent> Events)> GetGroupedByMonth(int maxMonths = 12)
		{
			var groups = new List<(GameDate, List<GameEvent>)>();
			GameDate currentMonth = default;
			List<GameEvent> currentGroup = null;

			for (int i = _events.Count - 1; i >= 0; i--)
			{
				var evt = _events[i];
				var evtMonth = new GameDate(evt.Timestamp.Year, evt.Timestamp.Month, 1);

				if (currentGroup == null || !evtMonth.Equals(currentMonth))
				{
					if (currentGroup != null && groups.Count >= maxMonths)
						break;

					currentMonth = evtMonth;
					currentGroup = new List<GameEvent>();
					groups.Add((currentMonth, currentGroup));
				}

				currentGroup.Add(evt);
			}

			return groups;
		}

		/// <summary>
		/// Exports recent events for save system.
		/// </summary>
		public List<GameEvent> ExportRecent(int count)
		{
			int start = Math.Max(0, _events.Count - count);
			return _events.GetRange(start, _events.Count - start);
		}

		/// <summary>
		/// Imports events from save data.
		/// </summary>
		public void ImportRecent(List<GameEvent> events)
		{
			if (events == null) return;
			_events.Clear();
			_events.AddRange(events);
		}

		/// <summary>
		/// Clears all events.
		/// </summary>
		public void Clear()
		{
			_events.Clear();
		}
	}
}
