# Feelings Persistence System

The Feelings Persistence System provides comprehensive support for saving and loading feelings data across multiple serialization formats. This modular system automatically detects available serializers and provides a unified API for all persistence operations.

## ✅ **Supported Serialization Methods**

| Serializer | Availability | Compilation Flag | Features |
|------------|--------------|------------------|----------|
| **Unity JsonUtility** | Always | None | Built-in, fast, recommended |
| **Newtonsoft.Json** | Package | `NEWTONSOFT_JSON` | Dictionary support, advanced features |
| **Binary Formatter** | Always | None | Compact, fast, platform-dependent |
| **Easy Save 3** | Asset Store | `EASYSAVE3` | Easy to use, encryption support |
| **Odin Serializer** | Asset Store | `ODIN_INSPECTOR` | Advanced Unity object support |
| **PlayerPrefs** | Always | None | Simple, cross-platform |
| **Unity Cloud Save** | Package | `UNITY_CLOUD_BUILD_API` | Cross-device sync |

## 🚀 **Quick Start**

### Basic Save/Load
```csharp
// Create and configure your feelings map
var feelingsMap = new BasicFeelingsMap();
feelingsMap.ApplyFeeling(BasicFeelingsMap.Joyful, 50.0f);

// Save with default serializer (automatically selected)
bool saveSuccess = feelingsMap.Save("player_emotions");

// Load back the data
var newMap = new BasicFeelingsMap();
bool loadSuccess = newMap.Load("player_emotions");
```

### Using Specific Serializers
```csharp
// Save with Unity JsonUtility
feelingsMap.Save("emotions.json", "Unity JsonUtility");

// Save with Newtonsoft.Json (if available)
feelingsMap.Save("emotions_advanced.json", "Newtonsoft.Json");

// Save with Easy Save 3 (if available)
feelingsMap.Save("emotions_easy", "Easy Save 3");
```

### Fallback Loading (Recommended)
```csharp
// Tries multiple serializers for maximum compatibility
bool success = feelingsMap.LoadWithFallback("player_emotions");
```

## 🏗️ **Architecture**

### Core Components

1. **IFeelingsSerializer Interface**
   - Common interface for all serializers
   - Standardized Save/Load/Exists/Delete operations

2. **FeelingsData Class**
   - Universal data structure for serialization
   - Compatible with all serializer implementations

3. **FeelingsPeristenceManager**
   - Central coordinator for all persistence operations
   - Automatic serializer detection and selection

4. **Individual Serializer Classes**
   - Each serialization method in its own file
   - Compilation flags prevent dependency issues

## 📁 **File Structure**

```
Assets/Feelings/Persistence/
├── IFeelingsSerializer.cs          # Common interface & data structure
├── JsonUtilitySerializer.cs        # Unity JsonUtility implementation
├── NewtonsoftJsonSerializer.cs     # Newtonsoft.Json implementation
├── BinarySerializer.cs             # Binary serialization
├── EasySave3Serializer.cs          # Easy Save 3 implementation
├── OdinSerializer.cs               # Odin Serializer implementation
├── PlayerPrefsSerializer.cs        # PlayerPrefs implementation
├── CloudSaveSerializer.cs          # Unity Cloud Save implementation
└── FeelingsPeristenceManager.cs    # Central manager
```

## ⚙️ **Configuration**

### Compilation Flags

Add these scripting define symbols in **Project Settings > Player > Other Settings > Scripting Define Symbols** when you have the corresponding packages/assets:

- `NEWTONSOFT_JSON` - When Newtonsoft.Json package is installed
- `EASYSAVE3` - When Easy Save 3 asset is imported
- `ODIN_INSPECTOR` - When Odin Inspector asset is imported
- `UNITY_CLOUD_BUILD_API` - When Unity Services packages are installed

### Default Serializer Priority

The system automatically selects the best available serializer in this order:

1. Easy Save 3 (if available)
2. Newtonsoft.Json (if available)
3. Unity JsonUtility
4. Odin Serializer (if available)
5. Binary Formatter
6. Unity PlayerPrefs
7. Unity Cloud Save (if available)

## 🔧 **Advanced Usage**

### Custom Serializer Selection
```csharp
// Get the persistence manager
var manager = FeelingsMap.PersistenceManager;

// Change default serializer
manager.SetDefaultSerializer("Newtonsoft.Json");

// Get serializer information
var status = manager.GetSerializerStatus();
foreach (var kvp in status)
{
    Debug.Log($"{kvp.Key}: {(kvp.Value ? "Available" : "Not Available")}");
}
```

