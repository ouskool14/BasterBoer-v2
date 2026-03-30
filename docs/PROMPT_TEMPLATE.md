# BasterBoer — AI Prompt Template

Copy the block below and fill in the `[YOUR TASK]` section. The rest stays as-is.

---

```
You are working on BasterBoer, an open-ended land management simulation set in South Africa built in Godot 4.6 using C# (.NET) for simulation systems and GDScript for UI and events.

Repository: https://github.com/ouskool14/BasterBoer-v2

---

## YOUR TASK

[DESCRIBE EXACTLY WHAT YOU ARE ASKED TO DO HERE]

---

## REQUIRED READING BEFORE WRITING CODE

Read these files in the repository before responding:
1. docs/ARCHITECTURE.md — system map, LOD tables, performance rules
2. docs/EXTENDING.md — how to add features without breaking existing systems
3. Relevant source file(s) for the system you are touching

---

## ARCHITECTURE — THE RULES THAT CANNOT BE BROKEN

### Rule 1: Simulate vs Render (most important rule in this codebase)
Everything in the world is simulated at all times as pure C# data (no scene nodes).
Only what is near the player is rendered (scene nodes, MultiMeshInstance3D).
Simulation systems (AnimalSystem, HerdBrain, FloraSystem, EconomySystem, TimeSystem) MUST NEVER instantiate Godot scene nodes. That is exclusively the render layer's job.

### Rule 2: Herd Instinct — Collective AI Only
HerdBrain makes ONE decision per herd. All AnimalStruct members follow it with variation.
NEVER add per-animal state machines, pathfinding, or per-frame decision logic.
Variation (spread, speed, animation timing) is produced by timer-based math — NOT per-frame RNG.
This is the single most performance-critical constraint in the project.

### Rule 3: Instancing is Non-Negotiable
Any object appearing more than once in the world uses MultiMeshInstance3D.
Visual variety is achieved via per-instance shader parameters, NOT unique meshes.
One draw call per species. Breaking this immediately blows the draw call budget.

---

## SYSTEM DEPENDENCIES

TimeSystem (heartbeat)
  └── drives daily/monthly ticks for AnimalSystem, FloraSystem, EconomySystem

AnimalSystem (singleton)
  ├── manages List<HerdBrain>
  ├── per-frame: herd.Tick(delta, playerPos) with LOD awareness
  └── monthly: aging, death compaction, population cleanup

HerdBrain (one per herd)
  ├── shared state: Thirst, Hunger, Fatigue, FearLevel
  ├── states: Grazing → Moving → Drinking → Resting → Fleeing → Alerting
  ├── owns AnimalStruct[] array
  └── LOD tiers: Full (0–150m) | High/Medium (150m–2km) | Background (2km+)

WorldChunkStreamer
  └── 3×3 active chunk grid — herds are NOT chunk-owned

Key singletons: AnimalSystem.Instance · GameState.Instance · WaterSystem.Instance
                WorldChunkStreamer.Instance · FenceSystem.Instance · TimeSystem.Instance

---

## NAMESPACES

LandManagementSim.Simulation  → HerdBrain, AnimalStruct, and simulation logic
BasterBoer.Core / BasterBoer.Core.Systems → GameState, WaterSystem, core data
WorldStreaming → WorldChunkStreamer, ChunkCoord
Do not introduce a new namespace.

---

## FILE LOCATIONS

All .cs simulation scripts are in the PROJECT ROOT (not in a Scripts/ subfolder).
GDScript files are also in the root.
docs/ contains markdown only. Assets/ contains GLBs. Scenes/ contains .tscn files.

---

## PERFORMANCE BUDGET — DO NOT EXCEED

Draw calls per frame:        < 300
Animal meshes rendered:      < 300
Herds at full AI:            < 8 simultaneously
RAM usage:                   < 1.5 GB
Monthly simulation tick:     < 20ms total

---

## C# CODE RULES

- NO GetNode() inside loops or per-frame methods (cache in _Ready())
- NO new allocations in _Process() or per-animal loops
- NO LINQ in hot paths — use direct array iteration
- NO simulation logic in _Process() — use TimeSystem tick signals
- Use struct for per-animal data (AnimalStruct), class for system managers (HerdBrain)

---

## OUTPUT FORMAT

- Show only the code changes required, not full file rewrites unless explicitly needed
- Specify: file name, method name, and exact insertion point for each change
- If you are unsure about intent, ask one clarifying question rather than making an assumption
- If your change would affect performance, state the estimated impact and why it is acceptable
```
