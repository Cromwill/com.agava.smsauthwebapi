using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmsAuthAPI.DTO;

namespace SmsAuthAPI.Program
{
    internal partial class HttpWebClient : HttpWebBase
    {
        public HttpWebClient(string connectId) : base(connectId) { }

        public async Task<Response> Login(Request request)
        {
            string path = $"{GetHttpPath(request.apiName)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.POST, string.Empty, request.body);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, request.apiName);

            return new Response(webRequest.result, webRequest.result.ToString(), webRequest.downloadHandler.text, false);
        }

        public async Task<Response> Regist(string apiName, string phone)
        {
            string path = $"{GetHttpPath(apiName, phone)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.POST);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, apiName);

            return new Response(webRequest.result, webRequest.result.ToString(), webRequest.downloadHandler.text, false);
        }

        public async Task<Response> Refresh(Request request)
        {
            string path = $"{GetHttpPath(request.apiName)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.POST, string.Empty, $"\"{request.refresh_token}\"");
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, request.apiName);

            return new Response(webRequest.result, webRequest.result.ToString(), webRequest.downloadHandler.text, false);
        }

        public async Task<Response> Unlink(Request request)
        {
            string path = $"{GetHttpPath(request.apiName, request.body)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.POST, request.access_token);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, request.apiName);

            return new Response(webRequest.result, webRequest.result.ToString(), webRequest.downloadHandler.text, false);
        }

        public async Task<Response> GetDevices(Request request)
        {
            string path = $"{GetHttpPath(request.apiName)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.GET, request.access_token);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, request.apiName);

            return new Response(webRequest.result, webRequest.result.ToString(), webRequest.downloadHandler.text, false);
        }

        public async Task<Response> GetRemote(string apiName, string key)
        {
            string path = $"{GetHttpPath(apiName, key.ToLower())}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.GET);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, apiName);

            var body = JsonConvert.DeserializeObject<string>(webRequest.downloadHandler.text);
            return new Response(webRequest.result, webRequest.result.ToString(), body, false);
        }

        public async Task<Response> SetCloudData(Request request, string key)
        {
            string path = $"{GetHttpPath(request.apiName, key)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.PUT, request.access_token, request.body);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, request.apiName);

            return new Response(webRequest.result, webRequest.result.ToString(), webRequest.downloadHandler.text, false);
        }

        public async Task<Response> GetCloudData(Request request, Action<float> progress)
        {
            string path = $"{GetHttpPath(request.apiName, request.body)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.GET, request.access_token);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest, progress);
            TryShowRequestInfo(webRequest, request.apiName);

            var body = JsonConvert.DeserializeObject<string>(webRequest.downloadHandler.text);
            return new Response(webRequest.result, webRequest.result.ToString(), body, false);
        }

        public async Task<Response> HasActiveAccount(Request request)
        {
            string path = $"{GetHttpPath(request.apiName, request.body, api: false)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.GET);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, request.apiName);

            return new Response(webRequest.result, webRequest.result.ToString(), webRequest.downloadHandler.text, false);
        }

        public async Task<Response> GetSanId(Request request)
        {
            string path = $"{GetHttpPath(request.apiName, request.body, api: false)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.GET);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, request.apiName);

            return new Response(webRequest.result, webRequest.result.ToString(), webRequest.downloadHandler.text, false);
        }

        public async Task<Response> SendSms(Request request)
        {
            string path = $"{GetHttpPath(request.apiName, request.body)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.POST);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, request.apiName);

            return new Response(webRequest.result, webRequest.result.ToString(), null, false);
        }


        public async Task<Response> SetTimespent(Request request)
        {
            string path = $"{GetHttpPath(request.apiName, null, api: false)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.POST, null, request.body);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, request.apiName);

            return new Response(webRequest.result, webRequest.result.ToString(), null, false);
        }

        public async Task<Response> OnUserAddApp(Request request)
        {
            string path = $"{GetHttpPath(request.apiName, null, api: false)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.POST, null, request.body);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, request.apiName);

            return new Response(webRequest.result, webRequest.result.ToString(), null, false);
        }
    }

    #region Test fuctions
#if UNITY_EDITOR || TEST
    internal partial class HttpWebClient : HttpWebBase 
    { 
        public async Task<Response> WriteCloudData(Request request, string phone)
        {
            string path = $"{GetHttpPath(request.apiName, phone)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.PUT, null, request.body);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, request.apiName);

            return new Response(webRequest.result, webRequest.result.ToString(), webRequest.downloadHandler.text, false);
        }

        public async Task<Response> GetCloudData(string apiName, string phone)
        {
            string path = $"{GetHttpPath(apiName, phone)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.GET);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, apiName);

            var body = JsonConvert.DeserializeObject<string>(webRequest.downloadHandler.text);
            return new Response(webRequest.result, webRequest.result.ToString(), body, false);
        }

        public async Task<Response> ClearAllCloudData(string apiName, string password)
        {
            string path = $"{GetHttpPath(apiName, password)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.POST);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, apiName);

            return new Response(webRequest.result, webRequest.result.ToString(), webRequest.downloadHandler.text, false);
        }

        public async Task<Response> Write(string apiName, string phone, ulong count)
        {
            string path = $"{GetHttpPath(apiName, phone)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.POST, null, count.ToString(), timeOut: false);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, apiName);

            return new Response(webRequest.result, webRequest.result.ToString(), webRequest.downloadHandler.text, false);
        }

        public async Task<Response> ClearOtp(string apiName, string route)
        {
            string path = $"{GetHttpPath(apiName, route)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.POST);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, apiName);

            return new Response(webRequest.result, webRequest.result.ToString(), webRequest.downloadHandler.text, false);
        }

        public async Task<Response> GetOtpCount(string apiName)
        {
            string path = $"{GetHttpPath(apiName)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.GET);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, apiName);

            return new Response(webRequest.result, webRequest.result.ToString(), webRequest.downloadHandler.text, false);
        }

        public async Task<Response> GetOtpWrites(string apiName, string otp)
        {
            string path = $"{GetHttpPath(apiName, otp)}";
            OnTryConnecting(path);

            var webRequest = CreateWebRequest(path, RequestType.GET);
            webRequest.SendWebRequest();

            await WaitProccessing(webRequest);
            TryShowRequestInfo(webRequest, apiName);

            return new Response(webRequest.result, webRequest.result.ToString(), webRequest.downloadHandler.text, false);
        }
    }
#endif
    #endregion
}
