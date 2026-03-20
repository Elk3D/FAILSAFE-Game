# FAILSAFE — Game Design Document

## Overview
FAILSAFE is a first-person atmospheric walking simulator / narrative adventure built in Unity.
The player explores a mysterious facility, uncovering a story told through environmental details,
collectable notes, voiced narrator lines, and cinematic epigraph sequences between chapters.

---

## Core Pillars
1. **Atmosphere** — Every space should feel intentional. Lighting, sound, and pacing create dread or wonder.
2. **Narrative Discovery** — Story is revealed through exploration, not cutscenes.
3. **Immersive Movement** — Headbob, footsteps, crouch, and ladder systems ground the player physically.
4. **Puzzle as Story** — Puzzles unlock narrative payoffs, not arbitrary rewards.

---

## Chapter Structure

| # | Chapter Name        | Setting                       | Key Mechanic         |
|---|---------------------|-------------------------------|----------------------|
| 1 | Awakening           | Entry corridor / lobby        | Basic exploration    |
| 2 | Descent             | Underground labs              | Keypad / sequence    |
| 3 | Truth               | Archive / server room         | Note-reading, items  |
| 4 | Failsafe            | Core chamber                  | Final puzzle         |
| — | End Game            | Escape / epilogue             | —                    |

---

## Player Abilities
- Walk, run, crouch, jump
- Ladder climbing
- Object interaction (examine, pick up, use)
- Inventory (item-based door unlocking)
- Reading notes/documents

---

## Puzzle Types
- **Keypad** — Enter a numeric code found in the environment
- **Sequence** — Activate switches/buttons in the correct order
- **Environment** — Place objects on pressure plates / align symbols
- **Item-gate** — Locked door requires a specific item

---

## Narrative Devices
- **Epigraph System** — Full-screen quote + author between scenes
- **Narrator VO** — Triggered dialogue lines (NarrativeTrigger zones)
- **Notes** — Readable documents scattered in the environment
- **Subtitles** — Captioned narrator/character lines (toggle in settings)

---

## Audio Design
- Dynamic ambient zones (blend in/out as player moves)
- Music crossfades between chapters via MusicManager
- Footstep surface variety: concrete, wood, metal, gravel, carpet, water
- Fall impact audio
