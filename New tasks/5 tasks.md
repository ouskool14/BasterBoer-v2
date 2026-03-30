## Prompt 1: World Bootstrap & System Wiring

```
# TASK: Implement World Bootstrap System

## CONTEXT
You are working on BasterBoer-v2, a farm/wildlife management simulation in Godot 4 (C# + GDScript).
The repo already contains many simulation systems (TimeSystem, AnimalSystem, WaterSystem, FenceSystem, EconomySystem, StaffSystem, WorldChunkStreamer) but they are NOT wired together at runtime.

Currently `project.godot` only autoloads WeatherSystem. The main scene (`Scenes/main.tscn`) doesn't instantiate or connect the core systems.

Your job is to create the bootstrap layer that makes the simulation actually run.

## FILES TO READ FIRST (in order)
1. `docs/ARCHITECTURE.md` - Core architectural rules (sim/render separation, tick budgets, chunk limits)
2. `GAME_VISION v0.6.md` - Design intent and pillars
3. `AGENTS.md` - Coding conventions you MUST follow
4. `project.godot` - Current autoload configuration
5. `Scenes/main.tscn` - Current scene structure
6. `TimeSystem.cs` - The heartbeat that should drive all ticks
7. `GameState.cs` - Global state container
8. `WorldChunkStreamer.cs` - Chunk streaming system

## DELIVERABLES

### 1. Create `Scripts/Core/Bootstrap.cs`
A C# autoload that:
- Registers itself as `/root/Bootstrap`
- Instantiates and registers child systems in correct order:
  1. GameState (must exist before others)
  2. TimeSystem
  3. EconomySystem
  4. WaterSystem
  5. AnimalSystem
  6. StaffSystem
  7. FenceSystem
- Exposes typed getters: `public static GameState Game => ...`
- Handles cleanup on tree exit

### 2. Create `Scripts/Core/SimulationTicker.cs`
A C# class that:
- Subscribes to TimeSystem signals (DayPassed, MonthPassed, SeasonChanged, YearPassed)
- On MonthPassed: calls heavy system updates (Economy, Animals, Staff, Water capacity changes)
- On DayPassed: calls light updates (weather effects, herd movement ticks)
- Respects tick budget from ARCHITECTURE.md (2ms for heavy monthly, spread across frames if needed)
- Emits signal when tick cycle completes

### 3. Modify `project.godot`
Add autoloads in this order:
```
Bootstrap = "*res://Scripts/Core/Bootstrap.cs"
```
(Bootstrap will instantiate others, so only one autoload needed)

### 4. Create or modify `Scenes/WorldRoot.tscn`
Scene structure:
```
WorldRoot (Node3D)
├── Player (existing player scene)
├── WorldChunkStreamer (configured with player reference)
├── AnimalRenderer (near-player rendering)
├── FenceRenderer (placeholder for now)
└── DebugOverlay (optional: shows tick timing, active chunks, system status)
```

### 5. Update `Scenes/main.tscn`
- Instance WorldRoot.tscn as child
- Remove any duplicate/orphan system nodes

## ACCEPTANCE CRITERIA
When pressing F5:
- [ ] No errors in console related to missing singletons
- [ ] `/root/Bootstrap` exists and is accessible
- [ ] `/root/Bootstrap.Game` returns valid GameState
- [ ] TimeSystem advances (visible in debug or print)
- [ ] MonthPassed signal fires and SimulationTicker responds
- [ ] WorldChunkStreamer tracks player position (debug print chunk coords)
- [ ] All systems gracefully handle being called before full initialization

## CONVENTIONS (from AGENTS.md)
- C# for all simulation logic
- GDScript only for UI/signals/thin glue
- PascalCase for C# public members
- Emit signals rather than direct coupling where possible
- Add XML doc comments to public methods
- No `Console.WriteLine` in production code; use Godot's GD.Print or a logger

## TESTING APPROACH
Create `Scripts/Debug/BootstrapTest.cs`:
- On _Ready: verify all systems accessible
- Subscribe to MonthPassed, print "Month X started"
- Subscribe to chunk streamer, print when chunks load/unload
- Can be toggled off via export bool
```

---

## Prompt 2: Water Vertical Slice

