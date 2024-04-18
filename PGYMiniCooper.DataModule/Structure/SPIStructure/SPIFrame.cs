using JetBrains.Annotations;
using PGYMiniCooper.DataModule.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure.SPIStructure
{
    public class SPIFrame : IFrame, INotifyPropertyChanged
    {
        public SPIFrame()
        {
            mISOdataBytes = new List<byte>();
            mOSIdataBytes = new List<byte>();
        }
        private int frameIndex;
        private double startTime;
        private double stopTime;
        private bool hasStop;
        private List<byte> mISOdataBytes;
        private List<byte> mOSIdataBytes;
        private eStartType start;
        private eErrorType errorType;
        private double frequency;
        public int FrameIndex { get => frameIndex; set => frameIndex = value; }
        public double StartTime { get => startTime; set => startTime = value; }
        public double StopTime { get => stopTime; set => stopTime = value; }
        public bool HasStop { get => hasStop; set => hasStop = value; }
        public List<byte> MISODataBytes { get => mISOdataBytes; set => mISOdataBytes = value; }
        public List<byte> MOSIDataBytes { get => mOSIdataBytes; set => mOSIdataBytes = value; }
        public eStartType Start { get => start; set => start = value; }
        public eErrorType ErrorType { get => errorType; set => errorType = value; }
        public double Frequency { get => frequency; set => frequency = value; }

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
