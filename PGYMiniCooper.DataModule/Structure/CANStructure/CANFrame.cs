using JetBrains.Annotations;
using PGYMiniCooper.DataModule.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace PGYMiniCooper.DataModule.Structure.CANStructure
{
   public class CANFrame : IFrame, INotifyPropertyChanged
    {

        public CANFrame()
        {
            dataBytes = new List<byte>();
            stopTimeList = new List<double>();
        }
        private int frameIndex;
       // private double bitStuffIndex;
        private double startTime;
        private double stopTime;
        private bool hasBitstufferoor;
        private bool hasStop;
        private List<double> stopTimeList;
        private eStartType start;
        public byte bRSBit; 
        private eErrorType errorType;
        private eFrameTypeCAN frameType;
        private eframeformates FrameFormat;
        private double frequency;
        private double brsfrequency;
        public int FrameIndex { get => frameIndex; set => frameIndex = value; }
     

     
        private int id;
        public int ID { get => id; set => id = value; }


        public List<double> DataStopTimeList
        {
            get
            {
                return dataStopTimeList;
            }
            set
            {
                dataStopTimeList = value;
                //RaisePropertyChanged("DataStopTimeList");
            }
        }
        private  List<double> dataStopTimeList;
        private bool isStandard=false;
        public bool IsStandard
        {
            get
            {
                return isStandard;
            }
            set
            {
                isStandard = value;
                //RaisePropertyChanged("IsStandard");
            }
        }
        private bool isExtended=false;
        public bool IsExtended
        {
            get
            {
                return isExtended;
            }
            set
            {
                isExtended = value;
                //RaisePropertyChanged("IsExtended");
            }
        }
        private eProtocol protocolType;


        public eProtocol ProtocolType { get => protocolType; set => protocolType = value; }
        public double StartTime { get => startTime; set => startTime = value; }
        public double StopTime { get => stopTime; set => stopTime = value; }
        public bool HasStop { get => hasStop; set => hasStop = value; }
        public bool HasBitstufferoor { get => hasBitstufferoor; set => hasBitstufferoor = value; }
        public List<double> StopTimeList { get => stopTimeList; set => stopTimeList = value; }
        private List<byte> dataBytes;

        public List<byte> DataBytes
        {
            get { return dataBytes; }
            set { dataBytes = value; }
        }


        private int dlcdataBytes;
        public int DLCDdataBytes { get => dlcdataBytes; set => dlcdataBytes = value; }

        private byte reservedBit;
        public byte ReservedBit { get => reservedBit; set => reservedBit = value; }

        private byte reservedBit1;
        public byte ReservedBit1 { get => reservedBit1; set => reservedBit1 = value; }

        private int iddataBytes;
        public int IDDdataBytes 
        { get => iddataBytes;
            set => iddataBytes = value; }

        private int idEdataBytes;
        public int IDEDdataBytes
        {
            get => idEdataBytes;
            set => idEdataBytes = value;

        }
        public byte BRSBit { get => bRSBit; set => bRSBit = value; }

        private byte rtrdataBytes;
        public byte RTRDdataBytes { get => rtrdataBytes; set => rtrdataBytes = value; }

        private byte ackdataBytes;
        public byte ACKDdataBytes { get => ackdataBytes; set => ackdataBytes = value; }

        private int iFS;
        public int IFS { get => iFS; set => iFS = value; }

        private int eOF;
        public int EOF { get => eOF; set => eOF = value; }

        private int crcdataBytes;


        public int CRCDdataBytes { get => crcdataBytes; set => crcdataBytes = value; }

        private byte crcDele;
        public byte CRCDele { get => crcDele; set => crcDele = value; }

        private byte ackDele;
        public byte ACKDele { get => ackDele; set => ackDele = value; }

        public eFrameTypeCAN FrameType { get => frameType; set => frameType = value; }

        public eStartType Start { get => start; set => start = value; }
        public eErrorType ErrorType { get => errorType; set => errorType = value; }

        public eframeformates Eframeformates { get => FrameFormat; set => FrameFormat= value; }
        public double Frequency { get => frequency; set => frequency = value; }
        public double BRSFrequency { get => brsfrequency; set => brsfrequency = value; }


    private bool isHighlighted;


    public bool IsHighlighted
    {
        get => isHighlighted;
        set
        {
            if (value == isHighlighted) return;
            isHighlighted = value;
            OnPropertyChanged();
        }
    }

        public string ProtocolName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
   }
}
