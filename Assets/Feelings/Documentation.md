# Feelings

Feelings is a system that allows you to give personality to your gaThe happier the character is, the more likely he'll join the quest.

## Overview

Feeling provides a simple FeelingsMap class, which you inherit from, and define the list of feelings and their relationships. The package includes 3 predefined implementations:

- Simple Love/Hate map
- Basic 6 feelings map: Sad, Mad, Scared, Joyful, Powerful, Peaceful
- Complex 42 feelings map: Similar to the basic 6 feelings, but with “sub-feelings”

## Usage

### Creating a new feelings map

Start by inheriting from the FeelingsMap class:

```csharp
public class SampleFeelingMap : FeelingsMap
{
}
```

Define the list of feelings as consts – this is not required, but makes things much easier to work with:

```csharp
public const string Happy = "Happy";

public const string Sad = "Sad";
```

The define the relations between those feelings in the constructor:

```csharp
public SampleFeelingMap()
{
    SetEffect(Happy, Sad, -1); // When I'm happy, I'm not Sad
    SetEffect(Sad, Happy, -1); // When I'm sad, I'm not happy
}
```

Here’s the full sample:

```csharp
public class SampleFeelingMap : FeelingsMap
{
    public const string Happy = "Happy";
    public const string Sad = "Sad";
    public SampleFeelingMap()
    {
        SetEffect(Happy, Sad, -1); // When I'm happy, I'm not Sad
        SetEffect(Sad, Happy, -1); // When I'm sad, I'm not happy
    }
}
```

### Using a feelings map

Once you’ve created your feelings map, using it is quite simple. You can create an instance of the feelings map as a field:

private SampleFeelingMap feelingMap = new SampleFeelingMap();

Whenever something happens to your character, you apply feelings to it. For example, say the player gave a gift to the character – the character is now happier, so we’ll call the ApplyFeeling method:

```csharp
feelingMap.ApplyFeeling(Happy, 2);
```

Now, whenever the character needs to make a decision, we can take its feelings under consideration. For example, will the character join the player on a quest?

```csharp
var characterHappinest = feelingMap.GetFeeling(Happy) / 100.0f;

var joinQuest = Random.value > characterHappinest;
```

The happier the character is, the more likely he’ll join the quest.

## Samples Scenes

The package comes with 2 sample scenes:

1. Simple: this scene shows using a simple UI the impact of each feeling, in a basic 6 feelings map, on the other feelings. It uses the predefined 6 feelings map.
2. Merchants: this scene shows how actions taken by the player can affect the prices requested by 2 merchants – one honest, the other dishonest.

## Persistence System

The Feelings Persistence System provides comprehensive data serialization and loading capabilities for the Unity Feelings emotion management system. It ## Overview

Feeling provides a simple FeelingsMap class, which you inherit from, and define the list of feelings and their relationships. The package includes 3 predefined implementations:

- Simple Love/Hate map
- Basic 6 feelings map: Sad, Mad, Scared, Joyful, Powerful, Peaceful
- Complex 42 feelings map: Similar to the basic 6 feelings, but with "sub-feelings"

The system also includes a comprehensive persistence system that allows you to save and load character emotional states using multiple serialization formats with automatic fallback and error handling.

### Usage

#### Basic Usage

```csharp
// Create a feelings map
var feelingsMap = new BasicFeelingsMap();

// Set some feelings
feelingsMap.ApplyFeeling(BasicFeelingsMap.Joyful, 30f);
feelingsMap.ApplyFeeling(BasicFeelingsMap.Mad, -20f);

// Save to file
string filePath = Path.Combine(Application.persistentDataPath, "my_feelings");
bool saved = feelingsMap.Save(filePath);

// Load from file
bool loaded = feelingsMap.Load(filePath);
```

#### Advanced Usage

