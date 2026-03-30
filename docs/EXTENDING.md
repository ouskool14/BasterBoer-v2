# BasterBoer — Extending the Game

> This document is for AI agents and contributors adding new features. Read it before writing code.
> For the system map and technical rules, see [ARCHITECTURE.md](ARCHITECTURE.md).
> For the full design intent behind every decision, see [GAME_VISION v0.6.md](../GAME_VISION%20v0.6.md).

---

## The Cardinal Rule

> **Extend, don't rewrite.**

If a system already exists, add to it. Do not replace its core logic unless explicitly instructed and the reason is stated. The most common AI failure mode on this project is replacing working, carefully optimised systems with simpler but performance-breaking alternatives (individual animal AI is the primary example — see below).

---

## Protected Systems — Do Not Rewrite Core Logic

These systems have deliberate architectural decisions that must not be reversed:

### HerdBrain.cs
**What it does:** One brain per herd. Makes all behavioural decisions. Individual animals follow with variation.

**What NOT to do:**
- ❌ Do not give `AnimalStruct` members their own state machines
- ❌ Do not add per-frame pathfinding per animal
- ❌ Do not replace the `SpeciesConfig` struct with a class hierarchy per species
- ❌ Do not call `_rng.NextDouble()` inside the per-animal loop in `UpdateIndividualAnimals` — variation must be timer-based (see existing pattern)
- ❌ Do not add `GetNode()` calls anywhere inside HerdBrain — it is a pure C# class with no scene node dependency

**How to extend it safely:**
- Add new `HerdState` enum values and handle them in `EvaluateStateTransitions()`, `ExecuteCurrentState()`, and `GetSpreadRadiusForState()`
- Add new fields to `SpeciesConfig` to drive new per-species behaviour
- Add new public methods following the pattern of `AlertToThreat()` for external system calls
- Add new need variables (e.g. `SaltLick`, `ShelterNeed`) following the pattern of `Thirst`/`Hunger`/`Fatigue`

### AnimalSystem.cs
**What it does:** Singleton manager for all herds. Drives per-frame ticks with LOD awareness. Handles monthly cleanup.

**What NOT to do:**
- ❌ Do not move simulation logic into `_Process()` — simulation belongs in TimeSystem tick callbacks
- ❌ Do not instantiate scene nodes from inside AnimalSystem
- ❌ Do not add LINQ in hot paths (the per-herd tick loop)

**How to extend it safely:**
- Register new herd types via `HerdFactory`
- Add new query methods (e.g. `GetHerdsOfSpecies()`, `GetHerdsInCamp()`) following the pattern of `GetHerdsInRadius()`
- New monthly logic goes in `OnMonthlyTick()`, never in `_Process()`

### WorldChunkStreamer.cs
**What it does:** Manages the 3×3 active chunk grid around the player. Loads/frees scene nodes as chunks enter and leave range.

**What NOT to do:**
- ❌ Do not generate chunks on the main thread — background thread only
- ❌ Do not give herds chunk ownership — herds cross chunk boundaries and are proximity-managed separately
- ❌ Do not expand beyond 9 active chunks without profiling first

**How to extend it safely:**
- New chunk-owned content (e.g. building ruins, mineral deposits) is added to the chunk's scene node on load and freed on unload — the underlying data lives in `GameState` or a C# data system, never in the chunk node itself

### AnimalRenderer.cs / MultiMesh pipeline
**What it does:** Reads herd position data and populates `MultiMeshInstance3D` instances for visible herds.

**What NOT to do:**
- ❌ Do not create unique mesh instances per animal
- ❌ Do not use `MeshInstance3D` for anything that appears more than once in the world
- ❌ Do not apply visual variety through separate meshes — use per-instance shader parameters

**How to extend it safely:**
- Add new visual states (wound marks, pregnancy belly) as shader parameters on the existing animal mesh
- Add new species by creating a `MultiMeshInstance3D` entry following the existing pattern

---

## How to Add a New Animal Species

