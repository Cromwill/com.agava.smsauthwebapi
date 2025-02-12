using System;
using UnityEngine.Scripting;

namespace SmsAuthAPI.DTO
{
    /// <summary>
    ///     Client start data.
    /// </summary>
    [Serializable, Preserve]
    public class StartUserData
    {
        public string device_name;

        public DateTime start_date;

        public string phone;

        public string app_app_id;
    }
}
