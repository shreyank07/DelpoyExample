using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
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
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;

namespace PGYMiniCooper.CoreModule.ViewModel
{
    public class WaveformListingModel : ViewModelBase
    {
        private readonly ConfigModel config;

        public WaveformListingModel(ConfigModel config, WaveformListHolder waveformHolder)
        {
            this.config = config;
            WfmHolder = waveformHolder;
            markerValueList = new List<int>() { 1, 2, 3, 4, 5, 6 };
            #region Markers
            MarkerList = new List<string>();
            MarkerList.Add("M1");
            MarkerList.Add("M2");
            MarkerList.Add("M3");
            MarkerList.Add("M4");
            MarkerList.Add("M5");
            MarkerList.Add("M6");

            markerTimingCollection = new ObservableCollection<MarkerTimingModel>();
            MarkerTimingCollection.Add(new MarkerTimingModel());
            this.checksetmarker = false;
            #endregion
        }

        public IEnumerable<eDataFormat> DataFormatList
        {
            get
            {
                return Enum.GetValues(typeof(eDataFormat)).Cast<eDataFormat>();
            }
        }
        List<int> markerValueList = new List<int>();
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

        #region Channel Names

        private string ch16Name = "CH16";

        public string Ch16Name
        {
            get
            {
                return ch16Name;
            }
            set
            {
                ch16Name = value;
                RaisePropertyChanged("Ch16Name");
            }
        }

        private string ch15Name = "CH15";

        public string Ch15Name
        {
            get
            {
                return ch15Name;
            }
            set
            {
                ch15Name = value;
                RaisePropertyChanged("Ch15Name");
            }
        }

        private string ch14Name = "CH14";

        public string Ch14Name
        {
            get
            {
                return ch14Name;
            }
            set
            {
                ch14Name = value;
                RaisePropertyChanged("Ch14Name");
            }
        }

        private string ch13Name = "CH13";

        public string Ch13Name
        {
            get
            {
                return ch13Name;
            }
            set
            {
                ch13Name = value;
                RaisePropertyChanged("Ch13Name");
            }
        }

        private string ch12Name = "CH12";

        public string Ch12Name
        {
            get
            {
                return ch12Name;
            }
            set
            {
                ch12Name = value;
                RaisePropertyChanged("Ch12Name");
            }
        }

        private string ch11Name = "CH11";

        public string Ch11Name
        {
            get
            {
                return ch11Name;
            }
            set
            {
                ch11Name = value;
                RaisePropertyChanged("Ch11Name");
            }
        }

        private string ch10Name = "CH10";

        public string Ch10Name
        {
            get
            {
                return ch10Name;
            }
            set
            {
                ch10Name = value;
                RaisePropertyChanged("Ch10Name");
            }
        }

        private string ch9Name = "CH9";

        public string Ch9Name
        {
            get
            {
                return ch9Name;
            }
            set
            {
                ch9Name = value;
                RaisePropertyChanged("Ch9Name");
            }
        }

        private string ch8Name = "CH8";

        public string Ch8Name
        {
            get
            {
                return ch8Name;
            }
            set
            {
                ch8Name = value;
                RaisePropertyChanged("Ch8Name");
            }
        }

        private string ch7Name = "CH7";

        public string Ch7Name
        {
            get
            {
                return ch7Name;
            }
            set
            {
                ch7Name = value;
                RaisePropertyChanged("Ch7Name");
            }
        }

        private string ch6Name = "CH6";

        public string Ch6Name
        {
            get
            {
                return ch6Name;
            }
            set
            {
                ch6Name = value;
                RaisePropertyChanged("Ch6Name");
            }
        }

        private string ch5Name = "CH5";

        public string Ch5Name
        {
            get
            {
                return ch5Name;
            }
            set
            {
                ch5Name = value;
                RaisePropertyChanged("Ch5Name");
            }
        }

        private string ch4Name = "CH4";

        public string Ch4Name
        {
            get
            {
                return ch4Name;
            }
            set
            {
                ch4Name = value;
                RaisePropertyChanged("Ch4Name");
            }
        }

        private string ch3Name = "CH3";

        public string Ch3Name
        {
            get
            {
                return ch3Name;
            }
            set
            {
                ch3Name = value;
                RaisePropertyChanged("Ch3Name");
            }
        }

        private string ch2Name = "CH2";

