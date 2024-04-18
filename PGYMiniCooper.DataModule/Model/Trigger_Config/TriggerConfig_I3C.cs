using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PGYMiniCooper.DataModule.Interfaces;
using System.Windows.Markup;

namespace PGYMiniCooper.DataModule.Model.Trigger_Config
{
    public class TriggerConfig_I3C : CommunityToolkit.Mvvm.ComponentModel.ObservableObject, ITriggerConfigViewModel
    {
        public TriggerConfig_I3C() { }

        #region I3C properties
        private eAcknowledgeType ackNckI3C = eAcknowledgeType.NACK;
        public eAcknowledgeType AckNckI3C
        {
            get
            {
                return ackNckI3C;
            }
            set
            {
                ackNckI3C = value;
                OnPropertyChanged("AckNckI3C");
            }
        }



        private eBroadcastCCC selCommandBrd = eBroadcastCCC.RSTDAA;

        public eBroadcastCCC SelCommandBrd
        {
            get
            {
                return selCommandBrd;
            }
            set
            {
                selCommandBrd = value;
                OnPropertyChanged("SelCommandBrd");
            }
        }

        private eDirectedCCC selCommandDir = eDirectedCCC.RSTDAA;

        public eDirectedCCC SelCommandDir
        {
            get
            {
                return selCommandDir;
            }
            set
            {
                selCommandDir = value;
                OnPropertyChanged("SelCommandDir");
            }
        }

        private string I3CslaveAddress;
        public string I3CSlaveAddress
        {
            get
            {
                return I3CslaveAddress;
            }

            set
            {
                I3CslaveAddress = value;
                OnPropertyChanged("I3CSlaveAddress");
            }
        }

        private eTransferType I3CslaveTransfer = eTransferType.WR;
        public eTransferType I3CSlaveTransfer
        {
            get
            {
                return I3CslaveTransfer;
            }

            set
            {
                I3CslaveTransfer = value;
                OnPropertyChanged("I3CSlaveTransfer");
            }
        }

        private eAcknowledgeType I3CslaveAck = eAcknowledgeType.NACK;
        public eAcknowledgeType I3CSlaveAck
        {
            get
            {
                return I3CslaveAck;
            }

            set
            {
                I3CslaveAck = value;
                OnPropertyChanged("I3CSlaveAck");
            }
        }

        private eI3CMessage selI3CMessage;

        public eI3CMessage SelI3CMessage
        {
            get
            {
                return selI3CMessage;
            }
            set
            {
                selI3CMessage = value;
                OnPropertyChanged("SelI3CMessage");
            }
        }

        private string _I3Cdata;
        public string I3CData
        {
            get
            {
                return _I3Cdata;
            }
            set
            {
                _I3Cdata = value;
                OnPropertyChanged("I3CData");
            }
        }
        string hdrCommand;
        public string HdrCommand
        {
            get
            {
                return hdrCommand;
            }

            set
            {
                hdrCommand = value;
                OnPropertyChanged("HdrCommand");
            }
        }

        string hdrData;
        public string HdrData
        {
            get
            {
                return hdrData;
            }

            set
            {
                hdrData = value;
                OnPropertyChanged("HdrData");
            }
        }
        #endregion


       
    }
}
