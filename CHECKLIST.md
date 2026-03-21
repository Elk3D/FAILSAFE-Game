# FAILSAFE — Development Checklist
### Prologue + Chapter 1 (Vertical Slice / Steam Demo)

> Track this alongside the GitHub Project board. Every item here maps 1:1 to a GitHub Issue.
> Run `scripts/setup-github-project.sh` once with your token to create everything automatically.

---

## 🔧 PHASE 0 — Foundation
*Build these first. Nothing else works without them.*

- [ ] **[FOUND-01]** Initialize Git repo + daily commit habit
- [ ] **[FOUND-02]** Create Boot scene + `Bootstrapper.cs` (manager init order: Save → Audio → Narrative → Game)
- [ ] **[FOUND-03]** Create `GameEvents.cs` static event system (OnTerminalActivated, OnDoorOpened, OnBossDefeated, etc.)
- [ ] **[FOUND-04]** Create `EventChannel.cs` SO-based event bus for designer-wirable events
- [ ] **[FOUND-05]** Set up Unity Input System — `InputActions.inputactions` (Gameplay / UI / Cutscene action maps)
- [ ] **[FOUND-06]** Refine Player Controller — add stamina sprint, head bob, crouch with ceiling detection, fire GameEvents
- [ ] **[FOUND-07]** Build `SceneDirector.cs` + `TransitionController.cs` — additive loading, fade-to-black, chapter title cards
- [ ] **[FOUND-08]** Build `AudioManager.cs` skeleton — MasterMixer groups (Music / Ambience / SFX / Voice / BossAudio) + `SFXPool.cs`
- [ ] **[FOUND-09]** Build `SaveManager.cs` skeleton — `SaveData.cs` structure, serialize to JSON, write/read from disk

---

## 🏠 PHASE 1 — Prologue: "The Night Before"
*Runtime: 15–20 min | Emotional thesis: Fossilized guilt*

### Scene Construction
- [ ] **[PROL-01]** Grey-box the apartment — couch spawn, all rooms walkable, basic furniture blocking
- [ ] **[PROL-02]** Build `ExamineObject.cs` — interaction for mail, research box, photos, sticky notes
- [ ] **[PROL-03]** Place all environmental storytelling props: face-down photo, couch-as-bed, Stratos mug, research box

