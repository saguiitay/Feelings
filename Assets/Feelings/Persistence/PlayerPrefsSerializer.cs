using System;
using System.Linq;
using UnityEngine;

namespace Feelings.Persistence
{
    /// <summary>
    /// PlayerPrefs serializer for simple cross-platform persistence.
    /// Note: PlayerPrefs has platform-specific limitations on data size.
    /// </summary>
    public class PlayerPrefsSerializer : IFeelingsSerializer
    {
        public string SerializerName => "Unity PlayerPrefs";
        
        public bool IsAvailable => true; // Always available in Unity
        
        private const int MAX_KEY_LENGTH = 100; // Platform-safe key length
        
        public bool Save(FeelingsData data, string keyPrefix)
        {
            try
            {
                // Clean the key prefix to be safe
                keyPrefix = SanitizeKey(keyPrefix);
                
                // Save metadata
                PlayerPrefs.SetString($"{keyPrefix}_version", data.version);
                PlayerPrefs.SetString($"{keyPrefix}_timestamp", data.timestamp.ToString());
                
                // Save feelings count and data
                if (data.feelings != null && data.feelings.Length > 0)
                {
                    PlayerPrefs.SetInt($"{keyPrefix}_feelings_count", data.feelings.Length);
                    
                    for (int i = 0; i < data.feelings.Length; i++)
                    {
                        var feeling = data.feelings[i];
                        PlayerPrefs.SetString($"{keyPrefix}_feeling_{i}_name", feeling.name);
                        PlayerPrefs.SetFloat($"{keyPrefix}_feeling_{i}_value", feeling.value);
                    }
                }
                else
                {
                    PlayerPrefs.SetInt($"{keyPrefix}_feelings_count", 0);
                }
                
                // Save effects count and data
                if (data.effects != null && data.effects.Length > 0)
                {
                    PlayerPrefs.SetInt($"{keyPrefix}_effects_count", data.effects.Length);
                    
                    for (int i = 0; i < data.effects.Length; i++)
                    {
                        var effectEntry = data.effects[i];
                        PlayerPrefs.SetString($"{keyPrefix}_effect_{i}_source", effectEntry.sourceFeelingName);
                        
                        if (effectEntry.effectsData != null && effectEntry.effectsData.Length > 0)
                        {
                            PlayerPrefs.SetInt($"{keyPrefix}_effect_{i}_count", effectEntry.effectsData.Length);
                            
                            for (int j = 0; j < effectEntry.effectsData.Length; j++)
                            {
                                var effect = effectEntry.effectsData[j];
                                PlayerPrefs.SetString($"{keyPrefix}_effect_{i}_{j}_target", effect.targetFeeling);
                                PlayerPrefs.SetFloat($"{keyPrefix}_effect_{i}_{j}_ratio", effect.ratio);
                            }
                        }
                        else
                        {
                            PlayerPrefs.SetInt($"{keyPrefix}_effect_{i}_count", 0);
                        }
                    }
                }
                else
                {
                    PlayerPrefs.SetInt($"{keyPrefix}_effects_count", 0);
                }
                
                PlayerPrefs.Save();
                Debug.Log($"[PlayerPrefsSerializer] Successfully saved feelings data with key prefix: {keyPrefix}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PlayerPrefsSerializer] Failed to save feelings data: {ex.Message}");
                return false;
            }
        }
        
        public FeelingsData Load(string keyPrefix)
        {
            try
            {
                keyPrefix = SanitizeKey(keyPrefix);
                
                if (!PlayerPrefs.HasKey($"{keyPrefix}_version"))
                {
                    Debug.LogWarning($"[PlayerPrefsSerializer] Data not found for key prefix: {keyPrefix}");
                    return null;
                }
                
                var data = new FeelingsData
                {
                    version = PlayerPrefs.GetString($"{keyPrefix}_version"),
                    timestamp = long.Parse(PlayerPrefs.GetString($"{keyPrefix}_timestamp", "0"))
                };
                
                // Load feelings
                int feelingsCount = PlayerPrefs.GetInt($"{keyPrefix}_feelings_count", 0);
                if (feelingsCount > 0)
                {
                    data.feelings = new FeelingsData.FeelingsEntry[feelingsCount];
                    
                    for (int i = 0; i < feelingsCount; i++)
                    {
                        data.feelings[i] = new FeelingsData.FeelingsEntry
                        {
                            name = PlayerPrefs.GetString($"{keyPrefix}_feeling_{i}_name"),
                            value = PlayerPrefs.GetFloat($"{keyPrefix}_feeling_{i}_value")
                        };
                    }
                }
                
                // Load effects
                int effectsCount = PlayerPrefs.GetInt($"{keyPrefix}_effects_count", 0);
                if (effectsCount > 0)
                {
                    data.effects = new FeelingsData.EffectsEntry[effectsCount];
                    
                    for (int i = 0; i < effectsCount; i++)
                    {
                        var effectEntry = new FeelingsData.EffectsEntry
                        {
                            sourceFeelingName = PlayerPrefs.GetString($"{keyPrefix}_effect_{i}_source")
                        };
                        
                        int effectDataCount = PlayerPrefs.GetInt($"{keyPrefix}_effect_{i}_count", 0);
                        if (effectDataCount > 0)
                        {
                            effectEntry.effectsData = new FeelingsData.EffectsEntry.EffectData[effectDataCount];
                            
                            for (int j = 0; j < effectDataCount; j++)
                            {
                                effectEntry.effectsData[j] = new FeelingsData.EffectsEntry.EffectData
                                {
                                    targetFeeling = PlayerPrefs.GetString($"{keyPrefix}_effect_{i}_{j}_target"),
                                    ratio = PlayerPrefs.GetFloat($"{keyPrefix}_effect_{i}_{j}_ratio")
                                };
                            }
                        }
                        
                        data.effects[i] = effectEntry;
                    }
                }
                
                Debug.Log($"[PlayerPrefsSerializer] Successfully loaded feelings data with key prefix: {keyPrefix}");
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PlayerPrefsSerializer] Failed to load feelings data: {ex.Message}");
                return null;
            }
        }
        
