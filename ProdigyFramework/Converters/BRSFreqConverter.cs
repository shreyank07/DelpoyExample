using ProdigyFramework.Helpers;
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
    public class BRSFreqConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
    
            int precision = 0;
            if (values[0] == null || values[0] == DependencyProperty.UnsetValue
                 || values[1] == null || values[1] == DependencyProperty.UnsetValue)
              
            return "0";



            if (parameter == null || parameter == DependencyProperty.UnsetValue)
                precision = 0;

            else if (!int.TryParse(parameter.ToString(), out precision))
                precision = 0;

            double strvalue = System.Convert.ToDouble(values[0]);
            double strvalue1 = System.Convert.ToDouble(values[1]);
            var T1 = (strvalue == 0 ? "-" : CUtilities.FormatNumber(strvalue, Units.BASE_UNIT)).Replace("M","");
            
            var T2= (strvalue1 == 0 ? "-" : CUtilities.FormatNumber(strvalue1, Units.BASE_UNIT) + "Hz");
            return (T1+"/"+ T2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
