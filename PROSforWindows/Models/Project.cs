using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROSforWindows.Models
{
    public class Project : INotifyPropertyChanged
    {
        string _output = "";
        public string Output
        {
            get { return _output; }
            set
            {
                if (_output != value)
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

        Dictionary<string, string> _parameters = new Dictionary<string, string>();
        public Dictionary<string, string> Parameters
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
