using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PROSforWindows.Controls
{
    /// <summary>
    /// Interaction logic for FolderPicker.xaml
    /// </summary>
    public partial class FolderPicker : UserControl, INotifyPropertyChanged
    {
        public string SelectButtonText
        {
            get { return (string)GetValue(SelectButtonTextProperty); }
            set { SetValue(SelectButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectButtonTextProperty =
            DependencyProperty.Register("SelectButtonText", typeof(string), typeof(FolderPicker), new PropertyMetadata("Select"));

        public ObservableCollection<Folder> Items { get; set; } = new ObservableCollection<Folder>();

        public event PropertyChangedEventHandler PropertyChanged;

        public FolderPicker()
        {
            InitializeComponent();
            DataContext = this;
            foreach (string path in Directory.GetLogicalDrives())
                Items.Add((new Folder(path)).LoadChildren(null, new RoutedEventArgs()));
            if (Items.Count == 1)
                Items[0].IsExpanded = true;
        }
    }

    public class Folder : INotifyPropertyChanged
    {
        string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                if (_path != value)
                {
                    _path = value;
                    OnPropertyChanged(nameof(Path));
                }
            }
        }

        bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                    if (IsExpanded)
                    {
                        IList<Folder> remove = null;
                        foreach (var f in Subfolders)
                        {
                            try
                            {
                                f.LoadChildren(this, new RoutedEventArgs());
                            }
                            catch (UnauthorizedAccessException)
                            {
                                if (remove == null) remove = new List<Folder>();
                                remove.Add(f);
                            }
                        }
                        if (remove != null) foreach (var f in remove) Subfolders.Remove(f);
                    }
                }
            }
        }

        public ObservableCollection<Folder> Subfolders { get; set; } = new ObservableCollection<Folder>();

        public Folder(string path)
        {
            Path = path;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Folder LoadChildren(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var s in Directory.GetDirectories(Path))
                    Subfolders.Add(new Folder(s));
            }
            catch (UnauthorizedAccessException) { throw; }
            return this;
        }
    }
}
