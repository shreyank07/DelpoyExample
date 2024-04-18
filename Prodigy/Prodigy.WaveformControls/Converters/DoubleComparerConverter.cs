// <copyright file="DoubleComparerConverter.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>
  
using System;
using System.Windows;
using System.Windows.Data;

namespace Prodigy.WaveformControls.Converters
{
    public class DoubleComparerConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 3)
            {
                if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue || values[2] == DependencyProperty.UnsetValue)
                {
                    return DependencyProperty.UnsetValue;
                }

                if (System.Convert.ToDouble(values[1]) - System.Convert.ToDouble(values[0]) == System.Convert.ToDouble(values[2]))
                {
                    return Visibility.Collapsed;
                }
            }

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}