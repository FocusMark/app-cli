using FocusMark.App.Cli.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FocusMark.App.Cli.Commands.AuthCommands
{
    [Unauthorized]
    [Command(Name = "logout", Description = "Logs out of your FocusMark account", UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.Throw, OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    public class LogoutCommand : CommandBase
    {
        public LogoutCommand(IConsole console, ILogger<LoginCommand> logger, IAuthorizationService authorizationService)
            : base(console, logger, authorizationService)
        {
        }

        protected override async Task<int> Execute(CommandLineApplication app)
        {
            try
            {
                await base.AuthorizationService.DeauthorizeUser();
                base.WriteInfoToConsole("Logout successful.");
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }
    }
}