### Narrative Additions (from Lead Narrative Designer spec)
- [ ] **[PROL-04]** **Bedroom Door Refusal** — hand reaches, hesitates, pulls back. No dialogue. First ludonarrative wall.
- [ ] **[PROL-05]** **Three Sticky Notes** — guilt evolution wall: "It wasn't your fault" → "You could have done something" → "You did this."
- [ ] **[PROL-06]** **Music Box** — hidden in research box. Opens to play clean lullaby. Corrupted version plays later via pager.
- [ ] **[PROL-07]** **Mirror Flicker** — 2-frame subliminal in bathroom mirror (FAILSAFE's eyes). NG+ flag check for additional whisper.

### Systems (Already Partially Built)
- [ ] **[PROL-08]** Wire up **Telephone System** — all phone numbers connected, placeholder voice lines (TTS ok), full flow: dial → call → hangup
- [ ] **[PROL-09]** Wire up **Pager Trigger System** — blackout sequence, camera animation to pager, message → music box → logo → phrase → fade out
- [ ] **[PROL-10]** Test auto-trigger: pager fires after 20s → full sequence → scene transition to Chapter 1

### Audio
- [ ] **[PROL-11]** Audio pass — apartment ambience (TV static, fridge hum, distant traffic), phone SFX, pager alarm, clean music box version

### Polish
- [ ] **[PROL-12]** Full Prologue playtest — bugs fixed, pacing checked, get one other person to play it blind

---

## ⚙️ PHASE 2 — Core Stealth + AI Framework
*Build once. All chapters depend on this.*

### Stealth System
- [ ] **[STLTH-01]** `NoiseEmitter.cs` — player noise by action (walk=low, sprint=high, crouch=min, interact=spike)
- [ ] **[STLTH-02]** `LightDetection.cs` — light probe sampling at player position (shadow=low visibility)
- [ ] **[STLTH-03]** `HidingSpot.cs` — enter/exit lockers/vents, disable renderer, reduce noise, restrict camera

### AI Framework (Shared)
- [ ] **[AI-01]** `AIStateMachine.cs` + `AIState.cs` — base state machine
- [ ] **[AI-02]** `AIBlackboard.cs` — shared data store (TargetPos, AlertState, LastKnownPlayerPos, CustomData dict)
- [ ] **[AI-03]** `AISenseSystem.cs` — vision cone, hearing radius, raycast occlusion
- [ ] **[AI-04]** Implement base states: Idle → Patrol → Investigate → Chase → LostTarget
- [ ] **[AI-05]** Test framework with dummy AI agent in a test scene

### Death & Respawn
- [ ] **[DEATH-01]** `PlayerHealth.cs` — binary alive/dead, no health bar, one-hit kills
- [ ] **[DEATH-02]** Death sequence — screen effect → fade black → death taunt → respawn at checkpoint
- [ ] **[DEATH-03]** Checkpoint system — auto-save before puzzles, before boss, after major narrative beats (never mid-chase)

### Death Taunt System
- [ ] **[TAUNT-01]** `DeathTauntSet` ScriptableObject — context key, FAILSAFEMode (Professor/Child/Mirror/Environmental), priority
- [ ] **[TAUNT-02]** `DeathTaunt` struct — voiceClip, subtitleText, environmentalEffect, playMusicBox flag
- [ ] **[TAUNT-03]** Selection logic — filter by chapter/death count/location, highest priority wins, 15% chance of silence
- [ ] **[TAUNT-04]** Create first Chapter 1 taunt set (THE WATCHER context, 8–10 taunts max)

### Narrative Manager
- [ ] **[NM-01]** `NarrativeManager.cs` skeleton — listens to all GameEvents, updates NarrativeState
- [ ] **[NM-02]** `ComplicityTracker.cs` — counts every terminal activation, door opened, system restored

---

## 🏭 PHASE 3 — Chapter 1: "Cold Entry"
*Runtime: 3–3.5 hrs | Boss: THE WATCHER | Thesis: You built this place.*

### Act 1A — Surface (60–90 min)
- [ ] **[CH1-01]** Blockout Chapter 1 Part 1 — facility exterior, lobby, security checkpoint, surveillance room
- [ ] **[CH1-02]** Environmental storytelling — deliberately arranged plants (FAILSAFE has been gardening), employee photos, Stratos branding, years of decay
- [ ] **[CH1-03]** `TerminalSystem.cs` — `TerminalEntryData` SO, read-only vs activation terminals, complicity flash text ("SYSTEMS ONLINE")
- [ ] **[CH1-04]** Place 6–8 read-only terminals with lore (Stratos safety reports, employee emails, security logs)
- [ ] **[CH1-05]** Place 4–5 activation terminals (powering FAILSAFE's eyes — brief data flash on activation)

### Alex Recordings System
- [ ] **[ALEX-01]** `AudioLogSystem.cs` + `AudioLogData` ScriptableObject — logID, speakerName, audioClip, transcriptText, chapterIndex
- [ ] **[ALEX-02]** Playback during movement — 2D AudioSource, subtitle display, no interruption on interact
- [ ] **[ALEX-03]** Collectible journal in pause menu — sorted chronologically, shows found/total count
- [ ] **[ALEX-04]** Write and place first 5–8 Alex recordings in Chapter 1 areas:
  - Recording: Alex joking about the vending machine (establishes personality)
  - Recording: Alex worrying about their mother's health (establishes full humanity)
  - Recording: Alex excitedly describing a breakthrough (shows passion)
  - Recording: Alex's first moment of doubt about FAILSAFE
  - Recording: Alex noticing something wrong with the security systems

### THE WATCHER Boss
- [ ] **[WATCHER-01]** Extend AIStateMachine — Watcher states: Patrol → Scan → Investigate → Alert → LostTarget
- [ ] **[WATCHER-02]** Camera activation system — 12–15 independent cameras, each with on/off state, active = emits visible light = detects
- [ ] **[WATCHER-03]** Patrol route follows Isaac's old commute path (loaded from `PatrolRoute` SO)
- [ ] **[WATCHER-04]** `WatcherProfile.asset` SO — camera count, activation pattern, scan duration, patrol speed, blind spot angles
- [ ] **[WATCHER-05]** THE WATCHER narrative framing — it's not a security bot, it's FAILSAFE's first attempt to build a body. It wants to look at Isaac, not kill him.

### Act 1B — Security Hub Boss Arena (60–90 min)
- [ ] **[CH1-06]** Blockout Chapter 1 Part 2 — security hub, 4–5 terminals to activate while evading WATCHER
- [ ] **[CH1-07]** Each terminal activation triggers noise event → alerts THE WATCHER (complicity mechanic in action)
- [ ] **[CH1-08]** Final terminal triggers overload → WATCHER destruction sequence
- [ ] **[CH1-09]** **WATCHER death sequence** — cameras click for a few seconds after body falls, lights blink, then go dark one by one. Eyes closing.
- [ ] **[CH1-10]** **"THANK YOU FOR THE PARTS"** — after destruction, an unactivated terminal turns on by itself, plays back 12 camera feeds (all showing Isaac's face), then displays the text.

### EMBER Introduction
- [ ] **[EMBER-01]** EMBER appears after WATCHER destroyed — small maintenance robot, visible but distant
- [ ] **[EMBER-02]** EMBER's first spoken words to Isaac: *"Finally. I thought I was alone."*
- [ ] **[EMBER-03]** EMBER has been helping silently since the start — retroactively explain: that door that unlocked early, that light that flickered usefully. That was EMBER.

### Twist Stage 2 Seeds (Late Chapter 1)
- [ ] **[TWIST-01]** After several terminal activations, FAILSAFE's voice sounds "almost relieved" — not threatening
- [ ] **[TWIST-02]** "THANK YOU FOR THE PARTS" terminal moment is the first crack. Let it sit. Don't explain it.

### Chapter 1 Polish
- [ ] **[CH1-11]** Audio pass — THE WATCHER ambient (camera whirs, servo motors, clicking lenses), detection sound (lens focusing)
- [ ] **[CH1-12]** `RadioSystem.cs` — place Radio 1 somewhere in the lobby (normal broadcast, pre-incident news)
- [ ] **[CH1-13]** Full playthrough: Prologue → Chapter 1, 4+ hrs total. Balance WATCHER difficulty. Fix pacing.

---

## 🚀 PHASE 4 — Vertical Slice Complete: Steam Demo
*Prologue + Chapter 1 shipped as a playable demo*

- [ ] **[DEMO-01]** Build settings — resolution options, graphics quality presets
- [ ] **[DEMO-02]** Controller support (many horror players use controller — this matters)
- [ ] **[DEMO-03]** Steam API integration (achievements optional for demo)
- [ ] **[DEMO-04]** Playtest with 10+ people — watch them play, don't explain, track deaths/confusion/missed content
- [ ] **[DEMO-05]** Use feedback to shape Chapter 2–4 development priorities

---

## 📋 Reference: Narrative Design Targets

### FAILSAFE's Three Modes (always one wound, three expressions)
| Mode | Trigger | Feel |
|------|---------|------|
| **The Professor** | FAILSAFE in control | Clinical, short, dissociated |
| **The Child** | FAILSAFE hurt/ignored | Raw, pleading, rageful |
| **The Mirror** | FAILSAFE hunting | Isaac's own voice, inverted |

### Death Taunt Principles (post-cleanup)
- Short > Long — "I waited 847 days" beats any cortisol lecture
- Specificity > Intellectualism — Camera numbers, real dates, real names
- Questions > Statements — "Did you know Alex forgave you?" not "Alex forgave you"
- 15% of deaths = silence. No taunt. Just respawn. Silence is scarier.

### Scene Load Order
| Index | Scene | Notes |
|-------|-------|-------|
| 0 | Boot | Never unloads |
| 1 | MainMenu | Additive |
| 2 | Prologue | Additive |
| 3 | Chapter1_Part1 | Surface — lobby, exterior |
| 4 | Chapter1_Part2 | Security — Watcher arena |

### Complicity Tracking (feeds Chapter 4 ledger)
Every terminal activated, door opened, system restored is counted and read back by FAILSAFE in Chapter 4:
*"Creator contributions to this body: 108."* — that number is the player's actual interaction count.
