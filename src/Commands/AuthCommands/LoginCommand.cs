using FocusMark.App.Cli.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FocusMark.App.Cli.Commands.AuthCommands
{
    [Command(Name = "login", Description = "Logs into your FocusMark account", UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.Throw, OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    public class LoginCommand : CommandBase
    {
        public LoginCommand(IConsole console, ILogger<LoginCommand> logger, IAuthorizationService authorizationService)
            : base(console, logger, authorizationService)
        {
        }

        protected override async Task<int> Execute(CommandLineApplication app)
        {
            bool isSuccessfulAuth = await base.AuthorizationService.AuthorizeUser();
            if (isSuccessfulAuth)
            {
                base.WriteInfoToConsole("Login Completed");
                return 0;
            }
            else
            {
                base.WriteErrorToConsole("Failed to log into your account");
                return 1;
            }
        }
    }
}
