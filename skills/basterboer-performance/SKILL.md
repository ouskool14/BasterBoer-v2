# SKILL: BasterBoer Performance Architecture

## When to use this skill
Read this file before writing any system, asset pipeline, or rendering code.
These rules are binding. A feature that violates them cannot ship — it must be
redesigned, not optimised later. "We'll fix it when it's slow" is not an option
on a mid-range PC target.

**Target hardware:** Mid-range PC circa 2021. Intel i5/Ryzen 5, 16GB RAM,
GTX 1060 / RX 580 class GPU. This is the rural South African player.
Every decision must serve this machine.

---

## 1. The Three Bottlenecks — Know Which One You're Hitting

Before writing any system, identify which hardware resource it strains.

### CPU — Killed by quantity of work per frame
- Runs all game logic: AI, simulation, pathfinding, physics
- C# is 3–10× faster than GDScript on tight loops — use it for all simulation
- Enemy: 200 animals each running individual AI every frame
- Solution: One herd brain, 200 bodies following its result

### GPU — Killed by draw call count, not polygon count
- Draws things on screen — fast at drawing the same thing many times
- Enemy: 1,000 trees each with unique meshes = 1,000 draw calls
- Solution: 1,000 trees sharing one mesh = 1 draw call (instancing)
- Low poly reduces data per draw call but NOT draw call count

### RAM — Killed by loading too much at once
- Holds all currently loaded meshes, textures, nodes, audio, data
- Enemy: Loading a 10,000 hectare world simultaneously
- Solution: Only the area near the player is loaded as scene nodes.
  Everything else is lightweight C# data objects.

---

## 2. Performance Budget — Hard Limits

These are warning lines. Exceed them → investigate before adding more content.

| Metric | Limit |
|--------|-------|
| Draw calls per frame | **< 300** |
| MultiMeshInstance3D instances visible | **< 6,000 total** |
| Herds running full behaviour AI | **< 8 simultaneously** |
| Individual animal meshes rendered | **< 300 simultaneously** |
| Active particle systems | **< 8 simultaneously** |
| Active 3D audio sources | **< 12** |
| RAM during gameplay | **< 4 GB** |
| Chunk load time | **< 80ms (background thread)** |
| Monthly simulation tick (all systems) | **< 20ms total** |

### How the 300 draw call budget is used:
- Terrain chunks (3×3 grid): ~9 draw calls
- Tree species (MultiMesh per species per chunk): ~30–50 draw calls
- Grass (MultiMesh per chunk): ~9 draw calls
- Animal herds (MultiMesh per species): ~10–20 draw calls
- Rocks/shrubs/ground cover: ~15 draw calls
- Buildings and infrastructure: ~20–40 draw calls
- Fence system (poles, sticks, wire per chunk): ~15–30 draw calls
- Water/river shaders: 0 additional (shader params on terrain)
- Fire (1–2 particle systems): ~2 draw calls
- UI: ~20–30 draw calls
- Roads: 0 additional (terrain texture blend)
- Ambient wildlife (MultiMesh per species): ~10 draw calls
- Headroom: ~60–100 draw calls

Any single naive system that spawns unique nodes per entity will consume
this entire budget alone. Do not do it.

---

## 3. Instancing Rules — Non-Negotiable

Any object that appears more than once uses `MultiMeshInstance3D`. No exceptions.

```csharp
// WRONG — unique node per tree
foreach (var pos in treePositions) {
    var treeNode = treeScene.Instantiate<Node3D>(); // hundreds of draw calls
    treeNode.Position = pos;
    AddChild(treeNode);
}

// CORRECT — one MultiMesh for all trees of this species
var mm = new MultiMesh {
    Mesh = acaciaMesh,
    TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
    InstanceCount = treePositions.Count
};
for (int i = 0; i < treePositions.Count; i++)
    mm.SetInstanceTransform(i, new Transform3D(Basis.Identity, treePositions[i]));
AddChild(new MultiMeshInstance3D { Multimesh = mm });
// Result: 1 draw call for all acacia trees in this chunk
```

### Instancing applies to:
- All tree species (one MultiMesh per species per active chunk)
- All animals of same species in a visible herd
- Grass blades (one MultiMesh per chunk, wind shader on GPU)
- Rock formations
- Fence posts and droppers (see FenceSystem.cs)
- Shrubs and ground cover
- Ambient wildlife of same species (40 queleas = 1 draw call)
- Building component types (walls, roofs of same material)

### Visual variety without unique meshes:
Use per-instance shader parameters — NOT separate meshes.

```csharp
// One acacia mesh. Shader controls drought stress, health, size variation.
mm.UseCustomData = true;
mm.SetInstanceCustomData(i, new Color(
    healthNormalised,    // r = health (affects colour)
    sizeVariation,       // g = scale offset
    droughtStress,       // b = drought appearance
    0f
));
// The shader reads these values and varies appearance.
// Still 1 draw call.
```

---

## 4. C# Code Performance Rules

