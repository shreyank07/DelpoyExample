using PGYMiniCooper.DataModule.Interface;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure.SPMIStructure
{
    public class SPMIFrameStructure : ViewModelBase, IFrame
    {
        public SPMIFrameStructure()
        {
            SPMIErrorType = new List<eErrorType>();
            hostDevice = eHostDevice.NA;
            interruptCount = -1;
            ByteCount = -1;
            TrasnsferType = eTransferType.NA;
        }
        private eProtocol protocolType;


        public eProtocol ProtocolType { get => protocolType; set => protocolType = value; }
        SPMIPacketStructure _Command;
        SPMIPacketStructure[] _Address;
        SPMIPacketStructure[] _Data;
        SPMIPacketStructure[] _Bytecount;
        int intAddress;
        int slaveId;
        int slaveId1;
        int mid;
        int mID;
        int mPL;
        int mID_TBO;

        public int MID_TBO
        {
            get { return mID_TBO; }
            set
            {
                mID_TBO = value;
            }
        }
        int mPL_TBO = 3;

        public int MPL_TBO
        {
            get { return mPL_TBO; }
            set
            {
                mPL_TBO = value;
            }
        }
        int slaveIdRCS;
        int index;
        double timeStamp;
        double arbitrationTimeStamp;
        double stopTime;
        int byteCount;
        bool hasC_bit = false;

        public bool HasC_bit
        {
            get { return hasC_bit; }
            set
            {
                hasC_bit = value;
            }
        }
        bool hasA_bit = false;

        public bool HasA_bit
        {
            get { return hasA_bit; }
            set
            {
                hasA_bit = value;
            }
        }
        bool hasSr_bit = false;

        public bool HasSr_bit
        {
            get { return hasSr_bit; }
            set
            {
                hasSr_bit = value;
            }
        }

        bool isMethod1orRCS = false;

        public bool IsMethod1orRCS
        {
            get { return isMethod1orRCS; }
            set
            {
                isMethod1orRCS = value;
            }
        }

        bool isMethod3 = false;

        public bool IsMethod3
        {
            get { return isMethod3; }
            set
            {
                isMethod3 = value;
            }
        }

        bool hasMPLSecondary = true;

        public bool HasMPLSecondary
        {
            get { return hasMPLSecondary; }
            set
            {
                hasMPLSecondary = value;
            }
        }

        bool hasMPLPrimary = true;

        public bool HasMPLPrimary
        {
            get { return hasMPLPrimary; }
            set
            {
                hasMPLPrimary = value;
            }
        }

        bool hasMPLBitsPrimary = false;

        public bool HasMPLBitsPrimary
        {
            get { return hasMPLBitsPrimary; }
            set
            {
                hasMPLBitsPrimary = value;
            }
        }

        bool hasMPLBitsSecondary = false;

        public bool HasMPLBitsSecondary
        {
            get { return hasMPLBitsSecondary; }
            set
            {
                hasMPLBitsSecondary = value;
            }
        }

        bool hasArbitration = false;

        public bool HasArbitration
        {
            get { return hasArbitration; }
            set
            {
                hasArbitration = value;
            }
        }

        int busTimeoutPeriod = 0;

        public int BusTimeoutPeriod
        {
            get { return busTimeoutPeriod; }
            set
            {
                busTimeoutPeriod = value;
            }
        }

        List<eErrorType> SPMIerrorType;
        eErrorType errorType;
        int interruptCount;

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

        public SPMIPacketStructure Command
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

        public SPMIPacketStructure[] Address
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

        public SPMIPacketStructure[] Data
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

        public SPMIPacketStructure[] BytecountList
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

        private bool _IsExpanded;
        public bool IsExpanded
        {
            get
            {
                return this._IsExpanded;
            }
            set
            {
                this._IsExpanded = value;
            }
        }


        public List<eErrorType> SPMIErrorType
        {
            get
            {
                return SPMIerrorType;
            }
            set
            {
                SPMIerrorType = value;
            }
        }

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

        public int MID
        {
            get
            {
                return mID;
            }

            set
            {
                mID = value;
            }
        }

        public int MPL
        {
            get
            {
                return mPL;
            }

            set
            {
                mPL = value;
            }
        }

        public int SlaveIdRCS
        {
            get
            {
                return slaveIdRCS;
            }

            set
            {
                slaveIdRCS = value;
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

        public double ArbitrationTimeStamp
        {
            get
            {
                return arbitrationTimeStamp;
            }

            set
            {
                arbitrationTimeStamp = value;
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

        bool hasOwnershipTranfer = false;
        public bool HasOwnershipTranfer
        {
            get { return hasOwnershipTranfer; }
            set
            {
                hasOwnershipTranfer = value;
                RaisePropertyChanged("HasOwnershipTranfer");
            }
        }
        public int SlaveIdBind
        {
            get
            {

                return slaveId1;
            }

            set
            {

                slaveId1 = value;



                RaisePropertyChanged("SlaveIdBind");
            }
        }

        public int MIDBind
        {
            get
            {
                return mid;
            }

            set
            {

                mid = value;



                RaisePropertyChanged("MIDBind");
            }
        }

        public string ProtocolName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
