using FocusMark.App.Cli.Models;
using FocusMark.App.Cli.Models.Project;
using FocusMark.App.Cli.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FocusMark.App.Cli.Commands.ProjectCommands
{
    [Command(Name = "create", Description = "Logs into your FocusMark account", UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.Throw, OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    public class CreateProjectCommand : CommandBase
    {
        private readonly IProjectService projectService;

        public CreateProjectCommand(IConsole console, ILogger<CreateProjectCommand> logger, IProjectService projectService, IAuthorizationService authorizationService)
            : base(console, logger, authorizationService)
        {
            this.projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        }

        [Option("-t|--title", CommandOptionType.SingleValue, Description = "The title of the project being created", LongName = "title", ShortName = "t")]
        [Required]
        public string Title { get; set; }

        public string Path { get; set; } = "/";

        protected override async Task<int> Execute(CommandLineApplication app)
        {
            var createRequest = new CreateProjectRequest
            {
                Title = this.Title
            };

            ApiResponse<string> createResponse;

            try
            {
                createResponse = await this.projectService.CreateProject(createRequest);
            }
            catch (UnauthorizedAccessException)
            {
                // TODO: Relogin
                base.WriteErrorToConsole("Unauthorized!");
                return 1;
            }
            catch(ApiUnavailableException)
            {
                base.WriteErrorToConsole("Unable to communicate with the server.");
                return 1;
            }

            if (createResponse.IsSuccessful)
            {
                base.WriteInfoToConsole($"{Title} created successfully.");
                return 0;
            }
            else
            {
                base.WriteErrorToConsole(string.Join("\n", createResponse.Errors));
                return 1;
            }
        }
    }
}
