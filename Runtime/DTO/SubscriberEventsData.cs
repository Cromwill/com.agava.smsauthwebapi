using System;
using UnityEngine.Scripting;

namespace SmsAuthAPI.DTO
{
    [Serializable, Preserve]
    public class SubscriberEventsData
    {
        public string san;
        public string phone;
        public string date;
        public string app_id;
        public string version;
        public string platform;
    }
}
