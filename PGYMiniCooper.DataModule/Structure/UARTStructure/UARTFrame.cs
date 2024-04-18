using JetBrains.Annotations;
using PGYMiniCooper.DataModule.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure.UARTStructure
{
    public class UARTFrame : IFrame,  INotifyPropertyChanged
    {
        public UARTFrame()
        {
            tXdataBytes = -1;
            rXdataBytes = -1;
        }
        private int frameIndex;
        private double startTime;
        private double stopTime;
        private bool hasStop;
        private bool hasStopTx;
        private bool hasStopRx;
        private Int16 tXdataBytes;
        private Int16 rXdataBytes;
        private eStartType start;
        private eErrorType errorType;
        private double frequency;
        public int FrameIndex { get => frameIndex; set => frameIndex = value; }
        public double StartTime { get => startTime; set => startTime = value; }
        public double StopTime { get => stopTime; set => stopTime = value; }
        public bool HasStop { get => hasStop; set => hasStop = value; }
        public Int16 TXDdataBytes { get => tXdataBytes; set => tXdataBytes = value; }
        public eStartType Start { get => start; set => start = value; }
        public eErrorType ErrorType { get => errorType; set => errorType = value; }
        public double Frequency { get => frequency; set => frequency = value; }
        public Int16 RXdataBytes { get => rXdataBytes; set => rXdataBytes = value; }
        public bool HasStopTx { get => hasStopTx; set => hasStopTx = value; }
        public bool HasStopRx { get => hasStopRx; set => hasStopRx = value; }
        private eProtocol protocolType;


        public eProtocol ProtocolType { get => protocolType; set => protocolType = value; }

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
