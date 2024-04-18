using CommunityToolkit.Mvvm.DependencyInjection;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Service;
using PGYMiniCooper.DataModule.Structure;
using Prodigy.Business;
using Prodigy.Common.Extensions;
using Prodigy.Interfaces;
using Prodigy.Interfaces.WaveformTypes;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using static DataProviderExtension;

namespace PGYMiniCooper.DataModule.Model
{
    internal class DataProvider : IDataProvider
    {
        private readonly IApiService apiService;
        private ConfigModel configuration;

        public event Action<string> CaptureStatusUpdated;
        public event Action OnTriggerOccur;
        public event Action OnAquisitionStopped;
        public event Action<bool> OnDecodeCompleted;
        public bool TriggerAlreadyfound { get; set; } = false;

        private Dictionary<string, int> protocolFrameCountMap = new Dictionary<string, int>();

        public int GetFrameCount(string protocolName)
        {
            if (protocolFrameCountMap.TryGetValue(protocolName, out var frameCount))
                return frameCount;
            else
                return 0;
        }

        public event EventHandler<WaveformAvailableEventArgs> WaveformAvailable;
        public event Action<IConfigModel, IResultModel> OnProtocolResultAdded;
        public event EventHandler<MessageAvailableEventArgs> OnMessageAvailable;

        private WaveformAvailableUpdate currentUpdate = null;
        private readonly Dictionary<string, PublishMessageThroughTap<MessagesAvailableResponse>> protocolMessagesAllowedThroughTap = new Dictionary<string, PublishMessageThroughTap<MessagesAvailableResponse>>();

        public DataProvider(IApiService apiService)
        {
            this.apiService = apiService;

            // subscrive for the events
            apiService.OnEventReceived += ApiService_OnEventReceived;
        }

        private void RaiseMessageAvailable(Prodigy.Interfaces.MessagesAvailableResponse messagesAvailable)
        {
            OnMessageAvailable?.Invoke(this, new MessageAvailableEventArgs(messagesAvailable.ProtocolName, messagesAvailable.TotalMessages));
        }

        private void ApiService_OnEventReceived(Google.Protobuf.IMessage message)
        {
            switch (message)
            {
                case Prodigy.Interfaces.WaveformTypes.EdgesAvailableResponse edgeAvailable:
                    // Edges available -> For digital waveform this is sufficient
                    var newUpdate = new WaveformAvailableUpdate(edgeAvailable.StartTime, edgeAvailable.EndTime, edgeAvailable.ChannelEdgeAvailable.Max(c => c.Count));
                    WaveformAvailable?.Invoke(this, new WaveformAvailableEventArgs(currentUpdate, newUpdate));

                    // update the waveform info
                    currentUpdate = newUpdate;
                    break;

                case Prodigy.Interfaces.MessagesAvailableResponse messagesAvailable:
                    // Decoded message available
                    protocolFrameCountMap[messagesAvailable.ProtocolName] = (int)messagesAvailable.TotalMessages;
                    protocolMessagesAllowedThroughTap[messagesAvailable.ProtocolName].Invoke(messagesAvailable);
                    break;

                case Prodigy.Interfaces.SystemStatusUpdate statusUpdate:

                    if (statusUpdate.CurrentState == SystemStates.RunCompleted)
                        OnDecodeCompleted?.Invoke(false);

                    break;
                case Prodigy.Interfaces.HardwareStatus hardwareStatus:
                    //if (hardwareStatus.StatusType == HardwareStatusType.HardwareMemoryBufferIsFull)
                    //    break;
                    if (hardwareStatus.StatusType == HardwareStatusType.TriggerFound)
                    {
                        SessionConfiguration.TriggerTime = hardwareStatus.Timestamp;
                        //TriggerAlreadyfound = true;
                        OnTriggerOccur?.Invoke();
                        //TriggerAlreadyfound = false;

                    }
                    break;
            }
        }

