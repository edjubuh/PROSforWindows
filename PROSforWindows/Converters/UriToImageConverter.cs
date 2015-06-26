using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PROSforWindows.Converters
{
    public class UriToImageConverter : IValueConverter
    {
        static Regex DriveLetter = new Regex(@"^[A-z]:\\$");
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (!(value is string) || !(File.Exists((string)value) || Directory.Exists(((string)value)))) throw new ArgumentException();

            var path = (string)value;
            if (DriveLetter.IsMatch(path)) // is a logical drive
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/img/HDD-26.png"));
            }
            if (Directory.Exists(path)) // is a directory
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/img/Folder-26.png"));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
