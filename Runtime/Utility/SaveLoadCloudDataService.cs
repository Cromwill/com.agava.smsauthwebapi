using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SmsAuthAPI.Program;
using SmsAuthAPI.DTO;
using UnityEngine.Networking;

namespace SmsAuthAPI.Utility
{
    public static class SaveLoadCloudDataService
    {
        /// <summary>
        ///     Save JSON to cloud. Json recieve only (string).
        /// </summary>
        public static async void SaveData(string json)
        {
            Tokens tokens = TokenLifeHelper.GetTokens();

            if (await TokenLifeHelper.IsTokensAlive(tokens) == false)
            {
                Debug.LogError("Tokens has expired");
                return;
            }

            var response = await SmsAuthApi.SetSave(tokens.access, json);

            if (response.statusCode != UnityWebRequest.Result.Success)
                Debug.LogError("CloudSave -> fail to save: " + response.statusCode + " Message: " + response.body);
        }

        /// <summary>
        ///     Load JSON from cloud. Json return only (string).
        /// </summary>
        public static async Task<string> LoadData()
        {
            Tokens tokens = TokenLifeHelper.GetTokens();

            if (await TokenLifeHelper.IsTokensAlive(tokens) == false)
            {
                Debug.LogError("Tokens has expired");
                return null;
            }

            var response = await SmsAuthApi.GetSave(tokens.access);

            if (response.statusCode != UnityWebRequest.Result.Success)
            {
                Debug.LogError("CloudSave -> fail to load: " + response.statusCode + " Message: " + response.reasonPhrase);
                return null;
            }
            else
            {
                string json;
                if (response.isBase64Encoded)
                {
                    byte[] bytes = Convert.FromBase64String(response.body);
                    json = Encoding.UTF8.GetString(bytes);
                }
                else
                {
                    json = response.body;
                }
#if UNITY_EDITOR || TEST
                Debug.Log("Cloud Saves Loaded {API}");
#endif
                return json;
            }
        }
    }
}
