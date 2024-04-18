using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure
{
    public class selection
    {
        public string Description { get; set; }
        public eFrameType QSPICommandType { get; set; }
        public eTransferType TransferType { get; set; }
    }


    public class CommandInfo
    {
        public bool IsDataLimited;
        public eTransferType TransferType;
        public byte DataCount; 
        public bool HasData;
        public eCommandPattern Pattern;
        public bool hasPECWithoutData;
        public bool hasMultiplePattern;
    }
}
