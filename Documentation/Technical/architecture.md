# FAILSAFE — Technical Architecture

## Namespace Structure
All game scripts live under the `FAILSAFE` root namespace:

```
FAILSAFE
├── Player          — Movement, camera, stats, inventory
├── Interaction     — Interactable base, pickups, examine
├── Gameplay
│   ├── Puzzles     — PuzzleBase + concrete puzzle types
│   ├── Triggers    — Scene/audio/narrative/cutscene triggers
│   ├── Doors       — Door base + locked/sliding variants
│   └── Items       — ItemData ScriptableObject
├── Narrative       — Chapters, dialogue, story events
├── Audio           — MusicManager, AmbientAudioZone
├── Environment     — LightFlicker, Ladder, MovingPlatform
├── Cutscenes       — CutsceneController (wraps Timeline)
├── UI              — HUD, menus, subtitles, notes viewer
├── Managers        — Game/Save/Audio/Settings singletons
└── Utilities       — Singleton<T>, Extensions, Constants
```

## Scene Flow
```
MainMenu
  └─► Chapter01_Awakening (via EpigraphSystem)
        └─► Chapter02_Descent
              └─► Chapter03_Truth
                    └─► Chapter04_Failsafe
                          └─► EndGame
```

## Singleton Managers (DontDestroyOnLoad)
- `GameManager`     — Game state machine
- `AudioManager`    — Central audio routing
- `SettingsManager` — Persisted player settings

## Save System
- JSON file written to `Application.persistentDataPath`
- Saves: current chapter, collected item IDs, completed puzzle IDs, playtime

## Input
- Uses Unity's legacy Input Manager
- Key buttons: Interact, Pause, Squint/Looking (from Interact.cs)
- Mouse axes: Mouse X / Mouse Y (MouseLook.cs)

## Dependency Graph (simplified)
```
GameManager
  ├── SaveManager
  ├── AudioManager ←── MusicManager, AmbientAudioZone
  └── ChapterManager

Player GameObject
  ├── PlayerMovement
  ├── MouseLook
  ├── PlayerStats
  ├── PlayerInventory
  └── Interact

UI Canvas
  ├── HUDController
  ├── SubtitleSystem
  ├── PauseMenu
  ├── NotesViewer
  └── EpigraphSystem
```
