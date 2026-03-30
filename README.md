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

# BasterBoer

> *"The player is a South African landowner — build, manage, and protect a piece of Africa on his own terms, where every decision has a consequence and no two playthroughs look the same."*

**BasterBoer** is an open-ended, consequence-driven land management simulation set in South Africa. The player manages a game farm, breeds wildlife, hosts trophy hunts, runs a safari lodge, fights drought and poaching — or do all of it at once. There is no win condition. There is only the land, and what the player makes of it.

---

## 🌍 What Is This Game?

The player acquires a piece of land — a farm, a game reserve, a smallholding, or a vast wilderness tract — and manages it across time. The player might:

- Run a game breeding operation on 300 hectares
- Operate a photographic safari lodge on 10,000 hectares in Limpopo
- Farm sheep and manage jackal pressure on 50,000 hectares in the Northern Cape
- Offer trophy hunting to international clients in the Waterberg
- Build a conservation reserve with research partnerships and anti-poaching units
- Or any combination of the above

All paths are equally valid. None is "the correct way to play."

---

## 🎮 Gameplay Pillars

| Pillar | Description |
|--------|-------------|
| **Land & Ecology** | Grass, water, soil, fire, invasive species, erosion, and biodiversity all interact. Weather creates unpredictable pressure. |
| **Animals** | Wildlife and livestock coexist, compete, and conflict. Herd instinct AI drives realistic group behaviour. Genetics and breeding are long-term investments. |
| **Economy** | Multiple ZAR-denominated revenue streams — trophy hunting, tourism, live game sales, carbon credits, research grants. |
| **People** | Staff with skills, morale, and loyalty. Community sentiment. Government policy. Nothing exists in isolation. |
| **Hunting** | Authentic hunting experiences — no glowing outlines, no easy mode. Trackers, baiting, and PHASA-compliant quotas. |
| **Building** | Homestead and lodge built piece by piece. Authentically African design — thatch, plaster, corrugated iron, stoep. |
| **Game Feel** | Music in the bakkie fading as you walk away. Boma fires at night. Chickens at dawn. Lions calling at dusk. The world is always breathing. |

---

## 🛠️ Tech Stack

- **Engine:** Godot 4.6
- **Language:** C# (.NET) for simulation, GDScript for UI/events
- **Style:** Low-poly 3D — stylised, not realistic. Beauty from light and colour, not polygon count.
- **Architecture:** Data-Oriented Design. Simulate everything, render only what's near the player.

For the full technical architecture, LOD system, performance budgets, and coding rules, see **[docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)**.

---

## 📋 Current State

Early prototype with:
- Third-person player character and driveable bakkie
- Procedural terrain with noise-based height and South African earth tones
- Procedural tree/shrub spawning
- C# simulation layer: `AnimalSystem`, `HerdBrain`, `EconomySystem`, `TimeSystem`, `FloraSystem`
- Data-oriented animal architecture with LOD-aware herd AI
- Herd reproduction, death cleanup, and genetic inheritance
- South African seasonal calendar with event system

---

## 🎯 Design Philosophy

- **The land is a character.** It degrades or recovers based on your decisions.
- **No win condition — only consequence.** Keeping cattle alive through drought is a victory. Building a Big Five reserve is another. Both are valid.
- **One difficulty: reality.** No easy mode. The apprentice phase teaches; it does not protect.
- **Stories emerge, they are not scripted.** The game does not tell you a story. You live one.

---

## 📖 Full Vision Document

The complete game design document is available at **[GAME_VISION v0.6.md](GAME_VISION%20v0.6.md)** — covering origin story, gameplay loops, player experience design, characters, and world details.

---

## 📄 License

This project is in active development. All rights reserved.
