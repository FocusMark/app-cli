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
        public string Path { get; set; }
        public string Color { get; set; }
    }
}
