using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmsAuthAPI.DTO;

namespace SmsAuthAPI.Program
{
    public static class SmsAuthApi
    {
        private static HttpWebClient _httpClient;
        private static string _uniqueId;

        public static bool Initialized => _httpClient != null;

        public static void Initialize(string connectId, string uniqueId)
        {
            if(string.IsNullOrEmpty(connectId))
                throw new InvalidOperationException(nameof(SmsAuthApi) + " Ip not entered");

            if(string.IsNullOrEmpty(uniqueId))
                throw new InvalidOperationException(nameof(SmsAuthApi) + " uniqueId not entered");

            if (Initialized)
                throw new InvalidOperationException(nameof(SmsAuthApi) + " has already been initialized");

            _uniqueId = uniqueId;
            _httpClient = new HttpWebClient(connectId);
        }

        public async static Task<Response> Login(LoginData loginData)
        {
            EnsureInitialize();

            var request = new Request()
            {
                apiName = "Login",
                body = JsonConvert.SerializeObject(loginData),
            };

            return await _httpClient.Login(request);
        }

        public async static Task<Response> Regist(string phoneNumber)
        {
            EnsureInitialize();
            return await _httpClient.Regist("Registration", phoneNumber);
        }

        public async static Task<Response> Refresh(string refreshToken)
        {
            EnsureInitialize();

            var request = new Request()
            {
                apiName = "Refresh",
                refresh_token = refreshToken,
            };

            return await _httpClient.Refresh(request);
        }

        public async static Task<Response> GetDevices(string accessToken)
        {
            EnsureInitialize();

            var request = new Request()
            {
                apiName = "Login",
                body = _uniqueId,
                access_token = accessToken,
            };

            return await _httpClient.GetDevices(request);
        }

        public async static Task<Response> Unlink(string accessToken, string deviceId)
        {
            EnsureInitialize();

            var request = new Request()
            {
                apiName = "Unlink",
                body = deviceId,
                access_token = accessToken,
            };

            return await _httpClient.Unlink(request);
        }

        public async static Task<Response> SampleAuth(string accessToken)
        {
            EnsureInitialize();
            await Task.Yield();
            var isTokenAlive = TokenLifeHelper.IsTokenAlive(accessToken);

            UnityEngine.Networking.UnityWebRequest.Result result;
            result = isTokenAlive == true ? UnityEngine.Networking.UnityWebRequest.Result.Success : UnityEngine.Networking.UnityWebRequest.Result.ConnectionError;

            return new Response(result, "Access Token expired", null, false);
        }

        public async static Task<Response> GetRemoteConfig(string remoteName)
        {
            EnsureInitialize();
            return await _httpClient.GetRemote("Remoteconfig", remoteName);
        }

        public async static Task<Response> SetSave(string accessToken, string body)
        {
            EnsureInitialize();

            var request = new Request()
            {
                apiName = "CloudSave",
                body = body,
                access_token = accessToken,
            };

            return await _httpClient.SetCloudData(request, key: _uniqueId);
        }

        public async static Task<Response> GetSave(string accessToken)
        {
            EnsureInitialize();

            var request = new Request()
            {
                apiName = "CloudSave",
                body = _uniqueId,
                access_token = accessToken,
            };

            return await _httpClient.GetCloudData(request);
        }

        private static void EnsureInitialize()
        {
            if (Initialized == false)
                throw new InvalidOperationException(nameof(SmsAuthApi) + " is not initialized");
        }

#if UNITY_EDITOR || TEST
        public async static Task<Response> Write(string phoneNumber, ulong count)
        {
            EnsureInitialize();
            return await _httpClient.Write("TestDataBase", phoneNumber, count);
        }

        public async static Task<Response> ClearOtpTable()
        {
            EnsureInitialize();

            return await _httpClient.ClearOtp("TestDataBase");
        }

        public async static Task<Response> GetOtpsCount()
        {
            EnsureInitialize();

            return await _httpClient.GetOtpCount("TestDataBase");
        }

        public async static Task<Response> GetOtpsWrites(string otp)
        {
            EnsureInitialize();

            return await _httpClient.GetOtpWrites("TestDataBase", otp);
        }
#endif
    }
}