```
# TASK: Implement Water System Vertical Slice

## CONTEXT
You are working on BasterBoer-v2, a farm/wildlife management simulation in Godot 4 (C# + GDScript).
The game vision states "Water is life" - it's one of the three core pillars (Land/Ecology, Animals, Economy).

The repo has `WaterSystem.cs` with basic water point tracking and `HerdBrain.cs` already calls `WaterSystem.Instance.FindBestWater(...)`. However, water points aren't instantiated in the world, have no visual representation, and player cannot interact with them.

Your job is to create a complete vertical slice: water exists → herds need it → player can affect it → consequences emerge.

## FILES TO READ FIRST
1. `GAME_VISION v0.6.md` - Section on "Pressure Systems" and water as visceral indicator
2. `docs/ARCHITECTURE.md` - Sim/render separation rules
3. `WaterSystem.cs` - Existing water point logic
4. `HerdBrain.cs` - How herds query water (FindBestWater method)
5. `WeatherSystem.cs` - Weather states that should affect water
6. `AGENTS.md` - Coding conventions

## DELIVERABLES

### 1. Extend `WaterSystem.cs`
Add to WaterPoint struct/class:
```csharp
public enum WaterSourceType { Dam, Borehole, Trough, River }
public float Capacity;        // 0.0 to 1.0 (percentage full)
public float MaxCapacity;     // liters (for display)
public float FlowRate;        // liters per day natural replenishment
public float ConsumptionRate; // current drain from herds
public bool IsOperational;    // false = needs repair
public int RepairCost;        // currency to fix
public float QualityFactor;   // 0.0-1.0, affects animal health
```

Add methods:
```csharp
public void TickDaily(WeatherState weather)  // evaporation/rain
public void TickMonthly()                     // decay, quality degradation
public float Consume(float liters)            // returns actual amount consumed
public void Repair()                          // sets IsOperational = true, costs money
public void Refill(float liters)              // manual refill (trough only)
```

### 2. Create `Scripts/Water/WaterPointData.cs`
Pure data class for serialization:
```csharp
public class WaterPointData {
    public string Id;
    public Vector3 WorldPosition;
    public WaterSourceType Type;
    public float Capacity;
    public float MaxCapacity;
    public bool IsOperational;
    // ... other fields
}
```

### 3. Create `Scripts/Water/WaterPointRenderer.cs`
Visual representation (render-side only):
- Spawns appropriate mesh based on WaterSourceType
- Updates visual water level (shader parameter or mesh scale)
- Shows repair indicator when !IsOperational
- Only renders when within player chunk range (respect ARCHITECTURE.md)
- Emits signal when player enters interaction range

### 4. Create `Scenes/WaterPoints/Dam.tscn`, `Borehole.tscn`, `Trough.tscn`
Each scene contains:
- WaterPointRenderer script
- Appropriate 3D mesh (placeholder boxes OK for now)
- Area3D for player interaction detection
- Export vars for configuration

### 5. Create `Scripts/Water/WaterInteraction.cs`
Player interaction handler:
- Detects when player is near water point
- Shows interaction prompt (signal to UI)
- On interact:
  - If !Operational: attempt repair (check funds via EconomySystem)
  - If Trough && Capacity < 0.5: option to refill
  - Show current status (capacity, quality, flow rate)

### 6. Integrate with HerdBrain.cs
Modify existing water-seeking behavior:
- Herds prefer: high capacity > nearby > high quality
- When drinking, call `waterPoint.Consume(herdSize * drinkRate)`
- If no water available or all dry: herd stress increases, health decreases
- Emit signal "HerdThirsty" when seeking water fails

### 7. Weather Integration
In WaterSystem.TickDaily:
```csharp
foreach (var wp in waterPoints) {
    if (weather.IsRaining) 
        wp.Capacity += wp.Type == Dam ? 0.02f : 0.005f;
    if (weather.IsDrought)
        wp.Capacity -= 0.01f * weather.Temperature / 30f;
    wp.Capacity = Clamp(wp.Capacity, 0, 1);
}
```

## ACCEPTANCE CRITERIA
- [ ] At least 3 water points exist in test world (1 dam, 1 borehole, 1 trough)
- [ ] Water points have visible representation near player
- [ ] Capacity decreases when herds drink
- [ ] Capacity changes with weather (rain fills, drought drains)
- [ ] Player can interact with water point (press E or similar)
- [ ] Player can repair broken water point (deducts money)
- [ ] Herds visibly prefer water points with higher capacity
- [ ] Herd health/stress affected when no water available
- [ ] All water state survives save/load (prep for Prompt 5)

## TEST SCENARIO
1. Start game with dam at 80% capacity
2. Fast-forward time (debug key)
3. Watch herds move toward dam and drink
4. Observe capacity decrease
5. Trigger drought weather
6. Watch capacity drop faster
7. Break the dam (debug command)
8. Observe herds get stressed, seek alternatives
9. Repair dam as player
10. Herds return to normal behavior

## CONVENTIONS
- Simulation logic in C# (WaterSystem, WaterPointData)
- Rendering/interaction can use GDScript if simpler
- Emit signals for state changes (WaterLevelCritical, WaterPointBroken, etc.)
- Water points must work even if not rendered (sim runs everywhere)
```

---

## Prompt 3: Fence Loop with Consequences

