using PGYMiniCooper.DataModule.Model.Trigger_Config;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Interfaces;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using ProdigyFramework.ComponentModel;
using ProdigyFramework.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace PGYMiniCooper.CoreModule.ViewModel
{
    [Serializable]
    public class TriggerViewModel : ViewModelBase
    {
        #region Constructor

        /// <summary>
        /// Serialization only
        /// </summary>
        public TriggerViewModel() : this(TriggerModel.GetInstance())
        {

        }

        public TriggerViewModel(TriggerModel triggerModel)
        {
            this.triggerModel = triggerModel;

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
        public IEnumerable<eQSPICommands> QSPICommandList
        {
            get
            {
                return Enum.GetValues(typeof(eQSPICommands)).Cast<eQSPICommands>();
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

        public eTriggerTypeList TriggerType
        {
            get { return triggerModel.TriggerType; }
            set
            {

                triggerModel.TriggerType = value;
                RaisePropertyChanged("TriggerType");
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

        private IConfigViewModel protocolSelection;

        public IConfigViewModel ProtocolSelection
        {
            get { return protocolSelection; }
            set
            {
                if(value!=null)
                triggerModel.ProtocolSelection = value.Model;

                protocolSelection = value;
                ProtocolSelectionMethod();
                RaisePropertyChanged(nameof(ProtocolSelection));
                
                //if (protocolSelection is ConfigViewModel_I3C)
                //{
                //    SelectedTrigger = new Trigger_Config.TriggerConfig_I3C();
                //}

            }
        }


        public void ProtocolSelectionMethod()
        {

            switch (protocolSelection)
            {
                case ConfigViewModel_I2C configViewModel_I2C:
                    SelectedTrigger = new TriggerConfig_I2C();
                    break;
                case ConfigViewModel_I3C configViewModel_I3C:
                    SelectedTrigger = new TriggerConfig_I3C();
                    break;
                case ConfigViewModel_SPI configViewModel_SPI:
                    SelectedTrigger = new TriggerConfig_SPI();
                    break;
                case ConfigViewModel_UART configViewModel_UART:
                    SelectedTrigger = new TriggerConfig_UART();
                    break;
                case ConfigViewModel_SPMI configViewModel_SPMI:
                    SelectedTrigger = new TriggerConfig_SPMI((dynamic)configViewModel_SPMI.Model); 
                    break;
                case ConfigViewModel_RFFE configViewModel_RFFE:
                    SelectedTrigger = new TriggerConfig_RFFE();
                    break;
                case ConfigViewModel_QSPI configViewModel_QSPI:
                    SelectedTrigger = new TriggerConfig_QSPI();
                    break;
                case ConfigViewModel_CAN configViewModel_CAN:
                    SelectedTrigger = new TriggerConfig_CAN((dynamic)configViewModel_CAN.Model);
                    break;
            }
        }

        public ITriggerConfigViewModel SelectedTrigger
        {
            get { return triggerModel.SelectedTrigger; }
            set
            {
                triggerModel.SelectedTrigger = value;
                RaisePropertyChanged(nameof(SelectedTrigger));
            }
        }

        private List<eBufferSize> bufferSizeList;

        public List<eBufferSize> BufferSizeList { get { return bufferSizeList; } set { bufferSizeList = value; RaisePropertyChanged("BufferSizeList"); } }


        private TriggerModel triggerModel;

        [XmlIgnore]
        public TriggerModel TriggerModel
        {
            get
            {
                return triggerModel;
            }
            set
            {
                triggerModel = value;
                RaisePropertyChanged("TriggerModel");
            }
        }

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
            if (TriggerModel.TriggerType == eTriggerTypeList.Auto)
            {
                return null;
            }
            if (TriggerModel.TriggerType == eTriggerTypeList.Pattern || TriggerModel.TriggerType == eTriggerTypeList.Timing)
            {
                triggerBytes.Add(0x15);//[42] // Pkt type
                triggerBytes.Add(0x0);    //[43] // Pkt len
                triggerBytes.Add(0);    //[44] // Pkt len
                byte temp = 0;
                if (TriggerModel.TriggerType == eTriggerTypeList.Timing)
                {
                    if (TriggerModel.TimingPulsePositiveSelected && TriggerModel.TimingTriggerTypeSelected == eTimingTriggerTypeList.Pulse_Width)
                        temp |= 0x80;
                    if (TriggerModel.PulseComparisonSelected == ePulseComparisonList.Greater_than && TriggerModel.TimingTriggerTypeSelected == eTimingTriggerTypeList.Pulse_Width)
                        temp |= 0x40;
                    if (TriggerModel.DelayComparisonSelected == ePulseComparisonList.Greater_than && TriggerModel.TimingTriggerTypeSelected == eTimingTriggerTypeList.Delay)
                        temp |= 0x10;
                    if (TriggerModel.TimingTriggerTypeSelected == eTimingTriggerTypeList.Delay)
                        temp |= 0x08;
                    if (TriggerModel.TimingTriggerTypeSelected == eTimingTriggerTypeList.Pulse_Width)
                        temp |= 0x04;
                }
                if (TriggerModel.TriggerType == eTriggerTypeList.Pattern)
                    temp |= 0x02;
                temp |= 0x20;
                triggerBytes.Add(temp);    //[45]
                if (TriggerModel.TimingTriggerTypeSelected == eTimingTriggerTypeList.Pulse_Width && TriggerModel.TriggerType == eTriggerTypeList.Timing)
                {
                    int count = 0;
                    int.TryParse(TriggerModel.PulseWidthCount, out count);
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
                if (TriggerModel.TimingTriggerTypeSelected == eTimingTriggerTypeList.Delay && TriggerModel.TriggerType == eTriggerTypeList.Timing)
                {
                    int count = 0;
                    int.TryParse(TriggerModel.DelayCount, out count);
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
                if (TriggerModel.TimingTriggerTypeSelected == eTimingTriggerTypeList.Pulse_Width && TriggerModel.TriggerType == eTriggerTypeList.Timing)
                    triggerBytes.Add(Convert.ToByte(GetChannelByte(TriggerModel.PulseWidthChannel) << 4));    //[50] // Pulse width trigger channel select
                else
                    triggerBytes.Add(0); //[50]

                if (TriggerModel.TimingTriggerTypeSelected == eTimingTriggerTypeList.Delay && TriggerModel.TriggerType == eTriggerTypeList.Timing)
                    triggerBytes.Add(Convert.ToByte((GetChannelByte(TriggerModel.DelayChannel1) << 4) | (GetChannelByte(TriggerModel.DelayChannel2))));    //[51] // Delay channel 1 , 2
                else
                    triggerBytes.Add(0); //[51]

                try
                {
                    if (TriggerModel.PatternFormat == ePatternFormat.Binary)
                    {
                        byte[] tempArray = BinaryToBytePattern(TriggerModel.PatternText, out byte[] mask);
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
                    else if (TriggerModel.PatternFormat == ePatternFormat.Hex)
                    {
                        if (TriggerModel.PatternText.Contains("0x"))
                            TriggerModel.PatternText = TriggerModel.PatternText.Replace("0x", "");
                        else if (TriggerModel.PatternText.Contains("0X"))
                            TriggerModel.PatternText = TriggerModel.PatternText.Replace("0X", "");
                        int intPattern = 0;
                        int.TryParse(TriggerModel.PatternText, System.Globalization.NumberStyles.HexNumber, null, out intPattern);
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
                    else if (TriggerModel.PatternFormat == ePatternFormat.Decimal)
                    {
                        int intPattern;
                        int.TryParse(TriggerModel.PatternText, out intPattern);
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
                    else if (TriggerModel.PatternFormat == ePatternFormat.Octal)
                    {
                        int intPattern = 0;
                        if (TriggerModel.PatternText != string.Empty)
                            intPattern = Convert.ToInt32(TriggerModel.PatternText, 8);
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
            if( (this.ProtocolSelection.ProtocolType == eProtocol.CAN) || (this.ProtocolSelection.ProtocolType == eProtocol.QSPI))
            {
                triggerBytes.Add(0x28);
            }
            else
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
            //triggerBytes.AddRange(GetParameters()); // parameters and mask [0] - [7] / [64] - [71]   //64-95
            triggerBytes.Add(1); //match count [8] / [72]
            triggerBytes.Add(0x08); // true action MSB 1 [9] / [73]
            triggerBytes.Add(0x0); //[10] / [74]
            if (this.protocolSelection != null)
            {
                if (this.ProtocolSelection.ProtocolType == eProtocol.I2C) // I2C Trigger
                    triggerBytes.Add(0x10); //  [11] / [75] 
                else if (this.ProtocolSelection.ProtocolType == eProtocol.SPI) // SPI Trigger
                {
                    TriggerConfig_SPI trigger = (TriggerConfig_SPI)SelectedTrigger;
                    if (trigger.IsMOSIChecked)
                        triggerBytes.Add(0x30); //  [11] / [75] 
                    else
                        triggerBytes.Add(0x20); //  [11] / [75] 
                }
                else if (this.ProtocolSelection.ProtocolType == eProtocol.UART) // UART Trigger
                {
                    var uartConfig = (ConfigViewModel_UART)this.ProtocolSelection;
                    if (uartConfig.ChannelIndex_TX != eChannles.None)
                    {
                        triggerBytes.Add(0x50); //  [11] / [75]
                    }
                    else if (uartConfig.ChannelIndex_RX != eChannles.None)
                    {
                        triggerBytes.Add(0x40); //  [11] / [75]
                    }
                    else
                        triggerBytes.Add(0x50); //  [11] / [75]
                }
                else if (this.ProtocolSelection.ProtocolType == eProtocol.I3C)
                    triggerBytes.Add(0x60);
                else if (this.ProtocolSelection.ProtocolType == eProtocol.SPMI)
                    triggerBytes.Add(0x70);
                else if (this.ProtocolSelection.ProtocolType == eProtocol.RFFE)
                    triggerBytes.Add(0x80);
                else if (this.ProtocolSelection.ProtocolType == eProtocol.CAN)
                    triggerBytes.Add(0xA0);
                else if (this.ProtocolSelection.ProtocolType == eProtocol.QSPI)
                    triggerBytes.Add(0xC0);
                else
                    triggerBytes.Add(0x0); //  [11] / [75] 

                if (this.ProtocolSelection.ProtocolType == eProtocol.I2C) // I2C Trigger
                {
                    TriggerConfig_I2C trigger = (TriggerConfig_I2C)SelectedTrigger;
                    if (trigger.I2CTriggerAtSelected == eI2CTriggerAtList.Address || trigger.I2CTriggerAtSelected == eI2CTriggerAtList.Address_Data)
                    {
                        if (TriggerModel.AddressComparison == eComparisonList.Equal_to)
                            triggerBytes.Add(0x01); //[12] / [76]
                        else
                            triggerBytes.Add(0x0); //[12] / [76]
                    }
                    else if (trigger.I2CTriggerAtSelected == eI2CTriggerAtList.Data || trigger.I2CTriggerAtSelected == eI2CTriggerAtList.Address_Data)
                    {
                        if (TriggerModel.AddressComparison == eComparisonList.Equal_to)
                            triggerBytes.Add(0x01); //[12] / [76]
                        else
                            triggerBytes.Add(0x0); //[12] / [76]
                    }
                    else
                        triggerBytes.Add(0x1); //[12] / [76]
                }
                else if (this.ProtocolSelection.ProtocolType == eProtocol.SPI) // SPI Trigger
                {
                    TriggerConfig_SPI trigger = (TriggerConfig_SPI)SelectedTrigger;
                    if (trigger.IsMOSIChecked)
                    {
                        if (trigger.MOSIComparison == eComparisonList.Equal_to)
                            triggerBytes.Add(0x01);//[12] / [76]
                        else
                            triggerBytes.Add(0x0);//[12] / [76]
                    }
                    else if (trigger.IsMISOChecked)
                    {
                        if (trigger.MISOComparison == eComparisonList.Equal_to)
                            triggerBytes.Add(0x01);//[12] / [76]
                        else
                            triggerBytes.Add(0x0);//[12] / [76]
                    }
                    else
                        triggerBytes.Add(0x1);//[12] / [76]
                }
                else if (this.ProtocolSelection.ProtocolType == eProtocol.UART)
                {
                    TriggerConfig_UART trigger = (TriggerConfig_UART)SelectedTrigger;
                    if (trigger.IsUARTDataChecked)
                    {
                        if (trigger.UARTDataComparison == eComparisonList.Equal_to)
                            triggerBytes.Add(0x1);//[12] / [76]
                        else
                            triggerBytes.Add(0x0);//[12] / [76]
                    }
                    else
                        triggerBytes.Add(0x1);//[12] / [76]
                }
                else if (this.ProtocolSelection.ProtocolType == eProtocol.I3C)
                {
                    TriggerConfig_I3C trigger = (TriggerConfig_I3C)SelectedTrigger;

                    if (trigger.SelI3CMessage == eI3CMessage.Directed)
                        triggerBytes.Add(0x3);
                    else if (trigger.SelI3CMessage == eI3CMessage.Broadcast)
                    {
                        if (trigger.SelCommandBrd == eBroadcastCCC.ENTDAA)
                            triggerBytes.Add(0x3);
                        else
                            triggerBytes.Add(0x1);
                    }
                    else
                        triggerBytes.Add(0x1);
                }
                else
                    triggerBytes.Add(0x01); //[12] / [76]
            }
            else
            {
                triggerBytes.Add(0x0);
                triggerBytes.Add(0x01);
            }
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

     

    }
}
