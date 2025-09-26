using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Feelings.Persistence
{
    /// <summary>
    /// Central manager for all feelings persistence operations.
    /// Automatically detects available serializers and provides a unified API.
    /// </summary>
    public class FeelingsPeristenceManager
    {
        private readonly Dictionary<string, IFeelingsSerializer> _serializers;
        private IFeelingsSerializer _defaultSerializer;
        
        /// <summary>
        /// Gets all available serializers.
        /// </summary>
        public IEnumerable<IFeelingsSerializer> AvailableSerializers => _serializers.Values.Where(s => s.IsAvailable);
        
        /// <summary>
        /// Gets the current default serializer.
        /// </summary>
        public IFeelingsSerializer DefaultSerializer => _defaultSerializer;
        
        public FeelingsPeristenceManager()
        {
            _serializers = new Dictionary<string, IFeelingsSerializer>();
            RegisterAllSerializers();
            SetDefaultSerializer();
        }
        
        /// <summary>
        /// Registers all available serializers.
        /// </summary>
        private void RegisterAllSerializers()
        {
            // Register all serializer implementations
            RegisterSerializer(new JsonUtilitySerializer());
            RegisterSerializer(new NewtonsoftJsonSerializer());
            RegisterSerializer(new BinarySerializer());
            RegisterSerializer(new EasySave3Serializer());
            RegisterSerializer(new OdinSerializer());
            RegisterSerializer(new PlayerPrefsSerializer());
            RegisterSerializer(new CloudSaveSerializer());
        }
        
        /// <summary>
        /// Registers a custom serializer.
        /// </summary>
        /// <param name="serializer">The serializer to register.</param>
        public void RegisterSerializer(IFeelingsSerializer serializer)
        {
            if (serializer != null)
            {
                _serializers[serializer.SerializerName] = serializer;
                Debug.Log($"[FeelingsPeristenceManager] Registered serializer: {serializer.SerializerName} (Available: {serializer.IsAvailable})");
            }
        }
        
        /// <summary>
        /// Sets the default serializer based on availability and priority.
        /// </summary>
        private void SetDefaultSerializer()
        {
            // Priority order for automatic selection
            string[] preferredOrder = {
                "Easy Save 3",
                "Newtonsoft.Json", 
                "Unity JsonUtility",
                "Odin Serializer",
                "Binary Formatter",
                "Unity PlayerPrefs",
                "Unity Cloud Save"
            };
            
            foreach (string serializerName in preferredOrder)
            {
                if (_serializers.TryGetValue(serializerName, out var serializer) && serializer.IsAvailable)
                {
                    _defaultSerializer = serializer;
                    Debug.Log($"[FeelingsPeristenceManager] Set default serializer to: {serializerName}");
                    return;
                }
            }
            
            // Fallback to first available serializer
            _defaultSerializer = _serializers.Values.FirstOrDefault(s => s.IsAvailable);
            
            if (_defaultSerializer != null)
            {
                Debug.Log($"[FeelingsPeristenceManager] Fallback to serializer: {_defaultSerializer.SerializerName}");
            }
            else
            {
                Debug.LogError("[FeelingsPeristenceManager] No available serializers found!");
            }
        }
        
        /// <summary>
        /// Sets a specific serializer as the default.
        /// </summary>
        /// <param name="serializerName">Name of the serializer to use as default.</param>
        /// <returns>True if the serializer was found and set as default.</returns>
        public bool SetDefaultSerializer(string serializerName)
        {
            if (_serializers.TryGetValue(serializerName, out var serializer) && serializer.IsAvailable)
            {
                _defaultSerializer = serializer;
                Debug.Log($"[FeelingsPeristenceManager] Default serializer changed to: {serializerName}");
                return true;
            }
            
            Debug.LogError($"[FeelingsPeristenceManager] Serializer '{serializerName}' not found or not available");
            return false;
        }
        
        /// <summary>
        /// Gets a specific serializer by name.
        /// </summary>
        /// <param name="serializerName">Name of the serializer.</param>
        /// <returns>The serializer if found, null otherwise.</returns>
        public IFeelingsSerializer GetSerializer(string serializerName)
        {
            _serializers.TryGetValue(serializerName, out var serializer);
            return serializer;
        }
        
        /// <summary>
        /// Saves feelings data using the default serializer.
        /// </summary>
        /// <param name="data">Feelings data to save.</param>
        /// <param name="location">Location/path/key for the save.</param>
        /// <returns>True if save was successful.</returns>
        public bool Save(FeelingsData data, string location)
        {
            if (_defaultSerializer == null)
            {
                Debug.LogError("[FeelingsPeristenceManager] No default serializer available");
                return false;
            }
            
            return _defaultSerializer.Save(data, location);
        }
        
        /// <summary>
        /// Saves feelings data using a specific serializer.
        /// </summary>
        /// <param name="data">Feelings data to save.</param>
        /// <param name="location">Location/path/key for the save.</param>
        /// <param name="serializerName">Name of the serializer to use.</param>
        /// <returns>True if save was successful.</returns>
        public bool Save(FeelingsData data, string location, string serializerName)
        {
            var serializer = GetSerializer(serializerName);
            if (serializer == null || !serializer.IsAvailable)
            {
                Debug.LogError($"[FeelingsPeristenceManager] Serializer '{serializerName}' not available");
                return false;
            }
            
            return serializer.Save(data, location);
        }
        
        /// <summary>
        /// Loads feelings data using the default serializer.
        /// </summary>
        /// <param name="location">Location/path/key to load from.</param>
        /// <returns>Loaded feelings data, or null if loading failed.</returns>
        public FeelingsData Load(string location)
        {
            if (_defaultSerializer == null)
            {
                Debug.LogError("[FeelingsPeristenceManager] No default serializer available");
                return null;
            }
            
            return _defaultSerializer.Load(location);
        }
        
        /// <summary>
        /// Loads feelings data using a specific serializer.
        /// </summary>
        /// <param name="location">Location/path/key to load from.</param>
        /// <param name="serializerName">Name of the serializer to use.</param>
        /// <returns>Loaded feelings data, or null if loading failed.</returns>
        public FeelingsData Load(string location, string serializerName)
        {
            var serializer = GetSerializer(serializerName);
            if (serializer == null || !serializer.IsAvailable)
            {
                Debug.LogError($"[FeelingsPeristenceManager] Serializer '{serializerName}' not available");
                return null;
            }
            
            return serializer.Load(location);
        }
        
        /// <summary>
        /// Attempts to load data by trying multiple serializers.
        /// Useful for migration between different serialization formats.
        /// </summary>
        /// <param name="location">Location/path/key to load from.</param>
        /// <returns>First successfully loaded data, or null if all attempts failed.</returns>
        public FeelingsData LoadWithFallback(string location)
        {
            // Try default serializer first
            if (_defaultSerializer != null)
            {
                var data = _defaultSerializer.Load(location);
                if (data != null)
                {
                    return data;
                }
            }
            
            // Try all other available serializers
            foreach (var serializer in AvailableSerializers.Where(s => s != _defaultSerializer))
            {
                try
                {
                    var data = serializer.Load(location);
                    if (data != null)
                    {
                        Debug.Log($"[FeelingsPeristenceManager] Successfully loaded data using fallback serializer: {serializer.SerializerName}");
                        return data;
                    }
                }
                catch
                {
                    // Continue to next serializer
                }
            }
            
            Debug.LogWarning($"[FeelingsPeristenceManager] Failed to load data from '{location}' with any available serializer");
            return null;
        }
        
        /// <summary>
        /// Gets information about all available serializers.
        /// </summary>
        /// <returns>Dictionary with serializer names and availability status.</returns>
        public Dictionary<string, bool> GetSerializerStatus()
        {
            return _serializers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.IsAvailable);
        }
        
        /// <summary>
        /// Logs detailed information about all registered serializers.
        /// </summary>
        public void LogSerializerInfo()
        {
            Debug.Log("=== Feelings Persistence Manager Status ===");
            Debug.Log($"Default Serializer: {(_defaultSerializer?.SerializerName ?? "None")}");
            Debug.Log($"Available Serializers: {AvailableSerializers.Count()}");
            
            foreach (var serializer in _serializers.Values)
            {
                string status = serializer.IsAvailable ? "✓ Available" : "✗ Not Available";
                Debug.Log($"  {serializer.SerializerName}: {status}");
            }
            Debug.Log("==========================================");
        }
    }
}