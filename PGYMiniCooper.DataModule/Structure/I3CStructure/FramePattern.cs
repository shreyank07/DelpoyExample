using PGYMiniCooper.DataModule.Interface;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure.I3CStructure
{
    public class FramePattern : ViewModelBase, IFrame
    {
        public FramePattern()
        {
            packetCollection = new List<IMessage>();
            Address = -1;
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
        private int index;

        private double frequency;

        public double Frequency
        {
            get { return frequency; }
            set { frequency = value; }
        }

        private int address;
        public int Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
            }
        }
        private eProtocol protocolType;


        public eProtocol ProtocolType { get => protocolType; set => protocolType = value; }
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
        private double timeStamp;

        private double stopTimé;
        public double StopTime
        {
            get { return stopTimé; }

            set
            {
                stopTimé = value;
            }
        }

        public eMajorFrame FrameType
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
        private eMajorFrame frameType;

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

        public List<IMessage> PacketCollection
        {
            get
            {
                return packetCollection;
            }
            set
            {
                packetCollection = value;
            }
        }
        List<IMessage> packetCollection;

        private bool isSelected = false;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        byte device;
        public byte Device
        {
            get
            {
                return device;
            }
            set
            {
                device = value;
            }
        }

        public string ProtocolName { get; set; }


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
    
}
}
