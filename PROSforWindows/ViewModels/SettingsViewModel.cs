using Newtonsoft.Json;
using PROSforWindows.Models;
using PROSforWindows.Models.Software;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using PROSforWindows.Helpers;
using System.Windows.Input;
using PROSforWindows.Commands;
using System.Threading.Tasks;

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
        public ObservableCollection<UpdateSource> UpdateSources { get; set; }

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
            UpdateSources = new ObservableCollection<UpdateSource>();

            HyperlinkCommand = new RelayCommand(openHyperlink);

            FetchAllAvailableSoftware();
        }

        public async void FetchAllAvailableSoftware()
        {
            using (var client = new WebClient())
                foreach (string url in (IEnumerable<string>)App.Current.Properties["installSources"])
                    await FetchAvailableSoftware(url, client);
        }

        public async Task FetchAvailableSoftware(string url)
        {
            using (var client = new WebClient())
            {
                await FetchAvailableSoftware(url, client);
            }
        }

        public async Task FetchAvailableSoftware(string url, WebClient client)
        {
            var source = await Task.Factory.StartNew(() =>
                JsonConvert.DeserializeObject<UpdateSource>(client.DownloadString(url),
                            new JsonSerializerSettings()
                            {
                                Context = new System.Runtime.Serialization.StreamingContext(System.Runtime.Serialization.StreamingContextStates.Other, url)
                            }
                        )
                );
            foreach (var software in source.Software)
            {
                software.UpdateSource = source.Url;
                if (InstalledSoftware.Any(s => s.Key == software.Key.ToString()))
                    InstalledSoftware.Where(s => s.Key == software.Key.ToString()).ForEach(async (s) =>
                    {
                        if (s.AvailableUpdates == null) s.AvailableUpdates = new ObservableCollection<AvailableSoftware>();
                        s.AvailableUpdates.Add(await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<AvailableSoftware>(software.ToString())));
                    });
            }
        }

        public ICommand HyperlinkCommand { get; set; }
        void openHyperlink(object o)
        {
            System.Diagnostics.Process.Start(o as string);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
