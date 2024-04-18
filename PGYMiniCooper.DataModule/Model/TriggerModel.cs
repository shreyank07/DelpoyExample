using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Interfaces;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Model
{
    public class TriggerModel : ViewModelBase
    {
        private TriggerModel() { }

        static TriggerModel _instance = null;
        public static TriggerModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new TriggerModel();
            }

            return _instance;
        }

        private ITriggerConfigViewModel selectedTrigger;

        public ITriggerConfigViewModel SelectedTrigger
        {
            get { return selectedTrigger; }
            set
            {
                selectedTrigger = value;
            }
        }

        private ePatternFormat patternFormat = ePatternFormat.Hex;

        public ePatternFormat PatternFormat { get { return patternFormat; } set { patternFormat = value; ChangText("pattern", value); RaisePropertyChanged("PatternFormat"); } }

        public int TriggerTypeIndex { get { return triggerTypeIndex; } set { triggerTypeIndex = value; RaisePropertyChanged("TriggerTypeIndex"); } }

        private int triggerTypeIndex = 0;

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

            if (type == "pattern")
            {
                PatternText = retval;
            }
            else if (type == "i2c_addr")
            {
                AddressValue = retval;
            }
            else if (type == "i2c_data")
            {
                DataValue = retval;
            }
            else if (type == "spi_mosi")
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
            else if (type == "uart_data")
            {
                UARTDataValue = retval;
            }
        }


        public eProtocolTypeList ProtocolTypeIndex
        {
            get { return protocolTypeIndex; }
            set
            {
                protocolTypeIndex = value;
                RaisePropertyChanged("ProtocolTypeIndex");
            }
        }

        private eProtocolTypeList protocolTypeIndex = eProtocolTypeList.I2C;

        public int TriggerPosition { get { return triggerPosition; } set { triggerPosition = value; RaisePropertyChanged("TriggerPosition"); } }

        private int triggerPosition = 25;

        public eI2CTriggerAtList I2CTriggerAtSelected { get { return i2CTriggerAtSelected; } set { i2CTriggerAtSelected = value; RaisePropertyChanged("I2CTriggerAtSelected"); } }

        private eI2CTriggerAtList i2CTriggerAtSelected = eI2CTriggerAtList.Start;

        public eChannelList PulseWidthChannel { get { return pulseWidthChannel; } set { pulseWidthChannel = value; RaisePropertyChanged("PulseWidthChannel"); } }

        private eChannelList pulseWidthChannel = eChannelList.CH1;

        public eTimingTriggerTypeList TimingTriggerTypeSelected
        {
            get { return timingTriggerTypeSelected; }
            set
            {
                timingTriggerTypeSelected = value;
                RaisePropertyChanged("TimingTriggerTypeSelected");
            }
        }

        private eTimingTriggerTypeList timingTriggerTypeSelected = eTimingTriggerTypeList.Pulse_Width;

        public ePulseComparisonList PulseComparisonSelected { get { return pulseComparisonSelected; } set { pulseComparisonSelected = value; RaisePropertyChanged("PulseComparisonSelected"); } }

        private ePulseComparisonList pulseComparisonSelected = ePulseComparisonList.Greater_than;

        public ePulseComparisonList DelayComparisonSelected { get { return delayComparisonSelected; } set { delayComparisonSelected = value; RaisePropertyChanged("DelayComparisonSelected"); } }

        private ePulseComparisonList delayComparisonSelected = ePulseComparisonList.Greater_than;

        public eBufferSize BufferSize { get { return bufferSize; } set { bufferSize = value; RaisePropertyChanged("BufferSize"); } }

        private eBufferSize bufferSize = eBufferSize._100KB;

        public string MOSIData { get { return mOSIData; } set { mOSIData = value; RaisePropertyChanged("MOSIData"); } }

        private string mOSIData = "0x00";

        public string MISOData { get { return mISOData; } set { mISOData = value; RaisePropertyChanged("MISOData"); } }

        private string mISOData = "0x00";

        public eComparisonList AddressComparison { get { return addressComparison; } set { addressComparison = value; RaisePropertyChanged("AddressComparison"); } }

        private eComparisonList addressComparison = eComparisonList.Equal_to;

        public eComparisonList DataComparison { get { return dataComparison; } set { dataComparison = value; RaisePropertyChanged("DataComparison"); } }

        private eComparisonList dataComparison = eComparisonList.Equal_to;

        public ePatternFormat AddressPattern { get { return addressPattern; } set { addressPattern = value; ChangText("i2c_addr", value); RaisePropertyChanged("AddressPattern"); } }

        private ePatternFormat addressPattern = ePatternFormat.Hex;

        public string AddressValue { get { return addressValue; } set { addressValue = value; RaisePropertyChanged("AddressValue"); } }

        private string addressValue = "0x00";
        public ePatternFormat DataPattern { get { return dataPattern; } set { dataPattern = value; ChangText("i2c_data", value); RaisePropertyChanged("DataPattern"); } }

        private ePatternFormat dataPattern = ePatternFormat.Hex;

        public string DataValue { get { return dataValue; } set { dataValue = value; RaisePropertyChanged("DataValue"); } }

        private string dataValue = "0x00";

        private bool activeLowCS = true;
        public bool ActiveLowCS { get { return activeLowCS; } set { activeLowCS = value; RaisePropertyChanged("ActiveLowCS"); } }

        private bool activeHighCS = false;
        public bool ActiveHighCS { get { return activeHighCS; } set { activeHighCS = value; RaisePropertyChanged("ActiveHighCS"); } }

        private bool risingClock = true;
        public bool RisingClock { get { return risingClock; } set { risingClock = value; RaisePropertyChanged("RisingClock"); } }

        private bool fallingClock = false;
        public bool FallingClock { get { return fallingClock; } set { fallingClock = value; RaisePropertyChanged("FallingClock"); } }

        private bool uARTAutoDetectSelected = false;
        public bool UARTAutoDetectSelected { get { return uARTAutoDetectSelected; } set { uARTAutoDetectSelected = value; RaisePropertyChanged("UARTAutoDetectSelected"); } }

        private bool uARTStartFallingSelected = false;
        public bool UARTStartFallingSelected { get { return uARTStartFallingSelected; } set { uARTStartFallingSelected = value; RaisePropertyChanged("UARTStartFallingSelected"); } }

        private bool uARTStartRisingSelected = true;
        public bool UARTStartRisingSelected { get { return uARTStartRisingSelected; } set { uARTStartRisingSelected = value; RaisePropertyChanged("UARTStartRisingSelected"); } }

        public string UARTDataValue { get { return uARTDataValue; } set { uARTDataValue = value; RaisePropertyChanged("UARTDataValue"); } }

        private string uARTDataValue = "0x00";

        public eComparisonList MOSIComparison { get { return mOSIComparison; } set { mOSIComparison = value; RaisePropertyChanged("MOSIComparison"); } }

        private eComparisonList mOSIComparison = eComparisonList.Equal_to;

        public eComparisonList MISOComparison { get { return mISOComparison; } set { mISOComparison = value; RaisePropertyChanged("MISOComparison"); } }

        private eComparisonList mISOComparison = eComparisonList.Equal_to;

        public ePatternFormat MOSIPattern { get { return mOSIPattern; } set { mOSIPattern = value; ChangText("spi_mosi", value); RaisePropertyChanged("MOSIPattern"); } }

        private ePatternFormat mOSIPattern = ePatternFormat.Hex;

        public ePatternFormat MISOPattern { get { return mISOPattern; } set { mISOPattern = value; ChangText("spi_miso", value); RaisePropertyChanged("MISOPattern"); } }

        private ePatternFormat mISOPattern = ePatternFormat.Hex;

        private bool isMOSIChecked = true;

        public bool IsMOSIChecked { get { return isMOSIChecked; } set { isMOSIChecked = value; RaisePropertyChanged("IsMOSIChecked"); } }

        private bool isMISOChecked = false;

        public bool IsMISOChecked { get { return isMISOChecked; } set { isMISOChecked = value; RaisePropertyChanged("IsMISOChecked"); } }

        public eComparisonList UARTDataComparison { get { return uARTDataComparison; } set { uARTDataComparison = value; RaisePropertyChanged("UARTDataComparison"); } }

        private eComparisonList uARTDataComparison = eComparisonList.Equal_to;

        public ePatternFormat UARTDataPattern { get { return uARTDataPattern; } set { uARTDataPattern = value; ChangText("uart_data", value); RaisePropertyChanged("UARTDataPattern"); } }

        private ePatternFormat uARTDataPattern = ePatternFormat.Hex;

        public bool IsUARTDataChecked { get { return isUARTDataChecked; } set { isUARTDataChecked = value; RaisePropertyChanged("IsUARTDataChecked"); } }

        private bool isUARTDataChecked = false;

        private bool timingPulsePositiveSelected = true;

        public bool TimingPulsePositiveSelected { get { return timingPulsePositiveSelected; } set { timingPulsePositiveSelected = value; RaisePropertyChanged("TimingPulsePositiveSelected"); } }

        private bool timingPulseNegativeSelected = false;

        public bool TimingPulseNegativeSelected { get { return timingPulseNegativeSelected; } set { timingPulseNegativeSelected = value; RaisePropertyChanged("TimingPulseNegativeSelected"); } }

        public eChannelList DelayChannel1 { get { return delayChannel1; } set { delayChannel1 = value; RaisePropertyChanged("DelayChannel1"); } }

        private eChannelList delayChannel1 = eChannelList.CH1;

        public eChannelList DelayChannel2 { get { return delayChannel2; } set { delayChannel2 = value; RaisePropertyChanged("DelayChannel2"); } }

        private eChannelList delayChannel2 = eChannelList.CH2;

        public string PatternText { get { return patternText; } set { patternText = value; RaisePropertyChanged("PatternText"); /*GetTriggerBytes();*/ } }

        private string patternText = "0x00";

        public string PulseWidthCount { get { return pulseWidthCount; } set { pulseWidthCount = value; RaisePropertyChanged("PulseWidthCount"); } }

        private string pulseWidthCount = "1000";

        public string DelayCount { get { return delayCount; } set { delayCount = value; RaisePropertyChanged("DelayCount"); } }

        private string delayCount = "1000";

        private eTransferType transferType = eTransferType.WR;

        public eTransferType TransferType { get { return transferType; } set { transferType = value; RaisePropertyChanged("TransferType"); } }

        private bool autoMenuSelected = false;
        public bool AutoMenuSelected
        {
            get { return autoMenuSelected; }
            set
            {
                autoMenuSelected = value;
                TriggerType = eTriggerTypeList.Auto;
                RaisePropertyChanged("AutoMenuSelected");
            }
        }

        private bool patternMenuSelected = false;
        public bool PatternMenuSelected
        {
            get { return patternMenuSelected; }
            set
            {
                patternMenuSelected = value;
                TriggerType = eTriggerTypeList.Pattern;
                RaisePropertyChanged("PatternMenuSelected");
            }
        }

        private bool protocolMenuSelected = false;
        public bool ProtocolMenuSelected
        {
            get { return protocolMenuSelected; }
            set
            {
                protocolMenuSelected = value;
                TriggerType = eTriggerTypeList.Protocol;
                RaisePropertyChanged("ProtocolMenuSelected");
            }
        }

        private bool timingMenuSelected = false;
        public bool TimingMenuSelected
        {
            get { return timingMenuSelected; }
            set
            {
                timingMenuSelected = value;
                TriggerType = eTriggerTypeList.Timing;
                RaisePropertyChanged("TimingMenuSelected");
            }
        }

        public eTriggerTypeList TriggerType
        {
            get { return triggerType; }
            set
            {
              
                triggerType = value;
                RaisePropertyChanged("TriggerType");
            }
        }

        private eTriggerTypeList triggerType = eTriggerTypeList.Auto;


        //New protocol parameters


       
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


      


        public IConfigModel ProtocolSelection { get; set; }

     
    }
}
