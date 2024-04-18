using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ProdigyFramework.ComponentModel;
using PGYMiniCooper.DataModule.Interfaces;
using System.Configuration;
using System.Xml.Serialization;

namespace PGYMiniCooper.DataModule.Model.Trigger_Config
{
    public class TriggerConfig_CAN : ViewModelBase, ITriggerConfigViewModel

    {
        private readonly ConfigModel_CAN configModel;

        public TriggerConfig_CAN() { }
        public TriggerConfig_CAN(ConfigModel_CAN configModel) 
        {
            this.configModel = configModel;
        }

        #region can

        private string cANID;
        public string CANID
        {
            get
            {
                return cANID;
            }

            set
            {
                cANID = value;
                RaisePropertyChanged("CANID");
            }
        }

        private string cANEXID;
        public string CANEXID
        {
            get
            {
                return cANEXID;
            }

            set
            {
                cANEXID = value;
                RaisePropertyChanged("CANEXID");
            }
        }


        private string cANDLC;
        public string CANDLC
        {
            get
            {
                return cANDLC;
            }

            set
            {
                cANDLC = value;
                RaisePropertyChanged("CANDLC");
            }
        }


        private string _candata;
        public string CANData
        {
            get
            {
                return _candata;
            }
            set
            {
                _candata = value;
                RaisePropertyChanged("CANData");
            }
        }


        private string _cancrc;
        public string CANCRC
        {
            get
            {
                return _cancrc;
            }
            set
            {
                _cancrc = value;
                RaisePropertyChanged("CANCRC");
            }
        }
        #endregion
       


