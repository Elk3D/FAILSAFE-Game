# FAILSAFE — Audio Bible

## Music
- Sparse, tense, ambient electronic / dark ambient
- No melody until Chapter 4 (payoff moment)
- Crossfades managed by `MusicManager`
- File format: `.ogg` (Unity streaming)

## Ambient Zones
- Each room/area has its own `AmbientAudioZone`
- Examples: HVAC hum, dripping water, electrical buzz, distant machinery
- All zones blend in/out — no hard cuts

## Footstep Surfaces
Footstep audio is triggered by `PlayerMovement.cs`. Assets go in:
```
Assets/Audio/SFX/Player/Footsteps/
  Concrete/    — heavy, reverberant
  Wood/        — hollow, creaky
  Metal/       — grating, resonant
  Gravel/      — crunchy
  Carpet/      — muffled, soft
  Water/       — splashing, wet
```
Provide at least 4 variations per surface to avoid repetition.

## SFX Conventions
- All clips: 44.1 kHz, 16-bit, mono (except music = stereo)
- Normalize to -3 dBFS peak
- No built-in reverb — Unity mixers handle room effect

## Dialogue / Narrator
- Narrator: calm, detached, slightly robotic delivery
- Assets: `Assets/Audio/Dialogue/Narrator/`
- Naming: `NAR_{chapter}_{line number}_{short_description}.ogg`
  - Example: `NAR_01_003_you_shouldnt_be_here.ogg`

## UI Sounds
- Menu hover: subtle click
- Menu confirm: clean chime
- Menu back: soft thud
- Keypad press: mechanical click
- Keypad correct: ascending tone
- Keypad wrong: descending buzz
