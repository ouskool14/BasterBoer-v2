# BasterBoer

> *"You are a South African landowner — build, manage, and protect a piece of Africa on your own terms, where every decision has a consequence and no two playthroughs look the same."*

**BasterBoer** is an open-ended, consequence-driven land management simulation set in South Africa. Manage a game farm, breed wildlife, host trophy hunts, run a safari lodge, fight drought and poaching — or do all of it at once. There is no win condition. There is only the land, and what you make of it.

---

## 🌍 What Is This Game?

The player acquires a piece of land — a farm, a game reserve, a smallholding, or a vast wilderness tract — and manages it across time. You might:

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

## 🚀 Getting Started

1. Install **Godot 4.6** with .NET support from [godotengine.org](https://godotengine.org/)
2. Clone this repository
3. Open Godot → **Import** → select `project.godot`
4. Press **F5** to run

### Controls

| Key | Action |
|-----|--------|
| `W / A / S / D` | Move |
| `Space` | Jump |
| `Shift` | Sprint |
| `E` | Interact |
| `Mouse` | Look around |
| `Esc` | Toggle mouse cursor |

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
