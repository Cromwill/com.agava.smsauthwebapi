using Newtonsoft.Json;
using UnityEngine;

namespace SmsAuthAPI.Utility
{
    /// <summary>
    ///     Save data to local storage on device.
    /// </summary>
    public static class SaveLoadLocalDataService
    {
        public static void Save<T>(T obj, string saveName) where T : class
        {
            string data = JsonConvert.SerializeObject(obj);
            UnityEngine.PlayerPrefs.SetString(saveName, data);
        }

        public static T Load<T>(string saveName) where T : class
        {
            if (UnityEngine.PlayerPrefs.HasKey(saveName) == false)
            {
                Debug.LogError("Saves not exhist");
                return null;
            }

            var loadData = UnityEngine.PlayerPrefs.GetString(saveName);
            T data = JsonConvert.DeserializeObject<T>(loadData);

            return data;
        }

        public static void Delete(string saveName)
        {
            UnityEngine.PlayerPrefs.DeleteKey(saveName);
        }
    }
}
