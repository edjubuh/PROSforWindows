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
using System.Linq;
using System.Collections.Generic;
using PROSforWindows.Properties;
using System.Collections.Specialized;

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
        bool _creatingProject;
        public bool CreatingProject
        {
            get { return _creatingProject; }
            set
            {
                if (_creatingProject != value)
                {
                    _creatingProject = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CreatingProject)));
                }
            }
        }

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
            CreatingProject = true;
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

        public StringCollection RecentFolders
        {
            get { return Settings.Default.RecentFolders; }
        }

        public ICommand OpenCommand { get; set; }
        void openButtonCommand(object o)
        {
            OpeningFolder = !OpeningFolder;
        }

        public ICommand OpenFolderCommand { get; set; }
        void openFolder(object o)
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

                if (Settings.Default.RecentFolders == null) Settings.Default.RecentFolders = new StringCollection();
                if(!Settings.Default.RecentFolders.Contains(Project.DirectoryPath)) Settings.Default.RecentFolders.Add(Project.DirectoryPath);
                Settings.Default.Save();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RecentFolders)));

                OpeningFolder = false;
                Project.Output = "> Opened project: " + Project.DirectoryPath;
            }
        }

        public ICommand RemoveRecentFolderCommand { get; set; }
        void removeRecentFolder(object o)
        {
            if (Settings.Default.RecentFolders.Contains(o as string))
                Settings.Default.RecentFolders.Remove(o as string);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RecentFolders)));
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

        #region Parameters button members
        public ICommand ClearParametersCommand { get; set; }
        void clearParameters(object o)
        {
            Project.Parameters.Clear();
        }

        public ICommand SaveParametersCommand { get; set; }
        void saveParameters(object o)
        {

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
            OpenFolderCommand = new RelayCommand(openFolder);
            RemoveRecentFolderCommand = new RelayCommand(removeRecentFolder);

            SettingsCommand = new RelayCommand(showSettings);

            ClearConsoleCommand = new ListenRelayCommand(Dispatcher.CurrentDispatcher)
            {
                Execute = clearConsole,
                CanExecuteDelegate = (o) => !string.IsNullOrWhiteSpace(Project.Output)
            }.ListenOn(Project, t => t.Output);

            ClearParametersCommand = new ListenRelayCommand(Dispatcher.CurrentDispatcher)
            {
                Execute = clearParameters,
                CanExecuteDelegate = (o) => Project.Parameters.Count > 0
            }.ListenOn(Project, p => p.Parameters);

            SaveParametersCommand = new ListenRelayCommand(Dispatcher.CurrentDispatcher)
            {
                Execute = saveParameters,
                CanExecuteDelegate = (o) => Project.Parameters.Count > 0
            }.ListenOn(Project, p => p.Parameters);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