        public byte[] GetTriggerBytes()
        {

            #region Byte Formation - Representation
            /*
             *      _________________________________________________________________________________________________________________________________
             *      |       7      |        6       |       5       |       4       |       3       |       2       |       1       |       0       |
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |                          id[0-6]                                                                              |sof            | [0]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |dlc                                           |          id[7-10]                                                              | [1]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |      Data                                                                                                                     | [2]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |           Data                                                                                                                | [3]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      
             *      */
            #endregion
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
            privateArray[28] = 0xFF;
            privateArray[29] = 0xFF;
            privateArray[30] = 0xFF;
            privateArray[31] = 0xFF;



            string CANID1 = CANID;  
            if (CANID != "" && CANID != null)
            {
                if (CANID.Contains("0x"))
                    CANID1 = CANID.Replace("0x", "");
                else if (CANID.Contains("0X"))
                    CANID1 = CANID.Replace("0X", "");
            }

            string EXCANID1 = CANEXID;
            if (CANEXID != "" && CANEXID != null)
            {

                if (CANEXID.Contains("0x"))
                    EXCANID1 = CANEXID.Replace("0x", "");
                else if (CANEXID.Contains("0X"))
                    EXCANID1 = CANEXID.Replace("0X", "");
            }


            string DLC1 = CANDLC;

            if (CANDLC != "" && CANDLC != null)
            {
                if ( CANDLC.Contains("0x"))
                    DLC1 = CANDLC.Replace("0x", "");
                else if (CANDLC.Contains("0X"))
                    DLC1 = CANDLC.Replace("0X", "");
            }

            int canid = -1;
            if (!int.TryParse(CANID1, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out canid))
                canid = -1;
            int candlc = -1;
            if (!int.TryParse(DLC1, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out candlc))
                candlc = -1;
            int canEXID = -1;
            if (!int.TryParse(EXCANID1, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out canEXID))
                canEXID = -1;
            //int cancrc = -1;
            //if (!int.TryParse(triggerModel.CANCRC, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out cancrc))
            //    cancrc = -1;
            // byte CommandValue = ProtocolInfoRepository.GetRFFECmdId((byte)triggerModel.SelRFFECommand);
            //Note: By default set the fields as don't care. On fields present, map the data and mask it

            #region Slave Address

            //if (ConfigModel.GetInstance().CANFDPA || ConfigModel.GetInstance().CANFDPA && ConfigModel.GetInstance().StandardPA)
            //{
            //    // privateArray[0] = 1;
            if (canid != -1 && canEXID <= -1)   //1,1,1
            {
                privateArray[0] = Convert.ToByte(0x0);
                privateArray[16] = Convert.ToByte(0xFF);
                privateArray[1] = Convert.ToByte(0x0);
                privateArray[17] = Convert.ToByte(0xff);
                privateArray[2] = Convert.ToByte(canid >> 8);
                privateArray[18] = Convert.ToByte(0xF8);
                privateArray[3] = Convert.ToByte((canid & 255));
                privateArray[19] = Convert.ToByte(0x00);


            }
            else if (canid <= -1 && canEXID != -1)
            {
                privateArray[0] = Convert.ToByte(canEXID >> 12);
                privateArray[16] = Convert.ToByte(0xC0);
                privateArray[1] = Convert.ToByte((canEXID >> 4) & 255);
                privateArray[17] = Convert.ToByte(0x0);
                privateArray[2] = Convert.ToByte((canEXID & 15) << 4);
                privateArray[18] = Convert.ToByte(0xF);
                privateArray[3] = Convert.ToByte(0);
                privateArray[19] = Convert.ToByte(0xff);


            }
            else if (canid != -1 && canEXID != -1)
            {
                privateArray[0] = Convert.ToByte(canEXID >> 12);
                privateArray[16] = Convert.ToByte(0xC0);
                privateArray[1] = Convert.ToByte((canEXID >> 4) & 255);
                privateArray[17] = Convert.ToByte(0x0);
                privateArray[2] = Convert.ToByte(((canEXID & 15) << 4) | canid >> 8);
                privateArray[18] = Convert.ToByte(0x8);
                privateArray[3] = Convert.ToByte((canid & 255));
                privateArray[19] = Convert.ToByte(0x00);

            }
            else if (canid <= -1 && canEXID <= -1)
            {
                privateArray[0] = Convert.ToByte(0x0);
                privateArray[16] = Convert.ToByte(0xFF);
                privateArray[1] = Convert.ToByte(0X0);
                privateArray[17] = Convert.ToByte(0xFF);
                privateArray[2] = Convert.ToByte(0X0);
                privateArray[18] = Convert.ToByte(0xFF);
                privateArray[3] = Convert.ToByte(0x0);
                privateArray[19] = Convert.ToByte(0xff);
            }


            if (candlc <= -1)
            {
                privateArray[4] = Convert.ToByte(0x0);
                privateArray[20] = Convert.ToByte(0XFF);
            }
            else if (candlc != -1)
            {
                privateArray[4] = Convert.ToByte(candlc << 4);
                privateArray[20] = Convert.ToByte(0x0F);
            }





            #endregion


            #region Data
            if (CANData != null && CANData != "")
            {
                string data = CANData;
                if (CANData != "" && CANData != null)
                {
                    if (CANData.Contains("0x"))
                        data = CANData.Replace("0x", "");
                    else if (CANData.Contains("0X"))
                        data = CANData.Replace("0X", "");
                }
                var num = 0;
             
                if (configModel.CANType==eCANType.CAN)
                {
                    num = 8;
                }
                else
                {
                    num = 10;
                }
                var strData = data.Trim().Split('-').Take(num).ToList();
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


                    //Data5
                    if (strData.Count() > 5)
                    {
                        intData = 0;
                        if (int.TryParse(strData[5], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                        {

                            privateArray[10] = Convert.ToByte(intData);
                            privateArray[26] = 0x0;

                        }
                    }


                    //Data6
                    if (strData.Count() > 6)
                    {
                        intData = 0;
                        if (int.TryParse(strData[6], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                        {

                            privateArray[11] = Convert.ToByte(intData);
                            privateArray[27] = 0x0;

                        }
                    }



                    //Data7
                    if (strData.Count() > 7)
                    {
                        intData = 0;
                        if (int.TryParse(strData[7], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                        {

                            privateArray[12] = Convert.ToByte(intData);
                            privateArray[28] = 0x0;

                        }
                    }



                    //Data8
                    if (strData.Count() > 8)
                    {
                        intData = 0;
                        if (int.TryParse(strData[8], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                        {

                            privateArray[13] = Convert.ToByte(intData);
                            privateArray[29] = 0x0;

                        }
                    }


                    //Data9
                    if (strData.Count() > 9)
                    {
                        intData = 0;
                        if (int.TryParse(strData[9], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                        {

                            privateArray[14] = Convert.ToByte(intData);
                            privateArray[30] = 0x0;

                        }
                    }


                    //Data10
                    if (strData.Count() > 10)
                    {
                        intData = 0;
                        if (int.TryParse(strData[10], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                        {

                            privateArray[15] = Convert.ToByte(intData);
                            privateArray[31] = 0x0;

                        }
                    }

                    //Data11
                    if (strData.Count() > 11)
                    {
                        intData = 0;
                        if (int.TryParse(strData[11], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                        {

                            privateArray[13] = Convert.ToByte(intData);
                            privateArray[29] = 0x0;

                        }
                    }

                    //Data12
                    if (strData.Count() > 12)
                    {
                        intData = 0;
                        if (int.TryParse(strData[12], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                        {

                            privateArray[14] = Convert.ToByte(intData);
                            privateArray[30] = 0x0;

                        }
                    }


                    //Data13
                    if (strData.Count() > 13)
                    {
                        intData = 0;
                        if (int.TryParse(strData[13], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                        {

                            privateArray[15] = Convert.ToByte(intData);
                            privateArray[31] = 0x0;

                        }
                    }
                }

            }







            //}

            //else
            //    if(ConfigModel.GetInstance().CANPA ||ConfigModel.GetInstance().CANFDPA && ConfigModel.GetInstance().ExtendedPA)
            //{
            //    if (canid != -1)
            //    {
            //        privateArray[0] = Convert.ToByte(((canid >> 4) & 0xff));
            //        privateArray[16] = Convert.ToByte(128);

            //    }
            //    else if (canid <= -1)
            //    {
            //        privateArray[0] = Convert.ToByte(0);
            //        privateArray[16] = Convert.ToByte(0xff);

            //    }


            //}
            #endregion


            return privateArray;
        }

    }
}
