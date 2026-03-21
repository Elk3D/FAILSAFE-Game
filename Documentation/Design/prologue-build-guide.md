# FAILSAFE — Prologue Build Guide

> **Purpose:** Step-by-step reference for building the Prologue sequence from scratch.
> The Prologue is not a numbered chapter — it precedes Chapter 01: Awakening.
> It exists to establish tone, introduce controls, and deliver the opening epigraph.

---

## Part 1 — Overview, Scene Structure & Core Mechanics

### 1.1 What the Prologue Does

| Goal | Delivery Method |
|------|----------------|
| Establish atmosphere | Lighting, ambient audio, no music |
| Introduce movement controls | Empty corridor with subtle prompts |
| Introduce interaction | One readable note, one locked door |
| Deliver opening epigraph | EpigraphSystem → fade into Chapter 01 |
| Set narrative tone | Narrator VO line on wake-up |

The Prologue should take **90–120 seconds** on first playthrough. No puzzle, no inventory, no fail state.

---

### 1.2 Scene Name & File

- **Scene file:** `Assets/Scenes/Prologue.unity`
- **Scene index:** 1 (index 0 = MainMenu)
- Add to Build Settings before any other work.

---

### 1.3 Scene Structure (Hierarchy)

```
Prologue [Scene Root]
├── _Managers
│   ├── GameManager          (carries over via DontDestroyOnLoad — do not duplicate)
│   ├── AudioManager         (carries over — do not duplicate)
│   └── SettingsManager      (carries over — do not duplicate)
│
├── Environment
│   ├── WakeRoom             — small starting room, player spawns here
│   │   ├── Geometry         — walls, floor, ceiling (ProBuilder or pre-built mesh)
│   │   ├── Lighting         — single overhead flicker light + fill
│   │   └── Props            — cot, desk, single note
│   └── Corridor             — connects WakeRoom to the locked exit door
│       ├── Geometry
│       ├── Lighting         — dim, directional toward exit
│       └── ExitDoor         — locked; opens on PrologueComplete event
│
├── Player
│   ├── PlayerMovement
│   ├── MouseLook
│   ├── PlayerStats
│   ├── PlayerInventory
│   └── Interact
│
├── Triggers
│   ├── NarrativeTrigger_Wake    — fires narrator VO on scene start
│   └── PrologueCompleteTrigger  — at exit door; fires epigraph → Chapter 01
│
└── UI [Canvas]
    ├── HUDController
    ├── SubtitleSystem
    └── EpigraphSystem
```

> **Rule:** Only instantiate managers if they are NOT already present (check `FindObjectOfType` or use the `Singleton<T>` base). The `_Managers` objects in this scene should be guarded by existence checks or moved to a persistent Bootstrap scene.

---

### 1.4 Player Spawn

- Create an empty GameObject named `PlayerSpawnPoint` inside `WakeRoom`.
- Position: face toward the desk (note is in immediate sightline).
- The Player prefab should be placed at this transform in `Awake` or pre-positioned in the scene.

---

### 1.5 Core Mechanics Active in the Prologue

Only enable what the Prologue actually uses. Disable or leave unconfigured anything else to keep scope tight.

| Mechanic | Active? | Notes |
|----------|---------|-------|
| Walk / Run | Yes | Default `PlayerMovement` settings |
| Crouch | Yes | Needed to read note on desk |
| Jump | No | No geometry requires it; disable in `PlayerMovement` |
| Ladder | No | Not present in Prologue |
| Interact (examine) | Yes | Used for the note only |
| Interact (pick up) | No | No pickup items in Prologue |
| Inventory | No | Leave empty; no item gates |
| Subtitles | Yes | VO line on wake needs captions |
| Pause Menu | Yes | Always available |

To disable Jump in `PlayerMovement.cs`, set `canJump = false` in the Inspector or guard the jump input behind a bool.

---

### 1.6 Lighting Setup

