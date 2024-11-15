using System;

namespace SmsAuthAPI.DTO
{
    /// <summary>
    ///     Client start data.
    /// </summary>
    public class StartUserData
    {
        public string device_name { get; set; }

        public DateTime start_date { get; set; }

        public string phone { get; set; }

        public string app_app_id { get; set; }
    }
}
