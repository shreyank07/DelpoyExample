using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure.RFFEStructure
{
    public class RFFEPacketStructure
    {
        byte _value;
        public byte Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        byte parity;
        public byte Parity
        {
            get
            {
                return parity;
            }
            set
            {
                parity = value;
            }
        }

        bool hasBP;
        public bool HasBP
        {
            get
            {
                return hasBP;
            }
            set
            {
                hasBP = value;
            }
        }

        eRFFECMDTYPE _cmdType;
        public eRFFECMDTYPE CmdType
        {
            get
            {
                return _cmdType;
            }
            set
            {
                _cmdType = value;
            }
        }
    }
}
