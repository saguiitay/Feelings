using System;
using System.IO;
using UnityEngine;

namespace Feelings.Persistence
{
    /// <summary>
    /// Unity JsonUtility serializer for feelings data.
    /// This is the recommended method for most Unity projects.
    /// </summary>
    public class JsonUtilitySerializer : IFeelingsSerializer
    {
        public string SerializerName => "Unity JsonUtility";
        
        public bool IsAvailable => true; // Always available in Unity
        
        public bool Save(FeelingsData data, string filePath)
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);
                
                // Ensure directory exists
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                File.WriteAllText(filePath, json);
                Debug.Log($"[JsonUtilitySerializer] Successfully saved feelings data to: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonUtilitySerializer] Failed to save feelings data: {ex.Message}");
                return false;
            }
        }
        
        public FeelingsData Load(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"[JsonUtilitySerializer] File not found: {filePath}");
                    return null;
                }
                
                string json = File.ReadAllText(filePath);
                var data = JsonUtility.FromJson<FeelingsData>(json);
                Debug.Log($"[JsonUtilitySerializer] Successfully loaded feelings data from: {filePath}");
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonUtilitySerializer] Failed to load feelings data: {ex.Message}");
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
                    Debug.Log($"[JsonUtilitySerializer] Successfully deleted: {filePath}");
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonUtilitySerializer] Failed to delete file: {ex.Message}");
                return false;
            }
        }
    }
}