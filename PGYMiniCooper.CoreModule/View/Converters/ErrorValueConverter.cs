using PGYMiniCooper.DataModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using ProtocolCoreModule;

namespace PGYMiniCooper.CoreModule.View.Converters
{
    public class ErrorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue)
            {
                return DependencyProperty.UnsetValue;
            }

            return EnumListToString((List<eErrorType>)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string EnumListToString(List<eErrorType> val)
        {
            string strarghex = "Pass";
            if (val.Count == 0)
                return strarghex;
            else
                return "Error";
            //for (int b = 0; b < val.Count; b++)
            //    strarghex += val[b].ToString() + ",";
            // return strarghex;
        }
    }
}
