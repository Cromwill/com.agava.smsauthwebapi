using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace SmsAuthAPI.Program
{
    internal abstract class HttpWebBase
    {
        private const string RootApi = "api";
        private const string AppJson = "application/json";
        private const string ContentType = "Content-Type";
        protected const int TimeOut = 59;

        private readonly string Ip;

        protected enum RequestType { POST, GET, PUT }

        public HttpWebBase(string connectId)
        {
            Ip = connectId;
#if UNITY_EDITOR || TEST
            Debug.Log("http id: " + Ip);
#endif
        }

        protected void OnTryConnecting(string path)
        {
#if UNITY_EDITOR || TEST
            Debug.Log(path);
#endif
        }

        protected string GetHttpPath(string apiName, string apiData = null)
        {
            apiData ??= string.Empty;

            string path = $"{Ip}/{RootApi}/{apiName.ToLower()}/{apiData}";

#if UNITY_EDITOR && TEST
            return $"http://{path}";
#else
            return $"https://{path}";
#endif
        }

        protected UnityWebRequest CreateWebRequest(string path, RequestType type, string accessToken = null, string uploadBody = null, bool timeOut = true)
        {
            var webRequest = new UnityWebRequest(path, type.ToString());
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            if (timeOut)
                webRequest.timeout = TimeOut;

            if (string.IsNullOrEmpty(uploadBody) == false)
                webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(uploadBody));

            webRequest.SetRequestHeader(ContentType, AppJson);

            if (string.IsNullOrEmpty(accessToken) == false)
                webRequest.SetRequestHeader("Authorization", $"Bearer {accessToken}");

            return webRequest;
        }

        protected async Task WaitProccessing(UnityWebRequest webRequest)
        {
            while (webRequest.result == UnityWebRequest.Result.InProgress)
                await Task.Yield();
        }

        protected void TryShowRequestInfo(UnityWebRequest webRequest, string method)
        {
#if UNITY_EDITOR || TEST
            Debug.Log($"response {method} done {webRequest.result}. Result: {webRequest.downloadHandler.text}");
#endif
            if (webRequest.result != UnityWebRequest.Result.Success)
                Debug.LogError($"Response {method} fail: {webRequest.error}, {webRequest.result}");
        }
    }
}
