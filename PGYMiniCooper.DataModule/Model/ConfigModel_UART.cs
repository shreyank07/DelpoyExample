using PGYMiniCooper.DataModule.Interface;
using System.Collections.Generic;

namespace PGYMiniCooper.DataModule.Model
{
    public class ConfigModel_UART : IConfigModel
    {
        private readonly List<ChannelInfo> channels = new List<ChannelInfo> { new ChannelInfo("TX", eChannles.None), new ChannelInfo("RX", eChannles.None) };

        /// <summary>
        /// Only for serialzation
        /// </summary>
        public ConfigModel_UART() { }

        public ConfigModel_UART(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public eChannles ChannelIndex_TX
        {
            get { return channels[0].ChannelIndex; }
            set { channels[0].ChannelIndex = value; }
        }

        public eChannles ChannelIndex_RX
        {
            get { return channels[1].ChannelIndex; }
            set { channels[1].ChannelIndex = value; }
        }

        public int BaudRate { get; set; } = 300;

        public int DataWidth { get; set; } = 8;

        public int StopBits { get; set; } = 1;

        public eBitOrder BitOrder { get; set; } = eBitOrder.MSBFirst;

        public bool ParitySelected { get; set; } = false;

        public eParity Parity { get; set; } = eParity.Odd;

        public IReadOnlyList<ChannelInfo> Channels => channels;

    }
}
