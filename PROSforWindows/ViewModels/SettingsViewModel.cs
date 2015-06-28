using PROSforWindows.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace PROSforWindows.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
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

        public string Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public SettingsViewModel(object o)
        {
            var foo = o.GetType().GetProperties().Where(p => p.PropertyType.Equals(typeof(Project))).ToArray();
            // If o contains one Project property, set it as this project
            if (o.GetType().GetProperties().Where(p => p.PropertyType.Equals(typeof(Project))).Count() == 1)
                Project = (Project)(o.GetType().GetProperties().Where(p => p.PropertyType.Equals(typeof(Project))).ToArray()[0].GetValue(o));
            else throw new ArgumentException("Parameter must contain exactly one Project object");
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
