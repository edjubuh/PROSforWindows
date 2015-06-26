using MahApps.Metro.Controls;
using PROSforWindows.ViewModels;
using System.Windows;

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
            DataContext = new MainViewModel();
        }

        private void console_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            console.ScrollToEnd();
        }
    }
}
