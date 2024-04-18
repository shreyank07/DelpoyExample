using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ProdigyFramework.Converters
{
    public class CircularIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long index = 0;
            if (value == DependencyProperty.UnsetValue || value == null)
                return DependencyProperty.UnsetValue;

            long.TryParse(value.ToString(), out index);
            int circulateAt = 10000;
            //if (parameter != null && parameter != DependencyProperty.UnsetValue)
            //    int.TryParse(parameter.ToString(), out circulateAt);

            long retindex = index % circulateAt;

            return (index >= circulateAt) ? (".." + retindex.ToString()) : retindex.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