### Direct Serializer Access
```csharp
// Get a specific serializer
var jsonSerializer = FeelingsMap.PersistenceManager.GetSerializer("Unity JsonUtility");

// Use it directly with FeelingsData
var snapshot = feelingsMap.CreateSnapshot();
jsonSerializer.Save(snapshot, "direct_save.json");

// Load directly
var loadedData = jsonSerializer.Load("direct_save.json");
if (loadedData != null)
{
    feelingsMap.RestoreFromSnapshot(loadedData);
}
```

### Migration Between Formats
```csharp
// Load with any available serializer
var data = FeelingsMap.PersistenceManager.LoadWithFallback("old_format_file");

// Save in new format
FeelingsMap.PersistenceManager.Save(data, "new_format_file", "Newtonsoft.Json");
```

## 📊 **Format Comparison**

### File Size Comparison (Approximate)
- **Binary**: ~60% smaller than JSON
- **Unity JsonUtility**: Baseline JSON size
- **Newtonsoft.Json**: ~20% larger (due to metadata)
- **PlayerPrefs**: N/A (stored in registry/preferences)

### Performance Comparison
- **Binary**: Fastest save/load
- **Unity JsonUtility**: Fast, good balance
- **Newtonsoft.Json**: Slower but more features
- **Easy Save 3**: Fast with additional features
- **PlayerPrefs**: Fast for small data

### Feature Comparison
| Feature | JsonUtility | Newtonsoft | Binary | EasySave3 | Odin | PlayerPrefs | CloudSave |
|---------|-------------|------------|--------|-----------|------|-------------|-----------|
| Dictionary Support | ❌ | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ |
| Human Readable | ✅ | ✅ | ❌ | Configurable | Configurable | ❌ | ✅ |
| Cross Platform | ✅ | ✅ | ❌ | ✅ | ✅ | ✅ | ✅ |
| Encryption | ❌ | ❌ | ❌ | ✅ | ✅ | ❌ | ✅ |
| Cloud Sync | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ |

## 🎯 **Best Practices**

### 1. Choose the Right Serializer

**For most projects:** Unity JsonUtility
```csharp
// Simple, fast, built-in
feelingsMap.Save("emotions.json"); // Uses default (JsonUtility)
```

**For complex data:** Newtonsoft.Json
```csharp
// When you need Dictionary support or advanced features
feelingsMap.Save("emotions.json", "Newtonsoft.Json");
```

**For commercial projects:** Easy Save 3
```csharp
// Best user experience, encryption, compression
feelingsMap.Save("emotions", "Easy Save 3");
```

**For cross-device:** Unity Cloud Save
```csharp
// Sync between devices
feelingsMap.Save("emotions", "Unity Cloud Save");
```

### 2. Error Handling
```csharp
public bool SafeSave(FeelingsMap map, string key)
{
    try
    {
        // Try primary method
        if (map.Save(key))
        {
            return true;
        }
        
        // Fallback to PlayerPrefs
        return map.Save(key, "Unity PlayerPrefs");
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"Save failed: {ex.Message}");
        return false;
    }
}
```

### 3. Version Management
```csharp
// The FeelingsData includes version information
var snapshot = feelingsMap.CreateSnapshot();
Debug.Log($"Data version: {snapshot.version}");

// You can check versions when loading
var loadedData = manager.Load("save_file");
if (loadedData != null && loadedData.version == "1.0")
{
    // Handle version-specific loading
}
```

## 🐛 **Troubleshooting**

### Common Issues

**"Serializer not available" errors:**
- Check that the required package/asset is imported
- Verify compilation flags are set correctly
- Use `FeelingsMap.PersistenceManager.LogSerializerInfo()` to debug

**Dictionary serialization fails with JsonUtility:**
- This is expected - JsonUtility doesn't support dictionaries
- The system automatically converts to arrays for JsonUtility
- Use Newtonsoft.Json if you need direct dictionary support

**Platform-specific issues with Binary serialization:**
- Binary format is not cross-platform compatible
- Use JSON formats for cross-platform saves

**PlayerPrefs limitations:**
- Limited data size on some platforms
- Use for simple saves only (< 1MB recommended)

## 📝 **Examples**

See `Assets/Feelings/Examples/PersistenceExample.cs` for a complete working example that demonstrates:

- Automatic serializer detection
- Save/Load operations
- Data integrity verification
- Fallback loading
- All serializer testing

## 🔄 **Migration Guide**

### From Manual Serialization
If you were using manual JSON serialization:

```csharp
// Old way
string json = JsonUtility.ToJson(someData);
File.WriteAllText(path, json);

// New way
feelingsMap.Save("emotions"); // Handles everything automatically
```

### Between Different Serializers
```csharp
// Load from old format
bool loaded = feelingsMap.LoadWithFallback("old_save");

if (loaded)
{
    // Save in new format
    feelingsMap.Save("new_save", "Newtonsoft.Json");
}
```

This persistence system provides maximum flexibility while maintaining ease of use, ensuring your feelings data can be saved and loaded reliably across all target platforms and scenarios.