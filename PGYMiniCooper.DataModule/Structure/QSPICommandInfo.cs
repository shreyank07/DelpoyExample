using PGYMiniCooper.DataModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolCoreModule.Structure
{
    public class CMDLines
    {
        public byte CMDline;
        public byte Addrline;
        public byte Dataline;
        public int DummyClkCycles;
    }
    public class QSPICommandInfo
    {
        public QSPICommandInfo()
        {
            Dlines = new Dictionary<eQSPIMode, CMDLines>();
        }
        public eQSPICommands QSPICommands = eQSPICommands.Unknown;
        public int AddressByteCount;
        public int DummyCycles;
        public Dictionary<eQSPIMode, CMDLines> Dlines;
    }
}
