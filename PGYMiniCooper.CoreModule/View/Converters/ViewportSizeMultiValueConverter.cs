using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PGYMiniCooper.CoreModule.View.Converters
{
    public class ViewportSizeMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // min
            // max
            // visible start
            // visible end
            // track length

            if (values == null || values.Length < 5)
                return DependencyProperty.UnsetValue;

            if ((values[0] is double) == false)
                return DependencyProperty.UnsetValue;
                        
            double minValue = (double)values[0];
            double maxValue = (double)values[1];
            double visibleStartValue = (double)values[2];
            double visibleEndValue = (double)values[3];
            double trackLength = (double)values[4];

            if (trackLength == 0) return DependencyProperty.UnsetValue;

            minValue = Math.Min(minValue, visibleStartValue);

            double thumbSizePercentage = 100d * (visibleEndValue - visibleStartValue) / (maxValue - minValue);
            double thumbSize = trackLength * thumbSizePercentage;


            return (thumbSize * (maxValue - minValue)) / (trackLength + thumbSize);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
