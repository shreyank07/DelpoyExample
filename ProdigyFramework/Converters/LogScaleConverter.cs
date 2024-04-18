using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ProdigyFramework.Converters
{
   public class LogScaleConverter : IValueConverter
    {
        private double val { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = (string)value;
            val = System.Convert.ToDouble(s);
            return System.Convert.ToInt64(Math.Exp(val));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

