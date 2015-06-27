using Newtonsoft.Json;
using PROSforWindows.Commands;
using PROSforWindows.Converters;
using PROSforWindows.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace PROSforWindows.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private static string executingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        Project _project = new Project();
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

        #region Open project members
        bool _openingFolder = false;
        public bool OpeningFolder
        {
            get { return _openingFolder; }
            set
            {
                if (_openingFolder != value)
                {
                    _openingFolder = value;
                    OnPropertyChanged(nameof(OpeningFolder));
                }
            }
        }

        public ICommand OpenCommand { get; set; }
        void openButtonCommand(object o)
        {
            OpeningFolder = !OpeningFolder;
        }

        public ICommand OpenFolderCommand { get; set; }
        void openFolderCommand(object o)
        {
            var path = (string)o;
            // The folder is a valid PROS project if there is the flashing utility or if there is a settings.json file
            if (File.Exists(path + "\\firmware\\uniflash.jar") || File.Exists(path + "\\settings.json"))
            {
                // Some clean-up operations that need to run if opening a project when one is already open
                Buttons.Clear();

                Project.DirectoryPath = path;
                // If there isn't a project settings file in the project, use the default one
                string projectSettingsPath = File.Exists(Project.DirectoryPath + "\\settings.json") ? (Project.DirectoryPath + "\\settings.json") : (executingDirectory + "\\Resources\\defaultsettings.json");
                var projectSettings = JsonConvert.DeserializeObject<ProjectSettings>(
                    (new StreamReader(projectSettingsPath).ReadToEnd()),
                    new JsonSerializerSettings()
                    { // Serializer settings and context in order to pass the Project in to each of the buttons
                        Context = new System.Runtime.Serialization.StreamingContext(System.Runtime.Serialization.StreamingContextStates.Other, Project)
                    });

                // Assigning project settings globally
                Project.Parameters = projectSettings.Parameters;
                foreach (var b in projectSettings.Buttons)
                    Buttons.Add(b);

                OpeningFolder = false;
                Project.Output = "> Opened project: " + Project.DirectoryPath;
            }
        }
        #endregion

        #region Buttons
        public ObservableCollection<Button> Buttons { get; set; } = new ObservableCollection<Button>();

        #endregion


        public ICommand ClearConsoleCommand { get; set; }
        void clearConsole(object o) { Project.Output = ""; }

        public MainViewModel()
        {
            OpenCommand = new RelayCommand(openButtonCommand);
            OpenFolderCommand = new RelayCommand(openFolderCommand);
            ClearConsoleCommand = new ListenRelayCommand(Dispatcher.CurrentDispatcher)
            {
                Execute = clearConsole,
                CanExecuteDelegate = (o) => !string.IsNullOrWhiteSpace(Project.Output)
            }.ListenOn(Project, t => t.Output);
        }

        #region INotifyPropertyChanged members
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        #endregion
    }
}
