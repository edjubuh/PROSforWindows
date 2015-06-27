using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace PROSforWindows.Models
{
    public class Button : INotifyPropertyChanged
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("desc")]
        public string Description { get; set; }

        [JsonProperty("batchPath")]
        public string BatchPath { get; set; }

        [JsonProperty("buttons")]
        public List<Button> Buttons { get; set; }

        Project _project;
        public Project Project
        {
            get { return _project; }
            set
            {
                if (_project != value)
                {
                    _project = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Project)));
                }
            }
        }

        static Regex regex = new Regex(@"{(?<key>\w{1,}),(?<desc>(\w|\s|\d){1,})}");

        public async void ExecuteCommand()
        {
            if (!File.Exists(BatchPath)) throw new FileNotFoundException("Unable to find file", BatchPath);
            if (!BatchPath.EndsWith(".pbat")) throw new FileFormatException("File: '" + BatchPath + "' not a .pbat");

            Project.Output += "\n\n\n";

            LinkedList<string> commands = new LinkedList<string>();
            using (var file = new StreamReader(BatchPath))
            {
                string command;
                while ((command = file.ReadLine()) != null)
                {
                    var matches = regex.Matches(command);
                    foreach (Match match in matches)
                    {
                        var key = match.Groups["key"].Value;

                    }
                }
            }
        }

        [OnDeserialized]
        public void OnDeserialied(StreamingContext context)
        {
            Project = context.Context as Project;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
