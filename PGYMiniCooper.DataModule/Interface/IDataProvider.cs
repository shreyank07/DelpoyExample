using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using System;
using System.Collections.Generic;

namespace PGYMiniCooper.DataModule.Interface
{
    public interface IDataProvider
    {
        event EventHandler<WaveformAvailableEventArgs> WaveformAvailable;
        event Action<IConfigModel, IResultModel> OnProtocolResultAdded;
        event EventHandler<MessageAvailableEventArgs> OnMessageAvailable;

        List<System.Windows.Point> GetWaveformPoints(eChannles channel, double startTime, double stopTime);

        List<DiscreteWaveForm> GetWaveformBufferTimestamp(double absStartTime, double absEndTime);

        void Initialize(ConfigModel configuration);

        void Reset();
        List<DiscreteWaveForm> GetWaveform(long startIndex, long stopIndex);
        List<IFrame> RequestFrames(string protocolName, int startIndex, int count);

        List<IFrame> RequestFrames(string protocolName, double startTime, double endTime);

        event Action<string> CaptureStatusUpdated;
        event Action OnTriggerOccur;
        event Action OnAquisitionStopped;
        event Action<bool> OnDecodeCompleted;
        bool TriggerAlreadyfound { get; }

        int GetFrameCount(string protocolName);
    }

    public class WaveformAvailableUpdate
    {
        public WaveformAvailableUpdate(double startTime, double endTime, long maxEdges)
        {
            StartTime = startTime;
            EndTime = endTime;
            MaxEdges = maxEdges;
        }

        public double StartTime { get; private set; }

        public double EndTime { get; private set; }

        public long MaxEdges { get; private set; }
    }

    public class WaveformAvailableEventArgs : EventArgs
    {
        public WaveformAvailableEventArgs(WaveformAvailableUpdate old, WaveformAvailableUpdate @new)
        {
            Old = old;
            New = @new;
        }

        public WaveformAvailableUpdate Old { get; private set; }

        public WaveformAvailableUpdate New { get; private set; }
    }

    public class MessageAvailableEventArgs : EventArgs
    {
        public MessageAvailableEventArgs(string protocolName, long count)
        {
            ProtocolName = protocolName;
            Count = count;
        }

        public string ProtocolName { get; }
        public long Count { get; }
    }
}
