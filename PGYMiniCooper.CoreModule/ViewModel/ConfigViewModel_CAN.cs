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
using System.Windows;

namespace PGYMiniCooper.CoreModule.ViewModel
{

    public class ConfigViewModel_CAN : ViewModelBase, IConfigViewModel
    {
        private readonly ConfigModel_CAN model;

        /// <summary>
        /// Serialization only
        /// </summary>
        public ConfigViewModel_CAN():this(new ConfigModel_CAN()) { }

        public ConfigViewModel_CAN(ConfigModel_CAN model)
        {
            this.model = model;
        }

        public eProtocol ProtocolType => eProtocol.CAN;

        public eChannles ChannelIndex 
        {
            get
            {
                return model.ChannelIndex;
            }
            set
            {
                model.ChannelIndex = value;
                if (model.ChannelIndex != eChannles.CH1)
                {
                    MessageBox.Show("Channel 1 is reserved for CAN","CAN /CANFD");
                    model.ChannelIndex = eChannles.CH1;
                }
                RaisePropertyChanged(nameof(ChannelIndex));

            }
        }
      
        public eCANType CANType
        {
            get
            {
                return model.CANType;
            }
            set
            {
            
                model.CANType = value;
                if (model.CANType == eCANType.CAN)
                    BRS = false;
                RaisePropertyChanged(nameof(CANType));
            }
        }

        public bool BRS
        {
            get
            {
                return model.BRS;
            }
            set
            {
                model.BRS = value;
                RaisePropertyChanged(nameof(BRS));
            }
        }
   
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

        public int BaudRateCANFD
        {
            get
            {
                return model.BaudRateCANFD;
            }
            set
            {
                model.BaudRateCANFD = value;
                RaisePropertyChanged(nameof(BaudRateCANFD));
            }
        }

        public int BaudRateBRS
        {
            get
            {
                return model.BaudRateBRS;
            }
            set
            {
                model.BaudRateBRS = value;
                RaisePropertyChanged(nameof(BaudRateBRS));
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
