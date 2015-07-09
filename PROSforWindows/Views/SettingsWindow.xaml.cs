using MahApps.Metro.Controls;
using PROSforWindows.ViewModels;
using System.Windows.Controls;

namespace PROSforWindows.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public SettingsWindow(object o) : this()
        {
            DataContext = new SettingsViewModel(o);
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}
