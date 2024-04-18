using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using PGYMiniCooper.DataModule.Structure.I3CStructure;
using PGYMiniCooper.DataModule.Structure.UARTStructure;
using Prodigy.WaveformControls.Interfaces;
using ProdigyFramework.Behavior;
using ProdigyFramework.Collections;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using static PGYMiniCooper.DataModule.Structure.UARTStructure.UARTFrame;

namespace PGYMiniCooper.CoreModule.ViewModel.ProtocolViewModel
{
    public class ResultViewModel_UART : ViewModelBase, IResultViewModel
    {
        public ResultViewModel_UART(ConfigViewModel_UART config, ResultModel_UART result, TriggerViewModel trigger)       
        {
            this.Config = config;
            wareHouse = result;
			this.trigger = trigger;
            result.OnFramesDecoded += Result_OnFramesDecoded;
            wareHouse.Reset();
            resultCollection = new AsyncObservableCollection<UARTFrame>();

            SearchParameter = new SearchFilterViewModel_UART((IList)ResultCollection);
            SearchParameter.OnSelectionChanged += SearchParameter_OnSelectionChanged;
        }

        private readonly TriggerViewModel trigger;
        private void Result_OnFramesDecoded(object sender, IReadOnlyList<IFrame> frames)
        {
            var uartFrames = frames.OfType<UARTFrame>();

            ((AsyncObservableCollection<UARTFrame>)resultCollection).AddRange(uartFrames);

            foreach (var frame in uartFrames)
            {
                if (SessionConfiguration.Triggerset && trigger.TriggerType == eTriggerTypeList.Protocol && Config == trigger.ProtocolSelection)
                {
                    if ((SessionConfiguration.TriggerTime >= frame.StartTime && SessionConfiguration.TriggerTime <= frame.StopTime) || ((SessionConfiguration.TriggerTime - (2 / frame.Frequency)) >= frame.StartTime && (SessionConfiguration.TriggerTime - (2 / frame.Frequency)) <= frame.StopTime))
                    {
                        wareHouse.TriggerPacket = frame.FrameIndex;
                        SessionConfiguration.Triggerset = false;
                    }
                }
            }
        }

        private ConfigViewModel_UART config;
        public ConfigViewModel_UART Config 
        {
            get { return config; }
            set
            {
                this.config = value;
                RaisePropertyChanged(nameof(config));
            } 
        }

        ResultModel_UART wareHouse;
        public ResultModel_UART WareHouse
        {
            get
            {
                return wareHouse;
            }
            set
            {
                wareHouse = value;
                RaisePropertyChanged("WareHouse");
            }
        }

        private IList<UARTFrame> resultCollection;
        public IList<UARTFrame> ResultCollection
        {
            get => resultCollection;
            set
            {
                resultCollection = value;
                RaisePropertyChanged(nameof(ResultCollection));
            }
        }

        private int selectedIndex = -1;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {

                selectedIndex = value;
                RaisePropertyChanged("SelectedIndex");
            }
        }

        private Command gotoTrigger;
        public Command GotoTrigger
        {
            get
            {
                if (gotoTrigger == null)
                    gotoTrigger = new Command(new Command.ICommandOnExecute(gotoTriggerMethod));
                return gotoTrigger;
            }
        }

      public  void gotoTriggerMethod()
        {
            SelectedFrame = ResultCollection[WareHouse.TriggerPacket];
        }
       

        public ICommand selectionCommand;
        public ICommand SelectionCommand
        {
            get
            {
                return selectionCommand ?? (selectionCommand = new RelayCommand((o) => OnSelectionChanged?.Invoke(this)));
            }
        }
        void GetCurrentFrame(ProtocolActivityHolder frame)
        {
            if (frame.Protocol == DataModule.eProtocol.UART)
                SelectedIndex = frame.Sample;
        }
       

        public void Serach()
        {
        }

        public SearchFilterViewModel_UART SearchParameter { get; }

        private void SearchParameter_OnSelectionChanged(IFrame frame)
        {
            SelectedFrame = frame;
        }

        private ICommand rowLoadedCommand;

