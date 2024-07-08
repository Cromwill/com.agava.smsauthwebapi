using System;
using UnityEngine.Scripting;

namespace SmsAuthAPI.DTO
{
    /// <summary>
    ///     Client data storage.
    /// </summary>
    [Serializable, Preserve]
    public class LoginData
    {
        public string phone { get; set; }
        public string otp_code { get; set; }
        public string device_id { get; set; }
        public string app_id { get; set; }
    }
}
