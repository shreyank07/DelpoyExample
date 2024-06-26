﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ProdigyFramework.Converters
{
    /// <summary>
    /// Converts the boolean to Visibility and back
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value
        /// is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visible = Visibility.Collapsed;
            if (parameter != null)
            {
                if (Enum.IsDefined(typeof(Visibility), parameter) == true)
                {
                    visible = (Visibility)Enum.Parse(typeof(Visibility), parameter.ToString());
                }
            }
            return (bool)value ? Visibility.Visible : visible;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value
        /// is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Visibility)value == Visibility.Visible) ? true : false;
        }
    }
}