        public ICommand RowLoadedCommand
        {
            get
            {
                rowLoadedCommand ??= new CommunityToolkit.Mvvm.Input.RelayCommand<IFrame>(f =>
                {
                    f.IsHighlighted = false;
                    // if search is enabled
                    if (SearchParameter.SearchMode == SearchMode.Search && SearchParameter.IsSearchActive)
                    {
                        f.IsHighlighted = SearchParameter.IsMatch(f);
                    }
                });

                return rowLoadedCommand;
            }
        }

        public Command SearchCommandVisibility
        {
            get
            {

                return new Command(new Command.ICommandOnExecute(SearchVisibility));

            }
        }

        public void SearchVisibility()
        {
            if (BorderVisibility == false)
                BorderVisibility = true;

            else
                if (BorderVisibility == true)
                BorderVisibility = false;

        }
        private bool borderVisibility = false;
        public bool BorderVisibility
        {
            get
            {
                return borderVisibility;
            }
            set
            {
                borderVisibility = value;
                RaisePropertyChanged("BorderVisibility");
            }
        }

        public void Reset()
        {
            SelectedFrame = null;
        }

        double TimeStampConverter(double actualTime)
        {
            return (actualTime - SessionConfiguration.TriggerTime);
        }

        private IEnumerable<IBusData> GetBusDiagramForFrame(double startTime, double stopTime, short dataBytes, bool hasStop)
        {
            string emptyVal = "  .";

            double StartTimestamp = TimeStampConverter(startTime);
            double StopTimestamp = TimeStampConverter(stopTime);
            double currentTime = StartTimestamp;

            //Start
            var timeFor1Bit = 1d / Config.BaudRate;

            var bus = new PolygonBusData();
            bus.StartIndex = currentTime * 1e9;

            currentTime += timeFor1Bit;

            bus.StopIndex = currentTime * 1e9;
            bus.Data = "Start" + emptyVal;
            bus.Brush = System.Windows.Media.Brushes.HotPink;

            yield return bus;

            //Data
            bus = new PolygonBusData();
            bus.StartIndex = currentTime * 1e9;
            currentTime = StopTimestamp;
            if (config.ParitySelected)
            {
                currentTime -= 1 * timeFor1Bit;
            }
            bus.StopIndex = currentTime * 1e9;

            if (dataBytes != -1)
            {
                bus.Data = "0x" + dataBytes.ToString("X2") + emptyVal;
            }
            bus.Brush = System.Windows.Media.Brushes.RoyalBlue;

            yield return bus;

            //parity
            if (config.ParitySelected)
            {
                bus = new PolygonBusData();
                bus.StartIndex = currentTime * 1e9;

                currentTime += timeFor1Bit;

                bus.StopIndex = currentTime * 1e9;
                bus.Data = "Parity" + emptyVal;
                bus.Brush = System.Windows.Media.Brushes.HotPink;

                yield return bus;
            }

            //Stop
            if (hasStop)
            {
                bus = new PolygonBusData();
                bus.StartIndex = currentTime * 1e9;

                currentTime += config.StopBits * timeFor1Bit;

                bus.StopIndex = currentTime * 1e9;
                bus.Data = "Stop" + emptyVal;
                bus.Brush = System.Windows.Media.Brushes.HotPink;

                yield return bus;
            }
        }

        public IEnumerable<IBusData> GetBusDiagramTx(double startTime, double stopTime)
        {

            var resultList = resultCollection.SkipWhile(r => r.StopTime < startTime).TakeWhile(r => r.StartTime <= stopTime).ToList();
            foreach (var currentPacket in resultList)
            {
               if (currentPacket.TXDdataBytes == -1) continue;

                foreach (var bus in GetBusDiagramForFrame(currentPacket.StartTime, currentPacket.StopTime, currentPacket.TXDdataBytes, currentPacket.HasStop))
                    yield return bus;
            }
        }

