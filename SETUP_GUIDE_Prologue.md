# Prologue Scene Setup Guide — "The Night Before"

Isaac Monroe's apartment. Small, depressing, lit by screens. No enemies, no combat. Pure environmental storytelling and dread.

---

## 1. Scene Creation

1. In Unity: **File > New Scene** (use the Basic URP template)
2. Save as `Assets/Scenes/Prologue_TheNightBefore.unity`
3. Delete the default Directional Light (the apartment has NO overhead lighting)

---

## 2. Apartment Greybox Layout

Build everything with **ProBuilder** or Unity primitives (cubes/planes). No art assets needed.

### Floor Plan

```
    ┌──────────────────────────────────────┐
    │                                      │
    │   MAIN ROOM (~7m × 5m)              │
    │                                      │
    │   ┌─────┐    ┌──────┐   ┌────────┐  │
    │   │COUCH│    │ DESK │   │KITCHEN │  │
    │   │     │    │      │   │COUNTER │  │
    │   └─────┘    └──────┘   └────────┘  │
    │                                      │
    │   ┌────┐                             │
    │   │ TV │    [SHELF w/ PHOTO]         │
    │   └────┘                             │
    │          ┌──────┐                    │
    │   [WINDOW]│FRONT│  [RESEARCH BOX]    │
    │          │DOOR  │                    │
    │          │+MAIL │                    │
    │          └──────┘                    │
    │                                      │
    │   ┌──────────┐  ┌──────────────┐     │
    │   │ BATHROOM │  │   BEDROOM    │     │
    │   │ 2.5×2m   │  │   3×3m       │     │
    │   │          │  │   (LOCKED)   │     │
    │   │ [MIRROR] │  │              │     │
    │   └──────────┘  └──────────────┘     │
    └──────────────────────────────────────┘
```

### Construction Steps

1. **Floor**: Plane at Y=0, scale to ~7×5m. Apply dark wood material (`#3B2F2F`)
2. **Walls**: ProBuilder cubes or thin boxes, 3m height, `#E8E0D0` (off-white). Walls near kitchen: tint yellowed (`#E0D8B0`)
3. **Ceiling**: Plane at Y=3m. Same off-white. Add a water stain decal/texture in one corner (darker patch)
4. **Bathroom walls**: Extend from main room. Leave a doorway opening (no door — just walk in)
5. **Bedroom wall**: Solid wall with door frame. Place a cube door (the BedroomDoorInteraction target)

### Furniture (all primitive cubes)

| Object | Approximate Size | Position | Material Color |
|--------|-----------------|----------|---------------|
| Couch | 2×0.8×0.8m | Against wall | `#4A4040` (dark gray-brown) |
| Desk | 1.2×0.75×0.6m | Near couch | `#5C4033` (dark wood) |
| TV Stand | 1×0.5×0.4m | Facing couch | `#3A3A3A` (dark gray) |
| TV Screen | 0.8×0.5×0.05m | On TV stand | Black (will get materials from TVInteraction) |
| Kitchen Counter | 2×0.9×0.6m | Against wall | `#606060` (gray) |
| Shelf | 0.8×1.2×0.2m | Wall-mounted | `#5C4033` (dark wood) |
| Photo Frame | 0.15×0.12×0.02m | On shelf, face-down | `#3B2F2F` (dark brown) |
| Bathroom Sink | 0.5×0.8×0.4m | Against bathroom wall | `#D0D0D0` (light gray) |
| Bathroom Mirror | 0.5×0.7×0.02m | Above sink, on wall | White/reflective |
| Research Box | 0.4×0.3×0.3m | Floor near desk | `#8B7355` (cardboard brown) |
| Front Door | 0.9×2.1×0.05m | Wall opening | `#5C4033` (wood) |
| Mail Pile | 0.3×0.05×0.2m | Floor at front door base | `#E8E0D0` (paper white) |
| Window | 1×1.2×0.02m | Wall cutout | N/A (covered by curtain) |
| Curtain | 1.2×1.4×0.05m | Over window | `#4A4A55` (dark blue-gray) |

