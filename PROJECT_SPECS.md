# Starfall Covenant — Full Project Specs

> Paste the block below at the start of any new Claude session to restore full engineering context.

---

## PASTE THIS INTO A NEW SESSION

```
---SPECS START---
PROJECT: Starfall Covenant
TYPE: 2D gacha mobile game
PLATFORMS: iOS 15+, Android 8+
ENGINE: Unity 6.4 (LTS), C#
STORY: See STORY_HANDOVER.md for full narrative context

## Unity Project Location
Path: /Users/williamtong/Projects/iOS App - Claude/StarfallCovenant/Starfall Convenant/
Note: Unity project folder is named "Starfall Convenant" (typo, do not rename — it will break Unity references)
Scripts path: Assets/_Project/Scripts/
Scenes path: Assets/Scenes/

## Tech Stack
| Layer | Choice |
|-------|--------|
| Engine | Unity 6.4 (2D URP) |
| Language | C# |
| UI | Unity uGUI (Canvas) + TextMeshPro |
| Rendering | Unity 2D: Sprite Renderer, Animator, 2D Physics |
| Persistence | Local JSON (JsonUtility to Application.persistentDataPath) |
| Audio | Unity AudioSource + AudioMixer |
| Content Config | ScriptableObjects + JSON |
| Min iOS | 15+ |
| Min Android | 8+ |

## Architecture
UI Layer (Canvas/uGUI) ←→ Manager Singletons ←→ ScriptableObjects (static data) + JSON save (runtime data)

Managers are DontDestroyOnLoad singletons:
- GameManager — central hub, owns PlayerSaveData, exposes currency methods
- SaveManager — reads/writes JSON to persistentDataPath
- ProgressionManager — level unlock logic, chapter queries
- GachaManager — weighted pulls, pity counter
- AudioManager — BGM + SFX

## Namespaces
- StarfallCovenant.Core — GameManager, SaveManager, SceneLoader, PlayerSaveData
- StarfallCovenant.Data — Enums, Stats, OwnedCharacter
- StarfallCovenant.ScriptableObjects — CharacterSO, LevelSO, AbilitySO, GachaBannerSO
- StarfallCovenant.Battle — BattleManager, PlayerController, EnemyController, EnemyAI, AbilitySystem, DamageCalculator, Projectile
- StarfallCovenant.Gacha — GachaManager, GachaAnimator
- StarfallCovenant.Progression — ProgressionManager, LevelUnlocker, CharacterUpgrader
- StarfallCovenant.UI — All UI scripts
- StarfallCovenant.Audio — AudioManager

## Scenes (Build Settings order)
0. MainMenu
1. WorldMap
2. Battle
3. Gacha

## Data Models

### PlayerSaveData (runtime, JSON saved)
- currency: int (premium, gacha pulls)
- softCurrency: int (upgrades)
- ownedCharacters: List<OwnedCharacter>
- activeParty: List<string> (character ids, max 3)
- completedLevels: List<string>
- currentChapter: int
- bannerPityCounters: Dictionary<string, int>

### OwnedCharacter
- id: string (GUID)
- templateId: string (references CharacterSO)
- level: int
- experience: int
- rarity: Rarity enum
- equippedAbilities: List<string>

### Stats
- hp, attack, defense, speed, specialPower: int
- ScaledByLevel(int level) → multiplier: 1 + (level-1) * 0.1

### Enums
- Rarity: Common(1), Rare(2), Epic(3), Legendary(4), Mythic(5)
- Race: Dragon, Elf, Daemon, Yokai, SpaceBeast
- AbilityType: BasicAttack, Dodge, Special, Ultimate
- BattleState: Intro, Active, Victory, Defeat, Paused
- EnemyState: Idle, Telegraph, Attack, Cooldown, Stunned, Dead

## ScriptableObjects (static content, set in Unity Inspector)

### CharacterSO
- characterId, characterName: string
- race: Race, baseRarity: Rarity
- baseStats: Stats
- abilities: AbilitySO[]
- portrait, battleSprite: Sprite
- animatorController: RuntimeAnimatorController
- lore: string

### LevelSO
- levelId, levelName: string
- chapterIndex, levelIndex: int
- storyDialogue: DialogueLine[]
- enemyWaves: EnemyWave[]
- bossConfig: EnemyConfig
- rewards: LevelRewards (currencyReward, softCurrencyReward, experienceReward, firstClearBonus)
- unlockAfterLevelId: string
- backgroundSprite: Sprite

### AbilitySO
- abilityId, abilityName: string
- type: AbilityType
- damage, range: float
- cooldown: float
- vfxPrefab: GameObject
- icon: Sprite

### GachaBannerSO
- bannerId, bannerName: string
- featuredCharacters, characterPool: CharacterSO[]
- rateTable: RarityRate[] (rarity + rate float)
- costPerPull: int (default 160)
- pityThreshold: int (default 90)
- guaranteedPityRarity: Rarity
- bannerArt: Sprite

## Battle System
- Real-time with cooldowns (not turn-based)
- Side-view 2D camera
- Player actions: tap (basic attack), dodge button (i-frames 0.25s, cooldown 1s), 2-3 ability buttons with cooldown timers
- Enemy AI state machine: Idle → Telegraph (0.5s warning) → Attack → Cooldown → Idle
- Boss fights: unique attack patterns, phase transitions
- Collision: Unity 2D Physics BoxCollider2D triggers
- Damage formula: rawDmg = attack + abilityDmg; reduced by defense (100/(100+defense)); 15% crit chance at 1.5x; ±10% variance
- Victory: boss HP = 0. Defeat: player HP = 0.
- On victory: grants currency + softCurrency + XP; marks level complete; first clear bonus

## Gacha System
- Pull cost: 160 currency (1x), 1600 (10x)
- Pity: soft pity starts at 75% of threshold (+5% per pull); hard pity at 90 pulls guarantees Legendary
- Duplicates: added to ownedCharacters (duplicate handling TBD)
- Chapter 1 banner: "Echoes of Oni Station"

## Core Loop
World Map → select level → Story Dialogue → Battle → Victory screen → rewards granted → back to World Map

## Progression
- Levels unlock sequentially via unlockAfterLevelId chain
- XP granted to active party members on level complete
- Character upgrade cost: 100 * 1.5^(level-1) soft currency

## Dialogue System
- DialogueLine[]: speakerName (string), speakerPortrait (Sprite), text (string)
- Typewriter effect at 0.03s per character
- Tap to skip typewriter; tap again to advance
- Triggers before battle in each level
- Max 6-8 lines per pre-battle sequence

## Audio
- BGM tracks: MainMenu, WorldMap, Battle, Boss, Gacha
- SFX: Attack, Hit, Dodge, Ability, Victory, Defeat, GachaPull, GachaReveal, ButtonClick
- AudioManager.PlayBGM("trackName") / PlaySFX("sfxName")

## Known Issues / Notes
- Unity project folder has typo: "Starfall Convenant" not "Starfall Covenant" — do not rename
- Scripts were written to Assets/_Project/ — if not visible in Unity, press Cmd+R to refresh
- TextMeshPro must be installed via Package Manager → Unity Registry → import TMP Essentials
- EnemyAI uses FindAnyObjectByType (not FindFirstObjectByType — deprecated in Unity 6)
- BattleHUD requires `using StarfallCovenant.Core` for GameManager access
---SPECS END---
```

