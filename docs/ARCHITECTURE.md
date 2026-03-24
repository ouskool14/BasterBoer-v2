# BasterBoer — Technical Architecture

> This document is the binding technical reference for all contributors and AI assistants.
> For the game vision, see [README.md](../README.md) and [GAME_VISION v0.6.md](../GAME_VISION%20v0.6.md).

---

## 1. Fundamental Rule: Simulate vs Render

> Everything in the world is simulated at all times. Only what is near the player is rendered.

| System | Simulated (always, C# data) | Rendered (proximity only, scene nodes) |
|---|---|---|
| Animal herds | `HerdBrain` C# class in `AnimalSystem` | Individual meshes via `MultiMeshInstance3D` |
| Flora | `FloraEntry` struct in `FloraSystem` | `MultiMeshInstance3D` per species per chunk |
| Grass | Coverage float per terrain sector | Blade meshes animated by wind shader |
| Staff | C# class in StaffSystem | Character node only when near player |
| Rivers/Dams | Water level float | Shader parameter on terrain mesh |
| Fire | `FireEvent` C# class | Particles + terrain colour overlay near player |
| Buildings | `BuildingData` C# class | Scene node loaded/freed with chunk |

---

## 2. System Architecture

```
TimeSystem (heartbeat)
  ├── daily tick → AnimalSystem.OnDailyTick(), health effects
  ├── monthly tick → AnimalSystem.OnMonthlyTick(), aging, death, reproduction
  ├── seasonal events → EconomySystem (hunting season, drought, fire risk)
  └── year tick → permit renewals, annual grants

AnimalSystem (singleton)
  ├── manages List<HerdBrain>
  ├── per-frame: herd.Tick(delta, playerPos) with LOD awareness
  ├── monthly: aging, death compaction (Array.Resize), population cleanup
  └── provides query interface (GetHerdsInRadius, AlertHerdsToThreat)

HerdBrain (one per herd, C# class)
  ├── shared state: Thirst, Hunger, Fatigue, FearLevel
  ├── state machine: Grazing → Moving → Drinking → Resting → Fleeing → Alerting
  ├── owns AnimalStruct[] array (cache-friendly value types)
  ├── reproduction timer (TryReproduce → SpawnOffspring)
  └── LOD-aware tick rates (Full/High/Medium/Background)

AnimalStruct (C# struct, per individual)
  ├── WorldPosition (offset from herd center)
  ├── Age, Health, Sex, Genetics
  ├── MeshInstanceId, CurrentAnimation
  └── NextVariationTime (timer-based, NOT per-frame RNG)

EconomySystem (monthly tick from TimeSystem)
  ├── revenue: trophy hunting, tourism, game sales, carbon credits, research
  ├── costs: staff wages, maintenance, fuel, permits
  └── loan processing

WorldChunkStreamer
  ├── 3×3 grid of active chunks around player
  ├── chunks own terrain, flora MultiMesh, buildings
  ├── herds are NOT chunk-owned (they cross boundaries)
  └── background thread chunk generation
```

---

## 3. The Herd Instinct Model

Animals are rendered individually but think collectively.

**HerdBrain** makes one decision → all animals execute it with biological variation:
- Slightly different speeds (young slower, dominant males at front)
- Spread formation while grazing, tight cluster while fleeing
- Reaction lag (signal ripples through herd, not instant)
- Wander offset from herd center during grazing

Variation is produced by timer-based math offsets, **not** per-frame RNG or individual AI.

**Exceptions:** Solitary species (leopard, caracal, honey badger) run individual state machines.

---

## 4. LOD System

### Behaviour LOD

| Distance | Activity |
|---|---|
| 0–150m | Full: all needs, awareness, animations active |
| 150–500m | Reduced: state changes only, simplified animation |
| 500m–2km | Minimal: position updated once/second |
| 2km+ | Simulation only: monthly tick, no world position |

### Visual LOD

| Distance | Render |
|---|---|
| 0–100m | Full mesh, full skeletal animation |
| 100–350m | Full mesh, simplified animation |
| 350–800m | Billboard impostor |
| 800m+ | Not rendered |

---

## 5. Performance Optimizations Applied

These optimizations are already implemented and must be preserved:

| Optimization | Location | Impact |
|---|---|---|
| Timer-based animation variation | `HerdBrain.UpdateIndividualAnimals` | Eliminates thousands of RNG calls per frame |
| Cached spread radius | `HerdBrain._currentSpreadRadius` | Removes per-animal switch evaluation |
| Direct field check for death | `animal.Health <= 0f` instead of `IsAlive` | Avoids property call overhead in hot loop |
| Safe vector normalization | `ExecuteCurrentState` | Avoids unnecessary sqrt when at target |
| Array compaction for dead animals | `AnimalSystem.OnMonthlyTick` | No gaps, no per-frame allocations |

---

## 6. Instancing Rules (Non-Negotiable)

Any object appearing more than once **must** use `MultiMeshInstance3D`:

- Trees → one `MultiMeshInstance3D` per species per chunk
- Animals → one `MultiMeshInstance3D` per species per visible herd
- Grass → one per chunk, animated by GPU wind shader
- Rocks, fence posts, shrubs → all instanced

Visual variety (size, health, coat colour) is achieved through **per-instance shader parameters**, not unique meshes.

---

## 7. C# Code Rules

- **Never** run heavy simulation in `_Process()` — simulation belongs in TimeSystem ticks
- Use **structs** for animal data (cache-friendly contiguous memory)
- Use **classes** for herd brains and system managers
- **Avoid LINQ** in hot paths — use direct array iteration
- Cache node references in `_Ready()` — never `GetNode()` inside loops
- Simulation systems must **never** instantiate scene nodes — that's the render layer's job
- Use background threads for chunk generation

---

## 8. Asset Rules

### Meshes (GLB)
- Trees: 200–800 triangles
- Animals: 400–1,200 triangles
- Rocks: 50–200 triangles
- One base mesh per species; scale/colour via shader

### Textures
- Combined into texture atlases per biome (max 2048×2048)
- No unique materials per object
- Animal coat variation via shader colour parameters

### Audio
- All ambient sounds use `AudioStreamPlayer3D` with distance attenuation
- Max 10 simultaneous 3D audio sources
- Music in bakkie attenuates with distance

---

## 9. Performance Budget (Target: Mid-Range PC ~2021)

| Metric | Target |
|---|---|
| Draw calls per frame | < 300 |
| Active MultiMesh instances visible | < 6,000 |
| Herds running full AI | < 8 simultaneously |
| Individual animal meshes rendered | < 300 |
| Active particle systems | < 8 |
| Active 3D audio sources | < 12 |
| RAM usage | < 1.5 GB |
| Chunk load time | < 80ms (background thread) |
| Monthly simulation tick | < 20ms total |

---

## 10. File Structure

```
claude-game/
├── project.godot
├── README.md                    ← Game overview and vision
├── GAME_VISION v0.6.md          ← Full design document
├── docs/
│   └── ARCHITECTURE.md          ← This file
├── Scenes/
│   ├── main.tscn                ← Main scene
│   ├── boer.tscn, bakkie.tscn, terrain.tscn
├── Scripts/                     ← GDScript (player, vehicle, terrain, UI)
│   ├── Player.gd, Bakkie.gd, Terrain.gd, TreeSpawner.gd
├── Assets/                      ← 3D models (GLB)
│   ├── Characters/              ← Player, NPCs
│   └── Kudu.glb, Zebra.glb, Cow.glb, etc.
├── AnimalSystem.cs              ← Singleton herd manager
├── AnimalStruct.cs              ← Per-animal data struct
├── HerdBrain.cs                 ← Herd AI controller
├── HerdFactory.cs               ← Species-specific herd creation
├── EconomySystem.cs             ← Financial processing
├── TimeSystem.cs                ← Game clock and signals
├── GameDate.cs                  ← Calendar with SA seasons
├── FloraSystem.cs / FloraEntry.cs / FloraPopulator.cs
├── TerrainGenerator.cs          ← Procedural terrain meshes
├── WorldChunk.cs / WorldChunkStreamer.cs / ChunkCoord.cs
├── GameState.cs                 ← Central data store
├── GeneticProfile.cs            ← Animal genetics
├── BehaviourLOD.cs              ← LOD tier definitions
└── Season.cs / SeasonalEvent.cs / SeasonalEventCalendar.cs
```

---

## 11. Why C# Over GDScript

C# compiles to native code via .NET. On CPU-bound work — iterating hundreds of animal structs, genetics calculations, terrain generation — it runs **3–10× faster** than GDScript.

GDScript remains acceptable for: UI, event-driven systems, and infrequent scripts (monthly ticks, one-off decisions).

Both coexist in the same Godot 4 project. C# systems expose data through `GameState` which GDScript can read.
