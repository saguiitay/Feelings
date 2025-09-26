using Feelings.Persistence;
using UnityEngine;

namespace Assets.Feelings.Scripts
{
    /// <summary>
    /// Singleton MonoBehaviour service that handles persistence for feelings systems.
    /// This service depends on feelings systems rather than the feelings systems depending on it,
    /// following the dependency inversion principle.
    /// Persists across scene changes and provides global access.
    /// </summary>
    public class FeelingsService : MonoBehaviour
    {
        private static FeelingsService _instance;
        private FeelingsPeristenceManager _persistenceManager;
        
        /// <summary>
        /// Gets the singleton instance of the FeelingsService.
        /// Creates one if it doesn't exist.
        /// </summary>
        public static FeelingsService Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Try to find existing instance
                    _instance = FindFirstObjectByType<FeelingsService>();
                    
                    if (_instance == null)
                    {
                        // Create new GameObject with FeelingsService
                        GameObject serviceObject = new GameObject("FeelingsService");
                        _instance = serviceObject.AddComponent<FeelingsService>();
                        
                        // Make it persist across scenes
                        DontDestroyOnLoad(serviceObject);
                        
                        Debug.Log("[FeelingsService] Created new singleton instance");
                    }
                }
                return _instance;
            }
        }
        
        /// <summary>
        /// Gets the persistence manager used by this service.
        /// </summary>
        public FeelingsPeristenceManager PersistenceManager => _persistenceManager;
        
        /// <summary>
        /// Unity Awake method - initializes the singleton.
        /// </summary>
        void Awake()
        {
            // Enforce singleton pattern
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePersistenceManager();
            }
            else if (_instance != this)
            {
                Debug.LogWarning("[FeelingsService] Multiple instances detected. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }
        }
        
        /// <summary>
        /// Initializes the persistence manager.
        /// </summary>
        private void InitializePersistenceManager()
        {
            _persistenceManager = new FeelingsPeristenceManager();
            Debug.Log("[FeelingsService] Persistence manager initialized");
        }
        
        /// <summary>
        /// Unity OnDestroy method - cleans up the singleton reference.
        /// </summary>
        void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
        
        /// <summary>
        /// Saves feelings data using the default serializer.
        /// </summary>
        /// <param name="feelingsProvider">The feelings system to save.</param>
        /// <param name="location">Where to save the data.</param>
        /// <returns>True if save was successful.</returns>
        public bool Save(FeelingsMap feelingsProvider, string location)
        {
            if (feelingsProvider == null)
            {
                Debug.LogError("[FeelingsService] Cannot save null feelings provider");
                return false;
            }
            
            var snapshot = feelingsProvider.CreateSnapshot();
            return _persistenceManager.Save(snapshot, location);
        }
        
        /// <summary>
        /// Saves feelings data using a specific serializer.
        /// </summary>
        /// <param name="feelingsProvider">The feelings system to save.</param>
        /// <param name="location">Where to save the data.</param>
        /// <param name="serializerName">Name of the serializer to use.</param>
        /// <returns>True if save was successful.</returns>
        public bool Save(IFeelingsSnapshotProvider feelingsProvider, string location, string serializerName)
        {
            if (feelingsProvider == null)
            {
                Debug.LogError("[FeelingsService] Cannot save null feelings provider");
                return false;
            }
            
            var snapshot = feelingsProvider.CreateSnapshot();
            return _persistenceManager.Save(snapshot, location, serializerName);
        }
        
        /// <summary>
        /// Loads feelings data using the default serializer.
        /// </summary>
        /// <param name="feelingsProvider">The feelings system to load into.</param>
        /// <param name="location">Where to load the data from.</param>
        /// <returns>True if load was successful.</returns>
        public bool Load(IFeelingsSnapshotProvider feelingsProvider, string location)
        {
            if (feelingsProvider == null)
            {
                Debug.LogError("[FeelingsService] Cannot load into null feelings provider");
                return false;
            }
            
            var data = _persistenceManager.Load(location);
            if (data != null)
            {
                feelingsProvider.RestoreFromSnapshot(data);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Loads feelings data using a specific serializer.
        /// </summary>
        /// <param name="feelingsProvider">The feelings system to load into.</param>
        /// <param name="location">Where to load the data from.</param>
        /// <param name="serializerName">Name of the serializer to use.</param>
        /// <returns>True if load was successful.</returns>
        public bool Load(IFeelingsSnapshotProvider feelingsProvider, string location, string serializerName)
        {
            if (feelingsProvider == null)
            {
                Debug.LogError("[FeelingsService] Cannot load into null feelings provider");
                return false;
            }
            
            var data = _persistenceManager.Load(location, serializerName);
            if (data != null)
            {
                feelingsProvider.RestoreFromSnapshot(data);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Attempts to load data by trying multiple serializers.
        /// </summary>
        /// <param name="feelingsProvider">The feelings system to load into.</param>
        /// <param name="location">Where to load the data from.</param>
        /// <returns>True if load was successful with any serializer.</returns>
        public bool LoadWithFallback(IFeelingsSnapshotProvider feelingsProvider, string location)
        {
            if (feelingsProvider == null)
            {
                Debug.LogError("[FeelingsService] Cannot load into null feelings provider");
                return false;
            }
            
            var data = _persistenceManager.LoadWithFallback(location);
            if (data != null)
            {
                feelingsProvider.RestoreFromSnapshot(data);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Checks if data exists at the specified location using the default serializer.
        /// </summary>
        /// <param name="location">Location to check.</param>
        /// <returns>True if data exists.</returns>
        public bool Exists(string location)
        {
            var defaultSerializer = _persistenceManager.DefaultSerializer;
            return defaultSerializer?.Exists(location) ?? false;
        }
        
        /// <summary>
        /// Checks if data exists at the specified location using a specific serializer.
        /// </summary>
        /// <param name="location">Location to check.</param>
        /// <param name="serializerName">Name of the serializer to use.</param>
        /// <returns>True if data exists.</returns>
        public bool Exists(string location, string serializerName)
        {
            var serializer = _persistenceManager.GetSerializer(serializerName);
            return serializer?.Exists(location) ?? false;
        }
        
        /// <summary>
        /// Deletes data at the specified location using the default serializer.
        /// </summary>
        /// <param name="location">Location to delete.</param>
        /// <returns>True if deletion was successful.</returns>
        public bool Delete(string location)
        {
            var defaultSerializer = _persistenceManager.DefaultSerializer;
            return defaultSerializer?.Delete(location) ?? false;
        }
        
        /// <summary>
        /// Deletes data at the specified location using a specific serializer.
        /// </summary>
        /// <param name="location">Location to delete.</param>
        /// <param name="serializerName">Name of the serializer to use.</param>
        /// <returns>True if deletion was successful.</returns>
        public bool Delete(string location, string serializerName)
        {
            var serializer = _persistenceManager.GetSerializer(serializerName);
            return serializer?.Delete(location) ?? false;
        }
        
        /// <summary>
        /// Sets a specific serializer as the default for this service.
        /// </summary>
        /// <param name="serializerName">Name of the serializer to use as default.</param>
        /// <returns>True if the serializer was found and set as default.</returns>
        public bool SetDefaultSerializer(string serializerName)
        {
            return _persistenceManager.SetDefaultSerializer(serializerName);
        }
        
        /// <summary>
        /// Gets information about all available serializers.
        /// </summary>
        /// <returns>Dictionary with serializer names and availability status.</returns>
        public System.Collections.Generic.Dictionary<string, bool> GetSerializerStatus()
        {
            return _persistenceManager.GetSerializerStatus();
        }
        
        /// <summary>
        /// Logs detailed information about all registered serializers.
        /// </summary>
        public void LogSerializerInfo()
        {
            _persistenceManager.LogSerializerInfo();
        }
        
        /// <summary>
        /// Static convenience method for saving feelings data.
        /// </summary>
        /// <param name="feelingsProvider">The feelings system to save.</param>
        /// <param name="location">Where to save the data.</param>
        /// <returns>True if save was successful.</returns>
        public static bool SaveStatic(FeelingsMap feelingsProvider, string location)
        {
            return Instance.Save(feelingsProvider, location);
        }
        
        /// <summary>
        /// Static convenience method for loading feelings data.
        /// </summary>
        /// <param name="feelingsProvider">The feelings system to load into.</param>
        /// <param name="location">Where to load the data from.</param>
        /// <returns>True if load was successful.</returns>
        public static bool LoadStatic(IFeelingsSnapshotProvider feelingsProvider, string location)
        {
            return Instance.Load(feelingsProvider, location);
        }
        
        /// <summary>
        /// Static convenience method for loading feelings data with fallback.
        /// </summary>
        /// <param name="feelingsProvider">The feelings system to load into.</param>
        /// <param name="location">Where to load the data from.</param>
        /// <returns>True if load was successful with any serializer.</returns>
        public static bool LoadWithFallbackStatic(IFeelingsSnapshotProvider feelingsProvider, string location)
        {
            return Instance.LoadWithFallback(feelingsProvider, location);
        }
        
        /// <summary>
        /// Editor-only method to reset the singleton instance.
        /// Useful for testing and development.
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void ResetInstanceForTesting()
        {
            if (_instance != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(_instance.gameObject);
                }
                else
                {
                    DestroyImmediate(_instance.gameObject);
                }
                _instance = null;
            }
        }
    }
}