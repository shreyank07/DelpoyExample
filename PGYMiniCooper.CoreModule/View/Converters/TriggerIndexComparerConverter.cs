using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PGYMiniCooper.CoreModule.View.Converters
{
    public class TriggerIndexComparerConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2)
                return DependencyProperty.UnsetValue;
            if (values[0] == null || values[1] == null || values[2] == null ||
                values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue || values[2] == DependencyProperty.UnsetValue)
                return DependencyProperty.UnsetValue;

            Enum.TryParse(values[2].ToString(), out eProtocol protocol);
            if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().TriggerType == eTriggerTypeList.Protocol)
            {
                if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.I2C && protocol == eProtocol.I2C)
                    return (values[0].Equals(values[1])) ? true : false;
                else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.SPI && protocol == eProtocol.SPI)
                    return (values[0].Equals(values[1])) ? true : false;
                else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.UART && protocol == eProtocol.UART)
                    return (values[0].Equals(values[1])) ? true : false;
                else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.I3C && protocol == eProtocol.I3C)
                    return (values[0].Equals(values[1])) ? true : false;
                else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.SPMI && protocol == eProtocol.SPMI)
                    return (values[0].Equals(values[1])) ? true : false;
                else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.RFFE && protocol == eProtocol.RFFE)
                    return (values[0].Equals(values[1])) ? true : false;
                else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.CAN && protocol == eProtocol.CAN)
                    return (values[0].Equals(values[1])) ? true : false;
                else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.QSPI && protocol == eProtocol.QSPI)
                    return (values[0].Equals(values[1])) ? true : false;
                else
                    return false;
            }
            return false;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
