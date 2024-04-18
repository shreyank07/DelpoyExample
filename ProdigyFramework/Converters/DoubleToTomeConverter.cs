using Prodigy.Business;
using Prodigy.Business.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ProdigyFramework.Converters
{
    public class DoubleToTimeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double strvalue = 0d;
            if (values.Count() == 2)
            {
                if (values[0] == DependencyProperty.UnsetValue || values[0] == null ||
                    values[1] == null || values[1] == DependencyProperty.UnsetValue)
                {
                    return "0.0000s";
                }

                double.TryParse(values[0].ToString(), out double time);
                double.TryParse(values[1].ToString(), out double sampleRate);

                strvalue = time / sampleRate;
            }
            else if (values.Count() == 3)
            {
                if (values[0] == DependencyProperty.UnsetValue || values[0] == null ||
                    values[1] == null || values[1] == DependencyProperty.UnsetValue ||
                    values[2] == null || values[2] == DependencyProperty.UnsetValue)
                {
                    return "0.0000s";
                }

                double.TryParse(values[0].ToString(), out double time);
                double.TryParse(values[1].ToString(), out double triggerTime);
                double.TryParse(values[2].ToString(), out double sampleRate);

                strvalue = (time - triggerTime) / sampleRate;
            }
            return strvalue.FormatNumber(Units.BASE_UNIT) + "s";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LongToTimeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double strvalue = 0d;
            if (values.Count() == 2)
            {
                if (values[0] == DependencyProperty.UnsetValue || values[0] == null ||
                    values[1] == null || values[1] == DependencyProperty.UnsetValue)
                {
                    return "0s";
                }

                double.TryParse(values[0].ToString(), out double time);
                double.TryParse(values[1].ToString(), out double sampleRate);

                strvalue = time / sampleRate;
            }
            else if (values.Count() == 3)
            {
                if (values[0] == DependencyProperty.UnsetValue || values[0] == null ||
                    values[1] == null || values[1] == DependencyProperty.UnsetValue ||
                    values[2] == null || values[2] == DependencyProperty.UnsetValue)
                {
                    return "0s";
                }

                double.TryParse(values[0].ToString(), out double time);
                double.TryParse(values[1].ToString(), out double triggerTime);
                double.TryParse(values[2].ToString(), out double sampleRate);

                strvalue = (time - triggerTime) / sampleRate;
            }
            return strvalue.ToPlotTimestampNotation();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
