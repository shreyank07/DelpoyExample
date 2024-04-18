using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Structure.I3CStructure;
using Prodigy.WaveformControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.CoreModule.ViewModel.Interfaces
{
    public interface IResultViewModel
    {
        event Action<IResultViewModel> OnSelectionChanged;

        IConfigViewModel Config { get; }

        int TriggerPacketIndex { get; }

        ICollection<IFrame> ResultCollection { get; }

        IFrame SelectedFrame { get; set; }

        List<ChannelInfo> AvailableBusChannels { get; }

        IEnumerable<IBusData> GetBusDiagram(ChannelInfo channel, double startTime, double endTime);

        void Serach();

        void Reset();
    }
}
