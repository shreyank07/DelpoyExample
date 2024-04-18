using CommunityToolkit.Mvvm.Input;
using PGYMiniCooper.CoreModule.View.CustomView;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using Prodigy.Business;
using Prodigy.Business.Extensions;
using ProdigyFramework.Behavior;
using ProdigyFramework.Collections;
using ProdigyFramework.ComponentModel;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.Primitives;

namespace PGYMiniCooper.CoreModule.ViewModel
{
    [XmlRoot("Configuration")]
    public class ConfigurationViewModel : ViewModelBase
    {
        /// <summary>
        /// Seralization only constructor
        /// </summary>
        public ConfigurationViewModel() : this(new ConfigModel())
        {
            ProtocolConfiguration.Add(new ConfigViewModel_I3C());
        }

        public ConfigurationViewModel(ConfigModel config)
        {
            this.config = config;

            channelList = new ObservableCollection<ChannelSelectionViewModel>();
            foreach (var item in Enum.GetValues(typeof(eChannles)).Cast<eChannles>().Where(x => x != eChannles.None))
                ChannelList.Add(new ChannelSelectionViewModel(item));

            channelListLA = new ObservableCollection<eChannles>();
            foreach (var item in Enum.GetValues(typeof(eChannles)).Cast<eChannles>().Where(x => x != eChannles.None))
                ChannelListLA.Add(item);
            GroupType = eGroupType.Group;
            AvailableNumberOfI3C = new ObservableCollection<int>();
            foreach (int number in Enumerable.Range(1, 8))
                AvailableNumberOfI3C.Add(number);

            voltageList = new List<eVoltage>();
            VoltageList.AddRange(Enum.GetValues(typeof(eVoltage)).Cast<eVoltage>());

            Trigger = new TriggerViewModel(config.Trigger);
        }

        private TriggerViewModel trigger;
        public TriggerViewModel Trigger
        {
            get { return trigger; }
            set
            {
                trigger = value;
                RaisePropertyChanged(nameof(Trigger));
            }
        }

        private ObservableCollection<int> availableNumberOfI3C;

        public ObservableCollection<int> AvailableNumberOfI3C
        {
            get { return availableNumberOfI3C; }
            private set
            {
                availableNumberOfI3C = value;
                RaisePropertyChanged(nameof(AvailableNumberOfI3C));
            }
        }

        private ObservableCollection<ChannelSelectionViewModel> channelList;

        [XmlIgnore]
        public ObservableCollection<ChannelSelectionViewModel> ChannelList
        {
            get
            {
                return channelList;
            }
            set
            {
                channelList = value;
                NotifyPropertyChanged("ChannelList");
            }
        }


        private ObservableCollection<eChannles> channelListLA;
        public ObservableCollection<eChannles> ChannelListLA
        {
            get
            {
                return channelListLA;
            }
            set
            {
                channelListLA = value;
                NotifyPropertyChanged("ChannelListLA");
            }
        }

        private List<eVoltage> voltageList;
        public List<eVoltage> VoltageList
        {
            get
            {
                return voltageList;
            }
            set
            {
                voltageList = value;
                NotifyPropertyChanged("VoltageList");
            }
        }

        public eVoltage SignalAmpCh1_2
        {
            get
            {
                return config.SignalAmpCh1_2;
            }

            set
            {
                config.SignalAmpCh1_2 = value;
                RaisePropertyChanged("SignalAmpCh1_2");
            }
        }
        public eVoltage SignalAmpCh3_4
        {
            get
            {
                return config.SignalAmpCh3_4;
            }

            set
            {
                config.SignalAmpCh3_4 = value;
                RaisePropertyChanged("SignalAmpCh3_4");
            }
        }
        public eVoltage SignalAmpCh5_6
        {
            get
            {
                return config.SignalAmpCh5_6;
            }

            set
            {
                config.SignalAmpCh5_6 = value;
                RaisePropertyChanged("SignalAmpCh5_6");
            }
        }
        public eVoltage SignalAmpCh7_8
        {
            get
            {
                return config.SignalAmpCh7_8;
            }

            set
            {
                config.SignalAmpCh7_8 = value;
                RaisePropertyChanged("SignalAmpCh7_8");
            }
        }

