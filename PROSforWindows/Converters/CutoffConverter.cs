﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace PROSforWindows.Converters
{
    [ValueConversion(typeof(double), typeof(bool))]
    public class CutoffConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
                return ((double)value) > double.Parse(parameter.ToString());
            else if (value is int)
                return ((int)value) > int.Parse(parameter.ToString());
            else return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
