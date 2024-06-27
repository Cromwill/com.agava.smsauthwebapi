using System;

namespace SmsAuthAPI.DTO
{
    [Serializable]
    public class Tokens
    {
        public string access;
        public string refresh;
    }
}
