using PGYMiniCooper.DataModule.Interface;
using System;
using System.Collections.Generic;

namespace PGYMiniCooper.DataModule.Model
{
    [Serializable]
    public class ConfigModel_CAN : IConfigModel
    {
        private readonly List<ChannelInfo> channels = new List<ChannelInfo> { new ChannelInfo("CAN", eChannles.CH1) };

        /// <summary>
        /// Only for serialzation
        /// </summary>
        public ConfigModel_CAN() { }

        public ConfigModel_CAN(string name)
        {
            this.Name = name;
        }


        public string Name
        {
            get { return channels[0].ChannelName; }
            set { channels[0].ChannelName = value; }
        }

        public eChannles ChannelIndex 
        {
            get { return channels[0].ChannelIndex; }
            set { channels[0].ChannelIndex = value; }
        }

        public eCANType CANType { get; set; } = eCANType.CAN;

        public int BaudRate { get; set; } = 125;

        public int BaudRateCANFD { get; set; } = 1000;
        public bool BRS { get; set; } = false;

        public int BaudRateBRS { get; set; } = 2000;

        public IReadOnlyList<ChannelInfo> Channels => channels;
    }
}
