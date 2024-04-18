using PGYMiniCooper.DataModule.Interface;
using ProdigyFramework.Collections;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure.I2CStructure
{
    public class AddressMessageModel : IMessage
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
        private bool isTenbitAddress = false;
        public bool IsTenBitAddress
        {
            get
            {
                return isTenbitAddress;
            }
            set
            {
                isTenbitAddress = value;
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


        private eProtocolMode protocolMode;
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


        private string protocolname;

        public string Protocolname
        {
            get { return protocolname; }
            set { protocolname = value; }
        }

        //QSPI
        public int PacketValue
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
        private int packetvalue;


        public double DummyStopTime
        {
            get
            {
                return dummystoptime;
            }
            set
            {
                dummystoptime = value;
            }
        }
        double dummystoptime;




        private eAddressType description;
        public eAddressType Description
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

        public eAcknowledgeType AckType
        {
            get
            {
                return ackType;
            }
            set
            {
                ackType = value;
            }
        }
        private eAcknowledgeType ackType;

        public eTransferType TransferType
        {
            get
            {
                return transferType;
            }
            set
            {
                transferType = value;
            }
        }
        private eTransferType transferType;

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

        public ePacketType PacketType
        {
            get
            {
                return ePacketType.Address;
            }
            set
            {
                packetType = value;
            }
        }
        private ePacketType packetType;

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

        eStartType start;
        public eStartType Start;

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

        private double stopTime;

        public double StopTime
        {
            get { return stopTime; }
            set { stopTime = value; }
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