        public eVoltage SignalAmpCh9_10
        {
            get
            {
                return config.SignalAmpCh9_10;
            }

            set
            {
                config.SignalAmpCh9_10 = value;
                RaisePropertyChanged("SignalAmpCh9_10");
            }
        }
        public eVoltage SignalAmpCh11_12
        {
            get
            {
                return config.SignalAmpCh11_12;
            }

            set
            {
                config.SignalAmpCh11_12 = value;
                RaisePropertyChanged("SignalAmpCh11_12");
            }
        }

        public eVoltage SignalAmpCh13_14
        {
            get
            {
                return config.SignalAmpCh13_14;
            }

            set
            {
                config.SignalAmpCh13_14 = value;
                RaisePropertyChanged("SignalAmpCh13_14");
            }
        }

        public eVoltage SignalAmpCh15_16
        {
            get
            {
                return config.SignalAmpCh15_16;
            }

            set
            {
                config.SignalAmpCh15_16 = value;
                RaisePropertyChanged("SignalAmpCh15_16");
            }
        }


      
        public eGeneralPurpose GeneralPurposeMode
        {
            get
            {
                return config.GeneralPurposeMode;
            }
            set
            {
                config.GeneralPurposeMode = value;
                
                RaisePropertyChanged("GeneralPurposeMode");

                if (value == eGeneralPurpose.Timing)
                    GroupType = eGroupType.Individual;
            }
        }

        public eSampleRate SampleRateLAPA
        {
            get
            {
                return config.SampleRateLAPA;
            }

            set
            {
                config.SampleRateLAPA = value;
            
                RaisePropertyChanged("SampleRateLAPA");
            }
        }

        public bool HasTwoClockSources
        {
            get
            {
                return config.HasTwoClockSources;
            }
            set
            {
                // Deselect previously selected channel
                if (value == false && config.SelectedClock2 != eChannles.None)
                    ChannelList.First(c => c.Channel == config.SelectedClock2).IsAvailable = true;

                config.HasTwoClockSources = value;
                RaisePropertyChanged(nameof(HasTwoClockSources));

                UpdateChannelAvailabilityForLA_GroupMode();

                Group_OnChannelSelectionChanged();
            }
        }

        public eChannles SelectedClock1
        {
            get
            {
                return config.SelectedClock1;
            }
            set
            {
                if (value != eChannles.None && ChannelList.First(c => c.Channel == value).IsAvailable == false)
                {
                    MessageBox.Show("Channel is already assigned to group configuration.");

                    Application.Current.Dispatcher.BeginInvoke(new Action(() => RaisePropertyChanged(nameof(SelectedClock1))));
                    return;
                }

                // Deselect previously selected channel
                if (config.SelectedClock1 != eChannles.None)
                    ChannelList.First(c => c.Channel == config.SelectedClock1).IsAvailable = true;

                config.SelectedClock1 = value;
                RaisePropertyChanged(nameof(SelectedClock1));

                UpdateChannelAvailabilityForLA_GroupMode();

                Group_OnChannelSelectionChanged();
            }
        }

        private void UpdateChannelAvailabilityForLA_GroupMode()
        {
            if (this.ConfigurationMode == eConfigMode.LA_Mode)
            {
                if (GroupType == eGroupType.Group)
                {
                    if (SelectedClock1 != eChannles.None)
                        ChannelList.First(c => c.Channel == SelectedClock1).IsAvailable = false;

                    if (HasTwoClockSources && SelectedClock2 != eChannles.None)
                        ChannelList.First(c => c.Channel == SelectedClock2).IsAvailable = false;
                }
            }
        }

        public eEdgeType SampleEdgeClock1
        {
            get
            {
                if (config.IsCLK1_RisingEdge)
                    return eEdgeType.RISING_EDGE;
                else if (config.IsCLK1_FallingEdge)
                    return eEdgeType.FALLING_EDGE;


                return eEdgeType.NO_EDGE;
            }
            set
            {
                config.IsCLK1_RisingEdge = false;
                config.IsCLK1_FallingEdge = false;
                if (value == eEdgeType.RISING_EDGE)
                    config.IsCLK1_RisingEdge = true;
                else if (value == eEdgeType.FALLING_EDGE)
                    config.IsCLK1_FallingEdge = true;

                RaisePropertyChanged(nameof(SampleEdgeClock1));
            }
        }

