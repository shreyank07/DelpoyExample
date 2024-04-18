using System;
using System.Linq;
using System.Threading.Tasks;
using PGYMiniCooper.DataModule.Model;
using Prodigy.Interfaces;
using Google.Protobuf.WellKnownTypes;
using PGYMiniCooper.DataModule.Model.Trigger_Config;
using System.Net.Sockets;
using Prodigy.Interfaces.I3C;

namespace PGYMiniCooper.DataModule.Service
{
    public class OnlineService : IProdigyService
    {
        string strLinkPacket =
               "00 FE B5 9C 8F 60 D4 BE D9 71 E2 FA 08 00 45 00 " +
               "04 24 01 00 40 00 01 88 F0 93 C0 A8 01 32 C0 A8 " +
               "01 3C 09 01 09 00 00 08 69 AF FF 02 00 24 00 00 " +
               "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 " +
               "02 04";

        string triggerPacket =
                "00 FE B5 9C 8F 60 D4 BE D9 71 E2 FA 08 00 45 00 " +
                "04 24 01 00 40 00 01 88 F0 93 C0 A8 01 32 C0 A8 " +
                "01 3C 09 01 09 00 00 08 69 AF 00 11 00 00 00 00 " +
                "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 " +
                "C0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 " +
                "00";
        string headerPacket =
                "00 FE B5 9C 8F 60 D4 BE D9 71 E2 FA 08 00 45 00 04 " +
                "24 01 00 40 00 01 88 F0 93 C0 A8 01 32 C0 A8 01 3C " +
                "09 01 09 00 00 08 69 AF 00 18 00 00 00 00 00 00 00  " +
                "00 00 00 00 00 00 00 00 00 00 00 00 00";

        public OnlineService(IApiService apiService)
        {
            apiService.OnEventReceived += ApiService_OnEventReceived;
            this.apiService = apiService;
        }

        private void ApiService_OnEventReceived(Google.Protobuf.IMessage message)
        {
            switch (message)
            {
                case SystemStatusUpdate statusUpdate:

                    systemStatus = statusUpdate.CurrentState;
                    break;
            }
        }

        public bool Initialize(ConfigModel configModel)
        {
            var task = InternalInitialize(configModel);

            task.Wait();

            return task.Result;
        }

