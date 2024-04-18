using PGYMiniCooper.DataModule.Interface;
using ProdigyFramework.Collections;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure.I2CStructure
{
    public class DataMessageModel : IMessage
    {
        public DataMessageModel()
        {
       
        }
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
        private bool stopFoundRx = false;

        public bool StopFoundRx
        {
            get
            {
                return stopFoundRx;
               
            }
            set
            {
                stopFoundRx = value;
            }
        }
        private bool stopFoundTx = false;

        public bool StopFoundTx
        {
            get
            {
                return stopFoundTx;

            }
            set
            {
                stopFoundTx = value;
            }
        }
        public eStartType Start;
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
        private int value = -1;

        private int mISOvalue = -1;

        public int MISOValue
        {
            get
            {
                return this.mISOvalue;
            }
            set
            {
                this.mISOvalue = value;
            }
        }


     


        //QSPI
        public long PacketValue
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

        private long packetvalue;
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


        








       public int AddressValue
        {
            get
            {
                return this.addressvalue;
            }
            set
            {
                this.addressvalue = value;
            }
        }
        private int addressvalue = -1;



       





        private int tXvalue = -1;

        public int TXValue
        {
            get
            {
                return this.tXvalue;
            }
            set
            {
                this.tXvalue = value;
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

        public byte TransmitBit
        {
            get
            {
                return transmitBit;
            }
            set
            {
                transmitBit = value;
            }
        }
        private byte transmitBit;

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
