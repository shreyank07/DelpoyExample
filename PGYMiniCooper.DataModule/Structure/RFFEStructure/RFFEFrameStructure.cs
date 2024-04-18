using PGYMiniCooper.DataModule.Interface;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure.RFFEStructure
{
    public class RFFEFrameStructure : ViewModelBase, IFrame
    {
        public RFFEFrameStructure()
        {
            RFFEerrorType = new List<eErrorType>();
            hostDevice = eHostDevice.NA;
            interruptCount = -1;
            ByteCount = -1;
            TrasnsferType = eTransferType.NA;
        }

        RFFEPacketStructure _Command;
        RFFEPacketStructure[] _Address;
        RFFEPacketStructure[] _Data;
        RFFEPacketStructure[] _Bytecount;
        int intAddress;
        int slaveId;
        int index;
        double timeStamp;
        double stopTime;
        int byteCount;
        List<eErrorType> RFFEerrorType;
        int interruptCount;
        eErrorType errorType;
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
        private eProtocol protocolType;


        public eProtocol ProtocolType { get => protocolType; set => protocolType = value; }
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

        public RFFEPacketStructure Command
        {
            get
            {
                return _Command;
            }

            set
            {
                _Command = value;
            }
        }

        public RFFEPacketStructure[] Address
        {
            get
            {
                return _Address;
            }

            set
            {
                _Address = value;
            }
        }

        public RFFEPacketStructure[] Data
        {
            get
            {
                return _Data;
            }

            set
            {
                _Data = value;
            }
        }

        public RFFEPacketStructure[] BytecountList
        {
            get
            {
                return _Bytecount;
            }

            set
            {
                _Bytecount = value;
            }
        }

        public List<eErrorType> RFFEErrorType
        {
            get
            {
                return RFFEerrorType;
            }
            set
            {
                RFFEerrorType = value;
            }
        }

        public int IntAddress
        {
            get
            {
                return intAddress;
            }

            set
            {
                intAddress = value;
            }
        }

        public int ByteCount
        {
            get
            {
                return byteCount;
            }

            set
            {
                byteCount = value;
            }
        }

        public int InterruptCount
        {
            get
            {
                return interruptCount;
            }
            set
            {
                interruptCount = value;
            }
        }

        public int SlaveId
        {
            get
            {
                return slaveId;
            }

            set
            {
                slaveId = value;
            }
        }

        public int FrameIndex
        {
            get
            {
                return index;
            }

            set
            {
                index = value;
            }
        }

        public double StartTime
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

        public double StopTime
        {
            get
            {
                return stopTime;
            }

            set
            {
                stopTime = value;
            }
        }

        private eTransferType trasnsferType;

        public eTransferType TrasnsferType
        {
            get
            {
                return trasnsferType;
            }
            set
            {
                trasnsferType = value;
            }
        }


        private bool isHighlighted;
        public bool IsHighlighted
        {
            get => isHighlighted;
            set
            {
                if (value == isHighlighted) return;
                isHighlighted = value;
                RaisePropertyChanged("IsHighlighted");
            }
        }

        public string ProtocolName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
