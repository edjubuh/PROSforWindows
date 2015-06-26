using PROSforWindows.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public ICommand ClearConsoleCommand { get; set; }
        void clearConsole(object o) { ConsoleOutput = ""; }

        public ICommand OpenCommand { get; set; }
        void openCommand(object o) { ConsoleOutput = "hi"; }

        public MainViewModel()
        {
            OpenCommand = new RelayCommand(openCommand);
            //ClearConsoleCommand = new RelayCommand(clearConsole, (o) => { return !string.IsNullOrWhiteSpace(ConsoleOutput); });
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
