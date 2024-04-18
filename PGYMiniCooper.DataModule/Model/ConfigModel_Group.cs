using System;
using System.Collections.Generic;
using System.Linq;

namespace PGYMiniCooper.DataModule.Interface
{
    [Serializable]
    public class ConfigModel_Group : IConfigModel
    {
        private readonly List<ChannelInfo> channels = new List<ChannelInfo>();

        public string Name { get; set; }

        public IReadOnlyList<ChannelInfo> Channels 
        {
            get
            {
                lock (channels) return channels.ToList();
            }
        }

        public void AddChannel(string name,  eChannles channle)
        {
            lock (channels)
            {
                channels.Add(new ChannelInfo(name, channle));
            }
        }

        public void AddChannel(ChannelInfo channel)
        {
            lock (channels)
            {
                channels.Add(channel);
            }
        }

        public void RemoveChannel(string name)
        {
            lock (channels)
            {
                channels.RemoveAll(c => c.ChannelName == name);
            }
        }

        public void CleanChannels()
        {
            channels.Clear();
        }
    }
}
