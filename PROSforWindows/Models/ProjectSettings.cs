using Newtonsoft.Json;
using System.Collections.Generic;

namespace PROSforWindows.Models
{
    /// <summary>
    /// Wrapper class for PROS project settings
    /// </summary>
    public class ProjectSettings
    {
        [JsonProperty("buttons")]
        public List<Button> Buttons { get; set; } = new List<Button>();

        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }
}
