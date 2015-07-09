using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace PROSforWindows.Converters
{
    public class ItemIsLastConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2) return false;

            return ((IList)values.First(o => o is IList)).IndexOf(values.First(o => !(o is IList))) == ((IList)values.First(o => o is IList)).Count-1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
