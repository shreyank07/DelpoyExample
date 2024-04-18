using PGYMiniCooper.DataModule.Interface;
using System.Collections.Generic;

namespace PGYMiniCooper.DataModule.Model
{
    public class ConfigModel_RFFE : IConfigModel
    {
        private readonly List<ChannelInfo> channels = new List<ChannelInfo> { new ChannelInfo("SCL", eChannles.None), new ChannelInfo("SDA", eChannles.None) };

        public string Name { get; set; }

        /// <summary>
        /// Only for serialzation
        /// </summary>
        public ConfigModel_RFFE() { }

        public ConfigModel_RFFE(string name)
        {
            this.Name = name;
        }

        public eChannles ChannelIndex_SCL
        {
            get { return channels[0].ChannelIndex; }
            set { channels[0].ChannelIndex = value; }
        }

        public eChannles ChannelIndex_SDA
        {
            get { return channels[1].ChannelIndex; }
            set { channels[1].ChannelIndex = value; }
        }

        public IReadOnlyList<ChannelInfo> Channels => channels;

    }
}
