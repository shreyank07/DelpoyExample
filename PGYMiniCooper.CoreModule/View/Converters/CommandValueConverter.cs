using PGYMiniCooper.DataModule.Model;
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
    public class CommandValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string cmdType = "-";
            if (value == DependencyProperty.UnsetValue || value == null)
                return "-";
            byte byteVal = System.Convert.ToByte(value);
            cmdType = ProtocolInfoRepository.GetRFFECmdType(byteVal).ToString();
            return cmdType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
