using PROSforWindows.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System.Threading.Tasks;

namespace PROSforWindows.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private MetroWindow window;
        private static string executingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        bool isExecuting = false;
        public bool IsExecuting
        {
            get
            {
                return isExecuting;
            }
            private set
            {
                if (value != isExecuting)
                {
                    isExecuting = value;
                    OnPropertyChanged(nameof(IsExecuting));
                }
            }
        }

        string consoleOutput = "";
        public string ConsoleOutput
        {
            get { return consoleOutput; }
            set
            {
                if (value != consoleOutput && !(value.Equals("\n\n\n") && string.IsNullOrEmpty(consoleOutput)))
                {
                    consoleOutput = value;
                    OnPropertyChanged(nameof(ConsoleOutput));
                }
            }
        }

        string projectDirectory = "";
        public string ProjectDirectory
        {
            get { return projectDirectory; }
            set
            {
                if (value != projectDirectory)
                {
                    projectDirectory = value;
                    OnPropertyChanged(nameof(ProjectDirectory));
                }
            }
        }


        Brush toolPanelBrush;
        public Brush ToolPanelBrush
        {
            get { return toolPanelBrush; }
            set
            {
                if (value != toolPanelBrush)
                {
                    toolPanelBrush = value;
                    OnPropertyChanged(nameof(ToolPanelBrush));
                }
            }
        }

        public ICommand WindowActivated { get; set; }
        void windowActivated(object o)
        {
            ToolPanelBrush = (Brush)(window.FindResource("CampusGoldBrush"));
        }

        public ICommand WindowDeactivated { get; set; }
        void windowDeactivated(object o)
        {
            ToolPanelBrush = (Brush)(window.FindResource("MoonDustGrayBrush"));
        }

        public ICommand ClearConsoleCommand { get; set; }
        void clearConsole(object o)
        {
            ConsoleOutput = "";
        }

        public ICommand OpenFolderCommand { get; set; }
        void openFolder(object o)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();
            if (File.Exists(dialog.SelectedPath + "\\firmware\\uniflash.jar"))
                ProjectDirectory = dialog.SelectedPath;
        }

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        public Dictionary<string, string> Parameters
        {
            get { return parameters; }
            private set
            {
                if (parameters != value)
                {
                    parameters = value;
                    OnPropertyChanged(nameof(Parameters));
                }
            }
        }

        static Regex regex = new Regex(@"{(?<key>\w{1,}),(?<desc>(\w|\s|\d){1,})}");
        async void ExecuteBatch(string path)
        {
            if (!path.EndsWith(".pbat") || !File.Exists(path)) throw new InvalidOperationException("Not a valid file");

            IsExecuting = true;
            ConsoleOutput += "\n\n\n";

            LinkedList<string> commands = new LinkedList<string>();
            using (var file = new StreamReader(path))
            {
                string command;
                while ((command = file.ReadLine()) != null)
                {
                    var matches = regex.Matches(command);
                    foreach (Match match in matches)
                    {
                        string key = match.Groups["key"].Value;
                        if (!parameters.ContainsKey(key))
                        {
                            parameters.Add(key,
                                await window.ShowInputAsync("Input for: " + key, match.Groups["desc"].Value));
                        }
                        command = command.Replace(match.Value, parameters[key]);
                    }
                    commands.AddLast(command);
                }
            }

            foreach (string command in commands)
            {
                executeCommand(command);
            }
            ConsoleOutput += "...done...";
            IsExecuting = false;
        }

        void ExecuteCommand(string command)
        {
            IsExecuting = true;
            ConsoleOutput += "\n\n\n";
            executeCommand(command);
            ConsoleOutput += "...done...";
            IsExecuting = false;
        }

        void executeCommand(string command, string workingDir = "")
        {
            // Work-around for compile-time constraint for default values on parameters
            if (string.IsNullOrWhiteSpace(workingDir)) workingDir = ProjectDirectory;

            ConsoleOutput += "> " + command + "\n";
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WorkingDirectory = workingDir;
            if (command.Contains(" "))
            {
                p.StartInfo.FileName = command.Split(' ')[0];
                p.StartInfo.Arguments = command.Substring(command.IndexOf(' '));
            }
            else
            {
                p.StartInfo.FileName = command;
            }
            p.Start();
            string error = "", output = "";
            while (!p.HasExited || (error = p.StandardError.ReadLine()) != null || (output = p.StandardOutput.ReadLine()) != null)
            {
                ConsoleOutput += string.IsNullOrEmpty(error) ? "" : (error + "\n");
                ConsoleOutput += string.IsNullOrEmpty(output) ? "" : (output + "\n");
            }
            ConsoleOutput += "\n";
        }

        public ObservableCollection<ButtonCommand> FileSystemCommands { get; set; }
        void uploadPreservingFileSystem(object o)
        {
            ExecuteCommand("echo Uploading Preserving File System");
        }
        void downloadFile(object o)
        {
            ExecuteCommand("echo downloading a file");
        }
        void uploadFile(object o)
        {
            ExecuteCommand("echo uploading a file");
        }


        public ICommand BuildCommand { get; set; }
        void buildCommand(object o)
        {
            ExecuteBatch(executingDirectory + "\\batch\\build.pbat");
        }

        public ICommand BuildUploadCommand { get; set; }
        void buildUploadCommand(object o)
        {
            ExecuteBatch(executingDirectory + "\\batch\\upload.pbat");
        }

        public ICommand CleanCommand { get; set; }
        void cleanCommand(object o)
        {
            ExecuteBatch(executingDirectory + "\\batch\\clean.pbat");
        }

        public ICommand ClearParametersCommand { get; set; }
        void clearParameters(object o)
        {
            Parameters.Clear();
        }


        public MainViewModel(MetroWindow window)
        {
            WindowActivated = new RelayCommand(windowActivated);
            WindowDeactivated = new RelayCommand(windowDeactivated);
            ClearConsoleCommand = new RelayCommand(clearConsole, _ => { return !string.IsNullOrWhiteSpace(ConsoleOutput); });
            OpenFolderCommand = new RelayCommand(openFolder);

            ClearParametersCommand = new ListenCommand
            {
                Execute = clearParameters,
                CanExecuteDelegate = (r) => Parameters.Count > 0
            }.ListenOn(this, t => Parameters);

            BuildCommand = createExecutionCommand(buildCommand);
            BuildUploadCommand = createExecutionCommand(buildUploadCommand);
            CleanCommand = createExecutionCommand(cleanCommand);

            FileSystemCommands = new ObservableCollection<ButtonCommand>(new List<ButtonCommand>()
                {
                    new ButtonCommand(createExecutionCommand(uploadPreservingFileSystem), "Upload preserving file system"),
                    new ButtonCommand(createExecutionCommand(uploadFile), "Upload file"),
                    new ButtonCommand(createExecutionCommand(downloadFile), "Download file")
                }
            );

            this.window = window;

            ToolPanelBrush = (Brush)(window.FindResource("CampusGoldBrush"));
        }

        ICommand createExecutionCommand(Action<object> action)
        {
            var command = new ListenCommand
            {
                Execute = action,
                CanExecuteDelegate = (r) => (!IsExecuting && !string.IsNullOrWhiteSpace(ProjectDirectory))
            }.ListenOn(this, t => t.IsExecuting)
            .ListenOn(this, t => t.ProjectDirectory);
            return command;
        }

        #region INotifyPropertyChanged members
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