```
# TASK: Implement Fence Patrol & Repair Loop

## CONTEXT
You are working on BasterBoer-v2, a farm/wildlife management simulation in Godot 4 (C# + GDScript).
The game vision identifies "patrol fences" as a core recurring task in the minute-to-minute loop.

The repo has `FenceSystem.cs` with segment/node graph logic and `FENCE_SYSTEM_INTEGRATION.md` describing the intended architecture. However, fences don't have condition/health, can't break, and player can't repair them.

Your job is to implement: fences degrade → breaches occur → consequences emerge → player patrols and repairs.

## FILES TO READ FIRST
1. `GAME_VISION v0.6.md` - "Recurring tasks" section, fence patrol
2. `FENCE_SYSTEM_INTEGRATION.md` - Integration architecture
3. `FenceSystem.cs` - Existing fence graph logic
4. `FenceRenderer.cs` - Existing rendering approach
5. `WorldChunkStreamer.cs` - Chunk system for integration
6. `docs/ARCHITECTURE.md` - Performance budgets
7. `AGENTS.md` - Coding conventions

## DELIVERABLES

### 1. Extend Fence Segment Data
In `FenceSystem.cs` or new `FenceSegmentData.cs`:
```csharp
public class FenceSegment {
    public string Id;
    public Vector3 StartPoint;
    public Vector3 EndPoint;
    public FenceType Type;           // Electric, Game, Standard
    public float Condition;          // 0.0 (destroyed) to 1.0 (perfect)
    public float MaxCondition;       // degrades over years
    public bool IsBreach;            // true if Condition < BreachThreshold
    public DateTime LastInspected;   // for patrol tracking
    public int RepairCost;           // scales with damage
    public List<string> LinkedZoneIds; // which paddocks/zones it borders
}

public enum FenceType {
    Standard,    // cheap, degrades fast, animals can push through
    Game,        // expensive, durable, keeps predators out
    Electric     // mid-cost, requires power, stuns animals
}
```

### 2. Create `Scripts/Fence/FenceHealthSystem.cs`
Manages fence degradation:
```csharp
public void TickMonthly() {
    foreach (var segment in allSegments) {
        // Natural degradation
        segment.Condition -= GetDegradationRate(segment.Type);
        
        // Weather damage
        if (currentWeather.HasStorm)
            segment.Condition -= 0.05f;
        
        // Check for breach
        if (segment.Condition < BREACH_THRESHOLD && !segment.IsBreach) {
            segment.IsBreach = true;
            EmitSignal("FenceBreached", segment.Id, segment.StartPoint);
        }
        
        // Max condition degrades over years (replacement eventually needed)
        segment.MaxCondition -= 0.001f;
    }
}
```

### 3. Create `Scripts/Fence/FenceBreachConsequences.cs`
Handles what happens when fences fail:
```csharp
// Subscribe to FenceBreached signal
public void OnFenceBreached(string segmentId, Vector3 location) {
    var segment = FenceSystem.GetSegment(segmentId);
    var zones = segment.LinkedZoneIds;
    
    // Consequence 1: Animals can escape
    foreach (var zoneId in zones) {
        var herdsInZone = AnimalSystem.GetHerdsInZone(zoneId);
        foreach (var herd in herdsInZone) {
            herd.EscapeRisk += 0.1f; // cumulative
            // Each tick, roll against EscapeRisk to lose animals
        }
    }
    
    // Consequence 2: Predator/poacher access
    IncreaseZoneThreatLevel(zones, 0.2f);
    
    // Consequence 3: Add to player alert queue
    AlertSystem.Add(AlertType.FenceBreach, location, Priority.High);
}
```

### 4. Create `Scripts/Fence/FenceInteraction.cs`
Player interaction with fences:
```csharp
// When player approaches damaged fence segment
public void OnPlayerNearSegment(FenceSegment segment) {
    if (segment.IsBreach || segment.Condition < 0.7f) {
        ShowInteractionPrompt("Repair Fence", segment.RepairCost);
    }
    
    // Always allow inspection
    ShowInspectionOption();
}

public void RepairFence(FenceSegment segment) {
    var cost = CalculateRepairCost(segment);
    if (EconomySystem.TrySpend(cost)) {
        segment.Condition = segment.MaxCondition;
        segment.IsBreach = false;
        segment.LastInspected = TimeSystem.CurrentDate;
        EmitSignal("FenceRepaired", segment.Id);
    } else {
        EmitSignal("InsufficientFunds", cost);
    }
}

public void InspectFence(FenceSegment segment) {
    segment.LastInspected = TimeSystem.CurrentDate;
    // Returns info to UI about condition, age, nearby threats
}
```

### 5. Integrate with WorldChunkStreamer
Per `FENCE_SYSTEM_INTEGRATION.md`:
```csharp
// In WorldChunkStreamer or FenceRenderer
public void OnChunkLoaded(Vector2I chunkCoord) {
    var segmentsInChunk = FenceSystem.GetSegmentsInChunk(chunkCoord);
    foreach (var segment in segmentsInChunk) {
        SpawnFenceVisual(segment);
        if (segment.IsBreach) {
            SpawnBreachIndicator(segment); // visible warning
        }
    }
}