        /// <summary>
        /// Initializes the communication and does run setup
        /// </summary>
        /// <param name="configModel"></param>
        /// <returns></returns>
        private async Task<bool> InternalInitialize(ConfigModel configModel)
        {
            // Initialized connection
            if (apiService.IsInitialized == false)
                apiService.Initialize();
            else
            {
                await apiService.Disconnect();
                systemStatus = null;
                await Task.Delay(500); // Disconnect previous session
            }

            // Setup session with user name
            apiService.Connect(userName);

            // Wait for server to be ready
            while (systemStatus.HasValue == false || systemStatus.Value != SystemStates.Ready)
                await Task.Delay(500);

            var strConfigValue = System.Configuration.ConfigurationManager.AppSettings["UseCompression"];
            bool useCompression = false;

            bool.TryParse(strConfigValue, out useCompression);

            // Setup configuration
            apiService.SendMessage(new Prodigy.Interfaces.SetupSystemMessage
            {
                CaptureServiceName = "CaptureApp1",
                UserName = userName,
                Configuration = new Prodigy.Interfaces.ProcessConfiguration { UseCompression = useCompression, RawDataProcessorConcurrency = 1, WaveformProcessorConcurrency = 1, EdgeProcessorConcurrency = 1 }
            });

            // Wait for setup to complete
            while (systemStatus != SystemStates.Setup)
                await Task.Delay(500);

            var deviceList = await apiService.RequestMessageAsync<GetDeviceListMessage, DeviceListResponse>(new GetDeviceListMessage(), cancellation: System.Threading.CancellationToken.None);

            if (deviceList == null || deviceList.Devices == null || deviceList.Devices.Count == 0)
                throw new Exception("No device connected");

            var selectedDevice = deviceList.Devices[0];
            if (deviceList.Devices.Count > 1)
            {
                // ask user to do selection
                selectedDevice = deviceList.Devices[1];
            }

            var runConfiguration = new RunConfiguration();
            runConfiguration.SelectedDevice = selectedDevice;
            var channels = configModel.ProtocolConfigList.SelectMany(c => c.Channels).Distinct().Select(c => (Prodigy.Interfaces.Channels)c.ChannelIndex);
         
            runConfiguration.ConnectedChannels.AddRange(channels);
            runConfiguration.SelectedSampleRate= (SampleRate)configModel.SampleRateLAPA;
            var VolatgeConfig = new VolatgeSelectedForChannel();
            VolatgeConfig.SingnalAMpCH1CH2 = (VoltageList)configModel.SignalAmpCh1_2;
            VolatgeConfig.SingnalAMpCH3CH4 = (VoltageList)configModel.SignalAmpCh3_4;
            VolatgeConfig.SingnalAMpCH5CH6 = (VoltageList)configModel.SignalAmpCh5_6;
            VolatgeConfig.SingnalAMpCH7CH8= (VoltageList)configModel.SignalAmpCh7_8;
            VolatgeConfig.SingnalAMpCH9CH10 = (VoltageList)configModel.SignalAmpCh9_10;
            VolatgeConfig.SingnalAMpCH11CH12= (VoltageList)configModel.SignalAmpCh11_12;
            VolatgeConfig.SingnalAMpCH13CH14 = (VoltageList)configModel.SignalAmpCh13_14;
            VolatgeConfig.SingnalAMpCH15CH16 = (VoltageList)configModel.SignalAmpCh15_16;
            runConfiguration.SelectedVoltage = VolatgeConfig;
         

            foreach (var protocolConfig in configModel.ProtocolConfigList)
            {
                switch (protocolConfig)
                {
                    case ConfigModel_I3C configModel_I3C:

                        var config = new Prodigy.Interfaces.I3C.Configuration_I3C { };
                        config.Id = 1;
                        config.ChannelIndexSCL = (Prodigy.Interfaces.Channels)configModel_I3C.ChannelIndex_SCL;
                        config.ChannelIndexSDA.Add((Prodigy.Interfaces.Channels)configModel_I3C.ChannelIndex_SDA);

                        runConfiguration.ProtocolConfigurations.Add(new ProtocolConfiguration
                        {
                            ProtocolType = "I3C",
                            ProtocolName = configModel_I3C.Name,
                            Configuration = Any.Pack(config)
                        });

                        break;
                        
                }
            }
            if (configModel.Trigger.SelectedTrigger is TriggerConfig_I3C triggerConfig_I3)
            {
                var TriggerConfig = new Prodigy.Interfaces.I3C.ProtocolTriggerConfig_I3C { };
                TriggerConfig.ProtocolName = "I3C__1";

                switch (triggerConfig_I3.SelI3CMessage)
                {
                    case eI3CMessage.Broadcast:
                        var Broadcasteconfig = new Prodigy.Interfaces.I3C.BroadcastTriggerCommand();
                        Broadcasteconfig.AckType = (Prodigy.Interfaces.I3C.AcknowledgeType)triggerConfig_I3.AckNckI3C;
                        Broadcasteconfig.BroadcastCommandType = (Prodigy.Interfaces.I3C.FrameType)triggerConfig_I3.SelCommandBrd;
                        Broadcasteconfig.HDRCommand = triggerConfig_I3.HdrCommand;
                        Broadcasteconfig.Data = triggerConfig_I3.I3CData;
                        TriggerConfig.Broadcast = new BroadcastTriggerCommand();
                        TriggerConfig.Broadcast = Broadcasteconfig;

                        break;
                    case eI3CMessage.Directed:
                        var DirectedConfig = new Prodigy.Interfaces.I3C.DirectedTriggerCommand();
                        DirectedConfig.AckType = (Prodigy.Interfaces.I3C.AcknowledgeType)triggerConfig_I3.AckNckI3C;
                        DirectedConfig.DirectedCommandType = (Prodigy.Interfaces.I3C.FrameType)triggerConfig_I3.SelCommandDir;
                        DirectedConfig.SlaveAdress = triggerConfig_I3.I3CSlaveAddress;
                        DirectedConfig.SlaveTransferType = (Prodigy.Interfaces.I3C.TransferType)triggerConfig_I3.I3CSlaveTransfer;
                        DirectedConfig.SlaveAck = (Prodigy.Interfaces.I3C.AcknowledgeType)triggerConfig_I3.I3CSlaveAck;
                        DirectedConfig.Data = triggerConfig_I3.I3CData;
                        TriggerConfig.Directed = new DirectedTriggerCommand();
                        TriggerConfig.Directed = DirectedConfig;
                        break;
                    case eI3CMessage.Private:
                        var privateConfig = new Prodigy.Interfaces.I3C.PrivateTriggerCommand();
                        privateConfig.SlaveAdress = triggerConfig_I3.I3CSlaveAddress;
                        privateConfig.SlaveTransferType = (Prodigy.Interfaces.I3C.TransferType)triggerConfig_I3.I3CSlaveTransfer;
                        privateConfig.SlaveAck = (Prodigy.Interfaces.I3C.AcknowledgeType)triggerConfig_I3.I3CSlaveAck;
                        if (triggerConfig_I3.I3CData != " ")
                            privateConfig.Data = triggerConfig_I3.I3CData;
                        TriggerConfig.Private = new PrivateTriggerCommand();
                        TriggerConfig.Private = privateConfig;
                        break;


                }




                runConfiguration.TriggerConfiguration = new TriggerConfiguration
                {
                    ProtocolType = "I3C",
                    TriggerType = (TriggerType)configModel.Trigger.TriggerType,
                    Trigger = Any.Pack(TriggerConfig),

                };

            }
            // Initialize the run with run information
            apiService.SendMessage(new InitializeRunMessage { UserName = userName, RunId = 1, Configuration = runConfiguration });

            // Wait for initialize to complete
            while (systemStatus != SystemStates.InitializeRun)
                await Task.Delay(500);

            return true;
        }

