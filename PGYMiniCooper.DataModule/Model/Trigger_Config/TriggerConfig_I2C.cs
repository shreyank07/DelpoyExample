using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule;
using System;
using System.Collections.Generic;
using ProdigyFramework.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Interfaces;

namespace PGYMiniCooper.DataModule.Model.Trigger_Config
{
    public class TriggerConfig_I2C : ViewModelBase, ITriggerConfigViewModel
    {


        public TriggerConfig_I2C() { }



        public eI2CTriggerAtList I2CTriggerAtSelected { get { return i2CTriggerAtSelected; } set { i2CTriggerAtSelected = value; RaisePropertyChanged("I2CTriggerAtSelected"); } }

        private eI2CTriggerAtList i2CTriggerAtSelected = eI2CTriggerAtList.Start;


        private string addressValue = "0x00";
        public string AddressValue { get { return addressValue; } set { addressValue = value; RaisePropertyChanged("AddressValue"); } }

        public ePatternFormat AddressPattern { get { return addressPattern; } set { addressPattern = value; 
                                                                                                            ChangText("i2c_addr", value);
                                                                                                            RaisePropertyChanged("AddressPattern"); } }

        private ePatternFormat addressPattern = ePatternFormat.Hex;


        private eTransferType transferType = eTransferType.WR;

        public eTransferType TransferType { get { return transferType; } set { transferType = value; RaisePropertyChanged("TransferType"); } }


        public string DataValue { get { return dataValue; } set { dataValue = value; RaisePropertyChanged("DataValue"); } }

        private string dataValue = "0x00";

        public ePatternFormat DataPattern { get { return dataPattern; } set { dataPattern = value; 
                ChangText("i2c_data", value);
                RaisePropertyChanged("DataPattern"); } }

        private ePatternFormat dataPattern = ePatternFormat.Hex;

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

            if (type == "i2c_addr")
            {
                AddressValue = retval;
            }
            else if (type == "i2c_data")
            {
                DataValue = retval;
            }
         
        }

