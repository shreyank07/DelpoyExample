using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Configuration;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.CANStructure;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using PGYMiniCooper.DataModule.Structure.I3CStructure;
using PGYMiniCooper.DataModule.Structure.SPIStructure;
using Prodigy.WaveformControls.Interfaces;
using ProdigyFramework.Behavior;
using Prodigy.Framework.Collections;
using ProdigyFramework.ComponentModel;
using Prodigy.Framework.Interfaces;
using Sprache;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.XPath;
using ProdigyFramework.Collections;

namespace PGYMiniCooper.CoreModule.ViewModel.ProtocolViewModel
{
    public class ResultViewModel_I3C : ViewModelBase, IResultViewModel
    {
          
        private readonly IDataProvider dataProvider;
        public ResultViewModel_I3C(ConfigViewModel_I3C config, ResultModel_I3C model, TriggerViewModel trigger, bool isCombinedViewActive)
        {
            this.Config = config;
            this.wareHouse = model;
            this.trigger = trigger;
            this.isCombinedViewActive = isCombinedViewActive;
            wareHouse.Reset();
            resultCollection = new ProdigyFramework.Collections.AsyncObservableCollection<FramePattern>();
            wareHouse.OnFramesDecoded += WareHouse_OnFramesDecoded;

            ASyncCollection = new AsyncVirtualizingCollection<I3CListModel>
(new ItemsProvider<I3CListModel>(new Action<IList<I3CListModel>, int, int>(FetchRange)), 200, 30000);
            dataProvider = Ioc.Default.GetService<IDataProvider>();
            dataProvider.OnMessageAvailable += this.DataProvider_OnMessageAvailable;
            SearchParameter = new SearchFilterViewModel_I3C((IList)resultCollection);
            SearchParameter.OnSelectionChanged += SearchParameter_SelectionChanged;
        }


        private void DataProvider_OnMessageAvailable(object sender, MessageAvailableEventArgs e)
        {
            if (e.ProtocolName == Config.Name)
                ASyncCollection.UpdateCount((int)e.Count);
        }


        private readonly bool isCombinedViewActive = false;

        private void WareHouse_OnFramesDecoded(object sender, IReadOnlyList<IFrame> frames)
        {
            ((AsyncObservableCollection<FramePattern>)resultCollection).AddRange(frames.OfType<FramePattern>());

            foreach (var frame in frames.OfType<FramePattern>())
            {
                if (SessionConfiguration.Triggerset && trigger.TriggerType == eTriggerTypeList.Protocol && Config == trigger.ProtocolSelection)
                {
                    if ((SessionConfiguration.TriggerTime >= frame.StartTime && SessionConfiguration.TriggerTime <= frame.PacketCollection.LastOrDefault().StopTime) || ((SessionConfiguration.TriggerTime - (2 / frame.Frequency)) >= frame.StartTime && (SessionConfiguration.TriggerTime - (2 / frame.Frequency)) <= frame.PacketCollection.LastOrDefault().StopTime))
                    {
                        wareHouse.TriggerPacket = frame.FrameIndex;
                        SessionConfiguration.Triggerset = false;
                    }
                }
            }
        }

