using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Helpers;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using Prodigy.Business;
using Prodigy.WaveformControls;
using Prodigy.WaveformControls.Interfaces;
using ProdigyFramework.Behavior;
using ProdigyFramework.Collections;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PGYMiniCooper.CoreModule.ViewModel
{
    public class TimingPlotViewModel : ViewModelBase, IResetViewModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private TrendInfoViewModel infoTrend;
        IDataProvider dataReceiver;
        ConfigurationViewModel configuration;
        ResultsViewModel resultsViewModel;
        bool isTrigger = false;

        /// <summary>
        /// Warning - Only for serialization
        /// </summary>
        public TimingPlotViewModel():this(new ConfigurationViewModel(), new ResultsViewModel()) { }

        public TimingPlotViewModel(ConfigurationViewModel configuration, ResultsViewModel resultsViewModel)
        {
            this.configuration = configuration;
            this.resultsViewModel = resultsViewModel;

            InfoTrend = new TrendInfoViewModel();
            dataReceiver = Ioc.Default.GetService<IDataProvider>();
        }

        public void Initialize()
        {
            lastChannel = WfmEnum.CH17;

            dataReceiver.OnTriggerOccur += DataReceiver_OnTriggerOccur;
            dataReceiver.WaveformAvailable += OnNewDataReceived;

            resultsViewModel.ResultAdded += ResultsViewModel_ResultAdded;

            if (configuration.ConfigurationMode == eConfigMode.LA_Mode)
            {
                if (configuration.GroupType == eGroupType.Group)
                {
                    // Create channel group bus 
                    for (int i = 0; i < configuration.ProtocolConfiguration.Count; i++)
                    {
                        var channelGroup = configuration.ProtocolConfiguration[i] as ConfigViewModel_Group;

                        if (channelGroup.Channels.Count > 0)
                        {
                            // Add clock
                            if (i < 3)
                            {
                                if (configuration.SelectedClock1 != eChannles.None)
                                {
                                    CreatePlot(new ChannelInfo("CLK1", configuration.SelectedClock1));
                                }
                            }
                            else
                            {
                                if (configuration.HasTwoClockSources && configuration.SelectedClock2 != eChannles.None)
                                {
                                    CreatePlot(new ChannelInfo("CLK2", configuration.SelectedClock2));
                                }
                            }

                            // Add data channels
                            foreach (var channel in channelGroup.Channels.Where(c => c.ChannelIndex != eChannles.None))
                            {
                                CreatePlot(channel);
                            }

                            // Add bus
                            ProtocolBusPlotViewModel busPlot = new ProtocolBusPlotViewModel
                                ((startIndex, stopIndex) =>
                                {
                                    return GetBusDiagram_Group(channelGroup, startIndex, stopIndex);
                                });
                            busPlot.Tag = channelGroup.Name;

                            busPlot.Channel = lastChannel++;
                            System.Windows.Application.Current.Dispatcher.Invoke(() => this.InfoTrend.Trend.AddWaveform(busPlot));
                        }
                    }
                }
                else
                {
                    // Create channel plot individual 
                    foreach (var channel in configuration.ProtocolConfiguration.SelectMany(c => c.Channels).Where(c => c.ChannelIndex != eChannles.None))
                    {
                        CreatePlot(channel);
                    }
                }
            }
        }

        private WfmEnum lastChannel = WfmEnum.CH17;

        private void ResultsViewModel_ResultAdded(IResultViewModel result)
        {
            try
            {
                var busChannels = result.AvailableBusChannels;

                // Initialize plot
                foreach (var channel in result.Config.Channels.Where(c => c.ChannelIndex != eChannles.None))
                {
                    CreatePlot(channel);

                    var busChannel = busChannels.FirstOrDefault(c => c.ChannelIndex == channel.ChannelIndex);

                    if (busChannel != null)
                    {
                        string busName = $"Bus_{busChannel.ChannelName}";
                        CreateBusPlot(busChannel, busName, result);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void CreateBusPlot(ChannelInfo busChannel, string busName, IResultViewModel result)
        {
            ProtocolBusPlotViewModel busPlot = new ProtocolBusPlotViewModel
                                ((startIndex, stopIndex) =>
                                {
                                    try
                                    {
                                        return result.GetBusDiagram(busChannel, startIndex / 1e9 + SessionConfiguration.TriggerTime, stopIndex / 1e9 + SessionConfiguration.TriggerTime);
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error(ex);
                                        throw ex;
                                    }
                                });

            busPlot.Tag = busName;

            busPlot.Channel = lastChannel++;
            System.Windows.Application.Current.Dispatcher.Invoke(() => this.InfoTrend.Trend.AddWaveform(busPlot));
        }

        private bool busUpdateRequired = true;

        private bool isActive = false;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (configuration.ConfigurationMode != eConfigMode.PA_Mode)
                {
                    if (IsActive == false && value)
                    {
                        // TODO: remove this temporary fix
                        RefreshPlot(true);
                    }
                }

                isActive = value;
                RaisePropertyChanged(nameof(IsActive));

                if (value && busUpdateRequired)
                {
                    busUpdateRequired = false;

                    IsLoading = true;
                    Task.Run(() =>
                    {
                        if (InfoTrend != null)
                            InfoTrend.Trend.IsDirty = true;

                        IsLoading = false;
                    });
                }
            }
        }

        private eDataFormat dataFormat = eDataFormat.Hex;
        public eDataFormat DataFormat
        {
            get
            {
                return dataFormat;
            }
            set
            {
                dataFormat = value;
                RaisePropertyChanged("DataFormat");
            }
        }
        public ICommand BusValueConverter
        {
            get
            {
                return new RelayCommand<string>(ViewSelection);
            }
        }

        void ViewSelection(string param)
        {
            //if (configuration.ConfigurationMode == eConfigMode.LA_Mode && configuration.Config.GeneralPurposeMode == eGeneralPurpose.State)
            //{
            //    //InfoTrend.Trend.MinimumIndex = (long)(bufferlist.FirstOrDefault().TimeStamp * 1e9);
            //    //InfoTrend.Trend.MaximumIndex = (bufferlist.Last().TimeStamp * 1e9);
            //    LastSavedWfmStart = InfoTrend.Trend.WfmShowingStartIndex;
            //    LastSavedWfmEnd = InfoTrend.Trend.WfmShowingStopIndex;
            //    //InfoTrend.Trend.PlotEvent = Prodigy.WaveformControls.View.PlotEvents.FIT;
            //   // Thread.Sleep(500);
            //    PlotWaveform(bufferlist, false);
            //    this.infoTrend.Trend.WfmShowingStartIndex = LastSavedWfmStart;
            //    this.infoTrend.Trend.WfmShowingStopIndex = LastSavedWfmEnd;
            //    this.infoTrend.Trend.DisplayLimited = !this.infoTrend.Trend.DisplayLimited;
            //}
        }

        private void DataReceiver_OnTriggerOccur()
        {
            TriggerPosition = SessionConfiguration.TriggerTime * 1e9;

            if (!(TriggerModel.GetInstance().TriggerType == eTriggerTypeList.Auto) && dataReceiver.TriggerAlreadyfound == true)
            {
                infoTrend.Trend.IsTVisible = true;

                IsLoading = true;
                Task.Run(() =>
                {
                    infoTrend.Trend.IsDirty = true;

                    IsLoading = false;
                });
            }
        }

        private bool fullwfmlabel = false;
        public bool Fullwfmlabel
        {
            get
            {
                return fullwfmlabel;
            }
            set
            {
                fullwfmlabel = value;
                RaisePropertyChanged("Fullwfmlabel");
            }
        }

        private double triggerPosition = 0;
        public double TriggerPosition
        {
            get { return triggerPosition; }
            set { triggerPosition = value; RaisePropertyChanged("TriggerPosition"); }
        }

        private double stopTime = 0;
        public double StopTime
        {
            get { return stopTime; }
            set { stopTime = value; RaisePropertyChanged("StopTime"); }
        }
        private double startTime = 00;
        public double StartTime
        {
            get { return startTime; }
            set { startTime = value; RaisePropertyChanged("StartTime"); }
        }

        private long maximum = 0;
        public long Maximum
        {
            get { return maximum; }
            set { maximum = value; RaisePropertyChanged("Maximum"); }
        }
        private int minimum = 00;
        public int Minimum
        {
            get { return minimum; }
            set { minimum = value; RaisePropertyChanged("Minimum"); }
        }
        private double scrollValue = 00;

        public double ScrollValue
        {
            get { return scrollValue; }
            set
            {
                scrollValue = value;
                RaisePropertyChanged("ScrollValue");
            }
        }

        private double viewPortSize = double.NaN;
        public double ViewPortSize
        {
            get { return viewPortSize; }
            set
            {
                viewPortSize = value;
                RaisePropertyChanged("ViewPortSize");
            }
        }


        bool isProcessing = false;
        public async void ScrollbarMouseUpEvent()
        {
            if (!isProcessing && configuration.ConfigurationMode != eConfigMode.PA_Mode)
            {
                isProcessing = true;
                IsLoading = true;
                await Task.Run(() =>
                {
                    try
                    {
                        //if (dataReceiver.TotalWaveformPoints > 0)
                        {

                            double wfmWidth = InfoTrend.Trend.WfmShowingStopIndex - InfoTrend.Trend.WfmShowingStartIndex;
                            double newStart = ScrollValue - wfmWidth / 2d;
                            double newStop = ScrollValue + wfmWidth / 2d;

                            if (InfoTrend.Trend.MinimumIndex > newStart)
                                newStart = InfoTrend.Trend.MinimumIndex;
                            if (InfoTrend.Trend.MaximumIndex < newStop)
                                newStop = InfoTrend.Trend.MaximumIndex;

                            InfoTrend.Trend.WfmShowingStartIndex = newStart;
                            InfoTrend.Trend.WfmShowingStopIndex = newStop;

                            InfoTrend.Trend.IsDirty = true;
                        }
                    }
                    catch { }
                    finally
                    {
                        isProcessing = false;
                        IsLoading = false;
                    }
                });
            }

        }

        public Command ScrollEvent
        {
            get
            {
                return new Command(new Command.ICommandOnExecute(ScrollbarMouseUpEvent));
            }
        }

        public Command GotoTriggerPosition
        {
            get
            {
                return new Command(new Command.ICommandOnExecute(GoToTrigger));
            }
        }

        public async void GoToTrigger()
        {
            if (!isProcessing && SessionConfiguration.TriggersetTView)
            {
                isProcessing = true;
                IsLoading = true;

                // trigger time is time at 0 sec.
                this.infoTrend.Trend.WfmShowingStartIndex = 0;
                this.infoTrend.Trend.WfmShowingStopIndex = this.InfoTrend.Trend.HorizontalZoomLimit;
                this.infoTrend.Trend.IsDirty = true;
                isProcessing = false;
                IsLoading = false;
            }

        }

        private bool isLoading = false;
        public bool IsLoading
        {
            get { return this.isLoading; }
            protected set
            {
                this.isLoading = value;
                this.RaisePropertyChanged("IsLoading");
            }
        }

        bool isUpdating = false;
        private void OnNewDataReceived(object sender, WaveformAvailableEventArgs e)
        {
            if (isUpdating == false && configuration.ConfigurationMode != eConfigMode.PA_Mode)
            {
                IsLoading = true;
                isUpdating = true;
                SessionConfiguration.IsAnimate = true;

                Task.Run(() =>
                {
                    try
                    {
                        InfoTrend.Trend.MinimumIndex = TimeStampConverter(e.New.StartTime) * 1e9;
                        InfoTrend.Trend.MaximumIndex = TimeStampConverter(e.New.EndTime) * 1e9;

                        if (e.Old == null || e.New.MaxEdges < 1e6 || e.Old.MaxEdges < 1e6)
                        {
                            InfoTrend.Trend.HorizontalZoomLimit = InfoTrend.Trend.MaximumIndex - InfoTrend.Trend.MinimumIndex;

                            var timeBetweenEdges = (InfoTrend.Trend.MaximumIndex - InfoTrend.Trend.MinimumIndex) / e.New.MaxEdges;
                            if (e.New.MaxEdges > 1e6)
                            {
                                InfoTrend.Trend.HorizontalZoomLimit = 1e6 * timeBetweenEdges;
                                InfoTrend.Trend.WfmShowingStartIndex = InfoTrend.Trend.MinimumIndex;
                                InfoTrend.Trend.WfmShowingStopIndex = InfoTrend.Trend.MinimumIndex + InfoTrend.Trend.HorizontalZoomLimit;
                            }

                            RefreshPlot(IsActive);
                            infoTrend.Trend.IsDirty = true;
                        }
                    }
                    finally
                    {
                        IsLoading = false;
                        isUpdating = false;
                       // SessionConfiguration.IsAnimate = false;
                    }
                });
            }
        }

        private void RefreshPlot(bool isActive)
        {
            if(isActive) 
            {
                // TODO: remove this temporary fix
                Task.Delay(1000).ContinueWith(t => InfoTrend.Trend.IsDirty = true);
            }
        }

        public void Reset()
        {
            dataReceiver.OnTriggerOccur -= DataReceiver_OnTriggerOccur;
            dataReceiver.WaveformAvailable -= OnNewDataReceived;
            resultsViewModel.ResultAdded -= ResultsViewModel_ResultAdded;

            InfoTrend.Trend.SelectedPlot = null;
            //InfoTrend.Trend.BusResultInfoView = null;
            InfoTrend.Trend.WfmShowingStartIndex = 0;
            InfoTrend.Trend.WfmShowingStopIndex = 0;
            InfoTrend.Trend.VCursorON = false;
            InfoTrend.Trend.MinimumIndex = 0;
            InfoTrend.Trend.MaximumIndex = 0;
            InfoTrend.Trend.MCursor1ON = false;
            InfoTrend.Trend.MCursor2ON = false;
            InfoTrend.Trend.MCursor3ON = false;
            InfoTrend.Trend.MCursor4ON = false;
            InfoTrend.Trend.PlotEvent = Prodigy.WaveformControls.View.PlotEvents.NONE;
            infoTrend.Trend.IsTVisible = false;
            infoTrend.Trend.IsMarkername = false;
            InfoTrend.Trend.HorizontalZoomLimit = 0;

            ObservableCollection<IPlotInfoView> tempPlotCollection = new ObservableCollection<IPlotInfoView>();
            foreach (var wfm in InfoTrend.Trend.PlotCollection.Select(x => x).ToList())
                tempPlotCollection.Add(wfm);
            for (int index = 0; index < tempPlotCollection.Count; index++)
            {
                if (tempPlotCollection.Any((plot) => plot.Channel == tempPlotCollection[index].Channel))
                {
                    IPlotInfoView removePlot = tempPlotCollection.First((plot) => plot.Channel == tempPlotCollection[index].Channel);
                    infoTrend.Trend.Remove(this, new Prodigy.WaveformControls.RemovePlotEventArgs(removePlot));
                }
            }

            tempPlotCollection.Clear();

            busUpdateRequired = true;
            Minimum = 0;
            Maximum = 0;
            ScrollValue = 0;
            ViewPortSize = double.NaN;
            isProcessing = false;
            IsLoading = false;
            StartTime = 0;
            StopTime = 0;
            TriggerPosition = 0;
            Fullwfmlabel = false;
            isTrigger = false;
        }

        private void CreatePlot(ChannelInfo channel)
        {
            try
            {
                IsLoading = true;

                DigitalPlotViewModel digitalPlot =
                           new DigitalPlotViewModel(GetChannel((int)channel.ChannelIndex),
                               0,
                               0,
                               channel.ChannelIndex,
                               1,
                               new DigitalPlotViewModel.GetPointsDataHandler(GetAllChannelDataPoints));

                digitalPlot.Tag = channel.ChannelName;
               // digitalPlot.TriggerPosition = 0;
                // Add waveform
                System.Windows.Application.Current.Dispatcher.Invoke(() => InfoTrend.Trend.AddWaveform(digitalPlot));

                IsLoading = false;
            }
            catch (Exception ex)
            {

            }
        }

        double TimeStampConverter(double actualTime)
        {
            return (actualTime - SessionConfiguration.TriggerTime);
        }
        public TrendInfoViewModel InfoTrend
        {
            get
            {
                return this.infoTrend;
            }
            set
            {
                this.infoTrend = value;
                RaisePropertyChanged("InfoTrend");
            }
        }

        Prodigy.Business.WfmEnum GetChannel(int index)
        {
            return (Prodigy.Business.WfmEnum)index;
        }

        private List<System.Windows.Point> GetAllChannelDataPoints(eChannles channel, int channelIndex, double startIndex, double stopIndex)
        {
            // Invalid channel index
            if (channel == eChannles.None)
                return new List<System.Windows.Point>();

            return dataReceiver.GetWaveformPoints(channel, startIndex, stopIndex);
        }

        private IEnumerable<IBusData> GetBusDiagram_Group(ConfigViewModel_Group channelGroup, double startTime, double stopTime)
        {
            double absStartTime = (startTime / 1e9) + SessionConfiguration.TriggerTime;
            double absEndTime = (stopTime / 1e9) + SessionConfiguration.TriggerTime;

            // Check if the Plot area changed or not
            // if not changed than return previous points collection
            var bufferlist = dataReceiver.GetWaveformBufferTimestamp(absStartTime, absEndTime);

            if (bufferlist.Any() == false)
                yield break;

            int validStart = bufferlist.FindIndex(b => b.TimeStamp >= absStartTime);
            int validEnd = bufferlist.FindLastIndex(b => b.TimeStamp <= absEndTime);

            if (validStart == -1 || validEnd == -1 || (validEnd - validStart) <= 0)
            {
                yield break;
                //throw new Exception("Invalid data - check if datareceiver has provided correct data.");
            }

            var validBufferList = bufferlist.Skip(validStart).Take(validEnd - validStart).ToList();

            DigitalWaveformEdgeFinding edgeFinding = new DigitalWaveformEdgeFinding(validBufferList);

            int index = configuration.ProtocolConfiguration.IndexOf(channelGroup);

            eChannles clock = configuration.Config.SelectedClock1;
            eEdgeType edgeType = configuration.SampleEdgeClock1;

            if (index > 2)
            {
                clock = configuration.Config.SelectedClock2;
                edgeType = configuration.SampleEdgeClock2;
            }

            // calculate min ui width
            double minUI = GetMinUIWidth(validBufferList, clock);

            Func<int, int> getNextEdgeIndex = (previousEdge) =>
            {
                if (edgeType == eEdgeType.RISING_EDGE)
                {
                    if (edgeFinding.GetNextRisingEdgeIndex(clock, previousEdge, out int nextEdgeIndex))
                    {
                        return nextEdgeIndex;
                    }
                }
                else
                {
                    if (edgeFinding.GetNextFallingEdgeIndex(clock, previousEdge, out int nextEdgeIndex))
                    {
                        return nextEdgeIndex;
                    }
                }

                return -1;
            };

            List<eChannles> channles = channelGroup.Channels.Select(c => c.ChannelIndex).ToList();
            int previousEdgeIndex = 0;
            int nextEdgeIndex = 0;
            while ((nextEdgeIndex = getNextEdgeIndex(previousEdgeIndex)) != -1)
            {
                // Create bus while edge is available
                PolygonBusData bus = new PolygonBusData();
                bus.StartIndex = (TimeStampConverter(bufferlist[validStart + nextEdgeIndex].TimeStamp) - minUI) * 1e9;
                bus.StopIndex = (TimeStampConverter(bufferlist[validStart + nextEdgeIndex].TimeStamp) + minUI) * 1e9;
                bus.Data = GetChannelData(bufferlist, validStart + nextEdgeIndex, channles);
                bus.Brush = GetChannelColor(index - 1);

                yield return bus;

                previousEdgeIndex = nextEdgeIndex;
            }
        }

        private double GetMinUIWidth(List<DiscreteWaveForm> bufferlist, eChannles clock)
        {
            DigitalWaveformEdgeFinding edgeFinding = new DigitalWaveformEdgeFinding(bufferlist);
            double minUIWidth = double.MaxValue;
            int previousEdgeIndex = 0;
            while (edgeFinding.GetNextEdge(clock, previousEdgeIndex, out _, out int nextEdgeIndex))
            {
                if (previousEdgeIndex > 0)
                {
                    double ui = bufferlist[nextEdgeIndex].TimeStamp - bufferlist[previousEdgeIndex].TimeStamp;

                    if (ui < minUIWidth)
                    {
                        minUIWidth = ui;
                    }                    
                }

                previousEdgeIndex = nextEdgeIndex;
            }

            return minUIWidth;
        }

        private string GetChannelData(List<DiscreteWaveForm> bufferlist, int index, List<eChannles> channles)
        {
            int retVal = 0;
            if (bufferlist.Count > index)
            {
                foreach (var channel in channles.OrderByDescending(x => x))
                {
                    retVal = (retVal << 1) | bufferlist[index].GetChannelState((int)channel);
                }
            }

            return $"0x{retVal.ToString("X2")}";
        }

        System.Windows.Media.Brush GetChannelColor(int index)
        {
            switch (index)
            {
                case 9:
                    return System.Windows.Media.Brushes.HotPink;
                case 10:
                    return System.Windows.Media.Brushes.DarkCyan;
                case 11:
                    return System.Windows.Media.Brushes.DarkGreen;
                case 12:
                    return System.Windows.Media.Brushes.DarkOrange;
                case 13:
                    return System.Windows.Media.Brushes.DarkViolet;
                case 14:
                    return System.Windows.Media.Brushes.DarkSalmon;
                case 15:
                    return System.Windows.Media.Brushes.BlueViolet;
                case 16:
                    return System.Windows.Media.Brushes.DeepSkyBlue;

                default: return System.Windows.Media.Brushes.DarkBlue;
            }
        }
    }
}
