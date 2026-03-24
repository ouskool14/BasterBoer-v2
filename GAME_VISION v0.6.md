# GAME VISION DOCUMENT
## Project: BasterBoer (Working Title)
### Version 0.6 — Master Reference for All Contributors

---

## 1. NORTH STAR

> *"You are a South African landowner — build, manage, and protect a piece of Africa on your own terms, where every decision has a consequence and no two playthroughs look the same."*

This is the single sentence every design, mechanic, and visual decision must serve. If something contradicts it, it does not belong in the game.

---

## 2. WHAT THIS GAME IS

BasterBoer is an open-ended, consequence-driven land management simulation set in South Africa. The player acquires a piece of land — a farm, a game reserve, a smallholding, or a vast wilderness tract — and manages it across time. There is no prescribed path to victory. There is no final level. The game is about the *relationship between a person and their land*, and all the complexity that comes with it.

The player may choose to:
- Run an intensive game breeding operation on 300 hectares
- Operate a photographic safari lodge on 10,000 hectares in Limpopo
- Farm sheep and manage jackal pressure on 50,000 hectares in the Northern Cape
- Offer trophy hunting to international clients in the Waterberg
- Build a conservation-focused reserve with research partnerships and anti-poaching units
- Or any combination of the above

All of these are equally valid. None is "the correct way to play."

---

## 3. CORE DESIGN PHILOSOPHY

### 3.1 The Land Defines the Game
The player's starting land — its size, location, terrain, rainfall, and soil — is the primary shaper of their experience. A small plot demands intensity, attention to detail, and high yield per hectare. A large tract demands patience, strategic thinking, and working with nature rather than against it. The land is not a backdrop. It is a character.

### 3.2 No Win Condition — Only Consequence
There are no achievement trees to complete. No unlock-all-items endgame. The game measures nothing except what the player chooses to care about. A player who keeps a small herd of cattle alive through three years of drought has won something profound. A player who builds an internationally recognised reserve has won something else. Both are valid. Neither is scored.

### 3.3 Emergent Complexity
Simple systems must interact to create complex, unpredictable situations. The weather affects the grass. The grass affects the animals. The animals affect the ecology. The ecology affects income. Income affects decisions. Decisions affect relationships — with staff, community, government, and the land itself. The player should never feel like they are managing a spreadsheet. They should feel like they are managing a *place*.

### 3.4 Authenticity Over Abstraction
This game is set in a specific country, with specific animals, specific legislation, specific cultural tensions, and specific landscapes. That specificity is the game's identity. South African conservation realities — PHASA-compliant hunting quotas, DFFE regulations, community land rights, drought cycles, load shedding, poaching pressure — are not background flavour. They are mechanics.

### 3.5 Non-Linear Growth
The player should always feel that expansion is *possible*, never that it is *required*. A perfectly managed small operation is a complete game. But the option to purchase adjacent land, drop fences, merge ecosystems, and take on new challenges must always exist. Growth comes with cost and consequence — new land means new problems.

### 3.6 Becoming, Not Managing
The player is not a faceless manager clicking through menus. They are a person with a history, a love for Africa, and a grand vision that is bigger than their current means. The game is about the gap between that vision and reality — and the slow, satisfying process of closing it. Every system should serve this emotional truth.

### 3.7 One Difficulty — Reality
There is no difficulty setting. There is no easy mode, normal mode, or hard mode. There is one mode: Africa. The game is difficult because real land ownership and management is difficult. Players who want a gentle experience will find it in other games. Players who survive the learning curve of BasterBoer become its most devoted advocates, because what they built was genuinely hard to build.

The difficulty is managed through the origin story and apprentice phase — which gives players context and foundation before handing them full responsibility — not through mechanical handicaps or slider settings. The game respects its players enough to be honest with them from the first hour.

---

## 4. TONE AND FEEL

- **Not a toy.** The game treats its subject matter seriously. Conservation, hunting, community relations, and land management are complex, morally layered topics. The game does not sanitise them.
- **Not punishing.** Difficulty comes from complexity, not cruelty. The game creates pressure through systems, not arbitrary events designed to frustrate.
- **Grounded and beautiful.** Visually and aurally, the game should evoke the real African bush — golden light, red dust, silence broken by bird calls and distant thunder. The aesthetic serves immersion, not spectacle.
- **Stories emerge, they are not scripted.** The game does not tell the player a story. The player lives one.

---

## 5. GAMEPLAY PILLARS

### Pillar 1: Land & Ecology
The foundation. Grass, water, soil, fire, invasive species, erosion, and biodiversity all interact. The land is alive and will degrade or recover based on what the player does and does not do. Weather systems — drought, flood, heat wave, frost — create unpredictable pressure.

**Rivers and Dams**
Rivers and dams are fixed landscape features — the player does not divert rivers or excavate new dams, but they live with what they have and manage it carefully. Both scale dynamically between full and empty based on rainfall, season, and drought state. A river running strong in February may be cracked clay by September in a drought year. A dam at 12% capacity after three dry years is one of the most visceral read-outs of how the farm is doing — no number required, the player sees it. Player interaction with water is through infrastructure: pump placement, pipeline from dam to trough, borehole drilling, irrigation channels. The water itself is a given. What you do with it is the choice.

**Seasonal Fire Management**
Fire is one of the most important and skilled land management tools in the bushveld. Controlled burns — done correctly, in the right month, with the right wind conditions and prepared firebreaks — reset the veld, improve spring grazing, and reduce the standing fuel load that makes uncontrolled fires catastrophic. A player who burns their eastern block correctly in late August gets measurably better grass coverage in October. A player who does not manage their firebreaks risks losing that block entirely when a lightning strike or a spark from a vehicle exhaust catches the dry September veld.

Fire that escapes the player's property has consequences beyond the fence line — damaged neighbour relationships, possible legal exposure, and a reputation cost in the community that takes time to repair. Drought years elevate fire risk across the entire system. The fire season is a real seasonal pressure that demands preparation, not reaction. Fire that burns through a camp destroys infrastructure and can kill animals. Fire that is managed well is also one of the most visually and atmospherically powerful things in the game — the blackened veld, the first green flush two weeks later, the animals drawn to graze the new growth.

### Pillar 2: Animals
Wildlife and livestock coexist, compete, and conflict. Each species has biological needs, genetic considerations, population dynamics, and economic value. Predator-prey relationships, disease vectors, and carrying capacity are real constraints. Genetics and breeding are long-term investments with long-term payoffs. Some farming cycles span multiple years — a buffalo herd takes years to grow, and the player must budget and plan accordingly.

