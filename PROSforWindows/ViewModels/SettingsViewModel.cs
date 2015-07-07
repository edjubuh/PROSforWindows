using PROSforWindows.Models;
using PROSforWindows.Models.Software;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
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

        public ObservableCollection<InstalledSoftware> InstalledSoftware { get; set; }
        public ObservableCollection<AvailableSoftware> AvailableSoftware { get; set; }

        public SettingsViewModel(object o)
        {
            // If o contains one Project property, set it as this project (we're expecting the fancy JSONSerializerSettings object)
            if (o.GetType().GetProperties().Where(p => p.PropertyType.Equals(typeof(Project))).Count() == 1)
                Project = (Project)(o.GetType().GetProperties().Where(p => p.PropertyType.Equals(typeof(Project))).ToArray()[0].GetValue(o));
            // Just in case
            else if (o is Project)
                Project = (Project)o;
            else throw new ArgumentException("Parameter must contain exactly one Project object");

            InstalledSoftware = new ObservableCollection<InstalledSoftware>((IEnumerable<InstalledSoftware>)App.Current.Properties["installed"]);
            using (var client = new WebClient())
            {
                foreach (string source in (IEnumerable<string>)App.Current.Properties["installSources"])
                {
                    var donwloaded = client.DownloadString(source);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
