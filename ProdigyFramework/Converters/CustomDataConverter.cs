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
    public class CustomDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue || value == null)
                return "-";
            var packetArr = (IList<byte>)value;
            if (packetArr.Count() == 0)
                return "-";
            else
            {
                // return String.Join(" ", packetArr.Select(obj=> String.Format("0x{0:X}", obj.Value)));
                StringBuilder sb = new StringBuilder();
                foreach (var p in packetArr)
                {
                    sb.Append(String.Format("0x{0:X}", p) + " ");
                }
                return sb.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
