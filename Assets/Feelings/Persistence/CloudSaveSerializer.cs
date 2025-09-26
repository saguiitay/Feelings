#if UNITY_CLOUD_BUILD_API || UNITY_SERVICES_CORE
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;

namespace Feelings.Persistence
{
    /// <summary>
    /// Unity Cloud Save serializer for cross-device synchronization.
    /// Requires Unity Services and Cloud Save package.
    /// </summary>
    public class CloudSaveSerializer : IFeelingsSerializer
    {
        public string SerializerName => "Unity Cloud Save";
        
        public bool IsAvailable => true; // Available when cloud save packages are imported
        
        private readonly bool _usePlayerScope;
        
        /// <summary>
        /// Creates a new Cloud Save serializer.
        /// </summary>
        /// <param name="usePlayerScope">If true, uses player-scoped data. If false, uses custom data scope.</param>
        public CloudSaveSerializer(bool usePlayerScope = true)
        {
            _usePlayerScope = usePlayerScope;
        }
        
        public bool Save(FeelingsData data, string key)
        {
            try
            {
                // Convert to async and wait for completion
                var task = SaveAsync(data, key);
                task.Wait();
                return task.Result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CloudSaveSerializer] Failed to save feelings data: {ex.Message}");
                return false;
            }
        }
        
        public FeelingsData Load(string key)
        {
            try
            {
                var task = LoadAsync(key);
                task.Wait();
                return task.Result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CloudSaveSerializer] Failed to load feelings data: {ex.Message}");
                return null;
            }
        }
        
        public bool Exists(string key)
        {
            try
            {
                var task = ExistsAsync(key);
                task.Wait();
                return task.Result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CloudSaveSerializer] Failed to check existence: {ex.Message}");
                return false;
            }
        }
        
        public bool Delete(string key)
        {
            try
            {
                var task = DeleteAsync(key);
                task.Wait();
                return task.Result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CloudSaveSerializer] Failed to delete data: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Async version of Save method.
        /// </summary>
        public async Task<bool> SaveAsync(FeelingsData data, string key)
        {
            try
            {
                // Convert to JSON for cloud storage
                string jsonData = JsonUtility.ToJson(data);
                
                var saveData = new Dictionary<string, object>
                {
                    { key, jsonData },
                    { $"{key}_metadata", new Dictionary<string, object>
                        {
                            { "serializer", SerializerName },
                            { "saved_at", DateTime.UtcNow.ToBinary() },
                            { "version", data.version }
                        }
                    }
                };
                
                if (_usePlayerScope)
                {
                    await CloudSaveService.Instance.Data.Player.SaveAsync(saveData);
                }
                else
                {
                    // For custom scope, you would implement custom data access here
                    await CloudSaveService.Instance.Data.Player.SaveAsync(saveData);
                }
                
                Debug.Log($"[CloudSaveSerializer] Successfully saved feelings data to cloud with key: {key}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CloudSaveSerializer] Failed to save feelings data to cloud: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Async version of Load method.
        /// </summary>
        public async Task<FeelingsData> LoadAsync(string key)
        {
            try
            {
                HashSet<string> keys = new HashSet<string> { key };
                
                Dictionary<string, Unity.Services.CloudSave.Models.Item> savedData;
                
                if (_usePlayerScope)
                {
                    savedData = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);
                }
                else
                {
                    savedData = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);
                }
                
                if (savedData.TryGetValue(key, out var item))
                {
                    string jsonData = item.Value.GetAs<string>();
                    var data = JsonUtility.FromJson<FeelingsData>(jsonData);
                    
                    Debug.Log($"[CloudSaveSerializer] Successfully loaded feelings data from cloud with key: {key}");
                    return data;
                }
                else
                {
                    Debug.LogWarning($"[CloudSaveSerializer] No data found in cloud for key: {key}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CloudSaveSerializer] Failed to load feelings data from cloud: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Async version of Exists method.
        /// </summary>
        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                HashSet<string> keys = new HashSet<string> { key };
                
                Dictionary<string, Unity.Services.CloudSave.Models.Item> savedData;
                
                if (_usePlayerScope)
                {
                    savedData = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);
                }
                else
                {
                    savedData = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);
                }
                
                return savedData.ContainsKey(key);
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Async version of Delete method.
        /// </summary>
        public async Task<bool> DeleteAsync(string key)
        {
            try
            {
                HashSet<string> keys = new HashSet<string> { key, $"{key}_metadata" };
                
                if (_usePlayerScope)
                {
                    await CloudSaveService.Instance.Data.Player.DeleteAsync(keys);
                }
                else
                {
                    await CloudSaveService.Instance.Data.Player.DeleteAsync(keys);
                }
                
                Debug.Log($"[CloudSaveSerializer] Successfully deleted cloud data for key: {key}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CloudSaveSerializer] Failed to delete cloud data: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Gets all available keys from cloud save.
        /// </summary>
        public async Task<List<string>> GetAllKeysAsync()
        {
            try
            {
                Dictionary<string, Unity.Services.CloudSave.Models.Item> allData;
                
                if (_usePlayerScope)
                {
                    allData = await CloudSaveService.Instance.Data.Player.LoadAllAsync();
                }
                else
                {
                    allData = await CloudSaveService.Instance.Data.Player.LoadAllAsync();
                }
                
                var feelingsKeys = new List<string>();
                foreach (var key in allData.Keys)
                {
                    if (!key.EndsWith("_metadata"))
                    {
                        feelingsKeys.Add(key);
                    }
                }
                
                return feelingsKeys;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[CloudSaveSerializer] Failed to get all keys: {ex.Message}");
                return new List<string>();
            }
        }
    }
}
#else
namespace Feelings.Persistence
{
    public class CloudSaveSerializer : IFeelingsSerializer
    {
        public string SerializerName => "Unity Cloud Save (Not Available)";
        public bool IsAvailable => false;
        
        public bool Save(FeelingsData data, string location)
        {
            UnityEngine.Debug.LogError("[CloudSaveSerializer] Unity Cloud Save is not available. Please install Unity Services packages.");
            return false;
        }
        
        public FeelingsData Load(string location) => null;
        public bool Exists(string location) => false;
        public bool Delete(string location) => false;
    }
}
#endif