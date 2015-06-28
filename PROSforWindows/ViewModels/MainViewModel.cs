using Newtonsoft.Json;
using PROSforWindows.Commands;
using PROSforWindows.Models;
using PROSforWindows.Resources;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace PROSforWindows.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
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

        #region New project members
        ICommand _newProjectCommand;
        public ICommand NewProjectCommand
        {
            get { return _newProjectCommand; }
            set
            {
                if (_newProjectCommand != value)
                {
                    _newProjectCommand = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewProjectCommand)));
                }
            }
        }
        void newProject(object o)
        {
            DialogService.GetCurrentWindow().ShowMessageAsync("Hello world!", "I'm working!");
        }

        #endregion

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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OpeningFolder)));
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
                string projectSettingsPath = File.Exists(Project.DirectoryPath + "\\settings.json") ? (Project.DirectoryPath + "\\settings.json") : (Project.executingDirectory + "\\Resources\\defaultsettings.json");
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

        #region Settings button members
        public event EventHandler ShowSettings;

        public ICommand SettingsCommand { get; set; }
        void showSettings(object o)
        {
            ShowSettings?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Buttons
        public ObservableCollection<Button> Buttons { get; set; } = new ObservableCollection<Button>();
        #endregion

        #region Console members
        public ICommand ClearConsoleCommand { get; set; }
        void clearConsole(object o) { Project.Output = ""; }
        #endregion

        public MainViewModel()
        {
            NewProjectCommand = new RelayCommand(newProject);
            OpenCommand = new RelayCommand(openButtonCommand);
            OpenFolderCommand = new RelayCommand(openFolderCommand);
            SettingsCommand = new RelayCommand(showSettings);
            ClearConsoleCommand = new ListenRelayCommand(Dispatcher.CurrentDispatcher)
            {
                Execute = clearConsole,
                CanExecuteDelegate = (o) => !string.IsNullOrWhiteSpace(Project.Output)
            }.ListenOn(Project, t => t.Output);
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