**WakeRoom:**
- One `Point Light` above the cot — set to flicker via `LightFlicker` component (low frequency, subtle).
- One low-intensity `Directional Light` or second dim `Point Light` as fill to keep the room readable.
- Color temperature: cool/blue-white (~5500K). No warm tones yet — warmth is Chapter 01's reward.

**Corridor:**
- Strip of `Spot Lights` or `Area Lights` along the ceiling, aimed downward.
- Intensity decreases toward the middle of the corridor, then increases slightly near the exit door — draw the eye forward.
- Baked or mixed lighting preferred for performance.

**Exit Door area:**
- Subtle rim light or god-ray particle to signal "this is the goal."

---

### 1.7 Scene Transition Flow

```
[Scene Load: Prologue]
    → NarrativeTrigger_Wake fires (narrator VO: opening line)
    → Player explores WakeRoom
    → Player reads note on desk (Interact)
    → Player moves through Corridor
    → Player reaches ExitDoor area
    → PrologueCompleteTrigger fires
        → EpigraphSystem plays opening epigraph
        → SceneManager loads Chapter01_Awakening
```

The `PrologueCompleteTrigger` should be a `Collider` (isTrigger = true) placed just in front of the ExitDoor, tagged `Player` only.

---

*End of Part 1. Continue in Part 2: Scripts to Create & Interactable Catalog.*

---

## Part 2 — Scripts to Create & Interactable Catalog

### 2.1 New Scripts Required for the Prologue

These scripts do not yet exist and must be created. Each is specific to the Prologue's needs.

| Script | Purpose | Attach To |
|--------|---------|-----------|
| `PrologueDirector.cs` | Orchestrates scene start: triggers VO, enables player input after brief delay | Empty GameObject `_PrologueDirector` |
| `NarrativeTrigger.cs` | Zone-based VO trigger (fires once, plays AudioClip via AudioManager) | Trigger colliders in scene |
| `PrologueCompleteTrigger.cs` | Fires epigraph then loads Chapter 01 | Trigger at ExitDoor |
| `ExitDoorController.cs` | Keeps door locked; listens for `PrologueComplete` event to unlock/open | ExitDoor GameObject |

> **Note:** `InspectNote.cs`, `Interact.cs`, `PlayerMovement.cs`, and `MouseLook.cs` already exist and require no changes.

---

### 2.2 PrologueDirector.cs — Spec

```
On Awake:
  - Disable PlayerMovement.isWorking (freeze player)
  - Disable MouseLook on all MouseLook components

On Start (after 1-frame delay):
  - Play NarratorVO_Wake AudioClip via AudioManager
  - Wait for clip length
  - Re-enable PlayerMovement and MouseLook
  - Fire event: OnPrologueStarted
```

This gives the impression the player "wakes up" before gaining control.

---

### 2.3 NarrativeTrigger.cs — Spec

```
Fields:
  - AudioClip voiceLine
  - bool firedOnce = false

OnTriggerEnter (Player tag):
  - if firedOnce: return
  - Play voiceLine via AudioManager
  - firedOnce = true
```

Place one in the Corridor midpoint for an optional atmospheric line.

---

### 2.4 PrologueCompleteTrigger.cs — Spec

```
OnTriggerEnter (Player tag):
  - Disable PlayerMovement
  - Call EpigraphSystem.Play(epigraphData)
  - EpigraphSystem.OnComplete → SceneManager.LoadScene("Chapter01_Awakening")
```

The `EpigraphSystem` already exists on the UI Canvas (per `architecture.md`). Wire it up via `FindObjectOfType<EpigraphSystem>()` or a serialized reference.

---

### 2.5 Interactable Catalog

Every object tagged `Interactable` in the Prologue scene, and how to configure it:

#### Object 1 — Wake Note (on desk in WakeRoom)