        public string Ch2Name
        {
            get
            {
                return ch2Name;
            }
            set
            {
                ch2Name = value;
                RaisePropertyChanged("Ch2Name");
            }
        }

        private string ch1Name = "CH1";

        public string Ch1Name
        {
            get
            {
                return ch1Name;
            }
            set
            {
                ch1Name = value;
                RaisePropertyChanged("Ch1Name");
            }
        }
        #endregion

        public void Reset()
        {
            var i2CConfig = config.ProtocolConfigList.OfType<ConfigModel_I2C>().FirstOrDefault();
            var spiConfig = config.ProtocolConfigList.OfType<ConfigModel_SPI>().FirstOrDefault();
            var UARTConfig = config.ProtocolConfigList.OfType<ConfigModel_UART>().FirstOrDefault();
            var i3cConfig = config.ProtocolConfigList.OfType<ConfigModel_I3C>().FirstOrDefault();
            var spmiConfig = config.ProtocolConfigList.OfType<ConfigModel_SPMI>().FirstOrDefault();
            var rffeconfig=config.ProtocolConfigList.OfType<ConfigModel_RFFE>().FirstOrDefault();
            var qspiconfig = config.ProtocolConfigList.OfType<ConfigModel_QSPI>().FirstOrDefault();
            var canconfig=config.ProtocolConfigList.OfType<ConfigModel_CAN>().FirstOrDefault();
            MarkerNumber = 0;
            MarkerName = "";
            MarkerNumber = 0;
            Markervalue.Clear();

            EventMarker2 = -1;
            EventMarker3 = -1;
            EventMarker4 = -1;
            EventMarker5 = -1;
            EventMarker6 = -1;
            EventMarker7 = -1;

            Difference1 = "0s";
            Difference2 = "0s";
            Difference3 = "0s";

            Checksetmarker = false;
            EventMarker1 = this.SelectedIndex;

            Ch1Name = "CH1";
            Ch2Name = "CH2";
            Ch3Name = "CH3";
            Ch4Name = "CH4";
            Ch5Name = "CH5";
            Ch6Name = "CH6";
            Ch7Name = "CH7";
            Ch8Name = "CH8";
            Ch9Name = "CH9";
            Ch10Name = "CH10";
            Ch11Name = "CH11";
            Ch12Name = "CH12";
            Ch13Name = "CH13";
            Ch14Name = "CH14";
            Ch15Name = "CH15";
            Ch16Name = "CH16";

            if (config.ConfigurationMode == eConfigMode.LA_Mode && config.GeneralPurposeMode == eGeneralPurpose.State)
            {
                UpdateClockChannelName(config.SelectedClock1, 1);
                if (config.HasTwoClockSources)
                    UpdateClockChannelName(config.SelectedClock2, 2);
            }
            else if (config.ConfigurationMode == eConfigMode.PA_Mode || config.ConfigurationMode == eConfigMode.Both)
            {
                if (i2CConfig != null)
                {
                    if (i2CConfig.ChannelIndex_SCL != eChannles.None)
                    {
                        UpdateChannelName(i2CConfig.ChannelIndex_SCL, "I2C Clk");
                    }
                    if (i2CConfig.ChannelIndex_SDA != eChannles.None)
                    {
                        UpdateChannelName(i2CConfig.ChannelIndex_SDA, "I2C Data");
                    }
                }
                if (spiConfig != null)
                {
                    if (spiConfig.ChannelIndex_CLK != eChannles.None)
                    {
                        UpdateChannelName(spiConfig.ChannelIndex_CLK, "SPI Clk");
                    }
                    if (spiConfig.ChannelIndex_CS != eChannles.None)
                    {
                        UpdateChannelName(spiConfig.ChannelIndex_CS, "SPI CS");
                    }
                    if (spiConfig.ChannelIndex_MOSI != eChannles.None)
                    {
                        UpdateChannelName(spiConfig.ChannelIndex_MOSI, "SPI MOSI");
                    }
                    if (spiConfig.ChannelIndex_MISO != eChannles.None)
                    {
                        UpdateChannelName(spiConfig.ChannelIndex_MISO, "SPI MISO");
                    }
                }
                if (UARTConfig!=null)
                {
                    if (UARTConfig.ChannelIndex_TX != eChannles.None)
                    {
                        UpdateChannelName(UARTConfig.ChannelIndex_TX, "UART TX");
                    }
                    if (UARTConfig.ChannelIndex_RX != eChannles.None)
                    {
                        UpdateChannelName(UARTConfig.ChannelIndex_RX, "UART RX");
                    }
                }
                if (i3cConfig!=null)
                {
                    if (i3cConfig.ChannelIndex_SCL != eChannles.None)
                    {
                        UpdateChannelName(i3cConfig.ChannelIndex_SCL, "I3C Clk");
                    }
                    if (i3cConfig.ChannelIndex_SDA != eChannles.None)
                    {
                        UpdateChannelName(i3cConfig.ChannelIndex_SDA, "I3C Data");
                    }
                }
                if (spmiConfig!=null)
                {
                    if (spmiConfig.ChannelIndex_SCL != eChannles.None)
                    {
                        UpdateChannelName(spmiConfig.ChannelIndex_SCL, "SPMI Clk");
                    }
                    if (spmiConfig.ChannelIndex_SDA != eChannles.None)
                    {
                        UpdateChannelName(spmiConfig.ChannelIndex_SDA, "SPMI Data");
                    }
                }
                if (rffeconfig!=null)
                {
                    if (rffeconfig.ChannelIndex_SCL != eChannles.None)
                    {
                        UpdateChannelName(rffeconfig.ChannelIndex_SCL, "RFFE Clk");
                    }
                    if (rffeconfig.ChannelIndex_SDA != eChannles.None)
                    {
                        UpdateChannelName(rffeconfig.ChannelIndex_SDA, "RFFE Data");
                    }
                }

                //QSPI 

                if (qspiconfig!=null)
                {
                    if (qspiconfig.ChannelIndex_CLK != eChannles.None)
                    {
                        UpdateChannelName(qspiconfig.ChannelIndex_CLK, "QSPI CLK");
                    }
                    if (qspiconfig.ChannelIndex_CS != eChannles.None)
                    {
                        UpdateChannelName(qspiconfig.ChannelIndex_CS, "QSPI CS");
                    }
                    if (qspiconfig.ChannelIndex_D0 != eChannles.None)
                    {
                        UpdateChannelName(qspiconfig.ChannelIndex_D0, "QSPI D0");
                    }
                    if (qspiconfig.ChannelIndex_D1 != eChannles.None)
                    {
                        UpdateChannelName(qspiconfig.ChannelIndex_D1, "QSPI D1");
                    }
                    if (qspiconfig.ChannelIndex_D2 != eChannles.None)
                    {
                        UpdateChannelName(qspiconfig.ChannelIndex_D2, "QSPI D2");
                    }
                    if (qspiconfig.ChannelIndex_D3 != eChannles.None)
                    {
                        UpdateChannelName(qspiconfig.ChannelIndex_D3, "QSPI D3");
                    }
                }

                if (canconfig!=null)
                {
                    if (canconfig.ChannelIndex != eChannles.None)
                    {
                        UpdateChannelName(canconfig.ChannelIndex, "CAN RX");
                    }
                }
            }
        }
        private void UpdateChannelName(eChannles chnl, string Text)
        {
            switch (chnl)
            {
                case eChannles.CH1:
                    Ch1Name = "CH1 -" + Text;
                    break;

                case eChannles.CH2:
                    Ch2Name = "CH2 -" + Text;
                    break;

                case eChannles.CH3:
                    Ch3Name = "CH3 -" + Text;
                    break;

                case eChannles.CH4:
                    Ch4Name = "CH4 -" + Text;
                    break;

                case eChannles.CH5:
                    Ch5Name = "CH5 -" + Text;
                    break;

                case eChannles.CH6:
                    Ch6Name = "CH6 -" + Text;
                    break;

                case eChannles.CH7:
                    Ch7Name = "CH7 -" + Text;
                    break;

                case eChannles.CH8:
                    Ch8Name = "CH8 -" + Text;
                    break;

                case eChannles.CH9:
                    Ch9Name = "CH9 -" + Text;
                    break;

                case eChannles.CH10:
                    Ch10Name = "CH10 -" + Text;
                    break;

                case eChannles.CH11:
                    Ch11Name = "CH11 -" + Text;
                    break;

                case eChannles.CH12:
                    Ch12Name = "CH12 -" + Text;
                    break;

                case eChannles.CH13:
                    Ch13Name = "CH13 -" + Text;
                    break;

                case eChannles.CH14:
                    Ch14Name = "CH14 -" + Text;
                    break;

                case eChannles.CH15:
                    Ch15Name = "CH15 -" + Text;
                    break;

                case eChannles.CH16:
                    Ch16Name = "CH16 -" + Text;
                    break;
            }
        }

