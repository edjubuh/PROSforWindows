using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PROSforWindows.Commands;
using PROSforWindows.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

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
                if (value != consoleOutput && !(new Regex(@"^(\n){1,}$").IsMatch(value) && string.IsNullOrEmpty(consoleOutput)))
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

        #region Window Color Functionality
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
        #endregion

        #region Commands
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
        public ICommand SettingsCommand { get; set; }
        void openSettingsWindow(object o)
        {
            var window = new SettingsWindow();
            window.DataContext = new SettingsViewModel(this);
            window.ShowDialog();
        }
        #endregion

        #region Console Commands
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
                await executeCommand(command);
            }
            ConsoleOutput += "...done...";
            IsExecuting = false;
        }

        async void ExecuteCommand(string command)
        {
            IsExecuting = true;
            ConsoleOutput += "\n\n\n";
            await executeCommand(command);
            ConsoleOutput += "...done...";
            IsExecuting = false;
        }

        async Task executeCommand(string command)
        {
            await executeCommand(command, ProjectDirectory);
        }

        private DataReceivedEventHandler dataRecievedHandler;

        async Task executeCommand(string command, string workingDirectory)
        {

            ConsoleOutput += "> " + command + "\n";
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
                    ConsoleOutput += e.Data + "\n";
                    stopwatch.Restart();
                };

                process.ErrorDataReceived += (s, e) =>
                {
                    ConsoleOutput += e.Data + "\n";
                    stopwatch.Restart();
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                while (!process.HasExited || stopwatch.ElapsedMilliseconds < 500) await Task.Delay(1);
            }

            ConsoleOutput += ConsoleOutput.EndsWith("\n") ? "" : "\n";
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

        public ICommand EnabledCommand { get; set; }
        void doNothing(object o) { }
        #endregion

        public MainViewModel(MetroWindow window)
        {
            dataRecievedHandler = new DataReceivedEventHandler((s, e) =>
            {
                ConsoleOutput += e.Data + "\n";
            });

            WindowActivated = new RelayCommand(windowActivated);
            WindowDeactivated = new RelayCommand(windowDeactivated);
            ClearConsoleCommand = new RelayCommand(clearConsole, _ => { return !string.IsNullOrWhiteSpace(ConsoleOutput); });
            OpenFolderCommand = new RelayCommand(openFolder);
            SettingsCommand = new RelayCommand(openSettingsWindow);

            ClearParametersCommand = new ListenCommand(Dispatcher.CurrentDispatcher)
            {
                Execute = clearParameters,
                CanExecuteDelegate = (r) => Parameters.Count > 0
            }.ListenOn(this, t => Parameters);

            BuildCommand = createExecutionCommand(buildCommand);
            BuildUploadCommand = createExecutionCommand(buildUploadCommand);
            CleanCommand = createExecutionCommand(cleanCommand);
            EnabledCommand = createExecutionCommand(doNothing);

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
            var command = new ListenCommand(Dispatcher.CurrentDispatcher)
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
