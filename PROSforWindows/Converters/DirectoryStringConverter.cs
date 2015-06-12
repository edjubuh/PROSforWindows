using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PROSforWindows.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class DirectoryStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if (!(value is string)) throw new InvalidOperationException("Value must be of type string");

            if (!Directory.Exists((string)value)) return value;

            return Directory.CreateDirectory((string)value).Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
