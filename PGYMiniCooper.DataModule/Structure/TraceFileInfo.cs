using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure
{
    public class TraceFileInfo
    {
        public long startWfmIndex;
        public long stopWfmIndex;
        public int startoffset;
        public int stopoffset;
        public ulong msbTime;
        public int fileNumber;
        public double startTime;
        public double stopTime;
        public int PrevMSBTime;
        public int MSBCounter;
    }
}
