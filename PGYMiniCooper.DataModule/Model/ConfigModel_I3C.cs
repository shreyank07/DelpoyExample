using PGYMiniCooper.DataModule.Interface;
using System;
using System.Collections.Generic;

namespace PGYMiniCooper.DataModule.Model
{
    [Serializable]
    public class ConfigModel_I3C : IConfigModel
    {
        private readonly List<ChannelInfo> channels = new List<ChannelInfo> { new ChannelInfo("SCL", eChannles.None), new ChannelInfo("SDA", eChannles.None) };

        /// <summary>
        /// Only for serialization
        /// </summary>
        public ConfigModel_I3C() { }

        public ConfigModel_I3C(string name) 
        { 
            this.Name = name;
            this.channels[0].ChannelName = $"{name}_{channels[0].ChannelName}";
            this.channels[1].ChannelName = $"{name}_{channels[1].ChannelName}";
        }

        public string Name { get; set; }

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