---

## Product Brief

**Game:** Starfall Covenant
**Genre:** 2D gacha action RPG
**Platforms:** iOS + Android
**Engine:** Unity 6.4

### Concept
A 2D gacha game set in a dystopian futuristic galaxy populated by dragons, elves, daemons, yokai, and mythical space creatures. References: Love and DeepSpace (battle mechanics), Isekai: Slow Life (gacha/progression).

### Core Loop
World Map → Story Dialogue → Boss Battle → Rewards → Gacha/Upgrade → repeat

### V1 Scope
- 1 chapter (5 levels + boss per level)
- Real-time 2D battle system
- Gacha with pity mechanic
- Character roster + leveling
- Local save (no server)

---

## Architecture Diagram

```
┌──────────────────────────────────────────────┐
│           UI Layer (Canvas + TMP)             │
│  MainMenu │ WorldMap │ Battle HUD │ Gacha     │
├──────────────────────────────────────────────┤
│           Manager Singletons                  │
│  GameManager │ SaveManager │ AudioManager     │
│  GachaManager │ ProgressionManager            │
├──────────────────────────────────────────────┤
│           Game Logic                          │
│  BattleManager │ EnemyAI │ DamageCalculator   │
│  PlayerController │ AbilitySystem             │
├──────────────────────────────────────────────┤
│           Data Layer                          │
│  ScriptableObjects (static) │ JSON save file  │
└──────────────────────────────────────────────┘
```

