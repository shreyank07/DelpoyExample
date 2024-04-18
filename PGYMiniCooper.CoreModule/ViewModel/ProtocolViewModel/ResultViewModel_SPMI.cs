using CommunityToolkit.Mvvm.DependencyInjection;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Helpers;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.CANStructure;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using PGYMiniCooper.DataModule.Structure.QSPIStructure;

using PGYMiniCooper.DataModule.Structure.SPMIStructure;
using Prodigy.Business;
using Prodigy.WaveformControls.Interfaces;
using ProdigyFramework.Behavior;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace PGYMiniCooper.CoreModule.ViewModel.ProtocolViewModel
{
    public class ResultViewModel_SPMI : ViewModelBase, IResultViewModel
    {
        public ResultViewModel_SPMI(ConfigViewModel_SPMI config, ResultModel_SPMI result, TriggerViewModel trigger)
        {
            //this.Config = config;
            //wareHouse = result;
            //// TODO: add logic to subscribe update from all protocols
            ////ResultAdder.Instance.OnSelectionChanged += GetCurrentFrame;

            this.Config = config;
            this.WareHouse = result;
            this.trigger = trigger;
            wareHouse.Reset();
            sPMIFrameCollection = new ProdigyFramework.Collections.AsyncObservableCollection<SPMIFrameStructure>();
            wareHouse.OnFramesDecoded += WareHouse_OnFramesDecoded;
            SearchParameter = new SearchFilterViewModel_SPMI((IList)sPMIFrameCollection);
            SearchParameter.OnSelectionChanged += SearchParameter_SelectionChanged;
        }


        private void WareHouse_OnFramesDecoded(object sender, IReadOnlyList<IFrame> frames)
        {
            var spmiFrames = frames.OfType<SPMIFrameStructure>();

            ((ProdigyFramework.Collections.AsyncObservableCollection<SPMIFrameStructure>)sPMIFrameCollection).AddRange(spmiFrames);

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

        private ConfigViewModel_SPMI config;
        public ConfigViewModel_SPMI Config
        {
            get { return config; }
            set
            {
                config = value;
                RaisePropertyChanged(nameof(Config));
            }
        }

        ResultModel_SPMI wareHouse;
        public ResultModel_SPMI WareHouse
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

        public SPMIFrameStructure SelectedFrame
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

        public IList<SPMIFrameStructure> SPMIFrameCollection
        {
            get
            {
                return sPMIFrameCollection;
            }
            set
            {
                sPMIFrameCollection = value;
                RaisePropertyChanged(nameof(SPMIFrameCollection));
            }
        }
        private IList<SPMIFrameStructure> sPMIFrameCollection;

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
            SelectedFrame = null;
            //SearchParameter.Reset();
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
            SelectedFrame = SPMIFrameCollection[WareHouse.TriggerPacket];
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
            this.SelectedFrame = (SPMIFrameStructure)frame;
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
        public void Serach()
        {
            throw new NotImplementedException();
        }
        public SearchFilterViewModel_SPMI SearchParameter { get; }
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

        IFrame IResultViewModel.SelectedFrame
        {
            get { return SelectedFrame; }
            set
            {
                SelectedFrame = (SPMIFrameStructure)value;
            }
        }

        ICollection<IFrame> IResultViewModel.ResultCollection => wareHouse.FrameCollection.OfType<IFrame>().ToList();

        List<ChannelInfo> IResultViewModel.AvailableBusChannels => new List<ChannelInfo> { new ChannelInfo("SPMI", ((IResultViewModel)this).Config.Channels.FirstOrDefault(c => c.ChannelIndex == config.ChannelIndex_SDA).ChannelIndex) };

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
            var resultList = SPMIFrameCollection.SkipWhile(r => r.StopTime < startTime).TakeWhile(r => r.StartTime <= stopTime).ToList();
            foreach (var currentPacket in resultList)
            {
                double StartTimestamp = TimeStampConverter(currentPacket.StartTime);
                double StopTimestamp = TimeStampConverter(currentPacket.StopTime);

                #region Arbitration
                if (currentPacket.HasArbitration)
                {
                    var eachBus = SPMIArbitrationBusDiagram(bufferlist, edgeFinding, currentPacket, StartTimestamp);
                    foreach (var b in eachBus)
                        yield return b;
                }
                #endregion
                #region SSC
                //SSC
                annotationStopIndex = bufferlist.FindIndex(obj => obj.TimeStamp == StartTimestamp) + 2;
                edgeFinding.GetPreviousEdge(dataChannel, annotationStopIndex, out eEdgeType edgeType, out int annotationStartIndex);
                
                StartTimestamp = bufferlist[annotationStartIndex].TimeStamp;
                // Does not make sense
                if (annotationStopIndex != -1)
                {
                    var bus = new PolygonBusData();
                    StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                    bus.StartIndex = StartTimestamp * 1e9;
                    bus.StopIndex = StopTimestamp * 1e9;
                    bus.Data = "SSC" + emptyVal;
                    bus.Brush = System.Windows.Media.Brushes.DarkViolet;

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
                    if (currentPacket.Command.CmdType == eSPMICMDTYPE.MASTER_READ || currentPacket.Command.CmdType == eSPMICMDTYPE.MASTER_WRITE ||
                     currentPacket.Command.CmdType == eSPMICMDTYPE.DDB_MA_R)
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

                eSPMICMDTYPE cmd = currentPacket.Command.CmdType;
                #region BP
                if (currentPacket.Command.HasBP)
                {
                    annotationStartIndex = annotationStopIndex;
                    edgeFinding.GetNextRisingEdgeIndex(clockChannel, annotationStartIndex, out annotationStopIndex);

                    if (cmd == eSPMICMDTYPE.REG_ZERO_WRITE || cmd == eSPMICMDTYPE.RESET || cmd == eSPMICMDTYPE.SHUTDOWN || cmd == eSPMICMDTYPE.SLEEP || cmd == eSPMICMDTYPE.WAKEUP)
                    {
                        edgeFinding.GetNextFallingEdgeIndex(clockChannel, annotationStartIndex, out annotationStopIndex);
                    }
                    StartTimestamp = StopTimestamp;
                    if (annotationStopIndex != -1)
                    {
                        var bus = new PolygonBusData();
                        StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                        bus.StartIndex = StartTimestamp * 1e9;
                        bus.StopIndex = StopTimestamp * 1e9;
                        bus.Data = "BPC" + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.DarkViolet;

                        yield return bus;
                    }
                }

                if (cmd == eSPMICMDTYPE.RESET || cmd == eSPMICMDTYPE.SHUTDOWN || cmd == eSPMICMDTYPE.SLEEP || cmd == eSPMICMDTYPE.WAKEUP || cmd == eSPMICMDTYPE.REG_ZERO_WRITE)
                {
                    if (config.SPMIVersion == eVersion.two)
                    {
                        annotationStartIndex = annotationStopIndex;
                        StartTimestamp = StopTimestamp;
                        if (edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 1, eEdgeType.RISING_EDGE, out annotationStopIndex))
                        {
                            var bus = new PolygonBusData();
                            StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                            bus.StartIndex = StartTimestamp * 1e9;
                            bus.StopIndex = StopTimestamp * 1e9;
                            bus.Data = currentPacket.Command.AckNackType == eAcknowledgeType.ACK ? "A" : "N" + emptyVal;
                            if (currentPacket.Command.AckNackType == eAcknowledgeType.ACK)
                                bus.Brush = System.Windows.Media.Brushes.Green;
                            else
                                bus.Brush = System.Windows.Media.Brushes.Crimson;

                            yield return bus;
                        }

                        //BPC
                        annotationStartIndex = annotationStopIndex;
                        edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 1, eEdgeType.RISING_EDGE, out annotationStopIndex);

                        edgeFinding.GetPreviousEdge(clockChannel, annotationStartIndex, out edgeType, out int dummystop);

                        if (annotationStopIndex == -1)
                        {
                            annotationStopIndex = dummystop;
                        }
                        StartTimestamp = StopTimestamp;

                        if (dummystop != -1)
                        {
                            var bus = new PolygonBusData();
                            StopTimestamp = (2 * (bufferlist[annotationStartIndex].TimeStamp - bufferlist[dummystop].TimeStamp)) + StartTimestamp;
                            bus.StartIndex = StartTimestamp * 1e9;
                            bus.StopIndex = StopTimestamp * 1e9;
                            bus.Data = "BPC" + emptyVal;
                            bus.Brush = System.Windows.Media.Brushes.DarkViolet;
                            yield return bus;
                        }
                    }
                }

                #region MOH

                if (cmd == eSPMICMDTYPE.TRFR_BUS_OWNERSHIP)
                {
                    int TBO_Start_Index = annotationStopIndex;
                    if (currentPacket.MPL_TBO != 3)
                    {
                        #region MPL
                        annotationStartIndex = annotationStopIndex;
                        if (currentPacket.MPL_TBO == 2)
                            edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 1, eEdgeType.RISING_EDGE, out annotationStopIndex);
                        else if (currentPacket.MPL_TBO == 1)
                            edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 2, eEdgeType.RISING_EDGE, out annotationStopIndex);
                        else if (currentPacket.MPL_TBO == 0)
                            edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 3, eEdgeType.RISING_EDGE, out annotationStopIndex);
                        else
                            edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 1, eEdgeType.RISING_EDGE, out annotationStopIndex);
                        StartTimestamp = StopTimestamp;

                        if (annotationStopIndex != -1)
                        {
                            var bus = new PolygonBusData();
                            StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                            bus.StartIndex = StartTimestamp * 1e9;
                            bus.StopIndex = StopTimestamp * 1e9;
                            if (currentPacket.MPL_TBO >= 0 && currentPacket.MPL_TBO < 3)
                                bus.Data = "MPL :" + currentPacket.MPL_TBO + emptyVal;
                            else
                                bus.Data = "MPL" + emptyVal;
                            bus.Brush = System.Windows.Media.Brushes.Wheat;

                            yield return bus;
                        }
                        #endregion
                        //Next 2 Cycles 0
                        #region MID
                        annotationStartIndex = annotationStopIndex;
                        StartTimestamp = bufferlist[annotationStartIndex].TimeStamp;
                        if (edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 2, eEdgeType.RISING_EDGE, out annotationStopIndex))
                        {
                            var bus = new PolygonBusData();
                            StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                            bus.StartIndex = StartTimestamp * 1e9;
                            bus.StopIndex = StopTimestamp * 1e9;
                            if (currentPacket.MID_TBO >= 0 && currentPacket.MID_TBO < 3)
                                bus.Data = "MID :" + currentPacket.MID_TBO + emptyVal;
                            else
                                bus.Data = "MID" + emptyVal;
                            bus.Brush = System.Windows.Media.Brushes.Wheat;

                            yield return bus;
                        }

                        #endregion
                    }

                    #region Parity
                    edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, TBO_Start_Index, 8, eEdgeType.RISING_EDGE, out annotationStartIndex);
                    edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 1, eEdgeType.RISING_EDGE, out annotationStopIndex);


                    if (annotationStopIndex != -1 && annotationStartIndex != -1)
                    {
                        var bus = new PolygonBusData();
                        StartTimestamp = bufferlist[annotationStartIndex].TimeStamp;
                        StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                        bus.StartIndex = StartTimestamp * 1e9;
                        bus.StopIndex = StopTimestamp * 1e9;
                        bus.Data = "P" + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.DarkViolet;

                        yield return bus;
                    }

                    #endregion

                    #region BHC
                    annotationStartIndex = annotationStopIndex;
                    edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 1, eEdgeType.RISING_EDGE, out annotationStopIndex);

                    edgeFinding.GetPreviousEdge(clockChannel, annotationStartIndex, out edgeType, out int dummystop);
                    if (annotationStopIndex == -1)
                    {
                        annotationStopIndex = annotationStartIndex;
                    }
                    StartTimestamp = StopTimestamp;
                    if (dummystop != -1)
                    {
                        var bus = new PolygonBusData();
                        StopTimestamp = (2 * (bufferlist[annotationStartIndex].TimeStamp - bufferlist[dummystop].TimeStamp)) + StartTimestamp;
                        bus.StartIndex = StartTimestamp * 1e9;
                        bus.StopIndex = StopTimestamp * 1e9;
                        bus.Data = "BHC" + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.DeepSkyBlue;

                        yield return bus;
                    }

                    #endregion
                }

                #endregion

                #endregion

                if (currentPacket.BytecountList != null && currentPacket.BytecountList.Count() > 0)
                {
                    var data = PlotSPMIAnnotation(bufferlist, edgeFinding, currentPacket.BytecountList, "Byte Count", ref annotationStartIndex, ref annotationStopIndex, ref StartTimestamp, ref StopTimestamp, bufferlist, cmd);
                    foreach (var item in data)
                        yield return item;
                }

                if (currentPacket.Address != null && currentPacket.Address.Count() > 0)
                {
                    var data = PlotSPMIAnnotation(bufferlist, edgeFinding, currentPacket.Address, "Address", ref annotationStartIndex, ref annotationStopIndex, ref StartTimestamp, ref StopTimestamp, bufferlist, cmd);
                    foreach (var item in data)
                        yield return item;
                }

                if (currentPacket.Data != null && currentPacket.Data.Count() > 0)
                {
                    if (cmd != eSPMICMDTYPE.REG_ZERO_WRITE)
                    {
                        var data = PlotSPMIAnnotation(bufferlist, edgeFinding, currentPacket.Data, "Data", ref annotationStartIndex, ref annotationStopIndex, ref StartTimestamp, ref StopTimestamp, bufferlist, cmd);
                        foreach (var item in data)
                            yield return item;
                    }
                }

            }
        }

        List<IBusData> PlotSPMIAnnotation(List<DiscreteWaveForm> bufferlist, DigitalWaveformEdgeFinding edgeFinding, SPMIPacketStructure[] currentList, string annoType, ref int annotationStartIndex, ref int annotationStopIndex, ref double StartTimestamp, ref double StopTimestamp, List<DiscreteWaveForm> bufferList, eSPMICMDTYPE cmdtype)
        {
            List<IBusData> busData = new List<IBusData>();

            int loopCount = 0;
            eChannles clockChannel = Config.ChannelIndex_SCL;
            string emptyVal = " .";
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
                    bus.Data = annoType + "=0x" + currentpacket.Value.ToString("X2") + emptyVal;
                    if (annoType == "Data")
                    {
                        bus.Brush = System.Windows.Media.Brushes.RoyalBlue;
                        if (cmdtype == eSPMICMDTYPE.AUTHENTICATE)
                        {
                            if (loopCount % 2 == 0)
                                bus.Data = "Challenge Data =0x" + currentpacket.Value.ToString("X2") + emptyVal;
                            else
                                bus.Data = "Response Data =0x" + currentpacket.Value.ToString("X2") + emptyVal;
                        }
                    }
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

                #region Ack/Nack
                if (annoType == "Data" && currentpacket.AckNackType != eAcknowledgeType.NA)
                {
                    annotationStartIndex = annotationStopIndex;
                    StartTimestamp = StopTimestamp;
                    if (edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndex, 1, eEdgeType.RISING_EDGE, out annotationStopIndex))
                    {
                        var bus = new PolygonBusData();
                        StopTimestamp = bufferlist[annotationStopIndex].TimeStamp;
                        bus.StartIndex = StartTimestamp * 1e9;
                        bus.StopIndex = StopTimestamp * 1e9;
                        bus.Data = currentpacket.AckNackType == eAcknowledgeType.ACK ? "A" : "N" + emptyVal;
                        if (currentpacket.AckNackType == eAcknowledgeType.ACK)
                            bus.Brush = System.Windows.Media.Brushes.Green;
                        else
                            bus.Brush = System.Windows.Media.Brushes.Crimson;

                        busData.Add(bus);
                    }

                    //BPC


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

                loopCount++;
            }

            return busData;
        }

        /// <summary>
        /// Bus diagram for Arbitration
        /// includes line and Polygon annotations
        /// </summary>
        ///
        IEnumerable<IBusData> SPMIArbitrationBusDiagram(List<DiscreteWaveForm> bufferlist, DigitalWaveformEdgeFinding edgeFinding, SPMIFrameStructure currentFrame, double StartTimestamp)
        {
            eChannles clockChannel = config.ChannelIndex_SCL;
            eChannles dataChannel = config.ChannelIndex_SDA;

            int sPMI_clockIndex = (int)Config.ChannelIndex_SCL;
            int sPMI_dataIndex = (int)Config.ChannelIndex_SDA;
            int ssc;
            string emptyVal = string.Empty;
            #region Arbitration Sequence
            double start = TimeStampConverter(currentFrame.ArbitrationTimeStamp);
            double stop = 0;
            var annotationStartIndexMArb = bufferlist.FindIndex(obj => obj.TimeStamp == start);
            if (annotationStartIndexMArb == -1)
                yield break;

            int annotationStopIndexMArb = 0;
            if (currentFrame.IsMethod1orRCS)
                edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndexMArb, 2, eEdgeType.RISING_EDGE, out annotationStopIndexMArb);
            else if (currentFrame.IsMethod3)
                edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndexMArb, 3, eEdgeType.RISING_EDGE, out annotationStopIndexMArb);
            else
                edgeFinding.GetNextFallingEdgeIndex(dataChannel, annotationStartIndexMArb, out annotationStopIndexMArb);

            start = bufferlist[annotationStartIndexMArb].TimeStamp;
            if (annotationStopIndexMArb != -1)
            {
                var bus = new PolygonBusData();
                stop = bufferlist[annotationStopIndexMArb].TimeStamp;
                bus.StartIndex = start * 1e9;
                bus.StopIndex = stop * 1e9;
                bus.Data = "Arbitration" + emptyVal;
                bus.Brush = System.Windows.Media.Brushes.HotPink;
                
                yield return bus;
            }
            else
            {
                yield break;
            }
            #endregion

            #region C bit
            annotationStartIndexMArb = annotationStopIndexMArb;
            start = bufferlist[annotationStartIndexMArb].TimeStamp;
            if (edgeFinding.GetNextRisingEdgeIndex(clockChannel, annotationStartIndexMArb, out annotationStopIndexMArb))
            {
                var bus = new PolygonBusData();
                stop = bufferlist[annotationStopIndexMArb].TimeStamp;
                bus.StartIndex = start * 1e9;
                bus.StopIndex = stop * 1e9;
                bus.Data = "C bit" + emptyVal;
                bus.Brush = System.Windows.Media.Brushes.HotPink;

                yield return bus;
            }
            else
            {
                yield break;
            }
            #endregion

            if (currentFrame.HasC_bit)
            {
                #region MID
                annotationStartIndexMArb = annotationStopIndexMArb;
                start = bufferlist[annotationStartIndexMArb].TimeStamp;
                if (edgeFinding.GetNextRisingEdgeIndex(clockChannel, annotationStartIndexMArb, out annotationStopIndexMArb))
                {
                    var bus = new PolygonBusData();
                    stop = bufferlist[annotationStopIndexMArb].TimeStamp;
                    bus.StartIndex = start * 1e9;
                    bus.StopIndex = stop * 1e9;
                    bus.Data = "MID : " + currentFrame.MID + emptyVal;
                    bus.Brush = System.Windows.Media.Brushes.HotPink;

                    yield return bus;
                }
                else
                {
                    yield break;
                }
                #endregion

                #region BPC
                annotationStartIndexMArb = annotationStopIndexMArb;
                start = bufferlist[annotationStartIndexMArb].TimeStamp;

                if (edgeFinding.GetNextRisingEdgeIndex(clockChannel, annotationStartIndexMArb, out annotationStopIndexMArb))
                {
                    var bus = new PolygonBusData();
                    stop = bufferlist[annotationStopIndexMArb].TimeStamp;
                    bus.StartIndex = start * 1e9;
                    bus.StopIndex = stop * 1e9;
                    bus.Data = "BPC" + emptyVal;
                    bus.Brush = System.Windows.Media.Brushes.Wheat;

                    yield return bus;
                }
                else
                {
                    yield break;
                }
                #endregion
            }

            #region A_bit
            annotationStartIndexMArb = annotationStopIndexMArb;
            start = bufferlist[annotationStartIndexMArb].TimeStamp;
            if (edgeFinding.GetNextRisingEdgeIndex(clockChannel, annotationStartIndexMArb, out annotationStopIndexMArb))
            {
                var bus = new PolygonBusData();
                stop = bufferlist[annotationStopIndexMArb].TimeStamp;
                bus.StartIndex = start * 1e9;
                bus.StopIndex = stop * 1e9;
                bus.Data = "A bit" + emptyVal;
                bus.Brush = System.Windows.Media.Brushes.Turquoise;
                
                yield return bus;
            }
            else
            {
                yield break;
            }
            #endregion

            if (currentFrame.HasA_bit)
            {
                #region Slave Arbitration - A bit

                #region BPC
                annotationStartIndexMArb = annotationStopIndexMArb;
                start = bufferlist[annotationStartIndexMArb].TimeStamp;

                if (edgeFinding.GetNextRisingEdgeIndex(clockChannel, annotationStartIndexMArb, out annotationStopIndexMArb))
                {
                    var bus = new PolygonBusData();
                    stop = bufferlist[annotationStopIndexMArb].TimeStamp;
                    bus.StartIndex = start * 1e9;
                    bus.StopIndex = stop * 1e9;
                    bus.Data = "BPC" + emptyVal;
                    bus.Brush = System.Windows.Media.Brushes.Wheat;

                    yield return bus;
                }
                else
                {
                    yield break;
                }
                #endregion

                #region RCS Slave ID
                annotationStartIndexMArb = annotationStopIndexMArb;
                start = bufferlist[annotationStartIndexMArb].TimeStamp;
                if (edgeFinding.GetPreviousEdge(dataChannel, bufferlist.FindIndex(obj => obj.TimeStamp == StartTimestamp) + 2 , out eEdgeType edgeType, out annotationStopIndexMArb))
                {
                    var bus = new PolygonBusData();
                    stop = bufferlist[annotationStopIndexMArb].TimeStamp;
                    bus.StartIndex = start * 1e9;
                    bus.StopIndex = stop * 1e9;
                    if (currentFrame.SlaveIdRCS != null)
                        bus.Data = "Slave (RCS)=" + "0x" + currentFrame.SlaveIdRCS.ToString("X2") + emptyVal;
                    else
                        bus.Data = "Slave (RCS)" + emptyVal;
                    bus.Brush = System.Windows.Media.Brushes.Orange;

                    yield return bus;
                }
                else
                {
                    yield break;
                }
                #endregion

                #endregion
            }
            else
            {
                #region MPL
                if (currentFrame.HasMPLPrimary)
                {
                    annotationStartIndexMArb = annotationStopIndexMArb;
                    bool edgeFound = false;
                    if (currentFrame.HasMPLBitsPrimary)
                    {
                        if (currentFrame.MPL == 0)
                        {
                            edgeFound = edgeFinding.GetNextRisingEdgeIndex(clockChannel, annotationStartIndexMArb, out annotationStopIndexMArb);
                        }
                        else if (currentFrame.MPL == 1)
                        {
                            edgeFound = edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndexMArb, 2, eEdgeType.RISING_EDGE, out annotationStopIndexMArb);
                        }
                        else if (currentFrame.MPL == 2)
                        {
                            edgeFound = edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndexMArb, 3, eEdgeType.RISING_EDGE, out annotationStopIndexMArb);
                        }
                        else
                        {
                            edgeFound = edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndexMArb, 4, eEdgeType.RISING_EDGE, out annotationStopIndexMArb);
                        }
                    }
                    else
                        edgeFound = edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndexMArb, 4, eEdgeType.RISING_EDGE, out annotationStopIndexMArb);

                    edgeFinding.GetPreviousEdge(dataChannel, bufferlist.FindIndex(obj => obj.TimeStamp == StartTimestamp) + 2, out eEdgeType edgeType, out ssc);
                    annotationStopIndexMArb = (annotationStopIndexMArb < ssc) ? annotationStopIndexMArb : ssc;

                    start = bufferlist[annotationStartIndexMArb].TimeStamp;
                    if (annotationStopIndexMArb != -1)
                    {
                        var bus = new PolygonBusData();
                        stop = bufferlist[annotationStopIndexMArb].TimeStamp;
                        bus.StartIndex = start * 1e9;
                        bus.StopIndex = stop * 1e9;
                        if (currentFrame.HasMPLBitsPrimary)
                            bus.Data = "MPL Primary, MPL : " + currentFrame.MPL + emptyVal;
                        else
                            bus.Data = "MPL Primary" + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.DarkSeaGreen;
                        
                        yield return bus;
                    }
                }
                #endregion

                if (currentFrame.HasSr_bit)
                {
                    #region Slave Arbitration - Sr bit

                    #region Sr bit
                    annotationStartIndexMArb = annotationStopIndexMArb;
                    start = bufferlist[annotationStartIndexMArb].TimeStamp;
                    if (edgeFinding.GetNextRisingEdgeIndex(clockChannel, annotationStartIndexMArb, out annotationStopIndexMArb))
                    {
                        var bus = new PolygonBusData();
                        stop = bufferlist[annotationStopIndexMArb].TimeStamp;
                        bus.StartIndex = start * 1e9;
                        bus.StopIndex = stop * 1e9;
                        bus.Data = "Sr bit" + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.RoyalBlue;

                        yield return bus;
                    }
                    else
                    {
                        yield break;
                    }

                    #endregion

                    #region RCS Slave ID
                    annotationStartIndexMArb = annotationStopIndexMArb;
                    start = bufferlist[annotationStartIndexMArb].TimeStamp;
                    if (edgeFinding.GetPreviousEdge(dataChannel, bufferlist.FindIndex(obj => obj.TimeStamp == StartTimestamp) + 2, out eEdgeType edgeType, out annotationStopIndexMArb))
                    {
                        var bus = new PolygonBusData();
                        stop = bufferlist[annotationStopIndexMArb].TimeStamp;
                        bus.StartIndex = start * 1e9;
                        bus.StopIndex = stop * 1e9;
                        if (currentFrame.SlaveIdRCS != null)
                            bus.Data = "Slave (RCS)=" + "0x" + currentFrame.SlaveIdRCS.ToString("X2") + emptyVal;
                        else
                            bus.Data = "Slave (RCS)" + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.Orange;

                        yield return bus;
                    }

                    #endregion

                    #endregion
                }
                else
                {
                    #region SR bit
                    annotationStartIndexMArb = annotationStopIndexMArb;
                    start = bufferlist[annotationStartIndexMArb].TimeStamp;
                    if (edgeFinding.GetNextRisingEdgeIndex(clockChannel, annotationStartIndexMArb, out annotationStopIndexMArb))
                    {
                        var bus = new PolygonBusData();
                        stop = bufferlist[annotationStopIndexMArb].TimeStamp;
                        bus.StartIndex = start * 1e9;
                        bus.StopIndex = stop * 1e9;
                        bus.Data = "Sr bit" + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.Turquoise;

                        yield return bus;
                    }
                    #endregion

                    #region MPL
                    if (currentFrame.HasMPLSecondary && !currentFrame.HasMPLBitsPrimary)
                    {
                        annotationStartIndexMArb = annotationStopIndexMArb;
                        bool edgeFound = false;
                        if (currentFrame.HasMPLBitsSecondary)
                        {
                            if (currentFrame.MPL == 0)
                            {
                                edgeFound = edgeFinding.GetNextRisingEdgeIndex(clockChannel, annotationStartIndexMArb, out annotationStopIndexMArb);
                            }
                            else if (currentFrame.MPL == 1)
                            {
                                edgeFound = edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndexMArb, 2, eEdgeType.RISING_EDGE, out annotationStopIndexMArb);
                            }
                            else if (currentFrame.MPL == 2)
                            {
                                edgeFound = edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndexMArb, 3, eEdgeType.RISING_EDGE, out annotationStopIndexMArb);
                            }
                            else
                            {
                                edgeFound = edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndexMArb, 4, eEdgeType.RISING_EDGE, out annotationStopIndexMArb);
                            }
                        }
                        else
                            edgeFound = edgeFinding.GetNextEdgeByCountWithEdge(clockChannel, annotationStartIndexMArb, 4, eEdgeType.RISING_EDGE, out annotationStopIndexMArb);

                        edgeFinding.GetPreviousEdge(dataChannel, bufferlist.FindIndex(obj => obj.TimeStamp == StartTimestamp) + 2, out eEdgeType edgeType, out ssc);
                        annotationStopIndexMArb = (annotationStopIndexMArb < ssc) ? annotationStopIndexMArb : ssc;

                        start = bufferlist[annotationStartIndexMArb].TimeStamp;
                        if (annotationStopIndexMArb != -1)
                        {
                            var bus = new PolygonBusData();
                            stop = bufferlist[annotationStopIndexMArb].TimeStamp;
                            bus.StartIndex = start * 1e9;
                            bus.StopIndex = stop * 1e9;
                            if (currentFrame.HasMPLBitsPrimary)
                                bus.Data = "MPL Secondary, MPL : " + currentFrame.MPL + emptyVal;
                            else
                                bus.Data = "MPL Secondary" + emptyVal;
                            bus.Brush = System.Windows.Media.Brushes.DarkSeaGreen;

                            yield return bus;
                        }
                    }
                    #endregion
                }
            }
        }
        #endregion
    }



    public class SearchFilterViewModel_SPMI : SearchFilterViewModelBase<IFrame>
    {

        public SearchFilterViewModel_SPMI(CollectionViewSource source) : base(source) { }

        public SearchFilterViewModel_SPMI(IList collection) : base(collection)
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


        private eSPMICMDTYPE commandType;
        public eSPMICMDTYPE CommandType
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
            var sPMIFrame = (SPMIFrameStructure)frame;



            Int64 slaveIDMID = Int64.Parse(sPMIFrame.SlaveId.ToString());
            string slaveid = String.Format("0x{0:X}", slaveIDMID);
            string slaveid1 = String.Format("{0:X}", slaveIDMID);
            Int64 RegAdd = Int64.Parse(sPMIFrame.IntAddress.ToString());
            string regadd = String.Format("0x{0:X}", RegAdd);
            string regadd1 = String.Format("{0:X}", RegAdd);
            Int64 ByteCounts = Int64.Parse(sPMIFrame.ByteCount.ToString());
            string bytecount = String.Format("0x{0:X}", ByteCounts);
            string bytecount1 = String.Format("{0:x}", ByteCounts);
     
            List<string> datStr = new List<string>();
            if (Data != null)
            {
                if (CheckHexa(Data.ToLower()))
                {
                    if (sPMIFrame.Data != null)
                    {
                        foreach (var db in sPMIFrame.Data)
                        {

                            datStr.Add($"0x{Convert.ToInt32(db.Value):X}");
                        }
                    }
                }
                else
                {
                    if (sPMIFrame.Data != null)
                    {
                        foreach (var db in sPMIFrame.Data)
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
            if (commandType != eSPMICMDTYPE.DEFAULT)
            {
                result &= sPMIFrame.Command.CmdType == commandType;
                searchValid = true;
            }
            else
            if (commandType == eSPMICMDTYPE.DEFAULT)
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