### Floor Dust & Worn Paths

- Use a particle system with slow-falling dust motes OR
- Apply a dusty overlay material to the floor plane
- Create a second slightly-raised plane with a cleaner material for worn paths:
  - Couch → Desk
  - Desk → Kitchen Counter
  - Kitchen → Bathroom doorway
  - These paths show where Isaac actually moves. Everything else is dusty.

---

## 3. Lighting Setup

**Critical**: No overhead room light. The apartment is dark, lit by screens and lamps.

### TV Light
- **Type**: Point Light
- **Color**: `#4488CC` (blue)
- **Intensity**: 2.5
- **Range**: 5m
- **Position**: Just in front of TV screen
- **Flicker**: Add a simple script that oscillates intensity between 2.0–3.0 at random intervals (0.05–0.2s). This is the dominant light source in the room.

### Desk Lamp
- **Type**: Point Light
- **Color**: `#FFD080` (warm yellow)
- **Intensity**: 1.5
- **Range**: 3m
- **Position**: On desk
- **Notes**: This is the "safe zone" light. Warm, steady, small radius. The desk area should feel like the only comfortable spot.

### Bathroom Fluorescent
- **Type**: Spot Light
- **Color**: `#D4E4F0` (white-blue)
- **Intensity**: 3
- **Spot Angle**: 90°
- **Range**: 4m
- **Position**: Ceiling of bathroom
- **Flicker**: Aggressive flicker — occasionally drops to 0 intensity for 0.1s, then snaps back. Harsh, institutional feel.

### Window Ambient
- **Type**: Point Light
- **Color**: `#FFB060` (warm orange)
- **Intensity**: 0.3
- **Range**: 2m
- **Position**: Behind curtain geometry
- **Notes**: Barely visible. Faint city glow seeping through fabric edges.

---

## 4. URP Post-Processing Volume

1. Create empty GameObject: `PostProcessing_Global`
2. Add **Volume** component, set **Mode** to **Global**
3. Create new **Volume Profile**
4. Add these overrides:

| Effect | Setting | Value |
|--------|---------|-------|
| **Chromatic Aberration** | Intensity | 0.1 |
| **Film Grain** | Type | Thin1 |
| **Film Grain** | Intensity | 0.15 |
| **Vignette** | Intensity | 0.3 |
| **Vignette** | Smoothness | 0.4 |
| **Bloom** | Threshold | 1.0 |
| **Bloom** | Intensity | 0.3 |
| **Color Adjustments** | Saturation | -10 |

**Note**: The existing `Squint.cs` uses PostProcessing v2 (legacy `PostProcessVolume`). For URP, you'll use the URP Volume system instead. The Squint script will need a minor update to work with URP volumes if you use it in this scene.

---

## 5. Player Prefab Setup

### Hierarchy

```
Player                          (tag: "Player")
├── CharacterController         (Height: 1.8, Radius: 0.3, Center: 0,0.9,0)
├── PlayerMovement              (see values below)
├── CharacterPush
├── AudioSource                 (footsteps)
│
└── NeckJoint                   (empty, Position: 0,1.6,0)
    └── MainCamera              (tag: "MainCamera")
        ├── Camera              (FOV: 80, Near: 0.1, Far: 100)
        ├── CinemachineBrain    (see Cinemachine setup below)
        ├── MouseLook            (sensitivityX: 2, sensitivityY: 2, lockCursor: true)
        ├── Interact
        ├── Squint
        ├── AudioListener
        │
        ├── HoldPos             (empty, Position: 0,0,0.5 — for GrabbableObject)
        │
        └── [UI Canvas]         (Screen Space - Overlay)
            ├── InteractionUI   (TMP_Text + Animation for popup/popout)
            ├── CrosshairUI     (UI.Image, small white dot, centered)
            ├── ExaminePanel    (see ExamineUI setup below)
            └── PhotoUI         (see PhotoInteraction setup below)
```

### PlayerMovement Inspector Values (Prologue-specific)

