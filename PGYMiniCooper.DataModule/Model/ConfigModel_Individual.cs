using PGYMiniCooper.DataModule.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Model
{

    [Serializable]
    public class ConfigModel_Individual : IConfigModel
    {
        private readonly List<ChannelInfo> channels = new List<ChannelInfo> { new ChannelInfo { ChannelName = "CH1" } };

        public string Name
        {
            get => Channels[0].ChannelName;
            set => Channels[0].ChannelName = value;
        }

        public IReadOnlyList<ChannelInfo> Channels
        {
            get
            {
                lock (channels) return channels.ToList();
            }
        }
    }
}

