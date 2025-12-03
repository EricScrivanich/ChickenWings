# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**ChickenWings** is a 2D side-scrolling action game built in Unity 6 (6000.0.34f1). The player controls a chicken flying through levels, defeating pig enemies and bosses while collecting eggs and rings. The game features a sophisticated level recording system for deterministic, replay-able gameplay.

## Build & Development

This is a Unity project - open in Unity Hub or Unity Editor directly. No command-line build system.

- **Unity Version:** 6000.0.34f1
- **Main Scenes:** `Assets/Scenes/Main/`, `Assets/Scenes/Boss/`
- **Level Editor Scene:** `Assets/Scenes/Boss/LevelCreator.unity`

## Core Architecture

### Level Data System (The Heart)

The game uses a hybrid serialization approach:

- **LevelData.cs** - ScriptableObject holding level configuration
- **LevelDataArrays.cs** - Optimized array storage for object positions/parameters
- **LevelDataSave.cs** - Serializable data for runtime persistence
- **LevelDataConverter.cs** - Singleton managing level loading/saving (JSON)

Objects are stored as typed DataStructs (Simple, FloatOne through FloatFive) based on parameter count. This enables memory-efficient storage of varying complexity.

### Spawn Step System

Time is quantized into "spawn steps" (~0.18 seconds each). Each level has an array of `spawnSteps` indicating when to spawn objects. This creates deterministic, replay-able gameplay where all behavior is fully determined by spawn step and parameters.

### Object Type Classification

- **Type 0:** Pigs (Bosses)
- **Type 1:** AI Enemies (drones, planes)
- **Type 2:** Buildings/Structures
- **Type 3:** Collectables (eggs, barrels)
- **Type 4:** Positioners (game state modifiers)
- **Type 5:** Rings & Collectibles
- **Negative Types:** Special positioning objects

### Key Systems

**SpawnStateManager.cs** (~1,700 lines) - Central spawner managing object pools and spawn transitions. Controls 6 pool types: Pigs, AI, Buildings, Collectables, Positioners, Rings.

**LevelRecordManager.cs** (~2,500 lines) - Master controller for the in-game level editor. Handles time step tracking, multi-object selection, timeline visualization, and playback.

**RecordableObjectPlacer.cs** - Editor representation of spawnable objects. Stores position, spawn/despawn times, and 18 editable parameters (Speed, Size, Jump_Height, Rotation, etc.).

### State Machine Pattern

Used extensively for enemies and bosses:
```csharp
public class PigStateManager : MonoBehaviour {
    PigBaseState currentState;
    void Update() => currentState.UpdateState(this);
    void SwitchState(PigBaseState newState) => currentState = newState;
}
```

## Key Folders

```
Assets/
├── _Folders/
│   ├── BOSS/                 # Boss implementations (Pigs, Helicopter, DeviledEgg)
│   ├── ENEMIES/              # Non-boss enemies (Planes, Drones, Silo, etc.)
│   ├── COLLECTIBLE/          # Pickups (Egg, Rings, Barn)
│   ├── _Player/              # Player controller and collision
│   ├── _NEWRecordingLogic/   # Level recording/editing system
│   ├── _Placeholders&Setups/ # SpawnStateManager and spawn configuration
│   └── _Manager/             # Game managers (Player, Score, Audio, etc.)
├── Levels/
│   ├── Main/                 # Main game levels (.asset files)
│   ├── Challenges/           # Challenge mode levels
│   └── Resources/            # Serialized LevelDataArrays
├── Editor/                   # Custom editor tools
│   ├── LevelCreatorNew.cs    # Main level creation tool
│   └── LevelDataArrayEditor.cs
└── Scenes/
    ├── Main/                 # Main gameplay scenes
    └── Boss/                 # Boss fight scenes
```

## Data Flow

```
LevelDataConverter.LoadLevel()
    → LevelData.InitializeData()
    → Creates DataStructs from LevelDataArrays
    → Instantiates RecordableObjectPool per type
    → SpawnStateManager spawns objects at spawn steps
    → LevelRecordManager (if in editor mode) allows editing
```

## Important Patterns

1. **Two-Layer Object Definition**
   - Editor layer: RecordableObjectPlacer (design-time)
   - Runtime layer: RecordableObjectPool (play-time)

2. **Pool Pattern** - All spawnable objects use object pooling via RecordableObjectPool

3. **ScriptableObjects** - Extensively used for configuration (LevelData, PigID, PlaneSpawnerID, etc.)

## Critical Notes

- Never modify `spawnSteps` directly - it controls when objects appear
- Pool creation is dynamic - don't hardcode pool sizes
- LevelData initialization handles three paths: normal, save load, checkpoint
- Positioner objects use negative type values - treated differently in spawning
- RandomSpawnData requires coordinated RNG values with spawn step timing

## Recommended Reading Order

1. `Assets/Levels/LevelData.cs` - Data structure foundation
2. `Assets/_Folders/_Placeholders&Setups/SpawnStateManager.cs` - Spawning system
3. `Assets/_Folders/_NEWRecordingLogic/LevelRecordManager.cs` - Editor system
4. `Assets/_Folders/BOSS/Pigs/_Script/PigStateManager.cs` - Enemy behavior pattern
5. `Assets/_Folders/_Player/_Script/Player_S.cs` - Player controller
