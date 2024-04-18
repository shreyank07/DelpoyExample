using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure
{
    [Serializable]
    public class HWPayload
    {
        public HWPayload()
        {
            symbolByte = new byte[4];
        }
        private double timeStamp;
        public double TimeStamp
        {
            get { return timeStamp; }
            set { timeStamp = value; }
        }

        private byte[] symbolByte;
        public byte[] SymbolByte
        {
            get { return symbolByte; }
            set { symbolByte = value; }
        }

        private byte channelChange;
        public byte ChannelChange
        {
            get
            {
                return channelChange;
            }
            set
            {
                channelChange = value;
            }
        }
    }
}
