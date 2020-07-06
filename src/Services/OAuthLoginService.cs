using FocusMark.App.Cli.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FocusMark.App.Cli.Services
{
    public class OAuthLoginService : ILoginService
    {
        // OAuth fields
        private const string authFlow = "code";
        private const string redirectUri = "http://localhost:9000";

        private readonly IHttpClientFactory clientFactory;
        private AuthConfiguration authConfiguration;

        public OAuthLoginService(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            IConfigurationSection focusmarkSection = configuration.GetSection("FocusMark");
            this.authConfiguration = new AuthConfiguration();
            focusmarkSection.Bind(this.authConfiguration);

            this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<JwtTokens> Login()
        {
            if (!HttpListener.IsSupported)
            {
                throw new HttpListenerNotSupportedException();
            }

            var listener = new HttpListener();
            listener.Prefixes.Add($"{redirectUri}/");
            listener.Start();

            HttpListenerContext requestContext = await this.HandleAuthCallback(listener);
            HttpListenerRequest httpRequest = requestContext.Request;

            string authCode = httpRequest.QueryString["code"];
            JwtTokens tokens = await this.RequestJwtTokens(authCode);
            if (tokens == null)
            {
                await this.RespondWithFailedLogin(requestContext);
                listener.Stop();
                return null;
            }

            await this.RespondWithSuccessfulLogin(requestContext);
            listener.Stop();

            return tokens;
        }

        private async Task<HttpListenerContext> HandleAuthCallback(HttpListener listener)
        {
            string[] requestedScopes = new string[] { AuthorizationScopes.OpenId, AuthorizationScopes.ApiProjectRead, AuthorizationScopes.ApiProjectWrite };
            string flattenedScopes = string.Join('+', requestedScopes);

            string url = $"{this.authConfiguration.AuthUrl}/{this.authConfiguration.LoginPath}?client_id={this.authConfiguration.ClientId}&response_type={authFlow}&scope={flattenedScopes}&redirect_uri={redirectUri}";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }

            HttpListenerContext context = await listener.GetContextAsync();
            return context;
        }

        private async Task<JwtTokens> RequestJwtTokens(string authCode)
        {
            HttpClient httpClient = this.clientFactory.CreateClient("Jwt Token Request");

            var bodyContent = new List<KeyValuePair<string, string>>()
            { 
                new KeyValuePair<string, string>("code", authCode),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", this.authConfiguration.ClientId),
                new KeyValuePair<string, string>("redirect_uri", redirectUri), 
            };

            var body = new FormUrlEncodedContent(bodyContent);
            var authResponse = await httpClient.PostAsync($"{this.authConfiguration.AuthUrl}/oauth2/token/", body);
            var tokenPayload = await authResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<JwtTokens>(tokenPayload);
        }

        private async Task RespondWithFailedLogin(HttpListenerContext requestContext)
        {
            byte[] failedBuffer = Encoding.UTF8.GetBytes("<html><body>FocusMark failed to log you in.</body></html>");

            requestContext.Response.StatusCode = 500;
            requestContext.Response.ContentLength64 = failedBuffer.Length;

            await requestContext.Response.OutputStream.WriteAsync(failedBuffer);
            requestContext.Response.OutputStream.Close();
        }

        private async Task RespondWithSuccessfulLogin(HttpListenerContext requestContext)
        {
            byte[] sucessBuffer = Encoding.UTF8.GetBytes("<html><body>FocusMark Login Completed.</body></html>");

            requestContext.Response.StatusCode = 200;
            requestContext.Response.ContentLength64 = sucessBuffer.Length;

            await requestContext.Response.OutputStream.WriteAsync(sucessBuffer);
            requestContext.Response.OutputStream.Close();
        }
    }
}
