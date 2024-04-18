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
    public class TriggerConfig_SPMI : ViewModelBase, ITriggerConfigViewModel
    {
        public TriggerConfig_SPMI() { }
        
        private readonly ConfigModel_SPMI configModel;
        public TriggerConfig_SPMI(ConfigModel_SPMI configModel)
        {
            this.configModel = configModel;
        }

        #region SPMI properties

        private eSPMICMDTYPE selSPMICommand;
        public eSPMICMDTYPE SelSPMICommand
        {
            get
            {
                return selSPMICommand;
            }
            set
            {
                selSPMICommand = value;
                RaisePropertyChanged("SelSPMICommand");
            }
        }

        private string SPMIslaveAddress;
        public string SPMISlaveAddress
        {
            get
            {
                return SPMIslaveAddress;
            }

            set
            {
                SPMIslaveAddress = value;
                RaisePropertyChanged("SPMISlaveAddress");
            }
        }

        private string sPMIBytecount;
        public string SPMIBytecount
        {
            get
            {
                return sPMIBytecount;
            }

            set
            {
                sPMIBytecount = value;
                RaisePropertyChanged("SPMIBytecount");
            }
        }

        private string sPMIregAddr;
        public string SPMIregAddr
        {
            get
            {
                return sPMIregAddr;
            }

            set
            {
                sPMIregAddr = value;
                RaisePropertyChanged("SPMIregAddr");
            }
        }

        private string _SPMIdata;
        public string SPMIData
        {
            get
            {
                return _SPMIdata;
            }
            set
            {
                _SPMIdata = value;
                RaisePropertyChanged("SPMIData");
            }
        }

        private eAcknowledgeType SPMIslaveAck;
        public eAcknowledgeType SPMISlaveAck
        {
            get
            {
                return SPMIslaveAck;
            }

            set
            {
                SPMIslaveAck = value;
                RaisePropertyChanged("SPMISlaveAck");
            }
        }
        #endregion
        public byte[] GetTriggerBytes()
        {
            byte[] privateArray = new byte[8];
            //Note: By default set the fields as don't care. On fields present, map the data and mask it
            privateArray[4] = 0xFF;
            privateArray[5] = 0xFF;
            privateArray[6] = 0xFF;
            privateArray[7] = 0xFF;

            int slaveAddr = -1;
            if (!int.TryParse(SPMISlaveAddress, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out slaveAddr))
                slaveAddr = -1;

            byte CommandValue = ProtocolInfoRepository.GetSPMICmdId((byte)SelSPMICommand);
            #region Slave Address
            if (SelSPMICommand != eSPMICMDTYPE.TRFR_BUS_OWNERSHIP)
            {
                if (SelSPMICommand == eSPMICMDTYPE.DDB_MA_R
                    || SelSPMICommand == eSPMICMDTYPE.MASTER_WRITE || SelSPMICommand == eSPMICMDTYPE.MASTER_READ)
                {
                    if (slaveAddr != -1)
                    {
                        //T only on MID
                        privateArray[0] = Convert.ToByte((slaveAddr & 0x3) << 4);
                        privateArray[4] = 0x0F;
                    }
                    else
                    {
                        privateArray[0] = Convert.ToByte((0x0 & 0x3) << 4);
                        privateArray[4] = 0xFF;
                    }
                }
                else
                {
                    if (slaveAddr != -1)
                    {
                        //T only on SlaveID
                        privateArray[0] = Convert.ToByte((slaveAddr & 0xF) << 4);
                        privateArray[4] = 0x0F;
                    }
                    else
                    {
                        privateArray[0] = Convert.ToByte((0x0 & 0xF) << 4);
                        privateArray[4] = 0xFF;
                    }
                }
            }

            #endregion
            int bytecount = -1;
            if (!int.TryParse(SPMIBytecount, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out bytecount))
                bytecount = -1;
            int regAddr = -1;
            if (!int.TryParse(SPMIregAddr, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out regAddr))
                regAddr = -1;
            switch ((int)SelSPMICommand)
            {
                case (int)eSPMICMDTYPE.EXT_REG_WRITE:
                    {
                        #region Command
                        if ((int)SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | ((CommandValue & 0xF0) >> 4));

                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                        }
                        #endregion

                        #region CommandBytecount
                        if ((int)SelSPMICommand != -1 && bytecount != -1)
                        {
                            privateArray[1] = Convert.ToByte((bytecount & 0x0F) << 4 | 0x0);
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if (regAddr != -1 && (int)SelSPMICommand != -1)
                        {
                            privateArray[1] = Convert.ToByte((privateArray[1]) | (regAddr >> 5));
                            privateArray[2] = Convert.ToByte((regAddr & 0x1F) << 3 | 0x0);
                            if (bytecount != -1)
                                privateArray[5] = Convert.ToByte(0x08);//Parity masked
                            else
                                privateArray[5] = Convert.ToByte(0xF8);//Parity masked
                            privateArray[6] = 0x7;
                        }
                        #endregion

                        #region Data
                        if (SPMIData != null && SPMIData != "" && (int)SelSPMICommand != -1)
                        {
                            var strData = SPMIData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    privateArray[2] = Convert.ToByte(privateArray[2] | (intData >> 6));
                                    privateArray[3] = Convert.ToByte((intData << 2) | 0x0);
                                    if (regAddr != -1)
                                        privateArray[6] = Convert.ToByte(0x04);
                                    else
                                        privateArray[6] = Convert.ToByte(0xFC);
                                    privateArray[7] = 0x03;
                                }
                            }
                        }
                        #endregion

                        break;
                    }

                case (int)eSPMICMDTYPE.EXT_REG_READ:
                    {
                        #region Command
                        // T on Command only
                        if ((int)SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | ((CommandValue & 0xF0) >> 4));

                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                        }
                        #endregion

                        #region CommandBytecount
                        if ((int)SelSPMICommand != -1 && bytecount != -1)
                        {
                            privateArray[1] = Convert.ToByte((bytecount & 0x0F) << 4 | 0x0);//Include parity bit
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if (regAddr != -1 && (int)SelSPMICommand != -1)
                        {
                            privateArray[1] = Convert.ToByte((privateArray[1]) | (regAddr >> 5));
                            privateArray[2] = Convert.ToByte((regAddr & 0x1F) << 3 | 0x0 << 2 | 0x1 << 1);
                            if (bytecount != -1)
                                privateArray[5] = 0x08;
                            else
                                privateArray[5] = 0xF8;
                            privateArray[6] = 0x07;
                        }
                        #endregion

                        #region Data
                        if (SPMIData != null && SPMIData != "" && (int)SelSPMICommand != -1)
                        {
                            var strData = SPMIData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    privateArray[2] = Convert.ToByte(privateArray[2] | (intData >> 7));
                                    privateArray[3] = Convert.ToByte((intData << 1) | 0x0);
                                    if (regAddr != -1)
                                        privateArray[6] = 0x06;
                                    else
                                        privateArray[6] = 0xFE;
                                    privateArray[7] = 0x01;

                                }
                            }
                        }
                        #endregion
                        break;
                    }

                case (int)eSPMICMDTYPE.EXT_REG_WRITE_LONG:
                    {
                        #region Command
                        if ((int)SelSPMICommand != -1) //bc 3 bit
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | (CommandValue & 0xF0) >> 4);
                            privateArray[1] = Convert.ToByte(((CommandValue & 0x08) << 4) | 0x0);//Include parity bit
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;

                            privateArray[5] = 0x7F;
                        }
                        #endregion

                        #region CommandBytecount
                        if ((int)SelSPMICommand != -1 && bytecount != -1)
                        {
                            privateArray[1] = Convert.ToByte(0x0 << 7 | (bytecount & 0x07) << 4 | 0x0);
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if (regAddr != -1 && (int)SelSPMICommand != -1)
                        {
                            privateArray[1] = Convert.ToByte((privateArray[1]) | (regAddr >> 13));
                            privateArray[2] = Convert.ToByte((((regAddr >> 8) & 0x1F) << 3) | ((regAddr & 0xC0) >> 6));
                            privateArray[3] = Convert.ToByte(((regAddr & 0x3F) << 2) | 0x0);
                            if (bytecount != -1)
                                privateArray[5] = 0x08;
                            else
                                privateArray[5] = 0x78;
                            privateArray[6] = 0x04;
                            privateArray[7] = 0x03;
                        }
                        #endregion
                        break;
                    }
                case (int)eSPMICMDTYPE.EXT_REG_READ_LONG:
                    {

                        #region Command
                        if ((int)SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | (CommandValue & 0xF0) >> 4);
                            privateArray[1] = Convert.ToByte(((CommandValue & 0x08) << 4) | 0x0);//Include parity bit
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                            privateArray[5] = 0x7F;
                        }
                        #endregion

                        #region CommandBytecount
                        if ((int)SelSPMICommand != -1 && bytecount != -1)
                        {
                            privateArray[1] = Convert.ToByte(0x1 << 7 | (bytecount & 0x07) << 4 | 0x0);//Include parity bit
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if (regAddr != -1 && (int)SelSPMICommand != -1)
                        {
                            privateArray[1] = Convert.ToByte((privateArray[1]) | (regAddr >> 13));
                            privateArray[2] = Convert.ToByte((((regAddr >> 8) & 0x1F) << 3) | ((regAddr & 0xC0) >> 6));
                            privateArray[3] = Convert.ToByte(((regAddr & 0x3F) << 2) | 0x0);
                            if (bytecount != -1)
                                privateArray[5] = 0x08;
                            else
                                privateArray[5] = 0x78;
                            privateArray[6] = 0x04;
                            privateArray[7] = 0x03;
                        }
                        #endregion
                        break;
                    }
                case (int)eSPMICMDTYPE.REG_WRITE:
                    {
                        #region Command
                        if ((int)SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | ((CommandValue & 0xE0) >> 4));
                            if (slaveAddr != -1)
                                privateArray[4] = 0x01;
                            else
                                privateArray[4] = 0xF1;
                        }
                        #endregion

                        #region CmdReg
                        //write
                        //if ((int)triggerModel.SelSPMICommand != -1 && regAddr != -1)
                        //{
                        //    privateArray[1] = Convert.ToByte(privateArray[1] | 0x0);
                        //    privateArray[5] = Convert.ToByte(0x0F);
                        //}
                        #endregion
                        #region address
                        if (regAddr != -1 && (int)SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte(privateArray[0] | ((regAddr & 0x10) >> 4));
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                            privateArray[1] = Convert.ToByte((regAddr & 0x0F) << 4);
                            privateArray[5] = 0x0F;
                        }
                        #endregion
                        #region Data
                        if (SPMIData != null && SPMIData != "" && (int)SelSPMICommand != -1)
                        {
                            var strData = SPMIData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    if (SPMIData != null && SPMIData != "")
                                    {
                                        privateArray[1] = Convert.ToByte(privateArray[1] | intData >> 5);
                                        privateArray[2] = Convert.ToByte((intData & 0x1F) << 3);
                                        if (regAddr != -1)
                                            privateArray[5] = 0x08;
                                        else
                                            privateArray[5] = 0xF8;
                                        privateArray[6] = 0x07;
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Ack/Nack
                        if ((int)SelSPMICommand != -1)
                        {
                            var ack = (SPMISlaveAck == eAcknowledgeType.ACK ? 1 : 0);
                            privateArray[2] = Convert.ToByte(privateArray[2] | ack);
                            if (SPMISlaveAck == eAcknowledgeType.NA)
                                privateArray[6] = Convert.ToByte(privateArray[6] & 0xFF); // ignoring ack
                            else
                                privateArray[6] = Convert.ToByte(privateArray[6] & 0xFE);
                        }
                        #endregion
                        break;
                    }
                case (int)eSPMICMDTYPE.REG_READ:
                    {
                        #region Command
                        if ((int)SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | ((CommandValue & 0xE0) >> 4));
                            if (slaveAddr != -1)
                                privateArray[4] = 0x01;
                            else
                                privateArray[4] = 0xF1;
                        }
                        #endregion

                        #region CmdReg
                        //read
                        //if ((int)triggerModel.SelSPMICommand != -1 && regAddr != -1)
                        //{
                        //    privateArray[0] = Convert.ToByte((privateArray[0]) | (regAddr >> 7));
                        //    privateArray[1] = Convert.ToByte(((regAddr & 0x0F) << 4) | 0x0 << 3 | 0x1 << 2);
                        //    privateArray[4] = 0x0;
                        //    privateArray[5] = Convert.ToByte(0x0F);
                        //}
                        #endregion
                        #region address
                        if (regAddr != -1 && (int)SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte(privateArray[0] | ((regAddr & 0x10) >> 4));
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                            privateArray[1] = Convert.ToByte((regAddr & 0x0F) << 4);
                            privateArray[5] = 0x0F;
                        }
                        #endregion
                        #region Data
                        if (SPMIData != null && SPMIData != "" && (int)SelSPMICommand != -1)
                        {
                            var strData = SPMIData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    if (SPMIData != null && SPMIData != "")
                                    {
                                        privateArray[1] = Convert.ToByte(privateArray[1] | intData >> 6);
                                        privateArray[2] = Convert.ToByte(((intData & 0x3F) << 2) | 0x0);
                                        if (regAddr != -1)
                                            privateArray[5] = 0x0C;
                                        else
                                            privateArray[5] = 0xFC;
                                        privateArray[6] = 0x03;
                                    }
                                }
                            }
                        }
                        #endregion

                        break;
                    }
                case (int)eSPMICMDTYPE.REG_ZERO_WRITE:
                    {
                        #region Command
                        if ((int)SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | ((CommandValue & 0x80) >> 4));
                            if (slaveAddr != -1)
                                privateArray[4] = 0x07;
                            else
                                privateArray[4] = 0xF7;
                        }
                        #endregion

                        #region CmdData
                        if ((int)SelSPMICommand != -1 && SPMIData != null && SPMIData != "")
                        {
                            var strData = SPMIData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    if ((int)SelSPMICommand != -1 && SPMIData != null && SPMIData != "")
                                    {
                                        privateArray[0] = Convert.ToByte((privateArray[0]) | (intData >> 4 & 0x07));
                                        if (slaveAddr != -1)
                                            privateArray[4] = 0x00;
                                        else
                                            privateArray[4] = 0xF0;
                                        privateArray[1] = Convert.ToByte((intData & 0x0F) << 4 | 0x0);
                                        privateArray[5] = 0x0F;
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Ack/Nack
                        if ((int)SelSPMICommand != -1)
                        {
                            var ack = (SPMISlaveAck == eAcknowledgeType.ACK ? 1 : 0);
                            privateArray[1] = Convert.ToByte(privateArray[1] | (ack << 1));
                            if (SPMISlaveAck == eAcknowledgeType.NA)
                                privateArray[5] = Convert.ToByte(privateArray[5] & 0xFF); // ignoring ack.
                            else
                                privateArray[5] = Convert.ToByte(privateArray[5] & 0xFD);
                        }
                        #endregion
                        break;
                    }


                case (int)eSPMICMDTYPE.TRFR_BUS_OWNERSHIP:
                    {
                        #region Command
                        if ((int)SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | (CommandValue & 0xF0) >> 4);
                            privateArray[1] = Convert.ToByte((CommandValue & 0x0F) << 4 | 0x02);//Include parity bit and ack
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        break;
                    }



                case (int)eSPMICMDTYPE.MASTER_WRITE:
                    {
                        #region Command
                        if ((int)SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | ((CommandValue & 0xF0) >> 4));
                            privateArray[1] = Convert.ToByte(((CommandValue & 0x0F) << 4) | 0x0);
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0; // for MID
                            else
                                privateArray[4] = 0xF0;
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if (regAddr != -1 && (int)SelSPMICommand != -1)
                        {
                            privateArray[1] = Convert.ToByte((privateArray[1]) | (regAddr >> 5));
                            privateArray[2] = Convert.ToByte(((regAddr & 0x1F) << 3) | 0x0);
                            privateArray[5] = 0x08;
                            privateArray[6] = 0x07;
                        }
                        #endregion

                        #region Data
                        if (SPMIData != null && SPMIData != "" && (int)SelSPMICommand != -1)
                        {
                            var strData = SPMIData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    privateArray[2] = Convert.ToByte(privateArray[2] | (intData >> 6));
                                    privateArray[3] = Convert.ToByte((intData << 2) | 0x0);
                                    if (regAddr != -1)
                                        privateArray[6] = 0x04;
                                    else
                                        privateArray[6] = 0xFC;
                                    privateArray[7] = 0x03;

                                }
                            }
                        }
                        #endregion
                        break;
                    }
                case (int)eSPMICMDTYPE.MASTER_READ:
                    {

                        #region Command
                        if ((int)SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | (CommandValue & 0xF0) >> 4);
                            privateArray[1] = Convert.ToByte((CommandValue & 0x0F) << 4 | 0x0);//Include parity bit
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0; // for MID
                            else
                                privateArray[4] = 0xF0;
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if (regAddr != -1 && (int)SelSPMICommand != -1)
                        {
                            privateArray[1] = Convert.ToByte((privateArray[1]) | (regAddr >> 5));
                            privateArray[2] = Convert.ToByte((regAddr & 0x1F) << 3 | 0x0 << 2 | 0x1 << 1);
                            privateArray[5] = 0x08;
                            privateArray[6] = 0x07;
                        }
                        #endregion

                        #region Data
                        if (SPMIData != null && SPMIData != "" && (int)SelSPMICommand != -1)
                        {
                            var strData = SPMIData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    privateArray[2] = Convert.ToByte(privateArray[2] | (intData >> 7));
                                    privateArray[3] = Convert.ToByte((intData << 1) | 0x0);
                                    if (regAddr != -1)
                                        privateArray[6] = 0x06;
                                    else
                                        privateArray[6] = 0xFE;
                                    privateArray[7] = 0x01;

                                }
                            }
                        }
                        #endregion
                        break;
                    }
                case (int)eSPMICMDTYPE.AUTHENTICATE:
                    {
                        #region Command
                        if ((int)SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | (CommandValue & 0xF0) >> 4);
                            privateArray[1] = Convert.ToByte((CommandValue & 0x0F) << 4 | 0x02);//Include parity bit and ack
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                            privateArray[5] = 0x0F;
                        }
                        #endregion
                        break;
                    }
                case (int)eSPMICMDTYPE.RESET:
                    {
                        #region Command
                        if ((int)SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | (CommandValue & 0xF0) >> 4);
                            privateArray[1] = Convert.ToByte((CommandValue & 0x0F) << 4 | 0x0);//Include parity bit and ack
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                            privateArray[5] = 0x0F;
                        }
                        #endregion
                        #region Ack/Nack
                        var ack = (SPMISlaveAck == eAcknowledgeType.ACK) ? 1 : 0;
                        privateArray[1] = Convert.ToByte(privateArray[1] | (ack << 1) | 0x0);
                        if (configModel.SPMIVersion == eVersion.one || SPMISlaveAck == eAcknowledgeType.NA)
                            privateArray[5] = 0x0F; // ignoring ack for version 1.0.
                        else
                            privateArray[5] = 0x0D;
                        #endregion
                        break;
                    }
                case (int)eSPMICMDTYPE.SLEEP:
                    {
                        #region Command
                        if ((int)SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | (CommandValue & 0xF0) >> 4);
                            privateArray[1] = Convert.ToByte((CommandValue & 0x0F) << 4 | 0x0);//Include parity bit and ack
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                            privateArray[5] = 0x0F;
                        }
                        #endregion
                        #region Ack/Nack
                        var ack = (SPMISlaveAck == eAcknowledgeType.ACK) ? 1 : 0;
                        privateArray[1] = Convert.ToByte(privateArray[1] | (ack << 1) | 0x0);
                        if (configModel.SPMIVersion == eVersion.one || SPMISlaveAck == eAcknowledgeType.NA)
                            privateArray[5] = 0x0F; // ignoring ack for version 1.0.
                        else
                            privateArray[5] = 0x0D;
                        #endregion
                        break;
                    }
                case (int)eSPMICMDTYPE.SHUTDOWN:
                    {
                        #region Command
                        if ((int)SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | (CommandValue & 0xF0) >> 4);
                            privateArray[1] = Convert.ToByte((CommandValue & 0x0F) << 4 | 0x0);//Include parity bit and ack
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                            privateArray[5] = 0x0F;
                        }
                        #endregion
                        #region Ack/Nack
                        var ack = (SPMISlaveAck == eAcknowledgeType.ACK) ? 1 : 0;
                        privateArray[1] = Convert.ToByte(privateArray[1] | (ack << 1) | 0x0);
                        if (configModel.SPMIVersion == eVersion.one || SPMISlaveAck == eAcknowledgeType.NA)
                            privateArray[5] = 0x0F; // ignoring ack for version 1.0.
                        else
                            privateArray[5] = 0x0D;
                        #endregion
                        break;
                    }
                case (int)eSPMICMDTYPE.WAKEUP:
                    {
                        #region Command
                        if ((int)SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | (CommandValue & 0xF0) >> 4);
                            privateArray[1] = Convert.ToByte((CommandValue & 0x0F) << 4 | 0x0);//Include parity bit and ack
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                            privateArray[5] = 0x0F;
                        }
                        #endregion
                        #region Ack/Nack
                        var ack = (SPMISlaveAck == eAcknowledgeType.ACK) ? 1 : 0;
                        privateArray[1] = Convert.ToByte(privateArray[1] | (ack << 1) | 0x0);
                        if (configModel.SPMIVersion == eVersion.one || SPMISlaveAck == eAcknowledgeType.NA)
                            privateArray[5] = 0x0F; // ignoring ack for version 1.0.
                        else
                            privateArray[5] = 0x0D;
                        #endregion
                        break;
                    }
                case (int)eSPMICMDTYPE.DDB_SL_R:
                    {
                        #region Command
                        if ((int)SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | (CommandValue & 0xF0) >> 4);
                            privateArray[1] = Convert.ToByte((CommandValue & 0x0F) << 4 | 0x0);//Include parity bit
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                            privateArray[5] = 0x0F;
                        }
                        #endregion
                        #region Data
                        if (SPMIData != null && SPMIData != "")
                        {
                            var strData = SPMIData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    privateArray[1] = Convert.ToByte((privateArray[1]) | (intData >> 14));
                                    privateArray[2] = Convert.ToByte(((intData >> 8) & 0x3F) << 2 | ((intData & 0x80) >> 7));
                                    privateArray[3] = Convert.ToByte((intData & 0x7F) << 1 | 0x0);
                                    privateArray[6] = 0x02;
                                    privateArray[7] = 0x01;

                                }
                            }
                        }
                        #endregion
                        break;
                    }
                case (int)eSPMICMDTYPE.DEFAULT:
                    {
                        break;
                    }

            }
            return privateArray;
        }

    }
}
