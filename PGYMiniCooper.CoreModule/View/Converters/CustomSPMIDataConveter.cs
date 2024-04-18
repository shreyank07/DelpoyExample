using PGYMiniCooper.DataModule.Structure.RFFEStructure;
using PGYMiniCooper.DataModule.Structure.SPMIStructure;
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
    [ValueConversion(typeof(SPMIPacketStructure[]), typeof(string))]
    public class CustomSPMIDataConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue || value == null)
                return "-";
            var packetArr = value as SPMIPacketStructure[];
            if (packetArr.Count() == 0)
                return -1;
            else
            {
                // return String.Join(" ", packetArr.Select(obj=> String.Format("0x{0:X}", obj.Value)));
                StringBuilder sb = new StringBuilder();
                foreach (var p in packetArr)
                {
                    if (!CheckOddParity(p.Value << 8 | p.Parity) && p.Parity != 2) //Parity=2 exceptional for checking Register 0 write
                        sb.Append(String.Format("0x{0:X}", p.Value) + " (F)" + " ");
                    else
                        sb.Append(String.Format("0x{0:X}", p.Value) + " ");
                }
                return sb.ToString();
            }
        }

        public bool CheckOddParity(int data)
        {
            int count = 0, p;
            while (data != 0)
            {
                count++;
                data &= (data - 1);
            }

            if (count % 2 != 0)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(RFFEPacketStructure[]), typeof(string))]
    public class CustomRFFEDataConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue || value == null)
                return "-";
            var packetArr = value as RFFEPacketStructure[];
            if (packetArr.Count() == 0)
                return -1;
            else
            {
                // return String.Join(" ", packetArr.Select(obj=> String.Format("0x{0:X}", obj.Value)));
                StringBuilder sb = new StringBuilder();
                foreach (var p in packetArr)
                {
                    if (!CheckOddParity(p.Value << 8 | p.Parity) && p.Parity != 2) //Parity=2 exceptional for checking Register 0 write
                        sb.Append(String.Format("0x{0:X}", p.Value) + " (F)" + " ");
                    else
                        sb.Append(String.Format("0x{0:X}", p.Value) + " ");
                }
                return sb.ToString();
            }
        }

        public bool CheckOddParity(int data)
        {
            int count = 0, p;
            while (data != 0)
            {
                count++;
                data &= (data - 1);
            }

            if (count % 2 != 0)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
