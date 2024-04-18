using CommunityToolkit.Mvvm.DependencyInjection;
using PGYMiniCooper.CoreModule.ViewModel;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Helpers;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using PGYMiniCooper.DataModule.Structure.I3CStructure;
using PGYMiniCooper.DataModule.Structure.QSPIStructure;
using PGYMiniCooper.DataModule.Structure.SPIStructure;
using Prodigy.WaveformControls.Interfaces;
using ProdigyFramework.Behavior;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace PGYMiniCooper.CoreModule.ViewModel.ProtocolViewModel
{
    public class ResultViewModel_QSPI : ViewModelBase, IResultViewModel
    {
        public ResultViewModel_QSPI(ConfigViewModel_QSPI config, ResultModel_QSPI model, TriggerViewModel trigger)
        {
         this.Config = config;
         this.WareHouse = model;
         this.trigger = trigger;
         wareHouse.Reset();
         qSPIFrameCollection = new ProdigyFramework.Collections.AsyncObservableCollection<QSPIFrame>();
         wareHouse.OnFramesDecoded += WareHouse_OnFramesDecoded;
         SearchParameter = new SearchFilterViewModel_QSPI((IList)qSPIFrameCollection);
         SearchParameter.OnSelectionChanged += SearchParameter_SelectionChanged;


        }

        private void WareHouse_OnFramesDecoded(object sender,IReadOnlyList<IFrame> frames)
        {
            var qspiFrames = frames.OfType<QSPIFrame>();

            ((ProdigyFramework.Collections.AsyncObservableCollection<QSPIFrame>)qSPIFrameCollection).AddRange(qspiFrames);

            foreach (var frame in qspiFrames)
            {
                if (SessionConfiguration.Triggerset && trigger.TriggerType == eTriggerTypeList.Protocol && Config == trigger.ProtocolSelection)
                {
                    if (SessionConfiguration.TriggerTime >= frame.StartTime && SessionConfiguration.TriggerTime <= frame.PacketCollection.LastOrDefault().StopTime)
                    {
                        wareHouse.TriggerPacket = frame.FrameIndex;
                        SessionConfiguration.Triggerset = false;
                    }
                }
            }
        }

        private readonly TriggerViewModel trigger;

        ResultModel_QSPI wareHouse;
        public ResultModel_QSPI WareHouse
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


        public QSPIFrame SelectedFrame
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
                    SearchParameter.SelectedFrame = value;
                    OnSelectionChanged?.Invoke(this);
                }
                RaisePropertyChanged("SelectedFrame");
            }
        }

        public IList<QSPIFrame> QSPIFrameCollection
        {
            get
            {
                return qSPIFrameCollection;
            }
            set
            {
                qSPIFrameCollection = value;
                RaisePropertyChanged(nameof(QSPIFrameCollection));
            }
        }
      private  IList<QSPIFrame> qSPIFrameCollection;


        public ICommand frameselectionCommand;
        public ICommand FrameSelectionCommand
        {
            get
            {
                return frameselectionCommand ?? (frameselectionCommand = new RelayCommand((o) => OnSelectionChanged?.Invoke(this)));
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

        public void Reset()
        {
            WareHouse.Reset();
            SelectedFrame = null;
            SearchParameter.Reset();
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

       public void gotoTriggerMethod()
        {
            SelectedFrame = QSPIFrameCollection[WareHouse.TriggerPacket];
        }



        private bool isqspiVisibility = true;
        public bool IsQspiVisibility
        {
            get
            {
                return isqspiVisibility;
            }
            set
            {
                isqspiVisibility = value;
                RaisePropertyChanged("IsQspiVisibility");
            }
        }


        private ConfigViewModel_QSPI config;
        public ConfigViewModel_QSPI Config
        {
            get { return config; }
            set
            {
                config = value;
                RaisePropertyChanged(nameof(Config));
            }
        }


        public ICommand selectionCommand;

        public ICommand SelectionCommand
        {
            get
            {
                return selectionCommand;
            }
        }

  

        private void SearchParameter_SelectionChanged(IFrame frame)
        {
            this.SelectedFrame = (QSPIFrame)frame;
        }

        public void Serach()
        {
            throw new NotImplementedException();
        }

        double TimeStampConverter(double actualTime)
        {
            return (actualTime - SessionConfiguration.TriggerTime);
        }

        public SearchFilterViewModel_QSPI SearchParameter { get; }

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
                SelectedFrame = (QSPIFrame)value;
            }
        }
        ICollection<IFrame> IResultViewModel.ResultCollection => wareHouse.QSPIFrameCollection.OfType<IFrame>().ToList();

        List<ChannelInfo> IResultViewModel.AvailableBusChannels => new List<ChannelInfo> { new ChannelInfo("QSPI", ((IResultViewModel)this).Config.Channels.Last(c => c.ChannelIndex != eChannles.None).ChannelIndex) };

        IEnumerable<IBusData> IResultViewModel.GetBusDiagram(ChannelInfo channel, double startTime, double stopTime)
        {
            var bufferlist = GetWavepoints(startTime, stopTime).ToList();

            var Data = "";
            string emptyVal = "  .";
            eQSPICommands frame = eQSPICommands.Unknown;
            var resultList = QSPIFrameCollection.SkipWhile(r => r.StopTime < startTime).TakeWhile(r => r.StartTime <= stopTime).ToList();
           
                foreach (var currentFrame in resultList)
            {

                frame = currentFrame.QSPICommandType;
                int packets = currentFrame.PacketCollection.Count;

                foreach (var currentPacket in currentFrame.PacketCollection)
                {
                    double StartTimestamp = TimeStampConverter(currentPacket.TimeStamp);
                    double StopTimestamp = TimeStampConverter(currentPacket.StopTime);

                    double DummyStoptime = TimeStampConverter(currentFrame.DummyStopTime);



                    switch (currentPacket.PacketType)
                    {
                        case ePacketType.Command:
                            {
                                var annotationStartIndex = bufferlist.FindIndex(obj => obj.TimeStamp == StartTimestamp);
                                {
                                    var bus = new PolygonBusData();
                                    bus.StartIndex = StartTimestamp * 1e9;
                                    bus.StopIndex = StopTimestamp * 1e9;
                                    StartTimestamp = StopTimestamp;
                                    bus.Data = "Command  = " + ProtocolInfoRepository.GetQSPICommandInfo((currentPacket as CommandMessageModel).PacketValue).QSPICommands.ToString() + emptyVal;
                                    bus.Brush = System.Windows.Media.Brushes.IndianRed;
                                    Data = ProtocolInfoRepository.GetQSPICommandInfo((currentPacket as CommandMessageModel).PacketValue).QSPICommands.ToString();

                                    yield return bus;
                                }
                                break;
                            }

                        case ePacketType.Address:
                            {
                                //StopTimestamp = TimeStampConverter(currentPacket.StopTime);
                                //var annotationStartIndex = bufferlist.FindIndex(obj => obj.TimeStamp == StartTimestamp);
                                {
                                    var bus = new PolygonBusData();
                                    bus.StartIndex = StartTimestamp * 1e9;
                                    bus.StopIndex = StopTimestamp * 1e9;
                                    //StartTimestamp = StopTimestamp;
                                    bus.Data = "Address = " + "0x" + (currentPacket as AddressMessageModel).PacketValue.ToString("X2") + emptyVal;
                                    bus.Brush = System.Windows.Media.Brushes.DarkGreen;

                                    yield return bus;
                                }




                                if (Data.ToString().ToUpper().Contains("READ"))
                                {
                                    StartTimestamp = StopTimestamp;
                                    //StartTimestamp = DummyStoptime;
                                    var bus = new PolygonBusData();
                                    bus.StartIndex = StartTimestamp * 1e9;
                                    bus.StopIndex = DummyStoptime * 1e9;
                                    bus.Data = "DummyCycle";
                                    bus.Brush = System.Windows.Media.Brushes.BlueViolet;

                                    yield return bus;
                                }
                                break;
                            }

                        case ePacketType.Data:
                            {
                                if (currentPacket is DataMessageModel)
                                {
                                    string displayVal = "";
                                    //int noOfBits = 8;
                                    {
                                        displayVal = "Data = " + "0x" + (currentPacket as DataMessageModel).PacketValue.ToString("X2") + emptyVal;
                                    }
                                    StartTimestamp = TimeStampConverter(currentPacket.TimeStamp);
                                    StopTimestamp = TimeStampConverter(currentPacket.StopTime);

                                    var bus = new PolygonBusData();
                                    bus.StartIndex = StartTimestamp * 1e9;
                                    bus.StopIndex = StopTimestamp * 1e9;
                                    bus.Data = displayVal;
                                    bus.Brush = System.Windows.Media.Brushes.RoyalBlue;

                                    yield return bus;
                                }

                                break;
                            }
                    }
                }
            }
        }

        #endregion
    }


    public class SearchFilterViewModel_QSPI : SearchFilterViewModelBase<IFrame>
    {
        public SearchFilterViewModel_QSPI(CollectionViewSource source) : base(source) { }

        public SearchFilterViewModel_QSPI(IList collection) : base(collection)
        {
        }

        private eQSPICommands commandType;
        public eQSPICommands CommandType
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
            //if ((frame is QSPIFrame framePattern) == false)
            //    return false;
            var qSPIFrame = (QSPIFrame)frame;
            //I3C Data

            List<string> DataList = new List<string>();
            if (data != null)
            {
                if (CheckHexa(data.ToLower()))
                {
                    foreach (var db in qSPIFrame.PacketCollection)
                    {
                        if (db is DataMessageModel dataMessage && dataMessage.PacketValue != null)
                        {
                            DataList.Add($"0x{Convert.ToInt32(dataMessage.PacketValue):X}");
                        }

                    }
                }
                else
                {
                    foreach (var db in qSPIFrame.PacketCollection)
                    {
                        if (db is DataMessageModel dataMessage && dataMessage.PacketValue != null)
                        {
                            DataList.Add($"{Convert.ToInt32(dataMessage.PacketValue):X}");
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
                    foreach (var db in qSPIFrame.PacketCollection)
                    {
                        if (db is AddressMessageModel addressMessage && addressMessage.PacketValue != null)
                        {
                            AddressList.Add($"0x{Convert.ToInt32(addressMessage.PacketValue):X}");
                        }

                    }
                }
                else
                {
                    foreach (var db in qSPIFrame.PacketCollection)
                    {
                        if (db is AddressMessageModel addressMessage && addressMessage.PacketValue != null)
                        {
                            AddressList.Add($"{Convert.ToInt32(addressMessage.PacketValue):X}");
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
            if (commandType != eQSPICommands.Any_Command)
            {
                result &= qSPIFrame.QSPICommandType == commandType;
                searchValid = true;
            }
            else
            if (commandType == eQSPICommands.Any_Command)
            {
                result &= true;
                searchValid = true;
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