public void OnChunkUnloaded(Vector2I chunkCoord) {
    // Remove visuals, keep simulation data
    DespawnFenceVisualsInChunk(chunkCoord);
}
```

### 6. Update FenceRenderer.cs
Add breach visualization:
- Damaged sections show visual wear (texture swap or color)
- Breached sections have gap in mesh
- Optional: particle effect at breach point
- Breach marker visible from distance (flag/icon)

### 7. Create Patrol Route Helper
```csharp
public class PatrolRouteGenerator {
    // Generate efficient route visiting all segments needing inspection
    public List<Vector3> GeneratePatrolRoute(List<FenceSegment> segments, Vector3 startPos) {
        // Prioritize: breaches > low condition > not inspected recently
        // Return waypoints for player or staff
    }
}
```

## ACCEPTANCE CRITERIA
- [ ] Fence segments have Condition value (0-1)
- [ ] Condition degrades monthly (faster for cheap fences, in storms)
- [ ] Breach occurs when Condition < threshold
- [ ] Breach has visible indicator in world
- [ ] Animals in breached zone have escape risk
- [ ] Player can inspect fence (shows condition, last inspected)
- [ ] Player can repair fence (costs money, restores condition)
- [ ] Fence visuals only load in active chunks
- [ ] Alert fires when breach occurs
- [ ] Patrol route can be generated for inspection

## TEST SCENARIO
1. Start with 10 fence segments, all at 90% condition
2. Fast-forward 6 months
3. Check that conditions have degraded
4. Trigger storm weather
5. Verify at least one breach occurs
6. Check that animals in affected zone show escape risk
7. Travel to breach location
8. Interact and repair
9. Verify breach indicator removed, condition restored
10. Verify escape risk in zone decreases

## PERFORMANCE NOTES
- Fence graph can be large (1000+ segments)
- Only visual representation loads per-chunk
- Simulation ticks ALL segments monthly (must be fast)
- Use spatial hashing for "segments in chunk" queries
- Batch condition updates, don't iterate per-frame
```

---

## Prompt 4: Minimal HUD & Event Log

```
# TASK: Implement Minimal HUD and Event Log System

## CONTEXT
You are working on BasterBoer-v2, a farm/wildlife management simulation in Godot 4 (C# + GDScript).
The game vision emphasizes "no easy mode" but also states the player needs clarity - information earned through checking and doing, not hand-holding.

Currently there is no UI layer in the game. The simulation runs but the player can't see time, money, alerts, or events.

Your job is to create a minimal, non-intrusive HUD that surfaces critical information and an event log that records what's happening in the simulation.

## FILES TO READ FIRST
1. `GAME_VISION v0.6.md` - UI philosophy, "vision board" concept, visceral indicators
2. `docs/ARCHITECTURE.md` - Separation of concerns
3. `GameState.cs` - What state needs displaying
4. `TimeSystem.cs` - Time/date signals
5. `EconomySystem.cs` - Money/transaction data
6. `AGENTS.md` - Conventions (GDScript OK for UI)

## DELIVERABLES

### 1. Create `UI/HUD/MainHUD.tscn` and `MainHUD.gd`
Minimal always-visible HUD:
```
┌─────────────────────────────────────────────────────────────┐
│ [Season Icon] 15 March Year 3    ☀️ 32°C           R 145,230│
│                                                    [3 alerts]│
└─────────────────────────────────────────────────────────────┘
```

Elements:
- **Date display**: Season icon + day + month + year
- **Time of day**: Optional subtle indicator (sun position or clock)
- **Weather**: Icon + temperature
- **Money**: Currency symbol + amount (changes color when low)
- **Alert badge**: Count of active alerts (click to expand)

GDScript structure:
```gdscript
extends CanvasLayer

@onready var date_label: Label = $DateLabel
@onready var weather_icon: TextureRect = $WeatherIcon
@onready var money_label: Label = $MoneyLabel
@onready var alert_badge: Button = $AlertBadge

func _ready():
    TimeSystem.DayPassed.connect(_on_day_passed)
    TimeSystem.SeasonChanged.connect(_on_season_changed)
    EconomySystem.BalanceChanged.connect(_on_balance_changed)
    AlertSystem.AlertAdded.connect(_on_alert_added)
    AlertSystem.AlertDismissed.connect(_on_alert_dismissed)

func _on_day_passed(day: int, month: int, year: int):
    date_label.text = "%d %s Year %d" % [day, MONTH_NAMES[month], year]

func _on_balance_changed(new_balance: float):
    money_label.text = "R %s" % format_currency(new_balance)
    money_label.modulate = Color.RED if new_balance < 10000 else Color.WHITE
