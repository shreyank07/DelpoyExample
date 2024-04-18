using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using ProdigyFramework.Behavior;
using ProdigyFramework.Collections;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using System.ComponentModel;
using System.Windows.Data;
using Prodigy.WaveformControls.Interfaces;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.I3CStructure;
using System.Xml.Linq;
using System.Collections;
using PGYMiniCooper.DataModule.Helpers;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace PGYMiniCooper.CoreModule.ViewModel.ProtocolViewModel
{
    public class ResultViewModel_I2C : ViewModelBase, IResultViewModel
    {
        private readonly IDataProvider dataReceiver;

        public ResultViewModel_I2C(ConfigViewModel_I2C config, ResultModel_I2C model,TriggerViewModel trigger)
        {
            this.Config = config;
            this.wareHouse = model;
            this.trigger = trigger;
            wareHouse.Reset();
            dataReceiver = Ioc.Default.GetService<IDataProvider>();
            frameCollection = new ProdigyFramework.Collections.AsyncObservableCollection<I2CFrame>();
            wareHouse.OnFramesDecoded += WareHouse_OnFramesDecoded;

            SearchParameter = new SearchFilterViewModel_I2C((IList)frameCollection);
            SearchParameter.OnSelectionChanged += SearchParameter_OnSelectionChanged;
        }


        private readonly TriggerViewModel trigger;
        private void WareHouse_OnFramesDecoded(object sender, IReadOnlyList<IFrame> frames)
        {
            ((AsyncObservableCollection<I2CFrame>)frameCollection).AddRange(frames.OfType<I2CFrame>());
       
            foreach(var frame in frames.OfType<I2CFrame>())
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

        #region Markers



        //datarowheader content trigger
        private int? _EventMarker1;
        public int? EventMarker1
        {
            get { return _EventMarker1; }
            set
            {
                _EventMarker1 = value;
                RaisePropertyChanged("EventMarker1");
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

     public   void gotoTriggerMethod()
        {
            SelectedFrame = FrameCollection[WareHouse.TriggerPacket];
        }


        #endregion

        public void Reset()
        {
            WareHouse.TriggerTime = 0;
            WareHouse.SelectedFrame = null;
        }

        public ICommand selectionCommand;
        public ICommand SelectionCommand
        {
            get
            {
                return selectionCommand ?? (selectionCommand = new RelayCommand((o) => OnSelectionChanged?.Invoke(this)));
            }
        }

        public Command childGridSelectionCommand;
        public Command ChildGridSelectionCommand
        {
            get
            {
                return childGridSelectionCommand;
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

        ResultModel_I2C wareHouse;
        public ResultModel_I2C WareHouse
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

        public ConfigViewModel_I2C Config { get; private set; }    

        public IList<I2CFrame> FrameCollection
        {
            get
            {
                return frameCollection;
            }
            set
            {
                frameCollection = value;
                RaisePropertyChanged(nameof(FrameCollection));
            }
        }
        IList<I2CFrame> frameCollection;

        public IFrame SelectedFrame
        {
            get
            {
                return wareHouse.SelectedFrame;
            }
            set
            {
                wareHouse.SelectedFrame = value;
     
                RaisePropertyChanged("SelectedFrame");

                if (value != null)
                {
                    SearchParameter.SelectedFrame = value;
                }
            }
        }
       
        private IFrame selectedFrame;

        #region search and filter

        private void SearchParameter_OnSelectionChanged(IFrame frame)
        {
            SelectedFrame = frame;
        }

        public SearchFilterViewModel_I2C SearchParameter { get; private set; }

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

        public void ResetMethod()
        {
            selectedFrame = null;
        }

        public void Serach()
        {
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

        private ePatternFormat patternFormat = ePatternFormat.Hex;

        public ePatternFormat PatternFormat { get { return patternFormat; } set { patternFormat = value; ChangText("pattern", value); RaisePropertyChanged("PatternFormat"); } }

        void ChangText(string type, ePatternFormat format)
        {
            string retval = "0x00";


            if (format == ePatternFormat.Decimal)

                retval = "00";
            else if (format == ePatternFormat.Hex)
                retval = "0x00";
            else
                retval = "00000000";

        }
        #endregion
        double TimeStampConverter(double actualTime)
        {
            return (actualTime - SessionConfiguration.TriggerTime);
        }

        #region IResultViewModel implementation

        public event Action<IResultViewModel> OnSelectionChanged;

        IFrame IResultViewModel.SelectedFrame
        {
            get { return SelectedFrame; }
            set
            {
                SelectedFrame = value;
            }
        }
        IConfigViewModel IResultViewModel.Config => Config;

        int IResultViewModel.TriggerPacketIndex => wareHouse.TriggerPacket;

        ICollection<IFrame> IResultViewModel.ResultCollection => frameCollection.OfType<IFrame>().ToList();

        List<ChannelInfo> IResultViewModel.AvailableBusChannels => new List<ChannelInfo> { new ChannelInfo("I2C", ((IResultViewModel)this).Config.Channels[1].ChannelIndex) };

        IEnumerable<IBusData> IResultViewModel.GetBusDiagram(ChannelInfo channel, double startTime, double stopTime)
        {
            var clock = Config.ChannelIndex_SCL;
            var data = Config.ChannelIndex_SDA;

            List<I2CFrame> resultList = frameCollection.SkipWhile(r => r.StopTime < startTime).TakeWhile(r => r.StartTime <= stopTime).ToList();

            var bufferList = dataReceiver.GetWaveformBufferTimestamp(startTime, stopTime);

            DigitalWaveformEdgeFinding edgeFinding = new DigitalWaveformEdgeFinding(bufferList);

            string emptyVal = "  .";
            foreach (var frame in resultList)
            {
                var currentPacket = frame;
                double StartTimestamp = currentPacket.StartTime;
                double StopTimestamp = currentPacket.StopTime;

                var wfmIndex = bufferList.FindIndex(p => p.TimeStamp == StartTimestamp);

                //Start
                if (wfmIndex < 0)
                    continue;
                if (wfmIndex != 0)
                    wfmIndex--;

                if (bufferList[wfmIndex + 1].GetChannelState(clock) == 1 && bufferList[wfmIndex + 1].GetChannelState(data) == 0)
                {
                    wfmIndex += 1;
                    StartTimestamp = bufferList[wfmIndex].TimeStamp;
                }

                var bus = new PolygonBusData();
                if (edgeFinding.GetNextRisingEdgeIndex(clock, wfmIndex, out int nextEdgeIndex))
                {
                    bus.StartIndex = TimeStampConverter(StartTimestamp) * 1e9;
                    bus.StopIndex = TimeStampConverter(bufferList[nextEdgeIndex].TimeStamp) * 1e9;
                    bus.Data = currentPacket.Start.ToString() + emptyVal;
                    bus.Brush = System.Windows.Media.Brushes.HotPink;

                    yield return bus;
                }

                wfmIndex = nextEdgeIndex;

                bus = new PolygonBusData();
                if (edgeFinding.GetNextEdgeByCountWithEdge(clock, wfmIndex, 7, eEdgeType.RISING_EDGE, out nextEdgeIndex))
                {
                    bus.StartIndex = TimeStampConverter(bufferList[wfmIndex].TimeStamp) * 1e9;
                    bus.StopIndex = TimeStampConverter(bufferList[nextEdgeIndex].TimeStamp) * 1e9;
                    bus.Data = "Addr=" + "0x" + (currentPacket.AddressFirst >> 1).ToString("X2") + emptyVal;
                    bus.Brush = System.Windows.Media.Brushes.DarkCyan;
                    yield return bus;
                }

                wfmIndex = nextEdgeIndex;

                //ReadWrite
                bus = new PolygonBusData();
                if (edgeFinding.GetNextRisingEdgeIndex(clock, wfmIndex, out nextEdgeIndex))
                {
                    bus.StartIndex = TimeStampConverter(bufferList[wfmIndex].TimeStamp) * 1e9;
                    bus.StopIndex = TimeStampConverter(bufferList[nextEdgeIndex].TimeStamp) * 1e9;
                    bus.Data = ((currentPacket.AddressFirst & 0x1) == 0x1 ? "R" : "W") + emptyVal;
                    bus.Brush = System.Windows.Media.Brushes.Green;
                    yield return bus;
                }

                wfmIndex = nextEdgeIndex;

                //Ack/Nack
                bus = new PolygonBusData();
                if (edgeFinding.GetNextRisingEdgeIndex(clock, wfmIndex, out nextEdgeIndex))
                {
                    bus.StartIndex = TimeStampConverter(bufferList[wfmIndex].TimeStamp) * 1e9;
                    bus.StopIndex = TimeStampConverter(bufferList[nextEdgeIndex].TimeStamp) * 1e9;
                    bus.Data = (((currentPacket.AckAddr & 0x0F) == 0x0F) ? "N" : "A") + emptyVal;
                    bus.Brush = System.Windows.Media.Brushes.DarkViolet;
                    yield return bus;
                }

                wfmIndex = nextEdgeIndex;

                if (currentPacket.AddressSecond != 0)
                {
                    //Address 10 bit
                    bus = new PolygonBusData();
                    if (edgeFinding.GetNextEdgeByCountWithEdge(clock, wfmIndex, 8, eEdgeType.RISING_EDGE, out nextEdgeIndex))
                    {
                        bus.StartIndex = TimeStampConverter(bufferList[wfmIndex].TimeStamp) * 1e9;
                        bus.StopIndex = TimeStampConverter(bufferList[nextEdgeIndex].TimeStamp) * 1e9;
                        bus.Data = "Addr(2)=" + "0x" + (currentPacket.AddressSecond).ToString("X2") + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.DarkCyan;

                        yield return bus;
                    }

                    wfmIndex = nextEdgeIndex;

                    //Ack/Nack
                    bus = new PolygonBusData();
                    if (edgeFinding.GetNextRisingEdgeIndex(clock, wfmIndex, out nextEdgeIndex))
                    {
                        bus.StartIndex = TimeStampConverter(bufferList[wfmIndex].TimeStamp) * 1e9;
                        bus.StopIndex = TimeStampConverter(bufferList[nextEdgeIndex].TimeStamp) * 1e9;
                        bus.Data = (((currentPacket.AckAddr & 0x0F) == 0x0F) ? "N" : "A") + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.DarkViolet;

                        yield return bus;
                    }
                }

                wfmIndex = nextEdgeIndex;

                int index = 0;
                foreach (var dataByte in currentPacket.DataBytes)
                {
                    bus = new PolygonBusData();
                    if (edgeFinding.GetNextEdgeByCountWithEdge(clock, wfmIndex, 8, eEdgeType.RISING_EDGE, out nextEdgeIndex))
                    {
                        bus.StartIndex = TimeStampConverter(bufferList[wfmIndex].TimeStamp) * 1e9;
                        bus.StopIndex = TimeStampConverter(bufferList[nextEdgeIndex].TimeStamp) * 1e9;
                        bus.Data = "Data=" + "0x" + dataByte.ToString("X2") + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.RoyalBlue;

                        yield return bus;
                    }
                    else
                    {
                        if (dataByte == currentPacket.DataBytes.Last() && currentPacket.DataBytes.Count() > 1)
                        {
                            bus.StartIndex = TimeStampConverter(bufferList[wfmIndex].TimeStamp) * 1e9;
                            bus.StopIndex = TimeStampConverter(bufferList[nextEdgeIndex].TimeStamp) * 1e9;
                            bus.Data = "Data=" + "0x" + dataByte.ToString("X2") + emptyVal;
                            bus.Brush = System.Windows.Media.Brushes.RoyalBlue;

                            yield return bus;
                        }
                    }

                    wfmIndex = nextEdgeIndex;

                    //Ack/Nack
                    bus = new PolygonBusData();
                    if (edgeFinding.GetNextRisingEdgeIndex(clock, wfmIndex, out nextEdgeIndex))
                    {
                        bus.StartIndex = TimeStampConverter(bufferList[wfmIndex].TimeStamp) * 1e9;
                        bus.StopIndex = TimeStampConverter(bufferList[nextEdgeIndex].TimeStamp) * 1e9;
                        bus.Data = (((currentPacket.AckData[index] & 0x1) == 0) ? "A" : "N") + emptyVal;
                        bus.Brush = System.Windows.Media.Brushes.DarkViolet;
                        yield return bus;
                    }

                    wfmIndex = nextEdgeIndex;

                    index++;
                }

                //Stop
                if (currentPacket.Stop == eStartStop.P)
                {
                    if (edgeFinding.GetNextRisingEdgeIndex(data, wfmIndex, out nextEdgeIndex))
                        StopTimestamp = bufferList[nextEdgeIndex].TimeStamp;

                    bus = new PolygonBusData();
                    bus.StartIndex = TimeStampConverter(bufferList[wfmIndex].TimeStamp) * 1e9;
                    bus.StopIndex = TimeStampConverter(StopTimestamp) * 1e9;
                    bus.Data = "P" + emptyVal;
                    bus.Brush = System.Windows.Media.Brushes.HotPink;
                    yield return bus;
                }
            }
        }

        #endregion
    }

    public class SearchFilterViewModel_I2C : SearchFilterViewModelBase<IFrame>
    {
        public SearchFilterViewModel_I2C(IList collection) : base(collection)
        {
        }

        private bool startSelected=true;
        public bool StartSelected
        {
            get { return startSelected; }
            set
            {
                startSelected = value;
                OnPropertyChanged(nameof(StartSelected));
            }
        }

        private bool repeatStartSelected;
        public bool RepeatStartSelected
        {
            get { return repeatStartSelected; }
            set
            {
                repeatStartSelected = value;
                OnPropertyChanged(nameof(RepeatStartSelected));
            }
        }

        private bool stopSelected;
        public bool StopSelected
        {
            get { return stopSelected; }
            set
            {
                stopSelected = value;
                OnPropertyChanged(nameof(StopSelected));
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

        private string ackAdd = "Any";

        public string ACKAdd
        {
            get { return ackAdd; }
            set
            {
                ackAdd = value;
                OnPropertyChanged(nameof(ACKAdd));
            }
        }

        private string transfer="Any";

        public string Transfertype
        {
            get { return  transfer; }
            set
            {
                transfer = value;
                OnPropertyChanged(nameof(Transfertype));
            }
        }

        private string ackData = "Any";

        public string ACKData
        {
            get { return ackData; }
            set
            {
                ackData = value;
                OnPropertyChanged(nameof(ACKData));
            }
        }

        private AddressHelper addressHelper = new AddressHelper();

        public override bool IsMatch(IFrame frame)
        {
            var i2cFrame = (I2CFrame)frame;

            var addressResult = addressHelper.ConvertAddress(new object[]
            {
                i2cFrame.AddressFirst,
                i2cFrame.AddressSecond
            });
            string address = $"0x{Convert.ToInt32(addressResult):X}";
            string address1 = $"{Convert.ToInt32(addressResult):X}";
            List<string> datStr = new List<string>();
            if (Data != null)
            {
                if (CheckHexa(Data.ToLower()))
                {
                    foreach (var db in i2cFrame.DataBytes)
                    {

                        datStr.Add($"0x{Convert.ToInt32(db):X}");
                    }
                }
                else
                {
                    foreach (var db in i2cFrame.DataBytes)
                    {
                        datStr.Add($"{Convert.ToInt32(db):X}");
                    }

                }
            }

            //List<string> datStr1 = new List<string>();


            //    datStr1.Add($"{Convert.ToInt32(db):X}");
            //   // dataack = (AckData[index] & 0x1) == 0 ? "ACK" : "NACK";
            //}

            string Transfer = addressHelper.ConvertAddressAcK(i2cFrame.AddressFirst, 2);
            string ackAddr = addressHelper.ConvertAddressAcK(i2cFrame.AddressFirst, 3);


            int index = 0;
            List<string> name = new List<string>();
            string dataack = "";
            foreach (var item in i2cFrame.AckData)

            {


                name.Add((i2cFrame.AckData[index] & 0x1) == 0 ? "ACK" : "NACK");
                index++;


            }

            List<string> NewData = new List<string>();
            foreach (var item in datStr)
            {
                var num = item.ToLower();
                NewData.Add(num);

            }

            bool result = true;


            bool searchValid = false;


            string addack = " ";
            addack = (i2cFrame.AckAddr & 0x0F) == 0x0F ? "NACK" : "ACK";

            if (!string.IsNullOrWhiteSpace(Address))
            {
                result &= (address.ToLower().Equals(Address.ToLower()) || address1.ToLower().Equals(Address.ToLower()));
                searchValid = true;
            }
            var item1 = -1;

            if (!string.IsNullOrWhiteSpace(Data))
            {
                result &= NewData.Contains(Data.ToLower());
                item1 = datStr.IndexOf(Data.ToLower());
                searchValid = true;
            }

            if (StartSelected)
            {
                result &= i2cFrame.Start.ToString().Equals("S");
                searchValid = true;
            }
            if (RepeatStartSelected)
            {
                result &= i2cFrame.Start.ToString().Equals("Sr");
                searchValid = true;
            }
            if (StopSelected)
            {
                result &= i2cFrame.Stop.ToString().Equals("P");
                searchValid = true;
            }
            if (!string.IsNullOrWhiteSpace(ACKAdd) && ACKAdd != "Any")
            {
                result &= addack.ToString().Equals(ACKAdd);
                searchValid = true;
            }
            if (!string.IsNullOrWhiteSpace(Transfer) && Transfertype != "Any" )
            {
                result &= Transfer.ToString().Equals(Transfertype);
                searchValid = true;
            }
            if (!string.IsNullOrWhiteSpace(ACKData) && ACKData != "Any")
            {

                if (item1 != -1)
                {
                    result &= ACKData.Equals(name[item1]);
                    searchValid = true;

                }

            }
            return result && searchValid;
        }

        public bool CheckHexa(string data)
        {
            return data.Contains("x");
        }
    }
}
