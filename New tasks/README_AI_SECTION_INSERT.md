## 🤖 For AI Assistants

> If you are an AI agent working on this project, read this section before anything else. It will save you from the most common mistakes.

**Read in this order:**
1. This section (you're here)
2. `docs/ARCHITECTURE.md` — binding technical rules, system map, LOD tables, performance budget
3. `docs/EXTENDING.md` — how to add features safely without breaking existing systems
4. `GAME_VISION v0.6.md` — full design document (use the TL;DR at the top; read the rest only if your task requires it)

**The three rules you must internalise before writing a single line:**

**Rule 1 — Simulate vs Render.**
Every system has two layers: a C# data layer that is always alive (no scene nodes), and a render layer that only exists near the player. Never instantiate scene nodes from simulation code (`AnimalSystem`, `FloraSystem`, `HerdBrain`, `EconomySystem`, `TimeSystem`). That is the render layer's job exclusively.

**Rule 2 — Herds think collectively.**
`HerdBrain` makes one decision. All animals in the herd execute it with variation. Do not give individual `AnimalStruct` members their own state machines, pathfinding, or per-frame decision logic. The variation (speed offsets, spread radius, reaction lag, animation timing) is produced by timer-based math using each animal's ID — not per-frame RNG, not individual AI. This is non-negotiable for both performance and biological authenticity.

**Rule 3 — Instancing is not optional.**
Any object appearing more than once uses `MultiMeshInstance3D`. One mesh per species, with shader parameters for variation. Unique meshes per animal/tree/rock will blow the draw call budget immediately.

**Key singletons (all use `.Instance`):**
`AnimalSystem` · `GameState` · `WaterSystem` · `WorldChunkStreamer` · `FenceSystem` · `TimeSystem`

**File location note:**
C# simulation scripts currently live in the **project root**, not in a `Scripts/` subfolder. GDScript files also live at root level. The `docs/` folder contains markdown only. `Assets/` contains GLBs. `Scenes/` contains `.tscn` files.

**Namespace note:**
Simulation classes use `LandManagementSim.Simulation` (e.g. `HerdBrain`). Core systems use `BasterBoer.Core` and `BasterBoer.Core.Systems`. Do not introduce a third namespace without flagging it.

**Performance budget (do not exceed):**
Draw calls < 300 · Animal meshes rendered < 300 · Herds at full AI < 8 · RAM < 1.5 GB · Monthly sim tick < 20ms

**When in doubt:** extend, don't rewrite. Ask for clarification rather than making assumptions about intent.
