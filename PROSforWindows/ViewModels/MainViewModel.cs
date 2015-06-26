using PROSforWindows.Commands;
using PROSforWindows.Converters;
using System;
using System.Collections.Generic;
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
        string _projectDirectory;
        public string ProjectDirectory
        {
            get { return _projectDirectory; }
            set
            {
                if (_projectDirectory != value)
                {
                    _projectDirectory = value;
                    OnPropertyChanged(nameof(ProjectDirectory));
                }
            }
        }

        string _consoleOutput = "hi";
        public string ConsoleOutput
        {
            get { return _consoleOutput; }
            set
            {
                if (_consoleOutput != value)
                {
                    _consoleOutput = value;
                    OnPropertyChanged(nameof(ConsoleOutput));
                }
            }
        }

        bool _openingFolder = false;
        public bool OpeningFolder
        {
            get { return _openingFolder; }
            set
            {
                if(_openingFolder != value)
                {
                    _openingFolder = value;
                    OnPropertyChanged(nameof(OpeningFolder));
                }
            }
        }

        public ICommand ClearConsoleCommand { get; set; }
        void clearConsole(object o) { ConsoleOutput = ""; }

        public ICommand OpenCommand { get; set; }
        void openCommand(object o)
        {
            OpeningFolder = !OpeningFolder;
        }

        public ICommand OpenFolderCommand { get; set; }
        void openFolderCommand(object o)
        {
            if (File.Exists((string)o + "\\firmware\\uniflash.jar"))
            {
                ProjectDirectory = (string)o;
                OpeningFolder = false;
                ConsoleOutput = "> Opened project: " + (string)o;
            }
        }

        public MainViewModel()
        {
            OpenCommand = new RelayCommand(openCommand);
            OpenFolderCommand = new RelayCommand(openFolderCommand);
            ClearConsoleCommand = new ListenRelayCommand(Dispatcher.CurrentDispatcher)
            {
                Execute = clearConsole,
                CanExecuteDelegate = (o) => !string.IsNullOrWhiteSpace(ConsoleOutput)
            }.ListenOn(this, t => t.ConsoleOutput);
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