| Field | Value |
|-------|-------|
| Tag | `Interactable` |
| Script | `InspectNote.cs` |
| `prompts[0]` | `"Read"` |
| `targetImage` | Sprite: `note_prologue_wake` (create in Art/Notes) |
| `Source` | AudioSource on same GameObject |
| `clips[0]` | paper shuffle in SFX |
| `clips[1]` | paper shuffle out SFX |
| `pageRend` | MeshRenderer of the note prop |
| `noteUI` | Reference to NoteUI Image on HUD Canvas |
| `targetColor` | Yellow (default) — or adjust to match scene palette |

**Note content (suggested text):**
> *"FAILSAFE protocol engaged. All personnel must remain in their designated sectors until further notice. This is not a drill."*

This is the first piece of narrative. Keep it short and foreboding.

#### Object 2 — ExitDoor

| Field | Value |
|-------|-------|
| Tag | `Interactable` |
| Script | `ExitDoorController.cs` (new) |
| Default state | Locked (displays "Locked" hover message) |
| Unlocked by | `PrologueComplete` event |
| Animation | Simple slide or rotate open (use Animator or iTween) |

When the player hovers over a locked door before the prologue completes, `Interact.cs` will call `Hovering()` on it — `ExitDoorController` should set `InteractionScript.message = "Locked"` in response.

---

### 2.6 Tags Required

Ensure these tags exist in `Project Settings → Tags & Layers`:

- `Interactable` — all interactable objects
- `Player` — Player GameObject (used by trigger OnTriggerEnter checks)
- `Ladder` — not used in Prologue but should be present globally
- `Slide` — not used in Prologue
- `Trampoline` — not used in Prologue
- `MovingPlatform` — not used in Prologue

---

*End of Part 2. Continue in Part 3: Phone System, Pager Sequence & Audio.*

---

## Part 3 — Phone System, Pager Sequence & Audio

### 3.1 Is There a Phone/Pager in the Prologue?

The Prologue is intentionally minimal — no puzzles, no codes. However, a **pager prop** on the desk serves as an environmental detail and foreshadowing device. It is **not interactive** in the Prologue; it becomes interactive in Chapter 02.

| Element | Prologue State | Notes |
|---------|---------------|-------|
| Pager prop | Present, non-interactive | Sits on desk near Wake Note |
| Pager screen | Texture showing `--- NEW MSG ---` | Static texture swap, no logic |
| Phone (wall-mounted) | Not present in Prologue | Introduced in Chapter 01 |
| Pager sequence puzzle | Locked — Chapter 02 only | Do not wire up in Prologue |

**Setup:** Place the pager mesh in WakeRoom on the desk. Assign a material with the `--- NEW MSG ---` texture. No script, no tag, no collider needed — it is pure dressing.

---

### 3.2 Narrator VO Sequence

The Prologue has **two VO lines**. Both are triggered automatically (not by player input).

| # | Trigger | Line (suggested) | Timing |
|---|---------|-----------------|--------|
| 1 | Scene start (PrologueDirector) | *"You are awake. For now."* | Plays before player gains control (~2s in) |
| 2 | NarrativeTrigger mid-Corridor | *"The door at the end. That's the only way forward."* | Fires once when player enters corridor |

**Audio file naming convention:**
```
Assets/Audio/Narrator/
  nar_prologue_wake.wav
  nar_prologue_corridor.wav
```

Both clips should be:
- Mono
- 44.1kHz / 16-bit
- Normalized to -3dBFS
- Slight reverb tail baked in (not applied at runtime) to match the facility aesthetic

---

### 3.3 Ambient Audio Setup

The Prologue uses **no music**. Silence broken only by ambient drones creates maximum tension on first playthrough.

#### AmbientAudioZone Configuration

Place two `AmbientAudioZone` trigger volumes:

| Zone | Location | Clip | Volume | Loop |
|------|----------|------|--------|------|
| `Ambient_WakeRoom` | WakeRoom bounds | `amb_facility_hum_low.wav` | 0.18 | Yes |
| `Ambient_Corridor` | Corridor bounds | `amb_corridor_distant_drip.wav` | 0.22 | Yes |

