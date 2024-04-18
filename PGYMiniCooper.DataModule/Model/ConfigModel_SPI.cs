using PGYMiniCooper.DataModule.Interface;
using System.Collections.Generic;

namespace PGYMiniCooper.DataModule.Model
{
    public class ConfigModel_SPI : IConfigModel
    {
        /// <summary>
        /// Only for seralization
        /// </summary>
        public ConfigModel_SPI() 
        {            
        }

        public ConfigModel_SPI(string name)
        {
            this.Name = name;
        }

        private readonly List<ChannelInfo> channels = new List<ChannelInfo> {
            new ChannelInfo("CS", eChannles.None),
            new ChannelInfo("CLK", eChannles.None),
            new ChannelInfo("MOSI", eChannles.None),
            new ChannelInfo("MISO", eChannles.None)
        };

        public string Name { get; set; }

        public eSPIPolarity Polarity { get; set; } = eSPIPolarity.Low;

        public eSPIPhase Phase { get; set; } = eSPIPhase.Low;

        public eChannles ChannelIndex_CS
        {
            get { return channels[0].ChannelIndex; }
            set { channels[0].ChannelIndex = value; }
        }

        public eChannles ChannelIndex_CLK
        {
            get { return channels[1].ChannelIndex; }
            set { channels[1].ChannelIndex = value; }
        }

        public eChannles ChannelIndex_MOSI
        {
            get { return channels[2].ChannelIndex; }
            set { channels[2].ChannelIndex = value; }
        }

        public eChannles ChannelIndex_MISO
        {
            get { return channels[3].ChannelIndex; }
            set { channels[3].ChannelIndex = value; }
        }

        public IReadOnlyList<ChannelInfo> Channels => channels;
    }
}
