using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure
{
    public class PlotInformation
    {
        public string ChannelName { get; set; }
        public int ChannelValue { get; set; }
        public eProtocol Protocol { get; set; }
        public bool IsBusDiagram { get; set; }
    }

    public class TimingPlotInformation
    {
        public string ChannelName { get; set; }
        public int ChannelValue { get; set; }
        public string ChannelGroupName { get; set; }
        public string Parameter { get; set; }
        public string clockSource { get; set; }
        public bool IsBusDiagram { get; set; }
    }
}
