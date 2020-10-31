﻿using FocusMark.App.Cli.Models;
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

        [Option("-p|--path", CommandOptionType.SingleOrNoValue, Description = "The path in the Project nesting hierarchy.")]
        public string Path { get; set; } = "/";

        //[Option("-sd|--start-date", CommandOptionType.SingleOrNoValue, Description = "The date that the Project will start on.")]
        //public DateTime? StartDate { get; set; }

        //[Option("-td|--target-date", CommandOptionType.SingleOrNoValue, Description = "The date that the Project is targeting for completion.")]
        //public DateTime? TargetDate { get; set; }

        //[Option("-k|--kind", CommandOptionType.SingleOrNoValue, Description = "The methodology kind that the Project will use. Allowed values currently are 'Kanban'")]
        //public ProjectKind Kind { get; set; } = ProjectKind.Kanban;

        [Option("-s|--status", CommandOptionType.SingleOrNoValue, Description = "The status of the Project.Valid Values: Planning = 0 (default), Active = 1, Archive = 2, Deleted = 3")]
        public ProjectStatus Status { get; set; } = ProjectStatus.Planning;

        [Option("-c|--color", CommandOptionType.SingleOrNoValue, Description = "The hex color that you want to represent this Project with")]
        public string Color { get; set; } = "#04d4b4";

        protected override async Task<int> Execute(CommandLineApplication app)
        {
            var createRequest = new CreateProjectRequest
            {
                Title = this.Title,
                Path = this.Path,
                Status = this.Status.ToString(),
                Kind = ProjectKind.Kanban.ToString(),
                Color = this.Color,
                TargetDate = 0,
                StartDate = 0
            };

            ApiResponse<CreateProjectResponse> createResponse;

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
