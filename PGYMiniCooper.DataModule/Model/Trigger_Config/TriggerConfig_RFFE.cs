using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGYMiniCooper.DataModule.Interfaces;
using ProdigyFramework.ComponentModel;

namespace PGYMiniCooper.DataModule.Model.Trigger_Config
{
    public class TriggerConfig_RFFE : ViewModelBase, ITriggerConfigViewModel
    {

        #region RFFE properties

        private eRFFECMDTYPE selRFFECommand;
        public eRFFECMDTYPE SelRFFECommand
        {
            get
            {
                return selRFFECommand;
            }
            set
            {
                selRFFECommand = value;
                RaisePropertyChanged("SelRFFECommand");
            }
        }

        private string RFFEslaveAddress;
        public string RFFESlaveAddress
        {
            get
            {
                return RFFEslaveAddress;
            }

            set
            {
                RFFEslaveAddress = value;
                RaisePropertyChanged("RFFESlaveAddress");
            }
        }

        private string rFFEBytecount;
        public string RFFEBytecount
        {
            get
            {
                return rFFEBytecount;
            }

            set
            {
                rFFEBytecount = value;
                RaisePropertyChanged("RFFEBytecount");
            }
        }

        private string rFFEregAddr;
        public string RFFEregAddr
        {
            get
            {
                return rFFEregAddr;
            }

            set
            {
                rFFEregAddr = value;
                RaisePropertyChanged("RFFEregAddr");
            }
        }

        private string _RFFEdata;
        public string RFFEData
        {
            get
            {
                return _RFFEdata;
            }
            set
            {
                _RFFEdata = value;
                RaisePropertyChanged("RFFEData");
            }
        }

        private string _RFFEmaskdata;
        public string RFFEMaskData
        {
            get
            {
                return _RFFEmaskdata;
            }
            set
            {
                _RFFEmaskdata = value;
                RaisePropertyChanged("RFFEMaskData");
            }
        }

        private eAcknowledgeType RFFEslaveAck;
        public eAcknowledgeType RFFESlaveAck
        {
            get
            {
                return RFFEslaveAck;
            }

            set
            {
                RFFEslaveAck = value;
                RaisePropertyChanged("RFFESlaveAck");
            }
        }

        private int rFFErxMId = -1;
        public int RFFERxMID
        {
            get
            {
                return rFFErxMId;
            }
            set
            {
                rFFErxMId = value;
                RaisePropertyChanged("RFFERxMID");
            }
        }

        private bool rFFEISIEnable;
        public bool RFFEISIEnable
        {
            get { return rFFEISIEnable; }
            set { rFFEISIEnable = value; RaisePropertyChanged("RFFEISIEnable"); }
        }

