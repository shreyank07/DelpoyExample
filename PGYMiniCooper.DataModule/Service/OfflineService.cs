using Google.Protobuf.WellKnownTypes;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using Prodigy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Service
{
    public class OfflineService : IProdigyService
    {
        private readonly IApiService apiService;
        private SystemStates? systemStatus;
        private string userName = "LA";

        public OfflineService(IApiService apiService) 
        {
            this.apiService = apiService;

            apiService.OnEventReceived += ApiService_OnEventReceived;
        }

        private void ApiService_OnEventReceived(Google.Protobuf.IMessage message)
        {
            switch(message)
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
                OfflineFilePath = SessionConfiguration.TraceFilePath,
                UserName = userName,
                Configuration = new Prodigy.Interfaces.ProcessConfiguration { UseCompression = useCompression, RawDataProcessorConcurrency = 1, WaveformProcessorConcurrency = 1, EdgeProcessorConcurrency = 1 }
            });

            // Wait for setup to complete
            while (systemStatus != SystemStates.Setup)
                await Task.Delay(500);

            var runConfiguration = new RunConfiguration();
            var channels = configModel.ProtocolConfigList.SelectMany(c => c.Channels).Distinct().Select(c => (Prodigy.Interfaces.Channels)c.ChannelIndex);
            runConfiguration.ConnectedChannels.AddRange(channels);

            foreach(var protocolConfig in configModel.ProtocolConfigList.OfType<ConfigModel_I3C>())
            {
                var config = new Prodigy.Interfaces.I3C.Configuration_I3C { };
                config.Id = 1;
                config.ChannelIndexSCL = (Prodigy.Interfaces.Channels)protocolConfig.ChannelIndex_SCL;
                config.ChannelIndexSDA.Add((Prodigy.Interfaces.Channels)protocolConfig.ChannelIndex_SDA);

                runConfiguration.ProtocolConfigurations.Add(new ProtocolConfiguration
                {
                    ProtocolType = "I3C",
                    ProtocolName = protocolConfig.Name,
                    Configuration = Any.Pack(config)
                });
            }

            // Initialize the run with run information
            apiService.SendMessage(new InitializeRunMessage { UserName = userName, RunId = 1, Configuration = runConfiguration });

            // Wait for initialize to complete
            while (systemStatus != SystemStates.InitializeRun)
                await Task.Delay(500);

            return true;
        }

        public void Reset()
        {
            apiService.Disconnect().Wait();
        }

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
    }
}
