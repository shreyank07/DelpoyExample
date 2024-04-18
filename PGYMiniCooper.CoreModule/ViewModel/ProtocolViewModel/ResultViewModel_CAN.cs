using CommunityToolkit.Mvvm.DependencyInjection;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.CANStructure;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using PGYMiniCooper.DataModule.Structure.RFFEStructure;
using PGYMiniCooper.DataModule.Structure.SPIStructure;
using Prodigy.WaveformControls.Interfaces;
using ProdigyFramework.Behavior;
using ProdigyFramework.Collections;
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
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;
using static PGYMiniCooper.DataModule.Structure.CANStructure.CANFrame;

namespace PGYMiniCooper.CoreModule.ViewModel.ProtocolViewModel
{
    public class ResultViewModel_CAN : ViewModelBase, IResultViewModel
    {
        private readonly IDataProvider dataReceiver;
        private bool selectFrameOnce = true;
        public ResultViewModel_CAN(ConfigViewModel_CAN config, ResultModel_CAN model, TriggerViewModel trigger)
        {
            this.Config = config;
            this.wareHouse = model;
            this.trigger = trigger;
            wareHouse.Reset();

            dataReceiver = Ioc.Default.GetService<IDataProvider>();
            resultCollection = new ProdigyFramework.Collections.AsyncObservableCollection<CANFrame>();
            wareHouse.OnFramesDecoded += WareHouse_OnFramesDecoded;

            SearchParameter = new SearchFilterViewModel_CAN((IList)resultCollection);
            SearchParameter.OnSelectionChanged += SearchParameter_OnSelectionChanged;
        }


