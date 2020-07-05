using FocusMark.App.Cli.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Serilog.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FocusMark.App.Cli
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // Deserialize data from API calls onto Domain models with private setters
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings { ContractResolver = new NonPublicPropertiesResolver() };

            IHostBuilder builder = new HostBuilder();
            builder.ConfigureAppConfiguration(configBuilder =>
            {
                configBuilder.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"{AppDomain.CurrentDomain.BaseDirectory}\\appsettings.json", optional: true)
                    .AddEnvironmentVariables();

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configBuilder.Build())
                    .Enrich.FromLogContext()
                    .CreateLogger();
            });
            builder.ConfigureServices((hostContext, services) => ConfigureServices(services, hostContext.Configuration));

            try
            {
                return await builder.RunCommandLineApplicationAsync<FocusMarkCommand>(args);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "{Timestamp:HH:mm:ss.fff zzz} [{Level:w3}] {Message:lj}{NewLine}{Exception}");
                Console.WriteLine(ex.Message);
                return 1;
            }
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddCliServices();

            services.AddDataProtection();
            services.AddHttpClient();

            services.AddLogging(config =>
            {
                config.ClearProviders();
                config.AddProvider(new SerilogLoggerProvider(Log.Logger));
                string minimumLevel = configuration.GetSection("Serilog:MinimumLevel")?.Value;

                if (!string.IsNullOrEmpty(minimumLevel))
                {
                    config.SetMinimumLevel(Enum.Parse<LogLevel>(minimumLevel));
                }
            });
        }
    }
}
