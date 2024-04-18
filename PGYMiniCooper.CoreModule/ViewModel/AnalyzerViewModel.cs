
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Model;
using ProdigyFramework.Behavior;
using System.Windows;
using PGYMiniCooper.CoreModule.ViewModel.ProtocolViewModel;
using PGYMiniCooper.DataModule.Service;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Model.Trigger_Config;
using CommunityToolkit.Mvvm.DependencyInjection;
using PGYMiniCooper.DataModule.Interface;

namespace PGYMiniCooper.CoreModule.ViewModel
{
    public class AnalyzerViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject, IResetViewModel
    {
        private readonly ServiceDelegator serviceDelegator;
        private readonly IDataProvider dataReceiver;
        public AnalyzerViewModel()
        {
            // Start ioc container
            serviceDelegator = new ServiceDelegator();

            dataReceiver = Ioc.Default.GetService<IDataProvider>();
            dataReceiver.CaptureStatusUpdated += AnalyzerViewModel_CaptureStatusUpdated;
            dataReceiver.OnTriggerOccur += DataReceiver_OnTriggerOccur;
            dataReceiver.OnAquisitionStopped += DataReceiver_OnAquisitionStopped;
            dataReceiver.OnDecodeCompleted += DataReceiver_OnDecodeCompleted;
            UpdateConfiguration(new ConfigurationViewModel(new ConfigModel()));
            //SimpleIoc.Default.GetInstance<ReportViewModel>().ReportStatusUpdated += AnalyzerViewModel_CaptureStatusUpdated;
            ReportViewModel.ReportStatusUpdated += AnalyzerViewModel_CaptureStatusUpdated;




            headerVM = new HeaderViewModel(this);
        }

