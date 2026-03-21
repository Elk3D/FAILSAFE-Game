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
