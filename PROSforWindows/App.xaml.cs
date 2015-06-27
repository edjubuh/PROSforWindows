using MahApps.Metro;
using PROSforWindows.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PROSforWindows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ThemeManager.AddAccent("PurdueAccent", new Uri("pack://application:,,,/Resources/PurdueAccent.xaml"));

            Application.Current.MainWindow.DataContext = new MainViewModel();

            base.OnStartup(e);
        }
    }
}
