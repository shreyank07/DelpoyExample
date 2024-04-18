

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
    public class ConfigViewModel_Individual : ViewModelBase, IConfigViewModel
    {
        private readonly ConfigModel_Individual model;

        /// <summary>
        /// Serialization only
        /// </summary>
        public ConfigViewModel_Individual():this(new ConfigModel_Individual()) { }

        public ConfigViewModel_Individual(ConfigModel_Individual model)
        {
            this.model = model;
        }

        public eProtocol ProtocolType => throw new NotImplementedException("Individual channel does not support Protocol type");

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

        public eChannles ChannelIndex
        {
            get { return model.Channels[0].ChannelIndex; }
            set
            {
                model.Channels[0].ChannelIndex = value;
                RaisePropertyChanged(nameof(ChannelIndex));
            }
        }

        public IReadOnlyList<ChannelInfo> Channels => model.Channels;

        public IConfigModel Model => model;
    }
}