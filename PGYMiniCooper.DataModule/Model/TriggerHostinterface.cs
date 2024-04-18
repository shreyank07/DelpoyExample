using PGYMiniCooper.DataModule.Structure;
using ProdigyFramework.ComponentModel;
using ProdigyFramework.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PGYMiniCooper.DataModule.Model
{
    class TriggerHostinterface : ViewModelBase
    {

        static TriggerHostinterface _instance = null;
        public static TriggerHostinterface GetInstance()
        {
            if (_instance == null)
            {
                _instance = new TriggerHostinterface();
            }

            return _instance;
        }

        #region Constructor
        public TriggerHostinterface()
        {
           // TriggerModel.GetInstance() = TriggerModel.GetInstance();

            I2CTriggerAtList = new List<eI2CTriggerAtList>();
            I2CTriggerAtList.Add(eI2CTriggerAtList.Start);
            I2CTriggerAtList.Add(eI2CTriggerAtList.Address);
            I2CTriggerAtList.Add(eI2CTriggerAtList.Data);
            I2CTriggerAtList.Add(eI2CTriggerAtList.Address_Data);
            I2CTriggerAtList.Add(eI2CTriggerAtList.Ack);
            I2CTriggerAtList.Add(eI2CTriggerAtList.Nack);
            I2CTriggerAtList.Add(eI2CTriggerAtList.Repeated_Start);
            I2CTriggerAtList.Add(eI2CTriggerAtList.Stop);

            ComparisonList = new List<eComparisonList>();
            ComparisonList.Add(eComparisonList.Equal_to);
            ComparisonList.Add(eComparisonList.Not_equal_to);

            TimingTriggerTypeList = new List<eTimingTriggerTypeList>();
            TimingTriggerTypeList.Add(eTimingTriggerTypeList.Pulse_Width);
            TimingTriggerTypeList.Add(eTimingTriggerTypeList.Delay);

            PulseComparisonList = new List<ePulseComparisonList>();
            PulseComparisonList.Add(ePulseComparisonList.Greater_than);
            PulseComparisonList.Add(ePulseComparisonList.Less_than);

            BufferSizeList = new List<eBufferSize>();
            BufferSizeList.Add(eBufferSize._100KB);
            BufferSizeList.Add(eBufferSize._500KB);
            BufferSizeList.Add(eBufferSize._1MB);
            BufferSizeList.Add(eBufferSize._2MB);
            BufferSizeList.Add(eBufferSize.Continuous_2MB);

            transferTypesList = new List<eTransferType>();
            TransferTypesList.Add(eTransferType.WR);
            TransferTypesList.Add(eTransferType.RD);

            i3cACKList = new List<eAcknowledgeType>() { eAcknowledgeType.ACK, eAcknowledgeType.NACK };
            overwriteLog = true;
        }
        #endregion

        #region Data Members

        public IEnumerable<eI3CMessage> I3CMessageTypeList
        {
            get
            {
                return Enum.GetValues(typeof(eI3CMessage)).Cast<eI3CMessage>();
            }
        }

        public IEnumerable<eSPMICMDTYPE> SPMICommandList
        {
            get
            {
                return Enum.GetValues(typeof(eSPMICMDTYPE)).Cast<eSPMICMDTYPE>();
            }
        }

        public IEnumerable<eInterruptSlot> InterruptSlots
        {
            get
            {
                return Enum.GetValues(typeof(eInterruptSlot)).Cast<eInterruptSlot>();
            }
        }

        public IEnumerable<eRFFECMDTYPE> RFFECommandList
        {
            get
            {
                return Enum.GetValues(typeof(eRFFECMDTYPE)).Cast<eRFFECMDTYPE>();
            }
        }

        private List<eTransferType> transferTypesList;
        public List<eTransferType> TransferTypesList
        {
            get
            {
                return transferTypesList;
            }
            set
            {
                transferTypesList = value;
                RaisePropertyChanged("TransferTypesList");
            }
        }

        private List<eBufferSize> bufferSizeList;

        public List<eBufferSize> BufferSizeList { get { return bufferSizeList; } set { bufferSizeList = value; RaisePropertyChanged("BufferSizeList"); } }


        //private TriggerModel triggerModel;
        //public TriggerModel TriggerModel
        //{
        //    get
        //    {
        //        return triggerModel;
        //    }
        //    set
        //    {
        //        triggerModel = value;
        //        RaisePropertyChanged("TriggerModel");
        //    }
        //}

        private IEnumerable<eBroadcastCCC> commandBrd = Enum.GetValues(typeof(eBroadcastCCC)).Cast<eBroadcastCCC>();
        public IEnumerable<eBroadcastCCC> CommandBrd
        {
            get
            {
                return commandBrd;
            }
        }

        private IEnumerable<eDirectedCCC> commandDir = Enum.GetValues(typeof(eDirectedCCC)).Cast<eDirectedCCC>();
        public IEnumerable<eDirectedCCC> CommandDir
        {
            get
            {
                return commandDir;
            }
        }

        private List<eAcknowledgeType> i3cACKList;
        public List<eAcknowledgeType> I3CACKList { get { return i3cACKList; } set { i3cACKList = value; RaisePropertyChanged("i3cACKList"); } }

        private List<eI2CTriggerAtList> i2CTriggerAtList;

        public List<eI2CTriggerAtList> I2CTriggerAtList { get { return i2CTriggerAtList; } set { i2CTriggerAtList = value; RaisePropertyChanged("I2CTriggerAtList"); } }


        private List<eComparisonList> comparisonList;

        public List<eComparisonList> ComparisonList { get { return comparisonList; } set { comparisonList = value; RaisePropertyChanged("ComparisonList"); } }

        private List<String> channelList;

        public List<string> ChannelList { get { return channelList; } set { channelList = value; RaisePropertyChanged("ChannelList"); } }

        private List<eTimingTriggerTypeList> timingTriggerTypeList;

        public List<eTimingTriggerTypeList> TimingTriggerTypeList { get { return timingTriggerTypeList; } set { timingTriggerTypeList = value; RaisePropertyChanged("TimingTriggerTypeList"); } }

        private List<ePulseComparisonList> pulseComparisonList;

        public List<ePulseComparisonList> PulseComparisonList { get { return pulseComparisonList; } set { pulseComparisonList = value; RaisePropertyChanged("PulseComparisonList"); } }

        #endregion

        public byte[] GetTriggerBytes()
        {
            /*
             *      _________________________________________________________________________________________________________________________________
             *      |       7      |        6       |       5       |       4       |       3       |       2       |       1       |       0       |
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |                                               LEVEL_0_PARAMETER_C0[07:00]                                                     | [0]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |                                               LEVEL_0_PARAMETER_C0[15:08]                                                     | [1]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |                                               LEVEL_0_PARAMETER_C0[23:16]                                                     | [2]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |                                               LEVEL_0_PARAMETER_C0[31:24]                                                     | [3]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |                                               LEVEL_0_MASK_C0[07:00]                                                          | [4]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |                                               LEVEL_0_MASK_C0[15:08]                                                          | [5]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |                                               LEVEL_0_MASK_C0[23:16]                                                          | [6]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |                                               LEVEL_0_MASK_C0[31:24]                                                          | [7]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |                                               LEVEL_0_MATCH_COUNT_C0                                                          | [8]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |                               LEVEL_0_FALSE_ACTION_C0          |                 LEVEL_0_TRUE_ACTION_C0                       | [9]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |                               LEVEL_0_TIMER_2_ACTION_C0        |                 LEVEL_0_TIMER_1_ACTION_C0                    | [10]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |    UART      | Data Select     |      SPI     |    I2C         |          Sel_pulse_timer_2     |       Sel_pulse_timer_1     | [11]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |                                                      Reserved                                                                 | [12]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |                                                       Reserved                                                                | [13]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |                                                       Reserved                                                                | [14]
             *      |-------------------------------------------------------------------------------------------------------------------------------|
             *      |                                                       Reserved                                                                | [15]
             *      |_______________________________________________________________________________________________________________________________|
*/
            MouseEventExtension.MouseEvent(MouseEventExtension.MouseEventFlags.LeftDown);
            MouseEventExtension.MouseEvent(MouseEventExtension.MouseEventFlags.LeftUp);
            List<byte> triggerBytes = new List<byte>();
            if (TriggerModel.GetInstance().TriggerType == eTriggerTypeList.Auto)
            {
                return null;
            }
            if (TriggerModel.GetInstance().TriggerType == eTriggerTypeList.Pattern || TriggerModel.GetInstance().TriggerType == eTriggerTypeList.Timing)
            {
                triggerBytes.Add(0x15);//[42] // Pkt type
                triggerBytes.Add(0x0);    //[43] // Pkt len
                triggerBytes.Add(0);    //[44] // Pkt len
                byte temp = 0;
                if (TriggerModel.GetInstance().TriggerType == eTriggerTypeList.Timing)
                {
                    if (TriggerModel.GetInstance().TimingPulsePositiveSelected && TriggerModel.GetInstance().TimingTriggerTypeSelected == eTimingTriggerTypeList.Pulse_Width)
                        temp |= 0x80;
                    if (TriggerModel.GetInstance().PulseComparisonSelected == ePulseComparisonList.Greater_than && TriggerModel.GetInstance().TimingTriggerTypeSelected == eTimingTriggerTypeList.Pulse_Width)
                        temp |= 0x40;
                    if (TriggerModel.GetInstance().DelayComparisonSelected == ePulseComparisonList.Greater_than && TriggerModel.GetInstance().TimingTriggerTypeSelected == eTimingTriggerTypeList.Delay)
                        temp |= 0x10;
                    if (TriggerModel.GetInstance().TimingTriggerTypeSelected == eTimingTriggerTypeList.Delay)
                        temp |= 0x08;
                    if (TriggerModel.GetInstance().TimingTriggerTypeSelected == eTimingTriggerTypeList.Pulse_Width)
                        temp |= 0x04;
                }
                if (TriggerModel.GetInstance().TriggerType == eTriggerTypeList.Pattern)
                    temp |= 0x02;
                temp |= 0x20;
                triggerBytes.Add(temp);    //[45]
                if (TriggerModel.GetInstance().TimingTriggerTypeSelected == eTimingTriggerTypeList.Pulse_Width && TriggerModel.GetInstance().TriggerType == eTriggerTypeList.Timing)
                {
                    int count = 0;
                    int.TryParse(TriggerModel.GetInstance().PulseWidthCount, out count);
                    byte[] bytes = BitConverter.GetBytes(count);
                    if (bytes.Length > 1)
                        triggerBytes.Add(bytes[1]);    //[46] // Count pulse width trigger
                    else
                        triggerBytes.Add(0); //[46] // Count pulse width trigger
                    triggerBytes.Add(bytes[0]);    //[47] // Count pulse width trigger
                }
                else
                {
                    triggerBytes.Add(0);    //[46] // Count pulse width trigger
                    triggerBytes.Add(0);    //[47] // Count pulse width trigger
                }
                if (TriggerModel.GetInstance().TimingTriggerTypeSelected == eTimingTriggerTypeList.Delay && TriggerModel.GetInstance().TriggerType == eTriggerTypeList.Timing)
                {
                    int count = 0;
                    int.TryParse(TriggerModel.GetInstance().DelayCount, out count);
                    byte[] bytes = BitConverter.GetBytes(count);
                    if (bytes.Length > 1)
                        triggerBytes.Add(bytes[1]);    //[48] // Count Delay trigger
                    else
                        triggerBytes.Add(0); //[48] // Count Delay trigger
                    triggerBytes.Add(bytes[0]);    //[49] // Count Delay trigger

                }
                else
                {
                    triggerBytes.Add(0);    //[48] // Count Delay trigger
                    triggerBytes.Add(0);    //[49]
                }
                if (TriggerModel.GetInstance().TimingTriggerTypeSelected == eTimingTriggerTypeList.Pulse_Width && TriggerModel.GetInstance().TriggerType == eTriggerTypeList.Timing)
                    triggerBytes.Add(Convert.ToByte(GetChannelByte(TriggerModel.GetInstance().PulseWidthChannel) << 4));    //[50] // Pulse width trigger channel select
                else
                    triggerBytes.Add(0); //[50]

                if (TriggerModel.GetInstance().TimingTriggerTypeSelected == eTimingTriggerTypeList.Delay && TriggerModel.GetInstance().TriggerType == eTriggerTypeList.Timing)
                    triggerBytes.Add(Convert.ToByte((GetChannelByte(TriggerModel.GetInstance().DelayChannel1) << 4) | (GetChannelByte(TriggerModel.GetInstance().DelayChannel2))));    //[51] // Delay channel 1 , 2
                else
                    triggerBytes.Add(0); //[51]

                try
                {
                    if (TriggerModel.GetInstance().PatternFormat == ePatternFormat.Binary)
                    {
                        byte[] tempArray = BinaryToBytePattern(TriggerModel.GetInstance().PatternText, out byte[] mask);
                        if (tempArray.Count() > 0)
                            triggerBytes.Add(tempArray[0]);    //[52] // pattern bits.
                        else
                            triggerBytes.Add(0);    //[52] // pattern bits.
                        if (tempArray.Count() > 1)
                            triggerBytes.Add(Convert.ToByte(tempArray[1]));    //[53]
                        else
                            triggerBytes.Add(0);    //[53]

                        triggerBytes.Add(mask[0]);    //[54]
                        triggerBytes.Add(mask[1]);    //[55]
                    }
                    else if (TriggerModel.GetInstance().PatternFormat == ePatternFormat.Hex)
                    {
                        if (TriggerModel.GetInstance().PatternText.Contains("0x"))
                            TriggerModel.GetInstance().PatternText = TriggerModel.GetInstance().PatternText.Replace("0x", "");
                        else if (TriggerModel.GetInstance().PatternText.Contains("0X"))
                            TriggerModel.GetInstance().PatternText = TriggerModel.GetInstance().PatternText.Replace("0X", "");
                        int intPattern = 0;
                        int.TryParse(TriggerModel.GetInstance().PatternText, System.Globalization.NumberStyles.HexNumber, null, out intPattern);
                        int bits10 = (intPattern & 0xFFFF);
                        byte[] bytes = BitConverter.GetBytes(bits10);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(bytes);

                        //triggerBytes.Add(Convert.ToByte(((bytes[2] & 0x03) << 6) | (bytes[3] >> 2))); //[52]
                        //triggerBytes.Add(Convert.ToByte((bytes[3] & 0x02) << 6));    //[53]
                        //For reversing bit as per UI.
                        byte tempB = Reverse(bytes[3]);
                        triggerBytes.Add(tempB); //[52]
                        tempB = Reverse(bytes[2]);
                        triggerBytes.Add(tempB);    //[53]

                        triggerBytes.Add(Convert.ToByte(0x0));    //[54]
                        triggerBytes.Add(Convert.ToByte(0x0));    //[55]
                    }
                    else if (TriggerModel.GetInstance().PatternFormat == ePatternFormat.Decimal)
                    {
                        int intPattern;
                        int.TryParse(TriggerModel.GetInstance().PatternText, out intPattern);
                        int bits10 = (intPattern & 0xFFFF);
                        byte[] bytes = BitConverter.GetBytes(bits10);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(bytes);

                        //triggerBytes.Add(Convert.ToByte(((bytes[2] & 0x03) << 6) | (bytes[3] >> 2))); //[52]
                        //triggerBytes.Add(Convert.ToByte((bytes[3] & 0x02) << 6));    //[53]
                        //For reversing bit as per UI.
                        byte tempB = Reverse(bytes[3]);
                        triggerBytes.Add(tempB); //[52]
                        tempB = Reverse(bytes[2]);
                        triggerBytes.Add(tempB);    //[53]

                        triggerBytes.Add(Convert.ToByte(0x0));    //[54]
                        triggerBytes.Add(Convert.ToByte(0x0));    //[55]
                    }
                    else if (TriggerModel.GetInstance().PatternFormat == ePatternFormat.Octal)
                    {
                        int intPattern = 0;
                        if (TriggerModel.GetInstance().PatternText != string.Empty)
                            intPattern = Convert.ToInt32(TriggerModel.GetInstance().PatternText, 8);
                        int bits10 = (intPattern & 0xFFFF);
                        byte[] bytes = BitConverter.GetBytes(bits10);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(bytes);

                        //triggerBytes.Add(Convert.ToByte(((bytes[2] & 0x03) << 6) | (bytes[3] >> 2))); //[52]
                        //triggerBytes.Add(Convert.ToByte((bytes[3] & 0x02) << 6));    //[53]
                        //For reversing bit as per UI.
                        byte tempB = Reverse(bytes[3]);
                        triggerBytes.Add(tempB); //[52]
                        tempB = Reverse(bytes[2]);
                        triggerBytes.Add(tempB);    //[53]

                        triggerBytes.Add(Convert.ToByte(0x0));    //[54]
                        triggerBytes.Add(Convert.ToByte(0x0));    //[55]
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Invalid Trigger Pattern Expression", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                //if (tempArray.Count() > 0)
                //    triggerBytes.Add(0);    //[54]
                //else
                //    triggerBytes.Add(0xFF);    //[54]
                //if (tempArray.Count() > 1)
                //    triggerBytes.Add(0x2F);    //[55]
                //else
                //    triggerBytes.Add(0xFF);    //[55]
                triggerBytes.Add(0);    //[56]
                triggerBytes.Add(0);    //[57]
                triggerBytes.Add(0);    //[58]
                triggerBytes.Add(0);    //[59]
                triggerBytes.Add(0);    //[60]
                triggerBytes.Add(0);    //[61]
                triggerBytes.Add(0);    //[62]
                triggerBytes.Add(0);    //[63]

                //try
                //{
                //    if (overwriteLog)
                //    {
                //        using (FileStream fs = new FileStream(@MiniCooperDirectoryInfo.AppPath + "\\triggerlog.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                //        {
                //            StreamReader sr = new StreamReader(fs);
                //            using (StreamWriter sw = new StreamWriter(fs))
                //            {
                //                // discard the contents of the file by setting the length to 0
                //                fs.SetLength(0);

                //                // write the new content
                //                foreach (var bytes in triggerBytes)
                //                {
                //                    //Log(bytes.ToString(),w);
                //                    sw.Write(bytes.ToString("X2") + " ");
                //                }
                //                sw.WriteLine();

                //            }
                //        }
                //        overwriteLog = false;
                //    }
                //    else
                //    {
                //        FileStream objFilestream = new FileStream(@MiniCooperDirectoryInfo.AppPath + "\\triggerlog.txt", FileMode.Append, FileAccess.Write);
                //        StreamWriter objStreamWriter = new StreamWriter((Stream)objFilestream);
                //        foreach (var bytes in triggerBytes)
                //        {
                //            //Log(bytes.ToString(),w);
                //            objStreamWriter.Write(bytes.ToString("X2") + " ");
                //        }
                //        objStreamWriter.WriteLine();
                //        objStreamWriter.Close();
                //        objFilestream.Close();
                //    }
                //}
                //catch (Exception ex)
                //{

                //}
                return triggerBytes.ToArray();
            }
            triggerBytes.Add(0x20);//[42]
            triggerBytes.Add(0x10);    //[43]
            triggerBytes.Add(0);    //[44]
            triggerBytes.Add(0);    //[45]
            triggerBytes.Add(0);    //[46]
            triggerBytes.Add(0);    //[47]
            triggerBytes.Add(0);    //[48]
            triggerBytes.Add(0);    //[49]
            triggerBytes.Add(0);    //[50]
            triggerBytes.Add(0);    //[51]
            triggerBytes.Add(0);    //[52]
            triggerBytes.Add(0);    //[53]
            triggerBytes.Add(0);    //[54]
            triggerBytes.Add(0);    //[55]
            triggerBytes.Add(0);    //[56]
            triggerBytes.Add(0);    //[57]
            triggerBytes.Add(0);    //[58]
            triggerBytes.Add(0);    //[59]
            triggerBytes.Add(0);    //[60]
            triggerBytes.Add(0);    //[61]
            triggerBytes.Add(0);    //[62]
            triggerBytes.Add(0);    //[63]
            #region if
            triggerBytes.AddRange(GetParameters()); // parameters and mask [0] - [7] / [64] - [71]
            triggerBytes.Add(1); //match count [8] / [72]
            triggerBytes.Add(0x08); // true action MSB 1 [9] / [73]
            triggerBytes.Add(0x0); //[10] / [74]
            if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.I2C) // I2C Trigger
                triggerBytes.Add(0x10); //  [11] / [75] 
            else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.SPI) // SPI Trigger
            {
                if (TriggerModel.GetInstance().IsMOSIChecked)
                    triggerBytes.Add(0x30); //  [11] / [75] 
                else
                    triggerBytes.Add(0x20); //  [11] / [75] 
            }
            else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.UART) // UART Trigger
            {
                // TODO: add relevent code here
                //ConfigModel config = ConfigModel.GetInstance();
                //if (config.SelectedUART_TX != eChannles.None)
                //{
                //    triggerBytes.Add(0x50); //  [11] / [75]
                //}
                //else if (config.SelectedUART_RX != eChannles.None)
                //{
                //    triggerBytes.Add(0x40); //  [11] / [75]
                //}
                //else
                //    triggerBytes.Add(0x50); //  [11] / [75]
            }
            else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.I3C)
                triggerBytes.Add(0x60);
            else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.SPMI)
                triggerBytes.Add(0x70);
            else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.RFFE)
                triggerBytes.Add(0x80);
            else
                triggerBytes.Add(0x0); //  [11] / [75] 

            if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.I2C) // I2C Trigger
            {
                if (TriggerModel.GetInstance().I2CTriggerAtSelected == eI2CTriggerAtList.Address || TriggerModel.GetInstance().I2CTriggerAtSelected == eI2CTriggerAtList.Address_Data)
                {
                    if (TriggerModel.GetInstance().AddressComparison == eComparisonList.Equal_to)
                        triggerBytes.Add(0x01); //[12] / [76]
                    else
                        triggerBytes.Add(0x0); //[12] / [76]
                }
                else if (TriggerModel.GetInstance().I2CTriggerAtSelected == eI2CTriggerAtList.Data || TriggerModel.GetInstance().I2CTriggerAtSelected == eI2CTriggerAtList.Address_Data)
                {
                    if (TriggerModel.GetInstance().AddressComparison == eComparisonList.Equal_to)
                        triggerBytes.Add(0x01); //[12] / [76]
                    else
                        triggerBytes.Add(0x0); //[12] / [76]
                }
                else
                    triggerBytes.Add(0x1); //[12] / [76]
            }
            else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.SPI) // SPI Trigger
            {
                if (TriggerModel.GetInstance().IsMOSIChecked)
                {
                    if (TriggerModel.GetInstance().MOSIComparison == eComparisonList.Equal_to)
                        triggerBytes.Add(0x01);//[12] / [76]
                    else
                        triggerBytes.Add(0x0);//[12] / [76]
                }
                else if (TriggerModel.GetInstance().IsMISOChecked)
                {
                    if (TriggerModel.GetInstance().MISOComparison == eComparisonList.Equal_to)
                        triggerBytes.Add(0x01);//[12] / [76]
                    else
                        triggerBytes.Add(0x0);//[12] / [76]
                }
                else
                    triggerBytes.Add(0x1);//[12] / [76]
            }
            else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.UART)
            {
                if (TriggerModel.GetInstance().IsUARTDataChecked)
                {
                    if (TriggerModel.GetInstance().UARTDataComparison == eComparisonList.Equal_to)
                        triggerBytes.Add(0x1);//[12] / [76]
                    else
                        triggerBytes.Add(0x0);//[12] / [76]
                }
                else
                    triggerBytes.Add(0x1);//[12] / [76]
            }
            else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.I3C)
           {
            //    if (TriggerModel.GetInstance().SelI3CMessage == eI3CMessage.Directed)
            //        triggerBytes.Add(0x3);
            //    else if (TriggerModel.GetInstance().SelI3CMessage == eI3CMessage.Broadcast)
            //    {
            //        if (TriggerModel.GetInstance().SelCommandBrd == eBroadcastCCC.ENTDAA)
            //            triggerBytes.Add(0x3);
            //        else
            //            triggerBytes.Add(0x1);
            //    }
            //    else
            //        triggerBytes.Add(0x1);
            }
            else
                triggerBytes.Add(0x01); //[12] / [76]
            triggerBytes.Add(0x0); //[13] / [77]
            triggerBytes.Add(0x0); //[14] / [78]
            triggerBytes.Add(0x0); //[15] / [79]
            #endregion

            #region Else If
            triggerBytes.Add(0x0); // [80]
            triggerBytes.Add(0x0); // [81]
            triggerBytes.Add(0x0); // [82]
            triggerBytes.Add(0x0); // [83]
            triggerBytes.Add(0x0); // [84]
            triggerBytes.Add(0x0); // [85]
            triggerBytes.Add(0x0); // [86]
            triggerBytes.Add(0x0); // [87]
            triggerBytes.Add(0x0); // [88]
            triggerBytes.Add(0x0); // [89]
            triggerBytes.Add(0x0); // [90]
            triggerBytes.Add(0x0); // [91]
            triggerBytes.Add(0x0); // [92]
            triggerBytes.Add(0x0); // [93]
            triggerBytes.Add(0x0); // [94]
            triggerBytes.Add(0x0); // [95]
            #endregion

            #region Level 1 If
            triggerBytes.Add(0x0); // [96]
            triggerBytes.Add(0x0); // [97]
            triggerBytes.Add(0x0); // [98]
            triggerBytes.Add(0x0); // [99]
            triggerBytes.Add(0x0); // [100]
            triggerBytes.Add(0x0); // [101]
            triggerBytes.Add(0x0); // [102]
            triggerBytes.Add(0x0); // [103]
            triggerBytes.Add(0x0); // [104]
            triggerBytes.Add(0x0); // [105]
            triggerBytes.Add(0x0); // [106]
            triggerBytes.Add(0x0); // [107]
            triggerBytes.Add(0x0); // [108]
            triggerBytes.Add(0x0); // [109]
            triggerBytes.Add(0x0); // [110]
            triggerBytes.Add(0x0); // [111]
            #endregion

            #region Level 1 Else If
            triggerBytes.Add(0x0); // [112]
            triggerBytes.Add(0x0); // [113]
            triggerBytes.Add(0x0); // [114]
            triggerBytes.Add(0x0); // [115]
            triggerBytes.Add(0x0); // [116]
            triggerBytes.Add(0x0); // [117]
            triggerBytes.Add(0x0); // [118]
            triggerBytes.Add(0x0); // [119]
            triggerBytes.Add(0x0); // [120]
            triggerBytes.Add(0x0); // [121]
            triggerBytes.Add(0x0); // [122]
            triggerBytes.Add(0x0); // [123]
            triggerBytes.Add(0x0); // [124]
            triggerBytes.Add(0x0); // [125]
            triggerBytes.Add(0x0); // [126]
            triggerBytes.Add(0x0); // [127]
            #endregion

            //try
            //{
            //    if (overwriteLog)
            //    {
            //        using (FileStream fs = new FileStream(@MiniCooperDirectoryInfo.AppPath + "\\triggerlog.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            //        {
            //            StreamReader sr = new StreamReader(fs);
            //            using (StreamWriter sw = new StreamWriter(fs))
            //            {
            //                // discard the contents of the file by setting the length to 0
            //                fs.SetLength(0);

            //                // write the new content
            //                foreach (var bytes in triggerBytes)
            //                {
            //                    //Log(bytes.ToString(),w);
            //                    sw.Write(bytes.ToString("X2") + " ");
            //                }
            //                sw.WriteLine();

            //            }
            //        }
            //        overwriteLog = false;
            //    }
            //    else
            //    {
            //        FileStream objFilestream = new FileStream(@MiniCooperDirectoryInfo.AppPath + "\\triggerlog.txt", FileMode.Append, FileAccess.Write);
            //        StreamWriter objStreamWriter = new StreamWriter((Stream)objFilestream);
            //        foreach (var bytes in triggerBytes)
            //        {
            //            //Log(bytes.ToString(),w);
            //            objStreamWriter.Write(bytes.ToString("X2") + " ");
            //        }
            //        objStreamWriter.WriteLine();
            //        objStreamWriter.Close();
            //        objFilestream.Close();
            //    }
            //}
            //catch (Exception ex)
            //{

            //}

            return triggerBytes.ToArray();
        }

        private bool overwriteLog;

        public static byte Reverse(byte inByte)
        {
            byte result = 0x00;

            for (byte mask = 0x80; Convert.ToInt32(mask) > 0; mask >>= 1)
            {
                // shift right current result
                result = (byte)(result >> 1);

                // tempbyte = 1 if there is a 1 in the current position
                var tempbyte = (byte)(inByte & mask);
                if (tempbyte != 0x00)
                {
                    // Insert a 1 in the left
                    result = (byte)(result | 0x80);
                }
            }

            return (result);
        }

        internal byte[] GetParameters()
        {
            byte[] returbytes = new byte[8];
            returbytes[4] = 0xFF;
            returbytes[5] = 0xFF;
            returbytes[6] = 0xFF;
            returbytes[7] = 0xFF;

            

            //if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.I2C) // I2C Trigger
            //{
            //  //  return GetI2CTriggerbytes();
            //}
            //else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.SPI) // SPI Trigger
            //{
            //    return GetSPITriggerbytes();
            //}
            //else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.UART) // UART Trigger
            //{
            //    return GetUARTTriggerbytes();
            //}
            //else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.I3C)
            //{
            //    return GetI3CTriggerbytes();
            //}
            //else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.SPMI)
            //{
            //    return GetSPMITriggerbytes();
            //}
            //else if (TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.RFFE)
            //{
            //    return GetRFFETriggerbytes();
            //}
            return returbytes;
        }
        private string GetEnumDescription(Enum enumObj)
        {
            if (enumObj == null)
            {
                return string.Empty;
            }
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
                return attrib.Description;
            }
        }

        private int GetChannelByte(eChannelList SelCh)
        {
            int tempCh = 0;
            switch (SelCh)
            {
                case eChannelList.CH1:
                    tempCh = 0;
                    break;
                case eChannelList.CH2:
                    tempCh = 1;
                    break;
                case eChannelList.CH3:
                    tempCh = 2;
                    break;
                case eChannelList.CH4:
                    tempCh = 3;
                    break;
                case eChannelList.CH5:
                    tempCh = 4;
                    break;
                case eChannelList.CH6:
                    tempCh = 5;
                    break;
                case eChannelList.CH7:
                    tempCh = 6;
                    break;
                case eChannelList.CH8:
                    tempCh = 7;
                    break;
                case eChannelList.CH9:
                    tempCh = 8;
                    break;
                case eChannelList.CH10:
                    tempCh = 9;
                    break;
                case eChannelList.CH11:
                    tempCh = 10;
                    break;
                case eChannelList.CH12:
                    tempCh = 11;
                    break;
                case eChannelList.CH13:
                    tempCh = 12;
                    break;
                case eChannelList.CH14:
                    tempCh = 13;
                    break;
                case eChannelList.CH15:
                    tempCh = 14;
                    break;
                case eChannelList.CH16:
                    tempCh = 15;
                    break;
                default:
                    break;
            }
            return tempCh;
        }

        //private byte[] BinaryToByte(string input)
        //{
        //    int numOfBytes = input.Length / 8;
        //    if (numOfBytes == 0)
        //        return new byte[] { 0};
        //    byte[] bytes = new byte[numOfBytes];
        //    for (int i = 0; i < numOfBytes; ++i)
        //    {
        //        bytes[i] = Convert.ToByte(input.Substring(8 * i, 8), 2);
        //    }
        //    return bytes;
        //}

        private byte[] BinaryToBytePattern(string input, out byte[] mask)
        {
            mask = new byte[2];
            mask[0] = 0xFF;
            mask[1] = 0xFF;
            input = Reverse(input); // Channel 1 value first as per UI.
            string tempStr = input;
            tempStr = tempStr.ToLower();
            for (int i = 0; i < tempStr.Length; i++)
            {
                if (i >= 16)
                    break;
                if (i < 8)
                {
                    if (tempStr[i] != 'x')
                        mask[0] &= (byte)~(1UL << (7 - i));
                }
                else
                {
                    if (tempStr[i] != 'x')
                        mask[1] &= (byte)(~(1UL << (15 - i)));
                }
            }
            StringBuilder sb = new StringBuilder(tempStr);
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] == 'x')
                    sb[i] = '0';
            }
            input = sb.ToString();
            int numOfBytes = input.Length / 8;
            if (input.Length > 8)
                numOfBytes = 2;
            if (numOfBytes == 0)
                return new byte[] { 0 };
            byte[] bytes = new byte[numOfBytes];
            int count = 8;
            for (int i = 0; i < numOfBytes; ++i)
            {
                //if (i == 1)
                //count = 2;
                bytes[i] = Convert.ToByte(input.Substring(8 * i, count), 2);
            }
            return bytes;
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
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

       
        public byte[] GetSPITriggerbytes()
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
            if (TriggerModel.GetInstance().IsMOSIChecked)
            {
                if (TriggerModel.GetInstance().MOSIData.Contains("0x"))
                    data = TriggerModel.GetInstance().MOSIData.Replace("0x", "");
                else if (TriggerModel.GetInstance().MOSIData.Contains("0X"))
                    data = TriggerModel.GetInstance().MOSIData.Replace("0X", "");
                else
                    data = TriggerModel.GetInstance().MOSIData;
                pattern = TriggerModel.GetInstance().MOSIPattern;
            }
            else if (TriggerModel.GetInstance().IsMISOChecked)
            {
                if (TriggerModel.GetInstance().MISOData.Contains("0x"))
                    data = TriggerModel.GetInstance().MISOData.Replace("0x", "");
                else if (TriggerModel.GetInstance().MISOData.Contains("0X"))
                    data = TriggerModel.GetInstance().MISOData.Replace("0X", "");
                else
                    data = TriggerModel.GetInstance().MISOData;
                pattern = TriggerModel.GetInstance().MISOPattern;
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
        public byte[] GetUARTTriggerbytes()
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
            ePatternFormat pattern = TriggerModel.GetInstance().UARTDataPattern;
            data = TriggerModel.GetInstance().UARTDataValue;

            if (data.Contains("0x"))
                data = data.Replace("0x", "");
            else if (data.Contains("0X"))
                data = data.Replace("0X", "");

            if (data != "" && data != null && TriggerModel.GetInstance().IsUARTDataChecked)
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
                        //privateArray[0] = Convert.ToByte(intData);
                        //privateArray[4] = 0;

                        privateArray[2] = Convert.ToByte((intData & 0x80) >> 7);
                        privateArray[6] = 0xFE;

                        privateArray[3] = Convert.ToByte((intData & 0x7F) << 1);
                        privateArray[7] = 0x01;
                    }
                    //if (strData.Count() > 1)
                    //{
                    //    intData = 0;
                    //    if (int.TryParse(strData[1], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                    //    {
                    //        privateArray[1] = Convert.ToByte(intData);
                    //        privateArray[5] = 0;
                    //    }
                    //    if (strData.Count() > 2)
                    //    {
                    //        intData = 0;
                    //        if (int.TryParse(strData[2], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                    //        {
                    //            privateArray[2] = Convert.ToByte(intData);
                    //            privateArray[6] = 0;
                    //        }
                    //        if (strData.Count() > 3)
                    //        {
                    //            intData = 0;
                    //            if (int.TryParse(strData[3], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                    //            {
                    //                privateArray[3] = Convert.ToByte(intData);
                    //                privateArray[7] = 0;
                    //            }
                    //        }
                    //    }
                    //}
                }
            }

            // for start bit in UART.
            //privateArray[2] = Convert.ToByte(privateArray[2] | 0x02);
            //privateArray[6] = Convert.ToByte(privateArray[6] & 0xFD);

            if (TriggerModel.GetInstance().UARTStartRisingSelected)
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
        //public byte[] GetI3CTriggerbytes()
        //{
        //    byte[] privateArray = new byte[8];
        //    //Note: By default set the fields as don't care. On fields present, map the data and mask it
        //    privateArray[4] = 0xFF;
        //    privateArray[5] = 0xFF;
        //    privateArray[6] = 0xFF;
        //    privateArray[7] = 0xFF;
        //    int i3cslaveAddr = -1;
        //    if (!int.TryParse(TriggerModel.GetInstance().I3CSlaveAddress, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out i3cslaveAddr))
        //        i3cslaveAddr = -1;
        //    if (TriggerModel.GetInstance().SelI3CMessage == eI3CMessage.Broadcast)
        //    {
        //        #region Byte Formation - Representation
        //        /*
        //         *      _________________________________________________________________________________________________________________________________
        //         *      |       7      |        6       |       5       |       4       |       3       |       2       |       1       |       0       |
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |                                   Address[6:0]                                                                |       R/W     | [0]
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |   Ack        |                                        Command [7:1]                                                           | [1]
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |  Command[0]  |   Command - T  |                           Data0[7:2]                                                          | [2]
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |           Data0 [1:0]         |   Data0 - T/P |                           Reserved                                            | [3]
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      
        //         *      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //         *                                                      HDR Message
        //         *      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //         *      
        //         *      _________________________________________________________________________________________________________________________________
        //         *      |       7      |        6       |       5       |       4       |       3       |       2       |       1       |       0       |
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |                                   Address[6:0]                                                                |       R/W     | [0]
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |   Ack        |                                        Command [7:1]                                                           | [1]
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |  Command[0]  |   Command - T  |       Preamble[1:0]           |                   DDR Command [15:12]                         | [2]
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |                                                   DDR Command [11:4]                                                          | [3]
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |                       DDR Command [3:0]                       |                   Reserved                                    | 
        //         *      
        //         *      */
        //        #endregion

        //        #region Master Address
        //        privateArray[0] = 0xFC;
        //        privateArray[4] = 0;
        //        privateArray[1] = Convert.ToByte((TriggerModel.GetInstance().AckNckI3C == eAcknowledgeType.ACK ? 0 : 1) << 7);
        //        privateArray[5] = 0x7F;
        //        #endregion

        //        #region Command
        //        if ((int)TriggerModel.GetInstance().SelCommandBrd != -1)
        //        {
        //            privateArray[1] = Convert.ToByte(privateArray[1] | ((int)TriggerModel.GetInstance().SelCommandBrd >> 1));
        //            privateArray[5] = Convert.ToByte(privateArray[5] & 0x80);
        //            privateArray[2] = Convert.ToByte(((int)TriggerModel.GetInstance().SelCommandBrd & 0x1) << 7);
        //            privateArray[6] = 0x7F;
        //        }
        //        #endregion

        //        #region Data
        //        if (TriggerModel.GetInstance().I3CData != null && TriggerModel.GetInstance().I3CData != "")
        //        {
        //            var strData = TriggerModel.GetInstance().I3CData.Trim().Split('-').Take(1).ToList();
        //            int intData;
        //            if (strData.Count() > 0)
        //            {
        //                //Data 0
        //                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
        //                {
        //                    privateArray[2] = Convert.ToByte(privateArray[2] | (intData >> 2));
        //                    privateArray[6] = Convert.ToByte(privateArray[6] & 0xC0);
        //                    privateArray[3] = Convert.ToByte((intData & 0x3) << 6);
        //                    privateArray[7] = 0x3F;
        //                }
        //            }
        //        }
        //        #endregion
        //    }
        //    else if (TriggerModel.GetInstance().SelI3CMessage == eI3CMessage.Directed)
        //    {
        //        #region Byte Formation - Representation
        //        /*
        //         *      _________________________________________________________________________________________________________________________________
        //         *      |       7      |        6       |       5       |       4       |       3       |       2       |       1       |       0       |
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |                                   Address[6:0]                                                                |       R/W     | [0]
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |   Ack        |                                        Command [7:1]                                                           | [1]
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |  Command[0]  |   Command - T  |                           Reserved                                                            | [2]
        //         *      |_______________________________________________________________________________________________________________________________|
        //         *      |       7      |        6       |       5       |       4       |       3       |       2       |       1       |       0       |
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |                                   Address[6:0]                                                                |       R/W     | [0]
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |   Ack        |                                          Data0 [7:1]                                                           | [1]
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |   Data0[0]   |   Data0 - T/P  |                           Data1[7:2]                                                          | [2]
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |           Data1 [1:0]         |   Data1 - T/P |                           Reserved                                            | [3]
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      
        //         *      */
        //        #endregion

        //        #region Master Address
        //        privateArray[0] = 0xFC;
        //        privateArray[4] = 0;
        //        privateArray[1] = Convert.ToByte((TriggerModel.GetInstance().AckNckI3C == eAcknowledgeType.ACK ? 0 : 1) << 7);
        //        privateArray[5] = 0x7F;
        //        #endregion

        //        #region Command
        //        if ((int)TriggerModel.GetInstance().SelCommandDir != -1)
        //        {
        //            privateArray[1] = Convert.ToByte(privateArray[1] | ((int)TriggerModel.GetInstance().SelCommandDir >> 1));
        //            privateArray[5] = Convert.ToByte(privateArray[5] & 0x80);
        //            privateArray[2] = Convert.ToByte(((int)TriggerModel.GetInstance().SelCommandDir & 0x1) << 7);
        //            privateArray[6] = 0x7F;
        //        }
        //        #endregion

        //        #region Slave Address
        //        if (i3cslaveAddr != -1)
        //        {
        //            privateArray[2] = Convert.ToByte(privateArray[2] | i3cslaveAddr >> 2);
        //            privateArray[6] = Convert.ToByte(privateArray[6] & 0xE0);
        //            privateArray[3] = Convert.ToByte(((i3cslaveAddr & 0x3) << 6) | ((int)TriggerModel.GetInstance().I3CSlaveTransfer << 5) | ((int)TriggerModel.GetInstance().I3CSlaveAck << 4));
        //            privateArray[7] = 0xF;
        //        }
        //        #endregion
        //    }
        //    else if (TriggerModel.GetInstance().SelI3CMessage == eI3CMessage.Private)
        //    {
        //        #region Byte Formation - Representation
        //        /*
        //         *      _________________________________________________________________________________________________________________________________
        //         *      |       7      |        6       |       5       |       4       |       3       |       2       |       1       |       0       |
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |                                   Address[6:0]                                                                |       R/W     | [0]
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |   Ack        |                                          Data0 [7:1]                                                           | [1]
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |   Data0[0]   |   Data0 - T/P  |                           Data1[7:2]                                                          | [2]
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      |           Data1 [1:0]         |   Data1 - T/P |                           Reserved                                            | [3]
        //         *      |-------------------------------------------------------------------------------------------------------------------------------|
        //         *      
        //         *      */
        //        #endregion


        //        #region Slave Address
        //        if (i3cslaveAddr != -1)
        //        {
        //            privateArray[0] = Convert.ToByte((i3cslaveAddr << 1) | (TriggerModel.GetInstance().I3CSlaveTransfer == eTransferType.WR ? 0 : 1));
        //            privateArray[4] = 0;
        //            privateArray[1] = Convert.ToByte((TriggerModel.GetInstance().I3CSlaveAck == eAcknowledgeType.ACK ? 0 : 1) << 7);
        //            privateArray[5] = 0x7F;
        //        }
        //        #endregion

        //        #region Data
        //        if (TriggerModel.GetInstance().I3CData != null && TriggerModel.GetInstance().I3CData != "")
        //        {
        //            var strData = TriggerModel.GetInstance().I3CData.Trim().Split('-').Take(2).ToList();
        //            int intData;
        //            if (strData.Count() > 0)
        //            {
        //                //Data 0
        //                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
        //                {
        //                    privateArray[1] = Convert.ToByte(privateArray[1] | (intData >> 1));
        //                    privateArray[5] = Convert.ToByte(privateArray[5] & 0x80);
        //                    privateArray[2] = Convert.ToByte((intData & 0x1) << 7);
        //                    privateArray[6] = 0x7F;
        //                }

        //                //Data 1
        //                if (strData.Count() > 1)
        //                {
        //                    intData = 0;
        //                    if (int.TryParse(strData[1], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
        //                    {
        //                        privateArray[2] = Convert.ToByte(privateArray[2] | (intData >> 2));
        //                        privateArray[6] = Convert.ToByte(privateArray[6] & 0xC0);
        //                        privateArray[3] = Convert.ToByte((intData & 0x3) << 6);
        //                        privateArray[7] = 0x3F;
        //                    }
        //                }
        //            }
        //        }
        //        #endregion
        //    }
        //    return privateArray;
        //}
        public byte[] GetSPMITriggerbytes()
        {
            byte[] privateArray = new byte[8];
            //Note: By default set the fields as don't care. On fields present, map the data and mask it
            privateArray[4] = 0xFF;
            privateArray[5] = 0xFF;
            privateArray[6] = 0xFF;
            privateArray[7] = 0xFF;

            int slaveAddr = -1;
            if (!int.TryParse(TriggerModel.GetInstance().SPMISlaveAddress, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out slaveAddr))
                slaveAddr = -1;

            byte CommandValue = ProtocolInfoRepository.GetSPMICmdId((byte)TriggerModel.GetInstance().SelSPMICommand);
            #region Slave Address
            if (TriggerModel.GetInstance().SelSPMICommand != eSPMICMDTYPE.TRFR_BUS_OWNERSHIP)
            {
                if (TriggerModel.GetInstance().SelSPMICommand == eSPMICMDTYPE.DDB_MA_R
                    || TriggerModel.GetInstance().SelSPMICommand == eSPMICMDTYPE.MASTER_WRITE || TriggerModel.GetInstance().SelSPMICommand == eSPMICMDTYPE.MASTER_READ)
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
            if (!int.TryParse(TriggerModel.GetInstance().SPMIBytecount, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out bytecount))
                bytecount = -1;
            int regAddr = -1;
            if (!int.TryParse(TriggerModel.GetInstance().SPMIregAddr, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out regAddr))
                regAddr = -1;
            switch ((int)TriggerModel.GetInstance().SelSPMICommand)
            {
                case (int)eSPMICMDTYPE.EXT_REG_WRITE:
                    {
                        #region Command
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | ((CommandValue & 0xF0) >> 4));

                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                        }
                        #endregion

                        #region CommandBytecount
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1 && bytecount != -1)
                        {
                            privateArray[1] = Convert.ToByte((bytecount & 0x0F) << 4 | 0x0);
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if (regAddr != -1 && (int)TriggerModel.GetInstance().SelSPMICommand != -1)
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
                        if (TriggerModel.GetInstance().SPMIData != null && TriggerModel.GetInstance().SPMIData != "" && (int)TriggerModel.GetInstance().SelSPMICommand != -1)
                        {
                            var strData = TriggerModel.GetInstance().SPMIData.Trim().Split('-').Take(1).ToList();
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
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | ((CommandValue & 0xF0) >> 4));

                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                        }
                        #endregion

                        #region CommandBytecount
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1 && bytecount != -1)
                        {
                            privateArray[1] = Convert.ToByte((bytecount & 0x0F) << 4 | 0x0);//Include parity bit
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if (regAddr != -1 && (int)TriggerModel.GetInstance().SelSPMICommand != -1)
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
                        if (TriggerModel.GetInstance().SPMIData != null && TriggerModel.GetInstance().SPMIData != "" && (int)TriggerModel.GetInstance().SelSPMICommand != -1)
                        {
                            var strData = TriggerModel.GetInstance().SPMIData.Trim().Split('-').Take(1).ToList();
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
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1) //bc 3 bit
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
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1 && bytecount != -1)
                        {
                            privateArray[1] = Convert.ToByte(0x0 << 7 | (bytecount & 0x07) << 4 | 0x0);
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if (regAddr != -1 && (int)TriggerModel.GetInstance().SelSPMICommand != -1)
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
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1)
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
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1 && bytecount != -1)
                        {
                            privateArray[1] = Convert.ToByte(0x1 << 7 | (bytecount & 0x07) << 4 | 0x0);//Include parity bit
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if (regAddr != -1 && (int)TriggerModel.GetInstance().SelSPMICommand != -1)
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
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1)
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
                        if (regAddr != -1 && (int)TriggerModel.GetInstance().SelSPMICommand != -1)
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
                        if (TriggerModel.GetInstance().SPMIData != null && TriggerModel.GetInstance().SPMIData != "" && (int)TriggerModel.GetInstance().SelSPMICommand != -1)
                        {
                            var strData = TriggerModel.GetInstance().SPMIData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    if (TriggerModel.GetInstance().SPMIData != null && TriggerModel.GetInstance().SPMIData != "")
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
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1)
                        {
                            var ack = (TriggerModel.GetInstance().SPMISlaveAck == eAcknowledgeType.ACK ? 1 : 0);
                            privateArray[2] = Convert.ToByte(privateArray[2] | ack);
                            if (TriggerModel.GetInstance().SPMISlaveAck == eAcknowledgeType.NA)
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
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1)
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
                        if (regAddr != -1 && (int)TriggerModel.GetInstance().SelSPMICommand != -1)
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
                        if (TriggerModel.GetInstance().SPMIData != null && TriggerModel.GetInstance().SPMIData != "" && (int)TriggerModel.GetInstance().SelSPMICommand != -1)
                        {
                            var strData = TriggerModel.GetInstance().SPMIData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    if (TriggerModel.GetInstance().SPMIData != null && TriggerModel.GetInstance().SPMIData != "")
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
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | ((CommandValue & 0x80) >> 4));
                            if (slaveAddr != -1)
                                privateArray[4] = 0x07;
                            else
                                privateArray[4] = 0xF7;
                        }
                        #endregion

                        #region CmdData
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1 && TriggerModel.GetInstance().SPMIData != null && TriggerModel.GetInstance().SPMIData != "")
                        {
                            var strData = TriggerModel.GetInstance().SPMIData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    if ((int)TriggerModel.GetInstance().SelSPMICommand != -1 && TriggerModel.GetInstance().SPMIData != null && TriggerModel.GetInstance().SPMIData != "")
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
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1)
                        {
                            var ack = (TriggerModel.GetInstance().SPMISlaveAck == eAcknowledgeType.ACK ? 1 : 0);
                            privateArray[1] = Convert.ToByte(privateArray[1] | (ack << 1));
                            if (TriggerModel.GetInstance().SPMISlaveAck == eAcknowledgeType.NA)
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
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1)
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
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1)
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
                        if (regAddr != -1 && (int)TriggerModel.GetInstance().SelSPMICommand != -1)
                        {
                            privateArray[1] = Convert.ToByte((privateArray[1]) | (regAddr >> 5));
                            privateArray[2] = Convert.ToByte(((regAddr & 0x1F) << 3) | 0x0);
                            privateArray[5] = 0x08;
                            privateArray[6] = 0x07;
                        }
                        #endregion

                        #region Data
                        if (TriggerModel.GetInstance().SPMIData != null && TriggerModel.GetInstance().SPMIData != "" && (int)TriggerModel.GetInstance().SelSPMICommand != -1)
                        {
                            var strData = TriggerModel.GetInstance().SPMIData.Trim().Split('-').Take(1).ToList();
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
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1)
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
                        if (regAddr != -1 && (int)TriggerModel.GetInstance().SelSPMICommand != -1)
                        {
                            privateArray[1] = Convert.ToByte((privateArray[1]) | (regAddr >> 5));
                            privateArray[2] = Convert.ToByte((regAddr & 0x1F) << 3 | 0x0 << 2 | 0x1 << 1);
                            privateArray[5] = 0x08;
                            privateArray[6] = 0x07;
                        }
                        #endregion

                        #region Data
                        if (TriggerModel.GetInstance().SPMIData != null && TriggerModel.GetInstance().SPMIData != "" && (int)TriggerModel.GetInstance().SelSPMICommand != -1)
                        {
                            var strData = TriggerModel.GetInstance().SPMIData.Trim().Split('-').Take(1).ToList();
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
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1)
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
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1)
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
                        var ack = (TriggerModel.GetInstance().SPMISlaveAck == eAcknowledgeType.ACK) ? 1 : 0;
                        privateArray[1] = Convert.ToByte(privateArray[1] | (ack << 1) | 0x0);
                        if (SessionConfiguration.Version == eVersion.one || TriggerModel.GetInstance().SPMISlaveAck == eAcknowledgeType.NA)
                            privateArray[5] = 0x0F; // ignoring ack for version 1.0.
                        else
                            privateArray[5] = 0x0D;
                        #endregion
                        break;
                    }
                case (int)eSPMICMDTYPE.SLEEP:
                    {
                        #region Command
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1)
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
                        var ack = (TriggerModel.GetInstance().SPMISlaveAck == eAcknowledgeType.ACK) ? 1 : 0;
                        privateArray[1] = Convert.ToByte(privateArray[1] | (ack << 1) | 0x0);
                        if (SessionConfiguration.Version == eVersion.one || TriggerModel.GetInstance().SPMISlaveAck == eAcknowledgeType.NA)
                            privateArray[5] = 0x0F; // ignoring ack for version 1.0.
                        else
                            privateArray[5] = 0x0D;
                        #endregion
                        break;
                    }
                case (int)eSPMICMDTYPE.SHUTDOWN:
                    {
                        #region Command
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1)
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
                        var ack = (TriggerModel.GetInstance().SPMISlaveAck == eAcknowledgeType.ACK) ? 1 : 0;
                        privateArray[1] = Convert.ToByte(privateArray[1] | (ack << 1) | 0x0);
                        if (SessionConfiguration.Version == eVersion.one || TriggerModel.GetInstance().SPMISlaveAck == eAcknowledgeType.NA)
                            privateArray[5] = 0x0F; // ignoring ack for version 1.0.
                        else
                            privateArray[5] = 0x0D;
                        #endregion
                        break;
                    }
                case (int)eSPMICMDTYPE.WAKEUP:
                    {
                        #region Command
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1)
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
                        var ack = (TriggerModel.GetInstance().SPMISlaveAck == eAcknowledgeType.ACK) ? 1 : 0;
                        privateArray[1] = Convert.ToByte(privateArray[1] | (ack << 1) | 0x0);
                        if (SessionConfiguration.Version == eVersion.one || TriggerModel.GetInstance().SPMISlaveAck == eAcknowledgeType.NA)
                            privateArray[5] = 0x0F; // ignoring ack for version 1.0.
                        else
                            privateArray[5] = 0x0D;
                        #endregion
                        break;
                    }
                case (int)eSPMICMDTYPE.DDB_SL_R:
                    {
                        #region Command
                        if ((int)TriggerModel.GetInstance().SelSPMICommand != -1)
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
                        if (TriggerModel.GetInstance().SPMIData != null && TriggerModel.GetInstance().SPMIData != "")
                        {
                            var strData = TriggerModel.GetInstance().SPMIData.Trim().Split('-').Take(1).ToList();
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
        public byte[] GetRFFETriggerbytes()
        {
            byte[] privateArray = new byte[8];
            //Note: By default set the fields as don't care. On fields present, map the data and mask it
            privateArray[4] = 0xFF;
            privateArray[5] = 0xFF;
            privateArray[6] = 0xFF;
            privateArray[7] = 0xFF;

            int slaveAddr = -1;
            if (!int.TryParse(TriggerModel.GetInstance().RFFESlaveAddress, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out slaveAddr))
                slaveAddr = -1;
            int bytecount = -1;
            if (!int.TryParse(TriggerModel.GetInstance().RFFEBytecount, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out bytecount))
                bytecount = -1;
            int regAddr = -1;
            if (!int.TryParse(TriggerModel.GetInstance().RFFEregAddr, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out regAddr))
                regAddr = -1;
            byte CommandValue = ProtocolInfoRepository.GetRFFECmdId((byte)TriggerModel.GetInstance().SelRFFECommand);
            //Note: By default set the fields as don't care. On fields present, map the data and mask it

            #region Slave Address
            if ((int)TriggerModel.GetInstance().SelRFFECommand == (int)eRFFECMDTYPE.MASTER_CXT_TRANSFER_WRITE || (int)TriggerModel.GetInstance().SelRFFECommand == (int)eRFFECMDTYPE.MASTER_CXT_TRANSFER_READ || (int)TriggerModel.GetInstance().SelRFFECommand == (int)eRFFECMDTYPE.MASTER_OWNERSHIP_HANDOVER
                || (int)TriggerModel.GetInstance().SelRFFECommand == (int)eRFFECMDTYPE.MASTER_WRITE || (int)TriggerModel.GetInstance().SelRFFECommand == (int)eRFFECMDTYPE.MASTER_READ)
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

            switch ((int)TriggerModel.GetInstance().SelRFFECommand)
            {
                case (int)eRFFECMDTYPE.EXT_REG_WRITE:
                    {
                        #region Command
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | ((CommandValue & 0xF0) >> 4));

                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                        }
                        #endregion

                        #region CommandBytecount
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1 && bytecount != -1)
                        {
                            privateArray[1] = Convert.ToByte((bytecount & 0x0F) << 4 | 0x0);
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if (regAddr != -1 && (int)TriggerModel.GetInstance().SelRFFECommand != -1)
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
                        if (TriggerModel.GetInstance().RFFEData != null && TriggerModel.GetInstance().RFFEData != "")
                        {
                            var strData = TriggerModel.GetInstance().RFFEData.Trim().Split('-').Take(1).ToList();
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
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | ((CommandValue & 0xF0) >> 4));

                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;
                        }
                        #endregion

                        #region CommandBytecount
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1 && bytecount != -1)
                        {
                            privateArray[1] = Convert.ToByte((bytecount & 0x0F) << 4 | 0x0);//Include parity bit
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if (regAddr != -1 && (int)TriggerModel.GetInstance().SelRFFECommand != -1)
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
                        if (TriggerModel.GetInstance().RFFEData != null && TriggerModel.GetInstance().RFFEData != "")
                        {
                            var strData = TriggerModel.GetInstance().RFFEData.Trim().Split('-').Take(1).ToList();
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
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1) //bc 3 bit
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
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1 && bytecount != -1)
                        {
                            privateArray[1] = Convert.ToByte(0x0 << 7 | (bytecount & 0x07) << 4 | 0x0);
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1 && regAddr != -1)
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
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1)
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
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1 && bytecount != -1)
                        {
                            privateArray[1] = Convert.ToByte(0x1 << 7 | (bytecount & 0x07) << 4 | 0x0);//Include parity bit
                            privateArray[5] = 0x0F;
                        }
                        #endregion

                        #region Register Address
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1 && regAddr != -1)
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
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1)
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
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1 && regAddr != -1)
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
                        if (TriggerModel.GetInstance().RFFEData != null && TriggerModel.GetInstance().RFFEData != "")
                        {
                            var strData = TriggerModel.GetInstance().RFFEData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    if (TriggerModel.GetInstance().RFFEData != null && TriggerModel.GetInstance().RFFEData != "")
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
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1)
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
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1 && regAddr != -1)
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
                        if (TriggerModel.GetInstance().RFFEData != null && TriggerModel.GetInstance().RFFEData != "")
                        {
                            var strData = TriggerModel.GetInstance().RFFEData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    if (TriggerModel.GetInstance().RFFEData != null && TriggerModel.GetInstance().RFFEData != "")
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
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | ((CommandValue & 0x80) >> 4));
                            if (slaveAddr != -1)
                                privateArray[4] = 0x07;
                            else
                                privateArray[4] = 0xF7;
                        }
                        #endregion

                        #region CmdData
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1 && TriggerModel.GetInstance().RFFEData != null && TriggerModel.GetInstance().RFFEData != "")
                        {
                            var strData = TriggerModel.GetInstance().RFFEData.Trim().Split('-').Take(1).ToList();
                            int intData;
                            if (strData.Count() > 0)
                            {
                                //Data 0
                                if (int.TryParse(strData[0], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out intData))
                                {
                                    if ((int)TriggerModel.GetInstance().SelRFFECommand != -1 && TriggerModel.GetInstance().RFFEData != null && TriggerModel.GetInstance().RFFEData != "")
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
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | (CommandValue & 0xF0) >> 4);
                            privateArray[1] = Convert.ToByte((CommandValue & 0x0F) << 4 | 0x0 << 3 | 0x1 << 2 | (TriggerModel.GetInstance().RFFEISIEnable ? 0x1 : 0x0) << 1);
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;

                            if (TriggerModel.GetInstance().RFFEISIEnable)
                                privateArray[5] = 0x0D;
                            else
                                privateArray[5] = 0x0F;

                            if (TriggerModel.GetInstance().RFFEISIEnable == true && TriggerModel.GetInstance().RFFEInterrupt == eInterruptSlot.INT15)
                            {
                                privateArray[1] = Convert.ToByte(privateArray[1] | 0x01);
                                privateArray[5] = 0x0C;
                            }

                            int word1 = 0x0;
                            int word2 = 0x0;
                            int word1Mask = 0xFF;
                            int word2Mask = 0xFF;

                            #region InterruptSlot
                            switch (TriggerModel.GetInstance().RFFEInterrupt)
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
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1)
                        {
                            privateArray[0] = Convert.ToByte((privateArray[0]) | (CommandValue & 0xF0) >> 4);
                            privateArray[1] = Convert.ToByte((CommandValue & 0x0F) << 4 | 0x0 << 3 | 0x1 << 2 | (TriggerModel.GetInstance().RFFESlaveAck == eAcknowledgeType.ACK ? 0x1 : 0x0) << 1 | 0x0);
                            if (slaveAddr != -1)
                                privateArray[4] = 0x0;
                            else
                                privateArray[4] = 0xF0;

                            if (TriggerModel.GetInstance().RFFESlaveAck == eAcknowledgeType.ACK)
                                privateArray[5] = 0x0D;
                            else
                                privateArray[5] = 0x0F;

                            if (TriggerModel.GetInstance().RFFERxMID != -1 && TriggerModel.GetInstance().RFFESlaveAck == eAcknowledgeType.ACK)
                            {
                                // privateArray[1] = Convert.ToByte(privateArray[1] | (0x0));
                                // privateArray[5] = 0x01;
                                privateArray[2] = Convert.ToByte(0x0 << 7 | (TriggerModel.GetInstance().RFFERxMID & 0x3) << 5 | 0x1);
                                privateArray[6] = 0x9F;
                            }
                        }
                        #endregion

                        break;
                    }

                case (int)eRFFECMDTYPE.MASKED_WRITE:
                    {
                        #region Command
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1)
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
                        if (TriggerModel.GetInstance().RFFEMaskData != null && TriggerModel.GetInstance().RFFEMaskData != "")
                        {
                            var strData = TriggerModel.GetInstance().RFFEMaskData.Trim().Split('-').Take(1).ToList();
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
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1)
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
                        if (TriggerModel.GetInstance().RFFEData != null && TriggerModel.GetInstance().RFFEData != "")
                        {
                            var strData = TriggerModel.GetInstance().RFFEData.Trim().Split('-').Take(1).ToList();
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
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1)
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
                        if (TriggerModel.GetInstance().RFFEData != null && TriggerModel.GetInstance().RFFEData != "")
                        {
                            var strData = TriggerModel.GetInstance().RFFEData.Trim().Split('-').Take(1).ToList();
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
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1)
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
                        if ((int)TriggerModel.GetInstance().SelRFFECommand != -1)
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