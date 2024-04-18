using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Model
{
    public class ProtocolActivityHolder
    {
        public ProtocolActivityHolder()
        {

        }
        private int index;

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        private int sample;

        public int Sample
        {
            get { return sample; }
            set { sample = value; }
        }

        private double timestamp;

        public double Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        private eProtocol protocol;

        public eProtocol Protocol
        {
            get { return protocol; }
            set { protocol = value; }
        }

        private string protocolName;

        public string ProtocolName
        {
            get { return protocolName; }
            set { protocolName = value; }
        }

        private eErrorType error;

        public eErrorType Error
        {
            get { return error; }
            set { error = value; }
        }
    }
}