Zones should overlap slightly at the doorway between WakeRoom and Corridor so the crossfade is smooth. `AmbientAudioZone` blends via `AudioSource` volume lerp — set fade speed to `1.5f` for a 1.5-second blend.

**Audio file naming convention:**
```
Assets/Audio/Ambient/
  amb_facility_hum_low.wav
  amb_corridor_distant_drip.wav
```

---

### 3.4 Footstep Surface Materials

`PlayerMovement.cs` plays `clips[0]` as a looping footstep. For the Prologue:

| Surface | Tag or Layer | Recommended Clip |
|---------|-------------|-----------------|
| WakeRoom floor (concrete) | — | `sfx_footstep_concrete_loop.wav` |
| Corridor floor (metal grating) | — | `sfx_footstep_metal_loop.wav` |

> The current `PlayerMovement` implementation uses a single `clips[0]` for all footsteps. If surface-variety footsteps are needed, that system upgrade is out of scope for the Prologue — use a single concrete loop for now and note it as a future task.

---

### 3.5 Sound Effects Checklist

| SFX | Source | Trigger |
|-----|--------|---------|
| Note page shuffle in | `InspectNote` AudioSource | On interact (open) |
| Note page shuffle out | `InspectNote` AudioSource | On interact (close) |
| Narrator VO 1 | `PrologueDirector` AudioSource | Scene start |
| Narrator VO 2 | `NarrativeTrigger` AudioSource | Corridor entry |
| Door unlock click | `ExitDoorController` AudioSource | PrologueComplete event |
| Door open (slide/creak) | `ExitDoorController` AudioSource | After unlock click |
| Ambient hum | `Ambient_WakeRoom` zone | Continuous |
| Ambient drip | `Ambient_Corridor` zone | Continuous |

All SFX AudioSources should have `PlayOnAwake = false` and `Spatial Blend = 1.0` (3D) except narrator VO, which should be `Spatial Blend = 0` (2D).

---

*End of Part 3. Continue in Part 4: Foreshadowing Checklist, Second Playthrough & Build Order.*

---

## Part 4 — Foreshadowing Checklist, Second Playthrough & Build Order

### 4.1 Foreshadowing Checklist

Every detail in the Prologue should mean something by the end of the game. Use this checklist when dressing the scene.

| Element | Where | What It Foreshadows |
|---------|-------|-------------------|
| Pager with `--- NEW MSG ---` | Desk, WakeRoom | Chapter 02 pager puzzle; the message is never shown in Prologue — player wonders what it says |
| Wake Note: "FAILSAFE protocol engaged" | Desk, WakeRoom | The game's central mystery — what is FAILSAFE? |
| Narrator line: *"You are awake. For now."* | Scene start VO | Implies previous sleeps, or death/reset cycles — revisit in Chapter 04 |
| Single flickering light above cot | WakeRoom ceiling | Power instability — Chapter 03 reveals the facility is failing |
| ExitDoor is locked with no explanation | Corridor end | The facility controls movement — a theme throughout the game |
| Corridor lights aimed at exit | Corridor | Subconscious direction: the game always points you forward even when it feels open |
| No other humans visible | Entire scene | Where is everyone? Answered in Chapter 03 |
| Cot is military-issue, not a hospital bed | WakeRoom prop | The player is not a patient — they are personnel |

**Rule:** Do not over-explain. These elements should feel like set dressing on first playthrough and only register as meaningful in retrospect.

---

### 4.2 Second Playthrough Considerations

On a second playthrough the player has full context. Make small accommodations:

| Situation | Approach |
|-----------|---------|
| Narrator VO fires again | Allow it — it recontextualizes. *"You are awake. For now."* lands differently after Chapter 04. |
| Wake Note content | Same text — no changes needed. |
| Pager message never shown | Keep it that way. The unanswered question is intentional. |
| Prologue length | 90–120s is fine for repeat play. Do not add a skip. |
| EpigraphSystem opening quote | Ensure the quote is chosen to reward second reads — something with double meaning. |

