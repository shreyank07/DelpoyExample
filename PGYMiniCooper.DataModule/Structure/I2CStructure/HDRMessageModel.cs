using PGYMiniCooper.DataModule.Interface;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure.I2CStructure
{
    public class HDRMessageModel : IMessage
    {
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

        public int Id1
        {
            get
            {
                return id1;
            }
            set
            {
                id1 = value;
            }
        }
        private int id1;

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



        private double stopTime;

        public double StopTime
        {
            get { return stopTime; }
            set { stopTime = value; }
        }

        public int Preamble
        {
            get
            {
                return preamble;
            }
            set
            {
                preamble = value;
            }
        }
        private int preamble;

        public int Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }
        private int value;

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

        public int ParityBit
        {
            get
            {
                return parityBit;
            }

            set
            {
                parityBit = value;
            }
        }
        private int parityBit;

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
        public ePreambleState Description { get; set; }


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
        public bool IsMaster1_1
        {
            get
            {
                return isMaster1_1;
            }
            set
            {
                isMaster1_1 = value;
            }
        }
        bool isMaster1_1 = false;
    }
}
