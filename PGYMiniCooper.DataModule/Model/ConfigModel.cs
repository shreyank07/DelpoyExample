using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Structure;
using ProdigyFramework.Collections;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace PGYMiniCooper.DataModule.Model
{
    [XmlRoot]
    public class ConfigModel : ViewModelBase
    {
        public ConfigModel()
        {
            ProtocolConfigList = new List<IConfigModel>();
            trigger = TriggerModel.GetInstance();

            configurationMode = eConfigMode.Both;
            generalPurposeMode = eGeneralPurpose.State;
            probeType = eProbeType.Type2;
            _CycleCount = 1;
            GroupType = eGroupType.Group;
            selectedCLK1_GRP1 = new ObservableCollection<eChannles>();
            SelectedCLK1_GRP2 = new ObservableCollection<eChannles>();
            SelectedCLK1_GRP3 = new ObservableCollection<eChannles>();
            SelectedCLK2_GRP1 = new ObservableCollection<eChannles>();
            SelectedCLK2_GRP2 = new ObservableCollection<eChannles>();
            SelectedCLK2_GRP3 = new ObservableCollection<eChannles>();
            IsCh3Visible = true;
            IsCh4Visible = true;
            IsCh5Visible = true;
            selectedIndividualChannels = new ObservableCollection<eChannles>();
     
            selectedClock1 = eChannles.CH1;
            selectedClock2 = eChannles.CH2;
            IsCh1Visible = true;
            IsCh2Visible = true;
            isCLK1_RisingEdge = true;
            isCLK2_RisingEdge = true;
       
        
            samplingEdge = eEdgeType.RISING_EDGE;
    
       
          

            var converter = new System.Windows.Media.BrushConverter();
            Ch1Fgd = "#cfd8dc";
            Ch2Fgd = "#cfd8dc";
            Ch3Fgd = "#cfd8dc";
            Ch4Fgd = "#cfd8dc";
            Ch5Fgd = "#cfd8dc";
            Ch6Fgd = "#cfd8dc";
            Ch7Fgd = "#cfd8dc";
            Ch8Fgd = "#cfd8dc";
            Ch9Fgd = "#cfd8dc";
            Ch10Fgd = "#cfd8dc";
            Ch11Fgd = "#cfd8dc";
            Ch12Fgd = "#cfd8dc";
            Ch13Fgd = "#cfd8dc";
            Ch14Fgd = "#cfd8dc";
            Ch15Fgd = "#cfd8dc";
            Ch16Fgd = "#cfd8dc";
        }
                
        public List<IConfigModel> ProtocolConfigList { get; set; }

        private eDataFormat dataFormat = eDataFormat.Hex;
        public eDataFormat DataFormat
        {
            get
            {
                return dataFormat;
            }
            set
            {
                dataFormat = value;                
                RaisePropertyChanged(nameof(DataFormat));
            }
        }
   
        public void ResetChannels()
        {
            SelectedClock1 = eChannles.None;
            SelectedClock2 = eChannles.None;
 

            SelectedCLK1_GRP1.Clear();
            SelectedCLK1_GRP2.Clear();
            SelectedCLK1_GRP3.Clear();
            SelectedCLK2_GRP1.Clear();
            SelectedCLK2_GRP2.Clear();
            SelectedCLK2_GRP3.Clear();
            SelectedIndividualChannels.Clear();
            //DataFormat = eDataFormat.Hex;
        }

        private TriggerModel trigger;

        [XmlIgnore]
        public TriggerModel Trigger
        {
            get { return trigger; }
            set { trigger = value; }
        }

        [NonSerialized]
        private bool isParity;
        public bool IsParity
        {
            get
            {
                return isParity;
            }
            set
            {
                isParity = value;
                RaisePropertyChanged("IsParity");
            }
        }
        [NonSerialized]
        private eEdgeType samplingEdge;
        public eEdgeType SamplingEdge
        {
            get
            {
                return samplingEdge;
            }
            set
            {
                samplingEdge = value;
                RaisePropertyChanged("SamplingEdge");
            }
        }

        #region Clock Source Properties
        [NonSerialized]
        private eChannles selectedClock1;
        public eChannles SelectedClock1
        {
            get
            {
                return selectedClock1;
            }
            set
            {
                selectedClock1 = value;
                RaisePropertyChanged("SelectedClock1");
            }
        }
        [NonSerialized]
        private eChannles selectedClock2;
        public eChannles SelectedClock2
        {
            get
            {
                return selectedClock2;
            }
            set
            {
                selectedClock2 = value;
                RaisePropertyChanged("SelectedClock2");
            }
        }
        [NonSerialized]
        private bool isCLK1_RisingEdge = true;
        public bool IsCLK1_RisingEdge
        {
            get
            {
                return isCLK1_RisingEdge;
            }
            set
            {
                isCLK1_RisingEdge = value;
                RaisePropertyChanged("IsCLK1_RisingEdge");
            }
        }
        [NonSerialized]
        private bool isCLK1_FallingEdge = false;
        public bool IsCLK1_FallingEdge
        {
            get
            {
                return isCLK1_FallingEdge;
            }
            set
            {
                isCLK1_FallingEdge = value;
                RaisePropertyChanged("IsCLK1_FallingEdge");
            }
        }
        [NonSerialized]
        private bool isCLK2_RisingEdge = true;
        public bool IsCLK2_RisingEdge
        {
            get
            {
                return isCLK2_RisingEdge;
            }
            set
            {
                isCLK2_RisingEdge = value;
                RaisePropertyChanged("IsCLK2_RisingEdge");
            }
        }
        [NonSerialized]
        private bool isCLK2_FallingEdge = false;
        public bool IsCLK2_FallingEdge
        {
            get
            {
                return isCLK2_FallingEdge;
            }
            set
            {
                isCLK2_FallingEdge = value;
                RaisePropertyChanged("isCLK2_FallingEdge");
            }
        }
        [NonSerialized]
        private bool hasTwoClockSources = false;

        public bool HasTwoClockSources
        {
            get
            {
                return hasTwoClockSources;
            }
            set
            {
                hasTwoClockSources = value;
                if (value == true)
                {
                    CycleCount = 2;
                    foreach(var chn in SelectedCLK1_GRP1)
                    {
                        if (SelectedClock2 == chn)
                            SelectedCLK1_GRP1 = RemoveChannels(SelectedCLK1_GRP1,SelectedClock2);
                    }
                    foreach (var chn in SelectedCLK1_GRP2)
                    {
                        if (SelectedClock2 == chn)
                            SelectedCLK1_GRP2 = RemoveChannels(SelectedCLK1_GRP2, SelectedClock2);
                    }
                    foreach (var chn in SelectedCLK1_GRP3)
                    {
                        if (SelectedClock2 == chn)
                            SelectedCLK1_GRP3 = RemoveChannels(SelectedCLK1_GRP3, SelectedClock2);
                    }
                    foreach (var chn in SelectedCLK2_GRP1)
                    {
                        if (SelectedClock2 == chn)
                            SelectedCLK2_GRP1 = RemoveChannels(SelectedCLK2_GRP1, SelectedClock2);
                    }
                    foreach (var chn in SelectedCLK2_GRP2)
                    {
                        if (SelectedClock2 == chn)
                            SelectedCLK2_GRP2 = RemoveChannels(SelectedCLK2_GRP2, SelectedClock2);
                    }
                    foreach (var chn in SelectedCLK2_GRP3)
                    {
                        if (SelectedClock2 == chn)
                            SelectedCLK2_GRP3 = RemoveChannels(SelectedCLK2_GRP3, SelectedClock2);
                    }
                }
                else
                {
                    CycleCount = 1;
                }
                RaisePropertyChanged("HasTwoClockSources");
            }
        }
        private ObservableCollection<eChannles> RemoveChannels(ObservableCollection<eChannles> ChannelList, eChannles noChnl)
        {
            ObservableCollection<eChannles> temp = new ObservableCollection<eChannles>();
            foreach (var chn in ChannelList)
            {
                if (chn != noChnl)
                    temp.Add(chn);
            }
            return temp;
        }
        #endregion


        #region Protocol Properties

        


        #endregion

        #region Grouping Channels properties
        [NonSerialized]
        private string clk1Grp1Bgd = Brushes.LightBlue.ToString();
        public string Clk1Grp1Bgd
        {
            get
            {
                return clk1Grp1Bgd;
            }

            set
            {
                clk1Grp1Bgd = value;
                RaisePropertyChanged("Clk1Grp1Bgd");
            }
        }
        [NonSerialized]
        private string clk1Grp2Bgd = Brushes.LightSeaGreen.ToString();
        public string Clk1Grp2Bgd
        {
            get
            {
                return clk1Grp2Bgd;
            }

            set
            {
                clk1Grp2Bgd = value;
                RaisePropertyChanged("Clk1Grp2Bgd");
            }
        }
        [NonSerialized]
        private string clk1Grp3Bgd = Brushes.GreenYellow.ToString();
        public string Clk1Grp3Bgd
        {
            get
            {
                return clk1Grp3Bgd;
            }

            set
            {
                clk1Grp3Bgd = value;
                RaisePropertyChanged("Clk1Grp3Bgd");
            }
        }
        [NonSerialized]
        private string clk2Grp1Bgd = Brushes.LawnGreen.ToString();
        public string Clk2Grp1Bgd
        {
            get
            {
                return clk2Grp1Bgd;
            }

            set
            {
                clk2Grp1Bgd = value;
                RaisePropertyChanged("Clk2Grp1Bgd");
            }
        }
        [NonSerialized]
        private string clk2Grp2Bgd = Brushes.Orange.ToString();
        public string Clk2Grp2Bgd
        {
            get
            {
                return clk2Grp2Bgd;
            }

            set
            {
                clk2Grp2Bgd = value;
                RaisePropertyChanged("Clk2Grp2Bgd");
            }
        }
        [NonSerialized]
        private string clk2Grp3Bgd = Brushes.LightSteelBlue.ToString();
        public string Clk2Grp3Bgd
        {
            get
            {
                return clk2Grp3Bgd;
            }

            set
            {
                clk2Grp3Bgd = value;
                RaisePropertyChanged("Clk2Grp3Bgd");
            }
        }
        [NonSerialized]
        private Color selColorClk1Grp1 = Colors.LightBlue;
        public Color SelColorClk1Grp1
        {
            get
            {
                return selColorClk1Grp1;
            }

            set
            {
                selColorClk1Grp1 = value;
                Clk1Grp1Bgd = new SolidColorBrush(value).ToString();
                RaisePropertyChanged("SelColorClk1Grp1");
            }
        }




        [NonSerialized]
        private Color selColorClk1Grp2 = Colors.LightSeaGreen;
        public Color SelColorClk1Grp2
        {
            get
            {
                return selColorClk1Grp2;
            }

            set
            {
                selColorClk1Grp2 = value;
                Clk1Grp2Bgd = new SolidColorBrush(value).ToString();
                RaisePropertyChanged("SelColorClk1Grp2");
            }
        }
        [NonSerialized]
        private Color selColorClk1Grp3 = Colors.GreenYellow;
        public Color SelColorClk1Grp3
        {
            get
            {
                return selColorClk1Grp3;
            }

            set
            {
                selColorClk1Grp3 = value;
                Clk1Grp3Bgd = new SolidColorBrush(value).ToString();
                RaisePropertyChanged("SelColorClk1Grp3");
            }
        }
        [NonSerialized]
        private Color selColorClk2Grp1 = Colors.LawnGreen;
        public Color SelColorClk2Grp1
        {
            get
            {
                return selColorClk2Grp1;
            }

            set
            {
                selColorClk2Grp1 = value;
                Clk2Grp1Bgd = new SolidColorBrush(value).ToString();
                RaisePropertyChanged("SelColorClk2Grp1");
            }
        }
        [NonSerialized]
        private Color selColorClk2Grp2 = Colors.Orange;
        public Color SelColorClk2Grp2
        {
            get
            {
                return selColorClk2Grp2;
            }

            set
            {
                selColorClk2Grp2 = value;
                Clk2Grp2Bgd = new SolidColorBrush(value).ToString();
                RaisePropertyChanged("SelColorClk2Grp2");
            }
        }
        [NonSerialized]
        private Color selColorClk2Grp3 = Colors.LightSteelBlue;
        public Color SelColorClk2Grp3
        {
            get
            {
                return selColorClk2Grp3;
            }

            set
            {
                selColorClk2Grp3 = value;
                Clk2Grp3Bgd = new SolidColorBrush(value).ToString();
                RaisePropertyChanged("SelColorClk2Grp3");
            }
        }
        [NonSerialized]
        private string ch1Fgd;
        public string Ch1Fgd
        {
            get
            {
                return ch1Fgd;
            }

            set
            {
                ch1Fgd = value;
                RaisePropertyChanged("Ch1Fgd");
            }
        }
        [NonSerialized]
        private string ch2Fgd;
        public string Ch2Fgd
        {
            get
            {
                return ch2Fgd;
            }

            set
            {
                ch2Fgd = value;
                RaisePropertyChanged("Ch2Fgd");
            }
        }
        [NonSerialized]
        private string ch3Fgd;
        public string Ch3Fgd
        {
            get
            {
                return ch3Fgd;
            }

            set
            {
                ch3Fgd = value;
                RaisePropertyChanged("Ch3Fgd");
            }
        }
        [NonSerialized]
        private string ch4Fgd;
        public string Ch4Fgd
        {
            get
            {
                return ch4Fgd;
            }

            set
            {
                ch4Fgd = value;
                RaisePropertyChanged("Ch4Fgd");
            }
        }
        [NonSerialized]
        private string ch5Fgd;
        public string Ch5Fgd
        {
            get
            {
                return ch5Fgd;
            }

            set
            {
                ch5Fgd = value;
                RaisePropertyChanged("Ch5Fgd");
            }
        }
        [NonSerialized]
        private string ch6Fgd;
        public string Ch6Fgd
        {
            get
            {
                return ch6Fgd;
            }

            set
            {
                ch6Fgd = value;
                RaisePropertyChanged("Ch6Fgd");
            }
        }
        [NonSerialized]
        private string ch7Fgd;
        public string Ch7Fgd
        {
            get
            {
                return ch7Fgd;
            }

            set
            {
                ch7Fgd = value;
                RaisePropertyChanged("Ch7Fgd");
            }
        }
        [NonSerialized]
        private string ch8Fgd;
        public string Ch8Fgd
        {
            get
            {
                return ch8Fgd;
            }

            set
            {
                ch8Fgd = value;
                RaisePropertyChanged("Ch8Fgd");
            }
        }
        [NonSerialized]
        private string ch9Fgd;
        public string Ch9Fgd
        {
            get
            {
                return ch9Fgd;
            }

            set
            {
                ch9Fgd = value;
                RaisePropertyChanged("Ch9Fgd");
            }
        }
        [NonSerialized]
        private string ch10Fgd;
        public string Ch10Fgd
        {
            get
            {
                return ch10Fgd;
            }

            set
            {
                ch10Fgd = value;
                RaisePropertyChanged("Ch10Fgd");
            }
        }

        [NonSerialized]
        private string ch11Fgd;
        public string Ch11Fgd
        {
            get
            {
                return ch11Fgd;
            }

            set
            {
                ch11Fgd = value;
                RaisePropertyChanged("Ch11Fgd");
            }
        }

        [NonSerialized]
        private string ch12Fgd;
        public string Ch12Fgd
        {
            get
            {
                return ch12Fgd;
            }

            set
            {
                ch12Fgd = value;
                RaisePropertyChanged("Ch12Fgd");
            }
        }

        [NonSerialized]
        private string ch13Fgd;
        public string Ch13Fgd
        {
            get
            {
                return ch13Fgd;
            }

            set
            {
                ch13Fgd = value;
                RaisePropertyChanged("Ch13Fgd");
            }
        }

        [NonSerialized]
        private string ch14Fgd;
        public string Ch14Fgd
        {
            get
            {
                return ch14Fgd;
            }

            set
            {
                ch14Fgd = value;
                RaisePropertyChanged("Ch14Fgd");
            }
        }

        [NonSerialized]
        private string ch15Fgd;
        public string Ch15Fgd
        {
            get
            {
                return ch15Fgd;
            }

            set
            {
                ch15Fgd = value;
                RaisePropertyChanged("Ch15Fgd");
            }
        }

        [NonSerialized]
        private string ch16Fgd;
        public string Ch16Fgd
        {
            get
            {
                return ch16Fgd;
            }

            set
            {
                ch16Fgd = value;
                RaisePropertyChanged("Ch16Fgd");
            }
        }


        [NonSerialized]
        private string clk1Grp1Text = "CLK1_GRP1";
        public string Clk1Grp1Text
        {
            get
            {
                return clk1Grp1Text;
            }

            set
            {
                clk1Grp1Text = value;
                RaisePropertyChanged("Clk1Grp1Text");
            }
        }
        [NonSerialized]
        private string clk1Grp2Text = "CLK1_GRP2";
        public string Clk1Grp2Text
        {
            get
            {
                return clk1Grp2Text;
            }

            set
            {
                clk1Grp2Text = value;
                RaisePropertyChanged("Clk1Grp2Text");
            }
        }
        [NonSerialized]
        private string clk1Grp3Text = "CLK1_GRP3";
        public string Clk1Grp3Text
        {
            get
            {
                return clk1Grp3Text;
            }

            set
            {
                clk1Grp3Text = value;
                RaisePropertyChanged("Clk1Grp3Text");
            }
        }
        [NonSerialized]
        private string clk2Grp1Text = "CLK2_GRP1";
        public string Clk2Grp1Text
        {
            get
            {
                return clk2Grp1Text;
            }

            set
            {
                clk2Grp1Text = value;
                RaisePropertyChanged("Clk2Grp1Text");
            }
        }
        [NonSerialized]
        private string clk2Grp2Text = "CLK2_GRP2";
        public string Clk2Grp2Text
        {
            get
            {
                return clk2Grp2Text;
            }

            set
            {
                clk2Grp2Text = value;
                RaisePropertyChanged("Clk2Grp2Text");
            }
        }
        [NonSerialized]
        private string clk2Grp3Text = "CLK2_GRP3";
        public string Clk2Grp3Text
        {
            get
            {
                return clk2Grp3Text;
            }

            set
            {
                clk2Grp3Text = value;
                RaisePropertyChanged("Clk2Grp3Text");
            }
        }
        [NonSerialized]
        private ObservableCollection<eChannles> selectedCLK1_GRP1;
        public ObservableCollection<eChannles> SelectedCLK1_GRP1
        {
            get
            {
                return selectedCLK1_GRP1;
            }

            set
            {
                selectedCLK1_GRP1 = value;
                RaisePropertyChanged("SelectedCLK1_GRP1");
            }
        }
        [NonSerialized]
        private ObservableCollection<eChannles> selectedCLK1_GRP2;
        public ObservableCollection<eChannles> SelectedCLK1_GRP2
        {
            get
            {
                return selectedCLK1_GRP2;
            }

            set
            {
                selectedCLK1_GRP2 = value;
                RaisePropertyChanged("SelectedCLK1_GRP2");
            }
        }
        [NonSerialized]
        private ObservableCollection<eChannles> selectedCLK1_GRP3;
        public ObservableCollection<eChannles> SelectedCLK1_GRP3
        {
            get
            {
                return selectedCLK1_GRP3;
            }

            set
            {
                selectedCLK1_GRP3 = value;
                RaisePropertyChanged("SelectedCLK1_GRP3");
            }
        }
        [NonSerialized]
        private ObservableCollection<eChannles> selectedCLK2_GRP1;
        public ObservableCollection<eChannles> SelectedCLK2_GRP1
        {
            get
            {
                return selectedCLK2_GRP1;
            }

            set
            {
                selectedCLK2_GRP1 = value;
                RaisePropertyChanged("SelectedCLK2_GRP1");
            }
        }
        [NonSerialized]
        private ObservableCollection<eChannles> selectedCLK2_GRP2;
        public ObservableCollection<eChannles> SelectedCLK2_GRP2
        {
            get
            {
                return selectedCLK2_GRP2;
            }

            set
            {
                selectedCLK2_GRP2 = value;
                RaisePropertyChanged("SelectedCLK2_GRP2");
            }
        }
        [NonSerialized]
        private ObservableCollection<eChannles> selectedCLK2_GRP3;
        public ObservableCollection<eChannles> SelectedCLK2_GRP3
        {
            get
            {
                return selectedCLK2_GRP3;
            }

            set
            {
                selectedCLK2_GRP3 = value;
                RaisePropertyChanged("SelectedCLK2_GRP3");
            }
        }

        #endregion

        #region Individual
        [NonSerialized]
        private ObservableCollection<eChannles> selectedIndividualChannels;
        public ObservableCollection<eChannles> SelectedIndividualChannels
        {
            get
            {
                return selectedIndividualChannels;
            }

            set
            {
                selectedIndividualChannels = value;
                RaisePropertyChanged("SelectedIndividualChannels");
            }
        }
        #endregion
        [NonSerialized]
        private eSampleRate sampleRateLA;
        [NonSerialized]
        private eSampleRate sampleRateLAPA;
        [NonSerialized]
        private eGroupType groupType = eGroupType.Group;
        [NonSerialized]
        private int _CycleCount;
        public int CycleCount
        {
            get
            {
                return _CycleCount;
            }
            set
            {
                _CycleCount = value;
                RaisePropertyChanged("CycleCount");
            }
        }
        [NonSerialized]
        private eConfigMode configurationMode;
        public eConfigMode ConfigurationMode
        {
            get
            {
                return configurationMode;
            }
            set
            {
                configurationMode = value;
                RaisePropertyChanged("ConfigurationMode");

                if(configurationMode == eConfigMode.LA_Mode)
                {
                    if(GroupType == eGroupType.Group)
                    {
                        // add group view model
                    }
                    else
                    {

                    }
                        // individual view model
                }
            }
        }

        [NonSerialized]
        private eProbeType probeType;
        public eProbeType ProbeType
        {
            get
            {
                return probeType;
            }
            set
            {
                probeType = value;
                RaisePropertyChanged("ProbeType");
            }
        }

        [NonSerialized]
        private eGeneralPurpose generalPurposeMode;
        public eGeneralPurpose GeneralPurposeMode
        {
            get
            {
                return generalPurposeMode;
            }
            set
            {
                generalPurposeMode = value;
                if (value == eGeneralPurpose.Timing)
                {
                    Clk1Grp1Text = "ASN1_GRP1";
                    Clk1Grp2Text = "ASN1_GRP2";
                    Clk1Grp3Text = "ASN1_GRP3";

                    Clk2Grp1Text = "ASN2_GRP1";
                    Clk2Grp2Text = "ASN2_GRP2";
                    Clk2Grp3Text = "ASN2_GRP3";
                }
                else if (value == eGeneralPurpose.State)
                {
                    Clk1Grp1Text = "CLK1_GRP1";
                    Clk1Grp2Text = "CLK1_GRP2";
                    Clk1Grp3Text = "CLK1_GRP3";

                    Clk2Grp1Text = "CLK2_GRP1";
                    Clk2Grp2Text = "CLK2_GRP2";
                    Clk2Grp3Text = "CLK2_GRP3";
                }
                RaisePropertyChanged("GeneralPurposeMode");
            }
        }



        //public eSampleRate SampleRateLA
        //{
        //    get
        //    {
        //        return sampleRateLA;
        //    }

        //    set
        //    {
        //        sampleRateLA = value;
        //        if (ConfigurationMode == eConfigMode.LA_Mode)
        //            UpdateStatus(value);
        //        RaisePropertyChanged("SampleRateLA");
        //    }
        //}

        void UpdateStatus(eSampleRate sampleRate)
        {
            if (sampleRate == eSampleRate.SR_125)
                HeaderInfo = "Timing PlotView - 125 MS/s";
            else if(sampleRate == eSampleRate.SR_250)
                HeaderInfo = "Timing PlotView - 250 MS/s";
            else if (sampleRate == eSampleRate.SR_500)
                HeaderInfo = "Timing PlotView - 500 MS/s";
            else if (sampleRate == eSampleRate.SR_1000)
                HeaderInfo = "Timing PlotView - 1000 MS/s";
        }
        public eSampleRate SampleRateLAPA
        {
            get
            {
                return sampleRateLAPA;
            }

            set
            {
                sampleRateLAPA = value;
                if (ConfigurationMode == eConfigMode.Both || ConfigurationMode==eConfigMode.LA_Mode)
                    UpdateStatus(value);
                RaisePropertyChanged("SampleRateLAPA");
            }
        }
        public eGroupType GroupType
        {
            get
            {
                return groupType;
            }

            set
            {
                groupType = value;
                RaisePropertyChanged("GroupType");

                if(groupType == eGroupType.Group)
                {
                    // Add 2 groups

                }
                else
                {
                    //add 16 individual channels
                }
            }
        }
      
     



        [NonSerialized]
        private string voltText = "0x384";
        public string VoltText
        {
            get
            {
                return voltText;
            }

            set
            {
                voltText = value;
                RaisePropertyChanged("VoltText");
            }
        }

        [NonSerialized]
        private eVoltage signalAmpCh1_2 = eVoltage._1_8V;
        public eVoltage SignalAmpCh1_2
        {
            get
            {
                return signalAmpCh1_2;
            }

            set
            {
                signalAmpCh1_2 = value;
                RaisePropertyChanged("SignalAmpCh1_2");
            }
        }
        [NonSerialized]
        private eVoltage signalAmpCh3_4 = eVoltage._1_8V;
        public eVoltage SignalAmpCh3_4
        {
            get
            {
                return signalAmpCh3_4;
            }

            set
            {
                signalAmpCh3_4 = value;
                RaisePropertyChanged("SignalAmpCh3_4");
            }
        }
        [NonSerialized]
        private eVoltage signalAmpCh5_6 = eVoltage._1_8V;
        public eVoltage SignalAmpCh5_6
        {
            get
            {
                return signalAmpCh5_6;
            }

            set
            {
                signalAmpCh5_6 = value;
                RaisePropertyChanged("SignalAmpCh5_6");
            }
        }
        [NonSerialized]
        private eVoltage signalAmpCh7_8 = eVoltage._1_8V;
        public eVoltage SignalAmpCh7_8
        {
            get
            {
                return signalAmpCh7_8;
            }

            set
            {
                signalAmpCh7_8 = value;
                RaisePropertyChanged("SignalAmpCh7_8");
            }
        }

        [NonSerialized]
        private eVoltage signalAmpCh9_10 = eVoltage._1_8V;
        public eVoltage SignalAmpCh9_10
        {
            get
            {
                return signalAmpCh9_10;
            }

            set
            {
                signalAmpCh9_10 = value;
                RaisePropertyChanged("SignalAmpCh9_10");
            }
        }

        [NonSerialized]
        private eVoltage signalAmpCh11_12 = eVoltage._1_8V;
        public eVoltage SignalAmpCh11_12
        {
            get
            {
                return signalAmpCh11_12;
            }

            set
            {
                signalAmpCh11_12 = value;
                RaisePropertyChanged("SignalAmpCh11_12");
            }
        }

        [NonSerialized]
        private eVoltage signalAmpCh13_14 = eVoltage._1_8V;
        public eVoltage SignalAmpCh13_14
        {
            get
            {
                return signalAmpCh13_14;
            }

            set
            {
                signalAmpCh13_14 = value;
                RaisePropertyChanged("SignalAmpCh13_14");
            }
        }

        [NonSerialized]
        private eVoltage signalAmpCh15_16 = eVoltage._1_8V;
        public eVoltage SignalAmpCh15_16
        {
            get
            {
                return signalAmpCh15_16;
            }

            set
            {
                signalAmpCh15_16 = value;
                RaisePropertyChanged("SignalAmpCh15_16");
            }
        }

        [NonSerialized]
        private eTriggerTypeList triggerType = eTriggerTypeList.Auto;
        public eTriggerTypeList TriggerType
        {
            get { return triggerType; }
            set
            {

                triggerType = value;
                RaisePropertyChanged("TriggerType");
            }
        }

        public eProtocolTypeList ProtocolTypeIndex
        {
            get { return protocolTypeIndex; }
            set
            {
                protocolTypeIndex = value;
                RaisePropertyChanged("ProtocolTypeIndex");
            }
        }
        [NonSerialized]
        private eProtocolTypeList protocolTypeIndex = eProtocolTypeList.I2C;
        public eTimingTriggerTypeList TimingTriggerTypeSelected
        {
            get { return timingTriggerTypeSelected; }
            set
            {
                timingTriggerTypeSelected = value;
                RaisePropertyChanged("TimingTriggerTypeSelected");
            }
        }

        [NonSerialized]
        private eTimingTriggerTypeList timingTriggerTypeSelected = eTimingTriggerTypeList.Pulse_Width;

        [NonSerialized]
        private bool triggerAuto = true;
      
        [NonSerialized]
        private string headerInfo = "Timing PlotView - 125 MS/s";
        public string HeaderInfo
        {
            get
            {
                return headerInfo;
            }
            set
            {
                headerInfo = value;
                RaisePropertyChanged("HeaderInfo");
            }
        }

  



        #region WaveformLising Configuration

        [NonSerialized]
        private bool isCh16Visible = false;
        public bool IsCh16Visible
        {
            get
            {
                return isCh16Visible;
            }
            set
            {
                isCh16Visible = value;
                RaisePropertyChanged("IsCh16Visible");
            }
        }

        [NonSerialized]
        private bool isCh15Visible = false;
        public bool IsCh15Visible
        {
            get
            {
                return isCh15Visible;
            }
            set
            {
                isCh15Visible = value;
                RaisePropertyChanged("IsCh15Visible");
            }
        }

        [NonSerialized]
        private bool isCh14Visible = false;
        public bool IsCh14Visible
        {
            get
            {
                return isCh14Visible;
            }
            set
            {
                isCh14Visible = value;
                RaisePropertyChanged("IsCh14Visible");
            }
        }

        [NonSerialized]
        private bool isCh13Visible = false;
        public bool IsCh13Visible
        {
            get
            {
                return isCh13Visible;
            }
            set
            {
                isCh13Visible = value;
                RaisePropertyChanged("IsCh13Visible");
            }
        }

        [NonSerialized]
        private bool isCh12Visible = false;
        public bool IsCh12Visible
        {
            get
            {
                return isCh12Visible;
            }
            set
            {
                isCh12Visible = value;
                RaisePropertyChanged("IsCh12Visible");
            }
        }

        [NonSerialized]
        private bool isCh11Visible = false;
        public bool IsCh11Visible
        {
            get
            {
                return isCh11Visible;
            }
            set
            {
                isCh11Visible = value;
                RaisePropertyChanged("IsCh11Visible");
            }
        }

        [NonSerialized]
        private bool isCh10Visible = false;
        public bool IsCh10Visible
        {
            get
            {
                return isCh10Visible;
            }
            set
            {
                isCh10Visible = value;
                RaisePropertyChanged("IsCh10Visible");
            }
        }
        [NonSerialized]
        private bool isCh9Visible = false;
        public bool IsCh9Visible
        {
            get
            {
                return isCh9Visible;
            }
            set
            {
                isCh9Visible = value;
                RaisePropertyChanged("IsCh9Visible");
            }
        }
        [NonSerialized]
        private bool isCh8Visible = false;
        public bool IsCh8Visible
        {
            get
            {
                return isCh8Visible;
            }
            set
            {
                isCh8Visible = value;
                RaisePropertyChanged("IsCh8Visible");
            }
        }
        [NonSerialized]
        private bool isCh7Visible = false;
        public bool IsCh7Visible
        {
            get
            {
                return isCh7Visible;
            }
            set
            {
                isCh7Visible = value;
                RaisePropertyChanged("IsCh7Visible");
            }
        }
        [NonSerialized]
        private bool isCh6Visible = false;
        public bool IsCh6Visible
        {
            get
            {
                return isCh6Visible;
            }
            set
            {
                isCh6Visible = value;
                RaisePropertyChanged("IsCh6Visible");
            }
        }
        [NonSerialized]
        private bool isCh5Visible = false;
        public bool IsCh5Visible
        {
            get
            {
                return isCh5Visible;
            }
            set
            {
                isCh5Visible = value;
                RaisePropertyChanged("IsCh5Visible");
            }
        }
        [NonSerialized]
        private bool isCh4Visible = false;
        public bool IsCh4Visible
        {
            get
            {
                return isCh4Visible;
            }
            set
            {
                isCh4Visible = value;
                RaisePropertyChanged("IsCh4Visible");
            }
        }
        [NonSerialized]
        private bool isCh3Visible = false;
        public bool IsCh3Visible
        {
            get
            {
                return isCh3Visible;
            }
            set
            {
                isCh3Visible = value;
                RaisePropertyChanged("IsCh3Visible");
            }
        }
        [NonSerialized]
        private bool isCh2Visible = false;
        public bool IsCh2Visible
        {
            get
            {
                return isCh2Visible;
            }
            set
            {
                isCh2Visible = value;
                RaisePropertyChanged("IsCh2Visible");
            }
        }
        [NonSerialized]
        private bool isCh1Visible = false;
        public bool IsCh1Visible
        {
            get
            {
                return isCh1Visible;
            }
            set
            {
                isCh1Visible = value;
                RaisePropertyChanged("IsCh1Visible");
            }
        }
        [NonSerialized]
        private bool isCLK1GRP1Visible = false;
        public bool IsCLK1GRP1Visible
        {
            get
            {
                return isCLK1GRP1Visible;
            }
            set
            {
                isCLK1GRP1Visible = value;
                RaisePropertyChanged("IsCLK1GRP1Visible");
            }
        }
        [NonSerialized]
        private bool isCLK1GRP2Visible = false;
        public bool IsCLK1GRP2Visible
        {
            get
            {
                return isCLK1GRP2Visible;
            }
            set
            {
                isCLK1GRP2Visible = value;
                RaisePropertyChanged("IsCLK1GRP2Visible");
            }
        }
        [NonSerialized]
        private bool isCLK1GRP3Visible = false;
        public bool IsCLK1GRP3Visible
        {
            get
            {
                return isCLK1GRP3Visible;
            }
            set
            {
                isCLK1GRP3Visible = value;
                RaisePropertyChanged("IsCLK1GRP3Visible");
            }
        }
        [NonSerialized]
        private bool isCLK2GRP1Visible = false;
        public bool IsCLK2GRP1Visible
        {
            get
            {
                return isCLK2GRP1Visible;
            }
            set
            {
                isCLK2GRP1Visible = value;
                RaisePropertyChanged("IsCLK2GRP1Visible");
            }
        }
        [NonSerialized]
        private bool isCLK2GRP2Visible = false;
        public bool IsCLK2GRP2Visible
        {
            get
            {
                return isCLK2GRP2Visible;
            }
            set
            {
                isCLK2GRP2Visible = value;
                RaisePropertyChanged("IsCLK2GRP2Visible");
            }
        }
        [NonSerialized]
        private bool isCLK2GRP3Visible = false;
        public bool IsCLK2GRP3Visible
        {
            get
            {
                return isCLK2GRP3Visible;
            }
            set
            {
                isCLK2GRP3Visible = value;
                RaisePropertyChanged("IsCLK2GRP3Visible");
            }
        }

        #endregion


        #region properties
        private bool isCh1ClkVisible = false;
        public bool IsCh1ClkVisible
        {
            get
            {
                return isCh1ClkVisible;
            }
            set
            {
                isCh1ClkVisible = value;
                RaisePropertyChanged("IsCh1ClkVisible");
            }
        }
        private bool isCh2ClkVisible = false;
        public bool IsCh2ClkVisible
        {
            get
            {
                return isCh2ClkVisible;
            }
            set
            {
                isCh2ClkVisible = value;
                RaisePropertyChanged("IsCh2ClkVisible");
            }
        }
        private bool isCh3ClkVisible = false;
        public bool IsCh3ClkVisible
        {
            get
            {
                return isCh3ClkVisible;
            }
            set
            {
                isCh3ClkVisible = value;
                RaisePropertyChanged("IsCh3ClkVisible");
            }
        }

        private bool isCh4ClkVisible = false;
        public bool IsCh4ClkVisible
        {
            get
            {
                return isCh4ClkVisible;
            }
            set
            {
                isCh4ClkVisible = value;
                RaisePropertyChanged("IsCh4ClkVisible");
            }
        }

        private bool isCh5ClkVisible = false;
        public bool IsCh5ClkVisible
        {
            get
            {
                return isCh5ClkVisible;
            }
            set
            {
                isCh5ClkVisible = value;
                RaisePropertyChanged("IsCh5ClkVisible");
            }
        }

        private bool isCh6ClkVisible = false;
        public bool IsCh6ClkVisible
        {
            get
            {
                return isCh6ClkVisible;
            }
            set
            {
                isCh6ClkVisible = value;
                RaisePropertyChanged("IsCh6ClkVisible");
            }
        }
        private bool isCh7ClkVisible = false;
        public bool IsCh7ClkVisible
        {
            get
            {
                return isCh7ClkVisible;
            }
            set
            {
                isCh7ClkVisible = value;
                RaisePropertyChanged("IsCh7ClkVisible");
            }
        }
        private bool isCh8ClkVisible = false;
        public bool IsCh8ClkVisible
        {
            get
            {
                return isCh8ClkVisible;
            }
            set
            {
                isCh8ClkVisible = value;
                RaisePropertyChanged("IsCh8ClkVisible");
            }
        }

        private bool isCh9ClkVisible = false;
        public bool IsCh9ClkVisible
        {
            get
            {
                return isCh9ClkVisible;
            }
            set
            {
                isCh9ClkVisible = value;
                RaisePropertyChanged("IsCh9ClkVisible");
            }
        }


        private bool isCh10ClkVisible = false;
        public bool IsCh10ClkVisible
        {
            get
            {
                return isCh10ClkVisible;
            }
            set
            {
                isCh10ClkVisible = value;
                RaisePropertyChanged("IsCh10ClkVisible");
            }
        }


        private bool isCh11ClkVisible = false;
        public bool IsCh11ClkVisible
        {
            get
            {
                return isCh11ClkVisible;
            }
            set
            {
                isCh11ClkVisible = value;
                RaisePropertyChanged("IsCh11ClkVisible");
            }
        }


        private bool isCh12ClkVisible = false;
        public bool IsCh12ClkVisible
        {
            get
            {
                return isCh12ClkVisible;
            }
            set
            {
                isCh12ClkVisible = value;
                RaisePropertyChanged("IsCh12ClkVisible");
            }
        }

        private bool isCh13ClkVisible = false;
        public bool IsCh13ClkVisible
        {
            get
            {
                return isCh13ClkVisible;
            }
            set
            {
                isCh13ClkVisible = value;
                RaisePropertyChanged("IsCh13ClkVisible");
            }
        }

        private bool isCh14ClkVisible = false;
        public bool IsCh14ClkVisible
        {
            get
            {
                return isCh14ClkVisible;
            }
            set
            {
                isCh14ClkVisible = value;
                RaisePropertyChanged("IsCh14ClkVisible");
            }
        }

        private bool isCh15ClkVisible = false;
        public bool IsCh15ClkVisible
        {
            get
            {
                return isCh15ClkVisible;
            }
            set
            {
                isCh15ClkVisible = value;
                RaisePropertyChanged("IsCh15ClkVisible");
            }
        }

        private bool isCh16ClkVisible = false;
        public bool IsCh16ClkVisible
        {
            get
            {
                return isCh16ClkVisible;
            }
            set
            {
                isCh16ClkVisible = value;
                RaisePropertyChanged("IsCh16ClkVisible");
            }
        }
        #endregion
    }
}