        byte[] cByte;
        int i = 1; string FileName = null;
        private SystemStates? systemStatus;
        private string userName = "LALive";
        private readonly IApiService apiService;

        /// <summary>
        /// Start the run
        /// </summary>
        public void StartRun()
        {
            apiService.SendMessage(new Prodigy.Interfaces.StartRunMessage { RunId = 1 });
        }

        /// <summary>
        /// Stop the run
        /// </summary>
        public void StopRun()
        {
            apiService.SendMessage(new Prodigy.Interfaces.StopRunMessage { RunId = 1 });
        }

        public void Reset()
        {
            //var resetparameter = HardwarePacketFactory.GetHWResetbytes();
            //if (resetparameter != null)
            //{
            //    SendExerciserPacket(new List<byte[]> { resetparameter });
            //    return true;
            //}
            //return false;
        }

        //internal bool SendExerciserPacket(List<byte[]> packets)
        //{
        //    try
        //    {
        //        service = service ?? serviceClient.GetService();
        //        if (service == null)
        //            return false;
        //        //using (StreamWriter fs = new StreamWriter(@I3CDirectoryInfo.ErrorPath + @"\USB.txt", true))
        //        //{
        //        //    foreach (var packet in packets)
        //        //    {
        //        //        fs.WriteLine(String.Join("-", packet.Select(obj => obj.ToString("X2"))));
        //        //    }
        //        //    fs.Close();
        //        //}
        //        service.ExerciserPacket(packets);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

    }
}
