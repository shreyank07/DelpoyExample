using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Interface
{
    public interface IFrame
    {
     
        double StartTime { get; set; }
         
        eProtocol ProtocolType{ get; set; }

        string ProtocolName { get; set; }

        double StopTime { get; set; }

        eErrorType ErrorType { get; set; }
        double Frequency { get; set; }
        int FrameIndex { get; set; }

        bool IsHighlighted { get; set; }
    }
}
