using PGYMiniCooper.DataModule.Interfaces;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PGYMiniCooper.DataModule.Model.Trigger_Config
{
    public class TriggerConfig_UART : ObservableObject, ITriggerConfigViewModel
    {
        private bool uARTAutoDetectSelected = false;
        public bool UARTAutoDetectSelected { get { return uARTAutoDetectSelected; } set { uARTAutoDetectSelected = value; OnPropertyChanged("UARTAutoDetectSelected"); } }

        private bool uARTStartFallingSelected = false;
        public bool UARTStartFallingSelected { get { return uARTStartFallingSelected; } set { uARTStartFallingSelected = value; OnPropertyChanged("UARTStartFallingSelected"); } }

        private bool uARTStartRisingSelected = true;
        public bool UARTStartRisingSelected { get { return uARTStartRisingSelected; } set { uARTStartRisingSelected = value; OnPropertyChanged("UARTStartRisingSelected"); } }

        public eComparisonList UARTDataComparison { get { return uARTDataComparison; } set { uARTDataComparison = value; OnPropertyChanged("UARTDataComparison"); } }

        private eComparisonList uARTDataComparison = eComparisonList.Equal_to;

        public ePatternFormat UARTDataPattern { get { return uARTDataPattern; } set { uARTDataPattern = value; ChangText("uart_data", value); OnPropertyChanged("UARTDataPattern"); } }

        private ePatternFormat uARTDataPattern = ePatternFormat.Hex;

        public bool IsUARTDataChecked { get { return isUARTDataChecked; } set { isUARTDataChecked = value; OnPropertyChanged("IsUARTDataChecked"); } }

        private bool isUARTDataChecked = false;


        public string UARTDataValue { get{ return uARTDataValue; } set { uARTDataValue = value; OnPropertyChanged("UARTDataValue"); } }

        private string uARTDataValue = "0x00";
        public TriggerConfig_UART() { }

        public byte[] GetTriggerBytes()
        {
            byte[] privateArray = new byte[8];
            //Note: By default set the fields as don't care. On fields present, map the data and mask it
            privateArray[4] = 0xFF;
            privateArray[5] = 0xFF;
            privateArray[6] = 0xFF;
            privateArray[7] = 0xFF;


            privateArray[0] = 0x0;
            privateArray[1] = 0x0;
            privateArray[2] = 0x0;
            privateArray[3] = 0x0;

            string data = "";
            ePatternFormat pattern = UARTDataPattern;
            data = UARTDataValue;

            if (data.Contains("0x"))
                data = data.Replace("0x", "");
            else if (data.Contains("0X"))
                data = data.Replace("0X", "");

            if (data != "" && data != null && IsUARTDataChecked)
            {
                int intData = 0;
                if (pattern == ePatternFormat.Decimal)
                {
                    int tempData;
                    List<string> strdt = new List<string>();
                    if (data.Trim().Split('-').Length > 1)
                        strdt = data.Trim().Split('-').Take(2).ToList();
                    else
                        strdt = data.Trim().Split('-').ToList();
                    data = "";
                    foreach (var dat in strdt)
                    {
                        Int32.TryParse(dat, out tempData);
                        if (data == "")
                            data = tempData.ToString("X");
                        else
                            data = data + "-" + tempData.ToString("X");
                    }
                }
                else if (pattern == ePatternFormat.Binary)
                {
                    List<string> strdt = new List<string>();
                    if (data.Trim().Split('-').Length > 1)
                        strdt = data.Trim().Split('-').Take(2).ToList();
                    else
                        strdt = data.Trim().Split('-').ToList();
                    data = "";
                    foreach (var dat in strdt)
                    {
                        if (data == "")
                            data = BinaryStringToHexString(dat);
                        else
                            data = data + "-" + BinaryStringToHexString(dat);
                    }
                }
                else if (pattern == ePatternFormat.Octal)
                {
                    List<string> strdt = new List<string>();
                    if (data.Trim().Split('-').Length > 1)
                        strdt = data.Trim().Split('-').Take(2).ToList();
                    else
                        strdt = data.Trim().Split('-').ToList();
                    data = "";
                    int tempInt;
                    foreach (var dat in strdt)
                    {
                        tempInt = Convert.ToInt32(dat, 8);
                        if (data == "")
                            data = tempInt.ToString("X");
                        else
                            data = data + "-" + tempInt.ToString("X");
                    }
                }
                List<string> strData = new List<string>();
                if (data.Trim().Split('-').Length > 1)
                {
                    if (data.Trim().Split('-').Length > 2)
                    {
                        if (data.Trim().Split('-').Length > 3)
                            strData = data.Trim().Split('-').Take(4).ToList();
                        else
                            strData = data.Trim().Split('-').Take(3).ToList();
                    }
                    else
                        strData = data.Trim().Split('-').Take(2).ToList();
                }
                else
                    strData = data.Trim().Split('-').ToList();
                if (strData.Count() > 0)
                {
                    //Data 0
                    if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                    {
                       

                        privateArray[2] = Convert.ToByte((intData & 0x80) >> 7);
                        privateArray[6] = 0xFE;

                        privateArray[3] = Convert.ToByte((intData & 0x7F) << 1);
                        privateArray[7] = 0x01;
                    }
                
                }
            }

       

            if (UARTStartRisingSelected)
            {
                privateArray[3] = Convert.ToByte(privateArray[3] | 0);
                privateArray[7] = Convert.ToByte(privateArray[7] & 0xFE);
            }
            else
            {
                privateArray[3] = Convert.ToByte(privateArray[3] | 1);
                privateArray[7] = Convert.ToByte(privateArray[7] & 0xFE);
            }
            return privateArray;
        }


        public string BinaryStringToHexString(string binary)
        {
            if (string.IsNullOrEmpty(binary))
                return binary;

            StringBuilder result = new StringBuilder(binary.Length / 8 + 1);

            // TODO: check all 1's or 0's... throw otherwise

            int mod4Len = binary.Length % 8;
            if (mod4Len != 0)
            {
                // pad to length multiple of 8
                binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');
            }

            for (int i = 0; i < binary.Length; i += 8)
            {
                string eightBits = binary.Substring(i, 8);
                result.AppendFormat("{0:X2}", Convert.ToByte(eightBits, 2));
            }

            return result.ToString();
        }

        void ChangText(string type, ePatternFormat format)
        {
            string retval = "0x00";
            if (format == ePatternFormat.Binary && type == "pattern")
                retval = "XXXXXXXXXXXXXXX";
            else if (format == ePatternFormat.Octal)
                retval = "00";
            else if (format == ePatternFormat.Decimal)
                retval = "00";
            else if (format == ePatternFormat.Hex)
                retval = "0x00";
            else
                retval = "00000000";

           if (type == "uart_data")
            {
                UARTDataValue = retval;
            }
        }
    }
}
