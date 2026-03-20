# FAILSAFE — Art Bible

## Visual Direction
Cold, clinical, brutalist architecture. Think abandoned research facility meets liminal space.
Pale fluorescent lighting occasionally flickering. Long corridors. Heavy doors. Rust and decay.

## Colour Palette
| Role              | Hex       |
|-------------------|-----------|
| Primary BG        | `#0D0D0D` |
| Concrete / walls  | `#3A3A3A` |
| Accent / warning  | `#E05000` |
| Terminal green    | `#39FF14` |
| Cold white light  | `#C8D8E8` |
| Rust              | `#7B3F00` |

## Lighting
- Baked + mixed mode
- Point lights for emergency signage (red/amber)
- Spot lights for task lighting
- LightFlicker component on degraded fluorescents

## Post-Processing
- Slight chromatic aberration
- Film grain (subtle)
- Vignette
- Bloom on light sources

## Model Specs
- Props: max 2000 tris for small items, 8000 for large env pieces
- Textures: 1024×1024 for props, 2048×2048 for large surfaces
- Normal maps required for all environment surfaces

## Folder Conventions
```
Assets/Art/Models/Environment/Indoor/   — walls, floors, ceilings, doors
Assets/Art/Models/Environment/Props/    — furniture, equipment, clutter
Assets/Art/Models/Items/                — collectible items
Assets/Art/Textures/                    — same subfolder mirroring
Assets/Art/Materials/                   — one material per surface type
```
