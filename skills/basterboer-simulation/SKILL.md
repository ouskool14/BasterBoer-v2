# SKILL: BasterBoer Simulation Architecture

## When to use this skill
Read this file before designing or writing ANY new system, class, or feature in
BasterBoer. This defines how the game world works at an architectural level.
Violating these rules produces code that cannot be integrated without a rewrite.

---

## 1. The Fundamental Law — Simulate vs Render

> Everything in the world is simulated at all times.
> Only what is near the player is rendered.

This is not a guideline. It is the load-bearing wall of the entire architecture.
Every system you write must respect this split.

| Layer | What it is | Technology | Runs |
|-------|-----------|------------|------|
| **Simulation** | C# data objects — herds, flora, economy, fire, water | Pure C# classes and structs | Always, every tick |
| **Render** | Scene nodes — meshes, particles, audio | Godot nodes + MultiMeshInstance3D | Proximity only, freed on unload |

A buffalo herd 3km from the player is **real** — it grazes, breeds, ages, and
can be poached. It has zero scene nodes. It is a C# object. When the player
drives within 150m, the render layer reads the herd data and spawns meshes.
When the player leaves, those meshes are freed. The C# data object continues
running undisturbed.

**The test:** Can your new class be instantiated without Godot running?
If yes, it belongs in the simulation layer. If no, examine why.

---

## 2. System Ownership Map

Every piece of data has exactly one owner. No system reads another system's
internal data directly — all cross-system communication goes through `GameState`
or signals.

```
GameState (singleton)
├── AnimalSystem          — herd brains, individual animal data, population
├── BreedingSystem        — genetics, conception, gestation, birth
├── FloraSystem           — grass coverage, tree health, invasive species
├── WaterSystem           — river levels, dam capacity, borehole yield
├── FireSystem            — active fire events, spread rate, affected zones
├── EconomySystem         — ZAR balance, revenue streams, expenses
├── StaffSystem           — staff members, skills, morale, wages
├── WeatherSystem         — drought state, rainfall, temperature, season
├── EcologySystem         — biodiversity score, ambient life densities
└── TimeSystem            — game clock, tick dispatch, season tracking
```

The **render layer** (WorldChunkStreamer + chunk nodes) reads from these systems
but never writes to them.

---

## 3. The TimeSystem Tick — Where Simulation Lives

Simulation logic runs on ticks, NEVER in `_Process()`.

```csharp
// TimeSystem dispatches ticks. Systems subscribe.
public enum TickType { Hourly, Daily, Weekly, Monthly }

// In AnimalSystem:
public void OnDailyTick() {
    foreach (var herd in _herds) {
        herd.Brain.UpdateHunger(deltaDay);
        herd.Brain.UpdateThirst(deltaDay);
        herd.Brain.EvaluateStateChange();
    }
}

public void OnMonthlyTick() {
    RunBreedingCheck();
    UpdatePopulationAge();
    ApplyMortality();
}
```

**Rule:** If your logic runs more than once per second, question why.
If it runs every frame, it belongs in the render layer as a visual update,
not in simulation.

---

## 4. The Herd Instinct Model

### HerdBrain (one C# class per herd — the decision maker)

```csharp
public class HerdBrain {
    // Drives all animals in this herd
    public HerdState CurrentState;
    public float Thirst;      // 0-1, rises over time
    public float Hunger;      // 0-1, rises when grass is low
    public float Fatigue;     // 0-1, rises with movement and heat
    public float FearLevel;   // 0-1, spikes on predator detection
    public Vector3 HerdCentre; // world position of herd centre of mass

    public void EvaluateStateChange() {
        // ONE decision. All animals execute it.
        if (FearLevel > 0.7f) { CurrentState = HerdState.Fleeing; return; }
        if (Thirst > 0.8f)    { CurrentState = HerdState.MovingToWater; return; }
        if (Hunger > 0.7f)    { CurrentState = HerdState.MovingToGraze; return; }
        if (Fatigue > 0.8f)   { CurrentState = HerdState.Resting; return; }
        CurrentState = HerdState.Grazing;
    }
}

public enum HerdState {
    Grazing, MovingToWater, MovingToGraze, Resting, Fleeing, Alerting, Breeding
}
```

### AnimalStruct (per-animal data — value type, never makes decisions)

```csharp
public readonly struct AnimalStruct {
    public readonly int Id;
    public readonly float Age;        // years
    public readonly Sex Sex;
    public readonly float Health;     // 0-1
    public readonly GeneticProfile Genetics;
    public readonly Vector3 OffsetFromCentre; // individual position within herd
    public readonly float ReactionLag;        // seconds of delay before responding to herd state change
}
```

### The Ripple Effect (reaction lag)