```csharp
// Save with specific serializer
feelingsMap.Save(filePath, "JsonUtility");

// Load with fallback (tries multiple serializers)
feelingsMap.LoadWithFallback(filePath);

// Direct persistence manager access
var manager = feelingsMap.PersistenceManager;
var snapshot = feelingsMap.CreateSnapshot();
manager.Save(snapshot, filePath, "Binary");
```

### Supported Serializers

| Serializer         | Description                      | Dependencies    | File Size | Performance |
|--------------------|----------------------------------|-----------------|-----------|-------------|
| **JsonUtility**    | Unity's built-in JSON serializer | None            | Medium    | Fast        |
| **Binary**         | .NET binary serialization        | None            | Small     | Very Fast   |
| **PlayerPrefs**    | Unity PlayerPrefs system         | None            | N/A       | Fast        |
| **NewtonsoftJson** | Newtonsoft.Json library          | Newtonsoft.Json | Medium    | Medium      |
| **EasySave3**      | Easy Save 3 asset                | Easy Save 3     | Small     | Fast        |
| **OdinSerializer** | Odin Inspector serializer        | Odin Inspector  | Small     | Fast        |
| **CloudSave**      | Unity Cloud Save                 | Unity Services  | N/A       | Slow        |

### Architecture

#### Core Components

1. **IFeelingsSerializer Interface**: Defines serialization contract
2. **FeelingsPeristenceManager**: Coordinates multiple serializers
3. **FeelingsData**: Serializable data structure
4. **Individual Serializers**: Format-specific implementations

#### Data Structure

```csharp
[Serializable]
public class FeelingsData
{
    public string version;           // Format version
    public FeelingsEntry[] feelings; // Feeling values
    public EffectsEntry[] effects;   // Cascading effects
    public long timestamp;           // Save timestamp
}
```

### Compilation Flags

The system uses conditional compilation to handle optional dependencies:

- `EASYSAVE3`: Enable Easy Save 3 serializer  
- `ODIN_INSPECTOR`: Enable Odin serializer
- `UNITY_CLOUD_BUILD_API || UNITY_SERVICES_CORE`: Enable Unity Cloud Save serializer

Add these flags in **Player Settings > Scripting Define Symbols** when the corresponding packages are installed.

### Integration with FeelingsMap

The persistence system is fully integrated into the `FeelingsMap` class:

#### Available Methods

```csharp
// Save/Load with default serializer
bool Save(string location)
bool Load(string location)

// Save/Load with specific serializer  
bool Save(string location, string serializerName)
bool Load(string location, string serializerName)

// Advanced loading with fallback
bool LoadWithFallback(string location)

// Snapshot operations
FeelingsData CreateSnapshot()
void RestoreFromSnapshot(FeelingsData data)

// Persistence manager access
static FeelingsPeristenceManager PersistenceManager { get; }
```

#### Thread Safety

All persistence operations are thread-safe and can be called from background threads. The `FeelingsMap` uses internal locking to ensure data consistency during save/load operations.

### Error Handling

The system provides comprehensive error handling:

- **File Not Found**: Returns `false` from load operations
- **Serialization Errors**: Logged with details, operations fail gracefully
- **Invalid Data**: Validation prevents corrupted data from being loaded
- **Missing Dependencies**: Serializers check for required components

### Performance Considerations

#### Recommendations by Use Case

- **Frequent Saves**: Use `Binary` or `PlayerPrefs`
- **Human Readable**: Use `JsonUtility` or `NewtonsoftJson`
- **Cross-Platform**: Use `JsonUtility` or `Binary`
- **Cloud Backup**: Use `CloudSave`
- **Asset Store Integration**: Use `EasySave3` or `OdinSerializer`

#### Optimization Tips

1. **Batch Operations**: Create snapshots once, save to multiple formats
2. **Background Saves**: Use threading for large data sets
3. **Compression**: Binary and Odin serializers provide smallest files
4. **Validation**: Validate data before restoration to prevent errors

### Examples

#### Example 1: Game Save System

