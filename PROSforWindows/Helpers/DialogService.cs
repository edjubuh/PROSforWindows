using System.Windows;
using System.Linq;
using System.Linq.Expressions;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;

namespace PROSforWindows.Resources
{
    // Based off of http://stackoverflow.com/questions/22083213/mahapps-messageboxes-using-mvvm
    public class DialogService
    {
        public static MetroWindow GetCurrentWindow()
        {
            var candidates = Application.Current.Windows.Cast<Window>().Where(w => w.IsActive && w is MetroWindow);
            if (candidates.Count() == 0)
                return Application.Current.MainWindow as MetroWindow;
            else
                return candidates.ToArray()[0] as MetroWindow;
        }
    }
}
