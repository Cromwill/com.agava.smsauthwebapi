using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SmsAuthAPI.Program;

namespace SmsAuthAPI.Utility
{
    public static class SheetRemoteConfigs
    {
        private const char CellSeporator = '|';

        public static RemoteDataItems Texts { get; private set; }

        public static IEnumerator Initialize()
        {
            bool isComplete = false;
            var wait = new WaitUntil(() => isComplete);

            Debug.Log($"[SheetRemoteConfigs] Start load file");
            LoadText(() => isComplete = true);

            yield return wait;
        }

        private static async void LoadText(Action onComplete)
        {
            var responce = await SmsAuthApi.GetRemoteServerConfig();

            if (responce.statusCode == UnityWebRequest.Result.Success)
            {
                string cvsRawData = responce.body;
                Texts = GetData(cvsRawData);
                Debug.Log($"[SheetRemoteConfigs] Complete to load file");
                onComplete?.Invoke();
            }
            else
            {
                Debug.LogError($"[SheetRemoteConfigs] Fail to load file");
                onComplete?.Invoke();
            }
        }

        private static RemoteDataItems GetData(string cvsRawData)
        {
            char lineEnding = GetPlatformSpecificLineEnd();
            string[] rows = cvsRawData.Split(lineEnding);
            int dataStartRawIndex = 1;
            int dataStartCellIndex = 1;

            RemoteDataItems data = new();

            for (int r = dataStartRawIndex; r < rows.Length; r++) //columns[0] - Raw Key on table, keyValuePairs -> <Column(key), columns[n](Value)>
            {
                string[] columnsKey = rows[0].Split(CellSeporator);
                string[] columns = rows[r].Split(CellSeporator);

                Dictionary<string, string> keyValuePairs = new();

                for (int c = dataStartCellIndex; c < columns.Length; c++)
                {
                    string key = columnsKey[c].TrimEnd();
                    string value = columns[c].TrimEnd();
                    keyValuePairs.Add(key, value);
                }

                data.Add(columns[0], keyValuePairs);
            }

            return data;
        }

        private static char GetPlatformSpecificLineEnd()
        {
            char lineEnding = '\n';
#if UNITY_IOS
            lineEnding = '\r';
#endif
            return lineEnding;
        }
    }
}
