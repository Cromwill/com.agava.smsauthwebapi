using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmsAuthAPI.DTO;
using UnityEngine;

namespace SmsAuthAPI.Program
{
    public static partial class SmsAuthApi
    {
        private static HttpWebClient _httpClient;
        private static string _uniqueId;

        public static bool Initialized => _httpClient != null;
        public static event Action<float> DownloadCloudSavesProgress;

        public static void Initialize(string connectId, string uniqueId)
        {
            if (string.IsNullOrEmpty(connectId))
                throw new InvalidOperationException(nameof(SmsAuthApi) + " Ip not entered");

            if (string.IsNullOrEmpty(uniqueId))
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

            return await _httpClient.GetCloudData(request, DownloadCloudSavesProgress);
        }

        public async static Task<Response> HasActiveAccount(string phoneNumber)
        {
            EnsureInitialize();

            var request = new Request()
            {
                apiName = "Account/subscription",
                body = phoneNumber,
            };

            return await _httpClient.HasActiveAccount(request);
        }

        public async static Task<Response> GetSanId(string phoneNumber)
        {
            EnsureInitialize();

            var request = new Request()
            {
                apiName = "Account/subscription/get-san-id",
                body = phoneNumber,
            };

            return await _httpClient.GetSanId(request);
        }

        private static void EnsureInitialize()
        {
            if (Initialized == false)
                throw new InvalidOperationException(nameof(SmsAuthApi) + " is not initialized");
        }
    }

    public static partial class SmsAuthApi
    {
        class TimespentAllUsersData
        {
            public string app_id;
            public ulong minutes;
        }

        class TimespentUserAllAppData
        {
            public string phone;
            public string device_id;
            public ulong minutes;
        }

        class AverageCountAppsUserData
        {
            public string phone { get; set; }
            public string san_id { get; set; }
            public string app_ids { get; set; }
            public int count { get; set; }
        }

        public async static void SetTimespentAllUsers(string appId, ulong minutes)
        {
            EnsureInitialize();

            string data = JsonConvert.SerializeObject(new TimespentAllUsersData()
            {
                app_id = appId,
                minutes = minutes
            });

            var request = new Request()
            {
                apiName = "Analytics/timespent-all-users",
                body = data,
            };

            await _httpClient.SetTimespent(request);
        }

        public async static void SetTimespentAllApp(string phone, string deviceId, ulong minutes)
        {
            EnsureInitialize();

            string data = JsonConvert.SerializeObject(new TimespentUserAllAppData()
            {
                phone = phone,
                device_id = deviceId,
                minutes = minutes
            });

            var request = new Request()
            {
                apiName = "Analytics/timespent-all-app",
                body = data,
            };

            await _httpClient.SetTimespent(request);
        }

        public async static void OnUserAddApp(string phone, string sanId)
        {
            EnsureInitialize();
            Debug.Log($"{phone} {sanId}");
            string data = JsonConvert.SerializeObject(new AverageCountAppsUserData()
            {
                phone = phone,
                san_id = sanId,
                app_ids = Application.identifier,
                count = 0,
            });

            var request = new Request()
            {
                apiName = "Analytics/user-add-app",
                body = data,
            };

            await _httpClient.OnUserAddApp(request);
        }
    }

    #region Test function
#if UNITY_EDITOR || TEST
    public static partial class SmsAuthApi
    { 
        public async static Task<Response> WriteSaveClouds(string phoneNumber, string body)
        {
            EnsureInitialize();

            var request = new Request()
            {
                apiName = "StressTest",
                body = body,
            };

            return await _httpClient.WriteCloudData(request, $"cloud-data/{phoneNumber}");
        }

        public async static Task<Response> GetSaveCloud(string phoneNumber)
        {
            EnsureInitialize();
            return await _httpClient.GetCloudData("StressTest", $"cloud-data/{phoneNumber}");
        }

        public async static Task<Response> ClearAllSaveCloud(string password)
        {
            EnsureInitialize();
            return await _httpClient.ClearAllCloudData("StressTest", $"cloud-data/{password}");
        }

        public async static Task<Response> Write(string phoneNumber, ulong count)
        {
            EnsureInitialize();
            return await _httpClient.Write("StressTest", phoneNumber, count);
        }

        public async static Task<Response> ClearOtpTable()
        {
            EnsureInitialize();

            return await _httpClient.ClearOtp("StressTest", "clear-otp-codes");
        }

        public async static Task<Response> GetOtpsCount()
        {
            EnsureInitialize();

            return await _httpClient.GetOtpCount("StressTest");
        }

        public async static Task<Response> GetOtpsWrites(string otp)
        {
            EnsureInitialize();

            return await _httpClient.GetOtpWrites("StressTest", otp);
        }
    }
#endif
    #endregion
}
