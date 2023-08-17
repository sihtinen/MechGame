using System;
using System.IO;

using UnityEngine;

using Newtonsoft.Json;

namespace Tensori.SaveSystem
{
    public static class JsonFileOperations
    {
        public static bool Write<T>(string filePath, T toSave)
        {
            try
            {
                string _json = JsonConvert.SerializeObject(toSave, Formatting.Indented);
                File.WriteAllText(filePath, _json);
            }
            catch (Exception exc)
            {
                Debug.LogError("Error while saving: Read ERRORS-file.\n" + exc);
                return false;
            }

            return true;
        }

        public static bool ReadJsonFromFile<T>(string filePath, ref T populateTarget)
        {
            if (populateTarget == null)
            {
                Debug.LogError($"JsonFileOperations.ReadJsonFromFile<{nameof(T)}>: populate target is null!");
                return false;
            }

            try
            {
                string _serializedString = File.ReadAllText(filePath);
                JsonConvert.PopulateObject(_serializedString, populateTarget);
            }
            catch (Exception exc)
            {
                Debug.LogError("Error while loading: Read ERRORS-file.\n" + exc);
                return false;
            }

            return true;
        }

        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None);
        }

        public static T FromJson<T>(string json) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
