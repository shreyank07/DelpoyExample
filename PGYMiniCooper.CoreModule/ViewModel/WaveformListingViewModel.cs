using CommunityToolkit.Mvvm.DependencyInjection;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using ProdigyFramework.Behavior;
using ProdigyFramework.Collections;
using ProdigyFramework.ComponentModel;
using ProdigyFramework.Converters;
using ProdigyFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PGYMiniCooper.CoreModule.ViewModel
{
    public class WaveformListingViewModel : ViewModelBase, IResetViewModel
    {
        public WaveformListingViewModel(ConfigurationViewModel configuration)
        {
            WfmHolder = new WaveformListHolder(configuration.Config);
            WfmList = new WaveformListingModel(configuration.Config, wfmHolder);
            this.configuration = configuration;

            ChannelList = new List<ChannelInfo>();
            EnabledChannels = new List<eChannles>();
        }

        WaveformListHolder wfmHolder;

        public WaveformListHolder WfmHolder
        {
            get
            {
                return wfmHolder;
            }
            set
            {
                wfmHolder = value;
                RaisePropertyChanged("WfmHolder");
            }
        }


        WaveformListingModel wfmList;

        public WaveformListingModel WfmList
        {
            get
            {
                return wfmList;
            }
            set
            {
                wfmList = value;
                RaisePropertyChanged("WfmList");
            }
        }

        long selectedIndex;
        public long SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                selectedIndex = value;
                RaisePropertyChanged("SelectedIndex");
            }
        }

        private List<ChannelInfo> channelList;

        public List<ChannelInfo> ChannelList
        {
            get { return channelList; }
            set
            {
                channelList = value;
                RaisePropertyChanged(nameof(ChannelList));
            }
        }

        public List<eChannles> enabledChannels;

        public List<eChannles> EnabledChannels
        {
            get { return enabledChannels; }
            set
            {
                enabledChannels = value;
                RaisePropertyChanged(nameof(EnabledChannels));
            }
        }


        private string GetChannelName(eChannles channel)
        {
            var channelInfo = channelList.FirstOrDefault(c => c.ChannelIndex == channel);

            if (channelInfo == null) return channel.ToString();

            return channelInfo.ChannelName;
        }

        #region Channel Names

        public string Ch16Name
        {
            get
            {
                return GetChannelName(eChannles.CH16);
            }
        }

        public string Ch15Name
        {
            get
            {
                return GetChannelName(eChannles.CH15);
            }
        }

        public string Ch14Name
        {
            get
            {
                return GetChannelName(eChannles.CH14);
            }
        }

        public string Ch13Name
        {
            get
            {
                return GetChannelName(eChannles.CH13);
            }
        }

        public string Ch12Name
        {
            get
            {
                return GetChannelName(eChannles.CH12);
            }
        }

        public string Ch11Name
        {
            get
            {
                return GetChannelName(eChannles.CH11);
            }
        }

        public string Ch10Name
        {
            get
            {
                return GetChannelName(eChannles.CH10);
            }
        }

        public string Ch9Name
        {
            get
            {
                return GetChannelName(eChannles.CH9);
            }
        }

        public string Ch8Name
        {
            get
            {
                return GetChannelName(eChannles.CH8);
            }
        }

        public string Ch7Name
        {
            get
            {
                return GetChannelName(eChannles.CH7);
            }
        }

        public string Ch6Name
        {
            get
            {
                return GetChannelName(eChannles.CH6);
            }
        }

        public string Ch5Name
        {
            get
            {
                return GetChannelName(eChannles.CH5);
            }
        }

        public string Ch4Name
        {
            get
            {
                return GetChannelName(eChannles.CH4);
            }
        }

        public string Ch3Name
        {
            get
            {
                return GetChannelName(eChannles.CH3);
            }
        }

        public string Ch2Name
        {
            get
            {
                return GetChannelName(eChannles.CH2);
            }
        }

        public string Ch1Name
        {
            get
            {
                return GetChannelName(eChannles.CH1);
            }
        }
        #endregion


        private Command gotoTrigger;
        private readonly ConfigurationViewModel configuration;

        public Command GotoTrigger
        {
            get
            {
                if (gotoTrigger == null)
                    gotoTrigger = new Command(new Command.ICommandOnExecute(gotoTriggerMethod));
                return gotoTrigger;
            }
        }

        void gotoTriggerMethod()
        {
            if (SessionConfiguration.TriggersetTView && SessionConfiguration.TriggerTime > 0)
            {
                WfmHolder.TriggerPacket = SessionConfiguration.TriggerIndex - 1;
                SelectedIndex = SessionConfiguration.TriggerIndex - 1;
            }
        }

        public void Initialize()
        {
            configuration.Config.PropertyChanged += Configuration_PropertyChanged;

            // Update the channel information
            ChannelList = configuration.ProtocolConfiguration.SelectMany(p => p.Channels.Where(c => c.ChannelIndex != eChannles.None)).ToList();
            EnabledChannels = ChannelList.Select(c => c.ChannelIndex).ToList();

            RaisePropertyChanged(nameof(Ch1Name));
            RaisePropertyChanged(nameof(Ch2Name));
            RaisePropertyChanged(nameof(Ch3Name));
            RaisePropertyChanged(nameof(Ch4Name));
            RaisePropertyChanged(nameof(Ch5Name));
            RaisePropertyChanged(nameof(Ch6Name));
            RaisePropertyChanged(nameof(Ch7Name));
            RaisePropertyChanged(nameof(Ch8Name));
            RaisePropertyChanged(nameof(Ch9Name));
            RaisePropertyChanged(nameof(Ch10Name));
            RaisePropertyChanged(nameof(Ch11Name));
            RaisePropertyChanged(nameof(Ch12Name));
            RaisePropertyChanged(nameof(Ch13Name));
            RaisePropertyChanged(nameof(Ch14Name));
            RaisePropertyChanged(nameof(Ch15Name));
            RaisePropertyChanged(nameof(Ch16Name));

            WfmHolder.Initialize();
        }

        private void Configuration_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ConfigModel.DataFormat))
            {
                wfmHolder.FireCollectionChangedEvent();
            }
        }

        public void Reset()
        {
            wfmHolder.Reset();
            wfmList.Reset();

            ChannelList.Clear();
            EnabledChannels.Clear();

            configuration.Config.PropertyChanged -= Configuration_PropertyChanged;
        }
    }
}