Animals are rendered as individuals — every impala is its own mesh, its own body in the world. But animals behave through **herd instinct**. A herd of impala shares one nervous system. One animal catches a predator's scent and the signal ripples outward — the whole group bolts. The herd gets thirsty and moves as one body toward the waterhole. They tire, they settle, they graze together. This is not a simplification. This is how prey animals actually behave. Individual animals follow the herd's collective state with natural variation — slightly different speeds, slightly different paths, a few seconds of lag — giving the appearance of individual thought without the cost of individual AI. Solitary animals — leopard, caracal, aardvark — are the exception and do run individual state machines.

The goal of many players will be a Big Five farm — lion, leopard, elephant, rhino, buffalo together on one property. This is a valid and extremely demanding long-term vision. It requires years of ecological preparation, significant capital, the right permits, the right infrastructure, and the ability to manage species that compete and conflict with each other and with the player's other operations. Not every farm gets there. The ones that do have built something genuinely rare.

**Micro-Wildlife and Ambient Life**
The world is not only the animals the player manages. It is alive with everything else. Termite mounds rise from the ground — ancient, permanent landmarks of the veld. Ant colonies move in columns across the paths. Hares bolt from the bakkie's headlights at night. Francolin call at dawn. A hadeda sits on the fence post. Yellow-billed hornbills follow the tractor. A monitor lizard crosses the road in front of you and doesn't hurry. None of this is managed. All of it is present. The ecological health of the farm is reflected in the richness of this ambient life — a degraded farm is quieter, emptier, browner. A healthy farm hums.

The game aims for a broader species count than any comparable title — not all present on one farm simultaneously, but drawn from an authentic South African roster that the player encounters over time and across different biomes. Birds especially are numerous: raptors that circle above good game country, oxpeckers on the buffalo, weavers in the fever trees, owls calling at night.

**Darting and Capture Operations**
Live capture is one of the most physically demanding, logistically complex, and financially significant operations on a game farm. The player can plan and execute capture operations — darting animals from a vehicle, from a helicopter, or using ground teams with nets and funnel systems. Triangle screens (large fabric panels on poles) are used to direct animals toward a capture boma or loading crate. The operation requires preparation: the right drugs drawn up by a vet, a suitable crate, a handling team, a vehicle to transport the animal.

Helicopter capture is available for players who can afford the charter cost — faster, more effective on open ground, essential for difficult terrain. Drone reconnaissance can be used to locate specific animals before a ground capture. Animals that are poorly handled sustain capture myopathy — a real and costly outcome. Animals that are expertly handled, correctly dosed, and transported under good conditions arrive at the buyer in condition.

Captured animals are a major revenue stream — live game sales. They can also be used to restock the player's own camps, translocate problem animals, or fulfil breeding programme requirements. The conveyor belt under a downed buffalo, dragged by the bakkie with a staff member riding it to keep the animal stable — this is a real technique and it is in the game.

### Pillar 3: Economy
Multiple revenue streams — none mandatory. Trophy hunting, photographic tourism, live game sales, livestock farming, carbon credits, research partnerships, and government grants are all available. The player builds the economic model that suits their land and philosophy. ZAR-denominated, South Africa-grounded. Profit is not the only measure of success, but survival requires it.

### Pillar 4: People
Staff, community, and external stakeholders. Rangers, guides, vets, professional hunters, lodge managers — all have skills, morale, loyalty, and needs. Staff make mistakes. They take initiative sometimes and disappoint other times. Their loyalty is earned, not given. The surrounding community has sentiment that matters. Political decisions from outside the farm — government culling orders, disease control zones, land policy changes — create pressure the player cannot fully control, only respond to.

### Pillar 5: External World
The player's land does not exist in isolation. Neighbouring farms have diseases that spread. Markets fluctuate. Government makes decisions. Drought does not stop at your fence line. This external pressure system ensures the game never becomes static or fully controllable.

### Pillar 6: Hunting
Hunting in BasterBoer is not a menu. It is an experience. The player hunts for two reasons: meat for the table and staff feeding, and trophy hunting with paying clients. There is no player hunger or sleep bar — the player character does not have survival mechanics. Hunting for food is a practical, recurring operation that feeds the staff and the homestead. Hunting for clients is a significant revenue stream that requires preparation, the right animals, the right permits, and the ability to deliver an authentic experience.

**No Outlines. No Easy Mode.**
Animals that move behind cover stay behind cover. There are no glowing outlines, no animal tracking overlays, no indicators that betray an animal's position through solid objects. If an impala steps behind a thicket, the player must read the terrain, anticipate the animal's direction, and move to intercept — or wait. Tracks in soft ground near the player reveal recent movement. A good tracker on the team reads sign that the player would miss entirely. This is hunting, not a shooting gallery.

**The Tracker and the Professional Hunter**
The player can hunt alone, but a skilled tracker changes the experience entirely. A tracker reads the ground — a broken twig, a scuff in the dust, the way the grass was pressed down — and communicates in the low language of the bush: a hand signal, a pointed finger, a slow crouch. The player who has invested in a skilled tracker has access to the bush in a way the solo hunter does not. A registered professional hunter (PH) can be brought in to run client hunts independently, taking the operational load off the player, or the player can acquire their own PH qualifications and run hunts personally.

**Bait Hunting**
Leopard are not driven past a blind. They are hunted on bait. The player selects a carcass, hangs it in a suitable tree, builds a hide at the right distance and downwind angle, and waits. The leopard may come that night. It may come three nights later. It may not come at all if the bait goes off or the wind shifts. Sitting in the hide at dusk — the light fading, the bush going quiet, then the sounds of night beginning — is one of the most atmospheric experiences in the game. The leopard arrives, or it doesn't. That uncertainty is the hunt.

**Hunting and the Ecology**
Hunting is not separate from the ecological system. Offtake is managed against the carrying capacity and breeding rates of each species. A player who over-hunts a species degrades the population and their own future revenue. PHASA-compliant quotas apply. A well-managed hunting operation is sustainable over decades. A poorly managed one collapses within a few years.

### Pillar 7: Building and Place
The player builds two things in BasterBoer: the working infrastructure of the farm, and the home they live in.

**Home Design**
The homestead is the player's personal anchor in the world. It is built piece by piece over the life of the farm — not from a template, but from individual choices about walls, roofs, extensions, and finishes. The design language is authentically African: plastered walls that the player can paint, thatched roofing that requires maintenance, wooden stoep pillars cut from trees, corrugated iron where money runs short, a braai area that expands as prosperity grows. The lodge, if the player builds one, uses the same system at a larger scale — each element placed and chosen, from the reed ceiling of the dining room to the plunge pool on the deck. This is not The Sims. It is the African equivalent — building a place that feels like it belongs where it stands. No two players' homesteads or lodges will look alike.

