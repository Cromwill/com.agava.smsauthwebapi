namespace SmsAuthAPI.DTO
{
    public class Request
    {
        public string apiName { get; set; }
        public string body { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }
}
