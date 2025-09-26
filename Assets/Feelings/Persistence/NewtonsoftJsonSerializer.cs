using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Feelings.Persistence
{
    /// <summary>
    /// Newtonsoft.Json serializer with full Dictionary support and advanced features.
    /// Requires Newtonsoft.Json package: com.unity.nuget.newtonsoft-json
    /// </summary>
    public class NewtonsoftJsonSerializer : IFeelingsSerializer
    {
        public string SerializerName => "Newtonsoft.Json";
        
        public bool IsAvailable => true; // Available when NEWTONSOFT_JSON is defined
        
        /// <summary>
        /// Advanced data structure that takes advantage of Newtonsoft.Json features.
        /// </summary>
        [System.Serializable]
        public class AdvancedFeelingsData
        {
            public string version = "1.0";
            public DateTime lastSaved;
            public Dictionary<string, float> feelings;
            public Dictionary<string, List<EffectInfo>> effects;
            public Dictionary<string, object> metadata; // Extensible metadata
            
            [System.Serializable]
            public class EffectInfo
            {
                public string targetFeeling;
                public float ratio;
                
                [JsonIgnore] // Example of advanced Newtonsoft.Json features
                public bool IsPositive => ratio > 0;
            }
        }
        
        private readonly JsonSerializerSettings _settings;
        
        public NewtonsoftJsonSerializer()
        {
            _settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
        }
        
        public bool Save(FeelingsData data, string filePath)
        {
            try
            {
                // Convert to advanced format
                var advancedData = ConvertToAdvanced(data);
                
                string json = JsonConvert.SerializeObject(advancedData, _settings);
                
                // Ensure directory exists
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                File.WriteAllText(filePath, json);
                Debug.Log($"[NewtonsoftJsonSerializer] Successfully saved feelings data to: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[NewtonsoftJsonSerializer] Failed to save feelings data: {ex.Message}");
                return false;
            }
        }
        
        public FeelingsData Load(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"[NewtonsoftJsonSerializer] File not found: {filePath}");
                    return null;
                }
                
                string json = File.ReadAllText(filePath);
                
                // Try to load as advanced format first, fall back to basic format
                try
                {
                    var advancedData = JsonConvert.DeserializeObject<AdvancedFeelingsData>(json, _settings);
                    var basicData = ConvertFromAdvanced(advancedData);
                    Debug.Log($"[NewtonsoftJsonSerializer] Successfully loaded advanced feelings data from: {filePath}");
                    return basicData;
                }
                catch
                {
                    // Fall back to basic format
                    var basicData = JsonConvert.DeserializeObject<FeelingsData>(json, _settings);
                    Debug.Log($"[NewtonsoftJsonSerializer] Successfully loaded basic feelings data from: {filePath}");
                    return basicData;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[NewtonsoftJsonSerializer] Failed to load feelings data: {ex.Message}");
                return null;
            }
        }
        
        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }
        
        public bool Delete(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Debug.Log($"[NewtonsoftJsonSerializer] Successfully deleted: {filePath}");
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[NewtonsoftJsonSerializer] Failed to delete file: {ex.Message}");
                return false;
            }
        }
        
        private AdvancedFeelingsData ConvertToAdvanced(FeelingsData basicData)
        {
            var advanced = new AdvancedFeelingsData
            {
                version = basicData.version,
                lastSaved = DateTime.FromBinary(basicData.timestamp),
                feelings = new Dictionary<string, float>(),
                effects = new Dictionary<string, List<AdvancedFeelingsData.EffectInfo>>(),
                metadata = new Dictionary<string, object>
                {
                    ["serializer"] = SerializerName,
                    ["timestamp"] = basicData.timestamp
                }
            };
            
            // Convert feelings
            if (basicData.feelings != null)
            {
                foreach (var feeling in basicData.feelings)
                {
                    advanced.feelings[feeling.name] = feeling.value;
                }
            }
            
            // Convert effects
            if (basicData.effects != null)
            {
                foreach (var effectGroup in basicData.effects)
                {
                    if (effectGroup.effectsData != null)
                    {
                        advanced.effects[effectGroup.sourceFeelingName] = effectGroup.effectsData
                            .Select(e => new AdvancedFeelingsData.EffectInfo 
                            { 
                                targetFeeling = e.targetFeeling, 
                                ratio = e.ratio 
                            }).ToList();
                    }
                }
            }
            
            return advanced;
        }
        
        private FeelingsData ConvertFromAdvanced(AdvancedFeelingsData advancedData)
        {
            var basic = new FeelingsData
            {
                version = advancedData.version,
                timestamp = advancedData.lastSaved.ToBinary()
            };
            
            // Convert feelings
            if (advancedData.feelings != null)
            {
                basic.feelings = advancedData.feelings.Select(kvp => new FeelingsData.FeelingsEntry
                {
                    name = kvp.Key,
                    value = kvp.Value
                }).ToArray();
            }
            
            // Convert effects
            if (advancedData.effects != null)
            {
                basic.effects = advancedData.effects.Select(kvp => new FeelingsData.EffectsEntry
                {
                    sourceFeelingName = kvp.Key,
                    effectsData = kvp.Value?.Select(e => new FeelingsData.EffectsEntry.EffectData
                    {
                        targetFeeling = e.targetFeeling,
                        ratio = e.ratio
                    }).ToArray() ?? new FeelingsData.EffectsEntry.EffectData[0]
                }).ToArray();
            }
            
            return basic;
        }
    }
}
