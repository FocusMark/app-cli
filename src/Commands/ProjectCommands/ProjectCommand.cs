using FocusMark.App.Cli.Commands.AuthCommands;
using FocusMark.App.Cli.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FocusMark.App.Cli.Commands.ProjectCommands
{
    [Unauthorized]
    [Command(Name = "project", Description = "Provides commands that can be used to itneract with projects within your account.", UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.Throw, OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    [Subcommand(
        typeof(CreateProjectCommand))]
    public class ProjectCommand : CommandBase
    {
        public ProjectCommand(IConsole console, ILogger logger, IAuthorizationService authorizationService) 
            : base(console, logger, authorizationService)
        {
        }

        protected override Task<int> Execute(CommandLineApplication app)
        {
            app.ShowHelp();
            return Task.FromResult(0);
        }
    }
}