**Farm Roads**
The roads on the farm exist when the player arrives. They are what they are — gravel, dirt, two tracks through the grass. No one upgrades to tar. The roads are the roads. The player can drive anywhere on the farm, but going off-road has consequence: soft ground after rain bogs the bakkie down, rocky terrain damages the suspension, a dry riverbed crossing that looked safe in February is a trap in March. Getting stuck is a real event — someone has to come pull you out, and if you are far from anyone and the radio is flat, you wait. The player learns the farm's terrain the same way a real farmer does: by driving it until they know every bad crossing and every short cut.

**Temporary Field Camps**
For multi-day operations deep in the farm — capture work, fence repair in a remote area, anti-poaching patrols — the player can set up temporary camps in the veld. A canvas tent, a fire, a folding table, a generator if needed. These are functional, impermanent, and atmospheric. The player sleeps out on the farm they are building. The sounds at night are different out there.

### Pillar 8: Game Feels
Music playing in the bakkie (Leon Schuster, Bok van Blerk, etc.) and when you get out of the bakkie, it still plays but the further you get away the softer it is. The player should have a home where they can light a boma fire. Staff quarters are separate from the homestead. The player can have a dog. Farm animals near the home. Everything should feel modular and organic — staff numbers can grow over time through family or arrivals if not managed. Animal sounds in the distance: chickens crowing at dawn, lions calling at dusk if you have them. The world is always on, always breathing.

**The Player Character Has No Survival Bars.** There is no hunger meter, no thirst meter, no sleep counter. The player is not managing their own survival — they are managing a farm. Fatigue, hunger, and rest are part of the world's texture (the character sits at the fire at night, the day ends, time passes) but they are never numerical constraints on what the player can do. The complexity of this game comes from the land and its systems, not from personal survival mechanics.

    Random standouts:
        - To move a buffalo if you are poor, place a large piece of conveyor belt rubber under the buffalo and drag it with a bakkie, have someone sit on the buffalo so it doesn't fall off.
        - Getting the bakkie stuck in a vlei in the wet season. Someone has to come pull you out.
        - Sitting in a leopard hide at last light. The bush going quiet. Waiting.

---

## 6. THE GAME LOOP

### 6.1 The Grand Vision
The player carries a vision for what their farm could become — a world-class African lodge, a conservation stronghold, a productive livestock operation, or all of the above. This vision is the emotional engine of every session. It is never fully achieved. It evolves.

### 6.2 The Vision Board
A central mechanic — a holographic miniature of the player's farm, located in the home office on the plaas. The player plans here: placing future buildings, marking zones, setting goals. The vision board is where dreaming happens. Execution happens outside, on the land itself. The two are always in tension — what you plan and what you can afford or achieve are rarely the same thing.

The vision board is not a reward system. It gives no dopamine for designing. It is a tool, not a toy. The satisfaction comes from making the real farm match the vision, not from the planning itself.

### 6.3 Core Loop (Minute to Minute)
The player has a vision. They go out onto the farm to close the gap between that vision and reality. Tasks vary in duration and impact:
- Short recurring tasks: feeding livestock, checking water levels, patrolling fences, upkeep
- Medium tasks: building infrastructure, relocating animals, training staff
- Long tasks: installing boreholes, constructing lodges, establishing new camps, culling programmes

The player delegates, prioritises, and makes decisions under constraint — time, money, and staff capacity are always limited.

### 6.4 Meta Loop (Hour to Hour, Season to Season)
Over longer sessions, the following evolve visibly:
- Ecology: soil quality, grass coverage, water depth, species diversity — getting healthier or degrading
- Herd size: growing, declining, or changing in genetic quality
- Staff loyalty and skill: improving through experience and training, declining through neglect or bad management
- Reputation: attracting or repelling professional hunters, tourists, researchers, and government attention
- The farm itself: visually and functionally transforming as the player builds, plants, and shapes the land

### 6.5 Different Loops at Different Stages
The game is not one loop from start to finish. Early game, mid game, and late game feel meaningfully different:
- Early: survival, learning, small decisions with immediate consequences
- Mid: growth, delegation, balancing competing priorities
- Late: legacy, stewardship, managing complexity at scale

---

## 7. ORIGIN STORY & STARTING SCENARIO — The Shop Window

*This section is subject to refinement but the core idea must be protected.*

The player's father died when they were young. They grew up on his farm — love for Africa, animals, landscape, trees, even the dust, is in their blood. They are a natural. But they have almost no money and limited formal experience. They have to start small and work their way up.

The origin story serves two purposes simultaneously: it teaches the player how the game works, and it shows them why the game is worth learning. It is the shop window. Every best feature of BasterBoer — the atmosphere, the tension, the physicality of the world, the weight of a real decision — should be present in this opening sequence in a controlled, beautiful form. The player who finishes the apprentice phase should feel two things: *I understand how this works*, and *I need to see what this becomes on my own land.*

This means the origin story is partly scripted — carefully enough to be reliable and visually extraordinary, open enough that the player feels they caused what happened. The golden hour light over the veld on the first morning. The first time a herd of impala bolts and disappears into the bush. A small financial crisis that the player solves just in time. Oom Fasie appearing unexpectedly at the fence line. A fire on the horizon at dusk that turns out to be a controlled burn on the neighbour's farm. The boma fire at night and the sound of something moving in the dark beyond the light.

### 7.1 Apprentice Phase
The game begins with the player working on someone else's farm — an experienced boer who takes them on. This phase serves as the tutorial but does not feel like one. The player observes, takes initiative, makes mistakes, and earns small amounts of money and growing skill. Tasks have clear outcomes and short time horizons. The grand vision is visible on the horizon but not yet reachable.

The apprentice phase is not long and it does not suppress the player's agency — it *focuses* it. The player cannot yet build where they want or choose which animals to buy, but every small decision they make here has consequence. They are not following a script. They are learning a world. Critically, this phase manages the difficulty curve that the absence of an easy mode requires — the player encounters the game's systems with guardrails before those same systems are fully their responsibility.

This phase ends the moment the player can act more capably on their own than they can under instruction. That transition — not a cutscene or a menu — is when the keys are handed over.

### 7.2 Flashbacks
At key moments, brief flashbacks to childhood on the father's farm surface — not as cutscenes but as memory fragments. These deepen the emotional stakes without slowing the game down.

