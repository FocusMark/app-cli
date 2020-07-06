using FocusMark.App.Cli.Models.Auth;
using Newtonsoft.Json;
using System;
using System.Text;

namespace FocusMark.App.Cli.Models
{
    public class JwtTokens
    {
        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        public bool IsAccessTokenExpired()
        {
            AccessToken token = this.GetAccessToken();
            if (token == null)
            {
                return true;
            }

            DateTime expiration = DateTimeOffset.FromUnixTimeSeconds(token.ExpiresAt).LocalDateTime;

            return expiration <= DateTime.Now;
        }

        public bool IsIdTokenExpired()
        {
            IdToken token = this.GetIdToken();
            if (token == null)
            {
                return true;
            }

            DateTime expiration = new DateTime(token.ExpiresAt);

            return expiration >= DateTime.Now;
        }

        public AccessToken GetAccessToken()
        {
            if (this.AccessToken == null)
            {
                return null;
            }

            string[] tokenParts = this.AccessToken.Split('.');
            if (tokenParts.Length != 3)
            {
                return null;
            }

            string encodedPayload = tokenParts[1];
            // .net allowed for improperly padded encoded strings while .net core is strict with it.
            // Not all tokens arrive with a length being a multiple of 4. Padding with '=' is required to make it so.
            encodedPayload = encodedPayload.PadRight(4 * ((encodedPayload.Length + 3) / 4), '=');
            byte[] jsonPayloadBuffer = Convert.FromBase64String(encodedPayload);
            string jsonPayload = Encoding.UTF8.GetString(jsonPayloadBuffer);

            return JsonConvert.DeserializeObject<AccessToken>(jsonPayload);
        }

        public IdToken GetIdToken()
        {
            if (this.IdToken == null)
            {
                return null;
            }

            string[] tokenParts = this.IdToken.Split('.');
            if (tokenParts.Length != 3)
            {
                return null;
            }

            string encodedPayload = tokenParts[1];
            // .net allowed for improperly padded encoded strings while .net core is strict with it.
            // Not all tokens arrive with a length being a multiple of 4. Padding with '=' is required to make it so.
            encodedPayload = encodedPayload.PadRight(4 * ((encodedPayload.Length + 3) / 4), '=');
            byte[] jsonPayloadBuffer = Convert.FromBase64String(encodedPayload);
            string jsonPayload = Encoding.UTF8.GetString(jsonPayloadBuffer);

            return JsonConvert.DeserializeObject<IdToken>(jsonPayload);
        }
    }
}