        private void UpdateClockChannelName(eChannles clock, int num)
        {
            switch (clock)
            {
                case eChannles.CH1:
                    Ch1Name = "Clk" + num + " - CH1";
                    break;

                case eChannles.CH2:
                    Ch2Name = "Clk" + num + " - CH2";
                    break;

                case eChannles.CH3:
                    Ch3Name = "Clk" + num + " - CH3";
                    break;

                case eChannles.CH4:
                    Ch4Name = "Clk" + num + " - CH4";
                    break;

                case eChannles.CH5:
                    Ch5Name = "Clk" + num + " - CH5";
                    break;

                case eChannles.CH6:
                    Ch6Name = "Clk" + num + " - CH6";
                    break;

                case eChannles.CH7:
                    Ch7Name = "Clk" + num + " - CH7";
                    break;

                case eChannles.CH8:
                    Ch8Name = "Clk" + num + " - CH8";
                    break;

                case eChannles.CH9:
                    Ch9Name = "Clk" + num + " - CH9";
                    break;

                case eChannles.CH10:
                    Ch10Name = "Clk" + num + " - CH10";
                    break;

                case eChannles.CH11:
                    Ch11Name = "Clk" + num + " - CH11";
                    break;

                case eChannles.CH12:
                    Ch12Name = "Clk" + num + " - CH12";
                    break;

                case eChannles.CH13:
                    Ch13Name = "Clk" + num + " - CH13";
                    break;

                case eChannles.CH14:
                    Ch14Name = "Clk" + num + " - CH14";
                    break;

                case eChannles.CH15:
                    Ch15Name = "Clk" + num + " - CH15";
                    break;

                case eChannles.CH16:
                    Ch16Name = "Clk" + num + " - CH16";
                    break;

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

        WfmListModel selectedFrame;

        public WfmListModel SelectedFrame
        {
            get
            {
                return selectedFrame;
            }
            set
            {
                selectedFrame = value;
                RaisePropertyChanged("SelectedFrame");
            }
        }

        private Command gotoTrigger;
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

        #region Markers

        private List<string> markerList;
        public List<string> MarkerList
        {
            get { return markerList; }
            set
            {
                markerList = value;
                RaisePropertyChanged("MarkerList");
            }
        }

        private string marker1 = "M1";
        public string Marker1
        {
            get { return marker1; }
            set
            {
                marker1 = value;
                if ((Markervalue.Count >= GetMarkerCount(marker1)) && (Markervalue.Count >= GetMarkerCount(marker2)))
                {
                    double temp1 = Math.Abs(Markervalue[GetMarkerCount(marker2) - 1].Timestamp - Markervalue[GetMarkerCount(marker1) - 1].Timestamp);
                    Difference1 = CUtilities.ToEngineeringNotation(temp1) + "s";
                }
                RaisePropertyChanged("Marker1");
            }
        }

        private string marker2 = "M2";
        public string Marker2
        {
            get { return marker2; }
            set
            {
                marker2 = value;
                if ((Markervalue.Count >= GetMarkerCount(marker1)) && (Markervalue.Count >= GetMarkerCount(marker2)))
                {
                    double temp1 = Math.Abs(Markervalue[GetMarkerCount(marker2) - 1].Timestamp - Markervalue[GetMarkerCount(marker1) - 1].Timestamp);
                    Difference1 = CUtilities.ToEngineeringNotation(temp1) + "s";
                }
                RaisePropertyChanged("Marker2");
            }
        }

        private string marker3 = "M3";
        public string Marker3
        {
            get { return marker3; }
            set
            {
                marker3 = value;
                if ((Markervalue.Count >= GetMarkerCount(marker3)) && (Markervalue.Count >= GetMarkerCount(marker4)))
                {
                    double temp2 = Math.Abs(Markervalue[GetMarkerCount(marker4) - 1].Timestamp - Markervalue[GetMarkerCount(marker3) - 1].Timestamp);
                    Difference2 = CUtilities.ToEngineeringNotation(temp2) + "s";
                }
                RaisePropertyChanged("Marker3");
            }
        }

        private string marker4 = "M4";
        public string Marker4
        {
            get { return marker4; }
            set
            {
                marker4 = value;
                if ((Markervalue.Count >= GetMarkerCount(marker3)) && (Markervalue.Count >= GetMarkerCount(marker4)))
                {
                    double temp2 = Math.Abs(Markervalue[GetMarkerCount(marker4) - 1].Timestamp - Markervalue[GetMarkerCount(marker3) - 1].Timestamp);
                    Difference2 = CUtilities.ToEngineeringNotation(temp2) + "s";
                }
                RaisePropertyChanged("Marker4");
            }
        }

        private string marker5 = "M5";
        public string Marker5
        {
            get { return marker5; }
            set
            {
                marker5 = value;
                if ((Markervalue.Count >= GetMarkerCount(marker5)) && (Markervalue.Count >= GetMarkerCount(marker6)))
                {
                    double temp3 = Math.Abs(Markervalue[GetMarkerCount(marker6) - 1].Timestamp - Markervalue[GetMarkerCount(marker5) - 1].Timestamp);
                    Difference3 = CUtilities.ToEngineeringNotation(temp3) + "s";
                }
                RaisePropertyChanged("Marker5");
            }
        }

        private string marker6 = "M6";
        public string Marker6
        {
            get { return marker6; }
            set
            {
                marker6 = value;
                if ((Markervalue.Count >= GetMarkerCount(marker5)) && (Markervalue.Count >= GetMarkerCount(marker6)))
                {
                    double temp3 = Math.Abs(Markervalue[GetMarkerCount(marker6) - 1].Timestamp - Markervalue[GetMarkerCount(marker5) - 1].Timestamp);
                    Difference3 = CUtilities.ToEngineeringNotation(temp3) + "s";
                }
                RaisePropertyChanged("Marker6");
            }
        }

        private string difference1 = "-";
        public string Difference1
        {
            get { return difference1; }
            set
            {
                difference1 = value;
                RaisePropertyChanged("Difference1");
            }
        }

        private string difference2 = "-";
        public string Difference2
        {
            get { return difference2; }
            set
            {
                difference2 = value;
                RaisePropertyChanged("Difference2");
            }
        }

        private string difference3 = "-";
        public string Difference3
        {
            get { return difference3; }
            set
            {
                difference3 = value;
                RaisePropertyChanged("Difference3");
            }
        }

        //datarowheader content trigger
        private long? _EventMarker1;
        public long? EventMarker1
        {
            get { return _EventMarker1; }
            set
            {
                _EventMarker1 = value;
                RaisePropertyChanged("EventMarker1");
            }
        }


        //textbox 
        private long? _EventMarker2;
        public long? EventMarker2
        {
            get { return _EventMarker2; }
            set
            {
                _EventMarker2 = value;
                RaisePropertyChanged("EventMarker2");
            }
        }

        private long? _EventMarker3;
        public long? EventMarker3
        {
            get { return _EventMarker3; }
            set
            {
                _EventMarker3 = value;
                RaisePropertyChanged("EventMarker3");
            }
        }

        private long? _EventMarker4;
        public long? EventMarker4
        {
            get { return _EventMarker4; }
            set
            {
                _EventMarker4 = value;
                RaisePropertyChanged("EventMarker4");
            }
        }

        private long? _EventMarker5;
        public long? EventMarker5
        {
            get { return _EventMarker5; }
            set
            {
                _EventMarker5 = value;
                RaisePropertyChanged("EventMarker5");
            }
        }

        private long? _EventMarker6;
        public long? EventMarker6
        {
            get { return _EventMarker6; }
            set
            {
                _EventMarker6 = value;
                RaisePropertyChanged("EventMarker6");
            }
        }

        private long? _EventMarker7;
        public long? EventMarker7
        {
            get { return _EventMarker7; }
            set
            {
                _EventMarker7 = value;
                RaisePropertyChanged("EventMarker7");
            }
        }

        private ObservableCollection<Markerdetails> markervalue;
        public ObservableCollection<Markerdetails> Markervalue
        {
            get
            {
                if (markervalue == null)
                    markervalue = new ObservableCollection<Markerdetails>();
                return markervalue;
            }
            set
            {
                markervalue = value;
            }
        }

        private ObservableCollection<MarkerTimingModel> markerTimingCollection;
        public ObservableCollection<MarkerTimingModel> MarkerTimingCollection
        {
            get
            {
                return markerTimingCollection;
            }
            set
            {
                markerTimingCollection = value;
                RaisePropertyChanged("MarkerTimingCollection");
            }
        }

        private string markername;
        public string MarkerName
        {
            get { return markername; }
            set
            {
                markername = value;

                RaisePropertyChanged("MarkerName");
                PreviousMarker = markername;
            }
        }

        private string markername1;
        public string MarkerName1
        {
            get { return markername1; }
            set
            {
                markername1 = value;

                RaisePropertyChanged("MarkerName1");
            }
        }

        private string markername2;
        public string MarkerName2
        {
            get { return markername2; }
            set
            {
                markername2 = value;

                RaisePropertyChanged("MarkerName2");
            }
        }

        private string markername3;
        public string MarkerName3
        {
            get { return markername3; }
            set
            {
                markername3 = value;

                RaisePropertyChanged("MarkerName3");
            }
        }

        private string markername4;
        public string MarkerName4
        {
            get { return markername4; }
            set
            {
                markername4 = value;

                RaisePropertyChanged("MarkerName4");
            }
        }

        private string markername5;
        public string MarkerName5
        {
            get { return markername5; }
            set
            {
                markername5 = value;

                RaisePropertyChanged("MarkerName5");
            }
        }

        private string markername6;
        public string MarkerName6
        {
            get { return markername6; }
            set
            {
                markername6 = value;

                RaisePropertyChanged("MarkerName6");
            }
        }

        private string previousMarker;
        public string PreviousMarker
        {
            get { return previousMarker; }
            set
            {
                previousMarker = value;
            }
        }

        private long previousMarkerIndex;
        public long PreviousMarkerIndex
        {
            get { return previousMarkerIndex; }
            set
            {
                previousMarkerIndex = value;
            }
        }

        private long markertextboxindex;
        public long Markertextboxindex
        {
            get { return markertextboxindex; }
            set
            {
                markertextboxindex = value;
            }
        }

        private int markernumber;
        public int MarkerNumber
        {
            get { return markernumber; }
            set
            {
                markernumber = value;
            }
        }

        private bool markertextboxreadonly;
        public bool Markertextboxreadonly
        {
            get
            {
                return markertextboxreadonly;
            }
            set
            {
                markertextboxreadonly = value;
                RaisePropertyChanged("Markertextboxreadonly");
            }
        }

        private Brush markertextboxcolor;
        public Brush Markertextboxcolor
        {
            get
            {
                return markertextboxcolor;
            }
            set
            {
                markertextboxcolor = value;
                RaisePropertyChanged("Markertextboxcolor");
            }
        }

        private bool markersetbutton;
        public bool Markersetbutton
        {
            get
            {
                return markersetbutton;
            }
            set
            {
                markersetbutton = value;
            }
        }

        private bool checksetmarker;
        public bool Checksetmarker
        {
            get
            {
                return checksetmarker;
            }
            set
            {
                checksetmarker = value;
                RaisePropertyChanged("Checksetmarker");
            }
        }

        private string renamemarkername;
        public string Renamemarkername
        {
            get
            {
                return renamemarkername;
            }
            set
            {
                renamemarkername = value;
            }
        }

        //Set unlimited markers
        private Command setMarker;
        public Command SetMarker
        {
            get
            {
                if (setMarker == null)
                    setMarker = new Command(new Command.ICommandOnExecute(SetMarkerMethod));

                return setMarker;
            }
        }

        public void SetMarkerMethod()
        {
            //Set markername
            if (Markervalue.Any(p => p.Index == this.SelectedIndex))
            {
                MessageBox.Show("Marker Already Set", "Markers", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (Markervalue.Count > 5)
            {
                MessageBox.Show("Markers limits exceeds..", "Markers", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MarkerNumber = markerValueList.FirstOrDefault();
            MarkerName = "M" + (MarkerNumber).ToString();
            markerValueList.RemoveAt(0);

            foreach (var each in Markervalue)
            {
                if (each.Markername == MarkerName)
                {
                    MarkerName = "M" + (MarkerNumber).ToString();
                }
            }
            Renamemarkername = MarkerName;

            if (Markervalue != null)
            {
                foreach (var each in Markervalue)
                {
                    if (each.Index == this.SelectedIndex)
                    {
                        Markervalue.Remove(each);
                        break;
                    }

                }
            }
            Markerdetails marker = new Markerdetails();
            marker.Index = this.SelectedFrame.Index;
            marker.Markername = MarkerName;
            marker.Timestamp = SelectedFrame.Timestamp - wfmHolder.TriggerTime;  // add proper time stamp.
            Markervalue.Add(marker);

            markersetbutton = true;
            Markertextboxreadonly = false;
            Markertextboxcolor = System.Windows.Media.Brushes.White;
            Markertextboxindex = this.SelectedIndex;
            if (Markervalue.Count == 1)
            {
                EventMarker2 = this.SelectedIndex;
                MarkerName1 = MarkerName;
            }
            else if (Markervalue.Count == 2)
            {
                EventMarker3 = this.SelectedIndex;
                MarkerName2 = MarkerName;
            }
            else if (Markervalue.Count == 3)
            {
                EventMarker4 = this.SelectedIndex;
                MarkerName3 = MarkerName;
            }
            else if (Markervalue.Count == 4)
            {
                EventMarker5 = this.SelectedIndex;
                MarkerName4 = MarkerName;
            }
            else if (Markervalue.Count == 5)
            {
                EventMarker6 = this.SelectedIndex;
                MarkerName5 = MarkerName;
            }
            else if (Markervalue.Count == 6)
            {
                EventMarker7 = this.SelectedIndex;
                MarkerName6 = MarkerName;
            }
            EventMarker1 = this.SelectedIndex;
            PreviousMarkerIndex = this.SelectedIndex;

            int num = Markervalue.Count;
            try
            {
                if ((MarkerName == marker1 && num >= GetMarkerCount(marker2)) || (MarkerName == marker2 && num >= GetMarkerCount(marker1)))
                {
                    double temp1 = Math.Abs(Markervalue[GetMarkerCount(marker2) - 1].Timestamp - Markervalue[GetMarkerCount(marker1) - 1].Timestamp);
                    Difference1 = CUtilities.ToEngineeringNotation(temp1) + "s";
                }
                else if ((MarkerName == marker3 && num >= GetMarkerCount(marker4)) || (MarkerName == marker4 && num >= GetMarkerCount(marker3)))
                {
                    double temp2 = Math.Abs(Markervalue[GetMarkerCount(marker4) - 1].Timestamp - Markervalue[GetMarkerCount(marker3) - 1].Timestamp);
                    Difference2 = CUtilities.ToEngineeringNotation(temp2) + "s";
                }
                else if ((MarkerName == marker5 && num >= GetMarkerCount(marker6)) || (MarkerName == marker6 && num >= GetMarkerCount(marker5)))
                {
                    double temp3 = Math.Abs(Markervalue[GetMarkerCount(marker6) - 1].Timestamp - Markervalue[GetMarkerCount(marker5) - 1].Timestamp);
                    Difference3 = CUtilities.ToEngineeringNotation(temp3) + "s";
                }
            }
            catch (Exception ex)
            {

            }
            //if two markers are set, Show difference in Toolbar
            //if (num == 2)
            //{
            //    //Set SelectedItem after clearing markers
            //    MarkerTimingCollection[0].Start = Markervalue[0];
            //    MarkerTimingCollection[0].Stop = Markervalue[1];

            //    MarkerTimingCollection[0].Difference = Math.Abs(Markervalue[0].Timestamp - Markervalue[1].Timestamp);
            //    Checksetmarker = true;
            //}
        }

        private int GetMarkerCount(string markerName)
        {
            int count = 0;
            string temp = markerName.Substring(markerName.Length - 1);
            int.TryParse(temp, out count);
            return count;
        }

        //Clear selected Marker
        private Command clearMarkerCommand;
        public Command ClearMarkerCommand
        {
            get
            {
                if (clearMarkerCommand == null)
                    clearMarkerCommand = new Command(new Command.ICommandOnExecute(ClearMarkerMethod));

                return clearMarkerCommand;
            }
        }

        public void ClearMarkerMethod()
        {
            int check = 0;
            if (Markervalue != null)
            {
                foreach (var each in Markervalue)
                {
                    if (each.Index == this.SelectedIndex)
                    {
                        Markervalue.Remove(each);
                        markerValueList.Add(Convert.ToInt32(each.Markername.Substring(1)));
                        check = 1;
                        if (this.SelectedIndex == EventMarker2)
                        {
                            EventMarker2 = -1;
                            Difference1 = "0s";
                        }
                        else if (this.SelectedIndex == EventMarker3)
                        {
                            EventMarker3 = -1;
                            Difference1 = "0s";
                        }
                        else if (this.SelectedIndex == EventMarker4)
                        {
                            EventMarker4 = -1;
                            Difference2 = "0s";
                        }
                        else if (this.SelectedIndex == EventMarker5)
                        {
                            EventMarker5 = -1;
                            Difference2 = "0s";
                        }
                        else if (this.SelectedIndex == EventMarker6)
                        {
                            EventMarker6 = -1;
                            Difference3 = "0s";
                        }
                        else if (this.SelectedIndex == EventMarker7)
                        {
                            EventMarker7 = -1;
                            Difference3 = "0s";
                        }
                        break;
                    }
                }

                if (previousMarkerIndex == this.SelectedIndex)
                    MarkerName = "";

                if (check == 1)
                {
                    EventMarker1 = this.SelectedIndex;
                    if (Markervalue.Count < 2)
                    {

                        Checksetmarker = false;
                    }
                    else
                    {
                        //Set SelectedItem after clearing markers
                        MarkerTimingCollection[0].Start = Markervalue[0];
                        MarkerTimingCollection[0].Stop = Markervalue[1];
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("No marker set for the SelectedIndex..", "PGY I3C", MessageBoxButton.OK);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("No Marker has been Set..", "PGY I3C", MessageBoxButton.OK);
            }
        }

        //Clear All Markers
        private Command clearAllMarkerCommand;
        public Command ClearAllMarkerCommand
        {
            get
            {
                if (clearAllMarkerCommand == null)
                    clearAllMarkerCommand = new Command(new Command.ICommandOnExecute(ClearAllMarkersMethod));

                return clearAllMarkerCommand;
            }
        }

        public void ClearAllMarkersMethod()
        {
            MarkerNumber = 0;
            markerValueList = new List<int>() { 1, 2, 3, 4, 5, 6 };
            if (Markervalue.Count != 0)
            {
                MarkerName = "";
                MarkerNumber = 0;
                Markervalue.Clear();

                EventMarker2 = -1;
                EventMarker3 = -1;
                EventMarker4 = -1;
                EventMarker5 = -1;
                EventMarker6 = -1;
                EventMarker7 = -1;

                Difference1 = "0s";
                Difference2 = "0s";
                Difference3 = "0s";

                Checksetmarker = false;
                EventMarker1 = this.SelectedIndex;
            }
            else
            {
                System.Windows.MessageBox.Show("No Marker has been Set..", "PGY I3C", MessageBoxButton.OK);
            }
        }

        //Goto particular markers
        private RelayCommand<object> gotoMarker;
        public RelayCommand<object> GotoMarkers
        {
            get
            {
                if (gotoMarker == null)
                    gotoMarker = new RelayCommand<object>(
                        (selectedMarker) =>
                        {
                            if (selectedMarker != null)
                            {

                                this.SelectedIndex = (selectedMarker as Markerdetails).Index;

                            }
                        });
                return gotoMarker;
            }
        }

        //private Command gotoTrigger;
        //public Command GotoTrigger
        //{
        //    get
        //    {
        //        if (gotoTrigger == null)
        //            gotoTrigger = new Command(new Command.ICommandOnExecute(gotoTriggerMethod));
        //        return gotoTrigger;
        //    }
        //}

        //void gotoTriggerMethod()
        //{
        //    if (WaveformListHolder.GetInstance().TriggerPacket != -1)
        //    {
        //        this.SelectedIndex = WaveformListHolder.GetInstance().TriggerPacket;
        //    }
        //    else
        //    {
        //        //Task.Factory.StartNew(() =>
        //        //{
        //        //    Application.Current.Dispatcher.Invoke((Action)delegate
        //        //    {
        //        //        var view = new CustomMessageBox
        //        //        {
        //        //            DataContext = this,
        //        //            Status = eCustomStatus.Information,
        //        //            Header = "Go to Trigger",
        //        //            Content = "Trigger not Found"
        //        //        };

        //        //        //show the dialog
        //        //        var result = DialogHost.Show(view, "RootDialog");
        //        //    });
        //        //});
        //        MessageBox.Show("Trigger Not Found", "Go To Trigger", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //}

        #endregion
    }
}