        public byte[] GetTriggerBytes()
        {
            #region Byte Formation - Representation
            /*
             *      _________________________________________________________________________________________________________________________________
             *      |       7      |        6       |       5       |       4       |       3       |       2       |       1       |       0       |
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |   Start      |     SR         |    Stop       |                                Address[6:2]                                   | [0]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |       Address[1:0]            |      RW       |    Ack        |                Data0 [7:4]                                    | [1]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |                          Data0[3:0]                           |   Data0 - T/P |                Data1[7:5]                     | [2]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |                          Data1 [4:0]                                          |   Data1 - T/P |          Reserved             | [3]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      
             *      */
            #endregion
            byte[] privateArray = new byte[8];
            //Note: By default set the fields as don't care. On fields present, map the data and mask it
            privateArray[4] = 0xFF;
            privateArray[5] = 0xFF;
            privateArray[6] = 0xFF;
            privateArray[7] = 0xFF;

            privateArray[0] = 0;
            #region Start
            if (I2CTriggerAtSelected == eI2CTriggerAtList.Start)
            {
                privateArray[0] = Convert.ToByte(0x80);
                privateArray[4] = Convert.ToByte(0x7F);
            }
            #endregion
            #region Repeated Start
            if (I2CTriggerAtSelected == eI2CTriggerAtList.Repeated_Start)
            {
                privateArray[0] = Convert.ToByte(0x40);
                privateArray[4] = Convert.ToByte(0xBF);
            }
            #endregion
            #region Stop
            if (I2CTriggerAtSelected == eI2CTriggerAtList.Stop)
            {
                privateArray[0] = Convert.ToByte(0x20);
                privateArray[4] = Convert.ToByte(0xDF);
            }
            #endregion
            #region Ack/Nack
            if (I2CTriggerAtSelected == eI2CTriggerAtList.Ack)
            {
                privateArray[1] = Convert.ToByte(0x0);
                privateArray[5] = Convert.ToByte(0xEF);
            }
            if (I2CTriggerAtSelected == eI2CTriggerAtList.Nack)
            {
                privateArray[1] = Convert.ToByte(0x10);
                privateArray[5] = Convert.ToByte(0xEF);
            }
            #endregion
            #region Slave Address
            if (I2CTriggerAtSelected == eI2CTriggerAtList.Address || I2CTriggerAtSelected == eI2CTriggerAtList.Address_Data)
            {
                if (AddressValue != "" && AddressValue != null)
                {
                    string temp_addSTr = "";
                    if (AddressValue.Contains("0x"))
                        temp_addSTr = AddressValue.Replace("0x", "");
                    else if (AddressValue.Contains("0X"))
                        temp_addSTr = AddressValue.Replace("0X", "");
                    else
                        temp_addSTr = AddressValue;
                    int address = 0;
                    if (AddressPattern == ePatternFormat.Decimal)
                        Int32.TryParse(temp_addSTr, out address);
                    else if (AddressPattern == ePatternFormat.Hex)
                        address = int.Parse(temp_addSTr, System.Globalization.NumberStyles.HexNumber);
                    else if (AddressPattern == ePatternFormat.Binary)
                        address = Convert.ToInt32(temp_addSTr, 2);
                    else if (AddressPattern == ePatternFormat.Octal)
                        address = Convert.ToInt32(temp_addSTr, 8);
                    ////privateArray[0] = Convert.ToByte((address << 1) | (msg.SlaveTransfer == eTransferType.WR ? 0 : 1));
                    //if (TransferType == eTransferType.Write)
                    //    privateArray[0] = Convert.ToByte((address << 1) | 0); // for write
                    //else
                    //    privateArray[0] = Convert.ToByte((address << 1) | 1); // for read
                    //privateArray[4] = 0;
                    ////privateArray[1] = Convert.ToByte((msg.SlaveAck == eAcknowledgeType.ACK ? 0 : 1) << 7);
                    //privateArray[1] = Convert.ToByte(0 << 7); // hardcoded for ack.
                    //privateArray[5] = 0x7F;
                    privateArray[0] = Convert.ToByte((address << 1) >> 3);
                    privateArray[4] = Convert.ToByte(0xE0);
                    if (TransferType == eTransferType.WR)
                        privateArray[1] = Convert.ToByte(((((address << 1) & 0x06) >> 1) << 6) | (0 << 5) | 0); // ack/nack yet to include.
                    else
                        privateArray[1] = Convert.ToByte(((((address << 1) & 0x06) >> 1) << 6) | (1 << 5) | 0); // ack/nack yet to include.
                    privateArray[5] = Convert.ToByte(0x1F);
                }
            }
            #endregion
            #region Data
            if (I2CTriggerAtSelected == eI2CTriggerAtList.Data || I2CTriggerAtSelected == eI2CTriggerAtList.Address_Data)
            {
                if (DataValue != "" && DataValue != null)
                {
                    string data = DataValue;
                    if (DataValue.Contains("0x"))
                        data = DataValue.Replace("0x", "");
                    else if (DataValue.Contains("0X"))
                        data = DataValue.Replace("0X", "");
                    int intData = 0;
                    if (DataPattern == ePatternFormat.Decimal)
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
                    else if (DataPattern == ePatternFormat.Binary)
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
                    else if (DataPattern == ePatternFormat.Octal)
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
                    var strData = data.Trim().Split('-').Take(2).ToList();
                    //foreach (var dat in strData)
                    //{
                    //    if (DataPattern == "Decimal")
                    //        Int32.TryParse(dat, out data);
                    //    else if (DataPattern == "Hex")
                    //        data = int.Parse(dat, System.Globalization.NumberStyles.HexNumber);
                    //}

                    if (strData.Count() > 0)
                    {
                        //Data 0
                        if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                        {
                            //privateArray[1] = Convert.ToByte(privateArray[1] | (intData >> 1));
                            //privateArray[5] = Convert.ToByte(privateArray[5] & 0x80);
                            //privateArray[2] = Convert.ToByte((intData & 0x1) << 7);
                            //privateArray[6] = 0x7F;
                            privateArray[1] = Convert.ToByte(privateArray[1] | (intData >> 4));
                            privateArray[5] = Convert.ToByte(privateArray[5] & 0xF0);
                            privateArray[2] = Convert.ToByte((intData & 0x0F) << 4); // ack/nack not added
                            privateArray[6] = 0x0F;
                        }

                        //Data 1
                        if (strData.Count() > 1)
                        {
                            intData = 0;
                            if (int.TryParse(strData[1], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                            {
                                //privateArray[2] = Convert.ToByte(privateArray[2] | (intData >> 2));
                                //privateArray[6] = Convert.ToByte(privateArray[6] & 0xC0);
                                //privateArray[3] = Convert.ToByte((intData & 0x3) << 6);
                                //privateArray[7] = 0x3F;
                                privateArray[2] = Convert.ToByte(privateArray[2] | (intData >> 5));
                                privateArray[6] = Convert.ToByte(privateArray[6] & 0xF8);
                                privateArray[3] = Convert.ToByte((intData & 0x1F) << 3);
                                privateArray[7] = 0x07;
                            }
                        }
                    }
                }
            }
            #endregion

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


    }
}