```

### 2. Create `UI/HUD/AlertPanel.tscn` and `AlertPanel.gd`
Expandable alert list:
```
┌──────────────────────────────┐
│ ⚠️ ALERTS                  ✕ │
├──────────────────────────────┤
│ 🔴 Fence breach - North     →│
│ 🟡 Dam low (23%)            →│
│ 🟡 Herd 3 stressed          →│
│ 🟢 Staff wages due in 5 days │
└──────────────────────────────┘
```

Features:
- Scrollable list of active alerts
- Color-coded priority (red/yellow/green)
- Click alert to pan camera to location (emit signal)
- Dismiss button per alert
- Auto-dismiss resolved alerts

### 3. Create `Scripts/Core/AlertSystem.cs`
Central alert management:
```csharp
public class Alert {
    public string Id;
    public AlertType Type;
    public AlertPriority Priority;
    public string Message;
    public Vector3? WorldLocation;  // null if not location-based
    public DateTime CreatedAt;
    public bool AutoDismiss;        // dismiss when condition resolves
    public Func<bool> ResolveCheck; // optional condition to auto-dismiss
}

public enum AlertType {
    FenceBreach, WaterLow, WaterDry, HerdStressed, HerdSick,
    AnimalDeath, StaffIssue, PaymentDue, LoanOverdue,
    SeasonChange, DroughtWarning, EventNotification
}

public enum AlertPriority { Critical, Warning, Info }

// Signals
[Signal] public delegate void AlertAddedEventHandler(Alert alert);
[Signal] public delegate void AlertDismissedEventHandler(string alertId);
[Signal] public delegate void AlertsChangedEventHandler(int count);
```

### 4. Create `UI/EventLog/EventLog.tscn` and `EventLog.gd`
Toggleable event history panel (press Tab or button):
```
┌─────────────────────────────────────────┐
│ EVENT LOG                    [Filter ▼] │
├─────────────────────────────────────────┤
│ Year 3, Month 3                         │
│   15th: Dam "North" dropped below 50%   │
│   14th: Sold 5 cattle for R12,500       │
│   12th: Fence segment repaired (-R800)  │
│   10th: New calf born (Herd 2)          │
│ Year 3, Month 2                         │
│   28th: Drought began                   │
│   ...                                   │
└─────────────────────────────────────────┘
```

Features:
- Chronological list grouped by month
- Filter by category (Animals, Economy, Infrastructure, Weather)
- Scrollable history (keep last 100 events in memory, older to disk)
- Click event to see details or jump to location

### 5. Create `Scripts/Core/EventLogger.cs`
Records simulation events:
```csharp
public class GameEvent {
    public string Id;
    public EventCategory Category;
    public DateTime Timestamp;
    public string Summary;          // "Sold 5 cattle for R12,500"
    public string Details;          // Extended info for detail view
    public Vector3? Location;
    public Dictionary<string, object> Metadata;
}

public enum EventCategory {
    Animal, Economy, Infrastructure, Weather, Staff, System
}

public class EventLogger : Node {
    private List<GameEvent> _events = new();
    
    public void Log(EventCategory cat, string summary, Vector3? loc = null) {
        var evt = new GameEvent {
            Id = Guid.NewGuid().ToString(),
            Category = cat,
            Timestamp = TimeSystem.CurrentDateTime,
            Summary = summary,
            Location = loc
        };
        _events.Add(evt);
        EmitSignal("EventLogged", evt);
        
        // Trim old events
        if (_events.Count > MAX_MEMORY_EVENTS)
            ArchiveOldEvents();
    }
}
```

### 6. Wire Systems to Logger
Add logging calls throughout existing systems:
```csharp
// In EconomySystem
public void ProcessSale(Sale sale) {
    // ... existing logic ...
    EventLogger.Log(EventCategory.Economy, 
        $"Sold {sale.Quantity} {sale.ItemType} for R{sale.Total:N0}");
}

// In AnimalSystem
public void OnAnimalBorn(Animal animal) {
    EventLogger.Log(EventCategory.Animal,
        $"New {animal.Species} born in {animal.HerdName}",
        animal.Position);
}

// In WaterSystem
public void OnWaterLevelCritical(WaterPoint wp) {
    EventLogger.Log(EventCategory.Infrastructure,
        $"{wp.Name} dropped below 25%", wp.Position);
    AlertSystem.Add(AlertType.WaterLow, $"{wp.Name} low ({wp.Capacity:P0})", 
        wp.Position, AlertPriority.Warning);
}
```

### 7. Create `UI/Interaction/InteractionPrompt.tscn`
Simple contextual prompt when near interactable:
```
        ┌─────────────────┐
        │ [E] Repair Fence│
        │     Cost: R800  │
        └─────────────────┘
              ▲
           [Player]