```csharp
public class GameSaveSystem : MonoBehaviour
{
    private BasicFeelingsMap characterEmotions;
    
    void SaveGame()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "character_emotions");
        
        // Save with primary serializer
        if (!characterEmotions.Save(savePath, "JsonUtility"))
        {
            // Fallback to binary if JSON fails
            characterEmotions.Save(savePath + "_backup", "Binary");
        }
    }
    
    void LoadGame()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "character_emotions");
        
        // Try loading with fallback
        if (!characterEmotions.LoadWithFallback(savePath))
        {
            // Initialize with defaults if load fails
            InitializeDefaultEmotions();
        }
    }
}
```

#### Example 2: Multiple Characters

```csharp
public class MultiCharacterSystem : MonoBehaviour
{
    private Dictionary<string, FeelingsMap> characterEmotions;
    
    void SaveAllCharacters()
    {
        foreach (var kvp in characterEmotions)
        {
            string characterName = kvp.Key;
            var emotions = kvp.Value;
            
            string filePath = Path.Combine(Application.persistentDataPath, 
                $"character_{characterName}_emotions");
                
            emotions.Save(filePath);
        }
    }
    
    void LoadCharacter(string characterName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, 
            $"character_{characterName}_emotions");
            
        var emotions = new BasicFeelingsMap();
        if (emotions.Load(filePath))
        {
            characterEmotions[characterName] = emotions;
        }
    }
}
```

#### Example 3: Serializer Comparison

```csharp
void CompareSerializers(FeelingsMap feelings)
{
    var snapshot = feelings.CreateSnapshot();
    var manager = feelings.PersistenceManager;
    
    foreach (var serializerName in manager.GetAvailableSerializers())
    {
        string filePath = $"test_{serializerName.ToLower()}";
        
        // Time the save operation
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        bool success = manager.Save(snapshot, filePath, serializerName);
        stopwatch.Stop();
        
        if (success)
        {
            var fileInfo = new FileInfo(filePath);
            Debug.Log($"{serializerName}: {fileInfo.Length} bytes, {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
```

### Testing

#### Integration Test

Run the `PersistenceIntegrationTest` component to verify:

1. Basic save/load functionality
2. State preservation accuracy
3. Multiple serializer compatibility
4. Error handling robustness

#### Manual Testing

1. Add `PersistenceExample` to a GameObject
2. Run the scene
3. Check console output for test results
4. Use context menu options for interactive testing

### Troubleshooting

#### Common Issues

**Problem**: Serializer not available
- **Solution**: Check compilation flags and package dependencies

**Problem**: Save operation fails silently
- **Solution**: Enable debug logging in `FeelingsPeristenceManager`

**Problem**: Load returns incorrect data
- **Solution**: Verify file exists and isn't corrupted; try fallback loading

**Problem**: Performance issues
- **Solution**: Use binary serialization for large datasets

#### Debug Information

Enable verbose logging by modifying `FeelingsPeristenceManager`:

```csharp
private const bool ENABLE_DEBUG_LOGGING = true; // Set to true for debugging
```

This will output detailed information about serialization operations to the Unity Console.

### Version Compatibility

- **Unity**: 2017.2.0f3 or later
- **.NET Framework**: 3.5 or later
- **Newtonsoft.Json**: Any version compatible with Unity
- **Easy Save 3**: Version 3.0 or later  
- **Odin Inspector**: Version 2.0 or later

### Future Enhancements

Planned improvements include:

1. **Async Operations**: Non-blocking save/load operations
2. **Compression**: Built-in data compression options
3. **Encryption**: Secure save data protection
4. **Version Migration**: Automatic data format upgrades
5. **Network Sync**: Real-time emotion synchronization

## Samples Scenes

By maintaining a graph of relations between various feelings, the character evolves as the player interacts with him.

For example, in an RPG game, the system can be give merchant a unique personality, such as an honest versus dishonest merchant, and have them adjust to the player behavior whenever he acts, such as buying something, threaten them, or tries to bribe them.
