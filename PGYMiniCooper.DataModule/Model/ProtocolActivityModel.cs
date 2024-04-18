using PGYMiniCooper.DataModule.Interface;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Diagnostics;
using System.Threading;
using PGYMiniCooper.DataModule.Structure;
using System.Windows.Controls;
using System.Configuration;
using ProdigyFramework.Collections;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace PGYMiniCooper.DataModule.Model
{
    public sealed class ProtocolActivityModel : ViewModelBase
    {
        private int inProgress = 0;
        long ellapsedTime;
        List<ProtocolActivityHolder> tempHolder;
        Dictionary<eProtocol, double> dcmaxTimeVal;
        Stopwatch stpTimer;
        bool clearI2C;
        bool clearSPI;
        bool clearUART;
        bool clearI3C;
        bool clearSPMI;
        bool clearRFFE;
        bool clearCAN;
        bool clearQSPI;
        private object lockerobj = new object();

        private ConfigModel configModel = null;
        private readonly IDataProvider dataReceiver;

        public ProtocolActivityModel(ConfigModel configuration)
        {
            this.configModel = configuration;
            tempHolder = new List<ProtocolActivityHolder>();
            dcmaxTimeVal = new Dictionary<eProtocol, double>();
            ProtocolCollection = new AsyncObservableCollection<ProtocolActivityHolder>();
            // TODO: add logic to subscribe update from all protocols


            //ResultAdder.Instance.OnAddResult += AddProtocols;
            //ResultAdder.Instance.OnClearBuffer += ClearBuffer;

            isI2CProtocol = false;
            isSPIProtocol = false;
            isUARTProtocol = false;
            //isCANProtocol = false;
            stpTimer = new Stopwatch();
            clearI2C = false;
            clearSPI = false;
            clearUART = false;
            clearI3C = false;
            clearSPMI = false;
            clearRFFE = false;
            clearCAN = false;
            clearQSPI = false;

            dataReceiver = Ioc.Default.GetService<IDataProvider>();

        }

        public void Reset()
        {
            dataReceiver.OnProtocolResultAdded -= OnProtocolResultAdded;
            dataReceiver.OnTriggerOccur -= DataReceiver_OnTriggerOccur;
        }

        private void OnProtocolResultAdded(IConfigModel config, IResultModel result)
        {
            // TODO: Remove this unused protocol activity related code - Activity view has been removed from view 
            //result.OnFrameDecoded += (s, f) => Result_OnFrameDecoded(config, f);
        }

        private void Result_OnFrameDecoded(IConfigModel config, IFrame frame)
        {
            // Add result to protocol view
            ProtocolCollection.Add(new ProtocolActivityHolder
            {
                Sample = frame.FrameIndex,

                Timestamp = frame.StartTime,
                Protocol = frame.ProtocolType,
                ProtocolName = config.Name,
                Error = frame.ErrorType,
            });
        }

        public void Initialize()
        {
            dataReceiver.OnProtocolResultAdded += OnProtocolResultAdded;
            dataReceiver.OnTriggerOccur += DataReceiver_OnTriggerOccur;

            protocolCollection.Clear();
            tempHolder.Clear();
            dcmaxTimeVal.Clear();
            isI2CProtocol = false;
            isSPIProtocol = false;
            isUARTProtocol = false;
            isI3CProtocol = false;
            isSPMIProtocol = false;
            isRFFEProtocol = false;
            isCANProtocol = false;
            isQSPIProtocol =false;

            var i2CConfig  = configModel.ProtocolConfigList.OfType<ConfigModel_I2C>().FirstOrDefault();
            var spiConfig  = configModel.ProtocolConfigList.OfType<ConfigModel_SPI>().FirstOrDefault();
            var UARTConfig = configModel.ProtocolConfigList.OfType<ConfigModel_UART>().FirstOrDefault();
            var i3cConfig  = configModel.ProtocolConfigList.OfType<ConfigModel_I3C>().FirstOrDefault();
            var spmiConfig = configModel.ProtocolConfigList.OfType<ConfigModel_SPMI>().FirstOrDefault();
            var rffeConfig = configModel.ProtocolConfigList.OfType<ConfigModel_RFFE>().FirstOrDefault();
            var qspiconfig = configModel.ProtocolConfigList.OfType<ConfigModel_QSPI>().FirstOrDefault();
            var canconfig  = configModel.ProtocolConfigList.OfType<ConfigModel_CAN>().FirstOrDefault();
            
            //SimpleIoc.Default.GetInstance<DataModule.Model.DataReceiver>().OnDataChanged -= UpdateMessage;
            //if (configModel.ConfigurationMode != eConfigMode.LA_Mode)
            //    SimpleIoc.Default.GetInstance<DataModule.Model.DataReceiver>().OnDataChanged += UpdateMessage;
            if ((i2CConfig != null) &&
                (i2CConfig.ChannelIndex_SCL != eChannles.None && i2CConfig.ChannelIndex_SDA != eChannles.None))
            {
                dcmaxTimeVal.Add(eProtocol.I2C, double.MinValue);
                IsI2CProtocol = true;
                clearI2C = false;
            }
            else
            {
                dcmaxTimeVal.Add(eProtocol.I2C, double.MaxValue);
                IsI2CProtocol = false;
                clearI2C = true;
            }
            if ((spiConfig!=null) &&
                ((spiConfig.ChannelIndex_CLK != eChannles.None && spiConfig.ChannelIndex_CS != eChannles.None) &&
                (spiConfig.ChannelIndex_MOSI != eChannles.None || spiConfig.ChannelIndex_MISO != eChannles.None)))
            {
                dcmaxTimeVal.Add(eProtocol.SPI, double.MinValue);
                IsSPIProtocol = true;
                clearSPI = false;
            }
            else
            {
                dcmaxTimeVal.Add(eProtocol.SPI, double.MaxValue);
                IsSPIProtocol = false;
                clearSPI = true;
            }
            if ((UARTConfig!=null) &&
                (UARTConfig.ChannelIndex_TX != eChannles.None || UARTConfig.ChannelIndex_RX != eChannles.None))
            {
                dcmaxTimeVal.Add(eProtocol.UART, double.MinValue);
                IsUARTProtocol = true;
                clearUART = false;
            }
            else
            {
                dcmaxTimeVal.Add(eProtocol.UART, double.MaxValue);
                IsUARTProtocol = false;
                clearUART = true;
            }

            if ((i3cConfig != null) &&
              (i3cConfig.ChannelIndex_SCL != eChannles.None && i3cConfig.ChannelIndex_SDA != eChannles.None))
            {
                dcmaxTimeVal.Add(eProtocol.I3C, double.MinValue);
                
                IsI3CProtocol = true;
                clearI3C = false;
            }
            else
            {
                dcmaxTimeVal.Add(eProtocol.I3C, double.MaxValue);
                IsI3CProtocol = false;
                clearI3C = true;
           }
            if ((spmiConfig!=null) &&
             (spmiConfig.ChannelIndex_SCL != eChannles.None && spmiConfig.ChannelIndex_SDA != eChannles.None))
            {
                dcmaxTimeVal.Add(eProtocol.SPMI, double.MinValue);
                IsSPMIProtocol = true;
                clearSPMI = false;
            }
            else
            {
                dcmaxTimeVal.Add(eProtocol.SPMI, double.MaxValue);
                IsSPMIProtocol = false;
                clearSPMI = true;
            }
            if ((rffeConfig!=null) &&
            (rffeConfig.ChannelIndex_SCL != eChannles.None && rffeConfig.ChannelIndex_SDA != eChannles.None))
            {
                dcmaxTimeVal.Add(eProtocol.RFFE, double.MinValue);
                IsRFFEProtocol = true;
                clearRFFE = false;
            }
            else
            {
                dcmaxTimeVal.Add(eProtocol.RFFE, double.MaxValue);
                IsRFFEProtocol = false;
                clearRFFE = true;
            }
            if ((canconfig!=null) &&
           (canconfig.ChannelIndex != eChannles.None ))
            {
                dcmaxTimeVal.Add(eProtocol.CAN, double.MinValue);
                IsCANProtocol = true;
                clearCAN = false;
            }
            else
            {
                dcmaxTimeVal.Add(eProtocol.CAN, double.MaxValue);
                IsCANProtocol = false;
                clearCAN = true;
            }

            //QSPI
            if ((qspiconfig!=null) &&
                ((qspiconfig.ChannelIndex_CLK != eChannles.None && qspiconfig.ChannelIndex_CS != eChannles.None) &&
                (qspiconfig.ChannelIndex_D0 != eChannles.None || qspiconfig.ChannelIndex_D1 != eChannles.None || qspiconfig.ChannelIndex_D2 != eChannles.None||qspiconfig.ChannelIndex_D3 != eChannles.None)))
            {
                dcmaxTimeVal.Add(eProtocol.QSPI, double.MinValue);
                IsQSPIProtocol = true;
                clearQSPI = false;
            }
            else
            {
                dcmaxTimeVal.Add(eProtocol.QSPI, double.MaxValue);
                IsQSPIProtocol = false;
                clearQSPI = true;
            }

            TriggerTime = 0;
            TriggerPacket = -1;
            stpTimer.Restart();
            ellapsedTime = stpTimer.ElapsedMilliseconds;
            ViewHeight();
        }

        private void DataReceiver_OnTriggerOccur()
        {
            TriggerTime = SessionConfiguration.TriggerTime;
        }

        private AsyncObservableCollection<ProtocolActivityHolder> protocolCollection;


        public AsyncObservableCollection<ProtocolActivityHolder> ProtocolCollection
        {
            get { return protocolCollection; }
            set
            {
                protocolCollection = value;
                RaisePropertyChanged("ProtocolCollection");
            }
        }
        object locker = new object();
        void UpdateMessage()
        {
            AddRemove(false, null, eProtocol.I2C);
        }

        public void AddRemove(bool isFrameAdd, List<IFrame> protocolList, eProtocol protocol, bool isClearBuffer = false)
        {
            lock (locker)
            {
                if (isFrameAdd)
                {
                    //Thread.Sleep(20);
                    foreach (var frame in protocolList)
                    {
                       
                        tempHolder.Add(new ProtocolActivityHolder()
                        {
                            Sample = frame.FrameIndex,
                            
                            Timestamp = frame.StartTime,
                            Protocol = protocol,
                            Error = frame.ErrorType,
                        });

                    }
                    if (dcmaxTimeVal.ContainsKey(protocol))
                        dcmaxTimeVal[protocol] = protocolList.Last().StartTime;
                    else
                        dcmaxTimeVal.Add(protocol, protocolList.Last().StartTime);
                }
                else
                {
                    if (TriggerModel.GetInstance().TriggerType == eTriggerTypeList.Protocol)
                    {
                        //if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.I2C)
                        //    TriggerPacket = ResultModel_I2C.GetInstance().TriggerPacket;
                        //else if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.SPI)
                        //    TriggerPacket = ResultModel_SPI.GetInstance().TriggerPacket;
                        //else if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.UART)
                        //    TriggerPacket = ResultModel_UART.GetInstance().TriggerPacket;
                        //else if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.I3C)
                        //{
                        //}//TODO: update code here//TriggerPacket = ResultModel_I3C.GetInstance().TriggerPacket;
                        //else if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.SPMI)
                        //    TriggerPacket = ResultModel_SPMI.GetInstance().TriggerPacket;
                        //else if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.RFFE)
                        //    TriggerPacket = ResultModel_RFFE.GetInstance().TriggerPacket;
                        //else if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.CAN)
                        //    TriggerPacket = ResultModel_CAN.GetInstance().TriggerPacket;
                        //qspi
                        //else if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.QSPI)
                        //    TriggerPacket = QSPIResultHolder.GetInstance().TriggerPacket;
                    }
                    TriggerTime = SessionConfiguration.TriggerTime;
                    if (!isClearBuffer)
                    {
                        double t = dcmaxTimeVal.Values.ToList().Min();
                        var sortedList = tempHolder.Where(p => p.Timestamp <= t).OrderBy(x => x.Timestamp).ToList();
                        foreach (var item in sortedList)
                        {
                            ProtocolCollection.Add(item);
                            if (item.Sample == 0)
                                SelectedIndex = 0;
                        }
                        tempHolder.RemoveAll(p => p.Timestamp <= t);
                    }
                    else
                    {
                        var sortedList = tempHolder.OrderBy(x => x.Timestamp).ToList();
                        foreach (var item in sortedList)
                        {
                            ProtocolCollection.Add(item);
                            if (item.Sample == 0)
                                SelectedIndex = 0;
                        }
                        tempHolder.Clear();
                    }
                }
            }
        }

        private bool AddProtocols(eProtocol protocol)
        {
            if (0 == Interlocked.Exchange(ref inProgress, 1))
            {
                eProtocol frameType = protocol;
                List<IFrame> protocolList = new List<IFrame>();
                if (frameType == eProtocol.SPI)
                {
                   // protocolList = ResultModel_SPI.GetInstance().AddtoAllProtocols();
                }
                else if (frameType == eProtocol.UART)
                {
                    //protocolList = ResultModel_UART.GetInstance().AddtoAllProtocols();
                }
                else if (frameType == eProtocol.I2C)
                {
                    //protocolList = ResultModel_I2C.GetInstance().AddtoAllProtocols();
                }
                else if (frameType == eProtocol.I3C)
                {
                    //TODO: update code here
                    //protocolList = ResultModel_I3C.GetInstance().AddtoAllProtocols();
                }
                else if (frameType == eProtocol.SPMI)
                {
                    //protocolList = ResultModel_SPMI.GetInstance().AddtoAllProtocols();
                }
                else if (frameType == eProtocol.RFFE)
                {
                    //protocolList = ResultModel_RFFE.GetInstance().AddtoAllProtocols();
                }
                else if(frameType== eProtocol.CAN)
                {
                    //protocolList = ResultModel_CAN.GetInstance().AddtoAllProtocols();
                }
                //qspi

                else if (frameType == eProtocol.QSPI)
                {
                   // protocolList = ResultModel_QSPI.GetInstance().AddtoAllProtocols();
                }
                AddRemove(true, protocolList, frameType);
                //foreach (var frame in protocolList)
                //{
                //    tempHolder.Add(new ProtocolActivityHolder()
                //    {
                //        Sample = frame.FrameIndex,
                //        Timestamp = frame.StartTime,
                //        Protocol = protocol,
                //        Error = frame.ErrorType,
                //    });

                //}
                //if (dcmaxTimeVal.ContainsKey(frameType))
                //    dcmaxTimeVal[frameType] = protocolList.Last().StartTime;
                //else
                //    dcmaxTimeVal.Add(frameType, protocolList.Last().StartTime);
                //if ((stpTimer.ElapsedMilliseconds - ellapsedTime) > 2500)
                //{
                //    double t = dcmaxTimeVal.Values.ToList().Min();
                //    var sortedList = tempHolder.Where(p => p.Timestamp <= t).OrderBy(x => x.Timestamp).ToList();
                //    //ProtocolCollection.SuspendCollectionChangeNotification();
                //    //foreach (var frame in sortedList)
                //    //{
                //    AddToCollection(sortedList);
                //    TriggerTime = SessionConfiguration.TriggerTime;
                //    //}
                //    //ProtocolCollection.NotifyChanges();
                //    tempHolder.RemoveAll(p => p.Timestamp <= t);
                //    ellapsedTime = stpTimer.ElapsedMilliseconds;

                //}
                //Release the lock
                Interlocked.Exchange(ref inProgress, 0);
                return true;
            }
            else
            {
                return false;
            }
        }
        void AddToCollection(List<ProtocolActivityHolder> frames)
        {
            //if (TriggerModel.GetInstance().TriggerType == eTriggerTypeList.Protocol)
            //{
            //    if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.I2C)
            //        TriggerPacket = ResultModel_I2C.GetInstance().TriggerPacket;
            //    //else if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.SPI)
            //    //    TriggerPacket = ResultModel_SPI.GetInstance().TriggerPacket;
            //    //else if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.UART)
            //    //    TriggerPacket = ResultModel_UART.GetInstance().TriggerPacket;
            //    else if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.CAN)
            //        TriggerPacket = ResultModel_CAN.GetInstance().TriggerPacket;
            //    else if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.CAN)
            //        TriggerPacket = ResultModel_CAN.GetInstance().TriggerPacket;
            //}
            foreach (var item in frames)
            {
                ProtocolCollection.Add(item);
                if (item.Sample == 0)
                    SelectedIndex = 0;
            }
        }

        private void ClearBuffer(eProtocol protocol)
        {
            if (protocol == eProtocol.I2C)
                clearI2C = true;
            else if (protocol == eProtocol.SPI)
                clearSPI = true;
            else if (protocol == eProtocol.UART)
                clearUART = true;
            else if (protocol == eProtocol.I3C)
                clearI3C = true;
            else if (protocol == eProtocol.SPMI)
                clearSPMI = true;
            else if (protocol == eProtocol.RFFE)
                clearRFFE = true;
            else if (protocol == eProtocol.CAN)
                clearCAN = true;
            else if (protocol == eProtocol.QSPI)
                clearQSPI = true;


            if (tempHolder.Count > 0 && clearI2C && clearSPI && clearUART && clearI3C && clearSPMI && clearRFFE && clearCAN && clearQSPI )
            {
                AddRemove(false, null, eProtocol.I2C, true);
                //var sortedList = tempHolder.OrderBy(x => x.Timestamp).ToList();
                //AddToCollection(sortedList);
                //TriggerTime = SessionConfiguration.TriggerTime;
                //tempHolder.Clear();
                //ProtocolCollection.SuspendCollectionChangeNotification();
                ////foreach (var frame in sortedList)
                ////{
                //ProtocolCollection.AddRange(sortedList);
                //if (protocolCollection.Count > 0)
                //    TriggerTime = SessionConfiguration.TriggerTime;
                ////}
                //ProtocolCollection.NotifyChanges();
            }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                if (ProtocolCollection.Count > 0 && value >= 0)
                {
                    var selectedPacket = ProtocolCollection[value];
                
                   
                    //if (ResultAdder.Instance.OnSelectionChanged != null)
                    //    ResultAdder.Instance.OnSelectionChanged(selectedPack
                    //// TODO: add logic to subscribe update from all protocols
                    //if (ResultAdder.Instance.OnSelectionChanged != null)
                    //    ResultAdder.Instance.OnSelectionChanged(selectedPacket);
                    //OnSubscribeDel(selectedPacket);
                    //IResultViewModel. result = null;

                }
                RaisePropertyChanged("SelectedIndex");
            }
        }
        //public delegate void del(ProtocolActivityHolder activityHolder);
        //public event del OnSubscribeDel;
        private int _TriggerPacket;
        public int TriggerPacket
        {
            get
            {
                return _TriggerPacket;
            }
            set
            {
                _TriggerPacket = value;
                RaisePropertyChanged("TriggerPacket");
            }
        }
        private double triggerTime;

        public double TriggerTime
        {
            get { return triggerTime; }
            set { triggerTime = value; RaisePropertyChanged("TriggerTime"); }
        }
        private bool isI2CProtocol;

        public bool IsI2CProtocol
        {
            get { return isI2CProtocol; }
            set { isI2CProtocol = value; RaisePropertyChanged("IsI2CProtocol"); }
        }
        private bool isSPIProtocol;

        public bool IsSPIProtocol
        {
            get { return isSPIProtocol; }
            set { isSPIProtocol = value; RaisePropertyChanged("IsSPIProtocol"); }
        }
        private bool isUARTProtocol;

        public bool IsUARTProtocol
        {
            get { return isUARTProtocol; }
            set { isUARTProtocol = value; RaisePropertyChanged("IsUARTProtocol"); }
        }

        private bool isI3CProtocol;

        public bool IsI3CProtocol
        {
            get { return isI3CProtocol; }
            set { isI3CProtocol = value; RaisePropertyChanged("IsI3CProtocol"); }
        }

        private bool isSPMIProtocol;

        public bool IsSPMIProtocol
        {
            get { return isSPMIProtocol; }
            set { isSPMIProtocol = value; RaisePropertyChanged("IsSPMIProtocol"); }
        }

        private bool isRFFEProtocol;

        public bool IsRFFEProtocol
        {
            get { return isRFFEProtocol; }
            set { isRFFEProtocol = value; RaisePropertyChanged("IsRFFEProtocol"); }
        }


        private bool isCANProtocol;

        public bool IsCANProtocol
        {
            get { return isCANProtocol; }
            set { isCANProtocol = value; RaisePropertyChanged("IsCANProtocol"); }
        }

        //qspi

        private bool isQSPIProtocol;

        public bool IsQSPIProtocol
        {
            get { return isQSPIProtocol; }
            set { isQSPIProtocol = value; RaisePropertyChanged("IsQSPIProtocol"); }
        }

        private int selectedIndex;


        private string protocolHeight = "*";
        public string ProtocolHeight
        {
            get
            {
                return protocolHeight;
            }
            set
            {
                protocolHeight = value;
                RaisePropertyChanged("ProtocolHeight");
            }
        }

        private string plotHeight = "*";
        public string PlotHeight
        {
            get
            {
                return plotHeight;
            }
            set
            {
                plotHeight = value;
                RaisePropertyChanged("PlotHeight");
            }
        }

        void ViewHeight()
        { 
            PlotHeight = "2*";
            ProtocolHeight = "*";
            if (IsI2CProtocol && IsSPIProtocol && IsUARTProtocol)
            {
                PlotHeight = "2.5*";
                ProtocolHeight = "1.1*";
            }
           else if (IsI2CProtocol && IsSPIProtocol && IsCANProtocol)
            {
                PlotHeight = "2.5*";
                ProtocolHeight = "1.1*";
            }
            else if ((IsI2CProtocol && IsSPIProtocol))
            {
                PlotHeight = "2*";
                ProtocolHeight = "1.5*";
            }
            else if ((IsSPIProtocol && IsUARTProtocol))
            {
                PlotHeight = "2*";
                ProtocolHeight = "1*";
            }
            else if ((IsI2CProtocol && IsUARTProtocol))
            {
                PlotHeight = "1*";
                ProtocolHeight = "1*";
            }
            else if ((IsI2CProtocol && IsCANProtocol))
            {
                PlotHeight = "1.5*";
                ProtocolHeight = "1*";
            }
            else if ((IsI3CProtocol && IsSPMIProtocol) || (IsSPMIProtocol && IsRFFEProtocol) || (IsRFFEProtocol && IsI3CProtocol))
            {
                PlotHeight = "2*";
                ProtocolHeight = "1*";
            }
            else if ((IsI2CProtocol && IsI3CProtocol) || (IsI2CProtocol && IsSPMIProtocol) || (IsI2CProtocol && IsRFFEProtocol))
            {
                PlotHeight = "2*";
                ProtocolHeight = "1*";
            }
            else if ((IsSPIProtocol && IsI3CProtocol) || (IsSPIProtocol && IsSPMIProtocol) || (IsSPIProtocol && IsRFFEProtocol))
            {
                PlotHeight = "2.2*";
                ProtocolHeight = "1*";
            }
            else if ((IsUARTProtocol && IsI3CProtocol) || (IsUARTProtocol && IsSPMIProtocol) || (IsUARTProtocol && IsRFFEProtocol))
            {
                PlotHeight = "2*";
                ProtocolHeight = "1*";
            }
            else if ((IsCANProtocol && IsI3CProtocol) || (IsCANProtocol && IsSPMIProtocol) || (IsCANProtocol && IsRFFEProtocol))
            {
                PlotHeight = "2*";
                ProtocolHeight = "1*";
            }


            //QSPI

            else if ((IsQSPIProtocol && IsI3CProtocol) || (IsQSPIProtocol && IsSPMIProtocol) || (IsQSPIProtocol && IsRFFEProtocol))
            {
                PlotHeight = "2*";
                ProtocolHeight = "1*";
            }

            else if (IsI2CProtocol || IsSPIProtocol || IsUARTProtocol || IsI3CProtocol || IsSPMIProtocol || IsRFFEProtocol || IsCANProtocol || IsQSPIProtocol)
            {
                if (IsI2CProtocol || IsI3CProtocol || IsSPMIProtocol || IsRFFEProtocol )
                {
                    PlotHeight = "1.5*";
                    ProtocolHeight = "2*";
                }
                else if (IsSPIProtocol)
                {
                    PlotHeight = "1*";
                    ProtocolHeight = "1.2*";
                }
                else if (IsUARTProtocol)
                {
                    PlotHeight = "1*";
                    ProtocolHeight = "1.3*";
                }
                else if (IsCANProtocol)
                {
                    PlotHeight = "1*";
                    ProtocolHeight = "1.3*";
                }
                else if (IsQSPIProtocol)
                {
                    PlotHeight = "1*";
                    ProtocolHeight = "1.2*";
                }
               
            }
        }

    }
}
