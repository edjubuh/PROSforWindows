using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace PROSforWindows.Models.Software
{
    public class UpdateSource : INotifyPropertyChanged
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
        }

        string _url;
        public string Url
        {
            get { return _url; }
            set
            {
                if (_url != value)
                {
                    _url = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Url)));
                }
            }
        }

        public ObservableCollection<AvailableSoftware> Software { get; set; }

        [OnDeserialized]
        public void OnDeserialied(StreamingContext context)
        {
            Url = context.Context as string;
            foreach (var available in Software)
                available.UpdateName = Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
