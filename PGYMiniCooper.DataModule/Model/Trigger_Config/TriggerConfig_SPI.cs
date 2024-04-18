using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGYMiniCooper.DataModule.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PGYMiniCooper.DataModule.Model.Trigger_Config
{
    public class TriggerConfig_SPI : ObservableObject, ITriggerConfigViewModel
    {
        public string MOSIData { get { return mOSIData; } set { mOSIData = value; OnPropertyChanged("MOSIData"); } }

        private string mOSIData = "0x00";

        public string MISOData { get { return mISOData; } set { mISOData = value; OnPropertyChanged("MISOData"); } }

        private string mISOData = "0x00";

        public eComparisonList MOSIComparison { get { return mOSIComparison; } set { mOSIComparison = value; OnPropertyChanged("MOSIComparison"); } }

        private eComparisonList mOSIComparison = eComparisonList.Equal_to;

        public eComparisonList MISOComparison { get { return mISOComparison; } set { mISOComparison = value; OnPropertyChanged("MISOComparison"); } }

        private eComparisonList mISOComparison = eComparisonList.Equal_to;

        public ePatternFormat MOSIPattern { get { return mOSIPattern; } set { mOSIPattern = value; ChangText("spi_mosi", value); OnPropertyChanged("MOSIPattern"); } }

        private ePatternFormat mOSIPattern = ePatternFormat.Hex;

        public ePatternFormat MISOPattern { get { return mISOPattern; } set { mISOPattern = value; ChangText("spi_miso", value); OnPropertyChanged("MISOPattern"); } }

        private ePatternFormat mISOPattern = ePatternFormat.Hex;

        private bool isMOSIChecked = true;

        public bool IsMOSIChecked
        {
            get { return isMOSIChecked; } 
            set 
            { isMOSIChecked = value;
               // IsMISOChecked = false;
                OnPropertyChanged("IsMOSIChecked");
             
            } }

        private bool isMISOChecked = false;

        public bool IsMISOChecked { get 
            { return isMISOChecked; }
            set { isMISOChecked = value;
                //IsMOSIChecked = false;
                OnPropertyChanged("IsMISOChecked");

               
            } }

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
            ePatternFormat pattern = ePatternFormat.Hex;
            if (IsMOSIChecked)
            {
                if (MOSIData.Contains("0x"))
                    data =MOSIData.Replace("0x", "");
                else if (MOSIData.Contains("0X"))
                    data = MOSIData.Replace("0X", "");
                else
                    data = MOSIData;
                pattern = MOSIPattern;
            }
            else if (IsMISOChecked)
            {
                if (MISOData.Contains("0x"))
                    data = MISOData.Replace("0x", "");
                else if (MISOData.Contains("0X"))
                    data = MISOData.Replace("0X", "");
                else
                    data = MISOData;
                pattern = MISOPattern;
            }

            if (data != "" && data != null)
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
                        privateArray[0] = Convert.ToByte(intData);
                        privateArray[4] = 0;
                    }
                    if (strData.Count() > 1)
                    {
                        intData = 0;
                        if (int.TryParse(strData[1], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                        {
                            privateArray[1] = Convert.ToByte(intData);
                            privateArray[5] = 0;
                        }
                        if (strData.Count() > 2)
                        {
                            intData = 0;
                            if (int.TryParse(strData[2], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                            {
                                privateArray[2] = Convert.ToByte(intData);
                                privateArray[6] = 0;
                            }
                            if (strData.Count() > 3)
                            {
                                intData = 0;
                                if (int.TryParse(strData[3], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    privateArray[3] = Convert.ToByte(intData);
                                    privateArray[7] = 0;
                                }
                            }
                        }
                    }
                }
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

         
          if (type == "spi_mosi")
            {
                if (IsMOSIChecked)
                {
                    MOSIData = retval;
                }
            }
            else if (type == "spi_miso")
            {
                if (IsMISOChecked)
                {
                    MISOData = retval;
                }
            }
       
        }
    }
}
