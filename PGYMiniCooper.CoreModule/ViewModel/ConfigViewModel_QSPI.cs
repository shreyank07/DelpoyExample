using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure.QSPIStructure;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.CoreModule.ViewModel
{
  
    public class ConfigViewModel_QSPI : ViewModelBase, IConfigViewModel
    {
        private readonly ConfigModel_QSPI model;

        /// <summary>
        /// Serialization only
        /// </summary>
        public ConfigViewModel_QSPI():this(new ConfigModel_QSPI()) { }

        public ConfigViewModel_QSPI(ConfigModel_QSPI model)
        {
            this.model = model;
        }

        public eProtocol ProtocolType => eProtocol.QSPI;

        public eQSPIMode Mode
        {
            get
            {
                return model.Mode;
            }
            set
            {
                model.Mode = value;
                RaisePropertyChanged(nameof(Mode));
            }
        }
  
        public eQSPIPolarity Polarity
        {
            get
            {
                return model.Polarity;
            }
            set
            {
                model.Polarity = value;
                RaisePropertyChanged(nameof(Polarity));
            }
        }

        public eQSPIChipSelect ChipSelect
        {
            get
            {
                return model.ChipSelect;
            }
            set
            {
                model.ChipSelect = value;
                RaisePropertyChanged(nameof(ChipSelect));
            }
        }

        
        public eChannles ChannelIndex_D0
        {
            get
            {
                return model.ChannelIndex_D0;
            }
            set
            {
                model.ChannelIndex_D0 = value;
                RaisePropertyChanged(nameof(ChannelIndex_D0));
            }
        }

        public eQSPIPhase Phase
        {
            get
            {
                return model.Phase;
            }
            set
            {
                model.Phase = value;
                RaisePropertyChanged(nameof(Phase));
            }
        }


        public eQSPIAddress QSPIByteAddresssType
        {
            get
            {
                return model.QSPIByteAddresssType;
            }
            set
            {
                model.QSPIByteAddresssType = value;
                RaisePropertyChanged(nameof(QSPIByteAddresssType));
            }
        }

        public eChannles ChannelIndex_D1
        {
            get
            {
                return model.ChannelIndex_D1;
            }
            set
            {
                model.ChannelIndex_D1 = value;
                RaisePropertyChanged(nameof(ChannelIndex_D1));
            }
        }

        public eChannles ChannelIndex_D2
        {
            get
            {
                return model.ChannelIndex_D2;
            }
            set
            {
                model.ChannelIndex_D2 = value;
                RaisePropertyChanged(nameof(ChannelIndex_D2));
            }
        }

        public eChannles ChannelIndex_D3
        {
            get
            {
                return model.ChannelIndex_D3;
            }
            set
            {
                model.ChannelIndex_D3 = value;
                RaisePropertyChanged(nameof(ChannelIndex_D3));
            }
        }
       
        public eChannles ChannelIndex_CS
        {
            get
            {
                return model.ChannelIndex_CS;
            }
            set
            {
                model.ChannelIndex_CS = value;
                RaisePropertyChanged(nameof(ChannelIndex_CS));
            }
        }

        public eChannles ChannelIndex_CLK
        {
            get
            {
                return model.ChannelIndex_CLK;
            }
            set
            {
                model.ChannelIndex_CLK = value;
                RaisePropertyChanged(nameof(ChannelIndex_CLK));
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
