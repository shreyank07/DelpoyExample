using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.CoreModule.ViewModel
{

    public class ConfigViewModel_I3C : ViewModelBase, IConfigViewModel
    {
        private readonly ConfigModel_I3C model;

        /// <summary>
        /// Serialization only
        /// </summary>
        public ConfigViewModel_I3C() : this(new ConfigModel_I3C()) { }

        public ConfigViewModel_I3C(ConfigModel_I3C model)
        {
            this.model = model;
         
        }

        public eProtocol ProtocolType => eProtocol.I3C;

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
