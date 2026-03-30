> ⚠️ **AI AGENTS: Read this TL;DR block first. The full document below is for humans and deep context — skip it unless a specific question requires it.**

---

## TL;DR — AI Quick Reference

**Genre:** Open-ended land management simulation. South Africa. No win condition.

**North Star:** *"You are a South African landowner — build, manage, and protect a piece of Africa on your own terms, where every decision has a consequence and no two playthroughs look the same."*

**Core Loop:**
- Player holds a vision for what their farm could become
- They go out onto the farm and close the gap between that vision and reality
- The land pushes back (drought, disease, poaching, finances, staff, time)
- Seasons close. The farm evolves. No ending.

**What the player manages:** Land & ecology · Animals · Economy (ZAR) · Staff & community · Hunting · Building

**What this game is NOT:**
- Not a survival game (no hunger/thirst bars for the player character)
- Not Planet Zoo (animal welfare is a consideration, not the entire game)
- Not a city builder (no zoning grids, no civic systems)
- Not linear (no campaign, no story to finish, no final boss)
- Not easy (one difficulty: reality — no sliders, no beginner mode)

**Engine:** Godot 4.6 · **Language:** C# for simulation, GDScript for UI/events · **Style:** Low-poly 3D

**Single most important technical rule:**
> Everything in the world is simulated at all times. Only what is near the player is rendered.

**Single most important animal architecture rule:**
> Animals think collectively (one `HerdBrain` per herd), not individually. Do NOT give individual animals their own AI state machines. This is both the performance model and the correct biological model.

**Currency:** South African Rand (ZAR) · **Starting year:** 2024

---

*Full design document follows. Sections 1–8 cover vision and philosophy. Section 9 covers reference games (AI: skip). Sections 10–14 cover technical architecture, performance, and player experience design.*

---
