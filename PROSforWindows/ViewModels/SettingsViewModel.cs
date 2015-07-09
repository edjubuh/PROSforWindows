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
using System.Windows;

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
            DownloadSoftwareCommand = new RelayCommand(downloadSoftware);
            CopyToClipboardCommand = new RelayCommand(copyToClipboard);

            FetchAllAvailableSoftware();
        }

        public async void FetchAllAvailableSoftware()
        {
            using (var client = new WebClient() { CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.BypassCache) })
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
            // Download the JSON file and deserialize it 
            var source = await Task.Factory.StartNew(() =>
                JsonConvert.DeserializeObject<UpdateSource>(client.DownloadString(url),
                            new JsonSerializerSettings()
                            {
                                Context = new System.Runtime.Serialization.StreamingContext(System.Runtime.Serialization.StreamingContextStates.Other, url)
                            }
                        )
                );
            List<AvailableSoftware> remove = new List<AvailableSoftware>();
            foreach (var available in source.Software)
            {
                available.UpdateSource = source.Url;
                if (InstalledSoftware.Any(i => i.Key == available.Key.ToString()))
                    InstalledSoftware.Where(i => i.Key == available.Key.ToString())
                        .ForEach((installed) =>
                    {
                        if (installed.AvailableUpdate == null ||
                            installed.AvailableUpdate.VersionInteger < available.VersionInteger)
                        {
                            installed.AvailableUpdate = available;
                        }

                        remove.Add(available);
                    });
            }

            foreach (var available in remove)
                source.Software.Remove(available);

            UpdateSources.Add(source);
        }

        public ICommand HyperlinkCommand { get; set; }
        void openHyperlink(object o)
        {
            System.Diagnostics.Process.Start(o as string);
        }

        public ICommand DownloadSoftwareCommand { get; set; }
        void downloadSoftware(object o)
        {

        }

        public ICommand CopyToClipboardCommand { get; set; }
        void copyToClipboard(object o)
        {
            if (o is string)
                Clipboard.SetText(o as string);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
