using JetBrains.Annotations;
using PGYMiniCooper.DataModule.Interface;
using ProdigyFramework.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PGYMiniCooper.DataModule.Structure.QSPIStructure
{

    public class QSPIFrame : IFrame, INotifyPropertyChanged
    {
        public QSPIFrame()
        {
            packetCollection = new List<IMessage>();
            Address = -1;


        }

        private int frameIndex;
        private double startTime;
        private double stopTime;
        private bool hasStop;
        private eProtocol protocolType;


        private eStartType start;
        private eErrorType errorType;
        private double frequency;
        private double dummystoptime;




        public eProtocol ProtocolType { get => protocolType; set => protocolType = value; }
        public int FrameIndex { get => frameIndex; set => frameIndex = value; }

        public double StartTime { get => startTime; set => startTime = value; }

        public double StopTime { get => stopTime; set => stopTime = value; }

        public bool HasStop { get => hasStop; set => hasStop = value; }


        public eStartType Start { get => start; set => start = value; }

        public eErrorType ErrorType { get => errorType; set => errorType = value; }

        public double Frequency { get => frequency; set => frequency = value; }

        public double DummyStopTime { get => dummystoptime; set => dummystoptime = value; }

      

        public event PropertyChangedEventHandler PropertyChanged;
       
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


        public void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        
        public eQSPICommands QSPICommandType
        {
            get
            {
                return qSPIcommandType;
            }

            set
            {
                qSPIcommandType = value;
            }
        }

        private eQSPICommands qSPIcommandType;



      

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

        public string ProtocolName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        List<IMessage> packetCollection;


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
