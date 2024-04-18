using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace PGYMiniCooper.CoreModule.View.Converters
{

    public class SlaveMIDConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string cmdType = "-";
            if (value == DependencyProperty.UnsetValue || value == null || value.ToString() == "-1")
                return "-";
            byte byteVal = System.Convert.ToByte(value);
            cmdType = "0x" + byteVal.ToString("X");
            return cmdType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
