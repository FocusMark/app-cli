using FocusMark.App.Cli.Data;
using FocusMark.App.Cli.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FocusMark.App.Cli.Services
{
    public class OAuthAuthorizationService : IAuthorizationService
    {
        private readonly ITokenRepository tokenRepository;
        private readonly ILoginService loginService;
        private readonly ILogger<OAuthAuthorizationService> logger;

        public OAuthAuthorizationService(ITokenRepository tokenRepository, ILoginService loginService, ILogger<OAuthAuthorizationService> logger)
        {
            this.tokenRepository = tokenRepository ?? throw new ArgumentNullException(nameof(tokenRepository));
            this.loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> AuthorizeUser()
        {
            await this.DeauthorizeUser();

            JwtTokens oauthTokens = await this.loginService.Login();
            if (oauthTokens == null)
            {
                return false;
            }

            try
            {
                await this.tokenRepository.SaveTokens(oauthTokens);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Task DeauthorizeUser()
        {
            return this.tokenRepository.DeleteTokens();
        }

        public async Task<JwtTokens> GetTokens()
        {
            this.logger.LogInformation("Fetching previously retrieved tokens.");
            JwtTokens tokens = await this.tokenRepository.GetTokens();
            if (tokens == null)
            {
                this.logger.LogError("User is not currently logged into their account.");
            }

            return tokens;
        }

        public async Task<bool> IsUserAuthorized()
        {
            JwtTokens tokens = await this.GetTokens();
            if (tokens == null)
            {
                return false;
            }

            return !tokens.IsAccessTokenExpired();
        }

        public Task<bool> RefreshAuthorization()
        {
            throw new NotImplementedException();
        }
    }
}
