#if EASYSAVE3
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Feelings.Persistence
{
    /// <summary>
    /// Easy Save 3 serializer with advanced features and encryption support.
    /// Requires Easy Save 3 asset from Unity Asset Store.
    /// </summary>
    public class EasySave3Serializer : IFeelingsSerializer
    {
        public string SerializerName => "Easy Save 3";
        
        public bool IsAvailable => true; // Available when EASYSAVE3 is defined
        
        private readonly ES3Settings _settings;
        
        public EasySave3Serializer(ES3Settings customSettings = null)
        {
            _settings = customSettings ?? new ES3Settings
            {
                location = ES3.Location.File,
                directory = ES3.Directory.PersistentDataPath,
                encryptionType = ES3.EncryptionType.None,
                compressionType = ES3.CompressionType.None
            };
        }
        
        public bool Save(FeelingsData data, string key)
        {
            try
            {
                ES3.Save($"{key}_version", data.version, _settings);
                ES3.Save($"{key}_timestamp", data.timestamp, _settings);
                
                if (data.feelings != null)
                {
                    var feelingsDict = data.feelings.ToDictionary(f => f.name, f => f.value);
                    ES3.Save($"{key}_feelings", feelingsDict, _settings);
                }
                
                if (data.effects != null)
                {
                    var effectsDict = data.effects.ToDictionary(
                        e => e.sourceFeelingName,
                        e => e.effectsData?.Select(ed => new { target = ed.targetFeeling, ratio = ed.ratio }).ToArray() ?? new object[0]
                    );
                    ES3.Save($"{key}_effects", effectsDict, _settings);
                }
                
                Debug.Log($"[EasySave3Serializer] Successfully saved feelings data with key: {key}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[EasySave3Serializer] Failed to save feelings data: {ex.Message}");
                return false;
            }
        }
        
        public FeelingsData Load(string key)
        {
            try
            {
                if (!ES3.KeyExists($"{key}_version", _settings))
                {
                    return null;
                }
                
                var data = new FeelingsData
                {
                    version = ES3.Load<string>($"{key}_version", _settings),
                    timestamp = ES3.Load<long>($"{key}_timestamp", _settings)
                };
                
                if (ES3.KeyExists($"{key}_feelings", _settings))
                {
                    var feelingsDict = ES3.Load<Dictionary<string, float>>($"{key}_feelings", _settings);
                    data.feelings = feelingsDict.Select(kvp => new FeelingsData.FeelingsEntry
                    {
                        name = kvp.Key,
                        value = kvp.Value
                    }).ToArray();
                }
                
                Debug.Log($"[EasySave3Serializer] Successfully loaded feelings data with key: {key}");
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[EasySave3Serializer] Failed to load feelings data: {ex.Message}");
                return null;
            }
        }
        
        public bool Exists(string key)
        {
            return ES3.KeyExists($"{key}_version", _settings);
        }
        
        public bool Delete(string key)
        {
            try
            {
                string[] keys = { $"{key}_version", $"{key}_timestamp", $"{key}_feelings", $"{key}_effects" };
                
                foreach (var k in keys)
                {
                    if (ES3.KeyExists(k, _settings))
                    {
                        ES3.DeleteKey(k, _settings);
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[EasySave3Serializer] Failed to delete data: {ex.Message}");
                return false;
            }
        }
    }
}
#else
namespace Feelings.Persistence
{
    public class EasySave3Serializer : IFeelingsSerializer
    {
        public string SerializerName => "Easy Save 3 (Not Available)";
        public bool IsAvailable => false;
        
        public bool Save(FeelingsData data, string location)
        {
            UnityEngine.Debug.LogError("[EasySave3Serializer] Easy Save 3 is not available.");
            return false;
        }
        
        public FeelingsData Load(string location) => null;
        public bool Exists(string location) => false;
        public bool Delete(string location) => false;
    }
}
#endif