```

- Appears above interactable objects
- Shows key binding + action + cost/info
- Fades in/out smoothly
- Controlled by interaction system signals

## ACCEPTANCE CRITERIA
- [ ] HUD displays date, weather, money at all times
- [ ] HUD updates reactively (signals, not polling)
- [ ] Alert badge shows count, expands to list on click
- [ ] Alerts color-coded by priority
- [ ] Clicking alert location pans camera (or marks on minimap)
- [ ] Event log accessible via keypress (Tab)
- [ ] Event log shows last 50+ events grouped by month
- [ ] Event log filterable by category
- [ ] All major simulation events logged automatically
- [ ] Interaction prompts appear near interactables
- [ ] UI scales appropriately (test at 1080p and 1440p)
- [ ] UI does not block gameplay view excessively

## STYLE GUIDELINES
- Minimal, functional aesthetic (think: management sim, not RPG)
- Semi-transparent backgrounds
- Readable fonts (14-16px minimum)
- Consistent color language:
  - Red: critical/danger/loss
  - Yellow: warning/attention
  - Green: positive/gain/resolved
  - Blue: information/neutral
- No animations that delay information access
- Keyboard navigable where possible

## CONVENTIONS
- UI scenes in `UI/` folder
- UI scripts in GDScript (per AGENTS.md)
- Connect to C# systems via signals
- Use Godot's theme system for consistent styling
- Export configuration vars for colors/sizes
```

---

## Prompt 5: Save/Load System

```
# TASK: Implement JSON Save/Load System

## CONTEXT
You are working on BasterBoer-v2, a farm/wildlife management simulation in Godot 4 (C# + GDScript).
The game vision specifies "JSON-based, full state serialisation" for save/load. The simulation has multiple interdependent systems that need to be persisted and restored correctly.

Your job is to implement a versioned, extensible save/load system that captures complete world state.

## FILES TO READ FIRST
1. `GAME_VISION v0.6.md` - Save system requirements
2. `docs/ARCHITECTURE.md` - System dependencies
3. `GameState.cs` - Global state to persist
4. `TimeSystem.cs` - Time state
5. `EconomySystem.cs` - Financial state
6. `AnimalSystem.cs` - Herd/animal state
7. `WaterSystem.cs` - Water point state
8. `FenceSystem.cs` - Fence graph state
9. `StaffSystem.cs` - Staff state
10. `AGENTS.md` - Coding conventions

## DELIVERABLES

### 1. Create `Scripts/Persistence/SaveData.cs`
Root save data structure:
```csharp
[Serializable]
public class SaveData {
    // Metadata
    public int SaveVersion = 1;
    public string SaveId;
    public DateTime SavedAt;
    public string GameVersion;
    
    // World generation
    public int WorldSeed;
    public Vector2I MapSize;
    
    // Time state
    public TimeData Time;
    
    // Core systems
    public EconomyData Economy;
    public List<HerdData> Herds;
    public List<WaterPointData> WaterPoints;
    public FenceGraphData Fences;
    public List<StaffData> Staff;
    public WeatherData Weather;
    
    // Event history (recent only)
    public List<GameEventData> RecentEvents;
    
    // Player state
    public PlayerData Player;
    
    // Settings that affect gameplay
    public DifficultySettings Difficulty;
}
```

### 2. Create System-Specific Data Classes
Each in `Scripts/Persistence/Data/`:

```csharp
// TimeData.cs
[Serializable]
public class TimeData {
    public int Day;
    public int Month;
    public int Year;
    public float TimeOfDay;  // 0-24
    public string CurrentSeason;
}

// EconomyData.cs
[Serializable]
public class EconomyData {
    public float Balance;
    public List<LoanData> ActiveLoans;
    public List<TransactionData> RecentTransactions;
    public float TotalRevenue;
    public float TotalExpenses;
    public Dictionary<string, float> AssetValues;
}

// HerdData.cs
[Serializable]
public class HerdData {
    public string Id;
    public string Species;
    public Vector3 CenterPosition;
    public int Population;
    public float AverageHealth;
    public float AverageAge;
    public float StressLevel;
    public float ThirstLevel;
    public string CurrentZoneId;
    public GeneticsData Genetics;  // simplified genetic averages
    public List<AnimalData> IndividualAnimals;  // optional detailed mode
}

// WaterPointData.cs (from Prompt 2)
[Serializable]  
public class WaterPointData {
    public string Id;
    public string Name;
    public Vector3 Position;
    public string Type;  // Dam, Borehole, Trough
    public float Capacity;
    public float MaxCapacity;
    public bool IsOperational;
    public float Quality;
}

// FenceGraphData.cs
[Serializable]
public class FenceGraphData {
    public List<FenceNodeData> Nodes;
    public List<FenceSegmentData> Segments;
    public List<ZoneData> Zones;
}

// StaffData.cs
[Serializable]
public class StaffData {
    public string Id;
    public string Name;
    public string Role;
    public float Skill;
    public float Morale;
    public float Salary;
    public string AssignedZoneId;
    public List<string> CurrentTasks;
}

// PlayerData.cs
[Serializable]
public class PlayerData {
    public Vector3 Position;
    public Vector3 Rotation;
    public List<string> UnlockedFeatures;
    public Dictionary<string, int> Statistics;
}
```

### 3. Create `Scripts/Persistence/SaveManager.cs`
Central save/load orchestrator:
```csharp
public class SaveManager : Node {
    private const string SAVE_DIR = "user://saves/";
    private const int CURRENT_VERSION = 1;
    
