using FocusMark.App.Cli.Models;
using LiteDB;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FocusMark.App.Cli.Data
{
    public class JwtTokenRepository : ITokenRepository
    {
        // We name it something a little obscure just to not draw attention to what it's contents hold.
        private const string DatabaseName = "dat.tmp";
        private const string TokenCollection = "tokens";

        // IDataProtectionProvider specific
        private const string TokenProtector = "focusmark:cli:auth";
        private const string AccessTokenProtector = "focusmark:cli:auth:access";
        private const string IdTokenProtector = "focusmark:cli:auth:id";
        private const string RefreshTokenProtector = "focusmark:cli:auth:refresh";

        private readonly IDatabaseFactory databaseFactory;
        private readonly IDataProtectionProvider protectionProvider;
        private readonly ILogger<JwtTokenRepository> logger;

        public JwtTokenRepository(IDatabaseFactory databaseFactory, IDataProtectionProvider protectionProvider, ILogger<JwtTokenRepository> logger)
        {
            this.databaseFactory = databaseFactory ?? throw new ArgumentNullException(nameof(databaseFactory));
            this.protectionProvider = protectionProvider ?? throw new ArgumentNullException(nameof(protectionProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task SaveTokens(JwtTokens tokens)
        {
            if (tokens is null || string.IsNullOrEmpty(tokens.AccessToken) || string.IsNullOrEmpty(tokens.IdToken) || string.IsNullOrEmpty(tokens.RefreshToken))
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            JwtTokens protectedTokens = this.ProtectTokens(tokens);
            ILiteDatabase database = this.databaseFactory.GetDatabase(DatabaseName);
            using (database)
            {
                ILiteCollection<JwtTokens> tokenCollection = database.GetCollection<JwtTokens>(TokenCollection);
                tokenCollection.Insert(protectedTokens);
            }

            return Task.CompletedTask;
        }

        public Task DeleteTokens()
        {
            ILiteDatabase database = this.databaseFactory.GetDatabase(DatabaseName);
            using (database)
            {
                database.DropCollection(TokenCollection);
            }

            return Task.CompletedTask;
        }

        public Task<JwtTokens> GetTokens()
        {
            // Get a reference to the database that stores the tokens.
            ILiteDatabase database = this.databaseFactory.GetDatabase(DatabaseName);
            JwtTokens protectedTokens = null;
            using (database)
            {
                ILiteCollection<JwtTokens> tokenCollection = database.GetCollection<JwtTokens>(TokenCollection);

                // Find the access token record. There should only ever be 1 record in the database at a time.
                protectedTokens = tokenCollection.FindOne(token => !string.IsNullOrEmpty(token.AccessToken));
                if (protectedTokens == null)
                {
                    return Task.FromResult<JwtTokens>(null);
                }
            }

            // Unprotect the tokens and return them.
            try
            {
                JwtTokens unprotectedTokens = this.UnprotectTokens(protectedTokens);
                return Task.FromResult(unprotectedTokens);
            }
            catch (CryptographicException)
            {
                this.logger.LogError("Failed to unprotect tokens. Potentially tampered with.");
                return Task.FromResult<JwtTokens>(null);
            }
        }

        private JwtTokens ProtectTokens(JwtTokens jwtTokens)
        {
            IDataProtector authProtector = this.protectionProvider.CreateProtector(TokenProtector);

            // Protect Access Token with a unique protector
            IDataProtector accessTokenProtector = authProtector.CreateProtector(AccessTokenProtector);
            string encryptedAccessToken = accessTokenProtector.Protect(jwtTokens.AccessToken);

            // Protect Id Token with a unique protector
            IDataProtector idTokenProtector = authProtector.CreateProtector(IdTokenProtector);
            string encryptedIdToken = idTokenProtector.Protect(jwtTokens.IdToken);

            // Protect Refresh Token with a unique protector
            IDataProtector refreshTokenProtector = authProtector.CreateProtector(RefreshTokenProtector);
            string encryptedRefreshToken = refreshTokenProtector.Protect(jwtTokens.RefreshToken);

            return new JwtTokens
            {
                AccessToken = encryptedAccessToken,
                IdToken = encryptedIdToken,
                RefreshToken = encryptedRefreshToken,
            };
        }

        private JwtTokens UnprotectTokens(JwtTokens protectedTokens)
        {
            // Create a Protector and start unprotecting the tokens
            IDataProtector authProtector = this.protectionProvider.CreateProtector(TokenProtector);

            IDataProtector accessTokenProtector = authProtector.CreateProtector(AccessTokenProtector);
            string accessToken = accessTokenProtector.Unprotect(protectedTokens.AccessToken);

            IDataProtector idTokenProtector = authProtector.CreateProtector(IdTokenProtector);
            string idToken = idTokenProtector.Unprotect(protectedTokens.IdToken);

            IDataProtector refreshTokenProtector = authProtector.CreateProtector(RefreshTokenProtector);
            string refreshToken = refreshTokenProtector.Unprotect(protectedTokens.RefreshToken);

            // Return the unprotected tokens for use.
            return new JwtTokens
            {
                AccessToken = accessToken,
                IdToken = idToken,
                RefreshToken = refreshToken,
            };
        }
    }
}
