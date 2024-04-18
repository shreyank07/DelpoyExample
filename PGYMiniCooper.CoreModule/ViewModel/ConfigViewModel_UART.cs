using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PGYMiniCooper.CoreModule.ViewModel
{
    public class ConfigViewModel_UART : ViewModelBase, IConfigViewModel
    {
        private readonly ConfigModel_UART model;

        /// <summary>
        /// Serialization only use
        /// </summary>
        public ConfigViewModel_UART() : this(new ConfigModel_UART()) { }

        public ConfigViewModel_UART(ConfigModel_UART model)
        {
            this.model = model;
        }

        public eProtocol ProtocolType => eProtocol.UART;

        public int BaudRate
        {
            get
            {
                return model.BaudRate;
            }
            set
            {
                model.BaudRate = value;
                RaisePropertyChanged(nameof(BaudRate));
            }
        }

        public int StopBits
        {
            get
            {
                return model.StopBits;
            }
            set
            {
                model.StopBits = value;
                RaisePropertyChanged(nameof(StopBits));
            }
        }

        public int DataWidth
        {
            get
            {
                return model.DataWidth;
            }
            set
            {
                model.DataWidth = value;
                RaisePropertyChanged(nameof(DataWidth));
            }
        }

        public bool ParitySelected
        {
            get { return model.ParitySelected; }
            set
            {
                model.ParitySelected = value;
                RaisePropertyChanged(nameof(ParitySelected));
            }
        }

        public eParity Parity
        {
            get { return model.Parity; }
            set
            {
                model.Parity = value;
                RaisePropertyChanged(nameof(Parity));
            }
        }

        public eBitOrder BitOrder
        {
            get { return model.BitOrder; }
            set
            {
                model.BitOrder = value;
                RaisePropertyChanged(nameof(BitOrder));
            }
        }

        public eChannles ChannelIndex_TX
        {
            get
            {
                return model.ChannelIndex_TX;
            }
            set
            {
                model.ChannelIndex_TX = value;
                RaisePropertyChanged(nameof(ChannelIndex_TX));
            }
        }

        public eChannles ChannelIndex_RX
        {
            get
            {
                return model.ChannelIndex_RX;
            }
            set
            {
                model.ChannelIndex_RX = value;
                RaisePropertyChanged(nameof(ChannelIndex_RX));
            }
        }

        public string Name
        {
            get
            {
                return model.Name;
            }
            set
            {
                model.Name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        public IConfigModel Model => model;

        IReadOnlyList<ChannelInfo> IConfigViewModel.Channels => model.Channels;
    }
}
