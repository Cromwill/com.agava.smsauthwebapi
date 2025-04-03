using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace SmsAuthAPI.Utility
{
    [Preserve, Serializable]
    public class RemoteDataItems
    {
        private readonly Dictionary<string, Dictionary<string, string>> _keyRemDatas = new();

        public IReadOnlyDictionary<string, Dictionary<string, string>> Data => _keyRemDatas;

        public void Add(string key, Dictionary<string, string> keyValuePair) => _keyRemDatas.Add(key, keyValuePair);
    }
}
