using FocusMark.App.Cli.Data;
using FocusMark.App.Cli.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FocusMark.App.Cli
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCliServices(this IServiceCollection services)
        {
            // Services
            services.AddSingleton<IAuthorizationService, OAuthAuthorizationService>();
            services.AddSingleton<ILoginService, OAuthLoginService>();
            services.AddSingleton<IProjectService, ProjectService>();

            // Data
            services.AddSingleton<ITokenRepository, JwtTokenRepository>();
            services.AddSingleton<IDatabaseFactory, LiteDatabaseFactory>();

            return services;
        }
    }
}