**Suggested opening epigraph:**
> *"The first step in any process of recovery is to remember what was lost."*
> — Internal Memo, FAILSAFE Division, Date Redacted

---

### 4.3 Build Order

Follow this sequence when building the Prologue. Each step is a stable checkpoint.

```
Step 1 — Scene Setup
  [ ] Create Prologue.unity
  [ ] Add to Build Settings (index 1)
  [ ] Block out WakeRoom geometry (ProBuilder boxes)
  [ ] Block out Corridor geometry
  [ ] Place Player prefab at spawn point
  [ ] Verify player can walk and look around
  CHECKPOINT: Player can move through empty grey-box scene

Step 2 — Core Interactable
  [ ] Add desk prop to WakeRoom
  [ ] Add InspectNote to note prop (tag: Interactable)
  [ ] Create note_prologue_wake sprite
  [ ] Wire up InspectNote fields in Inspector
  [ ] Test: player can read and close the note
  CHECKPOINT: Note readable, movement freezes during read

Step 3 — Exit Door
  [ ] Add ExitDoor mesh to Corridor end
  [ ] Create ExitDoorController.cs
  [ ] Door shows "Locked" on hover
  [ ] Add PrologueCompleteTrigger collider in front of door
  CHECKPOINT: Player reaches door, sees locked message

Step 4 — Narrative Triggers
  [ ] Create PrologueDirector.cs
  [ ] Wire up NarratorVO_Wake placeholder audio
  [ ] Create NarrativeTrigger in Corridor
  [ ] Wire up nar_prologue_corridor placeholder audio
  [ ] Test: VO fires on scene start and at corridor trigger
  CHECKPOINT: Both VO lines play correctly, once each

Step 5 — Scene Transition
  [ ] Wire PrologueCompleteTrigger to EpigraphSystem
  [ ] Create EpigraphData ScriptableObject with opening quote
  [ ] EpigraphSystem.OnComplete loads Chapter01_Awakening
  [ ] Test full flow: scene start → note → door → epigraph → Chapter 01
  CHECKPOINT: Full prologue plays end-to-end without errors

Step 6 — Lighting Pass
  [ ] Replace grey-box with final or near-final geometry
  [ ] Set up WakeRoom flicker light (LightFlicker component)
  [ ] Set up Corridor strip lights
  [ ] Bake lighting (Mixed mode)
  CHECKPOINT: Lighting matches design spec in Part 1

Step 7 — Audio Pass
  [ ] Replace placeholder VO with recorded clips
  [ ] Add Ambient_WakeRoom AmbientAudioZone
  [ ] Add Ambient_Corridor AmbientAudioZone
  [ ] Wire up all SFX AudioSources per Part 3 checklist
  [ ] Set all VO sources to Spatial Blend 0, all SFX to 1
  CHECKPOINT: Full audio plays correctly on run-through

Step 8 — Props & Dressing
  [ ] Add pager prop to desk (non-interactive, static texture)
  [ ] Add military cot prop
  [ ] Dress corridor with pipes, cable trays, signage
  [ ] Verify foreshadowing checklist (Part 4.1) is met
  CHECKPOINT: Scene reads as an inhabited, real space

Step 9 — Polish & QA
  [ ] Subtitles display correctly for both VO lines
  [ ] Pause menu works in Prologue
  [ ] No NullReferenceExceptions in Console
  [ ] Test on target minimum spec (if known)
  [ ] Verify scene index in Build Settings is correct
  [ ] Playtest full prologue cold (fresh scene load)
  CHECKPOINT: Prologue ships
```

---

### 4.4 Out of Scope for Prologue

Do not build these in the Prologue. They belong to later chapters:

- Keypad puzzles
- Inventory system / item pickup
- Pager puzzle sequence
- Phone interactions
- Ladder geometry
- Moving platforms
- Save system (save triggers go in Chapter 01+)
- Any second player character or NPC

---

*End of Prologue Build Guide.*