### 7.3 The First Plot
When the apprentice phase ends, the player acquires their first piece of land — small, imperfect, with potential. From here the game opens up fully. The origin story fades into the background and becomes context, not constraint.

---

## 8. WHAT THIS GAME IS NOT

- **Not a points game.** There is no score. There is no leaderboard goal.
- **Not an achievement hunter.** Unlocking everything is not the objective.
- **Not Planet Zoo.** Animal welfare is a consideration, not the entire game.
- **Not a city builder.** The player builds a home and a lodge, but infrastructure always exists to serve the land and the life on it — not the other way around. There are no zoning grids, no population happiness meters, no civic systems. What is built here is personal and functional, not civic.
- **Not a survival game.** The player character has no hunger bar, no thirst bar, no sleep counter. Survival mechanics apply to the animals, the ecology, and the finances — not to the player's body. The difficulty of this game comes from the land, not from personal attrition.
- **Not linear.** There is no campaign to complete, no story to finish, no final boss.
- **Not grid-bound.** While early development uses a grid for simplicity, the final game should feel organic — natural shapes, irregular boundaries, terrain that looks like Africa, not a spreadsheet.
- **Not easy.** There is one difficulty: reality. No sliders, no adjustments, no beginner mode. The apprentice phase exists to teach, not to protect.

---

## 9. REFERENCE GAMES (Tone and Mechanic Inspiration)

These games are referenced not to be copied, but to illustrate the *feeling* the player should have:

| Game | What to Borrow |
|------|----------------|
| **Banished** | Organic, consequence-driven growth; no winning, only surviving and thriving |
| **Cities: Skylines / SimCity** | Freedom to build and shape; systems that interact at scale |
| **Civilization series** | Long-arc decision making; the world changes around you |
| **Factorio** | Deeply interlocking systems; the satisfaction of optimisation |
| **Dungeon Keeper 2 / Black & White 2** | Personality and tone; the world reacts to your character |
| **Age of Empires** | Resource management under pressure; the feeling of building something real |
| **Subnautica** | Exploration and discovery within a living world; atmosphere above all |
| **Ranch Simulator** | Physical presence on the land; driving, doing, being there — the proof that this audience exists and wants more depth than Ranch Simulator gives them |
| **theHunter: Call of the Wild** | The gold standard for African bush atmosphere and sound design; proves the setting sells internationally; BasterBoer gives its players the reserve to own, not just visit |
| **Farm Manager series** | The failure case — what happens when farming simulation has no emergent complexity, no consequence depth, and no challenge. BasterBoer is the game Farm Manager players wish they were playing |
| **The Sims** | The emotional relationship between a player and a home they built themselves; the satisfaction of a personalised space that feels like theirs |

---

## 10. TECHNICAL FOUNDATION

- **Engine:** Godot 4.6
- **Language:** C# (.NET) — chosen for CPU performance at scale. Simulation systems (AnimalSystem, BreedingSystem, FloraSystem, WorldMap chunk generation) are written in C#. UI and event-driven systems may remain in GDScript where practical. Both languages coexist in the same project.
- **Visual Style:** Low poly 3D — stylised, not realistic. Clean geometry, warm colour palette, strong silhouettes. The aesthetic should read immediately as Africa without trying to photograph it. Think rolling terrain with flat-shaded meshes, low-poly acacia trees, animals with clear readable shapes. Beauty comes from light and colour, not polygon count.
- **Perspective:** 3D world with a dual-mode camera — third-person ground level when moving through the farm, overhead strategic view when planning. The Vision Board is the natural transition point between these two modes.
- **Architecture:** Singleton-based (`GameState` as central data store), modular systems (AnimalSystem, EconomySystem, EventSystem, etc.), signal-driven communication between systems
- **Animal Model:** Herd instinct architecture. Each herd runs one C# brain (thirst, hunger, fatigue, fear, awareness). Individual animals are rendered as separate meshes following the herd state with natural variation. No individual AI per animal except for solitary species. See Section 13 for full detail.
- **Currency:** South African Rand (ZAR)
- **Starting Year:** 2024
- **Save System:** JSON-based, full state serialisation

### Visual Direction Notes
The low poly 3D style is a deliberate creative choice — not a budget compromise. It allows the game to:
- Run on a wide range of hardware without sacrificing atmosphere
- Convey the vast scale of the African landscape without photorealistic asset overhead
- Age well — stylised games do not date the way realistic games do
- Focus the player on systems and land, not graphical fidelity

Colour and light carry the emotional weight. Dawn light, midday heat, the golden hour before sunset, the blue-grey of a coming storm — these are achieved through lighting and atmosphere, not texture resolution.

---

## 11. CURRENT DEVELOPMENT STATE

Version 0.3 (logic layer) + early 3D prototype. Systems scaffolded in GDScript:
- GameState (central data store)
- WorldMap (currently replaced by 3D procedural terrain)

3D prototype includes:
- Third-person player character (GLB imported)
- Bakkie with enter/exit and driving mechanics
- Procedural terrain with noise-based height and colour
- Procedural tree and shrub spawner using raycasts
- Basic world environment and lighting

---

## 12. GUIDING QUESTIONS FOR EVERY FEATURE

Before adding any mechanic, system, visual, or piece of content, ask:

1. Does this make the land feel more alive?
2. Does this give the player a meaningful choice with a real consequence?
3. Does this fit a specific, authentic South African context?
4. Could two players make opposite decisions here and both be right?
5. Does this push the player toward one "correct" path — and if so, can it be redesigned so it does not?
6. Does this serve the story of a person becoming who they were always meant to be?

If a feature fails questions 1, 2, and 4, reconsider it.

---

*This document is a living reference. It should be updated as the vision sharpens, but its core principles should remain stable. When in doubt, return to the North Star.*

---

## 13. PERFORMANCE ARCHITECTURE — Rules for Scale

*This section is binding for all technical decisions. Every new system, asset, or mechanic must be evaluated against these rules before implementation. When in doubt, refer here first.*

---

### 13.1 Understanding the Three Bottlenecks

The game runs on three hardware resources. Each has a different job and a different failure mode. Knowing which one a system strains determines how to build it correctly.

**CPU (Central Processing Unit)**
The CPU runs your game logic — AI decisions, simulation calculations, pathfinding, physics, and anything written in C# or GDScript. The enemy of the CPU is *quantity of work per frame*. The more individual objects running logic every frame, the slower everything gets. A herd of 200 buffalo each running their own pathfinding and sensory AI every frame will destroy frame rate on any hardware. The same herd running one shared brain with 200 bodies following its result costs a fraction of that. C# was chosen as the primary language specifically because it processes CPU-bound work significantly faster than GDScript — roughly 3 to 10 times faster on tight loops and heavy math. This matters at the scale this game operates.