When a herd bolts, animals don't all move at frame 0.
Each animal has a small random `ReactionLag` (0.0–0.8s based on position in herd
and individual sensitivity). This produces the visual of a signal rippling through
the group — biologically accurate, computationally free.

```csharp
// In render layer only — visual position update
float effectiveReactionTime = Time.GetTicksMsec() / 1000f - animal.ReactionLag;
bool animalHasReacted = effectiveReactionTime > herdBrain.LastStateChangeTime;
```

---

## 5. LOD Ladder — Behaviour and Visual

### Behaviour LOD (AnimalSystem responsibility)

| Distance from Player | Tick Rate |
|----------------------|-----------|
| 0 – 150m | Full — every TimeSystem hourly tick |
| 150 – 500m | Reduced — state changes only, no per-tick awareness |
| 500m – 2km | Minimal — position updated once per in-game day |
| 2km+ | Simulation only — monthly tick, no world position |

### Visual LOD (Render layer responsibility)

| Distance | Render Method |
|----------|---------------|
| 0 – 100m | Full mesh + full skeletal animation |
| 100 – 350m | Full mesh + simplified animation |
| 350 – 800m | Billboard sprite facing camera |
| 800m+ | Not rendered |

These distances are tuned for the performance budget. Do not increase them
without profiling.

---

## 6. Chunk System

### WorldChunkStreamer owns all visual content
- Active chunks: 3×3 grid around player (9 chunks max)
- Chunk size: 256 world units
- Each chunk owns: terrain tiles, MultiMeshInstance3D nodes for trees/grass/rocks
- Chunks do NOT own herds (herds cross chunk boundaries)
- Buildings ARE chunk-owned — loaded/freed with chunk

### ChunkCoord
```csharp
// From FenceSystem.cs — this is the established pattern
public struct ChunkCoord {
    public int X, Z;
    public static ChunkCoord FromWorldPosition(Vector3 pos, float chunkSize) {
        return new ChunkCoord {
            X = Mathf.FloorToInt(pos.X / chunkSize),
            Z = Mathf.FloorToInt(pos.Z / chunkSize)
        };
    }
}
```

### Chunk lifecycle
```csharp
// When chunk loads:
// 1. Read C# data from FloraSystem, BuildingData etc.
// 2. Populate MultiMesh instances from that data
// 3. Register with FenceSystem, WaterSystem for visual updates

// When chunk unloads:
// 1. Free all scene nodes
// 2. C# simulation data remains alive and continues ticking
// 3. Deregister from visual update systems
```

---

## 7. Water System — Shader, Not Geometry

Rivers and dams are NOT separate meshes. They are shader parameters on existing
terrain geometry. Adding new mesh geometry for water violates the draw call budget.

```csharp
// WaterSystem holds the data:
public class RiverSegment {
    public float WaterLevel; // 0-1, 0 = dry cracked clay, 1 = full bank
}

public class Dam {
    public float Capacity; // 0-1
}

// Render layer applies it to the terrain shader:
terrainMesh.SetShaderParameter("river_level", segment.WaterLevel);
terrainMesh.SetShaderParameter("dam_capacity", dam.Capacity);
// No new geometry. No additional draw calls.
```

---

## 8. Fire System

```csharp
public class FireEvent {
    public Vector3 Origin;
    public float SpreadRadius;     // grows over time
    public float SpreadRatePerHour; // modified by wind, fuel load, drought state
    public HashSet<string> AffectedZoneIds;
    public bool IsControlledBurn;
    public bool IsContained;
}
```

Fire render: particle systems + terrain colour overlay near player.
Fire simulation: C# data object updated on hourly tick.
One fire event = 1–2 draw calls regardless of fire size (particle system).

---

## 9. GameState — The Data Bus

All simulation systems read/write through GameState.
Never cache a reference to another system directly — always go through GameState.

```csharp
// WRONG
private AnimalSystem _animalSystem; // direct reference to another system

// CORRECT
var herdsNearWater = GameState.Instance.AnimalSystem.GetHerdsNear(waterPos, 200f);
```

GameState is the single source of truth. If two systems need to share data,
that data lives in GameState, not in either system.

---

## 10. Adding a New System — Checklist

Before writing a new system, verify:

- [ ] Does it separate simulation data (C# class) from render (Godot nodes)?
- [ ] Does it tick via TimeSystem, not `_Process()`?
- [ ] Does it register with GameState as its owner?
- [ ] Does it use structs for per-entity data (value types, cache-friendly)?
- [ ] Does its render output use MultiMeshInstance3D, not individual nodes?
- [ ] Does it have a LOD strategy (what happens at 500m, at 2km)?
- [ ] Does it stay within the performance budget? (See performance SKILL.md)
- [ ] Is it chunk-aware if it has spatial data?
- [ ] Does it have zero Godot API imports in the simulation class?
