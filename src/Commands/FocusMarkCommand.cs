using FocusMark.App.Cli.Commands.AuthCommands;
using FocusMark.App.Cli.Commands.ProjectCommands;
using FocusMark.App.Cli.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FocusMark.App.Cli.Commands
{
    [Command(Name = "fm", FullName = "focusmark", UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.Throw, OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    [Unauthorized]
    [VersionOptionFromMember("--version", MemberName = nameof(CommandVersion))]
    [Subcommand(
        typeof(LoginCommand),
        typeof(LogoutCommand),
        typeof(ProjectCommand))]
    public class FocusMarkCommand : CommandBase
    {
        public FocusMarkCommand(IConsole console, ILogger<FocusMarkCommand> logger, IAuthorizationService authorizationService)
            : base(console, logger, authorizationService)
        {
        }

        public string CommandVersion => "0.0.1";

        protected override Task<int> Execute(CommandLineApplication app)
        {
            app.ShowHelp();
            return Task.FromResult(0);
        }
    }
}