---

## Project Folder Structure

```
Assets/
├── _Project/
│   ├── Scripts/
│   │   ├── Core/           GameManager, SaveManager, SceneLoader
│   │   ├── Battle/         BattleManager, PlayerController, EnemyController,
│   │   │                   EnemyAI, AbilitySystem, DamageCalculator, Projectile
│   │   ├── Gacha/          GachaManager, GachaAnimator
│   │   ├── Progression/    ProgressionManager, LevelUnlocker, CharacterUpgrader
│   │   ├── UI/             MainMenuUI, WorldMapUI, BattleHUD, DialogueUI,
│   │   │                   GachaUI, RosterUI, LevelNodeUI
│   │   ├── Data/           Enums, Stats, OwnedCharacter, PlayerSaveData
│   │   │   └── ScriptableObjects/  CharacterSO, LevelSO, AbilitySO, GachaBannerSO
│   │   └── Audio/          AudioManager
│   ├── ScriptableObjects/
│   │   ├── Characters/
│   │   ├── Levels/
│   │   ├── Abilities/
│   │   └── Banners/
│   ├── Prefabs/
│   │   ├── Characters/
│   │   ├── Enemies/
│   │   ├── Projectiles/
│   │   └── VFX/
│   ├── Art/
│   │   ├── Characters/
│   │   ├── Enemies/
│   │   ├── UI/
│   │   ├── Map/
│   │   └── Backgrounds/
│   ├── Animations/
│   └── Audio/
│       ├── BGM/
│       └── SFX/
├── Scenes/         MainMenu, WorldMap, Battle, Gacha
├── Settings/
└── TextMesh Pro/
```

---

## Implementation Phases

| Phase | Scope | Status |
|-------|-------|--------|
| 1 — Foundation | Project setup, data models, ScriptableObjects, SaveManager, MainMenu | ✅ Scripts written |
| 2 — Battle | BattleScene, PlayerController, EnemyAI, BattleHUD, Victory/Defeat | ✅ Scripts written |
| 3 — World Map & Story | WorldMapUI, level nodes, DialogueUI, full level flow | ✅ Scripts written |
| 4 — Gacha & Characters | GachaManager, GachaAnimator, RosterUI, CharacterUpgrader | ✅ Scripts written |
| 5 — Polish | Audio, VFX, animations, balance, platform builds | 🔲 Pending |

---

## Verification Checklist

- [ ] MainMenu loads, buttons navigate correctly
- [ ] WorldMap shows level nodes, locked/unlocked states correct
- [ ] Dialogue plays before battle, tap to advance works
- [ ] Battle: player attacks, dodges, abilities fire
- [ ] Enemy AI telegraphs then attacks
- [ ] Boss dies → Victory screen → rewards granted → saved
- [ ] Player dies → Defeat screen → retry works
- [ ] Gacha pull deducts currency, respects pity counter
- [ ] Character level up costs correct soft currency
- [ ] App kill + relaunch → progress preserved
- [ ] Builds successfully on iOS simulator + Android emulator