    [Signal] public delegate void SaveStartedEventHandler();
    [Signal] public delegate void SaveCompletedEventHandler(string path);
    [Signal] public delegate void SaveFailedEventHandler(string error);
    [Signal] public delegate void LoadStartedEventHandler();
    [Signal] public delegate void LoadCompletedEventHandler();
    [Signal] public delegate void LoadFailedEventHandler(string error);
    
    public async Task<bool> SaveGame(string slotName) {
        EmitSignal("SaveStarted");
        
        try {
            var data = new SaveData {
                SaveId = Guid.NewGuid().ToString(),
                SavedAt = DateTime.UtcNow,
                SaveVersion = CURRENT_VERSION,
                GameVersion = GetGameVersion(),
                
                WorldSeed = GameState.WorldSeed,
                MapSize = GameState.MapSize,
                
                Time = TimeSystem.Instance.ExportData(),
                Economy = EconomySystem.Instance.ExportData(),
                Herds = AnimalSystem.Instance.ExportHerds(),
                WaterPoints = WaterSystem.Instance.ExportData(),
                Fences = FenceSystem.Instance.ExportData(),
                Staff = StaffSystem.Instance.ExportData(),
                Weather = WeatherSystem.Instance.ExportData(),
                
                Player = ExportPlayerData(),
                RecentEvents = EventLogger.Instance.ExportRecent(100)
            };
            
            var json = JsonSerializer.Serialize(data, GetJsonOptions());
            var path = $"{SAVE_DIR}{slotName}.json";
            
            // Ensure directory exists
            DirAccess.MakeDirRecursiveAbsolute(SAVE_DIR);
            
            // Write with backup
            await WriteWithBackup(path, json);
            
            EmitSignal("SaveCompleted", path);
            return true;
        }
        catch (Exception ex) {
            GD.PrintErr($"Save failed: {ex.Message}");
            EmitSignal("SaveFailed", ex.Message);
            return false;
        }
    }
    
    public async Task<bool> LoadGame(string slotName) {
        EmitSignal("LoadStarted");
        
        try {
            var path = $"{SAVE_DIR}{slotName}.json";
            
            if (!FileAccess.FileExists(path)) {
                EmitSignal("LoadFailed", "Save file not found");
                return false;
            }
            
            var json = FileAccess.GetFileAsString(path);
            var data = JsonSerializer.Deserialize<SaveData>(json, GetJsonOptions());
            
            // Version migration
            data = MigrateIfNeeded(data);
            
            // Restore in dependency order
            GameState.Initialize(data.WorldSeed, data.MapSize);
            TimeSystem.Instance.ImportData(data.Time);
            EconomySystem.Instance.ImportData(data.Economy);
            WaterSystem.Instance.ImportData(data.WaterPoints);
            FenceSystem.Instance.ImportData(data.Fences);
            StaffSystem.Instance.ImportData(data.Staff);
            AnimalSystem.Instance.ImportHerds(data.Herds);
            WeatherSystem.Instance.ImportData(data.Weather);
            
            RestorePlayerData(data.Player);
            EventLogger.Instance.ImportRecent(data.RecentEvents);
            
            EmitSignal("LoadCompleted");
            return true;
        }
        catch (Exception ex) {
            GD.PrintErr($"Load failed: {ex.Message}");
            EmitSignal("LoadFailed", ex.Message);
            return false;
        }
    }
    
    private SaveData MigrateIfNeeded(SaveData data) {
        if (data.SaveVersion == CURRENT_VERSION)
            return data;
            
        // Apply migrations in order
        if (data.SaveVersion < 2) {
            data = MigrateV1ToV2(data);
        }
        // Future migrations here
        
        return data;
    }
    
    public List<SaveSlotInfo> GetSaveSlots() {
        var slots = new List<SaveSlotInfo>();
        var dir = DirAccess.Open(SAVE_DIR);
        
        if (dir == null) return slots;
        
        dir.ListDirBegin();
        string fileName;
        while ((fileName = dir.GetNext()) != "") {
            if (fileName.EndsWith(".json")) {
                var info = ReadSlotInfo($"{SAVE_DIR}{fileName}");
                if (info != null) slots.Add(info);
            }
        }
        
        return slots.OrderByDescending(s => s.SavedAt).ToList();
    }
}
```

### 4. Add Export/Import Methods to Each System
Example pattern for each system:
```csharp
// In TimeSystem.cs
public TimeData ExportData() {
    return new TimeData {
        Day = _currentDay,
        Month = _currentMonth,
        Year = _currentYear,
        TimeOfDay = _timeOfDay,
        CurrentSeason = _currentSeason.ToString()
    };
}

