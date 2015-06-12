using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PROSforWindows.Converters
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string)) throw new InvalidOperationException("Value is not a string");
            return string.IsNullOrEmpty((string)value) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
