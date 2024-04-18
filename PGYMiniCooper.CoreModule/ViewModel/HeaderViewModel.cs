using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using ProdigyFramework.Behavior;
using ProdigyFramework.ComponentModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace PGYMiniCooper.CoreModule.ViewModel
{
    public class HeaderViewModel : ViewModelBase
    {
        private AnalyzerViewModel analyzerViewModel;
        private readonly HostInterface hostInterface;
      
        public HeaderViewModel(AnalyzerViewModel analyzerViewModel)
        {
            this.analyzerViewModel = analyzerViewModel;

            hostInterface = new HostInterface();
            activeWindow = "Setup View";
            //isStartEnable = false;
            //isStopEnable = false;
            //IsDecodeStopEnable = false;
            SessionConfiguration.PublishDecodeStatus += SessionConfiguration_PublishDecodeStatus;

            Ioc.Default.GetService<IDataProvider>().OnDecodeCompleted += HeaderViewModel_OnDecodeCompleted;
        }

        private void HeaderViewModel_OnDecodeCompleted(bool memoryLimitReached)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                IsDecoding = false;
                StoppingDecoding = false;
            }));
        }

        private void SessionConfiguration_PublishDecodeStatus(int hardwareVersion)
        {
            HWVersion = hardwareVersion;
        }

        #region Data Members

        private bool isConnected = false;

        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                isConnected = value;
                RaisePropertyChanged("IsConnected");
            }
        }

        private bool connectionProgress = false;

        public bool ConnectionProgress
        {
            get { return connectionProgress; }
            set
            {
                connectionProgress = value;
                RaisePropertyChanged("ConnectionProgress");
            }
        }

        #endregion

        #region Command

        private ICommand testConnectionCommand;

        public ICommand TestConnectionCommand
        {
            get
            {
                if(testConnectionCommand == null)
                {
                    testConnectionCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(testConnectionMethod, () =>
                    {
                        return ConnectionProgress == false && IsCapturing == false && StoppingDecoding == false;
                    });

                    this.PropertyChanged += (s, e) =>
                    {
                        if (nameof(ConnectionProgress) == e.PropertyName || nameof(IsCapturing) == e.PropertyName || nameof(StopDecodeCommand) == e.PropertyName)
                            ((CommunityToolkit.Mvvm.Input.RelayCommand)TestConnectionCommand).NotifyCanExecuteChanged();
                    };
                }
                
                return testConnectionCommand;
            }
        }

        private ICommand startCaptureCommand;

        public ICommand StartCaptureCommand
        {
            get
            {
                if (startCaptureCommand == null)
                {
                    startCaptureCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(runMethod, () =>
                    {
                        return IsConnected && IsCapturing == false && IsDecoding == false;
                    });

                    this.PropertyChanged += (s, e) =>
                    {
                        if (nameof(IsConnected) == e.PropertyName || nameof(IsCapturing) == e.PropertyName || nameof(IsDecoding) == e.PropertyName)
                            ((CommunityToolkit.Mvvm.Input.RelayCommand)StartCaptureCommand).NotifyCanExecuteChanged();
                    };
                }

                return startCaptureCommand;
            }
        }

        private ICommand stopCaptureCommand;

        public ICommand StopCaptureCommand
        {
            get
            {
                if (stopCaptureCommand == null)
                {
                    stopCaptureCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(StopAcquisition, () =>
                    {
                        return IsCapturing == true;
                    });

                    this.PropertyChanged += (s, e) =>
                    {
                        if (nameof(IsCapturing) == e.PropertyName)
                            ((CommunityToolkit.Mvvm.Input.RelayCommand)StopCaptureCommand).NotifyCanExecuteChanged();
                    };
                }

                return stopCaptureCommand;
            }
        }

        private ICommand stopDecodeCommand;

        public ICommand StopDecodeCommand
        {
            get
            {
                if (stopDecodeCommand == null)
                {
                    stopDecodeCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(StopDecode, () =>
                    {
                        return IsDecoding == true && StoppingDecoding == false;
                    });

                    this.PropertyChanged += (s, e) =>
                    {
                        if (nameof(IsDecoding) == e.PropertyName || nameof(StoppingDecoding) == e.PropertyName)
                            ((CommunityToolkit.Mvvm.Input.RelayCommand)StopDecodeCommand).NotifyCanExecuteChanged();
                    };
                }

                return stopDecodeCommand; 
            }
        }

        public Command ResetHardwareCommand
        {
            get
            {
                return new Command(new Command.ICommandOnExecute(ResetHardware));
            }
        }

        private ICommand loadOfflineCommand;

        public ICommand LoadOfflineCommand
        {
            get
            {
                if (loadOfflineCommand == null)
                {
                    loadOfflineCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(OfflineFileDecode, () =>
                    {
                        return IsCapturing == false && IsDecoding == false;
                    });

                    this.PropertyChanged += (s, e) =>
                    {
                        if (nameof(IsCapturing) == e.PropertyName || nameof(IsDecoding) == e.PropertyName)
                            ((CommunityToolkit.Mvvm.Input.RelayCommand)LoadOfflineCommand).NotifyCanExecuteChanged();
                    };
                }

                return loadOfflineCommand;
            }
        }

        public Command SaveCommand
        {
            get
            {
                return new Command(new Command.ICommandOnExecute(SaveDefaultConfiguration));
            }
        }

        public Command SaveAsCommand
        {
            get
            {
                return new Command(new Command.ICommandOnExecute(SaveAsConfiguration));
            }
        }

        public Command SaveCapture
        {
            get
            {
                return new Command(new Command.ICommandOnExecute(SaveTraceFile));
            }
        }

        public Command SaveAsCapture
        {
            get
            {
                return new Command(new Command.ICommandOnExecute(SaveAsTraceFileOption));
            }
        }

        public Command LoadCommand
        {
            get
            {
                return new Command(new Command.ICommandOnExecute(LoadConfiguration));
            }
        }

        public Command LoadDefaultCommand
        {
            get
            {
                return new Command(new Command.ICommandOnExecute(LoadDefaultConfiguration));
            }
        }

        public Command CloseAppCommand
        {
            get
            {
                return new Command(new Command.ICommandOnExecute(Close));
            }
        }

        public ICommand WindowSelection
        {
            get
            {
                return new RelayCommand<string>(ViewSelection);
            }
        }



        private bool isSaveCaptureEnable = false;
        private bool isSaveAsCaptureEnable = true;

        public bool IsSaveCaptureEnable
        {
            get { return isSaveCaptureEnable; }
            set { isSaveCaptureEnable = value; RaisePropertyChanged("IsSaveCaptureEnable"); }
        }

        public bool IsSaveAsCaptureEnable
        {
            get { return isSaveAsCaptureEnable; }
            set { isSaveAsCaptureEnable = value; RaisePropertyChanged("IsSaveAsCaptureEnable"); }
        }


        private bool isSaveSetupEnable = true;
        private bool isSaveAsSetupEnable = true;

        public bool IsSaveSetupEnable
        {
            get { return isSaveSetupEnable; }
            set { isSaveSetupEnable = value; RaisePropertyChanged("IsSaveSetupEnable"); }
        }

        public bool IsSaveAsSetupEnable
        {
            get { return isSaveAsSetupEnable; }
            set { isSaveAsSetupEnable = value; RaisePropertyChanged("IsSaveAsSetupEnable"); }
        }

        private string title = "PGY-LA-EMBD";
        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged("Title"); }
        }

        private string version = Application.Current.MainWindow.GetType().Assembly.GetName().Version.ToString();
        public string Version
        {
            get { return version; }
            set { version = value; RaisePropertyChanged("Version"); }
        }

        private int hwVerion = 0;
        public int HWVersion
        {
            get
            {
                return hwVerion;
            }
            set
            {
                hwVerion = value;
                RaisePropertyChanged("HWVersion");
            }
        }


        #endregion

        #region Methods
        void ViewSelection(string param)
        {
            if (param == "SearchView")
                analyzerViewModel.SearchFilterVM.SearchSelected = true;
            if (param == "FilterInView")
                analyzerViewModel.SearchFilterVM.FilterInSelected = true;
            if (param == "FilterOutView")
                analyzerViewModel.SearchFilterVM.FilterOutSelected = true;
            if (param == "SearchView" || param == "FilterInView" || param == "FilterOutView")
                param = "SearchFilterView";
            ActiveWindow = param; 
        }

        string activeWindow;
        public string ActiveWindow
        {
            get
            {
                return activeWindow;
            }
            set
            {
                activeWindow = value;
                RaisePropertyChanged("ActiveWindow");

                analyzerViewModel.DcTimingPlotView.IsActive = false;
                analyzerViewModel.DcTimingPlotView.IsActive = false;
                switch (activeWindow)
                {
                    case "L-View":
                        analyzerViewModel.DcTimingPlotView.IsActive = true;
                        break;
                    case "P-View":
                        break;
                    case "T-View":
                        analyzerViewModel.DcTimingPlotView.IsActive = true;
                        break;
                }
            }
        }

        private void SaveConfiguration(string filePath)
        {
            var serializer = new ConfigurationContainer().UseAutoFormatting()
                                                                .UseOptimizedNamespaces()
                                                                .EnableImplicitTyping(typeof(ConfigurationViewModel), typeof(TriggerViewModel))
                                                                .EnableReferences()
                                                                // Additional configurations...
                                                                .Create();

            // TODO: find an alternate to serialize
            using (var writer = new StreamWriter(filePath))
            {
                string strConfig = serializer.Serialize(analyzerViewModel.ConfigVM);

                writer.Write(strConfig);
            }
        }

        private void LoadConfiguration(string filePath)
        {
            var serializer = new ConfigurationContainer().UseAutoFormatting()
                                                                .UseOptimizedNamespaces()
                                                                .EnableImplicitTyping(typeof(ConfigurationViewModel))
                                                                .EnableReferences()
                                                                // Additional configurations...
                                                                .Create();

            ConfigurationViewModel loadedConfig = null;
            // TODO: find an alternate to serialize
            using (XmlReader reader = XmlReader.Create((new StreamReader(filePath))))
            {
                loadedConfig = (ConfigurationViewModel)serializer.Deserialize(reader);
            }

            analyzerViewModel.UpdateConfiguration(loadedConfig);
        }

        void SaveDefaultConfiguration()
        {
            try
            {
                SaveConfiguration(MiniCooperDirectoryInfo.DefaultSettingFile);

                MessageBox.Show("Configuration saved successfully to default path", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Failed to Save configuration", "Save configuration", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void LoadDefaultConfiguration()
        {
            try
            {
                LoadConfiguration(MiniCooperDirectoryInfo.DefaultSettingFile);

                MessageBox.Show("configuration loaded successfully", "Load configuration", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Failed to load default configuration", "Load configuration", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void LoadConfiguration()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
                openDialog.DefaultExt = ".xml";
                openDialog.Filter = "XML Configuration (.xml)|*.xml";
                openDialog.RestoreDirectory = true;
                openDialog.ShowDialog();
                if (!string.IsNullOrEmpty(openDialog.FileName))
                {
                    LoadConfiguration(openDialog.FileName);

                    MessageBox.Show("configuration loaded successfully", "Load configuration", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load configuration", "Load configuration", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void SaveTraceFile()
        {
            try
            {
                if (!string.IsNullOrEmpty(SessionConfiguration.TraceFilePath))
                {
                    string filePath = SessionConfiguration.TraceFilePath;
                    filePath += ".xml";

                    SaveConfiguration(filePath);

                    MessageBox.Show("Capture saved successfully", "Save Capture", MessageBoxButton.OK, MessageBoxImage.Information);

                    SessionConfiguration.IsSave = true;
                }
                else
                    MessageBox.Show("Captured data not found", "Save Capture", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to save capture", "Save Capture", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        bool saveas = false;
        void SaveAsTraceFileOption()
        {

            try
            {
                //if (!string.IsNullOrEmpty(SessionConfiguration.TraceFilePath))
                //{
                Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
                saveDialog.DefaultExt = ".xml";
                saveDialog.Filter = "XML Configuration (.xml)|*.xml";
                saveDialog.RestoreDirectory = true;
                string fileName = string.Empty;
                saveDialog.ShowDialog();
                if (!string.IsNullOrEmpty(saveDialog.FileName))
                {
                    Task.Factory.StartNew(() =>
                    {
                        fileName = saveDialog.FileName;

                        string folderPath = Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName);

                        if (!Directory.Exists(SessionConfiguration.TraceFilePath))
                        {
                            if (SessionConfiguration.TraceFilePath == null)
                            {
                                SessionConfiguration.TraceFilePath = folderPath;
                            }
                        }


                        var source = new DirectoryInfo(SessionConfiguration.TraceFilePath);

                        var target = new DirectoryInfo(folderPath);
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        foreach (FileInfo files in source.GetFiles())
                        {
                            files.CopyTo(Path.Combine(target.FullName, files.Name), true);
                        }

                        if (SessionConfiguration.SourceType == eSourceType.Live)
                        {
                            var msBoxResult = MessageBox.Show("Do you want to updates folder for subsequent live captures ?", "Update capture location", MessageBoxButton.YesNo, MessageBoxImage.Question);

                            if (msBoxResult == MessageBoxResult.Yes)
                            {
                                SessionConfiguration.TraceFilePath = folderPath;
                            }
                        }

                    }).Wait();

                    SaveConfiguration(fileName);

                    SessionConfiguration.IsSaveas = true;
                    SessionConfiguration.IsSaveasname = true;
                    SessionConfiguration.IsSave = false;
                }
                else
                {
                    MessageBox.Show("Failed to save capture", "Save Capture", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                //}
                //else
                //    System.Windows.Forms.MessageBox.Show("Captured data not found", "Save Capture", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                // System.Windows.Forms.MessageBox.Show("Failed to save capture", "Save Capture", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void SaveAsConfiguration()
        {
            try
            {
                Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
                saveDialog.DefaultExt = ".xml";
                saveDialog.Filter = "XML Configuration (.xml)|*.xml";
                saveDialog.RestoreDirectory = true;
                saveDialog.ShowDialog();
                if (!string.IsNullOrEmpty(saveDialog.FileName))
                {
                    SaveConfiguration(saveDialog.FileName);

                    MessageBox.Show("configuration saved successfully", "Save configuration", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to save configuration", "Save configuration", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to save configuration", "Save configuration", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        void OfflineFileDecode()
        {
            // TODO: remove this after testing
            //ResultViewModel_I2C.GetInstance().BorderVisibility = false;
            if (!SessionConfiguration.IsDecodeActive)
            {
                string filePath = string.Empty;
                string fileName = string.Empty;
                SessionConfiguration.ConnectionStatus = false;
                Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
                openDialog.DefaultExt = ".xml";
                openDialog.Filter = "XML Configuration (.xml)|*.xml";
                openDialog.RestoreDirectory = true;
                openDialog.ShowDialog();

                if (!string.IsNullOrEmpty(openDialog.FileName))
                {
                    fileName = openDialog.FileName;
                    filePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fileName), System.IO.Path.GetFileNameWithoutExtension(fileName));
                    if (Directory.Exists(filePath))
                    {
                        // All view model should be reset here -> which intern makes the models to reset
                        // Configuration reset - import from file
                        // plot reset
                        // results reset
                        // decoder reset
                        // waveform list reset
                        // ******View selection should happen based on config mode - LA / PA / LA-PA

                        analyzerViewModel.Reset();
                        hostInterface.Reset();

                        SessionConfiguration.SourceType = eSourceType.Offline;

                        //Load configuration .XML file and Desrialise
                        LoadConfiguration(fileName);

                        var configuration = analyzerViewModel.ConfigVM.Config;

                        StatisticsRepository.GetInstance().Initialize();

                        analyzerViewModel.Initialize();

                        Task.Run(() =>
                        {
                            if (hostInterface.Initialize_Offline(configuration, filePath) != eResponseFlag.Success)
                            {
                                MessageBox.Show("Decoding is still in progress. please wait..", "RD Trace File", MessageBoxButton.OK, MessageBoxImage.Information);
                                return;
                            }

                            hostInterface.StartRun();
                        });

                        if (configuration.ConfigurationMode == eConfigMode.LA_Mode)
                            ViewSelection("T-View");
                        else
                            ViewSelection("P-View");

                        IsDecoding = true;
                    }
                    else
                    {
                        MessageBox.Show("Dat file Folder did not exist !", "RD Trace File", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Decoding is still in progress. please wait..", "RD Trace File", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        bool forceStop = false;
        void testConnectionMethod()
        {
            //IsStartEnable = true;
            //IsStopEnable = false;
            //SessionConfiguration.ConnectionStatus = true;
            //return;
            StoppingDecoding = false;
            if (SessionConfiguration.IsDecodeActive)
            {
                var result = MessageBox.Show("Decoding is still in progress. \nDo you want to stop decoding and Start capture?", "Live capture", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                    return;
                else
                {
                    StopDecode();
                    StoppingDecoding = true;
                }
            }

            this.ConnectionProgress = true;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (StoppingDecoding)
                    {
                        Application.Current.Dispatcher.Invoke(() => analyzerViewModel.Status = "Stopping Analysis...");

                        while (SessionConfiguration.IsDecodeActive)
                            Thread.Sleep(100);

                        Application.Current.Dispatcher.Invoke(() => analyzerViewModel.Status = "Analysis Stopped.");
                        StoppingDecoding = false;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        analyzerViewModel.Reset();
                        hostInterface.Reset();

                        analyzerViewModel.Status = "Establishing connection with device..";

                        analyzerViewModel.Initialize();
                    });

                    if (hostInterface.Initialize_Online(analyzerViewModel.ConfigVM.Config) == eResponseFlag.Success)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            analyzerViewModel.Status = "Connection successful with device..";
                            IsConnected = true;
                        });

                        //IsStartEnable = true;
                        //IsStopEnable = false;
                        //IsDecodeStopEnable = false;
                    }
                    else
                        Application.Current.Dispatcher.Invoke(() => analyzerViewModel.Status = "Connection failed with device..");

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.IsConnected = true;
                        this.ConnectionProgress = false;
                        IsSaveAsCaptureEnable = true;
                        IsSaveCaptureEnable = false;
                        IsSaveSetupEnable = true;
                        IsSaveAsSetupEnable = true;
                        SessionConfiguration.IsDecodeActive = false;
                        SessionConfiguration.StopCapture = false;
                        SessionConfiguration.StopDecode = false;
                        SessionConfiguration.StopForcedDecode = false;
                    });
                }
                catch ( Exception ex )
                {
                    Application.Current.Dispatcher.Invoke(() => analyzerViewModel.Status = "Connection failed with device..");
                    this.ConnectionProgress = false;
                }
            });
        }

        bool isCapturing = false;
        bool isDecoding = false;

        public bool IsCapturing 
        { 
            get => isCapturing;
            set
            {
                isCapturing = value;
                RaisePropertyChanged(nameof(IsCapturing));
            }
        }

        public bool IsDecoding 
        { 
            get => isDecoding;
            set
            {
                isDecoding = value;

                RaisePropertyChanged(nameof(IsDecoding));
            }
        }

        bool stoppingDecoding = false;

        public bool StoppingDecoding
        {
            get => stoppingDecoding;
            set
            {
                stoppingDecoding = value;
                RaisePropertyChanged(nameof(StoppingDecoding));
            }
        }


        //bool isStartEnable = false;
        //public bool IsStartEnable
        //{
        //    get
        //    {
        //        return isStartEnable;
        //    }
        //    set
        //    {
        //        isStartEnable = value;
        //        isSaveAsCaptureEnable = !value;
        //        RaisePropertyChanged("IsStartEnable");
        //    }
        //}

        //bool isStopEnable = false;
        //public bool IsStopEnable
        //{
        //    get
        //    {
        //        return isStopEnable;
        //    }
        //    set
        //    {
        //        isStopEnable = value;
        //        RaisePropertyChanged("IsStopEnable");
        //    }
        //}



        //bool isDecodeStopEnable = false;
        //public bool IsDecodeStopEnable
        //{
        //    get
        //    {
        //        return isDecodeStopEnable;
        //    }
        //    set
        //    {
        //        isDecodeStopEnable = value;
        //        RaisePropertyChanged("IsDecodeStopEnable");
        //    }
        //}

        void runMethod()
        {
            eResponseFlag captureState = eResponseFlag.No_Result;

            try
            {
                IsCapturing = true;
                IsDecoding = true;

                ////IsStartEnable = false;

                //analyzerViewModel.Reset();
                //hostInterface.Reset();

                StatisticsRepository.GetInstance().Initialize();
                if (TriggerModel.GetInstance().TriggerType == eTriggerTypeList.Auto)
                    analyzerViewModel.Status = "Capturing Data..";
                else
                    analyzerViewModel.Status = "Waiting for trigger..";

                //analyzerViewModel.Initialize();

                //hostInterface.Initialize_Online(analyzerViewModel.ConfigVM.Config);

                //captureState = hostInterface.StartCapture(analyzerViewModel.ConfigVM.Trigger.GetTriggerBytes());
                Task.Run(hostInterface.StartRun);
                captureState = eResponseFlag.Success;

                //IsStopEnable = true;
                //IsDecodeStopEnable= true;
                IsSaveAsCaptureEnable = false;
                IsSaveCaptureEnable = false;
                IsSaveAsSetupEnable= false;
                IsSaveSetupEnable= false;
                if (analyzerViewModel.ConfigVM.Config.ConfigurationMode == eConfigMode.LA_Mode)
                    ActiveWindow = "T-View";
                else
                    ActiveWindow = "P-View";
            }
            catch (Exception ex)
            {
                //handle any error occur
                IsCapturing = false;
                IsDecoding = false;

                if (captureState == eResponseFlag.Success)
                    hostInterface.StopCapture();
            }
        }

     public   void StopDecode()
        {
            //IsStopEnable = false;
            StoppingDecoding = true;
        
            IsSaveAsCaptureEnable = true;
            IsSaveSetupEnable=true;
            IsSaveAsSetupEnable = true;
            IsSaveCaptureEnable = true;
            SessionConfiguration.IsAnimate = false;
            SessionConfiguration.StopCapture = true;
            SessionConfiguration.StopForcedDecode = true;
            //IsDecodeStopEnable = false;
        }


     public   void StopAcquisition()
        {
            try
            {
                Task.Run(hostInterface.StopRun);
                IsCapturing = false;
                IsConnected = false;
                //IsStopEnable = false;
                //IsDecodeStopEnable = true;
                IsSaveAsCaptureEnable = true;
                IsSaveCaptureEnable = true;
                IsSaveAsSetupEnable = true;
                IsSaveSetupEnable= true;
                Thread.Sleep(5000);
                if (SessionConfiguration.isBufferOverflow)
                    analyzerViewModel.Status = "Acquisition stopped.";
                else if(SessionConfiguration.IsDecodeActive)
                    analyzerViewModel.Status = "Acquisition stopped.";
                else
                    analyzerViewModel.Status = "Acquisition stopped.";
                SessionConfiguration.IsAnimate = false;
                SessionConfiguration.StopCapture = true;
            }
            catch (Exception ex)
            {
                IsCapturing = false;
            }
        }

        void ResetHardware()
        {
           var res = hostInterface.ResetHardwareDevice();
            if(res != eResponseFlag.Success)
                MessageBox.Show("Connection not establish with PGY Hardware..", "Reset", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Hardware reset successful..", "Reset", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public void CloseCaptureApp()
        {
            Process[] GetPArry = Process.GetProcesses();
            foreach (Process process in GetPArry)
            {
                string ProcessName = process.ProcessName;

                ProcessName = ProcessName.ToLower().Trim();
                if (ProcessName.CompareTo("prodigycapturetrayapplication") == 0)
                {
                    process.Kill();
                    break;
                }
            }
        }

        public event Action RequestClose;

        public virtual void Close()
        {
            try
            {
               // string fileName = MiniCooperDirectoryInfo.CurrentSettingsFile;
              //  var configurations = ConfigModel.GetInstance();
               // SerializeObject<ConfigModel>.SerializeData(configurations, fileName);
            }
            catch { }
            finally
            {
                //CloseCaptureApp();
                if (RequestClose != null)
                {
                    RequestClose();
                }
            }
        }

        #endregion
    }
}