        private void DataReceiver_OnDecodeCompleted(bool memoryLimitReached)
        {
            string message = "";
            // Show message box
            // followed by stop aquisition
            if (memoryLimitReached)
            {
               // HeaderVM.IsDecodeStopEnable = false;
                if (SessionConfiguration.SourceType == eSourceType.Live)
                {
                    var result = MessageBox.Show("Memory limit reached and Analysis Stopped. \nDo you want to stop Acquisition?", "Live capture", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {

                        HeaderVM.StopAcquisition();
                        

                    }
                }

                message += "Memory limit reached.";
            }

            if (SessionConfiguration.StopForcedDecode || memoryLimitReached)
            {
                message += " Analysis stopped.";
               //HeaderVM.StoppingDecoding = false;
            }
            else
            {
                message += " Analysis complete.";
               // HeaderVM.IsDecodeStopEnable = false;
            }

            Status = message;
        }

        private void DataReceiver_OnAquisitionStopped()
        {
            Status = "Aquisition Stopped.";
        }

        private void DataReceiver_OnTriggerOccur()
        {
            if(ConfigVM.Trigger.TriggerType!=eTriggerTypeList.Auto )
            Status = "(" + GetTriggerstatus() + ")" + " -> Trigger found at = " + SessionConfiguration.TriggerTime + "s";
        }
        
        private string GetTriggerstatus()
        {
            string res = "";
            TriggerViewModel trigger = configVM.Trigger;
            if (ConfigVM.Trigger.TriggerType == eTriggerTypeList.Protocol)
            {
                if (trigger.SelectedTrigger is TriggerConfig_I2C config_I2C)
                {
                    res = "I2C Trigger";
                    if (config_I2C.I2CTriggerAtSelected == eI2CTriggerAtList.Ack)
                        res = "I2C Trigger" + "(Ack)";
                    else if (config_I2C.I2CTriggerAtSelected == eI2CTriggerAtList.Nack)
                        res = "I2C Trigger" + "(Nack)";
                    else if (config_I2C.I2CTriggerAtSelected == eI2CTriggerAtList.Start)
                        res = "I2C Trigger" + "(Start)";
                    else if (config_I2C.I2CTriggerAtSelected == eI2CTriggerAtList.Stop)
                        res = "I2C Trigger" + "(Stop)";
                    else if (config_I2C.I2CTriggerAtSelected == eI2CTriggerAtList.Repeated_Start)
                        res = "I2C Trigger" + "(Repeated Start)";
                    else if (config_I2C.I2CTriggerAtSelected == eI2CTriggerAtList.Address)
                    {
                        if (trigger.TriggerModel.AddressComparison == eComparisonList.Equal_to)
                            res = "I2C Trigger" + "(Address) " + config_I2C.TransferType + " = " + config_I2C.AddressValue;
                        else
                            res = "I2C Trigger" + "(Address) " + config_I2C.TransferType + " != " + config_I2C.AddressValue;
                    }
                    else if (config_I2C.I2CTriggerAtSelected == eI2CTriggerAtList.Data)
                    {
                        if (trigger.TriggerModel.AddressComparison == eComparisonList.Equal_to)
                            res = "I2C Trigger" + "(Data)" + " = " + config_I2C.DataValue;
                        else
                            res = "I2C Trigger" + "(Data)" + " != " + config_I2C.DataValue;
                    }
                    else if (config_I2C.I2CTriggerAtSelected == eI2CTriggerAtList.Address_Data)
                    {

                        if (trigger.TriggerModel.AddressComparison == eComparisonList.Equal_to)
                            res = "I2C Trigger" + "(Addr + Data)" + " Addr = " + config_I2C.AddressValue + " Data = " + config_I2C.DataValue;
                        else
                            res = "I2C Trigger" + "(Addr + Data)" + " Addr != " + config_I2C.AddressValue + " Data != " + config_I2C.DataValue;
                    }
                }
                else if (trigger.SelectedTrigger is TriggerConfig_SPI config_SPI)
                {
                    res = "SPI Trigger";
                    if (config_SPI.IsMOSIChecked)
                    {
                        if (config_SPI.MOSIComparison == eComparisonList.Equal_to)
                            res = "SPI Trigger" + "(MOSI)" + " = " + config_SPI.MOSIData;
                        else
                            res = "SPI Trigger" + "(MOSI)" + " != " + config_SPI.MOSIData;
                    }
                    else if (config_SPI.IsMISOChecked)
                    {
                        if (config_SPI.MISOComparison == eComparisonList.Equal_to)
                            res = "SPI Trigger" + "(MISO)" + " = " + config_SPI.MISOData;
                        else
                            res = "SPI Trigger" + "(MISO)" + " != " + config_SPI.MISOData;
                    }
                }
                else if (trigger.SelectedTrigger is TriggerConfig_UART config_UART)
                {
                    res = "UART Trigger";
                    if (config_UART.IsUARTDataChecked)
                    {
                        if (config_UART.UARTDataComparison == eComparisonList.Equal_to)
                            res = "UART Trigger" + "(Data)" + " = " + config_UART.UARTDataValue;
                        else
                            res = "UART Trigger" + "(Data)" + " != " + config_UART.UARTDataValue;
                    }
                }
                else if (trigger.SelectedTrigger is TriggerConfig_SPMI config_SPMI)
                {
                    res = "SPMI Trigger" + "= " + config_SPMI.SelSPMICommand;
                }
                else if (trigger.SelectedTrigger is TriggerConfig_RFFE config_RFFE)
                { 
                    res = "RFFE Trigger" + "= " + config_RFFE.SelRFFECommand;
                }
                else if (trigger.SelectedTrigger is TriggerConfig_I3C config_I3C)
                {
                    res = "I3C Trigger " + config_I3C.SelI3CMessage;
                }
                else if (trigger.SelectedTrigger is TriggerConfig_CAN config_CAN)
                {
                    res = "CAN Trigger";
                }
                else if (trigger.SelectedTrigger is TriggerConfig_QSPI config_QSPI)
                {
                    res = "QSPI Trigger" + config_QSPI.SelQSPICommand;
                }

            }
            else if (ConfigVM.Trigger.TriggerType == eTriggerTypeList.Pattern)
            {
                res = "Pattern Trigger" + " = " + trigger.TriggerModel.PatternText;
            }
            else if (ConfigVM.Trigger.TriggerType == eTriggerTypeList.Timing)
            {
                if (trigger.TriggerModel.TimingTriggerTypeSelected == eTimingTriggerTypeList.Pulse_Width)
                {
                    res = "Pulse Trigger";
                    if (trigger.TriggerModel.PulseComparisonSelected == ePulseComparisonList.Greater_than)
                        res = "Pulse Width Trigger" + "(" + trigger.TriggerModel.PulseWidthChannel + ")" + " > " + trigger.TriggerModel.PulseWidthCount + "ns";
                    else
                        res = "Pulse Width Trigger" + "(" + trigger.TriggerModel.PulseWidthChannel + ")" + " < " + trigger.TriggerModel.PulseWidthCount + "ns";
                }
                else if (trigger.TriggerModel.TimingTriggerTypeSelected == eTimingTriggerTypeList.Delay)
                {
                    res = "Delay Trigger";
                    if (trigger.TriggerModel.DelayComparisonSelected == ePulseComparisonList.Greater_than)
                        res = "Delay Trigger b/w " + "(" + trigger.TriggerModel.DelayChannel1 + "," + trigger.TriggerModel.DelayChannel2 + ")" + " >= " + trigger.TriggerModel.DelayCount + "ns";
                    else
                        res = "Delay Trigger b/w " + "(" + trigger.TriggerModel.DelayChannel1 + "," + trigger.TriggerModel.DelayChannel2 + ")" + " <= " + trigger.TriggerModel.DelayCount + "ns";
                }
            }
            else
                res = "";


            return res;
        }

        //private void AnalyzerViewModel_ReportStatusUpdated(string statusMesssage)
        //{
        //    this.Status = statusMesssage;
        //}
        private void AnalyzerViewModel_CaptureStatusUpdated(string statusMesssage)
        {
            this.Status = statusMesssage;
        }

        public void UpdateConfiguration(ConfigurationViewModel configuration)
        {
            ConfigVM = configuration;
            WaveformListingVM = new WaveformListingViewModel(ConfigVM);
            ProtocolActivityVM = new ProtocolActivityViewModel(ConfigVM);
            ProtoVM = new ResultsViewModel(ConfigVM);
            ReportVM = new ReportViewModel(ConfigVM, protocolActivityVM, waveformListingVM, ProtoVM);
            SearchFilterVM = new SearchFilterViewModel();
            DcProdigyPlotView = new ProtocolPlotViewModel(ConfigVM, ProtoVM, ProtocolActivityVM);
            DcTimingPlotView = new TimingPlotViewModel(ConfigVM, ProtoVM);
            Analytics = new AnalyticsViewModel();
        }

        private AnalyticsViewModel analytics;
        public AnalyticsViewModel Analytics
        {
            get
            {
                return analytics;
            }
            set
            {
                analytics = value;
                OnPropertyChanged();
            }
        }

        ResultsViewModel protoVM;

        public ResultsViewModel ProtoVM
        {
            get { return protoVM; }
            set
            {
                protoVM = value;
                OnPropertyChanged();
            }
        }

        HeaderViewModel headerVM;

        public HeaderViewModel HeaderVM
        {
            get { return headerVM; }
            set
            {
                headerVM = value;
                OnPropertyChanged();
            }
        }

        ConfigurationViewModel configVM;

        public ConfigurationViewModel ConfigVM
        {
            get { return configVM; }
            set
            {
                configVM = value; OnPropertyChanged();
            }
        }


        ReportViewModel reportVM;

        public ReportViewModel ReportVM
        {
            get { return reportVM; }
            set
            {
                reportVM = value;
                OnPropertyChanged();
            }
        }

        SearchFilterViewModel searchFilterVM;

        public SearchFilterViewModel SearchFilterVM
        {
            get { return searchFilterVM; }
            set
            {
                searchFilterVM = value;
                OnPropertyChanged();
            }
        }

        private WaveformListingViewModel waveformListingVM;
        public WaveformListingViewModel WaveformListingVM
        {
            get { return waveformListingVM; }
            set { waveformListingVM = value; OnPropertyChanged(); }
        }

        private ProtocolPlotViewModel dcProdigyPlotView;
        public ProtocolPlotViewModel DcProdigyPlotView
        {
            get { return dcProdigyPlotView; }
            set { dcProdigyPlotView = value; OnPropertyChanged(); }
        }

        private TimingPlotViewModel dcTimingPlotView;
        public TimingPlotViewModel DcTimingPlotView
        {
            get { return dcTimingPlotView; }
            set { dcTimingPlotView = value; OnPropertyChanged(); }
        }

        private ProtocolActivityViewModel protocolActivityVM;

        public ProtocolActivityViewModel ProtocolActivityVM
        {
            get { return protocolActivityVM; }
            set
            {
                protocolActivityVM = value; 
                OnPropertyChanged();
            }
        }

        private string status;

        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged();
            }
        }

