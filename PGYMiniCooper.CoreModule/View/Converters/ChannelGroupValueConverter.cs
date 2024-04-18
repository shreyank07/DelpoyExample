using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PGYMiniCooper.CoreModule.View.Converters
{
    public class ChannelGroupValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }
            if (parameter == null)
            {
                return DependencyProperty.UnsetValue;
            }
            // to change here.
            UInt16 retVal = 0;
            string retStr = "0x";
            //UInt16 val = (UInt16)value;
            //ConfigModel config = ConfigModel.GetInstance();
            //int i = 0;
            //int numofChannels = 0;
            //switch (parameter.ToString())
            //{
            //    case "C1G1":
            //        for (i = (config.SelectedCLK1_GRP1.Count()-1); i >= 0;i--)
            //        {
            //            retVal = (UInt16)(retVal << 1);
            //            retVal |= GetChannelByte(val, config.SelectedCLK1_GRP1[i]);
            //        }
            //        //foreach (var chn in ConfigModel.GetInstance().SelectedCLK1_GRP1)
            //        //{
            //        //    retVal |= GetChannelByte(val, chn);
            //        //}
            //        numofChannels = config.SelectedCLK1_GRP1.Count;
            //        break;
            //    case "C1G2":
            //        for (i = (config.SelectedCLK1_GRP2.Count() - 1); i >= 0; i--)
            //        {
            //            retVal = (UInt16)(retVal << 1);
            //            retVal |= GetChannelByte(val, config.SelectedCLK1_GRP2[i]);
            //        }
            //        //foreach (var chn in ConfigModel.GetInstance().SelectedCLK1_GRP2)
            //        //{
            //        //    retVal |= GetChannelByte(val, chn);
            //        //}
            //        numofChannels = config.SelectedCLK1_GRP2.Count;
            //        break;
            //    case "C1G3":
            //        for (i = (config.SelectedCLK1_GRP3.Count() - 1); i >= 0; i--)
            //        {
            //            retVal = (UInt16)(retVal << 1);
            //            retVal |= GetChannelByte(val, config.SelectedCLK1_GRP3[i]);
            //        }
            //        numofChannels = config.SelectedCLK1_GRP3.Count;
            //        //foreach (var chn in ConfigModel.GetInstance().SelectedCLK1_GRP3)
            //        //{
            //        //    retVal |= GetChannelByte(val, chn);
            //        //}
            //        break;
            //    case "C2G1":
            //        for (i = (config.SelectedCLK2_GRP1.Count() - 1); i >= 0; i--)
            //        {
            //            retVal = (UInt16)(retVal << 1);
            //            retVal |= GetChannelByte(val, config.SelectedCLK2_GRP1[i]);
            //        }
            //        numofChannels = config.SelectedCLK2_GRP1.Count;
            //        //foreach (var chn in ConfigModel.GetInstance().SelectedCLK2_GRP1)
            //        //{
            //        //    retVal |= GetChannelByte(val, chn);
            //        //}
            //        break;
            //    case "C2G2":
            //        for (i = (config.SelectedCLK2_GRP2.Count() - 1); i >= 0; i--)
            //        {
            //            retVal = (UInt16)(retVal << 1);
            //            retVal |= GetChannelByte(val, config.SelectedCLK2_GRP2[i]);
            //        }
            //        numofChannels = config.SelectedCLK2_GRP2.Count;
            //        //foreach (var chn in ConfigModel.GetInstance().SelectedCLK2_GRP2)
            //        //{
            //        //    retVal |= GetChannelByte(val, chn);
            //        //}
            //        break;
            //    case "C2G3":
            //        for (i = (config.SelectedCLK2_GRP3.Count() - 1); i >= 0; i--)
            //        {
            //            retVal = (UInt16)(retVal << 1);
            //            retVal |= GetChannelByte(val, config.SelectedCLK2_GRP3[i]);
            //        }
            //        numofChannels = config.SelectedCLK2_GRP3.Count;
            //        //foreach (var chn in ConfigModel.GetInstance().SelectedCLK2_GRP3)
            //        //{
            //        //    retVal |= GetChannelByte(val, chn);
            //        //}
            //        break;
            //    default:
            //        retVal = 0;
            //        break;
            //}
            //// retStr += retVal.ToString("X2");
            //if (config.DataFormat == eDataFormat.Hex)
            //    retStr += retVal.ToString("X2");
            //else if (config.DataFormat == eDataFormat.Binary)
            //    retStr = System.Convert.ToString(retVal, 2).PadLeft(numofChannels, '0');
            //else if (config.DataFormat == eDataFormat.Octal)
            //    retStr = System.Convert.ToString(retVal, 8);
            //else if (config.DataFormat == eDataFormat.ASCII)
            //{
            //    //byte[] bytes = BitConverter.GetBytes(retVal);
            //    //string text = System.Text.Encoding.ASCII.GetString(bytes);
            //    //retStr = text;
            //     retStr = System.Convert.ToChar(retVal).ToString();
            //}
            //else if (config.DataFormat == eDataFormat.Decimal)
            //    retStr = retVal.ToString();
            //else if (config.DataFormat == eDataFormat.Gray_Code)
            //    retStr = System.Convert.ToString((retVal >> 1) ^ retVal);
            //else if (config.DataFormat == eDataFormat._2s_Complement)
            //    retStr = ProdigyFramework.Helpers.CustomExtensions.findTwoscomplement(new StringBuilder(System.Convert.ToString(retVal, 2)));
            //else
            //    retStr = retVal.ToString();
            return retStr;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private UInt16 GetChannelByte(UInt16 val, eChannles chan)
        {
            UInt16 retVal = 0;
            switch (chan)
            {
                case eChannles.CH1:
                    retVal = (UInt16)(val & 0x1);
                    break;
                case eChannles.CH2:
                    retVal = (UInt16)((val & 0x2) >> 1);
                    break;
                case eChannles.CH3:
                    retVal = (UInt16)((val & 0x4) >> 2);
                    break;
                case eChannles.CH4:
                    retVal = (UInt16)((val & 0x8) >> 3);
                    break;
                case eChannles.CH5:
                    retVal = (UInt16)((val & 0x10) >> 4);
                    break;
                case eChannles.CH6:
                    retVal = (UInt16)((val & 0x20) >> 5);
                    break;
                case eChannles.CH7:
                    retVal = (UInt16)((val & 0x40) >> 6);
                    break;
                case eChannles.CH8:
                    retVal = (UInt16)((val & 0x80) >> 7);
                    break;
                case eChannles.CH9:
                    retVal = (UInt16)((val & 0x100) >> 8);
                    break;
                case eChannles.CH10:
                    retVal = (UInt16)((val & 0x200) >> 9);
                    break;
                case eChannles.CH11:
                    retVal = (UInt16)((val & 0x400) >> 10);
                    break;
                case eChannles.CH12:
                    retVal = (UInt16)((val & 0x800) >> 11);
                    break;
                case eChannles.CH13:
                    retVal = (UInt16)((val & 0x1000) >> 12);
                    break;
                case eChannles.CH14:
                    retVal = (UInt16)((val & 0x2000) >> 13);
                    break;
                case eChannles.CH15:
                    retVal = (UInt16)((val & 0x4000) >> 14);
                    break;
                case eChannles.CH16:
                    retVal = (UInt16)((val & 0x8000) >> 15);
                    break;
                default:
                    break;
            }
            return retVal;
        }
    }
}
