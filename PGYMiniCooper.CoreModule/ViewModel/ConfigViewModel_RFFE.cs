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
    public class ConfigViewModel_RFFE : ViewModelBase, IConfigViewModel
    {
        private readonly ConfigModel_RFFE model;

        /// <summary>
        /// Serialization only
        /// </summary>
        public ConfigViewModel_RFFE():this(new ConfigModel_RFFE()) { }

        public ConfigViewModel_RFFE(ConfigModel_RFFE model)
        {
            this.model = model;
        }

        public eProtocol ProtocolType => eProtocol.RFFE;

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

        public IConfigModel Model => model;

        IReadOnlyList<ChannelInfo> IConfigViewModel.Channels => model.Channels;
    }
}
