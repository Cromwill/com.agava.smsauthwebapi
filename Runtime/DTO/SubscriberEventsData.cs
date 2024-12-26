using System;
using UnityEngine.Scripting;

namespace SmsAuthAPI.DTO
{
    [Serializable, Preserve]
    public class SubscriberEventsData
    {
        public string san { get; set; }
        public string phone { get; set; }
        public string date { get; set; }
        public string app_id { get; set; }
        public string version { get; set; }
        public string platform { get; set; }
    }
}