1. Add the species to the `Species` enum in `AnimalStruct.cs`
2. Add a `SpeciesConfig` entry in `HerdFactory.cs` with correct biological values (speed, awareness radius, drink frequency, etc.)
3. Add a GLB mesh asset to `Assets/` (target: 400–1,200 triangles, one mesh, no unique textures)
4. Register the species in `AnimalRenderer` so it gets a `MultiMeshInstance3D` slot
5. Add species-specific economic data in `EconomySystem` (live sale value, trophy value, quota)
6. Do not create a subclass of `HerdBrain` for the new species — use `SpeciesConfig` to drive behaviour variation

---

## How to Add a New Economy Revenue Stream

1. Add a new revenue category to `EconomySystem.cs` following the existing pattern (monthly tick → calculate → `GameState.Balance += amount`)
2. Add relevant permit/compliance checks against `GameState` data
3. Add a `Transaction` record with the correct category for the ledger
4. Wire seasonal modifiers through `SeasonalEventCalendar` if the stream has seasonal variation
5. Do not add UI directly to EconomySystem — raise a signal and let the HUD layer respond

---

## How to Add a New Building Type

1. Define a `BuildingData` C# class entry with type, condition, chunk ID, and world position
2. Register the building in `GameState`'s building list on placement
3. The chunk streamer will handle spawning/freeing the scene node based on proximity
4. Building meshes must use instanced components where the same piece appears on multiple buildings (walls, roof sections, pillars)
5. Do not place buildings by direct scene tree instantiation at runtime outside the chunk load path

---

## How to Add a New Game System

If you are adding a system that does not exist yet (e.g. `FireSystem`, `StaffSystem`, `WeatherSystem`):

1. The system must be a C# singleton with an `Instance` property
2. Simulation data lives in the C# class — no scene nodes, no GDScript
3. The system subscribes to `TimeSystem` signals (`OnDailyTick`, `OnMonthlyTick`) — it does not run logic in `_Process()`
4. The system communicates with other systems through `GameState` or direct singleton calls — not through Godot signals between C# singletons (signals are for C# → GDScript communication)
5. Any visual representation of the system's state is handled by a separate render/HUD layer, never by the system itself
6. Add the system to the dependency table in `ARCHITECTURE.md` before writing code

---

## Namespace Rules

| Code type | Namespace |
|---|---|
| Simulation systems (HerdBrain, AnimalSystem, FloraSystem, etc.) | `LandManagementSim.Simulation` |
| Core data and singletons (GameState, WaterSystem) | `BasterBoer.Core` or `BasterBoer.Core.Systems` |
| World streaming (WorldChunkStreamer, ChunkCoord) | `WorldStreaming` |
| GDScript | No namespace |

Do not introduce a new namespace without flagging it. The current three-namespace split already exists — work within it.

---

## Performance Checklist Before Submitting Any Code

- [ ] No `GetNode()` calls inside loops or per-frame methods
- [ ] No `new` allocations inside `_Process()` or per-animal loops
- [ ] No LINQ in hot paths
- [ ] No per-frame RNG where timer-based variation would work
- [ ] Any new object appearing more than once uses `MultiMeshInstance3D`
- [ ] Any new simulation logic runs on a TimeSystem tick, not `_Process()`
- [ ] New scene nodes are only ever instantiated by the render/chunk layer, not simulation systems
- [ ] Monthly simulation tick still completes in < 20ms total (test with Godot profiler)

---

## What "Extend" Looks Like vs What "Rewrite" Looks Like

**Extending — correct:**
```csharp
// Adding salt lick need to existing HerdBrain needs system
public float SaltNeed { get; private set; }

// In UpdateNeeds():
SaltNeed = Math.Clamp(SaltNeed + deltaHours * 0.01f, 0f, 1f);

// In EvaluateStateTransitions():
if (SaltNeed > 0.8f && CurrentState == HerdState.Grazing)
{
    TransitionToState(HerdState.Moving);
    SetSaltLickTarget();
}
```

**Rewriting — wrong:**
```csharp
// Creating per-animal decision logic
for (int i = 0; i < Animals.Length; i++)
{
    if (Animals[i].Thirst > 0.5f)
        Animals[i].TargetPosition = FindNearestWater(Animals[i].WorldPosition); // ❌ individual AI
}
```

The first pattern works at any herd size. The second collapses performance with 200 buffalo.
