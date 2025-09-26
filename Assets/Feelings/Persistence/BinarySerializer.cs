using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Feelings.Persistence
{
    /// <summary>
    /// Binary serializer for compact feelings data storage.
    /// Provides the smallest file size but platform-dependent format.
    /// </summary>
    public class BinarySerializer : IFeelingsSerializer
    {
        public string SerializerName => "Binary Formatter";
        
        public bool IsAvailable => true; // Available in most .NET environments
        
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
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, data);
                }
                
                Debug.Log($"[BinarySerializer] Successfully saved feelings data to: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BinarySerializer] Failed to save feelings data: {ex.Message}");
                return false;
            }
        }
        
        public FeelingsData Load(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"[BinarySerializer] File not found: {filePath}");
                    return null;
                }
                
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    var formatter = new BinaryFormatter();
                    var data = (FeelingsData)formatter.Deserialize(stream);
                    Debug.Log($"[BinarySerializer] Successfully loaded feelings data from: {filePath}");
                    return data;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BinarySerializer] Failed to load feelings data: {ex.Message}");
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
                    Debug.Log($"[BinarySerializer] Successfully deleted: {filePath}");
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BinarySerializer] Failed to delete file: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Gets file size information for binary files.
        /// </summary>
        /// <param name="filePath">Path to the binary file.</param>
        /// <returns>File size in bytes, or -1 if file doesn't exist.</returns>
        public long GetFileSize(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var fileInfo = new FileInfo(filePath);
                    return fileInfo.Length;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BinarySerializer] Failed to get file size: {ex.Message}");
            }
            return -1;
        }
    }
}