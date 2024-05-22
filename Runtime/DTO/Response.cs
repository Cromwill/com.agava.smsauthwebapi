using static UnityEngine.Networking.UnityWebRequest;

namespace SmsAuthAPI.DTO
{
    public class Response
    {
        public Response(Result statusCode, string reasonPhrase, string body, bool isBase64Encoded)
        {
            this.statusCode = statusCode;
            this.body = body;
            this.reasonPhrase = reasonPhrase;
            this.isBase64Encoded = isBase64Encoded;
        }

        public Result statusCode { get; private set; }
        public string reasonPhrase { get; private set; }
        public string body { get; private set; }
        public bool isBase64Encoded { get; private set; }
    }
}