| Field | Value | Reason |
|-------|-------|--------|
| walkSpeed | 2.5 | Slow, depressed pace |
| runSpeed | 4.0 | Sluggish — Isaac doesn't want to run |
| crouchSpeed | 1.2 | Barely moving |
| jumpSpeed | 0 | **Disable jumping** — no reason to jump in the apartment |
| defGravity | 20 | Default |
| limitDiagonalSpeed | true | |
| toggleRun | false | Hold to run |
| airControl | false | |
| fallingDamageThreshold | Infinity | No fall damage in apartment |

### MouseLook Inspector Values

| Field | Value |
|-------|-------|
| axes | MouseXAndY |
| sensitivityX | 2 |
| sensitivityY | 2 |
| minimumY | -60 |
| maximumY | 60 |
| lockCursor | true |
| working | true |

### Interact Inspector Values

| Field | Assignment |
|-------|-----------|
| InteractionUI | → InteractionUI Text object |
| CrosshairUI | → CrosshairUI Image object |

### Input Manager Buttons Required

Set these up in **Edit > Project Settings > Input Manager**:

| Button Name | Default Key |
|------------|-------------|
| Interact | E |
| Squint | Mouse 1 (right-click) |
| Run | Left Shift |
| Crouch | Left Control |
| Jump | Space |

### Cinemachine Setup

1. Add **CinemachineBrain** component to MainCamera
2. Create a **CinemachineCamera** (formerly CinemachineVirtualCamera in CM 2.x) in the scene:
   - Name it `VCam_Player`
   - Set **Body**: Transposer, Follow = Player's NeckJoint
   - Set **Aim**: Composer (or POV for first-person)
   - Priority: 10 (default active camera)
3. **Important**: BedroomDoorInteraction and PhotoInteraction automatically disable the CinemachineBrain during their camera animation sequences (camera transform lerps). This prevents Cinemachine from fighting the manual camera control. The brain re-enables when the sequence ends.
4. If using Cinemachine 2.x (namespace `Cinemachine`), change the `using Unity.Cinemachine;` in BedroomDoorInteraction.cs and PhotoInteraction.cs to `using Cinemachine;`

### Required Packages

Ensure these are installed via **Window > Package Manager**:

| Package | ID | Notes |
|---------|-----|-------|
| **Cinemachine** | `com.unity.cinemachine` | Camera management. Scripts reference `CinemachineBrain`. |
| **ProBuilder** | `com.unity.probuilder` | Greybox geometry. No script dependencies. |
| **TextMeshPro** | `com.unity.textmeshpro` | All UI text uses `TextMeshProUGUI` / `TMP_Text`. Import TMP Essentials when prompted. |

---

## 6. UI Setup

### InteractionUI (TextMeshPro)
- Canvas > **TextMeshPro - Text (UI)** named "InteractionUI"
- Anchor: bottom center
- Font size: 18, color white, center alignment
- Add Animation component with clips: `An_InteractTextPopup`, `An_InteractTextPopout`
- **NOTE**: Interact.cs now uses `TextMeshProUGUI` instead of legacy `Text`

### CrosshairUI (existing pattern)
- Canvas > Image named "CrosshairUI"
- Anchor: center, size 4x4
- White dot sprite, starts disabled

### ExaminePanel (NEW — for ExamineUI.cs)
1. Canvas > Create empty "ExaminePanel"
2. Add **Image** component: color `(0, 0, 0, 0.7)` — dark semi-transparent
3. Add **CanvasGroup** component: alpha = 0
4. Anchor: stretch-stretch (full screen) with margins (100px each side)
5. Child: **TextMeshPro - Text (UI)** named "ExamineText"
   - Anchor: center, stretch width
   - Font size: 22, color white, center alignment
   - Line spacing: 1.2
6. Add **ExamineUI.cs** script to ExaminePanel
7. Assign ExamineText field → the TMP_Text child

### PhotoUI (for PhotoInteraction.cs)
1. Canvas > Create empty "PhotoPanel"
2. Child: **Image** named "PhotoImage"
   - Anchor: center, size 400x300 (adjust for photo aspect ratio)
   - Preserve Aspect: true