        private eInterruptSlot rFFEinterrupt;
        public eInterruptSlot RFFEInterrupt
        {
            get
            {
                return rFFEinterrupt;
            }
            set
            {
                rFFEinterrupt = value;
                RaisePropertyChanged("RFFEInterrupt");
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
            if (!int.TryParse(RFFESlaveAddress, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out slaveAddr))
                slaveAddr = -1;
            int bytecount = -1;
            if (!int.TryParse(RFFEBytecount, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out bytecount))
                bytecount = -1;
            int regAddr = -1;
            if (!int.TryParse(RFFEregAddr, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out regAddr))
                regAddr = -1;
            byte CommandValue = ProtocolInfoRepository.GetRFFECmdId((byte)SelRFFECommand);
            //Note: By default set the fields as don't care. On fields present, map the data and mask it

            #region Slave Address
            if ((int)SelRFFECommand == (int)eRFFECMDTYPE.MASTER_CXT_TRANSFER_WRITE || (int)SelRFFECommand == (int)eRFFECMDTYPE.MASTER_CXT_TRANSFER_READ || (int)SelRFFECommand == (int)eRFFECMDTYPE.MASTER_OWNERSHIP_HANDOVER
                || (int)SelRFFECommand == (int)eRFFECMDTYPE.MASTER_WRITE || (int)SelRFFECommand == (int)eRFFECMDTYPE.MASTER_READ)
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

            #endregion

            switch ((int)SelRFFECommand)
            {
                case (int)eRFFECMDTYPE.EXT_REG_WRITE:
                    {
                        #region Command
                        if ((int)SelRFFECommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | ((CommandValue & 0xF0) >> 4));

                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                        }
                        #endregion

                        #region CommandBytecount
                        if ((int)SelRFFECommand != -1 && bytecount != -1)
                        {
                            privateArray[1] = Convert.ToByte((bytecount & 0x0F) << 4 | 0x0);
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if (regAddr != -1 && (int)SelRFFECommand != -1)
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
                        if (RFFEData != null && RFFEData != "")
                        {
                            var strData = RFFEData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    privateArray[2] = Convert.ToByte(privateArray[2] | (intData >> 6));
                                    privateArray[3] = Convert.ToByte(((intData & 0x3F) << 2));
                                    if (regAddr != -1)
                                        privateArray[6] = Convert.ToByte(0x04);//parity mask
                                    else
                                        privateArray[6] = Convert.ToByte(0xFC);
                                    privateArray[7] = 0x03;

                                }
                            }
                        }
                        #endregion

                        break;
                    }

                case (int)eRFFECMDTYPE.EXT_REG_READ:
                    {
                        #region Command
                        // T on Command only
                        if ((int)SelRFFECommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | ((CommandValue & 0xF0) >> 4));

                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                        }
                        #endregion

                        #region CommandBytecount
                        if ((int)SelRFFECommand != -1 && bytecount != -1)
                        {
                            privateArray[1] = Convert.ToByte((bytecount & 0x0F) << 4 | 0x0);//Include parity bit
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if (regAddr != -1 && (int)SelRFFECommand != -1)
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
                        if (RFFEData != null && RFFEData != "")
                        {
                            var strData = RFFEData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    privateArray[2] = Convert.ToByte(privateArray[2] | (intData >> 7));
                                    privateArray[3] = Convert.ToByte(((intData & 0x7F) << 1) | 0x0);
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

                case (int)eRFFECMDTYPE.EXT_REG_WRITE_LONG:
                    {
                        #region Command
                        if ((int)SelRFFECommand != -1) //bc 3 bit
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
                        if ((int)SelRFFECommand != -1 && bytecount != -1)
                        {
                            privateArray[1] = Convert.ToByte(0x0 << 7 | (bytecount & 0x07) << 4 | 0x0);
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if ((int)SelRFFECommand != -1 && regAddr != -1)
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
                case (int)eRFFECMDTYPE.EXT_REG_READ_LONG:
                    {

                        #region Command
                        if ((int)SelRFFECommand != -1)
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
                        if ((int)SelRFFECommand != -1 && bytecount != -1)
                        {
                            privateArray[1] = Convert.ToByte(0x1 << 7 | (bytecount & 0x07) << 4 | 0x0);//Include parity bit
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if ((int)SelRFFECommand != -1 && regAddr != -1)
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
                case (int)eRFFECMDTYPE.REG_WRITE:
                    {
                        #region Command
                        if ((int)SelRFFECommand != -1)
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
                        if ((int)SelRFFECommand != -1 && regAddr != -1)
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
                        if (RFFEData != null && RFFEData != "")
                        {
                            var strData = RFFEData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    if (RFFEData != null && RFFEData != "")
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
                        break;
                    }
                case (int)eRFFECMDTYPE.REG_READ:
                    {
                        #region Command
                        if ((int)SelRFFECommand != -1)
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
                        if ((int)SelRFFECommand != -1 && regAddr != -1)
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
                        if (RFFEData != null && RFFEData != "")
                        {
                            var strData = RFFEData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    if (RFFEData != null && RFFEData != "")
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
                case (int)eRFFECMDTYPE.REG_ZERO_WRITE:
                    {
                        #region Command
                        if ((int)SelRFFECommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | ((CommandValue & 0x80) >> 4));
                            if (slaveAddr != -1)
                                privateArray[4] = 0x07;
                            else
                                privateArray[4] = 0xF7;
                        }
                        #endregion

                        #region CmdData
                        if ((int)SelRFFECommand != -1 && RFFEData != null && RFFEData != "")
                        {
                            var strData = RFFEData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    if ((int)SelRFFECommand != -1 && RFFEData != null && RFFEData != "")
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

                        break;
                    }
                case (int)eRFFECMDTYPE.INT_SUMMARY_IDENT:
                    {
                        #region Command
                        if ((int)SelRFFECommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | (CommandValue & 0xF0) >> 4);
                            privateArray[1] = Convert.ToByte((CommandValue & 0x0F) << 4 | 0x0 << 3 | 0x1 << 2 | (RFFEISIEnable ? 0x1 : 0x0) << 1);
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;

                            if (RFFEISIEnable)
                                privateArray[5] = 0x0D;
                            else
                                privateArray[5] = 0x0F;

                            if (RFFEISIEnable == true && RFFEInterrupt == eInterruptSlot.INT15)
                            {
                                privateArray[1] = Convert.ToByte(privateArray[1] | 0x01);
                                privateArray[5] = 0x0C;
                            }

                            int word1 = 0x0;
                            int word2 = 0x0;
                            int word1Mask = 0xFF;
                            int word2Mask = 0xFF;

                            #region InterruptSlot
                            switch (RFFEInterrupt)
                            {
                                case eInterruptSlot.INT14:
                                    word1 |= 0x80;
                                    word1Mask = 0x7F;
                                    break;
                                case eInterruptSlot.INT13:
                                    word1 |= 0x40;
                                    word1Mask = 0xBF;
                                    break;
                                case eInterruptSlot.INT12:
                                    word1 |= 0x20;
                                    word1Mask = 0xDF;
                                    break;
                                case eInterruptSlot.INT11:
                                    word1 |= 0x10;
                                    word1Mask = 0xEF;
                                    break;
                                case eInterruptSlot.INT10:
                                    word1 |= 0x08;
                                    word1Mask = 0xF7;
                                    break;
                                case eInterruptSlot.INT9:
                                    word1 |= 0x04;
                                    word1Mask = 0xFB;
                                    break;
                                case eInterruptSlot.INT8:
                                    word1 |= 0x02;
                                    word1Mask = 0xFD;
                                    break;
                                case eInterruptSlot.INT7:
                                    word1 |= 0x01;
                                    word1Mask = 0xFE;
                                    break;
                                case eInterruptSlot.INT6:
                                    word2 |= 0x80;
                                    word2Mask = 0x7F;
                                    break;
                                case eInterruptSlot.INT5:
                                    word2 |= 0x40;
                                    word2Mask = 0xBF;
                                    break;
                                case eInterruptSlot.INT4:
                                    word2 |= 0x20;
                                    word2Mask = 0xDF;
                                    break;
                                case eInterruptSlot.INT3:
                                    word2 |= 0x10;
                                    word2Mask = 0xEF;
                                    break;
                                case eInterruptSlot.INT2:
                                    word2 |= 0x08;
                                    word2Mask = 0xF7;
                                    break;
                                case eInterruptSlot.INT1:
                                    word2 |= 0x04;
                                    word2Mask = 0xFB;
                                    break;
                                case eInterruptSlot.INT0:
                                    word2 |= 0x02;
                                    word2Mask = 0xFD;
                                    break;
                            }
                            #endregion

                            privateArray[2] = Convert.ToByte(word1);
                            privateArray[3] = Convert.ToByte(word2);
                            privateArray[6] = Convert.ToByte(word1Mask);
                            privateArray[7] = Convert.ToByte(word2Mask);
                        }
                        #endregion

                        break;
                    }

                case (int)eRFFECMDTYPE.MASTER_OWNERSHIP_HANDOVER:
                    {
                        #region Command
                        if ((int)SelRFFECommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | (CommandValue & 0xF0) >> 4);
                            privateArray[1] = Convert.ToByte((CommandValue & 0x0F) << 4 | 0x0 << 3 | 0x1 << 2 | (RFFESlaveAck == eAcknowledgeType.ACK ? 0x1 : 0x0) << 1 | 0x0);
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;

                            if (RFFESlaveAck == eAcknowledgeType.ACK)
                                privateArray[5] = 0x0D;
                            else
                                privateArray[5] = 0x0F;

                            if (RFFERxMID != -1 && RFFESlaveAck == eAcknowledgeType.ACK)
                            {
                                // privateArray[1] = Convert.ToByte(privateArray[1] | (0x0));
                                // privateArray[5] = 0x01;
                                privateArray[2] = Convert.ToByte(0x0 << 7 | (RFFERxMID & 0x3) << 5 | 0x1);
                                privateArray[6] = 0x9F;
                            }
                        }
                        #endregion

                        break;
                    }

                case (int)eRFFECMDTYPE.MASKED_WRITE:
                    {
                        #region Command
                        if ((int)SelRFFECommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | (CommandValue & 0xF0) >> 4);
                            privateArray[1] = Convert.ToByte((CommandValue & 0x0F) << 4 | 0x0);
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if (regAddr != -1)
                        {
                            privateArray[1] = Convert.ToByte((privateArray[1]) | (regAddr >> 5));
                            privateArray[2] = Convert.ToByte((regAddr & 0x1F) << 3 | 0x0);
                            privateArray[5] = 0x08;
                            privateArray[6] = 0x07;
                        }
                        #endregion

                        #region Data
                        if (RFFEMaskData != null && RFFEMaskData != "")
                        {
                            var strData = RFFEMaskData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    privateArray[2] = Convert.ToByte(privateArray[2] | (intData >> 6));
                                    privateArray[3] = Convert.ToByte((intData << 2) | 0x0);
                                    privateArray[6] = 0x04;
                                    privateArray[7] = 0x03;

                                }
                            }
                        }
                        #endregion

                        break;
                    }

                case (int)eRFFECMDTYPE.MASTER_WRITE:
                    {
                        #region Command
                        if ((int)SelRFFECommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | (CommandValue & 0xF0) >> 4);
                            privateArray[1] = Convert.ToByte((CommandValue & 0x0F) << 4 | 0x0);
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if (regAddr != -1)
                        {
                            privateArray[1] = Convert.ToByte((privateArray[1]) | (regAddr >> 5));
                            privateArray[2] = Convert.ToByte((regAddr & 0x1F) << 3 | 0x0);
                            privateArray[5] = 0x08;
                            privateArray[6] = 0x07;
                        }
                        #endregion

                        #region Data
                        if (RFFEData != null && RFFEData != "")
                        {
                            var strData = RFFEData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    privateArray[2] = Convert.ToByte(privateArray[2] | (intData >> 6));
                                    privateArray[3] = Convert.ToByte((intData << 2) | 0x0);
                                    privateArray[6] = 0x04;
                                    privateArray[7] = 0x03;

                                }
                            }
                        }
                        #endregion
                        break;
                    }
                case (int)eRFFECMDTYPE.MASTER_READ:
                    {

                        #region Command
                        if ((int)SelRFFECommand != -1)
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

                        #region Register Address
                        if (regAddr != -1)
                        {
                            privateArray[1] = Convert.ToByte((privateArray[1]) | (regAddr >> 5));
                            privateArray[2] = Convert.ToByte((regAddr & 0x1F) << 3 | 0x0 << 2 | 0x1 << 1);
                            privateArray[5] = 0x08;
                            privateArray[6] = 0x07;
                        }
                        #endregion

                        #region Data
                        if (RFFEData != null && RFFEData != "")
                        {
                            var strData = RFFEData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    privateArray[2] = Convert.ToByte(privateArray[2] | (intData >> 7));
                                    privateArray[3] = Convert.ToByte((intData << 1) | 0x0);
                                    privateArray[6] = 0x06;
                                    privateArray[7] = 0x01;

                                }
                            }
                        }
                        #endregion
                        break;
                    }

                case (int)eRFFECMDTYPE.MASTER_CXT_TRANSFER_WRITE:
                    {
                        #region Command
                        if ((int)SelRFFECommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | (CommandValue & 0xF0) >> 4);
                            privateArray[1] = Convert.ToByte((CommandValue & 0x0F) << 4 | 0x0 << 3);//Include parity bit
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region ByteCount
                        if (bytecount != -1)
                        {
                            privateArray[1] = Convert.ToByte(privateArray[1] | bytecount >> 5);
                            privateArray[2] = Convert.ToByte((bytecount & 0x1F) << 3 | 0x0 << 2);
                            privateArray[5] = 0x08;
                            privateArray[6] = 0x07;
                        }
                        #endregion

                        #region Register Address
                        if (regAddr != -1)
                        {
                            privateArray[2] = Convert.ToByte((privateArray[2]) | (regAddr >> 6));
                            privateArray[3] = Convert.ToByte((regAddr & 0x3F) << 2 | 0x0 << 1);
                            privateArray[6] = 0x04;
                            privateArray[7] = 0x03;
                        }
                        #endregion
                        break;
                    }

                case (int)eRFFECMDTYPE.MASTER_CXT_TRANSFER_READ:
                    {

                        #region Command
                        if ((int)SelRFFECommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | (CommandValue & 0xF0) >> 4);
                            privateArray[1] = Convert.ToByte((CommandValue & 0x0F) << 4 | 0x0 << 3 | 0x0 << 2);//Include parity bit
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region ByteCount
                        if (bytecount != -1)
                        {
                            privateArray[1] = Convert.ToByte(privateArray[1] | bytecount >> 5);
                            privateArray[2] = Convert.ToByte((bytecount & 0x1F) << 3 | 0x0 << 2);
                            privateArray[5] = 0x08;
                            privateArray[6] = 0x07;
                        }
                        #endregion

                        #region Register Address
                        if (regAddr != -1)
                        {
                            privateArray[2] = Convert.ToByte((privateArray[2]) | (regAddr >> 6));
                            privateArray[3] = Convert.ToByte((regAddr & 0x3F) << 2 | 0x0);
                            privateArray[6] = 0x04;
                            privateArray[7] = 0x03;
                        }
                        #endregion
                        break;
                    }

            }
            return privateArray;
        }
    }
}
