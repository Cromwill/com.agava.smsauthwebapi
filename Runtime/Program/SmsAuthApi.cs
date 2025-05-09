﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmsAuthAPI.DTO;
using UnityEngine;
using UnityEngine.Networking;

namespace SmsAuthAPI.Program
{
    public static partial class SmsAuthApi
    {
        private static HttpWebClient _httpClient;
        private static string _appId;

        public static bool Initialized => _httpClient != null;
        public static event Action<float> DownloadCloudSavesProgress;

        public static void Initialize(string connectId, string appId)
        {
            if (string.IsNullOrEmpty(connectId))
                throw new InvalidOperationException(nameof(SmsAuthApi) + " Ip not entered");

            if (string.IsNullOrEmpty(appId))
                throw new InvalidOperationException(nameof(SmsAuthApi) + " appId not entered");

            if (Initialized)
                throw new InvalidOperationException(nameof(SmsAuthApi) + " has already been initialized");

            _appId = appId;
            _httpClient = new HttpWebClient(connectId);
        }

        public async static Task<Response> Login(LoginData loginData)
        {
            EnsureInitialize();

            Debug.Log($"Phone: {loginData.phone} Code: {loginData.otp_code} Device_id: {loginData.device_id} App_id: {loginData.app_id}");

            var request = new Request()
            {
                apiName = "Login",
                body = System.Text.Json.JsonSerializer.Serialize(loginData, typeof(LoginData)),
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

        public async static Task<Response> GetDevices(string accessToken, string app_id)
        {
            EnsureInitialize();

            var request = new Request()
            {
                apiName = "Login",
                body = app_id,
                access_token = accessToken,
            };

            return await _httpClient.GetDevices(request);
        }

        public async static Task<Response> Unlink(string accessToken, UnlinkData unlinkData)
        {
            EnsureInitialize();

            var request = new Request()
            {
                apiName = "Unlink",
                body = System.Text.Json.JsonSerializer.Serialize(unlinkData, typeof(UnlinkData)),
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

        public async static Task<Response> GetRemoteServerConfig()
        {
            EnsureInitialize();
            return await _httpClient.GetServerRemote("RemoteConfig/getremote-table");
        }

        public async static Task<Response> GetPluginSettings(string remoteName)
        {
            EnsureInitialize();
            return await _httpClient.GetPluginSettings("RemoteConfig", remoteName);
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

            return await _httpClient.SetCloudData(request, key: _appId);
        }

        public async static Task<Response> GetSave(string accessToken)
        {
            EnsureInitialize();

            var request = new Request()
            {
                apiName = "CloudSave",
                body = _appId,
                access_token = accessToken,
            };

            return await _httpClient.GetCloudData(request, DownloadCloudSavesProgress);
        }

        public async static Task<Response> DeleteAccount(string accessToken)
        {
            EnsureInitialize();

            var request = new Request()
            {
                apiName = "DeleteAccount",
                body = _appId,
                access_token = accessToken,
            };

            return await _httpClient.DeleteAccount(request, key: _appId);
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

        public async static Task<Response> HasTempActiveAccount(string phoneNumber, string accessToken)
        {
            EnsureInitialize();

            var request = new Request()
            {
                apiName = "account/subscription/temp/get-user",
                body = phoneNumber,
                access_token = accessToken,
            };

            return await _httpClient.HasTempActiveAccount(request);
        }

        public async static Task<Response> SendTempActiveAccountData(string phoneNumber, string accessToken)
        {
            EnsureInitialize();

            var request = new Request()
            {
                apiName = "account/subscription/temp/activation",
                body = phoneNumber,
                access_token = accessToken,
            };

            return await _httpClient.SendTempActiveAccountData(request);
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

        public static async Task<Response> SendStartData(StartUserData data)
        {
            EnsureInitialize();

            var request = new Request()
            {
                apiName = "Registration/start-save-data",
                body = JsonConvert.SerializeObject(data),
            };

            return await _httpClient.SendStartData(request);
        }

        private static void EnsureInitialize()
        {
            if (Initialized == false)
                throw new InvalidOperationException(nameof(SmsAuthApi) + " is not initialized");
        }
    }

    public static partial class SmsAuthApi
    {
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

        public async static void SetTimespentAllApp(string phone, string appId, ulong seconds)
        {
            EnsureInitialize();

            string data = JsonConvert.SerializeObject(new TimespentUserAllAppData()
            {
                phone = phone,
                app_id = appId,
                seconds = seconds,
            });

            var request = new Request()
            {
                apiName = "Analytics/timespent-user-app",
                body = data,
            };

            await _httpClient.SetTimespent(request);
        }

        public async static void SendEventSubscriberData(string san, string phone, string date, string appId, string version, string platform)
        {
            EnsureInitialize();

            string data = JsonConvert.SerializeObject(new SubscriberEventsData()
            {
                san = san,
                phone = phone,
                date = date,
                app_id = appId,
                version = version,
                platform = platform,
            });

            var request = new Request()
            {
                apiName = "Analytics/send-event-subscriber-data",
                body = data,
            };

            await _httpClient.SendEventSubscriberData(request);
        }

        public async static void OnUserAddApp(string phone, string sanId, string appId)
        {
            EnsureInitialize();

            string data = JsonConvert.SerializeObject(new AverageCountAppsUserData()
            {
                phone = phone,
                san_id = sanId,
                app_id = appId,
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