### Never in _Process() — simulation belongs in TimeSystem ticks
```csharp
// WRONG — runs every frame, destroys CPU budget
public override void _Process(double delta) {
    foreach (var herd in AnimalSystem.AllHerds) {  // never
        herd.UpdateHunger(delta);
    }
}

// CORRECT — runs once per game-day
public void OnDailyTick() {
    foreach (var herd in _herds) {
        herd.Brain.UpdateHunger(_dayDeltaHours);
    }
}
```

### Use structs for per-entity data
```csharp
// CORRECT — value type, stored contiguously, cache-friendly iteration
public readonly struct AnimalStruct {
    public readonly int Id;
    public readonly float Age;
    public readonly float Health;
    public readonly GeneticProfile Genetics;
}

// Keep the array of structs, not array of class references
private AnimalStruct[] _animals; // good: cache-friendly
private Animal[] _animals;       // bad: array of heap pointers, cache misses
```

### No LINQ in hot paths
```csharp
// WRONG — LINQ allocates on every call, GC pressure in tick loops
var thirstyHerds = _herds.Where(h => h.Brain.Thirst > 0.8f).ToList();

// CORRECT — direct iteration, zero allocation
for (int i = 0; i < _herds.Count; i++) {
    if (_herds[i].Brain.Thirst > 0.8f) ProcessThirstyHerd(_herds[i]);
}
```

### Cache all node references
```csharp
// WRONG
public override void _Process(double delta) {
    GetNode<Label>("HUD/Balance").Text = FormatZAR(balance); // GetNode every frame
}

// CORRECT
private Label _balanceLabel;
public override void _Ready() { _balanceLabel = GetNode<Label>("HUD/Balance"); }
public override void _Process(double delta) { _balanceLabel.Text = FormatZAR(balance); }
```

### Never allocate in tick loops
```csharp
// WRONG — creates garbage every tick
public void OnDailyTick() {
    var nearbyWater = new List<WaterSource>(); // allocation in hot path
    foreach (var herd in _herds) { ... }
}

// CORRECT — reuse a cached list
private readonly List<WaterSource> _nearbyWaterCache = new();
public void OnDailyTick() {
    _nearbyWaterCache.Clear(); // reuse, no allocation
    foreach (var herd in _herds) { ... }
}
```

### Chunk generation on background thread only
```csharp
// WRONG — generates on main thread, causes frame hitch
public void LoadChunk(ChunkCoord coord) {
    GenerateChunkData(coord); // blocks main thread for 80ms+
    BuildChunkVisuals(coord);
}

// CORRECT
public void LoadChunk(ChunkCoord coord) {
    Task.Run(() => {
        var data = GenerateChunkData(coord); // background thread
        Callable.From(() => BuildChunkVisuals(data)).CallDeferred(); // back to main
    });
}
```

---

## 5. Asset Rules

### Polygon targets
| Asset Type | Triangle Budget |
|-----------|----------------|
| Animals | 400–750 triangles |
| Trees | 200–800 triangles |
| Rocks | 50–200 triangles |
| Buildings (individual component) | 100–400 triangles |
| Fence pole / dropper | 30–80 triangles |

Low poly is a technical requirement, not just an aesthetic. More polygons per
draw call means less headroom for draw call count.

### Textures
- All flora and terrain textures combined into atlases — one atlas per biome
- Maximum atlas resolution: 2048×2048
- No unique material per object — each unique material = one more draw call
- Animal coat variation via shader colour parameters, NOT separate textures

### Audio
- All ambient sounds: `AudioStreamPlayer3D` with distance attenuation
- Maximum 12 simultaneous 3D audio sources
- Animal calls triggered by EventSystem or herd state — not always-on proximity nodes
- Bakkie music: single `AudioStreamPlayer3D` on bakkie node, attenuates with distance

---

## 6. What Low Poly Does NOT Fix

Low poly reduces VRAM per mesh and GPU work per draw call. It does NOT:
- Reduce draw call count → instancing does that
- Reduce CPU load from simulation → C# tick architecture does that
- Reduce RAM pressure from too many nodes → chunking does that

Low poly is one layer. It requires all the other layers to function.

---

## 7. The Simulate vs Render Draw Call Impact

These systems cost ZERO additional draw calls because they are implemented correctly:

| System | Why zero cost |
|--------|--------------|
| Rivers | Shader parameter on terrain mesh — no new geometry |
| Dams | Water plane mesh scaled by capacity parameter |
| Roads | Terrain texture blend — no separate mesh |
| Fire | 1–2 particle systems regardless of fire size |
| Micro-wildlife flocks | MultiMesh per species — 40 birds = 1 draw call |
| Building health/damage | Shader parameter on existing mesh |

If your new system cannot be described in this table, explain why before building it.

---

## 8. Pre-Implementation Checklist

Before writing a single line of a new system:

- [ ] Which of the three bottlenecks does this touch? (CPU / GPU / RAM)
- [ ] Does it use MultiMesh for any repeated visual element?
- [ ] Does simulation logic run in TimeSystem ticks, not `_Process()`?
- [ ] Does it add < 10 draw calls at full load?
- [ ] Are per-entity data types structs, not classes?
- [ ] Is there a chunk unload strategy that frees scene nodes?
- [ ] Does it stay within the < 20ms monthly tick budget?
- [ ] Can it be disabled entirely on the simulation side without touching the render layer?
