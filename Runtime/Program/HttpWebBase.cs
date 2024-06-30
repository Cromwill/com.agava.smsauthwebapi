using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace SmsAuthAPI.Program
{
    internal abstract class HttpWebBase
    {
        private const string RootApi = "/api";
        private const string AppJson = "application/json";
        private const string ContentType = "Content-Type";
        protected const int TimeOut = 59;

        private readonly string Ip;

        protected enum RequestType { POST, GET, PUT }

        public HttpWebBase(string connectId) => Ip = connectId;

        protected void OnTryConnecting(string path)
        {
#if UNITY_EDITOR || TEST
            Debug.Log(path);
#endif
        }

        protected string GetHttpPath(string apiName, string apiData = null, bool api = true)
        {
            apiData ??= string.Empty;

            string apiRoute = string.Empty;

            if (api)
                apiRoute = RootApi;

            string path = $"{Ip}{apiRoute}/{apiName.ToLower()}/{apiData}";

            return $"https://{path}";
        }

        protected UnityWebRequest CreateWebRequest(string path, RequestType type, string accessToken = null, string uploadBody = null, bool timeOut = true)
        {
            Debug.Log($"path - {path}\nReq type - {type}\ntoken - {accessToken}\n, body - {uploadBody}\ntimeout - {timeOut}");
            var webRequest = new UnityWebRequest(path, type.ToString());
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            if (timeOut)
                webRequest.timeout = TimeOut;

            if (string.IsNullOrEmpty(uploadBody) == false)
                webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(uploadBody));

            webRequest.SetRequestHeader(ContentType, AppJson);

            if (string.IsNullOrEmpty(accessToken) == false)
                webRequest.SetRequestHeader("Authorization", $"Bearer {accessToken}");

            Debug.Log($"uri - {webRequest.uri}\n" +
                $"url - {webRequest.url}\n" +
                $"error - {webRequest.error}");
            return webRequest;
        }

        protected async Task WaitProccessing(UnityWebRequest webRequest, Action<float> progress = null)
        {
            while (webRequest.result == UnityWebRequest.Result.InProgress)
            {
                progress?.Invoke(webRequest.downloadProgress);
                await Task.Yield();
            }
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

    public class CertificateHandlerPublicKey : CertificateHandler
    {
        private static readonly string s_publicKey = "somepublickey";

        protected override bool ValidateCertificate(byte[] certificateData)
        {
            X509Certificate2 certificate = new X509Certificate2(certificateData);
            string pk = certificate.GetPublicKeyString();

            Debug.Log(pk);

            if (pk.ToLower().Equals(s_publicKey.ToLower()))
                return true;

            return false;
        }
    }
}
