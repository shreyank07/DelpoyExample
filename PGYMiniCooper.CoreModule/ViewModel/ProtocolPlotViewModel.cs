using PGYMiniCooper.CoreModule.ViewModel.ProtocolViewModel;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using Prodigy.WaveformControls;
using Prodigy.WaveformControls.Interfaces;
using ProdigyFramework.Behavior;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Prodigy.Business;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using System.Runtime.Remoting.Channels;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using PGYMiniCooper.DataModule.Interface;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace PGYMiniCooper.CoreModule.ViewModel
{
    public class ProtocolPlotViewModel : ViewModelBase, IResetViewModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private TrendInfoViewModel infoTrend;
        private Timer dispatcherTimer;

        IDataProvider dataReceiver;
        ConfigurationViewModel configuration;
        ProtocolActivityViewModel protocolActivity;
        ResultsViewModel resultsViewModel;

        public ProtocolPlotViewModel(ConfigurationViewModel configViewModel, ResultsViewModel resultsViewModel, ProtocolActivityViewModel protocolActivity)
        {
            this.configuration = configViewModel;
            this.resultsViewModel = resultsViewModel;
            this.protocolActivity = protocolActivity;
            
            InfoTrend = new TrendInfoViewModel();
            dataReceiver = Ioc.Default.GetService<IDataProvider>();
        }

        public void Initialize()
        {
            dataReceiver.OnTriggerOccur += DataReceiver_OnTriggerOccur;
            dataReceiver.WaveformAvailable += OnNewDataReceived;
            resultsViewModel.ResultAdded += ResultsViewModel_ResultAdded;
        }

        WfmEnum lastWfm = WfmEnum.CH17;
        private async void ResultsViewModel_ResultAdded(IResultViewModel result)
        {
            // Subscribe to all results selection changes

            // Add plot - this step is only required during initialize
            try
            {
                var busChannels = result.AvailableBusChannels;

                foreach (var channel in result.Config.Channels.Where(c => c.ChannelIndex != eChannles.None))
                {
                    DigitalPlotViewModel digitalPlot =
                               new DigitalPlotViewModel(GetChannel((int)channel.ChannelIndex),
                                   0,
                                   0,
                                   channel.ChannelIndex,
                                   1,
                                   new DigitalPlotViewModel.GetPointsDataHandler(GetAllChannelDataPoints));

                    digitalPlot.Tag = channel.ChannelName;
                    digitalPlot.TriggerPosition = 0;
                    // Add waveform
                    System.Windows.Application.Current.Dispatcher.Invoke(() => InfoTrend.Trend.AddWaveform(digitalPlot));

                    var busChannel = busChannels.FirstOrDefault(c => c.ChannelIndex == channel.ChannelIndex);

                    if (busChannel != null)
                    {
                        string busName = $"Bus_{busChannel.ChannelName}";
                        CreateBusPlot(busChannel, busName, result);
                    }
                }

                result.OnSelectionChanged += PlotAllBusDiagram;
            }
            catch (Exception ex) { }
        }

        private void CreateBusPlot(ChannelInfo busChannel, string busName, IResultViewModel result)
        {
            ProtocolBusPlotViewModel i3cbusPlot = new ProtocolBusPlotViewModel
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

            i3cbusPlot.Tag = busName;

            i3cbusPlot.Channel = lastWfm++;
            System.Windows.Application.Current.Dispatcher.Invoke(() => this.InfoTrend.Trend.AddWaveform(i3cbusPlot));
        }

        private void DataReceiver_OnTriggerOccur()
        {
            if (!(TriggerModel.GetInstance().TriggerType == eTriggerTypeList.Auto) && dataReceiver.TriggerAlreadyfound == true)
            {
                infoTrend.Trend.IsTVisible = true;
                if (dispatcherTimer == null)
                    dispatcherTimer = new Timer(updateMinMax, null, 3000, 3000);
            }
        }

        private bool isLoading = false;
        public bool IsLoading
        {
            get { return this.isLoading; }
            set
            {
                this.isLoading = value;
                this.RaisePropertyChanged("IsLoading");
            }
        }
        private void updateMinMax(object state)
        {
            RefreshPlot();
        }

        bool isProcessing = false;
        bool isRefreshplot = false;
        public void OnNewDataReceived(object sender, WaveformAvailableEventArgs e)
        {
            if (configuration.ConfigurationMode == eConfigMode.PA_Mode || configuration.ConfigurationMode == eConfigMode.Both)
            {
                InfoTrend.Trend.MinimumIndex = TimeStampConverter(e.New.StartTime) * 1e9;
                InfoTrend.Trend.MaximumIndex = TimeStampConverter(e.New.EndTime) * 1e9;

                if (e.Old == null || e.New.MaxEdges < 1000 || e.Old.MaxEdges < 1000)
                {
                    InfoTrend.Trend.HorizontalZoomLimit = InfoTrend.Trend.MaximumIndex - InfoTrend.Trend.MinimumIndex;

                    // Wait for at least 1000 sample points
                    if (e.New.MaxEdges > 1e6)
                    {
                        var timeBetweenEdges = (InfoTrend.Trend.MaximumIndex - InfoTrend.Trend.MinimumIndex) / e.New.MaxEdges;
                        InfoTrend.Trend.HorizontalZoomLimit = 1e6 * timeBetweenEdges;
                    }

                    this.infoTrend.Trend.WfmShowingStartIndex = InfoTrend.Trend.MinimumIndex;
                    this.infoTrend.Trend.WfmShowingStopIndex = InfoTrend.Trend.MinimumIndex + InfoTrend.Trend.HorizontalZoomLimit;

                    IsLoading = true;
                    Task.Run(() =>
                    {
                        try
                        {
                            InfoTrend.Trend.IsDirty = true;
                        }
                        finally
                        {
                            IsLoading = false;
                        }
                    });
                }

                SessionConfiguration.IsAnimate = true;
            }
        }
        public Command GotoTriggerPosition
        {
            get
            {
                return new Command(new Command.ICommandOnExecute(GoToTrigger));
            }
        }

        public void GoToTrigger()
        {

            if (SessionConfiguration.TriggerProtcolset && configuration.Trigger.TriggerType == eTriggerTypeList.Protocol)
            {
              
                if(resultsViewModel.WareHouse != null && resultsViewModel.WareHouse.TriggerPacket!=null)
                {
                    resultsViewModel.WareHouse.gotoTriggerMethod();
                }
                else
                {
                    foreach (var result in resultsViewModel.ProtocolResults)

                    {

                        if (result.Config.ProtocolType == eProtocol.I3C)
                        {
                            var i3cResult = (ResultViewModel_I3C)result;
                            if (i3cResult.WareHouse.TriggerPacket != -1)
                                i3cResult.gotoTriggerMethod();
                        }
                        else
                              if (result.Config.ProtocolType == eProtocol.I2C)
                        {
                            var i2cResult = (ResultViewModel_I2C)result;
                            if (i2cResult.WareHouse.TriggerPacket != -1)
                                i2cResult.gotoTriggerMethod();
                        }
                        else
                             if (result.Config.ProtocolType == eProtocol.SPI)
                        {
                            var spiResult = (ResultViewModel_SPI)result;
                            if (spiResult.WareHouse.TriggerPacket != -1)
                                spiResult.gotoTriggerMethod();
                        }
                        else
                             if (result.Config.ProtocolType == eProtocol.UART)
                        {
                            var uartResult = (ResultViewModel_UART)result;
                            if (uartResult.WareHouse.TriggerPacket != -1)
                                uartResult.gotoTriggerMethod();
                        }
                      else   if (result.Config.ProtocolType == eProtocol.CAN)
                        {
                            var canResult = (ResultViewModel_CAN)result;
                            if (canResult.WareHouse.TriggerPacket != -1)
                                canResult.gotoTriggerMethod();
                        }
                        else if (result.Config.ProtocolType == eProtocol.QSPI)
                        {
                            var qspiResult = (ResultViewModel_QSPI)result;
                            if (qspiResult.WareHouse.TriggerPacket != -1)
                                qspiResult.gotoTriggerMethod();
                        }


                    }
                }
                
            }
                //if (SessionConfiguration.TriggerProtcolset && protocolActivity.ProtocolActivity.ProtocolCollection.Count > 0 && TriggerModel.GetInstance().TriggerType == eTriggerTypeList.Protocol)
                //{
                //    protocolActivity.SelectedFrame = null;

                //    var resultWithTriggerPacket = resultsViewModel.ProtocolResults.FirstOrDefault(r => r.TriggerPacketIndex != -1);

                //    if (resultWithTriggerPacket != null)
                //    {
                //        var triggerPacket = protocolActivity.ProtocolActivity.ProtocolCollection.FirstOrDefault(p => p.Protocol == resultWithTriggerPacket.Config.ProtocolType && p.Sample == resultWithTriggerPacket.TriggerPacketIndex);

                //        if (triggerPacket != null)
                //            protocolActivity.SelectedFrame = triggerPacket;
                //    }
                //}
            }

        bool RefreshPlot()
        {
            if (SessionConfiguration.TriggerProtcolset && !isProcessing && protocolActivity.ProtocolActivity.ProtocolCollection.Count > 0 && protocolActivity.SelectedFrame != null && !isRefreshplot)
            {
                isRefreshplot = true;
                dispatcherTimer?.Dispose();
                isProcessing = true;

                var result = resultsViewModel.ProtocolResults.FirstOrDefault(r => r.SelectedFrame != null);
                if (result != null)
                    PlotAllBusDiagram(result);

                isProcessing = false;
                return true;
            }
            return false;
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

        public object Points { get; private set; }
        public object PlotModel { get; private set; }

        public void Reset()
        {
            if (dispatcherTimer != null)
            {
                dispatcherTimer.Dispose();
                dispatcherTimer = null;
            }

            dataReceiver.OnTriggerOccur -= DataReceiver_OnTriggerOccur;
            dataReceiver.WaveformAvailable -= OnNewDataReceived;
            // Unsubscribe to all results selection changes
            resultsViewModel.ResultAdded -= ResultsViewModel_ResultAdded;

            infoTrend.Trend.PlotEvent = Prodigy.WaveformControls.View.PlotEvents.NONE;
            infoTrend.Trend.SelectedPlot = null;
            infoTrend.Trend.BusResultInfoView = null;
            infoTrend.Trend.WfmShowingStartIndex = 0;
            infoTrend.Trend.WfmShowingStopIndex = 0;
            infoTrend.Trend.VCursorON = false;
            infoTrend.Trend.MinimumIndex = 0;
            infoTrend.Trend.MaximumIndex = 0;
            infoTrend.Trend.MCursor1ON = false;
            infoTrend.Trend.MCursor2ON = false;
            infoTrend.Trend.MCursor3ON = false;
            infoTrend.Trend.MCursor4ON = false;
            infoTrend.Trend.IsTVisible = false;
            infoTrend.Trend.IsMarkername = false;
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
            isProcessing = false;
            IsLoading = false;
            lastWfm = WfmEnum.CH17;
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

        void focusSelectedPacket(IResultViewModel resultViewModel)
        {
            IsLoading = true;

            Task.Run(() =>
            {
                try
                {
                    // Display the frame in 70% area
                    double wfmStartIndex = TimeStampConverter(resultViewModel.SelectedFrame.StartTime) * 1e9;
                    double wfmStopIndex = TimeStampConverter(resultViewModel.SelectedFrame.StopTime) * 1e9;

                    double start = (wfmStartIndex - 0.25 * (wfmStopIndex - wfmStartIndex));
                    double stop = (wfmStopIndex + 0.25 * (wfmStopIndex - wfmStartIndex));

                    if (infoTrend.Trend.HorizontalZoomLimit < (stop - start))
                        infoTrend.Trend.HorizontalZoomLimit = stop - start;

                    this.infoTrend.Trend.WfmShowingStartIndex = start;
                    this.infoTrend.Trend.WfmShowingStopIndex = stop;

                    this.InfoTrend.Trend.IsDirty = true;

                }
                catch(Exception ex)
                {
                    log.Error(ex);
                }
                finally
                {
                    IsLoading = false;
                }
            });
        }

        public async void PlotAllBusDiagram(IResultViewModel resultViewModel)
        {
            try
            {
                if (!isProcessing)
                {
                    isProcessing = true;
                    //IsLoading = true;

                    if (resultViewModel.SelectedFrame == null)
                        return;

                    focusSelectedPacket(resultViewModel);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                isProcessing = false;
                //IsLoading = false;
            }
        }
    }
}