        public eChannles SelectedClock2
        {
            get
            {
                return config.SelectedClock2;
            }
            set
            {
                if (value != eChannles.None && ChannelList.First(c => c.Channel == value).IsAvailable == false)
                {
                    MessageBox.Show("Channel is already assigned to group configuration.");

                    Application.Current.Dispatcher.BeginInvoke(new Action(() => RaisePropertyChanged(nameof(SelectedClock2))));
                    return;
                }

                // Deselect previously selected channel
                if (config.SelectedClock2 != eChannles.None)
                    ChannelList.First(c => c.Channel == config.SelectedClock2).IsAvailable = true;

                config.SelectedClock2 = value;
                RaisePropertyChanged(nameof(SelectedClock2));

                UpdateChannelAvailabilityForLA_GroupMode();

                Group_OnChannelSelectionChanged();
            }
        }

        public eEdgeType SampleEdgeClock2
        {
            get
            {
                if (config.IsCLK2_RisingEdge)
                    return eEdgeType.RISING_EDGE;
                else if (config.IsCLK2_FallingEdge)
                    return eEdgeType.FALLING_EDGE;

                return eEdgeType.NO_EDGE;
            }
            set
            {
                config.IsCLK2_RisingEdge = false;
                config.IsCLK2_FallingEdge = false;
                if (value == eEdgeType.RISING_EDGE)
                    config.IsCLK2_RisingEdge = true;
                else if (value == eEdgeType.FALLING_EDGE)
                    config.IsCLK2_FallingEdge = true;

                RaisePropertyChanged(nameof(SampleEdgeClock2));
            }
        }

        public IEnumerable<eGroupType> GroupTypeList
        {
            get
            {
                return Enum.GetValues(typeof(eGroupType)).Cast<eGroupType>();
            }
        }
        public List<eBitOrder> BitOrderList
        {
            get
            {
                return Enum.GetValues(typeof(eBitOrder)).Cast<eBitOrder>().ToList();
            }
        }


        private ConfigModel config;

        [XmlIgnore]
        public ConfigModel Config
        {
            get
            {
                return config;
            }
        }

        ObservableCollection<IConfigViewModel> protocolConfiguration = new ObservableCollection<IConfigViewModel>();

        public ObservableCollection<IConfigViewModel> ProtocolConfiguration
        {
            get { return protocolConfiguration; }
            set
            {
                protocolConfiguration = null;

                // update model while importing configuration
                if (value != null)
                {
                    ObservableCollection<IConfigViewModel> tmpProtocolConfig = new ObservableCollection<IConfigViewModel>();
                    foreach (var protocolConfig in value)
                    {
                        if (protocolConfig is ConfigViewModel_Group configGroup)
                        {
                            var group = new ConfigViewModel_Group((ConfigModel_Group)configGroup.Model, channelList);
                            group.OnChannelSelectionChanged += Group_OnChannelSelectionChanged;

                            tmpProtocolConfig.Add(group);
                        }
                        else
                        {
                            tmpProtocolConfig.Add(protocolConfig);
                        }
                    }

                    protocolConfiguration = tmpProtocolConfig;
                    config.ProtocolConfigList = new List<IConfigModel>(value.Select(c => c.Model));
                }

                RaisePropertyChanged("ProtocolConfiguration");
            }
        }

        private void Group_OnChannelSelectionChanged()
        {
            protocolConfiguration.OfType<ConfigViewModel_Group>().ToList().ForEach(g => g.UpdateChannelSelection());
        }

        private ICommand protocolSelectionCommand;
        public ICommand ProtocolSelectionCommand
        {
            get
            {
                if (protocolSelectionCommand == null)
                    protocolSelectionCommand = new RelayCommand<string>(protocolSelectionCommand_Execute, protocolSelectionCommand_CanExecute);
                return protocolSelectionCommand;
            }
        }

