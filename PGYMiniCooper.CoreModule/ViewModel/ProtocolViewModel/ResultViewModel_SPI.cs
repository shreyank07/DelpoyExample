using CommunityToolkit.Mvvm.DependencyInjection;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Helpers;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using PGYMiniCooper.DataModule.Structure.RFFEStructure;
using PGYMiniCooper.DataModule.Structure.SPIStructure;
using Prodigy.WaveformControls.Interfaces;
using ProdigyFramework.Behavior;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace PGYMiniCooper.CoreModule.ViewModel.ProtocolViewModel
{
    public class ResultViewModel_SPI : ViewModelBase, IResultViewModel
    {
        public ResultViewModel_SPI(ConfigViewModel_SPI config, ResultModel_SPI model, TriggerViewModel trigger)
        {
            this.Config = config;
            this.wareHouse = model;
            this.trigger = trigger;
            wareHouse.Reset();
            sPIFrameCollection = new ProdigyFramework.Collections.AsyncObservableCollection<SPIFrame>();
            wareHouse.OnFramesDecoded += WareHouse_OnFramesDecoded;

            SearchParameter = new SearchFilterViewModel_SPI((System.Collections.IList)SPIFrameCollection);
            SearchParameter.OnSelectionChanged += SearchParameter_OnSelectionChanged;
        }
        private readonly TriggerViewModel trigger;
        private void WareHouse_OnFramesDecoded(object sender, IReadOnlyList<IFrame> frames)
        {
            var spiFrames = frames.OfType<SPIFrame>();

            ((ProdigyFramework.Collections.AsyncObservableCollection<SPIFrame>)sPIFrameCollection).AddRange(spiFrames);

            foreach (var frame in spiFrames)
            {
                if (SessionConfiguration.Triggerset && trigger.TriggerType == eTriggerTypeList.Protocol && Config == trigger.ProtocolSelection)
                {
                    if (SessionConfiguration.TriggerTime >= frame.StartTime && SessionConfiguration.TriggerTime <= frame.StopTime || ((SessionConfiguration.TriggerTime - (2 / frame.Frequency)) >= frame.StartTime && (SessionConfiguration.TriggerTime - (2 / frame.Frequency)) <= frame.StopTime))
                    {
                        wareHouse.TriggerPacket = frame.FrameIndex;
                        SessionConfiguration.Triggerset = false;
                    }
                }
            }
        }

        private ConfigViewModel_SPI config;
        public ConfigViewModel_SPI Config 
        {
            get { return config; }
            set
            {
                this.config = value;
                RaisePropertyChanged(nameof(Config));
            } 
        }


        public IList<SPIFrame> SPIFrameCollection
        {
            get
            {
                return sPIFrameCollection;
            }
            set
            {
                sPIFrameCollection = value;
                RaisePropertyChanged(nameof(SPIFrameCollection));
            }
        }
      private  IList<SPIFrame> sPIFrameCollection;



        ResultModel_SPI wareHouse;
        public ResultModel_SPI WareHouse
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
            SelectedFrame =SPIFrameCollection [WareHouse.TriggerPacket];
        }
       

        public ICommand selectionCommand;
        public ICommand SelectionCommand
        {
            get
            {
                return selectionCommand ?? (selectionCommand = new RelayCommand((o) => OnSelectionChanged?.Invoke(this)));
            }
        }

        #region Search and filter

        public void Serach() { }

        private void SearchParameter_OnSelectionChanged(IFrame frame)
        {
            SelectedFrame = frame;
        }

        public SearchFilterViewModel_SPI SearchParameter { get; private set; }

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

        #endregion
        public void Reset()
        {
            wareHouse.TriggerTime= 0;
            wareHouse.SelectedFrame = null;
        }

        double TimeStampConverter(double actualTime)
        {
            return (actualTime - SessionConfiguration.TriggerTime);
        }

        private IDataProvider dataReceiver = null;

        private List<DiscreteWaveForm> GetWavepoints(double startTime, double stopTime)
        {
            if (dataReceiver == null)
            {
                dataReceiver = Ioc.Default.GetService<IDataProvider>();
            }

            return dataReceiver.GetWaveformBufferTimestamp(startTime, stopTime);
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
                wareHouse.SelectedFrame = (SPIFrame)value;
                RaisePropertyChanged("SelectedFrame");
            }
        }

        ICollection<IFrame> IResultViewModel.ResultCollection => SPIFrameCollection.OfType<IFrame>().ToList();

        List<ChannelInfo> IResultViewModel.AvailableBusChannels => new List<ChannelInfo> { new ChannelInfo("SPI", ((IResultViewModel)this).Config.Channels.Last(c => c.ChannelIndex != eChannles.None).ChannelIndex) };

        IEnumerable<IBusData> IResultViewModel.GetBusDiagram(ChannelInfo channel, double startTime, double stopTime)
        {
            var bufferlist = GetWavepoints(startTime, stopTime).ToList();
            var edgeFinding = new DigitalWaveformEdgeFinding(bufferlist);

            eChannles clockChannel = Config.ChannelIndex_CLK;
            eChannles csChannel = Config.ChannelIndex_CS;
            int lastEdge = 0;
            var annotationStopIndex = 0;
            string emptyVal = "  .";
            var resultList = SPIFrameCollection.SkipWhile(r => r.StopTime < startTime).TakeWhile(r => r.StartTime <= stopTime).ToList();

            foreach (var currentPacket in resultList)
            {

                double StartTimestamp = currentPacket.StartTime;
                double StopTimestamp = currentPacket.StopTime;

                var annotationStartIndex = bufferlist.FindIndex(obj => obj.TimeStamp == StartTimestamp);
                var stopIndex = bufferlist.FindIndex(obj => obj.TimeStamp == StopTimestamp);
                if (annotationStartIndex == -1 || stopIndex == -1)
                    continue;
                if (annotationStartIndex + 1 < bufferlist.Count)
                    annotationStartIndex += 1;
                annotationStopIndex = annotationStartIndex;
                var bus = new PolygonBusData();
                if (annotationStopIndex != -1)
                {
                    StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                    bus.StartIndex = TimeStampConverter(StartTimestamp) * 1e9;
                    bus.StopIndex = TimeStampConverter(StopTimestamp) * 1e9;
                    bus.Data = "S" + emptyVal;
                    bus.Brush = System.Windows.Media.Brushes.HotPink;

                    yield return bus;
                }
                annotationStopIndex = annotationStartIndex - 1;
                StartTimestamp = StopTimestamp;
                int currentIndex = annotationStopIndex;
                int firstedge = annotationStopIndex;
                int secondedge = 0;
                bool isRising = false;

                if (Config.Polarity == eSPIPolarity.Low && Config.Phase == eSPIPhase.Low)
                {
                    isRising = true;
                    edgeFinding.GetNextRisingEdgeIndex(clockChannel, annotationStopIndex, out secondedge);
                }
                else if (Config.Polarity == eSPIPolarity.Low && Config.Phase == eSPIPhase.High)
                {
                    isRising = false;
                    edgeFinding.GetNextFallingEdgeIndex(clockChannel, annotationStopIndex, out secondedge);
                }
                else if (Config.Polarity == eSPIPolarity.High && Config.Phase == eSPIPhase.Low)
                {
                    isRising = false;
                    edgeFinding.GetNextFallingEdgeIndex(clockChannel, annotationStopIndex, out secondedge);
                }
                else if (Config.Polarity == eSPIPolarity.High && Config.Phase == eSPIPhase.High)
                {
                    isRising = true;
                    edgeFinding.GetNextRisingEdgeIndex(clockChannel, annotationStopIndex, out secondedge);
                }

                double diff = bufferlist[secondedge].TimeStamp - bufferlist[firstedge].TimeStamp;

                int dataCount = Math.Max(currentPacket.MOSIDataBytes.Count, currentPacket.MISODataBytes.Count);

                for (int index = 0; index < dataCount; index++)
                {
                    if (config.Polarity == eSPIPolarity.Low && config.Phase == eSPIPhase.Low)
                        edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStopIndex, 8, eEdgeType.RISING_EDGE, out annotationStopIndex);
                    else if (config.Polarity == eSPIPolarity.Low && config.Phase == eSPIPhase.High)
                        edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStopIndex, 8, eEdgeType.FALLING_EDGE, out annotationStopIndex);
                    else if (config.Polarity == eSPIPolarity.High && config.Phase == eSPIPhase.Low)
                        edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStopIndex, 8, eEdgeType.FALLING_EDGE, out annotationStopIndex);
                    else if (config.Polarity == eSPIPolarity.High && config.Phase == eSPIPhase.High)
                        edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStopIndex, 8, eEdgeType.RISING_EDGE, out annotationStopIndex);

                    //annotationStopIndex += 1;
                    bus = new PolygonBusData();
                    if (annotationStopIndex != -1 && annotationStopIndex <= stopIndex)
                    {
                        //if (bufferlist[annotationStopIndex].TimeStamp - bufferlist[currentIndex].TimeStamp > (8 / currentPacket.Frequency))
                        //{
                        //    annotationStopIndex = GetPreviousEdge(annotationStopIndex, sPI_clockIndex, bufferlist);
                        //    annotationStopIndex = GetPreviousEdge(annotationStopIndex, sPI_clockIndex, bufferlist);
                        //    if (isRising == false)
                        //        annotationStopIndex = GetPreviousEdge(annotationStopIndex, sPI_clockIndex, bufferlist);
                        //}
                        StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                        bus.StartIndex = TimeStampConverter(StartTimestamp) * 1e9;
                        bus.StopIndex = TimeStampConverter(StopTimestamp) * 1e9;

                        if (currentPacket.MOSIDataBytes.Count != 0 && currentPacket.MISODataBytes.Count != 0)
                            bus.Data = "MOSI/MISO=" + "0x" + currentPacket.MOSIDataBytes[index].ToString("X2") + "/" + "0x" + currentPacket.MISODataBytes[index].ToString("X2") + emptyVal;
                        else if (currentPacket.MOSIDataBytes.Count != 0)
                            bus.Data = "MOSI=" + "0x" + currentPacket.MOSIDataBytes[index].ToString("X2") + emptyVal;
                        else if (currentPacket.MISODataBytes.Count != 0)
                            bus.Data = "MISO=" + "0x" + currentPacket.MISODataBytes[index].ToString("X2") + emptyVal;

                        bus.Brush = System.Windows.Media.Brushes.RoyalBlue;

                        yield return bus;
                    }
                    else
                    {
                        //if (annotationStopIndex != -1)
                        //annotationStopIndex = GetPreviousSPIStopIndex(annotationStopIndex, sPI_CSIndex, bufferlist);
                        //else
                        annotationStopIndex = stopIndex;
                        if (annotationStopIndex != -1)
                        {
                            StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                            bus.StartIndex = TimeStampConverter(StartTimestamp) * 1e9;
                            bus.StopIndex = TimeStampConverter(StopTimestamp) * 1e9;

                            if (currentPacket.MOSIDataBytes.Count != 0 && currentPacket.MISODataBytes.Count != 0)
                                bus.Data = "MOSI/MISO=" + "0x" + currentPacket.MOSIDataBytes[index].ToString("X2") + "/" + "0x" + currentPacket.MISODataBytes[index].ToString("X2") + emptyVal;
                            else if (currentPacket.MOSIDataBytes.Count != 0)
                                bus.Data = "MOSI=" + "0x" + currentPacket.MOSIDataBytes[index].ToString("X2") + emptyVal;
                            else if (currentPacket.MISODataBytes.Count != 0)
                                bus.Data = "MISO=" + "0x" + currentPacket.MISODataBytes[index].ToString("X2") + emptyVal;

                            bus.Brush = System.Windows.Media.Brushes.RoyalBlue;

                            yield return bus;
                        }
                    }
                    StartTimestamp = StopTimestamp;
                    currentIndex = annotationStopIndex;
                }
                if (currentPacket.HasStop)
                {
                    StartTimestamp = StopTimestamp;
                    annotationStopIndex = bufferlist.FindIndex(obj => obj.TimeStamp == StartTimestamp);
                    if (annotationStopIndex + 1 < bufferlist.Count)
                        annotationStopIndex = annotationStopIndex + 1;
                    if (annotationStopIndex != -1)
                    {
                        bus = new PolygonBusData();
                        StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                        bus.StartIndex = TimeStampConverter(StartTimestamp) * 1e9;
                        bus.StopIndex = TimeStampConverter(StopTimestamp) * 1e9;
                        bus.Data = "P" + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.HotPink;

                        yield return bus;
                    }
                }
            }
        }

        //void DrawSPIBus(Prodigy.Business.WfmEnum channel, int spi_startFrameIndex, int spi_stopFrameIndex)
        //{
        //    int sPI_clockIndex = (int)ConfigModel.GetInstance().SelectedSPI_CLK;
        //    int sPI_CSIndex = (int)ConfigModel.GetInstance().SelectedSPI_CS;
        //    int sPMI_clockIndex = (int)ConfigModel.GetInstance().SelectedSPMI_SCL;
        //    int sPMI_dataIndex = (int)ConfigModel.GetInstance().SelectedSPMI_SDA;
        //    prevIndexHolder[2] = spi_startFrameIndex;
        //    prevIndexHolder[3] = spi_stopFrameIndex;
        //    int lastEdge = 0;
        //    ProtocolBusPlotViewModel SPIbusPlot = new ProtocolBusPlotViewModel();

        //    SPIbusPlot.Tag = "SPI BUS";
        //    SPIbusPlot.VerticalScale = 4;
        //    if (busOffset.ContainsKey(eProtocol.SPI))
        //        SPIbusPlot.VerticalOffset = busOffset[eProtocol.SPI];
        //    else
        //        SPIbusPlot.VerticalOffset = -3.5;
        //    SPIbusPlot.Channel = channel;
        //    ObservableCollection<PolygonBusData> BusData = new ObservableCollection<PolygonBusData>();
        //    var annotationStopIndex = 0;
        //    string emptyVal = "  .";
        //    for (int j = spi_startFrameIndex; j <= spi_stopFrameIndex; j++)
        //    {
        //        var currentPacket = SPIResults.WareHouse.SPIFrameCollection[j];
        //        double StartTimestamp = SPITimeStampConverter(currentPacket.StartTime);
        //        double StopTimestamp = SPITimeStampConverter(currentPacket.StopTime);

        //        var annotationStartIndex = bufferlist.FindIndex(obj => obj.TimeStamp == StartTimestamp);
        //        var stopIndex = bufferlist.FindIndex(obj => obj.TimeStamp == StopTimestamp);
        //        if (annotationStartIndex == -1 || stopIndex == -1)
        //            continue;
        //        if (annotationStartIndex + 1 < bufferlist.Count)
        //            annotationStartIndex += 1;
        //        annotationStopIndex = annotationStartIndex;
        //        var bus = new PolygonBusData();
        //        if (annotationStopIndex != -1)
        //        {
        //            StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //            bus.StartIndex = StartTimestamp * 1e9;
        //            bus.StopIndex = StopTimestamp * 1e9;
        //            bus.Data = "S" + emptyVal;
        //            bus.Brush = System.Windows.Media.Brushes.HotPink;
        //            BusData.Add(bus);
        //        }
        //        annotationStopIndex = annotationStartIndex - 1;
        //        StartTimestamp = StopTimestamp;
        //        int currentIndex = annotationStopIndex;
        //        int firstedge = annotationStopIndex;
        //        int secondedge = 0;
        //        bool isRising = false;

        //        if (ConfigModel_SPI.GetInstance().Polarity == eSPIPolarity.Low && ConfigModel_SPI.GetInstance().Phase == eSPIPhase.Low)
        //        {
        //            isRising = true;
        //            secondedge = GetNextRisingEdgeIndexByCount(annotationStopIndex, sPI_clockIndex, bufferlist, 1, ref lastEdge);
        //        }
        //        else if (ConfigModel_SPI.GetInstance().Polarity == eSPIPolarity.Low && ConfigModel_SPI.GetInstance().Phase == eSPIPhase.High)
        //        {
        //            isRising = false;
        //            secondedge = GetNextFallingEdgeIndexByCount(annotationStopIndex, sPI_clockIndex, bufferlist, 1, ref lastEdge);
        //        }
        //        else if (ConfigModel_SPI.GetInstance().Polarity == eSPIPolarity.High && ConfigModel_SPI.GetInstance().Phase == eSPIPhase.Low)
        //        {
        //            isRising = false;
        //            secondedge = GetNextFallingEdgeIndexByCount(annotationStopIndex, sPI_clockIndex, bufferlist, 1, ref lastEdge);
        //        }
        //        else if (ConfigModel_SPI.GetInstance().Polarity == eSPIPolarity.High && ConfigModel_SPI.GetInstance().Phase == eSPIPhase.High)
        //        {
        //            isRising = true;
        //            secondedge = GetNextRisingEdgeIndexByCount(annotationStopIndex, sPI_clockIndex, bufferlist, 1, ref lastEdge);
        //        }

        //        double diff = bufferlist[secondedge].TimeStamp - bufferlist[firstedge].TimeStamp;

        //        int dataCount = Math.Max(currentPacket.MOSIDataBytes.Count, currentPacket.MISODataBytes.Count);

        //        for (int index = 0; index < dataCount; index++)
        //        {
        //            if (ConfigModel_SPI.GetInstance().Polarity == eSPIPolarity.Low && ConfigModel_SPI.GetInstance().Phase == eSPIPhase.Low)
        //                annotationStopIndex = GetNextRisingEdgeIndexByCount(annotationStopIndex, sPI_clockIndex, bufferlist, 8, ref lastEdge);
        //            else if (ConfigModel_SPI.GetInstance().Polarity == eSPIPolarity.Low && ConfigModel_SPI.GetInstance().Phase == eSPIPhase.High)
        //                annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStopIndex, sPI_clockIndex, bufferlist, 8, ref lastEdge);
        //            else if (ConfigModel_SPI.GetInstance().Polarity == eSPIPolarity.High && ConfigModel_SPI.GetInstance().Phase == eSPIPhase.Low)
        //                annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStopIndex, sPI_clockIndex, bufferlist, 8, ref lastEdge);
        //            else if (ConfigModel_SPI.GetInstance().Polarity == eSPIPolarity.High && ConfigModel_SPI.GetInstance().Phase == eSPIPhase.High)
        //                annotationStopIndex = GetNextRisingEdgeIndexByCount(annotationStopIndex, sPI_clockIndex, bufferlist, 8, ref lastEdge);

        //            //annotationStopIndex += 1;
        //            bus = new PolygonBusData();
        //            if (annotationStopIndex != -1 && annotationStopIndex <= stopIndex)
        //            {
        //                //if (bufferlist[annotationStopIndex].TimeStamp - bufferlist[currentIndex].TimeStamp > (8 / currentPacket.Frequency))
        //                //{
        //                //    annotationStopIndex = GetPreviousEdge(annotationStopIndex, sPI_clockIndex, bufferlist);
        //                //    annotationStopIndex = GetPreviousEdge(annotationStopIndex, sPI_clockIndex, bufferlist);
        //                //    if (isRising == false)
        //                //        annotationStopIndex = GetPreviousEdge(annotationStopIndex, sPI_clockIndex, bufferlist);
        //                //}
        //                StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //                bus.StartIndex = StartTimestamp * 1e9;
        //                bus.StopIndex = StopTimestamp * 1e9;

        //                if (currentPacket.MOSIDataBytes.Count != 0 && currentPacket.MISODataBytes.Count != 0)
        //                    bus.Data = "MOSI/MISO=" + "0x" + currentPacket.MOSIDataBytes[index].ToString("X2") + "/" + "0x" + currentPacket.MISODataBytes[index].ToString("X2") + emptyVal;
        //                else if (currentPacket.MOSIDataBytes.Count != 0)
        //                    bus.Data = "MOSI=" + "0x" + currentPacket.MOSIDataBytes[index].ToString("X2") + emptyVal;
        //                else if (currentPacket.MISODataBytes.Count != 0)
        //                    bus.Data = "MISO=" + "0x" + currentPacket.MISODataBytes[index].ToString("X2") + emptyVal;

        //                bus.Brush = System.Windows.Media.Brushes.RoyalBlue;
        //                BusData.Add(bus);
        //            }
        //            else
        //            {
        //                //if (annotationStopIndex != -1)
        //                //annotationStopIndex = GetPreviousSPIStopIndex(annotationStopIndex, sPI_CSIndex, bufferlist);
        //                //else
        //                annotationStopIndex = stopIndex;
        //                if (annotationStopIndex != -1)
        //                {
        //                    StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //                    bus.StartIndex = StartTimestamp * 1e9;
        //                    bus.StopIndex = StopTimestamp * 1e9;

        //                    if (currentPacket.MOSIDataBytes.Count != 0 && currentPacket.MISODataBytes.Count != 0)
        //                        bus.Data = "MOSI/MISO=" + "0x" + currentPacket.MOSIDataBytes[index].ToString("X2") + "/" + "0x" + currentPacket.MISODataBytes[index].ToString("X2") + emptyVal;
        //                    else if (currentPacket.MOSIDataBytes.Count != 0)
        //                        bus.Data = "MOSI=" + "0x" + currentPacket.MOSIDataBytes[index].ToString("X2") + emptyVal;
        //                    else if (currentPacket.MISODataBytes.Count != 0)
        //                        bus.Data = "MISO=" + "0x" + currentPacket.MISODataBytes[index].ToString("X2") + emptyVal;

        //                    bus.Brush = System.Windows.Media.Brushes.RoyalBlue;
        //                    BusData.Add(bus);
        //                }
        //            }
        //            StartTimestamp = StopTimestamp;
        //            currentIndex = annotationStopIndex;
        //        }
        //        if (currentPacket.HasStop)
        //        {
        //            StartTimestamp = StopTimestamp;
        //            annotationStopIndex = bufferlist.FindIndex(obj => obj.TimeStamp == StartTimestamp);
        //            if (annotationStopIndex + 1 < bufferlist.Count)
        //                annotationStopIndex = annotationStopIndex + 1;
        //            if (annotationStopIndex != -1)
        //            {
        //                bus = new PolygonBusData();
        //                StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //                bus.StartIndex = StartTimestamp * 1e9;
        //                bus.StopIndex = StopTimestamp * 1e9;
        //                bus.Data = "P" + emptyVal;
        //                bus.Brush = System.Windows.Media.Brushes.HotPink;
        //                BusData.Add(bus);
        //            }
        //        }
        //    }
        //    foreach (var bus in BusData)
        //        SPIbusPlot.BusFullData.Add(bus);
        //    System.Windows.Application.Current.Dispatcher.Invoke(
        //new Action(() => this.InfoTrend.Trend.BusResultInfoView = SPIbusPlot));
        //    if (BusData.Count == 0)
        //    {
        //        prevIndexHolder[2] = -1;
        //        prevIndexHolder[3] = -1;
        //    }
        //}

        //public async void PlotSPIBusDiagram(string param)
        //{
        //    try
        //    {
        //        if (!isspiBusy)
        //        {
        //            isspiBusy = true;
        //            IsLoading = true;
        //            int sPI_clockIndex = (int)ConfigModel.GetInstance().SelectedSPI_CLK;
        //            int sPI_CSIndex = (int)ConfigModel.GetInstance().SelectedSPI_CS;
        //            //SPI
        //            if (configuration.Config.IsSPISelected || configuration.Config.IsAllSelected)
        //            {
        //                if (SPIResults.WareHouse.SelectedFrame == null)
        //                    return;

        //                int SelectedIndex = SPIResults.WareHouse.SelectedFrame.FrameIndex;
        //                int ResCount = SPIResults.WareHouse.SPIFrameCollection.Count - 1;
        //                int startFrameIndex = (SelectedIndex - 10 < 0) ? 0 : SelectedIndex - 10;
        //                int stopFrameIndex = (SelectedIndex + 10 > ResCount) ? ResCount : SelectedIndex + 10;

        //                double start = SPIResults.WareHouse.SPIFrameCollection[startFrameIndex].StartTime;
        //                double stop = SPIResults.WareHouse.SPIFrameCollection[stopFrameIndex].StopTime == 0 ? SPIResults.WareHouse.SPIFrameCollection[stopFrameIndex].StartTime : SPIResults.WareHouse.SPIFrameCollection[stopFrameIndex].StopTime;

        //                int lastEdge = 0;

        //                if (!(SelectedIndex >= prevIndexHolder[2] && SelectedIndex <= prevIndexHolder[3]) || (prevIndexHolder[2] == -1 && prevIndexHolder[3] == -1) || LastProtocolClicked != eProtocol.SPI)
        //                {
        //                    if (!CheckTimestampExist(eProtocol.SPI) || LastProtocolClicked != eProtocol.SPI)
        //                    {
        //                        //if timestamp does not exist in buffer, again read from file and replot
        //                        DiscreteWaveForm[] buffer = await dataReceiver.GetWaveformBufferTimestampAsync(start, stop);
        //                        bufferlist = new List<DiscreteWaveForm>();
        //                        bufferlist = buffer.Select(p => new DiscreteWaveForm { ChannelValue = p.ChannelValue, TimeStamp = TimeStampConverter(p.TimeStamp) }).ToList();
        //                        if (bufferlist.Count > 0)
        //                            PlotWaveform(bufferlist);
        //                        else
        //                            return;
        //                    }
        //                    else
        //                    {
        //                        //Take only required edges to avoid delay in rendering
        //                        bufferlist = bufferlist.Where(x => x.TimeStamp >= TimeStampConverter(start) && x.TimeStamp <= TimeStampConverter(stop)).ToList();
        //                    }

        //                    if (bufferlist.Count == 0)
        //                        return;
        //                    prevIndexHolder[2] = startFrameIndex;
        //                    prevIndexHolder[3] = stopFrameIndex;

        //                    ProtocolBusPlotViewModel SPIbusPlot = new ProtocolBusPlotViewModel();

        //                    SPIbusPlot.Tag = "SPI BUS";
        //                    SPIbusPlot.VerticalScale = 4;
        //                    if (busOffset.ContainsKey(eProtocol.SPI))
        //                        SPIbusPlot.VerticalOffset = busOffset[eProtocol.SPI];
        //                    else
        //                        SPIbusPlot.VerticalOffset = -3.5;
        //                    SPIbusPlot.Channel = Prodigy.Business.WfmEnum.CH14;
        //                    ObservableCollection<PolygonBusData> BusData = new ObservableCollection<PolygonBusData>();
        //                    var annotationStopIndex = 0;
        //                    string emptyVal = "  .";
        //                    for (int j = startFrameIndex; j <= stopFrameIndex; j++)
        //                    {
        //                        var currentPacket = SPIResults.WareHouse.SPIFrameCollection[j];
        //                        double StartTimestamp = SPITimeStampConverter(currentPacket.StartTime);
        //                        double StopTimestamp = SPITimeStampConverter(currentPacket.StopTime);

        //                        var annotationStartIndex = bufferlist.FindIndex(obj => obj.TimeStamp == StartTimestamp);
        //                        var stopIndex = bufferlist.FindIndex(obj => obj.TimeStamp == StopTimestamp);
        //                        if (annotationStartIndex == -1 || stopIndex == -1)
        //                            continue;
        //                        if (annotationStartIndex + 1 < bufferlist.Count)
        //                            annotationStartIndex += 1;
        //                        annotationStopIndex = annotationStartIndex;
        //                        var bus = new PolygonBusData();
        //                        if (annotationStopIndex != -1)
        //                        {
        //                            StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //                            bus.StartIndex = StartTimestamp * 1e9;
        //                            bus.StopIndex = StopTimestamp * 1e9;
        //                            bus.Data = "S" + emptyVal;
        //                            bus.Brush = System.Windows.Media.Brushes.HotPink;
        //                            BusData.Add(bus);
        //                        }
        //                        annotationStopIndex = annotationStartIndex - 1;
        //                        StartTimestamp = StopTimestamp;
        //                        int currentIndex = annotationStopIndex;
        //                        int firstedge = annotationStopIndex;
        //                        int secondedge = 0;
        //                        bool isRising = false;

        //                        if (ConfigModel_SPI.GetInstance().Polarity == eSPIPolarity.Low && ConfigModel_SPI.GetInstance().Phase == eSPIPhase.Low)
        //                        {
        //                            isRising = true;
        //                            secondedge = GetNextRisingEdgeIndexByCount(annotationStopIndex, sPI_clockIndex, bufferlist, 1, ref lastEdge);
        //                        }
        //                        else if (ConfigModel_SPI.GetInstance().Polarity == eSPIPolarity.Low && ConfigModel_SPI.GetInstance().Phase == eSPIPhase.High)
        //                        {
        //                            isRising = false;
        //                            secondedge = GetNextFallingEdgeIndexByCount(annotationStopIndex, sPI_clockIndex, bufferlist, 1, ref lastEdge);
        //                        }
        //                        else if (ConfigModel_SPI.GetInstance().Polarity == eSPIPolarity.High && ConfigModel_SPI.GetInstance().Phase == eSPIPhase.Low)
        //                        {
        //                            isRising = false;
        //                            secondedge = GetNextFallingEdgeIndexByCount(annotationStopIndex, sPI_clockIndex, bufferlist, 1, ref lastEdge);
        //                        }
        //                        else if (ConfigModel_SPI.GetInstance().Polarity == eSPIPolarity.High && ConfigModel_SPI.GetInstance().Phase == eSPIPhase.High)
        //                        {
        //                            isRising = true;
        //                            secondedge = GetNextRisingEdgeIndexByCount(annotationStopIndex, sPI_clockIndex, bufferlist, 1, ref lastEdge);
        //                        }

        //                        double diff = bufferlist[secondedge].TimeStamp - bufferlist[firstedge].TimeStamp;

        //                        int dataCount = Math.Max(currentPacket.MOSIDataBytes.Count, currentPacket.MISODataBytes.Count);

        //                        for (int index = 0; index < dataCount; index++)
        //                        {
        //                            if (ConfigModel_SPI.GetInstance().Polarity == eSPIPolarity.Low && ConfigModel_SPI.GetInstance().Phase == eSPIPhase.Low)
        //                                annotationStopIndex = GetNextRisingEdgeIndexByCount(annotationStopIndex, sPI_clockIndex, bufferlist, 8, ref lastEdge);
        //                            else if (ConfigModel_SPI.GetInstance().Polarity == eSPIPolarity.Low && ConfigModel_SPI.GetInstance().Phase == eSPIPhase.High)
        //                                annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStopIndex, sPI_clockIndex, bufferlist, 8, ref lastEdge);
        //                            else if (ConfigModel_SPI.GetInstance().Polarity == eSPIPolarity.High && ConfigModel_SPI.GetInstance().Phase == eSPIPhase.Low)
        //                                annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStopIndex, sPI_clockIndex, bufferlist, 8, ref lastEdge);
        //                            else if (ConfigModel_SPI.GetInstance().Polarity == eSPIPolarity.High && ConfigModel_SPI.GetInstance().Phase == eSPIPhase.High)
        //                                annotationStopIndex = GetNextRisingEdgeIndexByCount(annotationStopIndex, sPI_clockIndex, bufferlist, 8, ref lastEdge);

        //                            //annotationStopIndex += 1;
        //                            bus = new PolygonBusData();
        //                            if (annotationStopIndex != -1 && annotationStopIndex <= stopIndex)
        //                            {
        //                                //if (bufferlist[annotationStopIndex].TimeStamp - bufferlist[currentIndex].TimeStamp > (8 / currentPacket.Frequency))
        //                                //{
        //                                //    annotationStopIndex = GetPreviousEdge(annotationStopIndex, sPI_clockIndex, bufferlist);
        //                                //    annotationStopIndex = GetPreviousEdge(annotationStopIndex, sPI_clockIndex, bufferlist);
        //                                //    if (isRising == false)
        //                                //        annotationStopIndex = GetPreviousEdge(annotationStopIndex, sPI_clockIndex, bufferlist);
        //                                //}
        //                                StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //                                bus.StartIndex = StartTimestamp * 1e9;
        //                                bus.StopIndex = StopTimestamp * 1e9;

        //                                if (currentPacket.MOSIDataBytes.Count != 0 && currentPacket.MISODataBytes.Count != 0)
        //                                    bus.Data = "MOSI/MISO=" + "0x" + currentPacket.MOSIDataBytes[index].ToString("X2") + "/" + "0x" + currentPacket.MISODataBytes[index].ToString("X2") + emptyVal;
        //                                else if (currentPacket.MOSIDataBytes.Count != 0)
        //                                    bus.Data = "MOSI=" + "0x" + currentPacket.MOSIDataBytes[index].ToString("X2") + emptyVal;
        //                                else if (currentPacket.MISODataBytes.Count != 0)
        //                                    bus.Data = "MISO=" + "0x" + currentPacket.MISODataBytes[index].ToString("X2") + emptyVal;

        //                                bus.Brush = System.Windows.Media.Brushes.RoyalBlue;
        //                                BusData.Add(bus);
        //                            }
        //                            else
        //                            {
        //                                if (annotationStopIndex != -1)
        //                                    annotationStopIndex = GetPreviousSPIStopIndex(annotationStopIndex, sPI_CSIndex, bufferlist);
        //                                else
        //                                    annotationStopIndex = stopIndex;
        //                                if (annotationStopIndex != -1)
        //                                {
        //                                    StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //                                    bus.StartIndex = StartTimestamp * 1e9;
        //                                    bus.StopIndex = StopTimestamp * 1e9;

        //                                    if (currentPacket.MOSIDataBytes.Count != 0 && currentPacket.MISODataBytes.Count != 0)
        //                                        bus.Data = "MOSI/MISO=" + "0x" + currentPacket.MOSIDataBytes[index].ToString("X2") + "/" + "0x" + currentPacket.MISODataBytes[index].ToString("X2") + emptyVal;
        //                                    else if (currentPacket.MOSIDataBytes.Count != 0)
        //                                        bus.Data = "MOSI=" + "0x" + currentPacket.MOSIDataBytes[index].ToString("X2") + emptyVal;
        //                                    else if (currentPacket.MISODataBytes.Count != 0)
        //                                        bus.Data = "MISO=" + "0x" + currentPacket.MISODataBytes[index].ToString("X2") + emptyVal;

        //                                    bus.Brush = System.Windows.Media.Brushes.RoyalBlue;
        //                                    BusData.Add(bus);
        //                                }
        //                            }
        //                            StartTimestamp = StopTimestamp;
        //                            currentIndex = annotationStopIndex;
        //                        }
        //                        if (currentPacket.HasStop)
        //                        {
        //                            StartTimestamp = StopTimestamp;
        //                            annotationStopIndex = bufferlist.FindIndex(obj => obj.TimeStamp == StartTimestamp);
        //                            if (annotationStopIndex + 1 < bufferlist.Count)
        //                                annotationStopIndex = annotationStopIndex + 1;
        //                            if (annotationStopIndex != -1)
        //                            {
        //                                bus = new PolygonBusData();
        //                                StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //                                bus.StartIndex = StartTimestamp * 1e9;
        //                                bus.StopIndex = StopTimestamp * 1e9;
        //                                bus.Data = "P" + emptyVal;
        //                                bus.Brush = System.Windows.Media.Brushes.HotPink;
        //                                BusData.Add(bus);
        //                            }
        //                        }
        //                    }
        //                    foreach (var bus in BusData)
        //                        SPIbusPlot.BusFullData.Add(bus);
        //                    System.Windows.Application.Current.Dispatcher.Invoke(
        //                new Action(() => this.InfoTrend.Trend.BusResultInfoView = SPIbusPlot));
        //                    if (BusData.Count == 0)
        //                    {
        //                        prevIndexHolder[2] = -1;
        //                        prevIndexHolder[3] = -1;
        //                    }
        //                }
        //                LastProtocolClicked = eProtocol.SPI;
        //                var fstart = SPITimeStampConverter(SPIResults.WareHouse.SelectedFrame.StartTime);
        //                var fstop = SPITimeStampConverter(SPIResults.WareHouse.SelectedFrame.StopTime);

        //                if (SPIResults.WareHouse.SelectedFrame.Frequency != 0)
        //                {
        //                    fstart = SPITimeStampConverter(SPIResults.WareHouse.SelectedFrame.StartTime) - (0.5 / SPIResults.WareHouse.SelectedFrame.Frequency);
        //                    if (fstart > bufferlist.FirstOrDefault().TimeStamp)
        //                        fstart = SPITimeStampConverter(SPIResults.WareHouse.SelectedFrame.StartTime);

        //                    fstop = SPITimeStampConverter(SPIResults.WareHouse.SelectedFrame.StopTime) + (0.75 / SPIResults.WareHouse.SelectedFrame.Frequency);
        //                    if (fstop > bufferlist.LastOrDefault().TimeStamp)
        //                        fstop = SPITimeStampConverter(SPIResults.WareHouse.SelectedFrame.StopTime);
        //                }

        //                this.infoTrend.Trend.WfmShowingStartIndex = (long)(fstart * 1e9);
        //                this.infoTrend.Trend.WfmShowingStopIndex = (long)(fstop * 1e9);
        //                this.infoTrend.Trend.DisplayLimited = !this.infoTrend.Trend.DisplayLimited;
        //            }
        //            IsLoading = false;
        //            isspiBusy = false;
        //        }
        //    }
        //    catch { }
        //    finally
        //    {
        //        IsLoading = false;
        //        isspiBusy = false;
        //        ResetSelection(eProtocol.SPI);
        //    }
        //}

        #endregion
    }

    public class SearchFilterViewModel_SPI : SearchFilterViewModelBase<IFrame>
    {
        public SearchFilterViewModel_SPI(CollectionViewSource source) : base(source)
        {
        }

        public SearchFilterViewModel_SPI(System.Collections.IList collection) : base(collection) { }

        private string mosi;

        public string MOSI
        {
            get { return mosi; }
            set
            {
                mosi = value;
                OnPropertyChanged(nameof(MOSI));
            }
        }

        private string miso;

        public string MISO
        {
            get => miso;
            set
            {
                miso = value;
                OnPropertyChanged(nameof(MISO));
            }
        }

        public override bool IsMatch(IFrame frame)
        {
            var spiFrame = (SPIFrame)frame;

            List<string> MOSIList = new List<string>();
            if (!string.IsNullOrEmpty(MOSI))
            {
                if (CheckHexa(MOSI.ToLower()))
                {
                    foreach (var db in spiFrame.MOSIDataBytes)
                    {
                        MOSIList.Add($"0x{Convert.ToInt32(db):X}");
                    }
                }
                else
                {
                    foreach (var db in spiFrame.MOSIDataBytes)
                    {
                        MOSIList.Add($"{Convert.ToInt32(db):X}");
                    }

                }
            }


            List<string> MISOList = new List<string>();
            if (!string.IsNullOrEmpty(MISO))
            {
                if (CheckHexa(MISO.ToLower()))
                {
                    foreach (var db in spiFrame.MISODataBytes)
                    {
                        MISOList.Add($"0x{Convert.ToInt32(db):X}");
                    }
                }
                else
                {
                    foreach (var db in spiFrame.MISODataBytes)
                    {
                        MISOList.Add($"{Convert.ToInt32(db):X}");
                    }

                }
            }
            List<string> MOSIData = new List<string>();
            foreach (var item in MOSIList)
            {
                var num = item.ToLower();
                MOSIData.Add(num);
            }

            List<string> MISOData = new List<string>();
            foreach (var item in MISOList)
            {
                var num1 = item.ToLower();
                MISOData.Add(num1);
            }

            bool result = true;

            bool searchValid = false;

            if (!string.IsNullOrEmpty(MOSI))
            {
                result &= MOSIData.Contains(MOSI.ToLower());

                searchValid = true;
            }
            if (!string.IsNullOrEmpty(MISO))
            {
                result &= MISOData.Contains(MISO.ToLower());

                searchValid = true;
            }


            return result && searchValid;
        }

        public bool CheckHexa(string data)
        {
            return data.Contains("x");
        }
    }
}
