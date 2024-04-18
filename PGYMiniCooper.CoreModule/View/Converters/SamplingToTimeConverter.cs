using Prodigy.Business.Extensions;
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
    public class SamplingToTimeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Minium 3 values are taken
            // values[0] - sample index / double
            // values[1] - trigger index / double
            // values[2] - sample rate / double

            if (values.Length < 3) return DependencyProperty.UnsetValue;

            if (double.TryParse(values[0].ToString(), out double sampleIndex) && double.TryParse(values[1].ToString(), out double triggerIndex) && double.TryParse(values[2].ToString(), out double sampleRate))
            {
                return ((sampleIndex - triggerIndex) / sampleRate).FormatTime(Prodigy.Business.Units.SECONDS);
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
