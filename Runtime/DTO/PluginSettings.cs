namespace SmsAuthAPI.DTO
{
    /// <summary>
    ///     Plugin settings data.
    /// </summary>
    public class PluginSettings
    {
        public string app_name { get; set; }
        public string platform { get; set; }
        public int released_version { get; set; }
        public int review_version { get; set; }
        public string plugin_state { get; set; }
        public string test_review { get; set; }
        public string common { get; set; }
    }
}