        public Command serachCommand
        {
            get
            {
                //I2CResultViewModel.GetInstance().Height1 = "auto";
                //I2CResultViewModel.GetInstance().Height2 = "auto";

                return new Command(new Command.ICommandOnExecute(() => protoVM.Serach(eProtocol.I2C)));
            }
        }



        public Command serachCommandUart
        {
            get
            {
           
                return new Command(new Command.ICommandOnExecute(() => protoVM.Serach(eProtocol.UART)));
            }
        }

        public Command SearchCommandSPI
        {
            get
            {
             
                return new Command(new Command.ICommandOnExecute(() => protoVM.Serach(eProtocol.SPI)));
            }
        }


        public Command SearchCommandCAN
        {
            get
            {
           
                return new Command(new Command.ICommandOnExecute(() => protoVM.Serach(eProtocol.CAN)));
            }
        }

     
        
        public void Initialize()
        {
            WaveformListingVM.Initialize();
            ProtocolActivityVM.Initialize();
            ProtoVM.Initialize();
            SearchFilterVM.Initialize();
            dcProdigyPlotView.Initialize();
            dcTimingPlotView.Initialize();
        }
  
        public void Reset()
        {
            WaveformListingVM.Reset();
            ProtocolActivityVM.Reset();
            ProtoVM.Reset();
            SearchFilterVM.Reset();
            dcProdigyPlotView.Reset();
            dcTimingPlotView.Reset();

            // Cleanup the resources
            System.GC.Collect();
        }
    }
}
