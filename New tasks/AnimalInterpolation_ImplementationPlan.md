# Animal Movement Interpolation — Implementation Plan
## BasterBoer v2 | AnimalRenderer + HerdBrain Smoothing Layer

---

## CONTEXT FOR THE IMPLEMENTING MODEL

You have access to the full project folder. The relevant files are:

- `AnimalRenderer.cs` — the render layer; builds MultiMesh transforms each frame from herd data
- `HerdBrain.cs` — the simulation layer; drives herd center position, movement direction, and individual animal offsets
- `AnimalStruct.cs` (or equivalent) — the per-animal data struct holding `WorldPosition` (an offset from herd center), `UniqueId`, and `MeshInstanceId`
- `BehaviourLODHelper.cs` (or equivalent) — defines LOD update intervals
- `TerrainQuery.cs` / `WorldChunkStreamer` — provides `GetHeight(x, z)` heightmap sampling

**Do not modify HerdBrain.cs or any simulation system.** All changes are confined to the rendering layer. The simulation is authoritative. The renderer is a consumer of simulation data that adds visual smoothing only.

---

## ROOT CAUSES BEING FIXED

### Cause 1 — LOD-lumped herd center movement
`HerdBrain.Tick()` accumulates delta time and only calls `UpdateBehavior()` when its interval fires. At Medium LOD this is approximately once per second. When it fires, `CenterPosition` moves by `speed × deltaTime` in a single step. The renderer reads this new position immediately and the entire herd jumps forward.

### Cause 2 — Individual offset reassignment snap
`UpdateIndividualAnimals()` assigns a new random `WorldPosition` offset to each animal every 2–7 seconds. The renderer applies this new offset directly, causing the animal to snap to its new spread position.

### Cause 3 — Terrain height applied post-position
The renderer samples `TerrainQuery.GetHeight()` using the already-jumped X/Z position. This amplifies vertical stepping when horizontal position has already moved in a lump.

---

## WHAT YOU ARE BUILDING

A **render-only interpolation cache** inside `AnimalRenderer.cs`. It is a lightweight data structure that sits between the simulation data and the MultiMesh transform writes. The simulation never knows it exists.

Two interpolation tiers:

- **Tier A** — Smooths herd center position and movement direction
- **Tier B** — Smooths individual animal offset changes

Both tiers share the same pattern: when the simulation value changes, start a timed blend from the last rendered value to the new target value. Each frame, advance the blend and write the interpolated value to the MultiMesh.

---

## STEP 1 — ADD THE RENDER STATE DATA STRUCTURES

Create two new structs. Place them at the bottom of `AnimalRenderer.cs` or in a new file `AnimalRenderState.cs` in the same namespace.

### HerdRenderState (one per visible herd)

```csharp
public struct HerdRenderState
{
    // Tier A — herd center
    public Vector3 RenderCenter;          // what we are currently drawing
    public Vector3 TargetCenter;          // where the sim says the herd is
    public Vector3 PreviousCenter;        // where RenderCenter was when TargetCenter last changed
    public float   CenterBlendElapsed;    // seconds since last target change
    public float   CenterBlendDuration;   // how long to take to reach TargetCenter

    // Tier A — movement direction / yaw
    public float   RenderYaw;            // current rendered heading (radians)
    public float   TargetYaw;
    public float   YawBlendElapsed;
    public float   YawBlendDuration;

    // Validity
    public bool    Initialised;          // false until first sim sample received
}
```

### AnimalRenderState (one per visible animal)

```csharp
public struct AnimalRenderState
{
    // Tier B — individual offset from herd center
    public Vector3 RenderOffset;         // current rendered offset
    public Vector3 TargetOffset;         // what the sim most recently assigned
    public Vector3 PreviousOffset;       // offset at the time the target last changed
    public float   OffsetBlendElapsed;
    public float   OffsetBlendDuration;  // constant: use 0.35f as the default

    public bool    Initialised;
}
```

---

## STEP 2 — ADD THE CACHE DICTIONARIES TO AnimalRenderer

Inside the `AnimalRenderer` class, add:

```csharp
// Keyed by HerdId (whatever identifier HerdBrain exposes — int, Guid, or string)
private Dictionary<int, HerdRenderState> _herdRenderStates = new();

// Keyed by AnimalStruct.UniqueId
private Dictionary<int, AnimalRenderState> _animalRenderStates = new();

// Constant blend duration for individual offset changes
private const float OffsetShuffleDuration = 0.35f;

// Maximum plausible jump distance before we snap instead of interpolate.
// If a herd center moves more than this between sim samples, skip the blend.
// Set to: maxAnimalSpeed (m/s) × longestLODInterval (s) × safetyMultiplier
// Example: 8 m/s × 1.0 s × 2.5 = 20 m
private const float SnapThreshold = 20f;
```

