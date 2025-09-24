# Feelings - Unity AI Behavior System

## Overview
This is a Unity 2017.2.0f3 project implementing an AI feelings/emotion system for NPCs or game characters. The core `FeelingsMap` class manages interconnected emotions with cascading effects, where affecting one feeling automatically influences related ones based on predefined ratios.

## Core Architecture

### FeelingsMap System
- **Core Class**: `Assets/Feelings/FeelingsMap.cs` - Base emotion management system
- **Value Range**: All feelings are clamped between -100.0 and 100.0
- **Effect Cascading**: Applying a feeling triggers effects on related feelings using configured ratios
- **Circular Reference Protection**: Uses `HashSet<string> handled` to prevent infinite loops

### Key Methods Pattern
```csharp
// Get current feeling value
float GetFeeling(string feelingName)

// Apply delta and trigger cascading effects  
float ApplyFeeling(string feelingName, float delta)

// Configure relationships between feelings
protected void SetEffect(string feelingA, string feelingB, float ratio)
```

### Predefined Maps (Inheritance Pattern)
Located in `Assets/Feelings/PredefinedMaps/`:
- **BasicFeelingsMap**: 6 core emotions (Mad, Scared, Joyful, Powerful, Peaceful, Sad) with opposing pairs
- **LoveHateMap**: Simple binary opposite system  
- **ComplexFeelingsMap**: Extended emotion set with nuanced relationships

### Sample Implementation Pattern
Found in `Assets/Feelings/Samples/Scripts/`:
- Merchant behavior maps (`HonestMerchangeMap`, `DishonestMerchangeMap`) show practical game applications
- UI integration examples (`SampleUI`, `SampleMerchantsUI`) demonstrate Unity component integration

## Development Conventions

### Feeling Name Constants
Always define feeling names as public const strings in derived classes:
```csharp
public const string Happy = "Happy";
public const string Angry = "Angry";
```

### Effect Configuration Pattern
Configure relationships in constructor using opposing pairs and cascading effects:
```csharp
// Direct opposites (full negative effect)
SetEffect(Love, Hate, -1);
SetEffect(Hate, Love, -1);

// Partial influences  
SetEffect(Happy, Trusting, 0.5f);
SetEffect(Angry, Happy, -2f);
```

### Unity Integration
- Extend FeelingsMap for specific NPC types or game scenarios
- Use Unity UI Sliders for real-time feeling visualization
- Implement action methods that apply feelings (BuySomething(), Threaten(), etc.)

## Project Structure
- **Assets/Feelings/**: Core system files
- **Assets/Feelings/PredefinedMaps/**: Ready-to-use emotion configurations
- **Assets/Feelings/Samples/**: Example implementations and UI demos
- **Feelings.csproj**: Unity 2017.2 project targeting .NET Framework 3.5

## Testing & Debugging
- Use `[DebuggerDisplay("{Name}: {Value}")]` attribute on Feeling class for easier debugging
- UI sliders in samples provide visual feedback for testing emotion interactions
- Merchant samples demonstrate practical price calculation based on feeling states

## Key Design Decisions
- String-based feeling identification allows dynamic emotion creation
- Dictionary storage enables runtime emotion addition without code changes  
- Ratio-based effects provide fine-tuned control over emotion interactions
- Value clamping prevents unrealistic emotion extremes