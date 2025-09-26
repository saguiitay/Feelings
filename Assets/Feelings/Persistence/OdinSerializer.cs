#if ODIN_INSPECTOR
using System;
using System.IO;
using Sirenix.Serialization;
using UnityEngine;

namespace Feelings.Persistence
{
    /// <summary>
    /// Odin Serializer for advanced Unity object serialization.
    /// Requires Odin Inspector asset from Unity Asset Store.
    /// </summary>
    public class OdinSerializer : IFeelingsSerializer
    {
        public string SerializerName => "Odin Serializer";
        
        public bool IsAvailable => true; // Available when ODIN_INSPECTOR is defined
        
        private readonly DataFormat _dataFormat;
        
        public OdinSerializer(DataFormat format = DataFormat.JSON)
        {
            _dataFormat = format;
        }
        
        public bool Save(FeelingsData data, string filePath)
        {
            try
            {
                // Ensure directory exists
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                byte[] bytes = SerializationUtility.SerializeValue(data, _dataFormat);
                File.WriteAllBytes(filePath, bytes);
                
                Debug.Log($"[OdinSerializer] Successfully saved feelings data to: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[OdinSerializer] Failed to save feelings data: {ex.Message}");
                return false;
            }
        }
        
        public FeelingsData Load(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"[OdinSerializer] File not found: {filePath}");
                    return null;
                }
                
                byte[] bytes = File.ReadAllBytes(filePath);
                var data = SerializationUtility.DeserializeValue<FeelingsData>(bytes, _dataFormat);
                
                Debug.Log($"[OdinSerializer] Successfully loaded feelings data from: {filePath}");
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[OdinSerializer] Failed to load feelings data: {ex.Message}");
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
                    Debug.Log($"[OdinSerializer] Successfully deleted: {filePath}");
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[OdinSerializer] Failed to delete file: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Gets information about the serialization format being used.
        /// </summary>
        public string GetFormatInfo()
        {
            return $"Odin Serializer using {_dataFormat} format";
        }
    }
}
#else
namespace Feelings.Persistence
{
    public class OdinSerializer : IFeelingsSerializer
    {
        public string SerializerName => "Odin Serializer (Not Available)";
        public bool IsAvailable => false;
        
        public bool Save(FeelingsData data, string location)
        {
            UnityEngine.Debug.LogError("[OdinSerializer] Odin Inspector is not available.");
            return false;
        }
        
        public FeelingsData Load(string location) => null;
        public bool Exists(string location) => false;
        public bool Delete(string location) => false;
    }
}
#endif