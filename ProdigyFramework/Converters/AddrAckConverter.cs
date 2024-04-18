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
    public class AddrAckConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue || value == null)
                return "";
            if ((System.Convert.ToInt32(parameter) == 0x01))
                return System.Convert.ToInt32(value) >> 1;
            else if ((System.Convert.ToInt32(parameter) == 0x02))
                return System.Convert.ToInt32(value) & 0x01;
            else
            {
                if ((System.Convert.ToInt32(value) & 0x0F) == 0x0F)
                    return "NACK";
                else
                    return "ACK";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AddressConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue || values[0] == null
               || values[1] == DependencyProperty.UnsetValue || values[1] == null)
                return DependencyProperty.UnsetValue;
            if (System.Convert.ToInt32(values[1]) == 0)
            {
                //7b Address
                var firstaddr = System.Convert.ToInt32(values[0]);
                firstaddr = firstaddr >> 1;
                return firstaddr;
            }
            else
            {
                //10b Address
                var firstaddr = System.Convert.ToInt32(values[0]);
                firstaddr = firstaddr >> 1;
                firstaddr = firstaddr & 0x3;
                var secondAddr = System.Convert.ToInt32(values[1]);
                return (firstaddr << 8 | secondAddr);

            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
