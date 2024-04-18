using PGYMiniCooper.DataModule.Interfaces;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule;
using System;
using System.Collections.Generic;
using System.Globalization;
using ProdigyFramework.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Model.Trigger_Config
{
    public class TriggerConfig_QSPI :ViewModelBase,ITriggerConfigViewModel
    {

        #region qspi properties
        //QSPI commadlist
        private eQSPICommands selQSPICommand;
        public eQSPICommands SelQSPICommand
        {
            get
            {
                return selQSPICommand;
            }
            set
            {
                selQSPICommand = value;
                RaisePropertyChanged("SelQSPICommand");
            }
        }




        //QSPIAddressList

        private string qspiAddressByte;

        public string QSPIAddressByte
        {
            get
            {
                return qspiAddressByte;
            }
            set
            {
                qspiAddressByte = value;
                RaisePropertyChanged("QSPIAddressByte");
            }
        }



        //QSPI Data

        private string qSPIData;
        public string QSPIData
        {
            get
            {
                return qSPIData;
            }

            set
            {
                qSPIData = value;
                RaisePropertyChanged("QSPIData");
            }
        }
        #endregion
        public byte[] GetTriggerBytes()
        {
            byte[] privateArray = new byte[32];
            //Note: By default set the fields as don't care. On fields present, map the data and mask it

            privateArray[16] = 0xFF;
            privateArray[17] = 0xFF;
            privateArray[18] = 0xFF;
            privateArray[19] = 0xFF;
            privateArray[20] = 0xFF;
            privateArray[21] = 0xFF;
            privateArray[22] = 0xFF;
            privateArray[23] = 0xFF;
            privateArray[24] = 0xFF;
            privateArray[25] = 0xFF;
            privateArray[26] = 0xFF;
            privateArray[27] = 0xFF;
            privateArray[29] = 0xFF;
            privateArray[30] = 0xFF;
            privateArray[28] = 0xFF;
            privateArray[31] = 0xFF;


            //QSPi Command

            if (SelQSPICommand.ToString() == "Any_Command")
            {
                privateArray[0] = Convert.ToByte(0x00);
                privateArray[16] = Convert.ToByte(0xFF);
            }
            else
            {
                var cmd = ProtocolInfoRepository.QSPICommandDictionary.First(x => x.Value.QSPICommands == SelQSPICommand).Key;

                if ((int)SelQSPICommand != -1)
                {
                    privateArray[0] = Convert.ToByte(cmd);
                    privateArray[16] = Convert.ToByte(0x00);
                }
                else
                {
                    privateArray[0] = Convert.ToByte(0x00);
                    privateArray[16] = Convert.ToByte(0xFF);
                }
            }
            string QSPIAddressByteCount = QSPIAddressByte;

            if (!string.IsNullOrEmpty(QSPIAddressByte))
            {
                string addressByte = QSPIAddressByte.Trim();

                if (addressByte.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    QSPIAddressByteCount = addressByte.Substring(2);
                else if (addressByte.StartsWith("0X", StringComparison.OrdinalIgnoreCase))
                    QSPIAddressByteCount = addressByte.Substring(2);
            }

            int qspiAddress = -1;
            if (!int.TryParse(QSPIAddressByteCount, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out qspiAddress))
            {

                if (!int.TryParse(QSPIAddressByteCount, out qspiAddress))
                {
                    qspiAddress = -1;
                }
            }

            if (qspiAddress != -1)
            {
                privateArray[1] = Convert.ToByte((qspiAddress >> 24) & 0xFF);
                privateArray[17] = 0x0;

                privateArray[2] = Convert.ToByte((qspiAddress >> 16) & 0xFF);
                privateArray[18] = 0x0;

                privateArray[3] = Convert.ToByte((qspiAddress >> 8) & 0xFF);
                privateArray[19] = 0x0;

                privateArray[4] = Convert.ToByte(qspiAddress & 0xFF);
                privateArray[20] = 0x0;
            }
            else
            {
                privateArray[1] = 0x0;
                privateArray[17] = 0xFF;

                privateArray[2] = 0x0;
                privateArray[18] = 0xFF;

                privateArray[3] = 0x0;
                privateArray[19] = 0xFF;

                privateArray[4] = 0x0;
                privateArray[20] = 0xFF;
            }

            //QSPI Data




            if (QSPIData != null && QSPIData != "")
            {
                string data = QSPIData;
                if (QSPIData != "" && QSPIData != null)
                {
                    if (QSPIData.Contains("0x"))
                        data = QSPIData.Replace("0x", "");
                    else if (QSPIData.Contains("0X"))
                        data = QSPIData.Replace("0X", "");
                    else if (QSPIData.Contains("0x0"))
                        data = QSPIData.Replace("0x0", "");
                    else if (QSPIData.Contains("0X0"))
                        data = QSPIData.Replace("0X0", "");
                }

                var strData = data.Trim().Split('-').Take(10).ToList();
                int intData;
                if (strData.Count() > 0)
                {
                    //Data 0
                    if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                    {
                        privateArray[5] = Convert.ToByte(intData);
                        privateArray[21] = 0x0;
                    }

                    //Data 1
                    if (strData.Count() > 1)
                    {
                        intData = 0;
                        if (int.TryParse(strData[1], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                        {

                            privateArray[6] = Convert.ToByte(intData);
                            privateArray[22] = 0x0;

                        }
                    }

                    //Data2
                    if (strData.Count() > 2)
                    {
                        intData = 0;
                        if (int.TryParse(strData[2], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                        {

                            privateArray[7] = Convert.ToByte(intData);
                            privateArray[23] = 0x0;

                        }
                    }

                    //Data3
                    if (strData.Count() > 3)
                    {
                        intData = 0;
                        if (int.TryParse(strData[3], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                        {

                            privateArray[8] = Convert.ToByte(intData);
                            privateArray[24] = 0x0;

                        }
                    }

                    //Data4
                    if (strData.Count() > 4)
                    {
                        intData = 0;
                        if (int.TryParse(strData[4], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                        {

                            privateArray[9] = Convert.ToByte(intData);
                            privateArray[25] = 0x0;
                        }
                    }
                }

                privateArray[10] = 0;
                privateArray[11] = 0;
                privateArray[12] = 0;
                privateArray[13] = 0;
                privateArray[14] = 0;
                privateArray[15] = 0;

                privateArray[26] = 0xFF;
                privateArray[27] = 0xFF;
                privateArray[28] = 0xFF;
                privateArray[29] = 0xFF;
                privateArray[30] = 0xFF;
                privateArray[31] = 0xFF;

            }
            return privateArray;
        }

    }
}
