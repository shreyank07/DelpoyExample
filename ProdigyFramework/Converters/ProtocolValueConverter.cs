using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ProdigyFramework.Converters
{
    class ProtocolValueConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int intVal = -1;
            if (int.TryParse(value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out intVal))
            {
                //if(frameDesc.Value != intVal)
                //    frameDesc.Value = intVal;
                if (intVal != -1)
                {
                    return "0x" + intVal.ToString("X");
                }
                else
                    return "X";
            }
            else
            {
                //if (frameDesc.Value != intVal)
                //    frameDesc.Value = -1;
                return "X";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string stringValue = value.ToString();
            if (stringValue == "X")
                return -1;
            int intVal;
            if (int.TryParse(stringValue, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out intVal))
            {
                return intVal;
            }
            else
            {
                return -1;
            }
        }
    }
}