        private void protocolSelectionCommand_Execute(string parameters)
        {
            if (parameters == null || string.IsNullOrEmpty(parameters))
                return; //throw new ArgumentException(nameof(parameters));

            string[] values = parameters.Split('_');

            if (values.Length < 2)
                throw new Exception("Invalid argumentCheck the bindings");

            var protocolType = (eProtocol)Enum.Parse(typeof(eProtocol), values[0]);
            bool Ischecked = bool.Parse(values[1]);

            int numberOfProtocol = 1;
            if (values.Length > 2 && !string.IsNullOrEmpty(values[2]))
                numberOfProtocol = int.Parse(values[2]);

            if (Ischecked == false)
                numberOfProtocol = 0;

            int numberProtocol_AlreadyAdded = 0;

            var protocolObjectType = CreateNewProtocolConfig(protocolType, "tmp").GetType();
            numberProtocol_AlreadyAdded = protocolConfiguration.Count(c => c.GetType() == protocolObjectType);

            int removeCount = 0;
            int addedCount = 0;
            if (numberProtocol_AlreadyAdded > numberOfProtocol)
                removeCount = numberProtocol_AlreadyAdded - numberOfProtocol;
            else
                addedCount = numberOfProtocol - numberProtocol_AlreadyAdded;

            for (int i = 0; i < addedCount; i++)
            {

                var config = CreateNewProtocolConfig(protocolType, $"{protocolType}" + $"__{numberProtocol_AlreadyAdded + i + 1}");
                Config.ProtocolConfigList.Add(config.Model);
                ProtocolConfiguration.Add(config);
            }

            var removeItems = protocolConfiguration.Where(p => p.GetType() == protocolObjectType).Reverse().Take(removeCount).ToList();
            removeItems.ForEach(r =>
            {
                protocolConfiguration.Remove(r);
                Config.ProtocolConfigList.Remove(r.Model);
                Trigger.TriggerType = eTriggerTypeList.Auto;
                foreach (var channel in r.Channels.Where(c => c.ChannelIndex != eChannles.None))
                {
                    var channelConfig = ChannelList.FirstOrDefault(c => c.Channel == channel.ChannelIndex);
                    if (channelConfig != null)
                        channelConfig.IsAvailable = true;
                }
            });

            ((RelayCommand<string>)ProtocolSelectionCommand).NotifyCanExecuteChanged();
        }

        private IConfigViewModel CreateNewProtocolConfig(eProtocol protocolType, string name)
        {
            IConfigViewModel protocolInstance = null;
            switch (protocolType)
            {
                case eProtocol.I2C:
                    protocolInstance = new ConfigViewModel_I2C(new ConfigModel_I2C(name));
                    break;
                case eProtocol.I3C:
                    protocolInstance = new ConfigViewModel_I3C(new ConfigModel_I3C(name));
                    break;
                case eProtocol.SPI:
                    protocolInstance = new ConfigViewModel_SPI(new ConfigModel_SPI(name));
                    break;
                case eProtocol.UART:
                    protocolInstance = new ConfigViewModel_UART(new ConfigModel_UART(name));
                    break;
                case eProtocol.SPMI:
                    protocolInstance = new ConfigViewModel_SPMI(new ConfigModel_SPMI(name));
                    break;
                case eProtocol.RFFE:
                    protocolInstance = new ConfigViewModel_RFFE(new ConfigModel_RFFE(name));
                    break;
                case eProtocol.CAN:
                    protocolInstance = new ConfigViewModel_CAN(new ConfigModel_CAN(name));
                    break;
                case eProtocol.QSPI:
                    protocolInstance = new ConfigViewModel_QSPI(new ConfigModel_QSPI(name));
                    break;



                default:
                    throw new NotImplementedException($"This protocol type is not implemented [{protocolType}].");
            }

            return protocolInstance;
        }

