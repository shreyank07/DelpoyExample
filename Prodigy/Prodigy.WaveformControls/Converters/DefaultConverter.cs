// <copyright file="DefaultConverter.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>
  
using System;
using System.Linq;
using System.Windows.Data;

namespace Prodigy.WaveformControls.Converters
{
    public class DefaultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}