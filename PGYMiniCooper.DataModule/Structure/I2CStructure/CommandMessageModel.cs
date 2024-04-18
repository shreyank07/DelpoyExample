using PGYMiniCooper.DataModule.Interface;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure.I2CStructure
{
    public class CommandMessageModel : IMessage
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

     

        public byte Value
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
        private byte value;


        //QSPI
        public byte PacketValue
        {
            get
            {
                return this.packetvalue;
            }
            set
            {
                this.packetvalue = value;
            }
        }
        private byte packetvalue;






        public eHostDevice HostDevice
        {
            get
            {
                return eHostDevice.Master;
            }
            set
            {

            }
        }

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

        public eCommand Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }
        private eCommand description;

        public ePacketType PacketType
        {
            get
            {
                return ePacketType.Command;
            }
            set
            {
                packetType = value;
            }
        }
        private ePacketType packetType;

        //QSPI

        private eQSPICommands describe;
        public eQSPICommands Describe
        {
            get
            {
                return describe;
            }
            set
            {
                describe = value;
            }
        }


        //qspi

        //public double DummyStopTime
        //{
        //    get
        //    {
        //        return dummystoptime;
        //    }
        //    set
        //    {
        //        dummystoptime = value;
        //        RaisePropertyChanged("Dummystoptime");
        //    }
        //}
        //double dummystoptime;


        private eErrorType errorType;
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

        public byte ParityBit
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
        private byte parityBit;


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
    }
}