        bool protocolSelectionCommand_CanExecute(string parameters)
        {
            if (parameters == null || string.IsNullOrEmpty(parameters))
                return true;

            string[] values = parameters.Split('_');

            if (values.Length < 2)
                throw new Exception("Invalid argumentCheck the bindings");

            var protocolType = (eProtocol)Enum.Parse(typeof(eProtocol), values[0]);
            bool currentState = bool.Parse(values[1]);

            var tmpConfig = CreateNewProtocolConfig(protocolType, "tmp");
            bool protocolAlreadyAdded = ProtocolConfiguration.Any(p => p.GetType() == tmpConfig.GetType());

            // Ischecked -> false -> there are items ? true is previous state
            // Ischecked -> true -> there are no items ? false is previous state
            bool previousState = currentState;
            if (currentState == false)
            {
                if (protocolAlreadyAdded) previousState = true;
            }
            else
            {
                if (protocolAlreadyAdded == false) previousState = false;
            }

            int noChannelsInUse = ProtocolConfiguration.Select(p => p.Channels.Count).Sum();
            var requiredChannelCount = tmpConfig.Channels.Count;

            // TODO: based on this number and avialable chennels update the combobox source to contain only those values
            if (protocolType == eProtocol.I3C)
            {
                int noOfI3c_Added = ProtocolConfiguration.Count(p => p.GetType() == tmpConfig.GetType());

                int noOfI3C_Available = noOfI3c_Added + ((ChannelList.Count - noChannelsInUse) / requiredChannelCount);

                if (AvailableNumberOfI3C.Count > noOfI3C_Available)
                    AvailableNumberOfI3C.RemoveAll(a => a > noOfI3C_Available);
                else if (AvailableNumberOfI3C.Count < noOfI3C_Available)
                {
                    for (int i = AvailableNumberOfI3C.Count; i < noOfI3C_Available; i++)
                        AvailableNumberOfI3C.Add(i + 1);
                }
            }

            // previous checked state
            if (previousState)
                return true;

            return requiredChannelCount + noChannelsInUse <= ChannelList.Count;
        }





        private eGroupType groupType = eGroupType.Group;

        public eGroupType GroupType
        {
            get
            {
                return groupType;
            }

            set
            {
                eGroupType previousSelection = groupType;
                groupType = value;

                // Check if group / individual switch and update the channel availability.
                if (previousSelection != groupType)
                {
                    ResetChannels();

                    if (config.ConfigurationMode == eConfigMode.LA_Mode)
                    {
                        Group_Individual_Method();
                    }
                }

                RaisePropertyChanged("GroupType");
            }
        }



        public void Group_Individual_Method()
        {
            if (GroupType == eGroupType.Group)
            {
                protocolConfiguration.Clear();

                // add group view model
                for (int i = 0; i <= 3; i++)
                {
                    var group_Config = new ConfigModel_Group() { Name = $"GRP{i + 1}_CLK1" };
                    config.ProtocolConfigList.Add(group_Config);

                    ConfigViewModel_Group viewModelGroup = new ConfigViewModel_Group(group_Config, channelList);
                    viewModelGroup.OnChannelSelectionChanged += Group_OnChannelSelectionChanged;
                    ProtocolConfiguration.Add(viewModelGroup);
                }

                for (int i = 0; i <= 3; i++)
                {
                    var group_Config = new ConfigModel_Group() { Name = $"GRP{i + 1}_CLK2" };
                    config.ProtocolConfigList.Add(group_Config);

                    ConfigViewModel_Group viewModelGroup = new ConfigViewModel_Group(group_Config, channelList);
                    viewModelGroup.OnChannelSelectionChanged += Group_OnChannelSelectionChanged;
                    ProtocolConfiguration.Add(viewModelGroup);
                }

                UpdateChannelAvailabilityForLA_GroupMode();
            }
            else
            {
                // individual view model

                protocolConfiguration.Clear();

                var group_Config = new ConfigModel_Group() { Name = "Individual" };
                config.ProtocolConfigList.Add(group_Config);

                ConfigViewModel_Group viewModelGroup = new ConfigViewModel_Group(group_Config, channelList);
                ProtocolConfiguration.Add(viewModelGroup);
            }
        }

        public eConfigMode ConfigurationMode
        {
            get
            {
                return config.ConfigurationMode;
            }
            set
            {
                if ((config.ConfigurationMode == eConfigMode.LA_Mode && value != eConfigMode.LA_Mode) ||
                    (config.ConfigurationMode != eConfigMode.LA_Mode && value == eConfigMode.LA_Mode))
                {
                    // TODO: Reset channel availability
                    ResetChannels();

                    protocolConfiguration.Clear();

                    ProtocolConfiguration = protocolConfiguration;
                }

                config.ConfigurationMode = value;

                if (config.ConfigurationMode == eConfigMode.LA_Mode)
                    Group_Individual_Method();
                Trigger.TriggerType = eTriggerTypeList.Auto;

                RaisePropertyChanged("ConfigurationMode");
            }

        }

        private void ResetChannels()
        {
            foreach (var channel in ChannelList)
            {
                channel.IsAvailable = true;
            }
        }

    }
}

