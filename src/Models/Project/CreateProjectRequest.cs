using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FocusMark.App.Cli.Models.Project
{
    public class CreateProjectRequest
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("targetDate")]
        public long TargetDate { get; set; }

        [JsonProperty("startDate")]
        public long StartDate { get; set; }
    }
}