---

## STEP 3 — ADD THE TIER A UPDATE METHOD (Herd Center + Yaw)

Add this method to `AnimalRenderer`. Call it once per visible herd per frame, before building MultiMesh transforms.

```csharp
private HerdRenderState UpdateHerdRenderState(HerdBrain herd, float delta)
{
    int id = herd.HerdId; // use whatever the actual herd identifier field is

    if (!_herdRenderStates.TryGetValue(id, out HerdRenderState state))
    {
        // First time we have seen this herd — snap to current sim position, no blend
        state = new HerdRenderState
        {
            RenderCenter       = herd.CenterPosition,
            TargetCenter       = herd.CenterPosition,
            PreviousCenter     = herd.CenterPosition,
            CenterBlendElapsed = 1f,
            CenterBlendDuration= 1f,
            RenderYaw          = herd.MovementYaw,   // or derive from MovementDirection
            TargetYaw          = herd.MovementYaw,
            YawBlendElapsed    = 1f,
            YawBlendDuration   = 1f,
            Initialised        = true
        };
        _herdRenderStates[id] = state;
        return state;
    }

    // --- Detect sim center change ---
    if (!state.TargetCenter.IsEqualApprox(herd.CenterPosition))
    {
        float jumpDistance = state.TargetCenter.DistanceTo(herd.CenterPosition);

        if (jumpDistance > SnapThreshold)
        {
            // Large jump: teleport / LOD transition / just entered render range
            // Snap immediately — do not interpolate across half the map
            state.RenderCenter       = herd.CenterPosition;
            state.TargetCenter       = herd.CenterPosition;
            state.PreviousCenter     = herd.CenterPosition;
            state.CenterBlendElapsed = 1f;
            state.CenterBlendDuration= 1f;
        }
        else
        {
            // Normal sim update — start a new blend segment
            state.PreviousCenter     = state.RenderCenter; // start from wherever we currently are
            state.TargetCenter       = herd.CenterPosition;
            state.CenterBlendElapsed = 0f;

            // Use the LOD update interval as the blend duration so we finish
            // exactly as the next sim update is expected to arrive.
            state.CenterBlendDuration = BehaviourLODHelper.GetUpdateInterval(herd.CurrentLOD);
            // Clamp to a sensible range in case LOD interval is very short or very long
            state.CenterBlendDuration = Mathf.Clamp(state.CenterBlendDuration, 0.05f, 1.5f);
        }
    }

    // --- Detect yaw change ---
    float simYaw = herd.MovementYaw; // or Mathf.Atan2(herd.MovementDirection.X, herd.MovementDirection.Z)
    if (!Mathf.IsEqualApprox(state.TargetYaw, simYaw, 0.01f))
    {
        state.PreviousCenter = state.RenderCenter; // reuse pattern for yaw
        state.TargetYaw       = simYaw;
        state.YawBlendElapsed = 0f;
        state.YawBlendDuration= state.CenterBlendDuration; // keep in sync with center
    }

    // --- Advance blend timers ---
    state.CenterBlendElapsed = Mathf.Min(state.CenterBlendElapsed + delta, state.CenterBlendDuration);
    state.YawBlendElapsed    = Mathf.Min(state.YawBlendElapsed    + delta, state.YawBlendDuration);

    // --- Compute interpolated values ---
    float centerT = (state.CenterBlendDuration > 0f)
        ? state.CenterBlendElapsed / state.CenterBlendDuration
        : 1f;
    centerT = EaseOut(centerT); // see Step 6

    float yawT = (state.YawBlendDuration > 0f)
        ? state.YawBlendElapsed / state.YawBlendDuration
        : 1f;
    yawT = EaseOut(yawT);

    state.RenderCenter = state.PreviousCenter.Lerp(state.TargetCenter, centerT);
    state.RenderYaw    = LerpAngle(state.RenderYaw, state.TargetYaw, yawT);

    _herdRenderStates[id] = state;
    return state;
}
```

---

## STEP 4 — ADD THE TIER B UPDATE METHOD (Individual Offset)

Add this method to `AnimalRenderer`. Call it once per visible animal per frame.

