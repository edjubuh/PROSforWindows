using Newtonsoft.Json;
using PROSforWindows.Commands;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

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

        ICommand _executeCommand;
        public ICommand ExecuteCommand
        {
            get { return _executeCommand; }
            set
            {
                if (_executeCommand != value)
                {
                    _executeCommand = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExecuteCommand)));
                }
            }
        }
        public async void Execute(object o)
        {
            if (!File.Exists(BatchPath)) throw new FileNotFoundException("Unable to find file", BatchPath);
            if (!BatchPath.EndsWith(".pbat")) throw new FileFormatException("File: '" + BatchPath + "' not a .pbat");

            Project.IsExecuting = true;
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

                        if(!Project.Parameters.ContainsKey(key)) // We don't have it cached
                        {

                        }
                    }

                    commands.AddLast(command);
                }
            }

            foreach(var command in commands)
            {
                await executeCommand(command);
            }

            Project.Output += "...done...";
            Project.IsExecuting = false;
        }

        async Task executeCommand(string command)
        {
            await executeCommand(command, Project.DirectoryPath);
        }

        async Task executeCommand(string command, string workingDirectory)
        {

            Project.Output += "> " + command + "\n";
            using (Process process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WorkingDirectory = workingDirectory;

                if (command.Contains(" "))
                {
                    process.StartInfo.FileName = command.Split(' ')[0];
                    process.StartInfo.Arguments = command.Substring(command.IndexOf(' '));
                }
                else
                {
                    process.StartInfo.FileName = command;
                }

                Stopwatch stopwatch = new Stopwatch();

                process.OutputDataReceived += (s, e) =>
                {
                    Project.Output += e.Data + "\n";
                    stopwatch.Restart();
                };

                process.ErrorDataReceived += (s, e) =>
                {
                    Project.Output += e.Data + "\n";
                    stopwatch.Restart();
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                while (!process.HasExited || stopwatch.ElapsedMilliseconds < 500) await Task.Delay(1);
            }

            Project.Output += Project.Output.EndsWith("\n") ? "" : "\n";
        }

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
        

        [OnDeserialized]
        public void OnDeserialied(StreamingContext context)
        {
            Project = context.Context as Project;

            if (BatchPath != null && !File.Exists(BatchPath))
            {
                // Fixing BatchPath to be a full file extension by first searching for it in the project's directory and subdirectories
                var candidates = Directory.GetFiles(Project.DirectoryPath, BatchPath, SearchOption.AllDirectories);
                if (candidates.Length > 1)
                    throw new FileNotFoundException("Multiple " + BatchPath + " files were found.", BatchPath);

                if (candidates.Length == 1)
                    BatchPath = candidates[0];
                else // assert candidates.Length == 0
                { // Otherwise search for it in the default batch files
                    candidates = Directory.GetFiles(Project.executingDirectory + "\\Resources\\batch", BatchPath);
                    if (candidates.Length > 1)
                        throw new FileNotFoundException("Multiple " + BatchPath + " files were found in " + Project.executingDirectory + "\\Resources\\batch");

                    if (candidates.Length == 1)
                        BatchPath = candidates[0];
                    else
                        throw new FileNotFoundException("Unable to find " + BatchPath + " in the project or in the default batch files.");
                }
            }


            ExecuteCommand = new ListenRelayCommand()
            {
                Execute = Execute,
                CanExecuteDelegate = (r) => !Project.IsExecuting
            }.ListenOn(Project, p => p.IsExecuting);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