        public List<DiscreteWaveForm> GetWaveform(long startIndex, long stopIndex)
        {
            try
            {
                var responseTask = apiService.RequestMessageAsync<RequestWavePoints, WavepointResponse>(new RequestWavePoints
                {
                    RequestType = WaveRequestType.Digital,

                    IndexBased = new IndexBasedDataSubset
                    {
                        Offset = startIndex,
                        Count = stopIndex - startIndex
                    }
                }, CancellationToken.None);

                responseTask.Wait();

                List<DiscreteWaveForm> points = new List<DiscreteWaveForm>();
                for (int i = 0; i < responseTask.Result.Digital.XValues.Count; i++)
                {
                    points.Add(new DiscreteWaveForm(responseTask.Result.Digital.XValues[i], (UInt16)responseTask.Result.Digital.YValues[i]));
                }
                return points;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<DiscreteWaveForm> GetWaveformBufferTimestamp(double absStartTime, double absEndTime)
        {
            var responseTask = apiService.RequestMessageAsync<RequestWavePoints, WavepointResponse>(new RequestWavePoints
            {
                RequestType = WaveRequestType.Digital,

                TimeBased = new TimeBasedDataSubset
                {
                    StartTime = absStartTime,
                    EndTime = absEndTime

                }
            }, CancellationToken.None);
            responseTask.Wait();

            List<DiscreteWaveForm> points = new List<DiscreteWaveForm>();
            for (int i = 0; i < responseTask.Result.Digital.XValues.Count; i++)
            {
                points.Add(new DiscreteWaveForm(responseTask.Result.Digital.XValues[i], (ushort)responseTask.Result.Digital.YValues[i]));
            }

            return points;
        }

        private ConcurrentDictionary<eChannles, CancellationTokenSource> channelRequests = new ConcurrentDictionary<eChannles, CancellationTokenSource>();

        public List<Point> GetWaveformPoints(eChannles channel, double referanceStartTime, double referenceStopTime)
        {
            if (channelRequests.TryRemove(channel, out var cancellationSource))
            {
                try { cancellationSource.Cancel(); }
                catch { /*Task cancelled exception*/ }
            }

            try
            {
                cancellationSource = new CancellationTokenSource(10000);
                channelRequests[channel] = cancellationSource;

                double startTime = (referanceStartTime / 1e9) + SessionConfiguration.TriggerTime;
                double stopTime = (referenceStopTime / 1e9) + SessionConfiguration.TriggerTime;

                if (startTime < 0 || stopTime < 0 || startTime == double.NaN || stopTime == double.NaN)
                    return new List<Point>();

                var responseTask = apiService.RequestMessageAsync<RequestEdges, EdgeResponse>(new RequestEdges
                {
                    Channel = (Channels)channel,

                    TimeBased = new TimeBasedDataSubset
                    {
                        StartTime = startTime,
                        EndTime = stopTime

                    }
                }, cancellationSource.Token);
                responseTask.Wait(cancellationSource.Token);

                channelRequests.TryRemove(channel, out _);

                var response = responseTask.Result.Edges;
                List<Point> points = new List<Point>();

                Func<int, int> getState = index => index % 2 == (response.FirstEdge == WaveEdgeType.Rise ? 0 : 1) ? 1 : 0;

                double prevState = response.FirstEdge == WaveEdgeType.Rise ? 0 : 1;
                // Add reference point
                points.Add(new System.Windows.Point((startTime - SessionConfiguration.TriggerTime) * 1e9, prevState));

                for (int i = 0; i < response.Edges.Count; i++)
                {
                    int currentState = getState(i);

                    // We dont need these points
                    if (response.Edges[i] < startTime)
                    {
                        prevState = currentState;

                        // Keep 1st point for reference
                        points[0] = new System.Windows.Point((response.Edges[i] - SessionConfiguration.TriggerTime) * 1e9, prevState);
                        continue;
                    }

                    // Add points only if state changes
                    if (prevState != currentState)
                    {
                        points.Add(new System.Windows.Point((response.Edges[i] - SessionConfiguration.TriggerTime) * 1e9, prevState));
                        points.Add(new System.Windows.Point((response.Edges[i] - SessionConfiguration.TriggerTime) * 1e9, currentState));
                    }

                    prevState = currentState;
                }
                points.Add(new System.Windows.Point((stopTime - SessionConfiguration.TriggerTime) * 1e9, prevState));

                return points;
            }
            catch
            {
                return new List<Point>();
            }
        }

        public void Initialize(ConfigModel configuration)
        {
            this.configuration = configuration;
            currentUpdate = null;

            if (configuration.ConfigurationMode != eConfigMode.LA_Mode)
            {
                var decoderScheduler = new System.Threading.Tasks.Schedulers.CurrentThreadTaskScheduler();

                foreach (var protocolConfig in configuration.ProtocolConfigList)
                {
                    IResultModel result = null;
                    switch (protocolConfig)
                    {
                        case ConfigModel_CAN configModel_CAN:
                            result = new ResultModel_CAN();
                            break;
                        case ConfigModel_I2C configModel_I2C:
                            result = new ResultModel_I2C();
                            break;
                        case ConfigModel_I3C configModel_I3C:
                            result = new ResultModel_I3C();
                            break;
                        case ConfigModel_QSPI configModel_QSPI:
                            result = new ResultModel_QSPI();
                            break;
                        case ConfigModel_RFFE configModel_RFFE:
                            result = new ResultModel_RFFE();
                            break;
                        case ConfigModel_SPI configModel_SPI:
                            result = new ResultModel_SPI();
                            break;
                        case ConfigModel_SPMI configModel_SPMI:
                            result = new ResultModel_SPMI();
                            break;
                        case ConfigModel_UART configModel_UART:
                            result = new ResultModel_UART();
                            break;
                    }

                    if (result != null)
                    {
                        protocolFrameCountMap[protocolConfig.Name] = 0;
                        protocolMessagesAllowedThroughTap[protocolConfig.Name] = TappedMessenger.Create<MessagesAvailableResponse>(1, TimeSpan.FromSeconds(5), RaiseMessageAvailable);

                        // Publish added result, so that results view model get to know the result for each protocol
                        OnProtocolResultAdded?.Invoke(protocolConfig, result);
                    }
                }
            }
        }

        public void Reset()
        {
            protocolFrameCountMap.Clear();
            protocolMessagesAllowedThroughTap.Clear();
        }

        public List<IFrame> RequestFrames(string protocolName, int startIndex, int count)
        {
            MessagesResponse messageResponse = null;

            while (true)
            {
                try
                {
                    messageResponse = apiService.RequestMessage<RequestMessages, MessagesResponse>(new RequestMessages
                    {
                        ProtocolName = protocolName,
                        IndexBased = new IndexBasedDataSubset
                        {
                            Offset = startIndex,
                            Count = count
                        }
                    }, CancellationToken.None);

                    if (messageResponse == null)
                        throw new Exception("No data received.");

                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());

                    Thread.Sleep(1000);
                }
            }

            var config = configuration.ProtocolConfigList.First(p => p.Name == protocolName);
            List<IFrame> frames = new List<IFrame>();
            foreach (var protoFrame in messageResponse.Messages.Select(m => m.Unpack<ProtocolFrame>()))
            {
                System.Diagnostics.Debug.WriteLine(protoFrame.ToString());

                IFrame frame = null;
                switch (config)
                {
                    case ConfigModel_I3C _:
                        frame = ToFrame_I3C(protoFrame);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                if (frame != null)
                {
                    frame.ProtocolName = protocolName;

                    frames.Add(frame);
                }
            }

            return frames;

        }

        private ConcurrentDictionary<string, CancellationTokenSource> protocolFrameRequests = new ConcurrentDictionary<string, CancellationTokenSource>();

        public List<IFrame> RequestFrames(string protocolName, double startTime, double endTime)
        {
            if (protocolFrameRequests.TryRemove(protocolName, out var cancellationSource))
            {
                try { cancellationSource.Cancel(); }
                catch { /*Task cancelled exception*/ }
            }

            MessagesResponse messageResponse = null;

            try
            {
                cancellationSource = new CancellationTokenSource(5000);
                protocolFrameRequests[protocolName] = cancellationSource;

                messageResponse = apiService.RequestMessage<RequestMessages, MessagesResponse>(new RequestMessages
                {
                    ProtocolName = protocolName,
                    TimeBased = new TimeBasedDataSubset
                    {
                        StartTime = startTime,
                        EndTime = endTime
                    }
                }, cancellationSource.Token);

                protocolFrameRequests.TryRemove(protocolName, out _);

                if (messageResponse == null)
                    throw new Exception("No data received.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());

                return new List<IFrame>();
            }

            var config = configuration.ProtocolConfigList.First(p => p.Name == protocolName);
            List<IFrame> frames = new List<IFrame>();
            foreach (var protoFrame in messageResponse.Messages.Select(m => m.Unpack<ProtocolFrame>()))
            {
                System.Diagnostics.Debug.WriteLine(protoFrame.ToString());

                IFrame frame = null;
                switch (config)
                {
                    case ConfigModel_I3C _:
                        frame = ToFrame_I3C(protoFrame);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                if (frame != null)
                {
                    frame.ProtocolName = protocolName;

                    frames.Add(frame);
                }
            }

            return frames;

        }
    }
}
