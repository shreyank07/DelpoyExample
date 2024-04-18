using PGYMiniCooper.DataModule.Structure.I2CStructure;
using ProdigyFramework.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Interface
{
    public interface IMessage
    {
        int Id { get; set; }

        double TimeStamp { get; set; }
        bool IsHdrExitFound { get; set; }

        int StartIndex
        {
            get;
            set;
        }

      
        int StopIndex { get; set; }


        eHostDevice HostDevice { get; set; }

        eProtocolMode ProtocolMode { get; set; }

        ePacketType PacketType { get; set; }

        eErrorType ErrorType { get; set; }


        double Frequency { get; set; }

        double StopTime { get; set; }

        bool Stop { get; set; }

        bool IsSelected { get; set; }

        bool HasAbort { get; set; }
    }
}