public void ImportData(TimeData data) {
    _currentDay = data.Day;
    _currentMonth = data.Month;
    _currentYear = data.Year;
    _timeOfDay = data.TimeOfDay;
    _currentSeason = Enum.Parse<Season>(data.CurrentSeason);
    
    // Emit signals to update dependents
    EmitSignal("TimeRestored");
}
```

### 5. Create `UI/SaveLoad/SaveLoadMenu.tscn` and `SaveLoadMenu.gd`
Simple save/load UI:
```
┌─────────────────────────────────────────┐
│ SAVE / LOAD                          ✕  │
├─────────────────────────────────────────┤
│ [Tab: Save] [Tab: Load]                 │
├─────────────────────────────────────────┤
│ ┌─────────────────────────────────────┐ │
│ │ Slot 1: "Farm Alpha"                │ │
│ │ Year 5, Month 8 - R234,500          │ │
│ │ Saved: 2026-03-15 14:32             │ │
│ └─────────────────────────────────────┘ │
│ ┌─────────────────────────────────────┐ │
│ │ Slot 2: "Drought Recovery"          │ │
│ │ Year 3, Month 2 - R12,100           │ │
│ │ Saved: 2026-03-10 09:15             │ │
│ └─────────────────────────────────────┘ │
│ ┌─────────────────────────────────────┐ │
│ │ + New Save                          │ │
│ └─────────────────────────────────────┘ │
├─────────────────────────────────────────┤
│              [Cancel]  [Confirm]        │
└─────────────────────────────────────────┘
```

Features:
- Tab to switch Save/Load mode
- List existing saves with preview info
- "New Save" option with name input
- Confirm before overwrite
- Delete save option (with confirm)
- Loading shows progress/status

### 6. Create `Scripts/Persistence/AutoSave.cs`
Automatic save system:
```csharp
public class AutoSave : Node {
    [Export] public int AutoSaveIntervalMinutes = 10;
    [Export] public int MaxAutoSaves = 3;
    
    private float _timeSinceLastSave = 0;
    
    public override void _Process(double delta) {
        _timeSinceLastSave += (float)delta;
        
        if (_timeSinceLastSave >= AutoSaveIntervalMinutes * 60) {
            PerformAutoSave();
            _timeSinceLastSave = 0;
        }
    }
    
    private async void PerformAutoSave() {
        var slotName = $"autosave_{DateTime.Now:yyyyMMdd_HHmmss}";
        await SaveManager.Instance.SaveGame(slotName);
        
        // Cleanup old autosaves
        CleanupOldAutoSaves();
    }
    
    private void CleanupOldAutoSaves() {
        var autoSaves = SaveManager.Instance.GetSaveSlots()
            .Where(s => s.SlotName.StartsWith("autosave_"))
            .OrderByDescending(s => s.SavedAt)
            .ToList();
            
        foreach (var old in autoSaves.Skip(MaxAutoSaves)) {
            SaveManager.Instance.DeleteSave(old.SlotName);
        }
    }
}
```

## ACCEPTANCE CRITERIA
- [ ] Save creates valid JSON file in user://saves/
- [ ] Save file contains all system states
- [ ] Load fully restores game to saved state
- [ ] Time continues correctly after load
- [ ] Economy balance matches saved value
- [ ] All herds restored with correct positions/stats
- [ ] Water points restored with correct levels
- [ ] Fence conditions restored
- [ ] Staff assignments restored
- [ ] Player position restored
- [ ] Version number included in save file
- [ ] Old saves can be migrated (migration framework exists)
- [ ] Save/Load UI allows slot management
- [ ] Autosave works on interval
- [ ] Autosave rotates (keeps last N)
- [ ] Load fails gracefully with corrupted file
- [ ] Save/Load shows progress feedback

## TESTING CHECKLIST
1. New game → play 5 minutes → save → quit → load → verify state
2. Advance 6 months → save → load → verify date
3. Spend money → save → load → verify balance
4. Move herds → save → load → verify positions
5. Damage fence → save → load → verify condition
6. Corrupt save file → attempt load → verify graceful failure
7. Create save v1 → upgrade game to v2 → load → verify migration
8. Let autosave trigger → verify file created
9. Trigger 5 autosaves → verify only last 3 kept

## CONVENTIONS
- All data classes in `Scripts/Persistence/Data/`
- Use System.Text.Json (not Newtonsoft) for .NET compatibility
- Indent JSON for readability (development); minify optional for release
- Include migration code even if not needed yet (future-proofing)
- Emit signals for UI feedback, don't couple to UI directly
- Handle async properly (don't block main thread on file I/O)
```

---

## Usage Notes

Each prompt is designed to be:
1. **Self-contained** - Includes all context needed
2. **Specific** - References exact files and patterns from your repo
3. **Testable** - Clear acceptance criteria
4. **Aligned** - Follows your AGENTS.md conventions

**Recommended order:** 1 → 2 → 3 → 4 → 5 (each builds on the previous)

You can paste these directly into separate Kilo Code agent sessions. If an agent needs clarification, the prompts reference specific files it can read for more context.