        public bool Exists(string keyPrefix)
        {
            keyPrefix = SanitizeKey(keyPrefix);
            return PlayerPrefs.HasKey($"{keyPrefix}_version");
        }
        
        public bool Delete(string keyPrefix)
        {
            try
            {
                keyPrefix = SanitizeKey(keyPrefix);
                
                if (!Exists(keyPrefix))
                {
                    return true; // Nothing to delete
                }
                
                // Delete metadata
                PlayerPrefs.DeleteKey($"{keyPrefix}_version");
                PlayerPrefs.DeleteKey($"{keyPrefix}_timestamp");
                
                // Delete feelings
                int feelingsCount = PlayerPrefs.GetInt($"{keyPrefix}_feelings_count", 0);
                PlayerPrefs.DeleteKey($"{keyPrefix}_feelings_count");
                
                for (int i = 0; i < feelingsCount; i++)
                {
                    PlayerPrefs.DeleteKey($"{keyPrefix}_feeling_{i}_name");
                    PlayerPrefs.DeleteKey($"{keyPrefix}_feeling_{i}_value");
                }
                
                // Delete effects
                int effectsCount = PlayerPrefs.GetInt($"{keyPrefix}_effects_count", 0);
                PlayerPrefs.DeleteKey($"{keyPrefix}_effects_count");
                
                for (int i = 0; i < effectsCount; i++)
                {
                    PlayerPrefs.DeleteKey($"{keyPrefix}_effect_{i}_source");
                    int effectDataCount = PlayerPrefs.GetInt($"{keyPrefix}_effect_{i}_count", 0);
                    PlayerPrefs.DeleteKey($"{keyPrefix}_effect_{i}_count");
                    
                    for (int j = 0; j < effectDataCount; j++)
                    {
                        PlayerPrefs.DeleteKey($"{keyPrefix}_effect_{i}_{j}_target");
                        PlayerPrefs.DeleteKey($"{keyPrefix}_effect_{i}_{j}_ratio");
                    }
                }
                
                PlayerPrefs.Save();
                Debug.Log($"[PlayerPrefsSerializer] Successfully deleted data for key prefix: {keyPrefix}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PlayerPrefsSerializer] Failed to delete data: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Sanitizes a key to be safe for PlayerPrefs usage.
        /// </summary>
        private string SanitizeKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = "feelings";
            }
            
            // Remove or replace unsafe characters
            key = key.Replace(" ", "_").Replace("/", "_").Replace("\\", "_");
            
            // Limit length for platform safety
            if (key.Length > MAX_KEY_LENGTH)
            {
                key = key.Substring(0, MAX_KEY_LENGTH);
            }
            
            return key;
        }
        
        /// <summary>
        /// Gets estimated size usage for debugging purposes.
        /// Note: This is approximate as PlayerPrefs doesn't provide exact size info.
        /// </summary>
        /// <param name="keyPrefix">Key prefix to analyze.</param>
        /// <returns>Estimated data size information.</returns>
        public string GetSizeEstimate(string keyPrefix)
        {
            if (!Exists(keyPrefix))
            {
                return "No data found";
            }
            
            int feelingsCount = PlayerPrefs.GetInt($"{keyPrefix}_feelings_count", 0);
            int effectsCount = PlayerPrefs.GetInt($"{keyPrefix}_effects_count", 0);
            
            return $"Feelings: {feelingsCount}, Effects: {effectsCount} groups";
        }
    }
}