        private readonly TriggerViewModel trigger;
        private void WareHouse_OnFramesDecoded(object sender, IReadOnlyList<IFrame> frames)
        {
            ((AsyncObservableCollection<CANFrame>)resultCollection).AddRange(frames.OfType<CANFrame>());
         
                    //SelectedFrame = resultCollection.FirstOrDefault();
            
            foreach (var frame in frames.OfType<CANFrame>())
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

        ResultModel_CAN wareHouse;
        public ResultModel_CAN WareHouse
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


        public ConfigViewModel_CAN Config { get; private set; }

        private IList<CANFrame> resultCollection;

        public IList<CANFrame> ResultCollection
        {
            get { return resultCollection; }
            set
            {
                resultCollection = value;
                RaisePropertyChanged(nameof(ResultCollection));
            }
        }

        public void Reset()
        {
            WareHouse.TriggerTime = 0;
            WareHouse.TriggerPacket = -1;

            SelectedFrame = null;
            WareHouse.Reset();
        }
        #region Trigger

        //Goto Trigger position
        private Command goToTriggerPosition;
        public Command GoToTriggerPositionCommand
        {
            get
            {
                if (goToTriggerPosition == null)
                    goToTriggerPosition = new Command(new Command.ICommandOnExecute(GoTriggerPosition));
                return goToTriggerPosition;
            }
        }

        private void GoTriggerPosition()
        {
            if (WareHouse.TriggerPacket < this.WareHouse.CANFrameCollection.Count)
            {
                this.SelectedIndex = WareHouse.TriggerPacket;
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

        #endregion

        public ICommand selectionCommand;
        public ICommand SelectionCommand
        {
            get
            {
                return selectionCommand ?? (selectionCommand = new RelayCommand((o) => OnSelectionChanged?.Invoke(this)));
            }
        }


        private int selectedIndex = 0;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                RaisePropertyChanged("SelectedIndex");
            }
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

        public SearchFilterViewModel_CAN SearchParameter { get; private set; }

        private void SearchParameter_OnSelectionChanged(IFrame frame)
        {
            SelectedFrame = frame;
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
                wareHouse.SelectedFrame = (CANFrame)value;
                RaisePropertyChanged(nameof(SelectedFrame));

                if (value != null)
                {
                    SearchParameter.SelectedFrame = value;
                    OnSelectionChanged?.Invoke(this);
                }
            }
        }

        ICollection<IFrame> IResultViewModel.ResultCollection => ResultCollection.OfType<IFrame>().ToList();

        List<ChannelInfo> IResultViewModel.AvailableBusChannels => new List<ChannelInfo> { new ChannelInfo("CAN", ((IResultViewModel)this).Config.Channels[0].ChannelIndex) };

        double TimeStampConverter(double actualTime)
        {
            return (actualTime - SessionConfiguration.TriggerTime);
        }


        double Currenttime = 0;
        double timeFor1Bit;
        private IBusData Get_SOF(double startTime, out double busEndTime)
        {
            //Start
            busEndTime = startTime + timeFor1Bit;
            return GetBus(startTime, busEndTime, "S");
        }
        private IBusData Get_ID(double startTime, double stopTime, int identifier) => GetBus(startTime, stopTime, "ID =" + "0x" + identifier.ToString("X2"));
     

        private bool CheckForBitStuffError(CANFrame frame, double startTime, double stopTime)
        {

            double StuffstartTime = 0;
            if (Config.CANType == eCANType.CAN && frame.IsStandard == true)
                StuffstartTime = frame.StopTimeList[9];
           else if (Config.CANType == eCANType.CAN && frame.IsExtended == true)
                StuffstartTime = frame.StopTimeList[12];
         else   if (Config.CANType == eCANType.CANFD && frame.IsStandard == true)
                StuffstartTime = frame.StopTimeList[13];
            else if (Config.CANType == eCANType.CANFD && frame.IsExtended == true)
                StuffstartTime = frame.StopTimeList[15];
            if (frame.HasBitstufferoor && StuffstartTime < stopTime)
                return true;
            else
                return false;
        }

        private IBusData Get_RTR(double startTime, double stopTime) => GetBus(startTime, stopTime, "RTR");

        private IBusData Get_IDFLAG(double startTime, double stopTime) => GetBus(startTime, stopTime, "IDE");

        private IBusData Get_ReservedBit(double startTime, double stopTime) => GetBus(startTime, stopTime, "R0 ");
        private IBusData Get_ReservedBit1(double startTime, double stopTime) => GetBus(startTime, stopTime, "R1");
        private IBusData Get_SRR(double startTime, double stopTime) => GetBus(startTime, stopTime, "SRR");

        private IBusData Get_EXtendedID(double startTime, double stopTime, int EXtededID) => GetBus(startTime, stopTime, "EX_ID =" + "0x" + EXtededID.ToString("X2"));

        private IBusData Get_EDL(double startTime, double stopTime) => GetBus(startTime, stopTime, "EDL");

        private IBusData Get_BRS(double startTime, double stopTime) => GetBus(startTime, stopTime, "BRS");

        private IBusData Get_ESI(double startTime, double stopTime) => GetBus(startTime, stopTime, "ESI");

        private IBusData Get_DLC(double startTime, double stopTime, int DLC) => GetBus(startTime, stopTime, "DLC =" + "0x" + DLC.ToString("X2"));

        private bool CheckForBitStuffErrorAndGet_DATA(CANFrame frame, double startTime, List<double> stopTime, List<byte> Data, out List<IBusData> buses)
        {
            buses = new List<IBusData>();

            double endTime = startTime;

            for (int i = 0; i < Data.Count; i++)
            {
                if (CheckForBitStuffError(frame, endTime, stopTime[i]))
                    return true;

                buses.Add(GetBus(endTime, stopTime[i], "Data =" + "0x" + Data[i].ToString("X2")));

                endTime = stopTime[i];
            }

            return false;
        }

        private IBusData Get_CRC(double startTime, double stopTime, int CRC) => GetBus(startTime, stopTime, "CRC =" + "0x" + CRC.ToString("X2"));

        private IBusData Get_CRCDel(double startTime, double stopTime) => GetBus(startTime, stopTime, "CRCDel");

        private IBusData Get_ACK(double startTime, double stopTime) => GetBus(startTime, stopTime, "ACK");

        private IBusData Get_ACKDel(double startTime, double stopTime) => GetBus(startTime, stopTime, "ACKdel");

        private IBusData GetBus(double startTime, double stopTime, string content)
        {
            var bus = new PolygonBusData();
            bus.StartIndex = TimeStampConverter(startTime) * 1e9;
            bus.StopIndex = TimeStampConverter(stopTime) * 1e9;
            bus.Data = content;
            bus.Brush = System.Windows.Media.Brushes.RoyalBlue;
            return bus;
        }

        private IBusData Get_EOF(double startTime, int EOF, out double busEndTime)
        {
            if (EOF == 0)
                busEndTime = startTime + (timeFor1Bit * 1);
            else if (EOF == 2)
                busEndTime = startTime + (timeFor1Bit * 2);
            else if (EOF == 6)
                busEndTime = startTime + (timeFor1Bit * 3);
            else if (EOF == 14)
                busEndTime = startTime + (timeFor1Bit * 4);
            else if (EOF == 30)
                busEndTime = startTime + (timeFor1Bit * 5);
            else if (EOF == 62)
                busEndTime = startTime + (timeFor1Bit * 6);
            else if (EOF == 63)
                busEndTime = startTime + (timeFor1Bit * 7);
            else
                busEndTime = startTime + (timeFor1Bit * 7);
            return GetBus(startTime, busEndTime, "EOF");
        }
        private IBusData Get_IFS(double startTime, int IFS, out double busEndTime)
        {
            if (IFS == 0)
                busEndTime = startTime + (timeFor1Bit * 1);
            else if (IFS == 4)
                busEndTime = startTime + (timeFor1Bit * 2);
            else if (IFS == 6)
                busEndTime = startTime + (timeFor1Bit * 3);
            else
                busEndTime = startTime + (timeFor1Bit * 3);

            return GetBus(startTime, busEndTime, "IFS");
        }

        private IEnumerable<IBusData> Get_Overload(double startTime)
        {
            double endTime = startTime + (timeFor1Bit * 6);

            var bus = new PolygonBusData();
            bus.StartIndex = TimeStampConverter(startTime) * 1e9;
            bus.StopIndex = TimeStampConverter(endTime) * 1e9;
            bus.Data = "Overload Flag";
            bus.Brush = System.Windows.Media.Brushes.Red;
            yield return bus;

            bus = new PolygonBusData();
            bus.StartIndex = TimeStampConverter(endTime) * 1e9;

            endTime += timeFor1Bit * 8;

            bus.StopIndex = TimeStampConverter(endTime) * 1e9;
            bus.Data = "Overload Delimiter";
            bus.Brush = System.Windows.Media.Brushes.Red;
            yield return bus;

            bus = new PolygonBusData();
            bus.StartIndex = TimeStampConverter(endTime) * 1e9;

            endTime += timeFor1Bit * 3;

            bus.StopIndex = TimeStampConverter(endTime) * 1e9;
            bus.Data = "IFS";
            bus.Brush = System.Windows.Media.Brushes.Red;
            yield return bus;
        }

        private IEnumerable<IBusData> Get_ErrorFlag(double startTime)
        {
            double endTime = startTime + (timeFor1Bit * 6);

            var bus = new PolygonBusData();
            bus.StartIndex = TimeStampConverter(startTime) * 1e9;
            bus.StopIndex = TimeStampConverter(endTime) * 1e9;
            bus.Data = "Error Flag";
            bus.Brush = System.Windows.Media.Brushes.Red;
            yield return bus;

            bus = new PolygonBusData();
            bus.StartIndex = TimeStampConverter(endTime) * 1e9;

            endTime += timeFor1Bit * 8;

            bus.StopIndex = TimeStampConverter(endTime) * 1e9;
            bus.Data = "Error Delimiter";
            bus.Brush = System.Windows.Media.Brushes.Red;
            yield return bus;

            bus = new PolygonBusData();
            bus.StartIndex = TimeStampConverter(endTime) * 1e9;

            endTime += timeFor1Bit * 3;

            bus.StopIndex = TimeStampConverter(endTime) * 1e9;
            bus.Data = "IFS";
            bus.Brush = System.Windows.Media.Brushes.Red;
            yield return bus;
        }

        private IBusData Get_StuffCount(double startTime, double stopTime) => GetBus(startTime, stopTime, "Stuff Count");

        IEnumerable<IBusData> IResultViewModel.GetBusDiagram(ChannelInfo channel, double startTime, double endTime)

        {
            List<CANFrame> resultList = resultCollection.SkipWhile(r => r.StopTime < startTime).TakeWhile(r => r.StartTime <= endTime).ToList();

            bool errorDetected = false;
            double formErrorDetectedTime = double.MinValue;
            foreach (var frame in resultList)
            {
                Currenttime = frame.StartTime;



                if (Config.CANType == eCANType.CAN)
                    timeFor1Bit = 1d / (Config.BaudRate * 1000d);
                else
               if (Config.CANType == eCANType.CANFD)
                    timeFor1Bit = 1d / (Config.BaudRateCANFD * 1000d);



                yield return Get_SOF(frame.StartTime, out Currenttime);

                if ((errorDetected = CheckForBitStuffError(frame, Currenttime, frame.StopTimeList[0])) == false)
                    yield return Get_ID(Currenttime, frame.StopTimeList[0], frame.IDDdataBytes);

                if (Config.CANType == eCANType.CAN && frame.IsStandard == true)
                {
                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[0], frame.StopTimeList[1])) == false)
                            yield return Get_RTR(frame.StopTimeList[0], frame.StopTimeList[1]);
                    }

                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[1], frame.StopTimeList[2])) == false)
                            yield return Get_IDFLAG(frame.StopTimeList[1], frame.StopTimeList[2]);
                    }

                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[2], frame.StopTimeList[3])) == false)
                            yield return Get_ReservedBit(frame.StopTimeList[2], frame.StopTimeList[3]);


                        if (frame.ReservedBit != 0)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[3];
                        }
                    }
                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[3], frame.StopTimeList[4])) == false)
                            yield return Get_DLC(frame.StopTimeList[3], frame.StopTimeList[4], frame.DLCDdataBytes);
                    }

                    Currenttime = frame.StopTimeList[4];
                    if (!errorDetected)
                    {
                        if (frame.DLCDdataBytes != 0 && frame.RTRDdataBytes == 0)
                        {
                            if (frame.DataBytes.Count != 0)
                            {
                                errorDetected = CheckForBitStuffErrorAndGet_DATA(frame, frame.StopTimeList[4], frame.DataStopTimeList, frame.DataBytes, out List<IBusData> dataBus);

                                double lastBusTime = double.MinValue;
                                foreach (var bus in dataBus)
                                {
                                    lastBusTime = bus.StartIndex * 1e9 + SessionConfiguration.TriggerTime;
                                    yield return bus;
                                }

                                Currenttime = frame.DataStopTimeList[frame.DataBytes.Count - 1];
                            }
                        }
                    }

                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, Currenttime, frame.StopTimeList[5])) == false)
                            yield return Get_CRC(Currenttime, frame.StopTimeList[5], frame.CRCDdataBytes);
                    }

                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[5], frame.StopTimeList[6])) == false)
                            yield return Get_CRCDel(frame.StopTimeList[5], frame.StopTimeList[6]);
                    }

                    if (!errorDetected)
                    {
                        if (frame.CRCDele != 1)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[6];
                        }
                    }

                    if (!errorDetected)
                    {
                        yield return Get_ACK(frame.StopTimeList[6], frame.StopTimeList[7]);

                        if (frame.ACKDdataBytes != 0)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[7];
                        }
                    }

                    if (!errorDetected)
                    {
                        yield return Get_ACKDel(frame.StopTimeList[7], frame.StopTimeList[8]);

                        if (frame.ACKDele != 1)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[8];
                        }
                    }

                    if (!errorDetected)
                    {
                        yield return Get_EOF(frame.StopTimeList[8], frame.EOF, out Currenttime);

                        if (frame.EOF != 127)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = Currenttime;
                        }
                    }

                    if (!errorDetected)
                    {
                        yield return Get_IFS(Currenttime, frame.IFS, out Currenttime);

                        if (frame.IFS != 7)
                        {
                            foreach (var bus in Get_Overload(Currenttime))
                                yield return bus;
                        }
                    }
                }
                else
                    if (Config.CANType == eCANType.CAN && frame.IsExtended == true)
                {

                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[0], frame.StopTimeList[1])) == false)
                            yield return Get_SRR(frame.StopTimeList[0], frame.StopTimeList[1]);
                    }

                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[1], frame.StopTimeList[2])) == false)
                            yield return Get_IDFLAG(frame.StopTimeList[1], frame.StopTimeList[2]);
                    }

                    if(!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[2], frame.StopTimeList[3])) == false)
                            yield return Get_EXtendedID(frame.StopTimeList[2], frame.StopTimeList[3], frame.IDEDdataBytes);
                    }


                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[3], frame.StopTimeList[4])) == false)
                            yield return Get_RTR(frame.StopTimeList[3], frame.StopTimeList[4]);
                    }
                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[4], frame.StopTimeList[5])) == false)
                            yield return Get_ReservedBit(frame.StopTimeList[4], frame.StopTimeList[5]);


                        if (frame.ReservedBit != 0)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[5];
                        }
                    }
                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[5], frame.StopTimeList[6])) == false)
                            yield return Get_ReservedBit1(frame.StopTimeList[5], frame.StopTimeList[6]);


                        if (frame.ReservedBit1 != 0)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[6];
                        }
                    }
                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[6], frame.StopTimeList[7])) == false)
                            yield return Get_DLC(frame.StopTimeList[6], frame.StopTimeList[7], frame.DLCDdataBytes);
                    }

                    Currenttime = frame.StopTimeList[7];
                    if (!errorDetected)
                    {
                       
                        if (frame.DLCDdataBytes != 0 && frame.RTRDdataBytes == 0)
                        {
                            if (frame.DataBytes.Count != 0)
                            {
                                errorDetected = CheckForBitStuffErrorAndGet_DATA(frame, frame.StopTimeList[7], frame.DataStopTimeList, frame.DataBytes, out List<IBusData> dataBus);

                                double lastBusTime = double.MinValue;
                                foreach (var bus in dataBus)
                                {
                                    lastBusTime = bus.StartIndex * 1e9 + SessionConfiguration.TriggerTime;
                                    yield return bus;
                                }

                                Currenttime = frame.DataStopTimeList[frame.DataBytes.Count - 1];
                            }
                        }
                    }
                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, Currenttime, frame.StopTimeList[8])) == false)
                            yield return Get_CRC(Currenttime, frame.StopTimeList[8], frame.CRCDdataBytes);
                    }

                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[8], frame.StopTimeList[9])) == false)
                            yield return Get_CRCDel(frame.StopTimeList[8], frame.StopTimeList[9]);
                    }

                    if (!errorDetected)
                    {
                        if (frame.CRCDele != 1)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[9];
                        }
                    }

                    if (!errorDetected)
                    {
                        yield return Get_ACK(frame.StopTimeList[9], frame.StopTimeList[10]);

                        if (frame.ACKDdataBytes != 0)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[10];
                        }
                    }

                    if (!errorDetected)
                    {
                        yield return Get_ACKDel(frame.StopTimeList[10], frame.StopTimeList[11]);

                        if (frame.ACKDele != 1)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[11];
                        }
                    }

                    if (!errorDetected)
                    {
                        yield return Get_EOF(frame.StopTimeList[11], frame.EOF, out Currenttime);

                        if (frame.EOF != 127)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = Currenttime;
                        }
                    }

                    if (!errorDetected)
                    {
                        yield return Get_IFS(Currenttime, frame.IFS, out Currenttime);

                        if (frame.IFS != 7)
                        {
                            foreach (var bus in Get_Overload(Currenttime))
                                yield return bus;
                        }
                    }
          
                }
                else
                    if (Config.CANType == eCANType.CANFD && frame.IsStandard == true)
                {
                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[0], frame.StopTimeList[1])) == false)
                            yield return Get_ReservedBit1(frame.StopTimeList[0], frame.StopTimeList[1]);


                        if (frame.ReservedBit1 != 0)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[1];
                        }
                    }

                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[1], frame.StopTimeList[2])) == false)
                            yield return Get_IDFLAG(frame.StopTimeList[1], frame.StopTimeList[2]);
                    }

                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[2], frame.StopTimeList[3])) == false)
                            yield return Get_EDL(frame.StopTimeList[2], frame.StopTimeList[3]);
                    }

                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[3], frame.StopTimeList[4])) == false)
                            yield return Get_ReservedBit(frame.StopTimeList[3], frame.StopTimeList[4]);


                        if (frame.ReservedBit != 0)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[4];
                        }
                    }
                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[4], frame.StopTimeList[5])) == false)
                            yield return Get_BRS(frame.StopTimeList[4], frame.StopTimeList[5]);
                    }
                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[5], frame.StopTimeList[6])) == false)
                            yield return Get_ESI(frame.StopTimeList[5], frame.StopTimeList[6]);
                    }

                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[6], frame.StopTimeList[7])) == false)
                            yield return Get_DLC(frame.StopTimeList[6], frame.StopTimeList[7], frame.DLCDdataBytes);
                    }

                    Currenttime = frame.StopTimeList[7];
                    if (!errorDetected)
                    {
                        if( frame.DLCDdataBytes!=0)
                     
                        {
                            if (frame.DLCDdataBytes > 0 && frame.DataBytes.Count == 0)
                                errorDetected = true;
                            if (frame.DataBytes.Count != 0)
                            {
                                errorDetected = CheckForBitStuffErrorAndGet_DATA(frame, frame.StopTimeList[7], frame.DataStopTimeList, frame.DataBytes, out List<IBusData> dataBus);

                                double lastBusTime = double.MinValue;
                                foreach (var bus in dataBus)
                                {
                                    lastBusTime = bus.StartIndex * 1e9 + SessionConfiguration.TriggerTime;
                                    yield return bus;
                                }

                                Currenttime = frame.DataStopTimeList[frame.DataBytes.Count - 1];
                            }
                        }
                    }

                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, Currenttime, frame.StopTimeList[8])) == false)
                            yield return Get_StuffCount(Currenttime, frame.StopTimeList[8]);
                    }
                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[8], frame.StopTimeList[9])) == false)
                            yield return Get_CRC(frame.StopTimeList[8], frame.StopTimeList[9], frame.CRCDdataBytes);
                    }

                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[9], frame.StopTimeList[10])) == false)
                            yield return Get_CRCDel(frame.StopTimeList[9], frame.StopTimeList[10]);
                    }

                    if (!errorDetected)
                    {
                        if (frame.CRCDele != 1)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[10];
                        }
                    }

                    if (!errorDetected)
                    {
                        yield return Get_ACK(frame.StopTimeList[10], frame.StopTimeList[11]);

                        if (frame.ACKDdataBytes != 0)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[11];
                        }
                    }

                    if (!errorDetected)
                    {
                        yield return Get_ACKDel(frame.StopTimeList[11], frame.StopTimeList[12]);

                        if (frame.ACKDele != 1)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[12];
                        }
                    }

                    if (!errorDetected)
                    {
                        yield return Get_EOF(frame.StopTimeList[12], frame.EOF, out Currenttime);

                        if (frame.EOF != 127)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = Currenttime;
                        }
                    }

                    if (!errorDetected)
                    {
                        yield return Get_IFS(Currenttime, frame.IFS, out Currenttime);

                        if (frame.IFS != 7)
                        {
                            foreach (var bus in Get_Overload(Currenttime))
                                yield return bus;
                        }
                    }
             

                }
                else
                     if (Config.CANType == eCANType.CANFD && frame.IsExtended == true)
                {
                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[0], frame.StopTimeList[1])) == false)
                            yield return Get_SRR(frame.StopTimeList[0], frame.StopTimeList[1]);
                    }
                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[1], frame.StopTimeList[2])) == false)
                            yield return Get_IDFLAG(frame.StopTimeList[1], frame.StopTimeList[2]);
                    }
                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[2], frame.StopTimeList[3])) == false)
                            yield return Get_EXtendedID(frame.StopTimeList[2], frame.StopTimeList[3], frame.IDEDdataBytes);
                    }
                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[3], frame.StopTimeList[4])) == false)
                            yield return Get_ReservedBit1(frame.StopTimeList[3], frame.StopTimeList[4]);
                 
                        if (frame.ReservedBit1 != 0)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[4];
                        }
                    }
                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[4], frame.StopTimeList[5])) == false)
                            yield return Get_EDL(frame.StopTimeList[4], frame.StopTimeList[5]);
                    }
                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[5], frame.StopTimeList[6])) == false)
                            yield return Get_ReservedBit(frame.StopTimeList[5], frame.StopTimeList[6]);
                   
                        if (frame.ReservedBit != 0)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[6];
                        }
                    }
                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[6], frame.StopTimeList[7])) == false)
                            yield return Get_BRS(frame.StopTimeList[6], frame.StopTimeList[7]);
                    }


                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[7], frame.StopTimeList[8])) == false)
                            yield return Get_ESI(frame.StopTimeList[7], frame.StopTimeList[8]);
                    }

                    if (!errorDetected)
                    {
                        if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[8], frame.StopTimeList[9])) == false)
                            yield return Get_DLC(frame.StopTimeList[8], frame.StopTimeList[9], frame.DLCDdataBytes);
                    }

                    Currenttime = frame.StopTimeList[9];
                    if (!errorDetected)
                    {

                        if (frame.DLCDdataBytes != 0 )
                        {
                            //Exceptioncase when bitstuff found in first data
                            if (frame.DLCDdataBytes > 0 && frame.DataBytes.Count == 0)
                                errorDetected = true;
                            if (frame.DataBytes.Count != 0)
                            {
                                errorDetected = CheckForBitStuffErrorAndGet_DATA(frame, frame.StopTimeList[9], frame.DataStopTimeList, frame.DataBytes, out List<IBusData> dataBus);

                                double lastBusTime = double.MinValue;
                                foreach (var bus in dataBus)
                                {
                                    lastBusTime = bus.StartIndex * 1e9 + SessionConfiguration.TriggerTime;
                                    yield return bus;
                                }

                                Currenttime = frame.DataStopTimeList[frame.DataBytes.Count - 1];
                            }
                        }
                    }

                    if (!errorDetected)
                    {
                        //if ((errorDetected = CheckForBitStuffError(frame, Currenttime, frame.StopTimeList[10])) == false)
                            yield return Get_StuffCount(Currenttime, frame.StopTimeList[10]);
                    }
                    if (!errorDetected)
                    {
                        //if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[10], frame.StopTimeList[11])) == false)
                            yield return Get_CRC(frame.StopTimeList[10], frame.StopTimeList[11], frame.CRCDdataBytes);
                    }

                    if (!errorDetected)
                    {
                       // if ((errorDetected = CheckForBitStuffError(frame, frame.StopTimeList[11], frame.StopTimeList[12])) == false)
                            yield return Get_CRCDel(frame.StopTimeList[11], frame.StopTimeList[12]);
                    }

                    if (!errorDetected)
                    {
                        if (frame.CRCDele != 1)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[12];
                        }
                    }

                    if (!errorDetected)
                    {
                        yield return Get_ACK(frame.StopTimeList[12], frame.StopTimeList[13]);

                        if (frame.ACKDdataBytes != 0)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[13];
                        }
                    }

                    if (!errorDetected)
                    {
                        yield return Get_ACKDel(frame.StopTimeList[13], frame.StopTimeList[14]);

                        if (frame.ACKDele != 1)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = frame.StopTimeList[14];
                        }
                    }

                    if (!errorDetected)
                    {
                        yield return Get_EOF(frame.StopTimeList[14], frame.EOF, out Currenttime);

                        if (frame.EOF != 127)
                        {
                            errorDetected = true;
                            formErrorDetectedTime = Currenttime;
                        }
                    }

                    if (!errorDetected)
                    {
                        yield return Get_IFS(Currenttime, frame.IFS, out Currenttime);

                        if (frame.IFS != 7)
                        {
                            foreach (var bus in Get_Overload(Currenttime))
                                yield return bus;
                        }
                    }

                  

                }

                if (errorDetected)
                {
                    if (formErrorDetectedTime == double.MinValue)
                    {
                        // Bitstuff error
                        double StuffstartTime = 0;
                        if (Config.CANType == eCANType.CAN && frame.IsStandard == true)
                          StuffstartTime = frame.StopTimeList[9];
                      else  if(Config.CANType == eCANType.CAN && frame.IsExtended == true)
                            StuffstartTime = frame.StopTimeList[12];
                      else  if(Config.CANType==eCANType.CANFD && frame.IsStandard == true)
                            StuffstartTime = frame.StopTimeList[13];
                        else if (Config.CANType == eCANType.CANFD && frame.IsExtended == true)
                            StuffstartTime = frame.StopTimeList[15];
                        foreach (var bus in Get_ErrorFlag(StuffstartTime))
                            yield return bus;
                    }
                    else
                    {
                        // form error
                        foreach (var bus in Get_ErrorFlag(formErrorDetectedTime))
                            yield return bus;
                    }

                }
            }


        }

        #endregion
    }

    public class SearchFilterViewModel_CAN : SearchFilterViewModelBase<IFrame>
    {
        public SearchFilterViewModel_CAN(IList collection) : base(collection)
        {
        }

        private string identifier;
        public string Identifier
        {
            get => identifier;
            set
            {
                identifier = value;
                OnPropertyChanged(nameof(Identifier));
            }
        }

        private string extendedIdentifier;
        public string ExtendedIdentifier
        {
            get => extendedIdentifier;
            set
            {
                extendedIdentifier = value;
                OnPropertyChanged(nameof(ExtendedIdentifier));
            }
        }

        private string dataLengthCode;
        public string DataLengthCode
        {
            get { return dataLengthCode; }
            set
            {
                dataLengthCode = value;
                OnPropertyChanged(nameof(DataLengthCode));
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

        private string cCheck;
        public string CCheck
        {
            get { return cCheck; }
            set
            {
                cCheck = value;
                OnPropertyChanged(nameof(CCheck));
            }
        }

        private eFrameTypeCAN frameType = eFrameTypeCAN.DataFrame;
        public eFrameTypeCAN FrameType
        {
            get { return frameType; }
            set
            {
                frameType = value;
                OnPropertyChanged(nameof(FrameType));
            }
        }

        public override bool IsMatch(IFrame frame)
        {
            var canFrame = (CANFrame)frame;

            Int64 intValID = Int64.Parse(canFrame.IDDdataBytes.ToString());
            string identifire = String.Format("0x{0:X}", intValID);
            string identifire1 = String.Format("{0:X}", intValID);
            Int64 intValIDEX = Int64.Parse(canFrame.IDEDdataBytes.ToString());
            string identifireEx = String.Format("0x{0:X}", intValIDEX);
            string identifire1Ex = String.Format("{0:X}", intValIDEX);
            Int64 intValDLC = Int64.Parse(canFrame.DLCDdataBytes.ToString());
            string dlc = String.Format("0x{0:X}", intValDLC);
            string dlc1 = String.Format("{0:x}", intValDLC);
            Int64 intValCRC = Int64.Parse(canFrame.CRCDdataBytes.ToString());
            string crc = String.Format("0x{0:X}", intValCRC);
            string crc1 = String.Format("{0:x}", intValCRC);
            List<string> datStr = new List<string>();
            if (!string.IsNullOrEmpty(Data))
            {
                if (CheckHexa(Data.ToLower()))
                {
                    foreach (var db in canFrame.DataBytes)
                    {

                        datStr.Add($"0x{Convert.ToInt32(db):X}");
                    }
                }
                else
                {
                    foreach (var db in canFrame.DataBytes)
                    {
                        datStr.Add($"{Convert.ToInt32(db):X}");
                    }

                }
            }
            List<string> NewData = new List<string>();
            foreach (var item in datStr)
            {
                var num = item.ToLower();
                NewData.Add(num);

            }
            //List<string> Frame = new List<string>();

            //    Frame.Add(FrameType.ToString());
            //}
            bool result = true;
            bool searchValid = false;
            if (!string.IsNullOrEmpty(Identifier))
            {
                result &= identifire.ToLower().Equals(Identifier.ToLower()) || identifire1.ToLower().Equals(Identifier.ToLower());
                searchValid = true;
            }
            if (!string.IsNullOrEmpty(ExtendedIdentifier))
            {
                result &= identifire.ToLower().Equals(ExtendedIdentifier.ToLower()) || identifire1Ex.ToLower().Equals(ExtendedIdentifier.ToLower());
                searchValid = true;
            }
            if (!string.IsNullOrEmpty(DataLengthCode))
            {
                result &= dlc.ToLower().Equals(DataLengthCode.ToLower()) || dlc1.ToLower().Equals(DataLengthCode.ToLower());
                searchValid = true;
            }

            if (!string.IsNullOrEmpty(Data))
            {
                result &= NewData.Contains(Data.ToLower());
                // item1 = datStr.IndexOf(search.Data);
                searchValid = true;
            }
            if (!string.IsNullOrEmpty(CCheck))
            {
                result &= crc.ToLower().Equals(CCheck.ToLower()) || crc1.ToLower().Equals(CCheck.ToLower());
                searchValid = true;
            }
            if (!string.IsNullOrEmpty(FrameType.ToString()))
            {
                result &= canFrame.FrameType.Equals(FrameType);
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