3. PhotoPanel starts disabled (SetActive false)
4. PhotoInteraction.photoUI → PhotoImage

---

## 7. Interactable Object Setup

### General Rule
Every interactable object needs:
1. A **Collider** (Box Collider is fine for greybox)
2. Tag set to **"Interactable"**
3. The appropriate script component

### Per-Object Setup

#### Couch, Kitchen Sink, Spice Rack, Fridge, Calendar, Coat Hook, Grief Book
- Script: **ExamineObject.cs**
- Set `examineTexts` to appropriate text for each object
- Set `hoverPrompt` (e.g., "Examine Couch", "Look at Calendar")

#### Bedroom Door
- Script: **BedroomDoorInteraction.cs**
- Assign `involuntarySound` AudioClip
- Add AudioSource component
- Collider on the door mesh

#### Photo Frame (on shelf)
- Script: **PhotoInteraction.cs**
- Start face-down: Rotation X=180 or Z=180
- Assign `photoSprite` (placeholder: solid color sprite)
- Assign `photoUI` → PhotoImage in canvas
- Assign `frameRenderer` → the frame's Renderer
- Add AudioSource component

#### TV Screen
- Script: **TVInteraction.cs**
- Create 3 materials:
  - `TV_Static_Normal`: Blue-ish noisy texture, emissive
  - `TV_Static_Structured`: Organized pattern (horizontal lines, grid), emissive
  - `TV_Off`: Solid black, non-emissive
- Assign all materials + tvLight + staticAudioSource + tvRenderer
- Add AudioSource with looping static noise clip

#### Bathroom Mirror
- Script: **MirrorAnomaly.cs** (on a TRIGGER zone, not the mirror itself)
- Create Box Collider trigger in front of mirror (~1m deep, mirror width)
- **Do NOT tag as "Interactable"** — this uses OnTriggerStay
- Create 2 materials:
  - `Mirror_Normal`: Default mirror/reflective
  - `Mirror_Anomaly`: Disturbing distortion (placeholder: slightly different color/pattern)
- Assign mirrorRenderer → the actual mirror mesh Renderer

#### Research Box
- Script: **ResearchBoxInteraction.cs**
- Add AudioSource component
- Assign `musicBoxNote` AudioClip (single crystalline note)
- Place on floor near desk

#### Window/Curtains
- Script: **WindowInteraction.cs**
- Add AudioSource component (set Spatial Blend to 1.0 for 3D)
- Assign `childLaughClip` AudioClip
- Set volume low (~0.2)

#### Front Door + Mail Pile
- Script: **FrontDoorInteraction.cs**
- Can go on the mail pile object or the door itself
- Default examineTexts are pre-filled

---

## 8. Placeholder Audio Clips Needed

Create or import placeholder audio for:

| Clip | Description | Used By |
|------|-------------|---------|
| `SFX_TVStatic_Loop` | TV white noise, looping | TVInteraction |
| `SFX_InvoluntarySound` | Isaac's involuntary gasp/whimper | BedroomDoorInteraction |
| `SFX_PhotoPickup` | Paper/frame pickup sound | PhotoInteraction |
| `SFX_MusicBoxNote` | Single crystalline bell/music box note | ResearchBoxInteraction |
| `SFX_ChildLaugh_Distant` | Distant child laughing, muffled | WindowInteraction |

---

## 9. Placeholder Materials Needed

| Material | Color/Type | Used By |
|----------|-----------|---------|
| `MAT_Floor_DarkWood` | `#3B2F2F` | Floor |
| `MAT_Wall_OffWhite` | `#E8E0D0` | Walls |
| `MAT_Wall_Yellowed` | `#E0D8B0` | Kitchen walls |
| `MAT_TV_Static_Normal` | Blue noise, emissive | TVInteraction |
| `MAT_TV_Static_Structured` | Organized pattern, emissive | TVInteraction |
| `MAT_TV_Off` | `#0A0A0A`, non-emissive | TVInteraction |
| `MAT_Mirror_Normal` | White/reflective | MirrorAnomaly |
| `MAT_Mirror_Anomaly` | Distorted variant | MirrorAnomaly |
