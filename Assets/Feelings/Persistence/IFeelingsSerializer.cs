using System;
using System.Collections.Generic;

namespace Feelings.Persistence
{
    /// <summary>
    /// Common interface for all feelings persistence implementations.
    /// </summary>
    public interface IFeelingsSerializer
    {
        /// <summary>
        /// Gets the name/identifier of this serialization method.
        /// </summary>
        string SerializerName { get; }
        
        /// <summary>
        /// Indicates whether this serializer is available/supported in the current environment.
        /// </summary>
        bool IsAvailable { get; }
        
        /// <summary>
        /// Saves feelings data using this serialization method.
        /// </summary>
        /// <param name="data">The feelings data to save.</param>
        /// <param name="location">Location/path/key where to save the data.</param>
        /// <returns>True if save was successful.</returns>
        bool Save(FeelingsData data, string location);
        
        /// <summary>
        /// Loads feelings data using this serialization method.
        /// </summary>
        /// <param name="location">Location/path/key where to load the data from.</param>
        /// <returns>Loaded feelings data, or null if loading failed.</returns>
        FeelingsData Load(string location);
        
        /// <summary>
        /// Checks if data exists at the specified location.
        /// </summary>
        /// <param name="location">Location/path/key to check.</param>
        /// <returns>True if data exists.</returns>
        bool Exists(string location);
        
        /// <summary>
        /// Deletes data at the specified location.
        /// </summary>
        /// <param name="location">Location/path/key to delete.</param>
        /// <returns>True if deletion was successful.</returns>
        bool Delete(string location);
    }
    
    /// <summary>
    /// Common data structure for feelings serialization.
    /// Compatible with all serialization methods.
    /// </summary>
    [System.Serializable]
    public class FeelingsData
    {
        public string version = "1.0";
        public long timestamp;
        public FeelingsEntry[] feelings;
        public EffectsEntry[] effects;
        
        [System.Serializable]
        public class FeelingsEntry
        {
            public string name;
            public float value;
        }
        
        [System.Serializable]
        public class EffectsEntry
        {
            public string sourceFeelingName;
            public EffectData[] effectsData;
            
            [System.Serializable]
            public class EffectData
            {
                public string targetFeeling;
                public float ratio;
            }
        }
        
        public FeelingsData()
        {
            timestamp = DateTime.UtcNow.ToBinary();
        }
    }
}