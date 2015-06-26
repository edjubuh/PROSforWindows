using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace PROSforWindows.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class UriToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if ((new Regex(@"^[A-z]:\\$").Matches((string)value).Count > 0)) return value;
            return ((string)value).Substring(((string)value).LastIndexOfAny(new char[] { '\\', '/' }) + 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