        public IEnumerable<IBusData> GetBusDiagramRx(double startTime, double stopTime)
        {


            var resultList = resultCollection.SkipWhile(r => r.StopTime < startTime).TakeWhile(r => r.StartTime <= stopTime).ToList();
            foreach (var currentPacket in resultList)
            {
          
                if (currentPacket.RXdataBytes == -1) continue;

                foreach (var bus in GetBusDiagramForFrame(currentPacket.StartTime, currentPacket.StopTime, currentPacket.RXdataBytes, currentPacket.HasStop))
                    yield return bus;
            }
        }

        #region IResultViewModel implementation

        public event Action<IResultViewModel> OnSelectionChanged;

        IConfigViewModel IResultViewModel.Config => Config;

        int IResultViewModel.TriggerPacketIndex => wareHouse.TriggerPacket;

        public IFrame SelectedFrame
        {
            get { return wareHouse.SelectedFrame; }
            set
            {
                wareHouse.SelectedFrame = (UARTFrame)value;
                RaisePropertyChanged(nameof(SelectedFrame));
            }
        }

        ICollection<IFrame> IResultViewModel.ResultCollection => ResultCollection.OfType<IFrame>().ToList();

        private List<ChannelInfo> availableBusChannels;
        List<ChannelInfo> IResultViewModel.AvailableBusChannels
        {
            get
            {
                if (availableBusChannels == null)
                {
                    availableBusChannels = new List<ChannelInfo>();
                    if (config.ChannelIndex_TX != eChannles.None)
                    {
                        availableBusChannels.Add(new ChannelInfo("TX", ((IResultViewModel)this).Config.Channels.FirstOrDefault(c => c.ChannelIndex == config.ChannelIndex_TX).ChannelIndex));
                    }
                    if (config.ChannelIndex_RX != eChannles.None)
                    {
                        availableBusChannels.Add(new ChannelInfo("RX", ((IResultViewModel)this).Config.Channels.FirstOrDefault(c => c.ChannelIndex == config.ChannelIndex_RX).ChannelIndex));
                    }
                }

                return availableBusChannels;
            }
        }

        IEnumerable<IBusData> IResultViewModel.GetBusDiagram(ChannelInfo channel, double startTime, double stopTime)
        {
            if (config.ChannelIndex_TX == channel.ChannelIndex)
            {
                foreach (var bus in GetBusDiagramTx(startTime, stopTime))
                    yield return bus;
            }
            else if (config.ChannelIndex_RX == channel.ChannelIndex)
            {
                foreach (var bus in GetBusDiagramRx(startTime, stopTime))
                    yield return bus;
            }
        }

        #endregion
    }

    public class SearchFilterViewModel_UART : SearchFilterViewModelBase<IFrame>
    {
        public SearchFilterViewModel_UART(IList collection) : base(collection)
        {
        }

        private string txValue;
        public string TxValue
        {
            get { return txValue; }
            set
            {
                txValue = value;
                OnPropertyChanged(nameof(TxValue));
            }
        }

        private string rxValue;
        public string RxValue
        {
            get => rxValue;
            set
            {
                rxValue = value;
                OnPropertyChanged(nameof(RxValue));
            }
        }

        public override bool IsMatch(IFrame frame)
        {
            var uartFrame = (UARTFrame)frame;

            Int64 intValTX = Int64.Parse(uartFrame.TXDdataBytes.ToString());
            string txData = String.Format("0x{0:X}", intValTX);
            string txData1 = String.Format("{0:X}", intValTX);
            Int64 intValRX = Int64.Parse(uartFrame.RXdataBytes.ToString());
            string rxData = String.Format("0x{0:X}", intValRX);
            string rxData1 = String.Format("{0:x}", intValRX);


            bool result = true;
            bool searchValid = false;
            if (!string.IsNullOrWhiteSpace(TxValue))
            {
                result &= txData.ToLower().Equals(TxValue.ToLower()) || txData1.ToLower().Equals(TxValue.ToLower());
                searchValid = true;
            }
            if (!string.IsNullOrWhiteSpace(RxValue))
            {
                result &= rxData.ToLower().Equals(RxValue.ToLower()) || rxData1.ToLower().Equals(RxValue.ToLower());
                searchValid = true;
            }

            return result && searchValid;
        }
    }
}
