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
    public class ConfigViewModel_SPMI : ViewModelBase, IConfigViewModel
    {
        private readonly ConfigModel_SPMI model;

        /// <summary>
        /// Serialization only
        /// </summary>
        public ConfigViewModel_SPMI():this(new ConfigModel_SPMI()) { }

        public ConfigViewModel_SPMI(ConfigModel_SPMI model)
        {
            this.model = model;
        }

        public eProtocol ProtocolType => eProtocol.SPMI;

        public eChannles ChannelIndex_SCL
        {
            get
            {
                return model.ChannelIndex_SCL;
            }
            set
            {
                model.ChannelIndex_SCL = value;
                RaisePropertyChanged(nameof(ChannelIndex_SCL));
            }
        }
    
        public eChannles ChannelIndex_SDA
        {
            get
            {
                return model.ChannelIndex_SDA;
            }
            set
            {
                model.ChannelIndex_SDA = value;
                RaisePropertyChanged(nameof(ChannelIndex_SDA));
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

        public eVersion SPMIVersion
        {
            get
            {
                return model.SPMIVersion;
            }
            set
            {
                model.SPMIVersion = value;
                RaisePropertyChanged(nameof(SPMIVersion));
            }
        }

        public IConfigModel Model => model;

        IReadOnlyList<ChannelInfo> IConfigViewModel.Channels => model.Channels;
    }
}
