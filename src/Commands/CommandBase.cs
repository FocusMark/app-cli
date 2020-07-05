using FocusMark.App.Cli.Commands.AuthCommands;
using FocusMark.App.Cli.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace FocusMark.App.Cli.Commands
{
    public abstract class CommandBase
    {
        public CommandBase(IConsole console, ILogger logger, IAuthorizationService authorizationService)
        {
            Console = console ?? throw new ArgumentNullException(nameof(console));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            AuthorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        }

        protected IConsole Console { get; }
        public ILogger Logger { get; }
        public IAuthorizationService AuthorizationService { get; }

        public async Task<int> OnExecute(CommandLineApplication app)
        {
            this.Logger.LogInformation("Checking for authorization");
            bool isUserAuthorized = await this.AuthorizationService.IsUserAuthorized();

            if (!isUserAuthorized && this.GetType().GetCustomAttribute<UnauthorizedAttribute>() == null)
            {
                this.Logger.LogError("User is not logged into an account. CLI will abort.");
                this.WriteErrorToConsole("You must be logged in first.");
                app.ShowHelp();
                return 1;
            }

            this.Logger.LogInformation("Authorization verified. Executing command");
            return await this.Execute(app);
        }

        protected abstract Task<int> Execute(CommandLineApplication app);

        protected void WriteInfoToConsole(string message)
        {
            this.Console.BackgroundColor = ConsoleColor.Black;
            this.Console.ForegroundColor = ConsoleColor.Yellow;
            this.Console.Out.WriteLine(message);
            this.Console.ResetColor();
        }

        protected void WriteErrorToConsole(string message)
        {
            this.Console.BackgroundColor = ConsoleColor.Red;
            this.Console.ForegroundColor = ConsoleColor.White;
            this.Console.Error.WriteLine(message);
            this.Console.ResetColor();
        }
    }
}
