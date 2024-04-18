using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Structure.SPMIStructure
{
    public class SPMIPacketStructure
    {
        public SPMIPacketStructure()
        {
            AckNackType = eAcknowledgeType.NA;
        }
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

        eAcknowledgeType ackNackType;

        public bool HasAckNack
        {
            get
            {
                return hasAckNack;
            }

            set
            {
                hasAckNack = value;
            }
        }

        public eAcknowledgeType AckNackType
        {
            get
            {
                return ackNackType;
            }

            set
            {
                ackNackType = value;
            }
        }

        bool hasAckNack;

        eSPMICMDTYPE _cmdType;
        public eSPMICMDTYPE CmdType
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
