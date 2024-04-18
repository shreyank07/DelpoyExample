using CommunityToolkit.Mvvm.Input;
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
using System.Windows.Input;
using System.Xml.Serialization;

namespace PGYMiniCooper.CoreModule.ViewModel
{
    public class ConfigViewModel_Group : ViewModelBase, IConfigViewModel
    {
        private readonly ConfigModel_Group model;
        private readonly IList<ChannelSelectionViewModel> supportedChannels;

        public event Action OnChannelSelectionChanged;

        /// <summary>
        /// Serialization only
        /// </summary>
        public ConfigViewModel_Group():this(new ConfigModel_Group(), new List<ChannelSelectionViewModel>()) { }

        public ConfigViewModel_Group(ConfigModel_Group model, IList<ChannelSelectionViewModel> supportedChannels)
        {
            this.model = model;
            this.supportedChannels = supportedChannels;
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

        [XmlArray("Channels")]
        [XmlArrayItem("Channel")]
        public List<ChannelInfo> ChannelList
        {
            get { return model.Channels.ToList(); }
            set
            {
                model.CleanChannels();

                if (value != null)
                {
                    foreach (var channel in value)
                    {
                        model.AddChannel(channel);
                    }
                }

                RaisePropertyChanged(nameof (ChannelList));
            }
        }

        public eProtocol ProtocolType => throw new NotImplementedException("Channel group does not implement ProtocolType");

        public IReadOnlyList<ChannelInfo> Channels => model.Channels;

        public IConfigModel Model => model;

        public IList<eChannles> ChannelsEnabled => model.Channels.Select(c => c.ChannelIndex).ToList();

        private ICommand channelSelectionCommand;

        public ICommand ChannelSelectionCommand
        {
            get
            {
                if(channelSelectionCommand==null)
                {
                    channelSelectionCommand = new RelayCommand<string>(parameter =>
                    {
                        if (string.IsNullOrEmpty(parameter) == false)
                        {
                            var parameterList = parameter.Split('_');

                            if (parameterList.Length > 1 && Enum.TryParse<eChannles>(parameterList[0], out eChannles channel))
                            {
                                var channelSelection = supportedChannels.FirstOrDefault(c => c.Channel == channel);
                                if (channelSelection == null)
                                {
                                    // Exception this should never happen
                                    RaisePropertyChanged(nameof(ChannelsEnabled));
                                    return;
                                }

                                if (parameterList[1] == bool.TrueString)
                                {
                                    if (channelSelection.IsAvailable == false)
                                    {
                                       
                                        MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                                       
                                        RaisePropertyChanged(nameof(ChannelsEnabled));
                                        return;
                                    }
                                    
                                  model.AddChannel(channel.ToString(), channel);
                                  channelSelection.IsAvailable = false;
                                }
                                else
                                {
                                   
                                    model.RemoveChannel(channel.ToString());
                                    channelSelection.IsAvailable = true;
                                }
                            }
                        }

                        RaisePropertyChanged(nameof(ChannelsEnabled));

                        OnChannelSelectionChanged?.Invoke();
                    });
                }

                return channelSelectionCommand;
            }

        }

        public void UpdateChannelSelection()
        {
            ((RelayCommand<string>)channelSelectionCommand)?.NotifyCanExecuteChanged();
        }
    }
}