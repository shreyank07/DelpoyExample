
using PGYMiniCooper.DataModule.Interface;
using ProdigyFramework.Collections;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure.CANStructure
{
    public class CANMessageModeel : IMessage

    {



        public CANMessageModeel() 
        {
            stopTimelist = new List<double>();
            dataStopTime = new List<double>();
            DataBytesCAN = new List<byte>();
        }
        #region CAN


        // 11_bit_Identifier
        public int IdentifierValue
        {
            get
            {
                return this.identifiervalue;
            }
            set
            {
                this.identifiervalue = value;
            }
        }

        private int identifiervalue = -1;


        //18Bit Identifier
        public int ExtendedIdentifier
        {
            get
            {
                return this.extendedIdentifier;
            }
            set
            {
                this.extendedIdentifier = value;
            }
        }
        private int extendedIdentifier = -1;

        #endregion


        //bytes of data
        private List<byte> dataBytesCAN;

        public List<byte> DataBytesCAN
        {
            get { return dataBytesCAN; }
            set { dataBytesCAN = value; }
        }




        public byte ACKValue
        {
            get
            {
                return this.ACKvalue;
            }
            set
            {
                this.ACKvalue = value;
            }
        }
        private byte ACKvalue = 0;


        public byte RTRValue
        {
            get
            {
                return this.RTRvalue;
            }
            set
            {
                this.RTRvalue = value;
            }
        }
        private byte RTRvalue = 0;

        public int DLCValue
        {
            get
            {
                return this.DLCvalue;
            }
            set
            {
                this.DLCvalue = value; ;
            }
        }
        private int DLCvalue = -1;


        public int CRCValue
        {
            get
            {
                return this.CRCvalue;
            }
            set
            {
                this.CRCvalue = value;
            }
        }
        private int CRCvalue = -1;


        public byte CRCDelValue
        {
            get
            {
                return this.CRCDelvalue;
            }
            set
            {
                this.CRCDelvalue = value;
            }
        }
        private byte CRCDelvalue = 0;

        public byte ACKDelValue
        {
            get
            {
                return this.aCKDelvalue;
            }
            set
            {
                this.aCKDelvalue = value;
            }
        }
        private byte aCKDelvalue = 0;


        public byte ReservedBit
        {
            get
            {
                return this.reservedBit;
            }
            set
            {
                this.reservedBit = value;
            }
        }
        private byte reservedBit = 0;

        public byte BRSBit
        {
            get
            {
                return this.bRSBit;
            }
            set
            {
                this.bRSBit = value;
            }
        }
        private byte bRSBit = 0;


        public byte ReservedBit1
        {
            get
            {
                return this.reservedBit1;
            }
            set
            {
                this.reservedBit1 = value;
            }
        }
        private byte reservedBit1 = 0;
        public eFrameTypeCAN FrameType
        {
            get
            {
                return frameType;
            }
            set
            {
                frameType = value;
            }
        }
        private eFrameTypeCAN frameType;


        public int IDFlagBit
        {
            get
            {
                return this.iDflag;
            }
            set
            {
                this.iDflag = value;
            }
        }
        private int iDflag = -1;



        private bool isStandard = false;
        public bool IsStandard
        {
            get
            {
                return isStandard;
            }
            set
            {
                isStandard = value;
            }
        }
        private bool isExtended = false;
        public bool IsExtended
        {
            get
            {
                return isExtended;
            }
            set
            {
                isExtended = value;
            }
        }
        private bool hasBitstuffError;
        public bool HasBitstuffError
        {
            get
            {
                return hasBitstuffError;
            }
            set
            {
                hasBitstuffError = value;
            }
        }

        public eStartType Start;
        public int IFS
        {
            get
            {
                return this.iFS;
            }
            set
            {
                this.iFS = value;
            }
        }
        private int iFS = -1;

        public int EOF
        {
            get
            {
                return this.eOF;
            }
            set
            {
                this.eOF = value;
            }
        }
        private int eOF = -1;


        public double BRSFrequency
        {
            get
            {
                return brsfrequency;
            }
            set
            {
                brsfrequency = value;
            }
        }
        double brsfrequency;


        public List<double> StopTimeList
        {
            get
            {
                return stopTimelist;
            }
            set
            {
                stopTimelist = value;
            }
        }
        private List<double> stopTimelist = new List<double>();




        public List<double> DataStopTime
        {
            get
            {
                return dataStopTime;
            }
            set
            {
                dataStopTime = value;
            }
        }
        private List<double> dataStopTime = new List<double>();

        #region FromImessage
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        private int id;
        public double TimeStamp
        {
            get
            {
                return timeStamp;
            }
            set
            {
                timeStamp = value;
            }
        }
        double timeStamp;
        private bool isHdrExitFound = false;
        public bool IsHdrExitFound
        {
            get
            {
                return isHdrExitFound;
            }
            set
            {
                isHdrExitFound = value;
            }
        }
        public int StartIndex
        {
            get
            {
                return startIndex;
            }
            set
            {
                startIndex = value;
            }
        }
        private int startIndex;

        public int StopIndex
        {
            get
            {
                return stopIndex;
            }
            set
            {
                stopIndex = value;
            }
        }
        private int stopIndex;


        public ePacketType PacketType
        {
            get
            {
                return packetType;
            }
            set
            {
                packetType = value;
            }
        }
        private ePacketType packetType;
        public eProtocolMode ProtocolMode
        {
            get
            {
                return protocolMode;
            }
            set
            {
                protocolMode = value;
            }
        }
        private eProtocolMode protocolMode;

        public eDataDescription Description { get; set; }


        public eHostDevice HostDevice
        {
            get
            {
                return hostDevice;
            }
            set
            {
                hostDevice = value;
            }
        }
        private eHostDevice hostDevice;
        public eErrorType ErrorType
        {
            get
            {
                return errorType;
            }
            set
            {
                errorType = value;
            }
        }
        private eErrorType errorType;




        private double stopTime;

        public double StopTime
        {
            get { return stopTime; }
            set { stopTime = value; }
        }

        public double Frequency
        {
            get
            {
                return frequency;
            }
            set
            {
                frequency = value;
            }
        }
        double frequency;

        bool stop;
        public bool Stop
        {
            get
            {
                return stop;
            }
            set
            {
                stop = value;
            }
        }

        private bool isSelected = false;

        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }

        private bool hasAbort;
        public bool HasAbort
        {
            get
            {
                return hasAbort;
            }
            set
            {
                hasAbort = value;
            }
        }









        //public byte DLCValue
        //{
        //    get
        //    {
        //        return this.DLCvalue;
        //    }
        //    set
        //    {
        //        this.DLCvalue = value; ;
        //    }
        //}
        //private byte DLCvalue = 0;



        //public long RTRValue
        //{
        //    get
        //    {
        //        return this.RTRvalue;
        //    }
        //    set
        //    {
        //        this.RTRvalue = value;
        //    }
        //}
        //private long RTRvalue = -1;


        //public int ACKValue
        //{
        //    get
        //    {
        //        return this.ACKvalue;
        //    }
        //    set
        //    {
        //        this.ACKvalue = value;
        //    }
        //}
        //private int ACKvalue = -1;

        //public double BRSFrequency
        //{
        //    get
        //    {
        //        return brsfrequency;
        //    }
        //    set
        //    {
        //        brsfrequency = value;
        //    }
        //}
        //double brsfrequency;



        //private List<byte> dataBytesCAN;

        //public List<byte> DataBytesCAN
        //{
        //    get { return dataBytesCAN; }
        //    set { dataBytesCAN = value; }
        //}






        //public int IFS
        //{
        //    get
        //    {
        //        return this.iFS;
        //    }
        //    set
        //    {
        //        this.iFS = value;
        //    }
        //}
        //private int iFS = -1;

        //public int EOF
        //{
        //    get
        //    {
        //        return this.eOF;
        //    }
        //    set
        //    {
        //        this.eOF = value;
        //    }
        //}
        //private int eOF = -1;

        //public long IDEEValue
        //{
        //    get
        //    {
        //        return this.IDEEvalue;
        //    }
        //    set
        //    {
        //        this.IDEEvalue = value;
        //    }
        //}
        //private long IDEEvalue = -1;

        //public int CRCValue
        //{
        //    get
        //    {
        //        return this.CRCvalue;
        //    }
        //    set
        //    {
        //        this.CRCvalue = value;
        //    }
        //}
        //private int CRCvalue = -1;


        //public int CRCDelValue
        //{
        //    get
        //    {
        //        return this.CRCDelvalue;
        //    }
        //    set
        //    {
        //        this.CRCDelvalue = value;
        //    }
        //}
        //private int CRCDelvalue = -1;

        //public int ACKDelValue
        //{
        //    get
        //    {
        //        return this.aCKDelvalue;
        //    }
        //    set
        //    {
        //        this.aCKDelvalue = value;
        //    }
        //}
        //private int aCKDelvalue = -1;


        //public int ReservedBit
        //{
        //    get
        //    {
        //        return this.reservedBit;
        //    }
        //    set
        //    {
        //        this.reservedBit = value;
        //    }
        //}
        //private int reservedBit = -1;

        //public int BRSBit
        //{
        //    get
        //    {
        //        return this.bRSBit;
        //    }
        //    set
        //    {
        //        this.bRSBit = value;
        //    }
        //}
        //private int bRSBit = -1;


        //public int ReservedBit1
        //{
        //    get
        //    {
        //        return this.reservedBit1;
        //    }
        //    set
        //    {
        //        this.reservedBit1 = value;
        //    }
        //}
        //private int reservedBit1 = -1;
        //public eFrameTypeCAN FrameType
        //{
        //    get
        //    {
        //        return frameType;
        //    }
        //    set
        //    {
        //        frameType = value;
        //    }
        //}
        //private eFrameTypeCAN frameType;


        //public List<double> StopTimeList
        //{
        //    get
        //    {
        //        return stopTimelist;
        //    }
        //    set
        //    {
        //        stopTimelist = value;
        //    }
        //}
        //private List<double> stopTimelist = new List<double>();




        //public List<double> DataStopTime
        //{
        //    get
        //    {
        //        return dataStopTime;
        //    }
        //    set
        //    {
        //        dataStopTime = value;
        //    }
        //}
        //private List<double> dataStopTime = new List<double>();


        //private bool isStandard = false;
        //public bool IsStandard
        //{
        //    get
        //    {
        //        return isStandard;
        //    }
        //    set
        //    {
        //        isStandard = value;
        //    }
        //}
        //private bool isExtended = false;
        //public bool IsExtended
        //{
        //    get
        //    {
        //        return isExtended;
        //    }
        //    set
        //    {
        //        isExtended = value;
        //    }
        //}
        //private bool hasBitstuffError;
        //public bool HasBitstuffError
        //{
        //    get
        //    {
        //        return hasBitstuffError;
        //    }
        //    set
        //    {
        //        hasBitstuffError = value;
        //    }
        //}

        //public int ID
        //{
        //    get
        //    {
        //        return this.iD;
        //    }
        //    set
        //    {
        //        this.iD = value;
        //    }
        //}
        //private int iD = -1;

        #endregion

    }
}