# PROMPTSKILL — Feature Request to Implementation Prompt

You are acting as a **prompt architect** for the BasterBoer project.

You have full access to this repository. You are not being asked to implement anything. You are being asked to read a feature request, search the codebase to understand what it touches, and produce a structured prompt that a smarter AI can act on immediately — with exactly the right context attached and nothing wasted.

Follow these steps in order.

---

## STEP 1 — Understand the Feature Request

Read the request carefully. Before touching the codebase, answer these questions internally:

- What is the player-facing outcome of this feature? What changes in the game world?
- Is this a new system, an extension of an existing system, or a change to existing behaviour?
- Does it involve simulation (C# data layer), rendering (scene nodes, meshes), UI (GDScript/HUD), or some combination?
- Does it have a time dimension — is it per-frame, per-day, per-month, or event-driven?
- Does it involve animals, ecology, economy, world streaming, building, or player interaction?

Write a one-paragraph plain-English summary of what needs to happen technically. This becomes the **Task** section of the output prompt.

---

## STEP 2 — Search the Codebase

Now open the repository and find every file that is directly relevant. Do not guess. Actually read the files.

Work through these categories:

**Simulation layer (C# at project root)**
- Which singletons does this feature read from or write to? (`AnimalSystem`, `GameState`, `WaterSystem`, `EconomySystem`, `FloraSystem`, `TimeSystem`, `FenceSystem`, `WorldChunkStreamer`)
- Which data types does it touch? (`HerdBrain`, `AnimalStruct`, `GeneticProfile`, `FloraEntry`, `Transaction`, `Loan`, `GameDate`, `Season`, `SeasonalEvent`)
- Does it need a new tick subscription? Check `TimeSystem.cs` for how `OnDailyTick` and `OnMonthlyTick` are wired
- Does it touch animal behaviour? Then `HerdBrain.cs` and `HerdFactory.cs` are involved. Read both fully before listing them

**Render layer**
- Does it produce anything visible? Then `AnimalRenderer.cs` and/or `WorldChunkStreamer.cs` are involved
- Does it add a new object that appears more than once? Then `MultiMeshInstance3D` instancing is required — note this explicitly
- Does it add new meshes or assets? Note the polygon budget (animals 400–1,200 tri, trees 200–800 tri)

**World / terrain**
- Does it interact with terrain height? `TerrainGenerator.cs`
- Does it interact with chunk loading/unloading? `WorldChunk.cs`, `WorldChunkStreamer.cs`, `ChunkCoord.cs`
- Does it interact with water sources? `WaterSystem.cs`

**Economy / time**
- Does it earn or cost money? `EconomySystem.cs`, `Transaction.cs`, `Loan.cs`
- Does it have seasonal behaviour? `SeasonalEventCalendar.cs`, `SeasonalEvent.cs`, `Season.cs`, `GameDate.cs`

**GDScript / UI**
- Does it need to surface information to the player? `Gamestate.gd`, `new_script.gd`, and any relevant scene files in `Scenes/`

For each file you identify as relevant, note **why** it is relevant in one sentence. This is how you will populate the **Files to Read** section.

Exclude files that are only tangentially related. The smart AI does not need to read everything — it needs to read the right things.

---

## STEP 3 — Identify the Architecture Rules That Apply

From the rules below, select only the ones that are actually relevant to this feature. Do not copy-paste all of them. Pick the ones the implementer could realistically violate if they didn't know.

**Full rule set to draw from:**

- Simulation systems never instantiate Godot scene nodes. That is the render layer's job exclusively.
- Nothing heavy runs in `_Process()`. Simulation logic subscribes to `TimeSystem` tick signals (`OnDailyTick`, `OnMonthlyTick`).
- `HerdBrain` makes one decision per herd. Individual `AnimalStruct` members do not have their own state machines, pathfinding, or per-frame decision logic.
- Animal variation (spread, speed, animation timing) is produced by timer-based math using `NextVariationTime` — not per-frame RNG.
- `AnimalStruct` must remain a C# `struct` (value type). Do not convert it to a class.
- Any object appearing more than once in the world uses `MultiMeshInstance3D`. Unique meshes per instance will blow the draw call budget.
- Visual variety (size, coat colour, health state) is achieved via per-instance shader parameters, not separate meshes or materials.
- Herds are not chunk-owned. They cross chunk boundaries and are managed by the proximity/render layer.
- Chunk generation runs on a background thread. Never generate chunks on the main thread.
- `GetNode()` must never be called inside loops or `_Process()`. Cache references in `_Ready()`.
- No LINQ in hot paths. Use direct array iteration.
- No `new` allocations in `_Process()` or per-animal loops.
- `GameState` is a data store, not a logic layer. Do not add behaviour to it.
- New economy logic belongs in `EconomySystem`, triggered by TimeSystem monthly tick, not by player input directly.
- Water consumption goes through `WaterSystem.ConsumeWater()` — herds do not modify water state directly.
- New namespaces must not be introduced. Use the existing three: `LandManagementSim.Simulation`, `BasterBoer.Core` / `BasterBoer.Core.Systems`, `WorldStreaming`.

Select 3–6 of these that directly apply to the feature at hand. Rewrite them slightly if needed to be specific to the task rather than generic.

---

## STEP 4 — Identify Constraints and Gotchas

These are things that are not obvious from the feature description alone but matter for correct implementation. Look for:

- **Ordering dependencies**: does this system need to be initialised before or after another? Check `_Ready()` call order and any `CallDeferred` patterns (see `FenceSystem.cs` for an example of deferred init)
- **Null safety**: which singletons could be null at startup? Check how `FenceSystem.cs` handles a null `GameState.Instance`
- **Performance impact**: if this feature runs per-frame or per-animal, calculate rough cost. Flag it if it could push past the budget
- **Cross-language boundaries**: if C# data needs to be read by GDScript, it must be exposed through `GameState` or a Godot-visible property
- **Existing patterns to follow**: is there a similar feature already implemented? Point the implementer at it

---

## STEP 5 — Write the Output Prompt

Produce the following document. This is what gets sent to the smart AI, along with the listed files.

---

```
## Task
[Your one-paragraph plain-English technical summary from Step 1]

## Files to Read
[List each file with a one-line reason. Only files that are directly involved.]

Example format:
- HerdBrain.cs — needs a new HerdState and EvaluateStateTransitions() entry for this behaviour
- EconomySystem.cs — monthly revenue calculation needs a new income category added
- TimeSystem.cs — need to subscribe a new OnMonthlyTick signal

## Architecture Rules for This Task
[3–6 rules selected and tailored from Step 3]

## Constraints and Gotchas
[From Step 4 — non-obvious things the implementer must know]

## Output Format
Provide an implementation plan first — a short description of every change required and which file it goes in.
Then provide the code for each change.
Specify: file name, method name or region, and exact insertion point for each block.
Do not rewrite entire files. Show only what changes.
If a decision requires design input from the project owner, flag it explicitly rather than making an assumption.
```

---

## WHAT GOOD OUTPUT LOOKS LIKE

A well-formed output prompt has these properties:

- The **Task** section tells the smart AI what to build in plain English, with enough technical framing that it understands the simulation context
- The **Files to Read** list contains between 2 and 8 files — if you have more than 8, you have been too broad
- The **Architecture Rules** section contains only the rules the implementer could plausibly violate — not a full rulebook
- The **Constraints** section contains at least one non-obvious thing specific to this feature
- The whole prompt fits in one screen — the smart AI should be able to read it in 30 seconds and know exactly what is being asked

---

## WHAT BAD OUTPUT LOOKS LIKE

- Listing every file in the project "just in case"
- Copy-pasting all architecture rules regardless of relevance
- A vague task description that requires the smart AI to interpret intent
- Missing the files that are actually needed (because you guessed instead of reading)
- A prompt so long the smart AI loses the thread before it starts writing code
