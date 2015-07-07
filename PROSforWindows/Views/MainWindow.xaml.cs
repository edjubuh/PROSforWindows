using MahApps.Metro.Controls;
using PROSforWindows.ViewModels;

namespace PROSforWindows.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var context = new MainViewModel();
            context.ShowSettings += (s, e) =>
            {
                var window = new SettingsWindow(s);
                window.Owner = this;
                window.ShowDialog();
            };
            DataContext = context;
        }

        private void console_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            console.ScrollToEnd();
        }
    }
}