**GPU (Graphics Processing Unit)**
The GPU draws things on screen. It is extremely fast at drawing the *same thing* many times but gets stressed by *variety* — many different meshes, many different materials, many separate draw calls. The enemy of the GPU is draw call count, not polygon count. One thousand trees using one mesh and one material equals one draw call. One thousand trees each with a unique mesh equals one thousand draw calls. Low poly reduces the data per draw call but does not reduce draw call count. That requires instancing. The GPU is largely unaffected by whether your logic runs in C# or GDScript — it receives the same rendering commands either way.

**RAM (Random Access Memory)**
RAM holds everything currently loaded — meshes, textures, scene nodes, audio, and all in-memory data. The enemy of RAM is *loading too much at once*. A 10,000 hectare world cannot live in RAM simultaneously. Only the portion near the player can. Everything beyond that must either be unloaded entirely or exist only as lightweight C# data — a struct or class instance, not a scene node. Scene nodes are expensive. Data objects are cheap.

---

### 13.2 The Fundamental Rule — Simulate vs Render

**The most important architectural decision in this game:**

> Everything in the world is simulated at all times. Only what is near the player is rendered.

This split is what makes a living 10,000 hectare world possible. It applies to every system:

| System | Simulated (always, C# data) | Rendered (proximity only, scene nodes) |
|---|---|---|
| Animal herds | `Herd` C# class in AnimalSystem | Individual animal meshes via MultiMesh |
| Flora | `FloraEntry` struct in FloraSystem | `MultiMeshInstance3D` per species per chunk |
| Grass | Coverage float per terrain sector | Blade meshes animated by wind shader |
| Insects | Colony data (size, food, location) | Particle system or shader near player |
| Staff | `StaffMember` C# class in StaffSystem | Character node only when near player |
| Weather | State flags in GameState | Visual effects, sky colour, particles |
| Rivers | Water level float per river segment | Shader parameter on terrain mesh — no new geometry |
| Dams | Capacity float (0–1) in WaterSystem | Water plane mesh scaled by capacity parameter |
| Fire | FireEvent C# class — origin, spread rate, affected zones | Particle systems + terrain colour overlay near player |
| Micro-wildlife | Population density floats per zone in EcologySystem | Billboard sprites + particle systems (ants, birds, hares) spawned near player |
| Roads | Road network graph in WorldData | Terrain texture blend — no separate mesh |
| Buildings | BuildingData C# class — type, condition, chunk ID | Scene node loaded with chunk, freed on unload |

A buffalo herd 3km from the player is real — it grazes, affects grass coverage, breeds, ages, triggers events, and can be poached. It is not drawn. It does not exist as a node in the scene tree. It is a C# object. When the player drives within 150m in the bakkie, the render layer reads the herd data and spawns individual animal meshes. When the player leaves, those meshes are freed from memory. The C# data object remains alive and continues to be simulated.

---

### 13.3 The Herd Instinct Model — How Animals Work

Animals in this game are rendered individually but think collectively. This is both the performance solution and the authentic biological model.

**The Herd Brain (one C# class per herd)**

Every herd of social animals has a single shared brain that drives all behaviour. This brain tracks:

- **Thirst** — rises over time, triggers movement toward the nearest known water source when critical
- **Hunger** — rises when grass coverage in current zone is low, triggers movement to better grazing
- **Fatigue** — rises with movement and heat, triggers rest behaviour during midday and night
- **Fear level** — spikes when a predator is detected (sight, sound, or scent depending on wind), triggers flight response
- **Awareness radius** — varies by species; impala are extremely alert, buffalo less so
- **Current state** — one of: Grazing, Moving, Drinking, Resting, Fleeing, Alerting

The brain makes one decision. All animals in the herd execute it.

**Individual Animals (C# structs, members of the herd)**

Each animal in the herd has its own data:
- World position (offset from herd centre point)
- Age, sex, health, genetics
- Visual mesh reference and current animation state

Individual animals do not make decisions. They follow the herd state with biological variation:
- Slightly different movement speeds (young animals slower, dominant males at the front)
- Slightly different positions (spread formation while grazing, tight cluster while fleeing)
- Slight reaction lag (the signal ripples through the herd, not all animals bolt at exactly the same frame)
- Natural wander offset from the herd centre during grazing (animals spread out, don't stack)

This variation is produced by simple math offsets driven by each animal's unique ID and age — not by individual AI. It produces the visual of a living herd from almost zero extra CPU cost.

**Solitary Species — Individual AI**

Species that do not form herds run their own individual state machines: leopard, caracal, aardvark, honey badger, pangolin, crocodile. These animals exist in far lower numbers and the cost of individual AI is acceptable.

**Predator Packs**

Wild dog packs and lion prides use a modified herd brain — a pack brain — but with cooperative hunting logic added. When hunting, the pack brain coordinates flanking behaviour through simple role assignment (one driver, two flankers), not complex pathfinding.

---

### 13.4 The LOD Ladder — Behaviour and Visual

Every herd and every animal has LOD states for both what it does and how it looks. These are managed separately but trigger at similar distances.

**Behaviour LOD**

| Distance from Player | Herd Brain Activity |
|---|---|
| 0 – 150m | Full: thirst, hunger, fatigue, fear, awareness, animation states all active |
| 150 – 500m | Reduced: state changes only (no per-frame awareness checks), animation simplified |
| 500m – 2km | Minimal: position updated once per second, no state changes except major events |
| 2km+ | Simulation only: monthly tick in AnimalSystem, no world position tracked |

**Visual LOD**

| Distance from Player | Render State |
|---|---|
| 0 – 100m | Full mesh per animal, full skeletal animation, full detail |
| 100 – 350m | Full mesh, simplified animation (fewer bone updates per frame) |
| 350m – 800m | Billboard impostor — flat sprite facing camera, no animation |
| 800m+ | Not rendered — behaviour LOD handles everything |

Godot's `GeometryInstance3D` LOD distance settings and `VisibilityNotifier3D` handle visual LOD transitions automatically once configured. Behaviour LOD is managed by the AnimalSystem tick rates.

---

### 13.5 Instancing Rules — Non-Negotiable

Any object that appears more than once in the world must use `MultiMeshInstance3D`. This is not optional.

- All tree species → one `MultiMeshInstance3D` per species per active chunk
- All animals of the same species in a visible herd → one `MultiMeshInstance3D` per species
- Grass blades → one `MultiMeshInstance3D` per chunk, animated by wind shader on GPU
- Rock formations → instanced
- Fence posts → instanced
- Shrubs and ground cover → instanced

Variety is the enemy of the GPU. Where visual variety is needed — different animal sizes, health states visible in coat colour, tree health — achieve it through **per-instance shader parameters**, not through unique meshes. One acacia mesh. Shader parameters control whether it looks lush or drought-stressed.

---

### 13.6 Chunk Rules — What Lives Where

The `WorldChunkStreamer` is the gatekeeper for all visual content in the 3D world. These rules govern ownership:

- Every chunk owns its terrain tiles, its `MultiMeshInstance3D` nodes for trees, grass, and rocks
- When a chunk loads, it reads the relevant slice of `FloraSystem` data and populates its MultiMesh instances
- When a chunk unloads, all its scene nodes are freed — the underlying C# data is untouched and continues to be simulated
- Herds are never chunk-owned — they move across chunk boundaries. They are proximity-owned by the render system
- Buildings and infrastructure are chunk-owned and persist between loads
- Target: no more than 9 active chunks at once (3×3 grid around player). Expand only after profiling confirms headroom

---

### 13.7 Asset Rules — GLB and Texture Guidelines

**Meshes (GLB)**
- All tree, animal, rock, and vegetation meshes must be GLB format
- Polygon targets: trees 200–800 triangles, animals 400–1200 triangles, rocks 50–200 triangles
- Low poly is a technical requirement, not only an aesthetic one — polygon count directly affects GPU cost per draw call
- One base mesh per species. Scale and colour variation is handled by the shader
- No LOD geometry baked into the GLB — Godot's built-in LOD and the billboard fallback handle this

**Textures**
- All flora and terrain textures must be combined into texture atlases — one atlas per biome region
- Maximum atlas resolution: 2048×2048 pixels
- Avoid unique materials per object — every unique material is an additional draw call
- Animal coat variation (different genetics, health states) is achieved through shader colour parameters, not separate textures

**Audio**
- All ambient sounds use `AudioStreamPlayer3D` with distance attenuation — never global
- Maximum 10 simultaneous 3D audio sources near the player
- Animal calls (lion at dusk, hyena at night, impala alarm snort) are triggered by the event system or herd state changes — not by always-on proximity nodes
- Music in the bakkie attenuates with distance from the vehicle using a single `AudioStreamPlayer3D` on the bakkie node

---

### 13.8 C# Code Performance Rules

- Never run heavy simulation logic in `_Process()` (called every frame). Simulation belongs in the TimeSystem tick — daily or monthly. `_Process()` is for visual updates only
- Use C# **structs** for individual animal data (value types stored contiguously in memory, cache-friendly for iteration)
- Use C# **classes** for herd brains and system managers (reference types, one instance per herd)
- Avoid LINQ in hot paths (per-frame or per-animal loops) — use direct array iteration
- Cache all node references in `_Ready()` — never call `GetNode()` inside loops
- Simulation systems (AnimalSystem, FloraSystem, BreedingSystem) must never instantiate scene nodes — that is exclusively the render layer's responsibility
- Use Godot's `Thread` class or C# `Task` for chunk generation to avoid frame hitches — never generate chunks on the main thread

---

### 13.9 The Performance Budget (Target Hardware: Mid-Range PC, ~2021)

| Metric | Target |
|---|---|
| Draw calls per frame | < 300 |
| Active `MultiMeshInstance3D` instances visible | < 6,000 total across all types |
| Herds running full behaviour AI | < 8 simultaneously |
| Individual animal meshes rendered | < 300 simultaneously |
| Active particle systems (fire, insects, ambient) | < 8 simultaneously |
| Active 3D audio sources | < 12 |
| RAM usage during gameplay | < 1.5 GB |
| Chunk load time | < 80ms (background thread) |
| Monthly simulation tick (all systems) | < 20ms total |

These are warning lines, not hard ceilings. Exceed them during development and investigate before adding more content.

**Note on the revised draw call target:** The original budget of 200 draw calls was set before rivers, dams, fire, micro-wildlife, home building components, and ambient particle systems were added to the scope. The revised target of 300 remains achievable because the new systems are implemented correctly:
- Rivers and dams are shader parameters on existing terrain geometry — zero additional draw calls
- Fire is 1–2 particle system draw calls regardless of fire size
- Micro-wildlife (birds, hares, insects) share MultiMesh instances per species — a flock of 40 queleas is one draw call
- Home and lodge building pieces are instanced per component type
- Roads are terrain texture blends — no separate mesh, no additional draw calls

The 300 target is still aggressive. Any system that naively spawns unique nodes per-entity will blow through it immediately. The instancing rules in Section 13.5 are non-negotiable and become more important, not less, as scope grows.

---

### 13.10 What Low Poly Actually Buys You

Low poly reduces **VRAM per mesh** and **GPU work per draw call**. It does not:
- Reduce draw call count — instancing does that
- Reduce CPU load from simulation logic — C# architecture and tick design do that
- Reduce RAM pressure from too many loaded nodes — chunking does that

Low poly is one necessary layer in a complete performance strategy. Protect it. Do not add high-polygon assets because a machine can handle it today — the baseline is a mid-range PC from a few years ago in a rural area. That is the player this game is built for.

---

### 13.11 Why C# and Not GDScript

C# compiles to native code via the .NET runtime. On CPU-bound work — iterating over hundreds of animal structs, running genetics calculations, generating terrain chunks — it runs roughly 3 to 10 times faster than interpreted GDScript. At the scale of this game, with a living world across thousands of hectares, that headroom is not optional.

GDScript remains acceptable for: UI logic in `HUD`, event-driven systems like `EventSystem` and `TimeSystem`, and any script that runs infrequently (once per month, once per decision). These do not benefit meaningfully from C# and can be migrated later if profiling reveals they are bottlenecks.

Mixing both languages in the same Godot 4 project is fully supported. C# systems expose their data through `GameState` which GDScript nodes can read normally.

---

## 14. PLAYER EXPERIENCE DESIGN — Tension, Agency, Clarity, Progression, Resolution

*This section defines how the player should feel during play — not what mechanics exist, but what the player experiences emotionally and cognitively at every stage. Every system, feature, and event in the game should be evaluated against these five dimensions. If a new mechanic scores zero across all five, reconsider it.*

---

### 14.1 Tension — What Pushes Back

The land does not want to be a game reserve. It does not want to be anything. It is indifferent. The player's ambition is what creates the tension — the gap between what you want this place to become and what it actually is. Africa is the antagonist. Not a villain. Not a punishing game master. Simply a force that does not care about your plans.

That tension is expressed through the following pressure systems. None are arbitrary. All are grounded in the real experience of South African farm ownership.

**Financial Reality — The Primary Antagonist**

The starting farmer is asset-rich and cash-poor. Land has value, but land does not pay a salary. The borehole breaks and the repair costs R80,000. Two animals die in transit and that is R40,000 walking out the door. The bank loan is approved, but the interest accumulates before the first sale. Every other pressure in the game is ultimately a financial problem downstream — disease, drought, staff, infrastructure — all of it arrives at the bank account eventually. This is the most universal pressure. It is always on.

*Design rule: The player should never feel that money problems are unfair. They should feel that money problems are the honest consequence of running a real operation on thin margins.*

**The Holy Trinity of Constraint — Time, Money, Knowledge**

At any moment the player is short on at least two of these. You can fix the broken pump yourself if you have the time and the knowledge, but not the money. You can hire someone if you have the money but not the time to supervise. You can learn if you have the time but not the money or the knowledge yet. These three variables interact with every decision and every problem. The game never gives the player all three simultaneously in the early years. That scarcity is the difficulty.

**The Land Fighting Back**

Neglect has visible, compounding consequences. Invasive species — lantana, bugweed, jointed cactus — spread if not managed. Overgrazing degrades the grass layer and the soil beneath it. A borehole that ran reliably for two years can run dry in a drought year. Fences need constant maintenance or they fail at the worst moment. The land does not punish dramatically. It degrades quietly, and recovery is slow and expensive. The player who ignores the land discovers this. The player who tends it discovers the opposite.

**Theft, Poaching, and Security**

Not dramatic raids. A slow, demoralising drain. Stock theft is common. A sheep goes missing. Then two. Someone cuts the eastern fence in the night. Diesel is siphoned from the tractor. Copper wire disappears from the solar installation. These are not events designed to punish the player — they are the background reality of rural South Africa. The player who invests in security infrastructure, loyal staff, and good community relations reduces this pressure measurably. The player who does not will lose things steadily.

*Design rule: Security investment must have measurable, visible effect on theft and poaching rates. If the player cannot reduce this pressure through smart decisions, it becomes arbitrary punishment, not tension.*

**Bureaucracy and Compliance**

DFFE permits for game movement. CITES documentation for regulated species. Environmental impact assessments before building near a wetland. Labour law compliance for staff. These are not obstacles placed to frustrate the player — they are the authentic texture of operating in South Africa. A permit application takes four months. The animals you want to buy are ready now. The plan sits frozen. The game does not mock the player for encountering this. It simply makes it real.

**Isolation**

A farm is far from everything and everyone qualified. The nearest vet who understands buffalo is in Mokopane. The nearest electrician who knows solar systems is two hours away and charges accordingly. When something breaks, the distance matters. This creates genuine dependency on the relationships the player has built — Oom Fasie, local community contacts, trusted staff — and makes those relationships valuable beyond their mechanical function.

**Staff Reality**

Your best ranger, the one you trained for eight months and trusted completely, leaves for a better-paying operation in the Waterberg. You are back to doing it yourself. Staff fail to arrive during lambing season. The most reliable person on the farm has a family crisis that takes them away for three weeks. These are not betrayals. They are people living their lives. The player who has built a team with depth and loyalty weathers these moments. The player who has one indispensable person and no redundancy does not.

**External Forces — Responsive, Not Random**

Government disease control zones, market price fluctuations, and neighbouring farm pressures are real and outside the player's control. However, these forces must always be *responsive to player decisions*. The player who invested in proper biosecurity infrastructure, veterinary relationships, and species diversification should have measurably different outcomes when a disease event hits than the player who did not. External pressure is not random punishment. It is a force that interacts with the player's accumulated choices. The player always has a variable in every equation — even when they cannot control the outcome entirely.

---

### 14.2 Agency — The Player Is Always Authoring

The player of BasterBoer is an author, not an executor. Every meaningful decision they make is an act of authorship over their land and the story it tells.

**Spatial Freedom Is Absolute**

There is no prescribed layout. The homestead goes where the player puts it. Camps, lodges, staff quarters, water infrastructure, feeding stations, bomas, hides — all placed freely on the terrain. Internal boundaries are drawn by the player. The farm's shape emerges from their decisions, not from a template. This spatial freedom is one of the game's most important forms of agency and must never be compromised by systems that override or invalidate placement choices arbitrarily.

**All Business Models Are Valid**

The player can stock any animal available to their region, even if the climate is not optimal — that is a consequence they choose to manage, not a locked option. They can have livestock, game, or both in any combination. They can bring in a registered professional hunter and let that person run hunting operations, or they can acquire their own PH qualifications and operate independently. They can pursue photographic tourism, trophy hunting, live game sales, livestock, carbon credits, research partnerships, or any blend. No path is locked. No path is recommended.

**Consequences Belong to the Player**

Agency is only meaningful if choices have real consequences — and those consequences belong to the player, not to the game. The player who stocks species poorly suited to their climate manages that difficulty. The player who builds the lodge in the wrong spot lives with the sightlines. The player who overgrazes their eastern block watches that block degrade. None of these are punishments. They are the honest outcome of decisions freely made. The game's job is to make consequences visible, legible, and connected to the choice that caused them — not to protect the player from them.

**External Pressure Amplifies Agency, Not Replaces It**

When external forces arrive — drought, disease, government decisions — they must interact with the player's accumulated choices, not override them. The player who prepared has better options. The player who did not has harder options. Both still have options. The moment the player feels that the game is deciding outcomes for them regardless of what they did, agency collapses. Every pressure system must have a player-side variable.

---

### 14.3 Clarity — The Player Always Knows What Needs Attention

BasterBoer has no objectives screen. No quest log. No achievement checklist. And yet the player should never feel lost about what to do *right now*. These two things are not in conflict. The clarity comes from the farm itself.

**The Living Problem Queue**

At any moment, the farm is generating a small number of visible, present problems with a time dimension. The water trough in Camp 3 is running low. The eastern fence has a damaged section the player's staff flagged two days ago. The grass coverage in the northern block is thinning after six weeks without rain. A staff member is overdue for their monthly wage review. These are not objective markers. They are the farm communicating its state. The player reads the farm — through walking it, driving it, and through the information their staff surface — and responds to what they find. The clarity is ambient, not instructional.

*Design rule: The player should never need to open a management screen to know what needs doing today. The world should tell them.*

**The Vision Board Provides Long-Term Clarity**

Short-term clarity comes from the farm's present state. Long-term clarity comes from the Vision Board — what the player is building toward. The gap between the current farm and the vision board is always visible. That gap is the game. The player is never confused about the long-term direction because they authored it themselves.

**AfriBees Magazine as Soft Guidance**

AfriBees arrives monthly. It carries seasonal advice, notices about upcoming permit windows, market price movements, and flavour content about what other operations are doing. It does not tell the player what to do. It surfaces information relevant to decisions the player might be considering. A player wondering whether to invest in a waterhole expansion reads an AfriBees piece on drought-year water strategy and has better information for their decision. The magazine is clarity without instruction.

**Oom Fasie as Human Signal**

Oom Fasie arrives when something is worth noticing. Not always when the player wants him. Not always with useful timing. But when he shows up and says something, it is worth listening to. He is not a tutorial character with a dialogue tree. He is a person who notices things. His presence is a gentle signal that something in the player's situation is worth attending to, without ever telling the player what to do about it.

---

### 14.4 Progression — The Farm Proves the Journey

Progression in BasterBoer is not tracked by a number. It is visible in the land, felt in the relationships, and heard in the world. The farm at year five should look, sound, and operate differently from the farm at year one — not because a progress bar filled, but because the player made ten thousand small decisions that accumulated into something real.

**The Land as Progress Canvas**

Ecological health is the most honest record of the player's journey. A neglected block of veld looks like it. A well-managed block looks like that too. The grass is actually greener, the soil colour has changed, the birdlife is audibly richer, the waterhole fuller and surrounded by more animal sign. The player who drives through a section they worked on for two years sees the consequence of that work in the world without needing a dashboard to confirm it. This visual, spatial record of progress is more satisfying than any meter.

**Herd Genetics as a Slow Arc**

A breeding programme is one of the game's most powerful long-form progress systems. The player who invests in selective breeding over years will see their animals change — heavier bodies, better horn development, healthier coats, higher demand from buyers. This is progress measured in generations, not levels. It requires patience, planning, and the willingness to make decisions whose payoff is years away. When the first animal from a carefully managed bloodline sells for significantly more than anything the player has sold before, that is progression that was earned.

**Staff as Living Progress**

A new ranger knows nothing. A ranger who has worked on this farm for three years knows everything about it. They start bringing the player information they did not ask for. They take initiative in situations that previously required instruction. They make fewer mistakes. Their loyalty is visible — they do not leave when a competitor offers slightly more money. The staff the player has grown are a record of investment and time, and they make the operation work better in ways that are felt rather than displayed.

**Reputation as an Outward Mirror**

The world's response to the player's farm changes over time. Early on, no one calls. There are no hunting clients asking for space, no researchers making enquiries, no government recognition. Over years of good management, the reputation builds. The right people start to hear about the operation. Boer van die Jaar is not an unlock — it is recognition that arrives when it is genuinely warranted, from the community, in character. It means something because the player lived the years that preceded it.

**The Three Stages — Properly Distinguished**

The early, mid, and late game are not phases in a tutorial flow. They are genuinely different experiences:

- *Early* — Survival and learning. Every rand matters. Every decision has an immediate visible consequence. The farm is rough and the vision feels far away. Problems are simple but unforgiving.
- *Mid* — Growth and delegation. The player can no longer do everything themselves. Staff must be trusted. Competing priorities pull in multiple directions simultaneously. The farm starts to look like something.
- *Late* — Legacy and scale. The operation has momentum. The complexity is deep. The player is managing a living ecosystem, a staff of people, a reputation, and a set of relationships that extend far beyond the fence line. The farm is the vision, or close to it. New challenges arrive at the level of stewardship, not survival.

---

### 14.5 Resolution — The Moments That Let the Player Exhale

A game without a win condition still needs resolution. Not at the macro level — the farm is never finished, the journey never ends — but at the micro level, where chapters close and the player feels the weight of what just happened. BasterBoer builds resolution through three structures.

**The Season as Natural Resolution Unit**

South Africa's farming year has a real rhythm: the rains, the dry season, the hunting season, the calving period, the fire risk months. Each season opens with its own pressures and closes with a reckoning. Did the herd hold through the dry season? Did the hunting season bring clients worth having back? Did the first rains arrive before the grass gave out? The season's end is not marked with a screen or a score. It is marked by the world changing — the first rains arrive, the temperature drops, the veld turns green over two weeks, the character sits at the boma fire as the day ends. The player exhales. The next season begins.

**The Event as a Story With an Ending**

Discrete situations develop over time and conclude — they are not instant events that fire and disappear. A poaching incident unfolds over several days: tracks found, a decision about how to respond, an outcome that is either a resolution or a failure. A three-year drought builds slowly, forces hard decisions about which animals to hold and which to sell, and then — eventually — the rain comes or it does not. A client arrives, hunts, and either leaves satisfied and books again or doesn't come back. Each of these is a story with a beginning, a middle, and an end. The resolution is built into the structure.

**The Private Win — The Game Noticing**

Section 3.2 describes a player who keeps a small herd of cattle alive through three years of drought and calls it a profound win. That win is only fully felt if the world acknowledges it. Not with a trophy. Not with a score. The game notices in character: Oom Fasie shows up and says something quietly to the point. A neighbouring farmer phones. AfriBees runs a short piece about operations that held through the drought years, and the player's farm is mentioned by name. The community noticed. The world the player built noticed. That acknowledgment — small, human, earned — is the resolution. The player should feel seen by the farm they chose to care for.

---

## CHARACTERS & WORLD DETAILS (Living Notes)

**Oom Fasie** — Die mal ou wat rondom die draai bly, in 'n houthuisie met 'n mercedes. Sy troeteldier is sy vlakvark. Hy help jou op random tye uit, nie altyd wanneer jy dit nodig het nie. Hy is 'n all rounder en doen alles basies perfek. Hy wil altyd betaal word maar gee partykeer vir jou 'n geskenk. He is not a tutorial character. He is a person.

**The Player's Dog** — The player should have the option to have a dog. It is not a mechanic. It is a companion.

**AfriBees Magazine** — A fictional monthly in-game publication delivered to the homestead with tips, event notices, and flavour content. The equivalent of Landbouweekblad arriving in the post. Serves as a soft tutorial layer without breaking immersion.

**Boer van die Jaar** — An award the player can receive. Not tied to a points system. Recognised by the community. Means something because it is earned, not unlocked.

**Supporting Characters** — Throughout the game, characters reveal themselves — some help, some hinder. They are not quest givers. They are people with their own agendas who cross the player's path at meaningful moments. Their timing is not always convenient. That is intentional.