```csharp
private AnimalRenderState UpdateAnimalRenderState(ref AnimalStruct animal, float delta)
{
    int uid = animal.UniqueId;

    if (!_animalRenderStates.TryGetValue(uid, out AnimalRenderState state))
    {
        state = new AnimalRenderState
        {
            RenderOffset       = animal.WorldPosition,
            TargetOffset       = animal.WorldPosition,
            PreviousOffset     = animal.WorldPosition,
            OffsetBlendElapsed = OffsetShuffleDuration,
            OffsetBlendDuration= OffsetShuffleDuration,
            Initialised        = true
        };
        _animalRenderStates[uid] = state;
        return state;
    }

    // Detect offset change (sim assigned a new spread position)
    if (!state.TargetOffset.IsEqualApprox(animal.WorldPosition, 0.01f))
    {
        state.PreviousOffset     = state.RenderOffset;
        state.TargetOffset       = animal.WorldPosition;
        state.OffsetBlendElapsed = 0f;
        // OffsetBlendDuration is constant — always 0.35 s
    }

    // Advance
    state.OffsetBlendElapsed = Mathf.Min(
        state.OffsetBlendElapsed + delta,
        state.OffsetBlendDuration
    );

    float t = state.OffsetBlendElapsed / state.OffsetBlendDuration;
    t = EaseOut(t);

    state.RenderOffset = state.PreviousOffset.Lerp(state.TargetOffset, t);

    _animalRenderStates[uid] = state;
    return state;
}
```

---

## STEP 5 — REWRITE THE MULTIMESH TRANSFORM ASSEMBLY

Locate the section of `AnimalRenderer._Process()` (or `UpdateFrame()`) that assembles transforms and writes them to the MultiMesh. Replace the direct position reads with interpolated values.

The existing pattern is roughly:

```csharp
// OLD — do not use
Vector3 worldPos = herd.CenterPosition + animals[i].WorldPosition;
worldPos.Y = TerrainQuery.GetHeight(worldPos.X, worldPos.Z);
mm.SetInstanceTransform(i, new Transform3D(basis, worldPos));
```

Replace with:

```csharp
// NEW

// Tier A: get smoothed herd center and yaw
HerdRenderState herdState = UpdateHerdRenderState(herd, delta);

// Build the shared herd-level basis from smoothed yaw
// (used as the base orientation for all animals in this herd)
Basis herdBasis = Basis.Identity.Rotated(Vector3.Up, herdState.RenderYaw);

for (int i = 0; i < animals.Length; i++)
{
    ref AnimalStruct animal = ref animals[i];

    // Tier B: get smoothed individual offset
    AnimalRenderState animalState = UpdateAnimalRenderState(ref animal, delta);

    // Compose final X/Z from smoothed center + smoothed offset
    float renderX = herdState.RenderCenter.X + animalState.RenderOffset.X;
    float renderZ = herdState.RenderCenter.Z + animalState.RenderOffset.Z;

    // Sample terrain height from interpolated X/Z position
    float renderY = TerrainQuery.GetHeight(renderX, renderZ);

    Vector3 worldPos = new Vector3(renderX, renderY, renderZ);

    // Optional terrain tilt (see Step 7)
    Basis finalBasis = herdBasis; // replace with tilted basis if implementing Step 7

    mm.SetInstanceTransform(i, new Transform3D(finalBasis, worldPos));
}
```

---

## STEP 6 — ADD THE EASING AND ANGLE HELPER METHODS

Add these to `AnimalRenderer` (or a shared MathUtils class if one exists):

```csharp
/// <summary>
/// Smoothstep ease-out. Input t must be in [0, 1].
/// Makes motion decelerate into the target rather than arriving linearly.
/// </summary>
private static float EaseOut(float t)
{
    t = Mathf.Clamp(t, 0f, 1f);
    return t * (2f - t); // quadratic ease-out
}

/// <summary>
/// Interpolates between two angles (in radians), taking the shortest arc.
/// t must be in [0, 1].
/// </summary>
private static float LerpAngle(float from, float to, float t)
{
    float diff = Mathf.Wrap(to - from, -Mathf.Pi, Mathf.Pi);
    return from + diff * t;
}
```

---

## STEP 7 — TERRAIN NORMAL TILT (OPTIONAL — DO AFTER STEPS 1–6 ARE WORKING)

This makes animals lean with the slope of the terrain rather than remaining perfectly upright on hills. It is a visual refinement only — implement it after the core interpolation is confirmed working.

Sample the heightmap at small offsets around the animal's X/Z to estimate the terrain normal:

