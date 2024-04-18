using PGYMiniCooper.DataModule.Interface;
using System.Collections.Generic;

namespace PGYMiniCooper.DataModule.Model
{
    public class ConfigModel_QSPI : IConfigModel
    {
        private readonly List<ChannelInfo> channels = new List<ChannelInfo> {
            new ChannelInfo("CS", eChannles.None),
            new ChannelInfo("CLK", eChannles.None),
            new ChannelInfo("D0", eChannles.None),
            new ChannelInfo("D1", eChannles.None),
            new ChannelInfo("D2", eChannles.None),
            new ChannelInfo("D3", eChannles.None)
        };


        /// <summary>
        /// Only for serialzation
        /// </summary>
        public ConfigModel_QSPI() { }

        public ConfigModel_QSPI(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public eQSPIPolarity Polarity { get; set; } = eQSPIPolarity.Low;

        public eQSPIPhase Phase { get; set; } = eQSPIPhase.Low;

        public eQSPIChipSelect ChipSelect { get; set; } = eQSPIChipSelect.Low;

        public eQSPIAddress QSPIByteAddresssType { get; set; } = eQSPIAddress.ThreeByte;

        public bool Visibility { get; set; } = true;

        public eQSPITransferRate DataRate { get; set; } = eQSPITransferRate.STR;

        public eQSPIMode Mode { get; set; } = eQSPIMode.Extended;

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

        public eChannles ChannelIndex_D0
        {
            get { return channels[2].ChannelIndex; }
            set { channels[2].ChannelIndex = value; }
        }

        public eChannles ChannelIndex_D1
        {
            get { return channels[3].ChannelIndex; }
            set { channels[3].ChannelIndex = value; }
        }

        public eChannles ChannelIndex_D2
        {
            get { return channels[4].ChannelIndex; }
            set { channels[4].ChannelIndex = value; }
        }

        public eChannles ChannelIndex_D3
        {
            get { return channels[5].ChannelIndex; }
            set { channels[5].ChannelIndex = value; }
        }

        public IReadOnlyList<ChannelInfo> Channels => channels;
    }
}


    









