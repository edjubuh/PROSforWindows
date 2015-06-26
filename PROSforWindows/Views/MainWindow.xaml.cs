using MahApps.Metro.Controls;
using PROSforWindows.ViewModels;
using System.Windows.Controls;

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
            DataContext = new MainViewModel(this);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            console.ScrollToEnd();
        }
    }
}
