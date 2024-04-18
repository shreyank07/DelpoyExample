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
  
    public class ConfigViewModel_SPI : ViewModelBase, IConfigViewModel
    {
        private readonly ConfigModel_SPI model;

        /// <summary>
        /// Serialization only
        /// </summary>
        public ConfigViewModel_SPI():this(new ConfigModel_SPI()) { }    

        public ConfigViewModel_SPI(ConfigModel_SPI model)
        {
            this.model = model;
        }

        public eProtocol ProtocolType => eProtocol.SPI;

        public eSPIPolarity Polarity
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

        public eSPIPhase Phase
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
      
        public eChannles ChannelIndex_MISO
        {
            get
            {
                return model.ChannelIndex_MISO;
            }
            set
            {
                model.ChannelIndex_MISO = value;
                RaisePropertyChanged(nameof(ChannelIndex_MISO));
            }
        }
      
        public eChannles ChannelIndex_MOSI
        {
            get
            {
                return model.ChannelIndex_MOSI;
            }
            set
            {
                model.ChannelIndex_MOSI = value;
                RaisePropertyChanged(nameof(ChannelIndex_MOSI));
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