        private int selectedIndex;

        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                selectedIndex = value;
                RaisePropertyChanged(nameof(SelectedIndex));
            }
        }

        private AsyncVirtualizingCollection<I3CListModel> aSyncCollection;
        public AsyncVirtualizingCollection<I3CListModel> ASyncCollection
        {
            get { return aSyncCollection; }
            set
            {
                aSyncCollection = value;

                RaisePropertyChanged("ASyncCollection");
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
        private void FetchRange(IList<I3CListModel> arg1, int startIndex, int count)
        {
            IsLoading = true;

            try
            {
                var list = GetPackets(startIndex, count);

                for (int i = 0; i < arg1.Count; i++)
                {
                    if (i >= list.Count) break;

                    arg1[i].Frame = list[i];
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private List<IFrame> GetPackets(int startIndex, int count)
        {
            return dataProvider.RequestFrames(Config.Name, startIndex, count);
        }

        #region Markers

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
            SelectedFrame = ResultCollection[WareHouse.TriggerPacket];
        }


        #endregion

        public void Reset()
        {
            WareHouse.TriggerTime = 0;
            SelectedIndex = 0;
            SelectedFrame = null;
        }

        public ICommand selectionCommand;
        public ICommand SelectionCommand
        {
            get
            {
                return selectionCommand ?? (selectionCommand ?? new RelayCommand((o) => OnSelectionChanged?.Invoke(this)));
            }
        }

        private ICommand frameselectionCommand;
        public ICommand FrameSelectionCommand
        {
            get
            {
                return frameselectionCommand;
            }
        }

        public ConfigViewModel_I3C Config { get; private set; }

        public FramePattern SelectedFrame
        {
            get
            {

                return wareHouse.SelectedFrame;
            }
            set
            {

                wareHouse.SelectedFrame = value;
                RaisePropertyChanged("SelectedFrame");

                if (wareHouse.SelectedFrame != null)
                {
                    //resultCollection = (IList<FramePattern>)ASyncCollection.GetDownloadedResults();
                   
                    SearchParameter.SelectedFrame = value;
                    OnSelectionChanged?.Invoke(this);
                }
            }
        }

        ResultModel_I3C wareHouse;
        private readonly TriggerViewModel trigger;

        public ResultModel_I3C WareHouse
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

        private IList<FramePattern> resultCollection;
        public IList<FramePattern> ResultCollection
        {
            get => resultCollection;
            set
            {
                resultCollection = value;
                RaisePropertyChanged(nameof(ResultCollection));
            }
        }

        double TimeStampConverter(double actualTime)
        {
            return (actualTime - SessionConfiguration.TriggerTime);
        }
        public void Serach()
        {
            throw new NotImplementedException();
        }

        public SearchFilterViewModel_I3C SearchParameter { get; }

        private void SearchParameter_SelectionChanged(IFrame frame)
        {
            this.SelectedFrame = (FramePattern)frame;
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

        #region IResultViewModel implementation

        public event Action<IResultViewModel> OnSelectionChanged;

        IConfigViewModel IResultViewModel.Config => Config;

        int IResultViewModel.TriggerPacketIndex => wareHouse.TriggerPacket;

        IFrame IResultViewModel.SelectedFrame
        {
            get { return SelectedFrame; }
            set
            {
                SelectedFrame = (FramePattern)value;
            }
        }

        ICollection<IFrame> IResultViewModel.ResultCollection => resultCollection.OfType<IFrame>().ToList();

        List<ChannelInfo> IResultViewModel.AvailableBusChannels => new List<ChannelInfo> { new ChannelInfo(Config.Name, ((IResultViewModel)this).Config.Channels[1].ChannelIndex) };

        public IReadOnlyList<I3CListModel> DownloadedItems { get; set; }

        IEnumerable<IBusData> IResultViewModel.GetBusDiagram(ChannelInfo channel, double startTime, double stopTime)
        {
            // TODO: Why this variable is used?
            string emptyVal = "  .";

            //// Start is more then starttime / Stop is less then stoptime - full frame visible
            //// start is less then starttime / stop is more then stoptime - frame stop is visible
            //// start is more then starttime / stop is more then stoptime - frame start is visible
            //// Start is less then starttime / stop is more then stop time - frame in the middle is shown

            //// negetive statetments
            //// start and stop more then starttime and stoptime
            //// start and stop is less then starttime and stoptime
            //IEnumerable<FramePattern> filteredFrames = resultCollection
            //    .Where(r => ((r.StopTime < startTime) || (r.StartTime > stopTime)) == false);

            // var downloadedResults = DownloadedItems;
            var downloadedResults = dataProvider.RequestFrames(Config.Name, startTime, stopTime);

            if (downloadedResults == null || downloadedResults.Count == 0)
                yield break;

            var resultList = downloadedResults.OfType<FramePattern>().SkipWhile(r => r.StopTime < startTime).TakeWhile(r => r.StartTime <= stopTime);

            foreach (var frame in resultList)
            {
                foreach (var packet in frame.PacketCollection.TakeWhile(p => p.TimeStamp < stopTime))
                {
                    double StartTimestamp = TimeStampConverter(packet.TimeStamp);
                    double StopTimestamp = TimeStampConverter(packet.StopTime);
                    switch (packet.PacketType)
                    {
                        case ePacketType.Command:
                            {
                                var bus = new PolygonBusData();
                                bus.StartIndex = StartTimestamp * 1e9;
                                bus.StopIndex = (StartTimestamp + 8 / packet.Frequency) * 1e9;
                                bus.Data = ProtocolInfoRepository.GetCommand((packet as CommandMessageModel).Value).ToString() + emptyVal;
                                bus.Brush = System.Windows.Media.Brushes.YellowGreen;
                                yield return bus;


                                bus = new PolygonBusData();
                                bus.StartIndex = (StartTimestamp + 8 / packet.Frequency) * 1e9;
                                bus.StopIndex = (StartTimestamp + 9 / packet.Frequency) * 1e9;
                                bus.Data = "T" + emptyVal;
                                bus.Brush = System.Windows.Media.Brushes.DarkOrange;
                                yield return bus;

                                if (packet.Stop)
                                {
                                    bus = new PolygonBusData();
                                    bus.StartIndex = (StartTimestamp + 9 / packet.Frequency) * 1e9;
                                    bus.StopIndex = StopTimestamp * 1e9;
                                    bus.Data = "P" + emptyVal;
                                    bus.Brush = System.Windows.Media.Brushes.IndianRed;
                                    yield return bus;
                                }
                                break;
                            }
                        case ePacketType.Address:
                            {
                                var bus = new PolygonBusData();
                                bus.StartIndex = StartTimestamp * 1e9;
                                bus.StopIndex = (StartTimestamp + 1 / packet.Frequency) * 1e9;
                                bus.Data = (packet as AddressMessageModel).Start.ToString() + emptyVal;
                                bus.Brush = System.Windows.Media.Brushes.HotPink;
                                yield return bus;

                                bus = new PolygonBusData();
                                bus.StartIndex = (StartTimestamp + 1 / packet.Frequency) * 1e9;
                                bus.StopIndex = (StartTimestamp + 8 / packet.Frequency) * 1e9;
                                bus.Data = "Addr=" + "0x" + (packet as AddressMessageModel).Value.ToString("X2") + emptyVal;
                                bus.Brush = System.Windows.Media.Brushes.DarkCyan;
                                yield return bus;

                                bus = new PolygonBusData();
                                bus.StartIndex = (StartTimestamp + 8 / packet.Frequency) * 1e9;
                                bus.StopIndex = (StartTimestamp + 9 / packet.Frequency) * 1e9;
                                bus.Data = ((packet as AddressMessageModel).TransferType == eTransferType.RD ? "R" : "W") + emptyVal;
                                bus.Brush = System.Windows.Media.Brushes.DarkSeaGreen;
                                yield return bus;


                                bus = new PolygonBusData();
                                bus.StartIndex = (StartTimestamp + 9 / packet.Frequency) * 1e9;
                                bus.StopIndex = (StartTimestamp + 10 / packet.Frequency) * 1e9;
                                bus.Data = ((packet as AddressMessageModel).AckType == eAcknowledgeType.ACK ? "A" : "N") + emptyVal;
                                bus.Brush = System.Windows.Media.Brushes.DarkViolet;
                                yield return bus;

                                if (packet.Stop)
                                {
                                    bus = new PolygonBusData();
                                    bus.StartIndex = (StartTimestamp + 10 / packet.Frequency) * 1e9;
                                    bus.StopIndex = StopTimestamp * 1e9;
                                    bus.Data = "P" + emptyVal;
                                    bus.Brush = System.Windows.Media.Brushes.IndianRed;
                                    yield return bus;
                                }
                                break;
                            }
                        case ePacketType.Data:
                            {
                                string displayVal = "";
                                int noOfBits = 8;
                                if (packet is PIDDAAMessageModel)
                                {
                                    displayVal = "PID=" + "0x" + (packet as PIDDAAMessageModel).Value.ToString("X2");
                                    noOfBits = 48;
                                }
                                else if (packet is DataMessageModel)
                                {
                                    switch ((packet as DataMessageModel).Description)
                                    {
                                        case eDataDescription.Data:
                                            if (frame.FrameType == eMajorFrame.Directed_SETDASA || frame.FrameType == eMajorFrame.Directed_SETNEWDA)
                                            {
                                                noOfBits = 7;
                                                displayVal = "Dynamic Addr=" + "0x" + ((packet as DataMessageModel).Value >> 1).ToString("X2");
                                            }
                                            else
                                                displayVal = "Data=" + "0x" + (packet as DataMessageModel).Value.ToString("X2");
                                            break;
                                        case eDataDescription.DAADynamicAddress:
                                            displayVal = "Dynamic Addr=" + "0x" + ((packet as DataMessageModel).Value >> 1).ToString("X2");
                                            break;
                                        case eDataDescription.DCR:
                                        case eDataDescription.DCRValue:
                                            displayVal = "DCR=" + "0x" + (packet as DataMessageModel).Value.ToString("X2");
                                            break;
                                        case eDataDescription.BCR:
                                        case eDataDescription.BCRValue:
                                            displayVal = "BCR=" + "0x" + (packet as DataMessageModel).Value.ToString("X2");
                                            break;
                                        default:
                                            displayVal = "Data=" + "0x" + (packet as DataMessageModel).Value.ToString("X2");
                                            break;

                                    }
                                }

                                var bus = new PolygonBusData();
                                bus.StartIndex = StartTimestamp * 1e9;
                                bus.StopIndex = (StartTimestamp + noOfBits / packet.Frequency) * 1e9;
                                bus.Data = displayVal + emptyVal;
                                bus.Brush = System.Windows.Media.Brushes.RoyalBlue;
                                yield return bus;

                                if (packet is DataMessageModel)
                                {
                                    var desc = (packet as DataMessageModel).Description;
                                    if (!(desc == eDataDescription.BCR || desc == eDataDescription.DCR))
                                    {
                                        bus = new PolygonBusData();
                                        bus.StartIndex = (StartTimestamp + noOfBits / packet.Frequency) * 1e9;
                                        bus.StopIndex = (StartTimestamp + (noOfBits + 1) / packet.Frequency) * 1e9;
                                        if (desc == eDataDescription.DAADynamicAddress)
                                        {
                                            if (((packet as DataMessageModel).TransmitBit & 0x1) == 0)
                                            {
                                                bus.Data = "A" + emptyVal;
                                            }
                                            else
                                            {
                                                bus.Data = "N" + emptyVal;
                                            }
                                        }
                                        else
                                            bus.Data = "T" + emptyVal;
                                        bus.Brush = System.Windows.Media.Brushes.DarkOrange;
                                        yield return bus;

                                        if (packet.Stop)
                                        {
                                            bus = new PolygonBusData();
                                            bus.StartIndex = (StartTimestamp + (noOfBits + 1) / packet.Frequency) * 1e9;
                                            bus.StopIndex = StopTimestamp * 1e9;
                                            bus.Data = "P" + emptyVal;
                                            bus.Brush = System.Windows.Media.Brushes.IndianRed;
                                            yield return bus;
                                        }
                                    }
                                }
                                break;
                            }
                        case ePacketType.HDR_Command:
                            {
                                var bus = new PolygonBusData();
                                bus.StartIndex = StartTimestamp * 1e9;
                                bus.StopIndex = StopTimestamp * 1e9;
                                bus.Data = "HDR_Cmd =" + "0x" + (packet as HDRMessageModel).Value.ToString("X5") + emptyVal;
                                bus.Brush = System.Windows.Media.Brushes.Pink;
                                yield return bus;

                                break;
                            }
                        case ePacketType.HDR_Exit:
                            {
                                var bus = new PolygonBusData();
                                bus.StartIndex = StartTimestamp * 1e9;
                                bus.StopIndex = StopTimestamp * 1e9;
                                bus.Data = "HDR_Exit" + emptyVal;
                                bus.Brush = System.Windows.Media.Brushes.IndianRed;
                                yield return bus;
                                break;
                            }
                        case ePacketType.Slave_Reset:
                            {
                                var bus = new PolygonBusData();
                                bus.StartIndex = StartTimestamp * 1e9;
                                bus.StopIndex = StopTimestamp * 1e9;
                                bus.Data = "Slave_Reset" + emptyVal;
                                bus.Brush = System.Windows.Media.Brushes.PaleVioletRed;
                                yield return bus;
                                break;
                            }
                        case ePacketType.HDR_Restart:
                            {
                                var bus = new PolygonBusData();
                                bus.StartIndex = StartTimestamp * 1e9;
                                bus.StopIndex = StopTimestamp * 1e9;
                                bus.Data = "HDR_Restart" + emptyVal;
                                bus.Brush = System.Windows.Media.Brushes.PaleVioletRed;
                                yield return bus;
                                break;
                            }
                        case ePacketType.HDR_CRC:
                            {
                                var bus = new PolygonBusData();
                                bus.StartIndex = StartTimestamp * 1e9;
                                bus.StopIndex = StopTimestamp * 1e9;
                                bus.Data = "HDR_CRC =" + "0x" + (packet as HDRMessageModel).Value.ToString("X") + emptyVal;
                                bus.Brush = System.Windows.Media.Brushes.LightCoral;
                                yield return bus;
                                break;
                            }
                        case ePacketType.HDR_Data:
                            {
                                var bus = new PolygonBusData();
                                bus.StartIndex = StartTimestamp * 1e9;
                                bus.StopIndex = StopTimestamp * 1e9;
                                bus.Data = "HDR_Data =" + "0x" + (packet as HDRMessageModel).Value.ToString("X5") + emptyVal;
                                bus.Brush = System.Windows.Media.Brushes.Cyan;
                                yield return bus;
                                break;
                            }
                    }
                }
            }
        }

        #endregion
    }

    public class SearchFilterViewModel_I3C : SearchFilterViewModelBase<IFrame>
    {
        public SearchFilterViewModel_I3C(CollectionViewSource source):base(source) { }

        public SearchFilterViewModel_I3C(IList collection) : base(collection)
        {
        }

        private eMajorFrame commandType;
        public eMajorFrame CommandType
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

        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged(nameof(Address));
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

        public override bool IsMatch(IFrame frame)
        {
            if ((frame is FramePattern framePattern) == false)
                return false;

            //I3C Data

            List<string> DataList = new List<string>();
            if (data != null)
            {
                if (CheckHexa(data.ToLower()))
                {
                    foreach (var db in framePattern.PacketCollection)
                    {
                        if (db is DataMessageModel dataMessage && dataMessage != null)
                        {
                            DataList.Add($"0x{Convert.ToInt32(dataMessage.Value):X}");
                        }

                    }
                }
                else
                {
                    foreach (var db in framePattern.PacketCollection)
                    {
                        if (db is DataMessageModel dataMessage && dataMessage != null)
                        {
                            DataList.Add($"{Convert.ToInt32(dataMessage.Value):X}");
                        }

                    }
                }
            }
            List<string> Data = new List<string>();
            foreach (var item in DataList)
            {
                var num = item.ToLower();
                Data.Add(num);
            }

            //I3C Address
            List<string> AddressList = new List<string>();
            if (address != null)
            {
                if (CheckHexa(address.ToLower()))
                {
                    foreach (var db in framePattern.PacketCollection)
                    {
                        if (db is AddressMessageModel addressMessage && addressMessage != null)
                        {
                            AddressList.Add($"0x{Convert.ToInt32(addressMessage.Value):X}");
                        }

                    }
                }
                else
                {
                    foreach (var db in framePattern.PacketCollection)
                    {
                        if (db is AddressMessageModel addressMessage && addressMessage != null)
                        {
                            AddressList.Add($"{Convert.ToInt32(addressMessage.Value):X}");
                        }

                    }
                }
            }
            List<string> Address = new List<string>();
            foreach (var item in AddressList)
            {
                var num = item.ToLower();
                Address.Add(num);
            }

            bool result = true;
            bool searchValid = false;


       
            // I3C Command
            if (commandType != eMajorFrame.NA)
            {
                result &= framePattern.FrameType == commandType;
                searchValid = true;
            }
            else
            if(commandType == eMajorFrame.NA) 
            {
                searchValid= true;
            }

            if (!string.IsNullOrWhiteSpace(address))
            {
                result &= Address.Contains(address);

                searchValid = true;
            }


            if (!string.IsNullOrWhiteSpace(data))
            {
                result &= Data.Contains(data);

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