```csharp
private Basis GetTerrainAlignedBasis(float x, float z, float yaw)
{
    const float sampleOffset = 0.5f; // metres

    float hCenter = TerrainQuery.GetHeight(x, z);
    float hRight  = TerrainQuery.GetHeight(x + sampleOffset, z);
    float hForward= TerrainQuery.GetHeight(x, z + sampleOffset);

    Vector3 right   = new Vector3(sampleOffset, hRight   - hCenter, 0f).Normalized();
    Vector3 forward = new Vector3(0f,           hForward - hCenter, sampleOffset).Normalized();
    Vector3 up      = right.Cross(forward).Normalized();

    // Yaw rotation around terrain-derived up vector
    Basis terrainBasis = new Basis(right, up, -forward);
    Basis yawBasis     = new Basis(Vector3.Up, yaw);

    return terrainBasis * yawBasis;
}
```

Use this in Step 5 in place of `Basis finalBasis = herdBasis`:

```csharp
Basis finalBasis = GetTerrainAlignedBasis(renderX, renderZ, herdState.RenderYaw);
```

**Important:** Do not call `GetTerrainAlignedBasis` once per animal — that is 3 height samples × 300 animals = 900 heightmap queries per frame. Call it once per herd (using the herd's render center X/Z) and share the resulting basis across all herd members. Animals in the same herd are close enough that one terrain normal per herd is visually indistinguishable from per-animal normals.

---

## STEP 8 — CACHE CLEANUP

Add cleanup to prevent the dictionaries from accumulating stale entries for herds and animals that have left render range.

In the section of `AnimalRenderer` that handles herds exiting render range (when `VisibilityNotifier3D` fires or when distance culling removes a herd from the visible set):

```csharp
private void OnHerdExitedRenderRange(int herdId, IEnumerable<int> animalUniqueIds)
{
    _herdRenderStates.Remove(herdId);
    foreach (int uid in animalUniqueIds)
        _animalRenderStates.Remove(uid);
}
```

If the renderer does not currently have an explicit exit callback, add a pass at the end of each frame that removes any entry whose herd is no longer in the visible set. Do not do this by iterating and removing during the render loop — collect keys to remove into a temporary list and remove after the loop.

---

## STEP 9 — IMPLEMENTATION ORDER AND TESTING GATES

Implement in this sequence. Confirm each gate before proceeding.

**Gate 1 — Tier A only, no easing**
Implement Steps 1, 2, 3, and 5 (herd center interpolation with linear t). Verify herd motion is smooth at all LOD levels. Profile: frame time should be unchanged within measurement noise.

**Gate 2 — Add easing**
Add Steps 6 and apply `EaseOut(t)` in the Tier A path. Confirm motion feels natural rather than mechanical. No performance impact expected.

**Gate 3 — Tier B**
Add Step 4 and wire into Step 5. Verify individual animals drift smoothly into new spread positions rather than snapping. Confirm the 0.35s shuffle duration feels right — adjust the constant if animals feel too sluggish or too fast.

**Gate 4 — Cache cleanup**
Add Step 8. Run for several in-game days and confirm dictionary sizes remain bounded. Check with a debug print of `_herdRenderStates.Count` and `_animalRenderStates.Count` every 30 seconds.

**Gate 5 — Terrain tilt (optional)**
Add Step 7 only after Gates 1–4 are stable. Confirm it is called once per herd, not once per animal. Profile before and after.

---

## CONSTANTS SUMMARY

| Constant | Value | Rationale |
|---|---|---|
| `SnapThreshold` | 20f metres | Max plausible move before snap-not-blend. Adjust if animals are faster. |
| `OffsetShuffleDuration` | 0.35f seconds | Offset blend time. Must be well under 2s (minimum variation interval). |
| `sampleOffset` (terrain tilt) | 0.5f metres | Height sample spacing for normal estimation. |
| `CenterBlendDuration` clamp min | 0.05f seconds | Prevents divide-by-zero and over-fast blends at Full LOD. |
| `CenterBlendDuration` clamp max | 1.5f seconds | Prevents very long glides if a high LOD interval is configured. |

---

## WHAT NOT TO CHANGE

- `HerdBrain.cs` — no changes whatsoever
- `AnimalStruct.cs` — no changes to data fields
- `BehaviourLODHelper.cs` — no changes; read its intervals, do not modify them
- `TerrainQuery.GetHeight()` — no changes; continue to call it as-is but from interpolated X/Z
- Simulation tick rates — no changes; the entire point is that these stay untouched

---

## EXPECTED OUTCOME

After implementation:

- Herds at Medium LOD (1s update interval) glide continuously rather than stepping forward once per second
- Individual animals drift into new spread positions over ~0.35 seconds rather than snapping
- Animals entering render range snap into position immediately (no sliding in from off-screen)
- Animals on slopes lean with the terrain (if Step 7 is implemented)
- Frame time is unchanged within profiling noise — all new work is lightweight float math and dictionary lookups, proportional to visible animal count only
