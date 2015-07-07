using System.ComponentModel;
using System.IO;

namespace PROSforWindows.Models
{
    public class Project : INotifyPropertyChanged
    {
        public static string executingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        string _output = "";
        public string Output
        {
            get { return _output; }
            set
            {
                if (_output != value && 
                    !(value == "\n\n\n" && _output == ""))
                {
                    _output = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Output)));
                }
            }
        }

        string _directoryPath = "";
        public string DirectoryPath
        {
            get { return _directoryPath; }
            set
            {
                if (_directoryPath != value)
                {
                    _directoryPath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DirectoryPath)));
                }
            }
        }

        ObservableDictionary<string, string> _parameters = new ObservableDictionary<string, string>();
        public ObservableDictionary<string, string> Parameters
        {
            get { return _parameters; }
            set
            {
                if (_parameters != value)
                {
                    _parameters = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Parameters)));
                }
            }
        }

        bool _isExecuting = false;
        public bool IsExecuting
        {
            get { return _isExecuting; }
            set
            {
                if (_isExecuting != value)
                {
                    _isExecuting = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExecuting)));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
