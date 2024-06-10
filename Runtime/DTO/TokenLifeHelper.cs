using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using UnityEngine;
using Newtonsoft.Json;
using SmsAuthAPI.Utility;
using SmsAuthAPI.Program;
using UnityEngine.Networking;

namespace SmsAuthAPI.DTO
{
    public static class TokenLifeHelper
    {
        public static readonly string Tokens = nameof(Tokens);

        public static bool IsTokenAlive(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken jwtToken = handler.ReadJwtToken(token);

            var expiryTime = Convert.ToInt64(jwtToken.Claims.First(claim => claim.Type == "exp").Value);

            DateTime expiryDateTime = DateTimeOffset.FromUnixTimeSeconds(expiryTime).UtcDateTime;

            return expiryDateTime > DateTime.UtcNow;
        }

        public static async Task<string> GetRefreshedToken(string token)
        {
            var refreshResponse = await SmsAuthApi.Refresh(token);

            if (refreshResponse.statusCode == UnityWebRequest.Result.Success)
            {
                Tokens tokensBack;

                if (refreshResponse.isBase64Encoded)
                {
                    byte[] bytes = Convert.FromBase64String(refreshResponse.body);
                    string json = Encoding.UTF8.GetString(bytes);
                    tokensBack = JsonConvert.DeserializeObject<Tokens>(json);
                }
                else
                {
                    tokensBack = JsonConvert.DeserializeObject<Tokens>(refreshResponse.body);
                }

                SaveLoadLocalDataService.Save(tokensBack, Tokens);
                return tokensBack.access;
            }
            else
            {
                Debug.LogError($"Refresh Token Validation Error :{refreshResponse.statusCode}-{refreshResponse.body}");
                return string.Empty;
            }
        }

        public static async Task<bool> IsTokensAlive(Tokens tokens)
        {
            if (IsTokenAlive(tokens.access))
            {
                return true;
            }
            else if (IsTokenAlive(tokens.refresh))
            {
                var currentToken = await GetRefreshedToken(tokens.refresh);

                if (string.IsNullOrEmpty(currentToken))
                    return false;
                else
                    return true;
            }
            else
            {
                SaveLoadLocalDataService.Delete(Tokens);
                return false;
            }
        }

        public static Tokens GetTokens() => SaveLoadLocalDataService.Load<Tokens>(Tokens);

        public static void ClearTokens() => SaveLoadLocalDataService.Delete(Tokens);
    }
}
