using CommunityToolkit.Mvvm.DependencyInjection;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Helpers;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using PGYMiniCooper.DataModule.Structure.QSPIStructure;
using PGYMiniCooper.DataModule.Structure.RFFEStructure;
using PGYMiniCooper.DataModule.Structure.SPMIStructure;
using Prodigy.WaveformControls.Interfaces;
using ProdigyFramework.Behavior;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace PGYMiniCooper.CoreModule.ViewModel.ProtocolViewModel
{
    public class ResultViewModel_RFFE : ViewModelBase, IResultViewModel
    {

        public ResultViewModel_RFFE(ConfigViewModel_RFFE config, ResultModel_RFFE result, TriggerViewModel trigger)
        {
            //this.Config = config;
            //wareHouse = result;
            //// TODO: add logic to subscribe update from all protocols
            ////ResultAdder.Instance.OnSelectionChanged += GetCurrentFrame;

            this.Config = config;
            this.WareHouse = result;
            this.trigger = trigger;
            wareHouse.Reset();
            rFFEFrameCollection = new ProdigyFramework.Collections.AsyncObservableCollection<RFFEFrameStructure>();
            wareHouse.OnFramesDecoded += WareHouse_OnFramesDecoded;
            SearchParameter = new SearchFilterViewModel_RFFE((IList)rFFEFrameCollection);
            SearchParameter.OnSelectionChanged += SearchParameter_SelectionChanged;
        }

        private void WareHouse_OnFramesDecoded(object sender, IReadOnlyList<IFrame> frames)
        {
            var spmiFrames = frames.OfType<RFFEFrameStructure>();

            ((ProdigyFramework.Collections.AsyncObservableCollection<RFFEFrameStructure>)rFFEFrameCollection).AddRange(spmiFrames);

            foreach (var frame in spmiFrames)
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



        private readonly TriggerViewModel trigger;

        private ConfigViewModel_RFFE config;
        public ConfigViewModel_RFFE Config 
        {
            get { return config; }
            set
            {
                config = value;
                RaisePropertyChanged(nameof(Config));
            } 
        }

        ResultModel_RFFE wareHouse;
        public ResultModel_RFFE WareHouse
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
        public RFFEFrameStructure SelectedFrame
        {
            get
            {
                return wareHouse.SelectedFrame;
            }
            set
            {
                wareHouse.SelectedFrame = value;

                if (wareHouse.SelectedFrame != null)
                {
                    //SearchParameter.SelectedFrame = value;
                    OnSelectionChanged?.Invoke(this);
                }
                RaisePropertyChanged("SelectedFrame");
            }
        }

        public IList<RFFEFrameStructure> RFFEFrameCollection
        {
            get
            {
                return rFFEFrameCollection;
            }
            set
            {
                rFFEFrameCollection = value;
                RaisePropertyChanged(nameof(RFFEFrameCollection));
            }
        }
        private IList<RFFEFrameStructure> rFFEFrameCollection;

        public ICommand frameselectionCommand;
        public ICommand FrameSelectionCommand
        {
            get
            {
                return frameselectionCommand ?? (frameselectionCommand = new RelayCommand((o) => OnSelectionChanged?.Invoke(this)));
            }
        }
        public void Reset()
        {
         
            WareHouse.Reset();
            wareHouse.SelectedFrame = null;
        }


        #region Trigger

      
        //Goto Trigger position
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

        public void gotoTriggerMethod()
        {
            SelectedFrame = RFFEFrameCollection[WareHouse.TriggerPacket];
        }

        #endregion

        public ICommand selectionCommand;
        public ICommand SelectionCommand
        {
            get
            {
                return selectionCommand ?? (selectionCommand = new RelayCommand((o) => OnSelectionChanged?.Invoke(this)));
            }
        }
        private void SearchParameter_SelectionChanged(IFrame frame)
        {
            this.SelectedFrame = (RFFEFrameStructure)frame;
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

        void GetCurrentFrame(ProtocolActivityHolder frame)
        {
            if (frame.Protocol == DataModule.eProtocol.RFFE)
                SelectedIndex = frame.Sample;
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

        public void Serach()
        {
            throw new NotImplementedException();
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
        public SearchFilterViewModel_RFFE SearchParameter { get; }

        #region IResultViewModel implementation

        public event Action<IResultViewModel> OnSelectionChanged;

        IConfigViewModel IResultViewModel.Config => Config;

        int IResultViewModel.TriggerPacketIndex => wareHouse.TriggerPacket;

        IFrame IResultViewModel.SelectedFrame
        {
            get { return SelectedFrame; }
            set
            {
                SelectedFrame = (RFFEFrameStructure)value;
            }
        }

        ICollection<IFrame> IResultViewModel.ResultCollection => wareHouse.FrameCollection.OfType<IFrame>().ToList();

        List<ChannelInfo> IResultViewModel.AvailableBusChannels => new List<ChannelInfo> { new ChannelInfo("RFFE", ((IResultViewModel)this).Config.Channels.FirstOrDefault(c => c.ChannelIndex == config.ChannelIndex_SDA).ChannelIndex) };

        IEnumerable<IBusData> IResultViewModel.GetBusDiagram(ChannelInfo channel, double startTime, double stopTime)
        {
            var bufferlist = GetWavepoints(startTime, stopTime).ToList();
            bufferlist = bufferlist.Select(p =>
            new DiscreteWaveForm
            {
                ChannelValue = p.ChannelValue,
                TimeStamp = TimeStampConverter(p.TimeStamp),
            }).ToList();
            var edgeFinding = new DigitalWaveformEdgeFinding(bufferlist);

            eChannles clockChannel = Config.ChannelIndex_SCL;
            eChannles dataChannel = Config.ChannelIndex_SDA;
            var annotationStopIndex = 0;
            string emptyVal = "  .";
            var resultList = RFFEFrameCollection.SkipWhile(r => r.StopTime < startTime).TakeWhile(r => r.StartTime <= stopTime).ToList();
            foreach (var currentPacket in resultList)
            {
                double StartTimestamp = TimeStampConverter(currentPacket.StartTime);
                double StopTimestamp = TimeStampConverter(currentPacket.StopTime);





                #region SSC
                //SSC
                annotationStopIndex = bufferlist.FindIndex(obj => obj.TimeStamp == StartTimestamp) + 2;
                edgeFinding.GetPreviousEdge(dataChannel, annotationStopIndex, out eEdgeType edgeType, out int annotationStartIndex);

                StartTimestamp = bufferlist[annotationStartIndex].TimeStamp;
               // if (annotationStopIndex != -1)
                {
                    var bus = new PolygonBusData();
                    StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                    bus.StartIndex = StartTimestamp * 1e9;
                    bus.StopIndex = StopTimestamp * 1e9;
                    bus.Data = "SSC" + emptyVal;
                    bus.Brush = System.Windows.Media.Brushes.ForestGreen;
                   yield return bus;
                }
                #endregion

                #region SlaveID
           


                //SlaveID
                annotationStartIndex = annotationStopIndex;
                StartTimestamp = StopTimestamp;
                if (edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 4, eEdgeType.RISING_EDGE, out annotationStopIndex))
                {
                    var bus = new PolygonBusData();
                    StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                    bus.StartIndex = StartTimestamp * 1e9;
                    bus.StopIndex = StopTimestamp * 1e9;
               
                        if (currentPacket.Command.CmdType == eRFFECMDTYPE.MASTER_CXT_TRANSFER_WRITE || currentPacket.Command.CmdType == eRFFECMDTYPE.MASTER_CXT_TRANSFER_READ || currentPacket.Command.CmdType == eRFFECMDTYPE.MASTER_WRITE || currentPacket.Command.CmdType == eRFFECMDTYPE.MASTER_READ || currentPacket.Command.CmdType == eRFFECMDTYPE.MASTER_OWNERSHIP_HANDOVER)
                            bus.Data = "MID=" + "0x" + currentPacket.SlaveId.ToString("X2") + emptyVal;
                    else
                        bus.Data = "Slave=" + "0x" + currentPacket.SlaveId.ToString("X2") + emptyVal;
                    bus.Brush = System.Windows.Media.Brushes.DarkCyan;

                    yield return bus;
                }
                #endregion

          

                #region Command
                annotationStartIndex = annotationStopIndex;
                StartTimestamp = StopTimestamp;
                if (edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 8, eEdgeType.RISING_EDGE, out annotationStopIndex))
                {
                    var bus = new PolygonBusData();
                    StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                    bus.StartIndex = StartTimestamp * 1e9;
                    bus.StopIndex = StopTimestamp * 1e9;
                    bus.Data = currentPacket.Command.CmdType.ToString() + " =" + "0x" + currentPacket.Command.Value.ToString("X2") + emptyVal;
                    bus.Brush = System.Windows.Media.Brushes.DarkGreen;

                    yield return bus;
                }
                #endregion

            

                #region Parity
                annotationStartIndex = annotationStopIndex;
                StartTimestamp = StopTimestamp;
                if (edgeFinding.GetNextRisingEdgeIndex(clockChannel, annotationStartIndex, out annotationStopIndex))
                {
                    var bus = new PolygonBusData();
                    StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                    bus.StartIndex = StartTimestamp * 1e9;
                    bus.StopIndex = StopTimestamp * 1e9;
                    bus.Data = "P" + emptyVal;
                    bus.Brush = System.Windows.Media.Brushes.DeepSkyBlue;

                    yield return bus;
                }
                #endregion
                eRFFECMDTYPE cmd = currentPacket.Command.CmdType;
                #region BPC
                if (currentPacket.Command.HasBP)
                {
                    annotationStartIndex = annotationStopIndex;
                    edgeFinding.GetNextRisingEdgeIndex(clockChannel, annotationStartIndex, out annotationStopIndex);
                  //  edgeFinding.GetPreviousEdge(dataChannel, annotationStopIndex, out eEdgeType edgeType, out int annotationStartIndex);
                    if (cmd == eRFFECMDTYPE.REG_ZERO_WRITE && annotationStartIndex != -1)
                    {

                        edgeFinding.GetPreviousEdge(clockChannel, annotationStartIndex, out edgeType, out int dummystop);
                        if (annotationStopIndex == -1)
                        {
                            annotationStopIndex = dummystop;
                        }
                        StartTimestamp = StopTimestamp;
                        StopTimestamp = (2 * (bufferlist[annotationStartIndex].TimeStamp - bufferlist[dummystop].TimeStamp)) + StartTimestamp;
                    }
                    else
                    {
                        StartTimestamp = StopTimestamp;
                        if (annotationStopIndex != -1)
                            StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                    }
                    //if (annotationStopIndex != -1)
                    {
                        var bus = new PolygonBusData();
                        //StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                        bus.StartIndex = StartTimestamp * 1e9;
                        bus.StopIndex = StopTimestamp * 1e9;
                        bus.Data = "BPC" + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.DarkViolet;
                   yield return bus;
                    }
                }
                #endregion

                #region Interrupts

                if (cmd == eRFFECMDTYPE.INT_SUMMARY_IDENT)
                {
                    annotationStartIndex = annotationStopIndex;
                  edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 1, eEdgeType.RISING_EDGE, out annotationStopIndex);
                    StartTimestamp = StopTimestamp;
                    var bus = new PolygonBusData();
                    if (annotationStopIndex != -1)
                    {
                        bus = new PolygonBusData();
                        StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                        bus.StartIndex = StartTimestamp * 1e9;
                        bus.StopIndex = StopTimestamp * 1e9;
                        bus.Data = "ISI" + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.Violet;
                       yield  return bus;
                    }

                    //BRC
                    annotationStartIndex = annotationStopIndex;
                    edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 1, eEdgeType.RISING_EDGE, out annotationStopIndex);
                    StartTimestamp = StopTimestamp;
                    if (annotationStopIndex != -1)
                    {
                        bus = new PolygonBusData();
                        StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                        bus.StartIndex = StartTimestamp * 1e9;
                        bus.StopIndex = StopTimestamp * 1e9;
                        bus.Data = "BRC" + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.HotPink;
                       yield return bus;
                    }

                    //BPC
                    annotationStartIndex = annotationStopIndex;
                    edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 8, eEdgeType.RISING_EDGE, out annotationStopIndex);

                    //int dummystop = GetPreviousEdge(annotationStartIndex, rFFE_clockIndex, bufferlist);
                    edgeFinding.GetPreviousEdge(clockChannel, annotationStartIndex, out edgeType, out int dummystop);
                    if (annotationStopIndex == -1)
                    {
                        annotationStopIndex = annotationStartIndex;
                        StartTimestamp = StopTimestamp;
                        StopTimestamp = (2 * (bufferlist[annotationStartIndex].TimeStamp - bufferlist[dummystop].TimeStamp)) + StartTimestamp;
                        bus = new PolygonBusData();
                        bus.StartIndex = StartTimestamp * 1e9;
                        bus.StopIndex = StopTimestamp * 1e9;
                        bus.Data = "BPC" + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.HotPink;
                     yield return bus;
                    }
                    else
                    {
                        StartTimestamp = StopTimestamp;
                        StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                        bus = new PolygonBusData();
                        bus.StartIndex = StartTimestamp * 1e9;
                        bus.StopIndex = StopTimestamp * 1e9;
                        bus.Data = "BPC" + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.HotPink;
                       yield return bus;
                    }

                    int ClockCycles = currentPacket.InterruptCount * 3;
                    int counter = 16;

                    for (int index = 0; index < ClockCycles; index += 3)
                    {
                        if (ClockCycles < 0)
                            break;

                        counter--;
                        annotationStartIndex = annotationStopIndex;
                        //annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferlist, 1, ref lastEdge);
                        edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 1, eEdgeType.RISING_EDGE, out annotationStopIndex);
                        StartTimestamp = StopTimestamp;
                        if (annotationStopIndex != -1)
                        {
                            bus = new PolygonBusData();
                            StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                            bus.StartIndex = StartTimestamp * 1e9;
                            bus.StopIndex = StopTimestamp * 1e9;
                            bus.Data = "INT" + emptyVal;
                            bus.Brush = System.Windows.Media.Brushes.MediumSlateBlue;
                           yield return bus;
                        }

                        annotationStartIndex = annotationStopIndex;
                        //annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferlist, 1, ref lastEdge);
                        edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 1, eEdgeType.RISING_EDGE, out annotationStopIndex);
                        StartTimestamp = StopTimestamp;
                        if (annotationStopIndex != -1)
                        {
                            bus = new PolygonBusData();
                            StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                            bus.StartIndex = StartTimestamp * 1e9;
                            bus.StopIndex = StopTimestamp * 1e9;
                            bus.Data = "ISC" + emptyVal;
                            bus.Brush = System.Windows.Media.Brushes.MediumVioletRed;
                            yield return bus;
                        }

                        annotationStartIndex = annotationStopIndex;
                        edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 1, eEdgeType.RISING_EDGE, out annotationStopIndex);
                        //annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferlist, 1, ref lastEdge);

                        //int dummy = GetPreviousEdge(annotationStartIndex, rFFE_clockIndex, bufferlist);
                        edgeFinding.GetPreviousEdge(clockChannel, annotationStartIndex, out edgeType, out int dummy);
                        if (annotationStopIndex == -1)
                        {
                            annotationStopIndex = annotationStartIndex;
                            StartTimestamp = StopTimestamp;
                            StopTimestamp = (2 * (bufferlist[annotationStartIndex].TimeStamp - bufferlist[dummy].TimeStamp)) + StartTimestamp;
                        }
                        else
                        {
                            if ((ClockCycles - index) == 1)
                            {
                                StartTimestamp = StopTimestamp;
                                StopTimestamp = (2 * (bufferlist[annotationStartIndex].TimeStamp - bufferlist[dummy].TimeStamp)) + StartTimestamp;
                            }
                            else
                            {
                                StartTimestamp = StopTimestamp;
                                StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                            }
                        }
                        bus = new PolygonBusData();
                        bus.StartIndex = StartTimestamp * 1e9;
                        bus.StopIndex = StopTimestamp * 1e9;
                        bus.Data = "BPC" + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.Violet;
                        yield return bus;

                    }
                }
                #endregion

                #region MOH
                if (cmd == eRFFECMDTYPE.MASTER_OWNERSHIP_HANDOVER)
                {
                    var bus = new PolygonBusData();
                    #region Ack
                    annotationStartIndex = annotationStopIndex;
                    //annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferlist, 1, ref lastEdge);
                    edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 1, eEdgeType.RISING_EDGE, out annotationStopIndex);
                    StartTimestamp = StopTimestamp;
                    if (annotationStopIndex != -1)
                    {
                        bus = new PolygonBusData();
                        StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                        bus.StartIndex = StartTimestamp * 1e9;
                        bus.StopIndex = StopTimestamp * 1e9;
                        bus.Data = "ACK" + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.Green;
                       yield return bus;
                    }

                    #endregion

                    //Next 2 Cycles 0
                    #region MID
                    

                    edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStopIndex, 2, eEdgeType.RISING_EDGE, out annotationStartIndex);
                    edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 2, eEdgeType.RISING_EDGE, out annotationStopIndex);
                    if (annotationStopIndex != -1 && annotationStartIndex != -1)
                    {
                        bus = new PolygonBusData();
                        StartTimestamp = bufferlist[annotationStartIndex].TimeStamp;
                        StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                        bus.StartIndex = StartTimestamp * 1e9;
                        bus.StopIndex = StopTimestamp * 1e9;
                        bus.Data = "MID" + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.HotPink;
                        yield return bus;
                    }


                    #endregion

                    #region Parity
              
                    edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStopIndex, 3, eEdgeType.RISING_EDGE, out annotationStartIndex);
                    edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 1, eEdgeType.RISING_EDGE, out annotationStopIndex);
                    if (annotationStopIndex != -1 && annotationStartIndex != -1)
                    {
                        bus = new PolygonBusData();
                        StartTimestamp = bufferlist[annotationStartIndex].TimeStamp;
                        StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                        bus.StartIndex = StartTimestamp * 1e9;
                        bus.StopIndex = StopTimestamp * 1e9;
                        bus.Data = "P" + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.DeepSkyBlue;
                       yield return bus;
                    }

                    #endregion

                    #region BHC
                    annotationStartIndex = annotationStopIndex;
                   
                    edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 1, eEdgeType.RISING_EDGE, out annotationStopIndex);
                   // var dummystop = GetPreviousEdge(annotationStartIndex, rFFE_clockIndex, bufferlist);
                    edgeFinding.GetPreviousEdge(clockChannel, annotationStartIndex, out edgeType, out int dummystop);
                    if (annotationStopIndex == -1)
                    {
                        annotationStopIndex = annotationStartIndex;
                    }
                    StartTimestamp = StopTimestamp;
                    StopTimestamp = (2 * (bufferlist[annotationStartIndex].TimeStamp - bufferlist[dummystop].TimeStamp)) + StartTimestamp;


                    bus = new PolygonBusData();
                    bus.StartIndex = StartTimestamp * 1e9;
                    bus.StopIndex = StopTimestamp * 1e9;
                    bus.Data = "BHC" + emptyVal;
                    bus.Brush = System.Windows.Media.Brushes.DeepSkyBlue;
                   yield return bus;

                    #endregion
                }

                #endregion

                if (currentPacket.BytecountList != null && currentPacket.BytecountList.Count() > 0)
                {
                    //var data = PlotRFFEAnnotation(currentPacket.BytecountList, "Byte Count", ref annotationStartIndex, ref annotationStopIndex, ref StartTimestamp, ref StopTimestamp, bufferlist, cmd);
                    var data = PlotRFFEAnnotation(bufferlist, edgeFinding, currentPacket.BytecountList, "Byte Count", ref annotationStartIndex, ref annotationStopIndex, ref StartTimestamp, ref StopTimestamp, bufferlist, cmd);
                    foreach (var item in data)
                        yield return item;
                }

                if (currentPacket.Address != null && currentPacket.Address.Count() > 0)
                {
                    //var data = PlotRFFEAnnotation(currentPacket.Address, "Address", ref annotationStartIndex, ref annotationStopIndex, ref StartTimestamp, ref StopTimestamp, bufferlist, cmd);
                    var data = PlotRFFEAnnotation(bufferlist, edgeFinding, currentPacket.Address, "Address", ref annotationStartIndex, ref annotationStopIndex, ref StartTimestamp, ref StopTimestamp, bufferlist, cmd);
                    foreach (var item in data)
                        yield return item;
                }

                if (currentPacket.Data != null && currentPacket.Data.Count() > 0)
                {
                    if (cmd != eRFFECMDTYPE.REG_ZERO_WRITE)
                    {
                        //var data = PlotRFFEAnnotation(currentPacket.Data, "Data", ref annotationStartIndex, ref annotationStopIndex, ref StartTimestamp, ref StopTimestamp, bufferlist, cmd);
                        var data = PlotRFFEAnnotation(bufferlist, edgeFinding, currentPacket.Data, "Data", ref annotationStartIndex, ref annotationStopIndex, ref StartTimestamp, ref StopTimestamp, bufferlist, cmd);
                        foreach (var item in data)
                            yield return item;
                    }
                }

            }
        }

        List<IBusData> PlotRFFEAnnotation(List<DiscreteWaveForm> bufferlist, DigitalWaveformEdgeFinding edgeFinding, RFFEPacketStructure[] currentList, string annoType, ref int annotationStartIndex, ref int annotationStopIndex, ref double StartTimestamp, ref double StopTimestamp, List<DiscreteWaveForm> bufferList, eRFFECMDTYPE cmdtype)
        {
            List<IBusData> busData = new List<IBusData>();

            int loopCount = 0;
            eChannles clockChannel = Config.ChannelIndex_SCL;
            string emptyVal = " .";
            bool datamask = false;
            foreach (var currentpacket in currentList)
            {
                #region Data/Address
                annotationStartIndex = annotationStopIndex;
                StartTimestamp = StopTimestamp;
                if (edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 8, eEdgeType.RISING_EDGE, out annotationStopIndex))
                {
                    var bus = new PolygonBusData();
                    StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                    bus.StartIndex = StartTimestamp * 1e9;
                    bus.StopIndex = StopTimestamp * 1e9;

                    if (annoType == "Data" && cmdtype == eRFFECMDTYPE.MASKED_WRITE && !datamask)
                    {
                        datamask = true;
                        bus.Data = "DataMask " + "=0x" + currentpacket.Value.ToString("X2") + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.RoyalBlue;
                    }
                    else
                        bus.Data = annoType + "=0x" + currentpacket.Value.ToString("X2") + emptyVal;

                    if (annoType == "Data")
                        bus.Brush = System.Windows.Media.Brushes.RoyalBlue;
                    else if (annoType == "Address")
                        bus.Brush = System.Windows.Media.Brushes.DarkCyan;
                    else
                        bus.Brush = System.Windows.Media.Brushes.SteelBlue;

                    busData.Add(bus);
                }
                #endregion

                #region Parity
                annotationStartIndex = annotationStopIndex;
                StartTimestamp = StopTimestamp;

                if (edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 1, eEdgeType.RISING_EDGE, out annotationStopIndex))
                {
                    var bus = new PolygonBusData();
                    StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                    bus.StartIndex = StartTimestamp * 1e9;
                    bus.StopIndex = StopTimestamp * 1e9;
                    bus.Data = "P" + emptyVal;
                    bus.Brush = System.Windows.Media.Brushes.DeepSkyBlue;
                    busData.Add(bus);
                }
                #endregion

                #region BP
                if (currentpacket.HasBP)
                {

                    annotationStartIndex = annotationStopIndex;
                    edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 1, eEdgeType.RISING_EDGE, out annotationStopIndex);

                    edgeFinding.GetPreviousEdge(clockChannel, annotationStartIndex, out eEdgeType edgeType, out int dummystop);
                    if (annotationStopIndex == -1)
                    {
                        annotationStopIndex = annotationStartIndex;
                    }
                    StartTimestamp = StopTimestamp;

                    if (dummystop != -1)
                    {
                        var bus = new PolygonBusData();
                        StopTimestamp = (2 * (bufferList[annotationStartIndex].TimeStamp - bufferList[dummystop].TimeStamp)) + StartTimestamp;
                        bus.StartIndex = StartTimestamp * 1e9;
                        bus.StopIndex = StopTimestamp * 1e9;
                        bus.Data = "BPC" + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.DarkViolet;
                        busData.Add(bus);
                    }
                }
                #endregion

            }

            return busData;
        }
        //ObservableCollection<PolygonBusData> PlotRFFEAnnotation(RFFEPacketStructure[] currentList, string annoType, ref int annotationStartIndex, ref int annotationStopIndex, ref double StartTimestamp, ref double StopTimestamp, List<DiscreteWaveForm> bufferList, eRFFECMDTYPE cmdtype)
        //{
        //    int rFFE_clockIndex = (int)ConfigModel.GetInstance().SelectedRFFE_SCL;
        //    int rFFE_dataIndex = (int)ConfigModel.GetInstance().SelectedRFFE_SDA;
        //    int lastEdge = 0;
        //    string emptyVal = " .";
        //    bool datamask = false;
        //    ObservableCollection<PolygonBusData> BusData = new ObservableCollection<PolygonBusData>();
        //    foreach (var currentpacket in currentList)
        //    {
        //        #region Data/Address
        //        annotationStartIndex = annotationStopIndex;
        //        annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferList, 8, ref lastEdge);
        //        StartTimestamp = StopTimestamp;
        //        if (annotationStopIndex != -1)
        //        {
        //            var bus = new PolygonBusData();
        //            StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //            bus.StartIndex = StartTimestamp * 1e9;
        //            bus.StopIndex = StopTimestamp * 1e9;

        //            if (annoType == "Data" && cmdtype == eRFFECMDTYPE.MASKED_WRITE && !datamask)
        //            {
        //                datamask = true;
        //                bus.Data = "DataMask " + "=0x" + currentpacket.Value.ToString("X2") + emptyVal;
        //                bus.Brush = System.Windows.Media.Brushes.RoyalBlue;
        //            }
        //            else
        //                bus.Data = annoType + "=0x" + currentpacket.Value.ToString("X2") + emptyVal;

        //            if (annoType == "Data")
        //                bus.Brush = System.Windows.Media.Brushes.RoyalBlue;
        //            else if (annoType == "Address")
        //                bus.Brush = System.Windows.Media.Brushes.DarkCyan;
        //            else
        //                bus.Brush = System.Windows.Media.Brushes.SteelBlue;

        //            BusData.Add(bus);
        //        }
        //        #endregion

        //        #region Parity
        //        annotationStartIndex = annotationStopIndex;
        //        annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferList, 1, ref lastEdge);
        //        StartTimestamp = StopTimestamp;

        //        if (annotationStopIndex != -1)
        //        {
        //            var bus = new PolygonBusData();
        //            StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //            bus.StartIndex = StartTimestamp * 1e9;
        //            bus.StopIndex = StopTimestamp * 1e9;
        //            bus.Data = "P" + emptyVal;
        //            bus.Brush = System.Windows.Media.Brushes.DeepSkyBlue;
        //            BusData.Add(bus);
        //        }
        //        #endregion

        //        #region BP
        //        if (currentpacket.HasBP)
        //        {

        //            annotationStartIndex = annotationStopIndex;
        //            annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferList, 1, ref lastEdge);

        //            int dummystop = GetPreviousEdge(annotationStartIndex, rFFE_clockIndex, bufferlist);
        //            if (annotationStopIndex == -1)
        //            {
        //                annotationStopIndex = annotationStartIndex;
        //            }
        //            StartTimestamp = StopTimestamp;

        //            if (dummystop != -1)
        //            {
        //                var bus = new PolygonBusData();
        //                StopTimestamp = (2 * (bufferList[annotationStartIndex].TimeStamp - bufferList[dummystop].TimeStamp)) + StartTimestamp;
        //                bus.StartIndex = StartTimestamp * 1e9;
        //                bus.StopIndex = StopTimestamp * 1e9;
        //                bus.Data = "BPC" + emptyVal;
        //                bus.Brush = System.Windows.Media.Brushes.DarkViolet;
        //                BusData.Add(bus);
        //            }
        //        }
        //        #endregion

        //    }

        //    return BusData;
        //}

        //void DrawRFFEBus(Prodigy.Business.WfmEnum channel, int rffe_startFrameIndex, int rffe_stopFrameIndex)
        //{
        //    int rFFE_clockIndex = (int)ConfigModel.GetInstance().SelectedRFFE_SCL;
        //    int rFFE_dataIndex = (int)ConfigModel.GetInstance().SelectedRFFE_SDA;
        //    prevIndexHolder[8] = rffe_startFrameIndex;
        //    prevIndexHolder[9] = rffe_stopFrameIndex;
        //    int lastEdge = 0;

        //    ProtocolBusPlotViewModel rFFEbusPlot = new ProtocolBusPlotViewModel();

        //    rFFEbusPlot.Tag = "RFFE BUS";
        //    rFFEbusPlot.VerticalScale = 4;
        //    if (busOffset.ContainsKey(eProtocol.RFFE))
        //        rFFEbusPlot.VerticalOffset = busOffset[eProtocol.RFFE];
        //    else
        //        rFFEbusPlot.VerticalOffset = -3.5;
        //    rFFEbusPlot.Channel = channel;
        //    ObservableCollection<PolygonBusData> BusData = new ObservableCollection<PolygonBusData>();
        //    var annotationStopIndex = 0;
        //    string emptyVal = "  .";
        //    for (int j = rffe_startFrameIndex; j <= rffe_stopFrameIndex; j++)
        //    {
        //        var currentPacket = RFFEResults.WareHouse.FrameCollection[j];
        //        double StartTimestamp = SPITimeStampConverter(currentPacket.StartTime);
        //        double StopTimestamp = SPITimeStampConverter(currentPacket.StopTime);
        //        eRFFECMDTYPE cmd = ProtocolInfoRepository.GetRFFECmdType(currentPacket.Command.Value);
        //        #region SSC
        //        //SSC
        //        annotationStopIndex = bufferlist.FindIndex(obj => obj.TimeStamp == StartTimestamp) + 2;
        //        var annotationStartIndex = GetPreviousEdge(annotationStopIndex, rFFE_dataIndex, bufferlist);
        //        StartTimestamp = bufferlist[annotationStartIndex].TimeStamp;
        //        if (annotationStopIndex != -1)
        //        {
        //            var bus = new PolygonBusData();
        //            StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //            bus.StartIndex = StartTimestamp * 1e9;
        //            bus.StopIndex = StopTimestamp * 1e9;
        //            bus.Data = "SSC" + emptyVal;
        //            bus.Brush = System.Windows.Media.Brushes.ForestGreen;
        //            BusData.Add(bus);
        //        }
        //        #endregion

        //        #region SlaveID
        //        //SlaveID
        //        annotationStartIndex = annotationStopIndex;
        //        annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferlist, 4, ref lastEdge);
        //        StartTimestamp = StopTimestamp;
        //        if (annotationStopIndex != -1)
        //        {
        //            var bus = new PolygonBusData();
        //            StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //            bus.StartIndex = StartTimestamp * 1e9;
        //            bus.StopIndex = StopTimestamp * 1e9;
        //            if (cmd == eRFFECMDTYPE.MASTER_CXT_TRANSFER_WRITE || cmd == eRFFECMDTYPE.MASTER_CXT_TRANSFER_READ || cmd == eRFFECMDTYPE.MASTER_WRITE || cmd == eRFFECMDTYPE.MASTER_READ || cmd == eRFFECMDTYPE.MASTER_OWNERSHIP_HANDOVER)
        //                bus.Data = "MID=" + "0x" + currentPacket.SlaveId.ToString("X2") + emptyVal;
        //            else
        //                bus.Data = "Slave=" + "0x" + currentPacket.SlaveId.ToString("X2") + emptyVal;
        //            bus.Brush = System.Windows.Media.Brushes.DarkCyan;
        //            BusData.Add(bus);
        //        }
        //        #endregion

        //        #region Command
        //        annotationStartIndex = annotationStopIndex;
        //        annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferlist, 8, ref lastEdge);
        //        StartTimestamp = StopTimestamp;
        //        if (annotationStopIndex != -1)
        //        {
        //            var bus = new PolygonBusData();
        //            StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //            bus.StartIndex = StartTimestamp * 1e9;
        //            bus.StopIndex = StopTimestamp * 1e9;
        //            bus.Data = ProtocolInfoRepository.GetRFFECmdType(currentPacket.Command.Value) + " =" + "0x" + currentPacket.Command.Value.ToString("X2") + emptyVal;
        //            bus.Brush = System.Windows.Media.Brushes.DarkGreen;
        //            BusData.Add(bus);
        //        }
        //        #endregion

        //        #region Parity
        //        annotationStartIndex = annotationStopIndex;
        //        annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferlist, 1, ref lastEdge);
        //        StartTimestamp = StopTimestamp;
        //        if (annotationStopIndex != -1)
        //        {
        //            var bus = new PolygonBusData();
        //            StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //            bus.StartIndex = StartTimestamp * 1e9;
        //            bus.StopIndex = StopTimestamp * 1e9;
        //            bus.Data = "P" + emptyVal;
        //            bus.Brush = System.Windows.Media.Brushes.DeepSkyBlue;
        //            BusData.Add(bus);
        //        }
        //        #endregion

        //        #region BPC
        //        if (currentPacket.Command.HasBP)
        //        {
        //            annotationStartIndex = annotationStopIndex;
        //            annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferlist, 1, ref lastEdge);

        //            if (cmd == eRFFECMDTYPE.REG_ZERO_WRITE && annotationStartIndex != -1)
        //            {
        //                int dummystop = GetPreviousEdge(annotationStartIndex, rFFE_clockIndex, bufferlist);
        //                if (dummystop == -1) return;
        //                if (annotationStopIndex == -1)
        //                {
        //                    annotationStopIndex = dummystop;
        //                }
        //                StartTimestamp = StopTimestamp;
        //                StopTimestamp = (2 * (bufferlist[annotationStartIndex].TimeStamp - bufferlist[dummystop].TimeStamp)) + StartTimestamp;
        //            }
        //            else
        //            {
        //                StartTimestamp = StopTimestamp;
        //                if (annotationStopIndex != -1)
        //                    StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //            }
        //            //if (annotationStopIndex != -1)
        //            {
        //                var bus = new PolygonBusData();
        //                //StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //                bus.StartIndex = StartTimestamp * 1e9;
        //                bus.StopIndex = StopTimestamp * 1e9;
        //                bus.Data = "BPC" + emptyVal;
        //                bus.Brush = System.Windows.Media.Brushes.DarkViolet;
        //                BusData.Add(bus);
        //            }
        //        }
        //        #endregion

        //        #region Interrupts

        //        if (cmd == eRFFECMDTYPE.INT_SUMMARY_IDENT)
        //        {
        //            annotationStartIndex = annotationStopIndex;
        //            annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferlist, 1, ref lastEdge);
        //            StartTimestamp = StopTimestamp;
        //            var bus = new PolygonBusData();
        //            if (annotationStopIndex != -1)
        //            {
        //                bus = new PolygonBusData();
        //                StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //                bus.StartIndex = StartTimestamp * 1e9;
        //                bus.StopIndex = StopTimestamp * 1e9;
        //                bus.Data = "ISI" + emptyVal;
        //                bus.Brush = System.Windows.Media.Brushes.Violet;
        //                BusData.Add(bus);
        //            }

        //            //BRC
        //            annotationStartIndex = annotationStopIndex;
        //            annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferlist, 1, ref lastEdge);
        //            StartTimestamp = StopTimestamp;
        //            if (annotationStopIndex != -1)
        //            {
        //                bus = new PolygonBusData();
        //                StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //                bus.StartIndex = StartTimestamp * 1e9;
        //                bus.StopIndex = StopTimestamp * 1e9;
        //                bus.Data = "BRC" + emptyVal;
        //                bus.Brush = System.Windows.Media.Brushes.HotPink;
        //                BusData.Add(bus);
        //            }

        //            //BPC
        //            annotationStartIndex = annotationStopIndex;
        //            annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferlist, 1, ref lastEdge);

        //            int dummystop = GetPreviousEdge(annotationStartIndex, rFFE_clockIndex, bufferlist);

        //            if (annotationStopIndex == -1)
        //            {
        //                annotationStopIndex = annotationStartIndex;
        //                StartTimestamp = StopTimestamp;
        //                StopTimestamp = (2 * (bufferlist[annotationStartIndex].TimeStamp - bufferlist[dummystop].TimeStamp)) + StartTimestamp;
        //                bus = new PolygonBusData();
        //                bus.StartIndex = StartTimestamp * 1e9;
        //                bus.StopIndex = StopTimestamp * 1e9;
        //                bus.Data = "BPC" + emptyVal;
        //                bus.Brush = System.Windows.Media.Brushes.HotPink;
        //                BusData.Add(bus);
        //            }
        //            else
        //            {
        //                StartTimestamp = StopTimestamp;
        //                StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //                bus = new PolygonBusData();
        //                bus.StartIndex = StartTimestamp * 1e9;
        //                bus.StopIndex = StopTimestamp * 1e9;
        //                bus.Data = "BPC" + emptyVal;
        //                bus.Brush = System.Windows.Media.Brushes.HotPink;
        //                BusData.Add(bus);
        //            }

        //            int ClockCycles = currentPacket.InterruptCount * 3;
        //            int counter = 16;

        //            for (int index = 0; index < ClockCycles; index += 3)
        //            {
        //                if (ClockCycles < 0)
        //                    break;

        //                counter--;
        //                annotationStartIndex = annotationStopIndex;
        //                annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferlist, 1, ref lastEdge);
        //                StartTimestamp = StopTimestamp;
        //                if (annotationStopIndex != -1)
        //                {
        //                    bus = new PolygonBusData();
        //                    StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //                    bus.StartIndex = StartTimestamp * 1e9;
        //                    bus.StopIndex = StopTimestamp * 1e9;
        //                    bus.Data = "INT" + emptyVal;
        //                    bus.Brush = System.Windows.Media.Brushes.MediumSlateBlue;
        //                    BusData.Add(bus);
        //                }

        //                annotationStartIndex = annotationStopIndex;
        //                annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferlist, 1, ref lastEdge);
        //                StartTimestamp = StopTimestamp;
        //                if (annotationStopIndex != -1)
        //                {
        //                    bus = new PolygonBusData();
        //                    StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //                    bus.StartIndex = StartTimestamp * 1e9;
        //                    bus.StopIndex = StopTimestamp * 1e9;
        //                    bus.Data = "ISC" + emptyVal;
        //                    bus.Brush = System.Windows.Media.Brushes.MediumVioletRed;
        //                    BusData.Add(bus);
        //                }

        //                annotationStartIndex = annotationStopIndex;
        //                annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferlist, 1, ref lastEdge);

        //                int dummy = GetPreviousEdge(annotationStartIndex, rFFE_clockIndex, bufferlist);

        //                if (annotationStopIndex == -1)
        //                {
        //                    annotationStopIndex = annotationStartIndex;
        //                    StartTimestamp = StopTimestamp;
        //                    StopTimestamp = (2 * (bufferlist[annotationStartIndex].TimeStamp - bufferlist[dummy].TimeStamp)) + StartTimestamp;
        //                }
        //                else
        //                {
        //                    if ((ClockCycles - index) == 1)
        //                    {
        //                        StartTimestamp = StopTimestamp;
        //                        StopTimestamp = (2 * (bufferlist[annotationStartIndex].TimeStamp - bufferlist[dummy].TimeStamp)) + StartTimestamp;
        //                    }
        //                    else
        //                    {
        //                        StartTimestamp = StopTimestamp;
        //                        StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //                    }
        //                }
        //                bus = new PolygonBusData();
        //                bus.StartIndex = StartTimestamp * 1e9;
        //                bus.StopIndex = StopTimestamp * 1e9;
        //                bus.Data = "BPC" + emptyVal;
        //                bus.Brush = System.Windows.Media.Brushes.Violet;
        //                BusData.Add(bus);

        //            }
        //        }
        //        #endregion

        //        #region MOH
        //        if (cmd == eRFFECMDTYPE.MASTER_OWNERSHIP_HANDOVER)
        //        {
        //            var bus = new PolygonBusData();
        //            #region Ack
        //            annotationStartIndex = annotationStopIndex;
        //            annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferlist, 1, ref lastEdge);
        //            StartTimestamp = StopTimestamp;
        //            if (annotationStopIndex != -1)
        //            {
        //                bus = new PolygonBusData();
        //                StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //                bus.StartIndex = StartTimestamp * 1e9;
        //                bus.StopIndex = StopTimestamp * 1e9;
        //                bus.Data = "ACK" + emptyVal;
        //                bus.Brush = System.Windows.Media.Brushes.Green;
        //                BusData.Add(bus);
        //            }

        //            #endregion

        //            //Next 2 Cycles 0
        //            #region MID
        //            annotationStartIndex = GetNextFallingEdgeIndexByCount(annotationStopIndex, rFFE_clockIndex, bufferlist, 2, ref lastEdge);
        //            annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferlist, 2, ref lastEdge);

        //            if (annotationStopIndex != -1 && annotationStartIndex != -1)
        //            {
        //                bus = new PolygonBusData();
        //                StartTimestamp = bufferlist[annotationStartIndex].TimeStamp;
        //                StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //                bus.StartIndex = StartTimestamp * 1e9;
        //                bus.StopIndex = StopTimestamp * 1e9;
        //                bus.Data = "MID" + emptyVal;
        //                bus.Brush = System.Windows.Media.Brushes.HotPink;
        //                BusData.Add(bus);
        //            }

        //            #endregion

        //            #region Parity
        //            annotationStartIndex = GetNextFallingEdgeIndexByCount(annotationStopIndex, rFFE_clockIndex, bufferlist, 3, ref lastEdge);
        //            annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferlist, 1, ref lastEdge);

        //            if (annotationStopIndex != -1 && annotationStartIndex != -1)
        //            {
        //                bus = new PolygonBusData();
        //                StartTimestamp = bufferlist[annotationStartIndex].TimeStamp;
        //                StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
        //                bus.StartIndex = StartTimestamp * 1e9;
        //                bus.StopIndex = StopTimestamp * 1e9;
        //                bus.Data = "P" + emptyVal;
        //                bus.Brush = System.Windows.Media.Brushes.DeepSkyBlue;
        //                BusData.Add(bus);
        //            }

        //            #endregion

        //            #region BHC
        //            annotationStartIndex = annotationStopIndex;
        //            annotationStopIndex = GetNextFallingEdgeIndexByCount(annotationStartIndex, rFFE_clockIndex, bufferlist, 1, ref lastEdge);

        //            var dummystop = GetPreviousEdge(annotationStartIndex, rFFE_clockIndex, bufferlist);

        //            if (annotationStopIndex == -1)
        //            {
        //                annotationStopIndex = annotationStartIndex;
        //            }
        //            StartTimestamp = StopTimestamp;
        //            StopTimestamp = (2 * (bufferlist[annotationStartIndex].TimeStamp - bufferlist[dummystop].TimeStamp)) + StartTimestamp;


        //            bus = new PolygonBusData();
        //            bus.StartIndex = StartTimestamp * 1e9;
        //            bus.StopIndex = StopTimestamp * 1e9;
        //            bus.Data = "BHC" + emptyVal;
        //            bus.Brush = System.Windows.Media.Brushes.DeepSkyBlue;
        //            BusData.Add(bus);

        //            #endregion
        //        }

        //        #endregion

        //        if (currentPacket.BytecountList != null && currentPacket.BytecountList.Count() > 0)
        //        {
        //            var data = PlotRFFEAnnotation(currentPacket.BytecountList, "Byte Count", ref annotationStartIndex, ref annotationStopIndex, ref StartTimestamp, ref StopTimestamp, bufferlist, cmd);
        //            foreach (var item in data)
        //                BusData.Add(item);
        //        }

        //        if (currentPacket.Address != null && currentPacket.Address.Count() > 0)
        //        {
        //            var data = PlotRFFEAnnotation(currentPacket.Address, "Address", ref annotationStartIndex, ref annotationStopIndex, ref StartTimestamp, ref StopTimestamp, bufferlist, cmd);
        //            foreach (var item in data)
        //                BusData.Add(item);
        //        }

        //        if (currentPacket.Data != null && currentPacket.Data.Count() > 0)
        //        {
        //            if (cmd != eRFFECMDTYPE.REG_ZERO_WRITE)
        //            {
        //                var data = PlotRFFEAnnotation(currentPacket.Data, "Data", ref annotationStartIndex, ref annotationStopIndex, ref StartTimestamp, ref StopTimestamp, bufferlist, cmd);
        //                foreach (var item in data)
        //                    BusData.Add(item);
        //            }
        //        }
        //    }

        //    foreach (var bust in BusData)
        //        rFFEbusPlot.BusFullData.Add(bust);
        //    System.Windows.Application.Current.Dispatcher.Invoke(
        //new Action(() => this.InfoTrend.Trend.BusResultInfoView = rFFEbusPlot));
        //    if (BusData.Count == 0)
        //    {
        //        prevIndexHolder[8] = -1;
        //        prevIndexHolder[9] = -1;
        //    }
        //}

        #endregion
    }

    public class SearchFilterViewModel_RFFE : SearchFilterViewModelBase<IFrame>
    {

        public SearchFilterViewModel_RFFE(CollectionViewSource source) : base(source) { }

        public SearchFilterViewModel_RFFE(IList collection) : base(collection)
        {
        }



        private string slaveID;

        public string SlaveID
        {
            get { return slaveID; }
            set
            {
                slaveID = value;
                OnPropertyChanged(nameof(SlaveID));
            }
        }

        private string regaddress;

        public string RegAddress
        {
            get { return regaddress; }
            set
            {
                regaddress = value;
                OnPropertyChanged(nameof(RegAddress));
            }
        }

        private string data;

        public string Data
        {
            get { return data; }
            set
            {
                data = value;
                OnPropertyChanged(nameof(Data));
            }
        }


        private eRFFECMDTYPE commandType;
        public eRFFECMDTYPE CommandType
        {
            get
            {
                return commandType;
            }
            set
            {
                commandType = value;
                OnPropertyChanged(nameof(commandType));
            }
        }


        private string byteCount;

        public string ByteCount
        {
            get { return byteCount; }
            set
            {
                byteCount = value;
                OnPropertyChanged(nameof(ByteCount));
            }
        }

        public override bool IsMatch(IFrame frame)
        {
            var rFFEFrame = (RFFEFrameStructure)frame;



            Int64 slaveIDMID = Int64.Parse(rFFEFrame.SlaveId.ToString());
            string slaveid = String.Format("0x{0:X}", slaveIDMID);
            string slaveid1 = String.Format("{0:X}", slaveIDMID);
            Int64 RegAdd = Int64.Parse(rFFEFrame.IntAddress.ToString());
            string regadd = String.Format("0x{0:X}", RegAdd);
            string regadd1 = String.Format("{0:X}", RegAdd);
            Int64 ByteCounts = Int64.Parse(rFFEFrame.ByteCount.ToString());
            string bytecount = String.Format("0x{0:X}", ByteCounts);
            string bytecount1 = String.Format("{0:x}", ByteCounts);

            List<string> datStr = new List<string>();
            if (Data != null)
            {
                if (CheckHexa(Data.ToLower()))
                {
                    if (rFFEFrame.Data != null)
                    {
                        foreach (var db in rFFEFrame.Data)
                        {

                            datStr.Add($"0x{Convert.ToInt32(db.Value):X}");
                        }
                    }
                }
                else
                {
                    if (rFFEFrame.Data != null)
                    {
                        foreach (var db in rFFEFrame.Data)
                        {
                            datStr.Add($"{Convert.ToInt32(db.Value):X}");
                        }
                    }

                }
            }
            List<string> NewData = new List<string>();
            foreach (var item in datStr)
            {
                var num = item.ToLower();
                NewData.Add(num);

            }

            bool result = true;
            bool searchValid = false;
            if (!string.IsNullOrEmpty(SlaveID))
            {
                result &= slaveid.ToLower().Equals(SlaveID.ToLower()) || slaveid1.ToLower().Equals(SlaveID.ToLower());
                searchValid = true;
            }
            if (!string.IsNullOrEmpty(RegAddress))
            {
                result &= regadd.ToLower().Equals(RegAddress.ToLower()) || regadd1.ToLower().Equals(RegAddress.ToLower());
                searchValid = true;
            }
            if (!string.IsNullOrEmpty(ByteCount))
            {
                result &= byteCount.ToLower().Equals(ByteCount.ToLower()) || bytecount1.ToLower().Equals(ByteCount.ToLower());
                searchValid = true;
            }

            if (!string.IsNullOrEmpty(Data))
            {
                result &= NewData.Contains(Data.ToLower());

                searchValid = true;
            }

            // I3C Command
            if (commandType != eRFFECMDTYPE.None)
            {
                result &= rFFEFrame.Command.CmdType == commandType;
                searchValid = true;
            }
            else
            if (commandType == eRFFECMDTYPE.None)
            {
                result &= true;
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
