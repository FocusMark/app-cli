using Newtonsoft.Json;

namespace FocusMark.App.Cli.Models.Project
{
    public class CreateProjectResponse
    {
        [JsonProperty("projectId")]
        public string ProjectId { get; set; }
    }
}
