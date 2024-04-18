using PGYMiniCooper.DataModule.Model;
using Prodigy.Business;
using System;
using System.Collections.Generic;
using System.Windows.Media;

using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PGYMiniCooper.DataModule.Interface
{
    public class ChannelInfo
    {
        public ChannelInfo() { }

        public ChannelInfo(string name, eChannles channel) 
        {
            ChannelName = name;
            ChannelIndex = channel;
        }

        private Color selColor = Colors.LightBlue;
        public Color SelColor
        {
            get
            {
                return selColor;
            }

            set
            {
                selColor = value;
               
            }
        }
        public string ChannelName { get; set; } = string.Empty;

        public eChannles ChannelIndex { get; set; } = eChannles.None;
    }

    public  interface IConfigModel
    {
        string Name { get; }

        IReadOnlyList<ChannelInfo> Channels { get; }
    }
}
