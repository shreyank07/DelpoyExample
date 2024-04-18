using CommunityToolkit.Mvvm.DependencyInjection;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Structure;
using Prodigy.Framework.Collections;
using ProdigyFramework.ComponentModel;
using Prodigy.Framework.Helpers;
using Prodigy.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PGYMiniCooper.DataModule.Model
{
    public class WfmListModel : DataWrapper
    {
        public WfmListModel()
        {
            IsLoading = true;
        }
        private bool isLoading = true;
        public bool IsLoading
        {
            get { return this.isLoading; }
            protected set
            {
                this.isLoading = value;
                RaisePropertyChanged("IsLoading");
            }
        }

        private double timestamp;
        public double Timestamp
        {
            get { return timestamp; }
            set
            {
                timestamp = value;
                RaisePropertyChanged("Timestamp");
            }
        }

        private UInt16 channelValues;

        public UInt16 ChannelValues
        {
            get { return channelValues; }
            set { channelValues = value; RaisePropertyChanged("ChannelValues"); }
        }

        public void SetMessage(WfmListModel item)
        {
            this.Timestamp = item.Timestamp;
            this.ChannelValues = item.ChannelValues;
            this.IsLoading = false;
            this.Index = item.Index;
        }
    }
    public class WaveformListHolder : ViewModelBase
    {
        private readonly ConfigModel config;
        IDataProvider dataReceiver;
        private Timer periodicTimer;
        public WaveformListHolder(ConfigModel configModel)
        {
            this.config = configModel;
            dataReceiver = Ioc.Default.GetService<IDataProvider>();           
        }

        private void FetchRange(IList<WfmListModel> arg1, int startIndex, int count)
        {
            IsLoading = true;

            try
            {
                var list = GetPackets(startIndex, count);

                for (int i = 0; i < arg1.Count; i++)
                {
                    if (i >= list.Count) break;

                    arg1[i].SetMessage((list[i]));
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private List<WfmListModel> GetPackets(int startIndex, int count)
        {
            List<WfmListModel> list = new List<WfmListModel>();
            TriggerTime = SessionConfiguration.TriggerTime;
            int no = startIndex;
            List<DiscreteWaveForm> buffer = dataReceiver.GetWaveform(startIndex, startIndex + count);
            for (int index = 0; index < buffer.Count(); index++)
            {
                WfmListModel pkt = new WfmListModel();
                pkt.Index = no++;
                pkt.Timestamp = buffer[index].TimeStamp;
                pkt.ChannelValues = buffer[index].ChannelValue;
                list.Add(pkt);
            }
            if (SessionConfiguration.TriggersetTView)
            {
                TriggerPacket = SessionConfiguration.TriggerIndex - 1;
            }
            return list;
        }

        private bool isLoading = false;
        public bool IsLoading
        {
            get { return this.isLoading; }
            protected set
            {
                this.isLoading = value;
                this.RaisePropertyChanged("IsLoading");
            }
        }

        private AsyncVirtualizingCollection<WfmListModel> aSyncCollection;
        public AsyncVirtualizingCollection<WfmListModel> ASyncCollection
        {
            get { return aSyncCollection; }
            set
            {
                aSyncCollection = value;

                RaisePropertyChanged("ASyncCollection");
            }
        }
        long wfmCounter = 0;
        long prevwfmCounter = 0;
        public void UpdateFrameCount(object state)
        {
            if (latestTraceFile == null)
                return;

            wfmCounter = latestTraceFile.New.MaxEdges;
            if (wfmCounter <= int.MaxValue && prevwfmCounter != wfmCounter)
            {
                triggerTime = SessionConfiguration.TriggerTime;
                ASyncCollection.UpdateCount((int)wfmCounter);
            }
            prevwfmCounter = wfmCounter;
        }

        public void FireCollectionChangedEvent()
        {
            //ASyncCollection.FireCollectionReset();
        }

        public void Initialize()
        {
            ASyncCollection = new AsyncVirtualizingCollection<WfmListModel>
            (new ItemsProvider<WfmListModel>(new Action<IList<WfmListModel>, int, int>(FetchRange))
            , 2000, 5000);
            ASyncCollection.UpdateCount(0);
            TriggerPacket = -1;

            // TODO: Waveform points are not stored -> in future we may need to merge edge data to get the requried information here.
            //dataReceiver.WaveformAvailable += DataReceiver_OnNewDataReceived;

            if (periodicTimer != null)
                periodicTimer.Dispose();

            periodicTimer = new Timer(UpdateFrameCount, null, 0, 5000);
        }

        WaveformAvailableEventArgs latestTraceFile = null;
        private void DataReceiver_OnNewDataReceived(object sender, WaveformAvailableEventArgs e)
        {
            latestTraceFile = e;
        }

        public void Reset()
        {
            if (periodicTimer != null)
            {
                periodicTimer.Dispose();
                periodicTimer = null;
            }

            //dataReceiver.WaveformAvailable -= DataReceiver_OnNewDataReceived;
            latestTraceFile = null;

            var i2CConfig = config.ProtocolConfigList.OfType<ConfigModel_I2C>().FirstOrDefault();
            var spiConfig = config.ProtocolConfigList.OfType<ConfigModel_SPI>().FirstOrDefault();
            var UARTConfig = config.ProtocolConfigList.OfType<ConfigModel_UART>().FirstOrDefault();
            var i3cConfig = config.ProtocolConfigList.OfType<ConfigModel_I3C>().FirstOrDefault();
            var spmiConfig = config.ProtocolConfigList.OfType<ConfigModel_SPMI>().FirstOrDefault();
            var rffeconfig=config.ProtocolConfigList.OfType<ConfigModel_RFFE>().FirstOrDefault();
            var qspiconfig = config.ProtocolConfigList.OfType<ConfigModel_QSPI>().FirstOrDefault();
            var  canconfig=config.ProtocolConfigList.OfType<ConfigModel_CAN>().FirstOrDefault();
            IsLoading = false;
            TriggerPacket = -1;
            ASyncCollection?.UpdateCount(0);
            wfmCounter = 0;
            prevwfmCounter = 0;
            #region Waveform Visibility
            config.IsCh1Visible = false;
            config.IsCh2Visible = false;
            config.IsCh3Visible = false;
            config.IsCh4Visible = false;
            config.IsCh5Visible = false;
            config.IsCh6Visible = false;
            config.IsCh7Visible = false;
            config.IsCh8Visible = false;
            config.IsCh9Visible = false;
            config.IsCh10Visible = false;
            config.IsCh11Visible = false;
            config.IsCh12Visible = false;
            config.IsCh13Visible = false;
            config.IsCh14Visible = false;
            config.IsCh15Visible = false;
            config.IsCh16Visible = false;

            config.IsCLK1GRP1Visible = false;
            config.IsCLK1GRP2Visible = false;
            config.IsCLK1GRP3Visible = false;
            config.IsCLK2GRP1Visible = false;
            config.IsCLK2GRP2Visible = false;
            config.IsCLK2GRP3Visible = false;


            var converter = new System.Windows.Media.BrushConverter();
            config.Ch1Fgd = "#cfd8dc";
            config.Ch2Fgd = "#cfd8dc";
            config.Ch3Fgd = "#cfd8dc";
            config.Ch4Fgd = "#cfd8dc";
            config.Ch5Fgd = "#cfd8dc";
            config.Ch6Fgd = "#cfd8dc";
            config.Ch7Fgd = "#cfd8dc";
            config.Ch8Fgd = "#cfd8dc";
            config.Ch9Fgd = "#cfd8dc";
            config.Ch10Fgd = "#cfd8dc";
            config.Ch11Fgd = "#cfd8dc";
            config.Ch12Fgd = "#cfd8dc";
            config.Ch13Fgd = "#cfd8dc";
            config.Ch14Fgd = "#cfd8dc";
            config.Ch15Fgd = "#cfd8dc";
            config.Ch16Fgd = "#cfd8dc";

            if (config.ConfigurationMode == eConfigMode.PA_Mode || config.ConfigurationMode == eConfigMode.Both)
            {
                if (i2CConfig != null)
                {
                    GetChannelVisibility(i2CConfig.ChannelIndex_SCL);
                    GetChannelVisibility(i2CConfig.ChannelIndex_SDA);
                }
                if (spiConfig != null)
                {
                    GetChannelVisibility(spiConfig.ChannelIndex_CLK);
                    GetChannelVisibility(spiConfig.ChannelIndex_CS);
                    GetChannelVisibility(spiConfig.ChannelIndex_MOSI);
                    GetChannelVisibility(spiConfig.ChannelIndex_MISO);
                }
                if (UARTConfig!=null)
                {
                    GetChannelVisibility(UARTConfig.ChannelIndex_RX);
                    GetChannelVisibility(UARTConfig.ChannelIndex_TX);
                }
                if (i3cConfig!=null)
                {
                    GetChannelVisibility(i3cConfig.ChannelIndex_SCL);
                    GetChannelVisibility(i3cConfig.ChannelIndex_SDA);
                }
                if (spmiConfig!=null)
                {
                    GetChannelVisibility(spmiConfig.ChannelIndex_SCL);
                    GetChannelVisibility(spmiConfig.ChannelIndex_SDA);
                }
                if (rffeconfig!=null)
                {
                    GetChannelVisibility(rffeconfig.ChannelIndex_SCL);
                    GetChannelVisibility(rffeconfig.ChannelIndex_SDA);
                }
                if (canconfig!=null)
                {
                    GetChannelVisibility(canconfig.ChannelIndex);
                }


                //QSPI
                if (qspiconfig!=null)
                {
                    GetChannelVisibility(qspiconfig.ChannelIndex_CLK);
                    GetChannelVisibility(qspiconfig.ChannelIndex_CS);
                    GetChannelVisibility(qspiconfig.ChannelIndex_D0);
                    GetChannelVisibility(qspiconfig.ChannelIndex_D1);
                    GetChannelVisibility(qspiconfig.ChannelIndex_D2);
                    GetChannelVisibility(qspiconfig.ChannelIndex_D3);
                }
            }
            else if (config.ConfigurationMode == eConfigMode.LA_Mode)
            {
                int i;
                int count;
                if (config.GeneralPurposeMode == eGeneralPurpose.State)
                {
                    GetChannelVisibility(config.SelectedClock1);
                }
                if (config.GroupType == eGroupType.Group)
                {
                    if (config.SelectedCLK1_GRP1 != null)
                    {
                        count = config.SelectedCLK1_GRP1.Count;
                        if (count > 0)
                            config.IsCLK1GRP1Visible = true;
                        for (i = 0; i < count; i++)
                        {
                            GetChannelVisibility(config.SelectedCLK1_GRP1[i]);
                            GetChannelColor(config.SelectedCLK1_GRP1[i], config.SelColorClk1Grp1);
                        }
                    }
                    if (config.SelectedCLK1_GRP2 != null)
                    {
                        count = config.SelectedCLK1_GRP2.Count;
                        if (count > 0)
                            config.IsCLK1GRP2Visible = true;
                        for (i = 0; i < count; i++)
                        {
                            GetChannelVisibility(config.SelectedCLK1_GRP2[i]);
                            GetChannelColor(config.SelectedCLK1_GRP2[i], config.SelColorClk1Grp2);
                        }
                    }
                    if (config.SelectedCLK1_GRP3 != null)
                    {
                        count = config.SelectedCLK1_GRP3.Count;
                        if (count > 0)
                            config.IsCLK1GRP3Visible = true;
                        for (i = 0; i < count; i++)
                        {
                            GetChannelVisibility(config.SelectedCLK1_GRP3[i]);
                            GetChannelColor(config.SelectedCLK1_GRP3[i], config.SelColorClk1Grp3);
                        }
                    }
                    if (config.HasTwoClockSources || config.GeneralPurposeMode == eGeneralPurpose.Timing)
                    {
                        if (config.GeneralPurposeMode == eGeneralPurpose.State)
                        {
                            GetChannelVisibility(config.SelectedClock2);
                        }
                        if (config.SelectedCLK2_GRP1 != null)
                        {
                            count = config.SelectedCLK2_GRP1.Count;
                            if (count > 0)
                                config.IsCLK2GRP1Visible = true;
                            for (i = 0; i < count; i++)
                            {
                                GetChannelVisibility(config.SelectedCLK2_GRP1[i]);
                                GetChannelColor(config.SelectedCLK2_GRP1[i], config.SelColorClk2Grp1);
                            }
                        }
                        if (config.SelectedCLK2_GRP2 != null)
                        {
                            count = config.SelectedCLK2_GRP2.Count;
                            if (count > 0)
                                config.IsCLK2GRP2Visible = true;
                            for (i = 0; i < count; i++)
                            {
                                GetChannelVisibility(config.SelectedCLK2_GRP2[i]);
                                GetChannelColor(config.SelectedCLK2_GRP2[i], config.SelColorClk2Grp2);
                            }
                        }
                        if (config.SelectedCLK2_GRP3 != null)
                        {
                            count = config.SelectedCLK2_GRP3.Count;
                            if (count > 0)
                                config.IsCLK2GRP3Visible = true;
                            for (i = 0; i < count; i++)
                            {
                                GetChannelVisibility(config.SelectedCLK2_GRP3[i]);
                                GetChannelColor(config.SelectedCLK2_GRP3[i], config.SelColorClk2Grp3);
                            }
                        }
                    }
                }
                else if (config.GroupType == eGroupType.Individual)
                {
                    if (config.SelectedIndividualChannels != null)
                    {
                        count = config.SelectedIndividualChannels.Count;
                        for (i = 0; i < count; i++)
                        {
                            GetChannelVisibility(config.SelectedIndividualChannels[i]);
                        }
                    }
                    if (config.HasTwoClockSources)
                    {
                        GetChannelVisibility(config.SelectedClock2);
                    }
                }
                else if (config.ConfigurationMode == eConfigMode.Both)
                {
                    if (i2CConfig != null)
                    {
                        GetChannelVisibility(i2CConfig.ChannelIndex_SCL);
                        GetChannelVisibility(i2CConfig.ChannelIndex_SDA);
                    }
                    if (spiConfig != null)
                    {
                        GetChannelVisibility(spiConfig.ChannelIndex_CLK);
                        GetChannelVisibility(spiConfig.ChannelIndex_CS);
                        GetChannelVisibility(spiConfig.ChannelIndex_MOSI);
                        GetChannelVisibility(spiConfig.ChannelIndex_MISO);
                    }
                    if (UARTConfig!=null)
                    {
                        GetChannelVisibility(UARTConfig.ChannelIndex_RX);
                        GetChannelVisibility(UARTConfig.ChannelIndex_TX);
                    }

                    if (i3cConfig != null)
                    {
                        GetChannelVisibility(i3cConfig.ChannelIndex_SCL);
                        GetChannelVisibility(i3cConfig.ChannelIndex_SDA);
                    }
                    if (spmiConfig!=null)
                    {
                        GetChannelVisibility(spmiConfig.ChannelIndex_SCL);
                        GetChannelVisibility(spmiConfig.ChannelIndex_SDA);
                    }
                    if (rffeconfig!=null)
                    {
                        GetChannelVisibility(rffeconfig.ChannelIndex_SCL);
                        GetChannelVisibility(rffeconfig.ChannelIndex_SDA);
                    }
                    if(canconfig!=null)
                    {
                        GetChannelVisibility(canconfig.ChannelIndex);
                    }



                    //QSPI

                    if (qspiconfig!=null)
                    {
                        GetChannelVisibility(qspiconfig.ChannelIndex_CLK);
                        GetChannelVisibility(qspiconfig.ChannelIndex_CS);
                        GetChannelVisibility(qspiconfig.ChannelIndex_D0);
                        GetChannelVisibility(qspiconfig.ChannelIndex_D1);
                        GetChannelVisibility(qspiconfig.ChannelIndex_D2);
                        GetChannelVisibility(qspiconfig.ChannelIndex_D3);
                    }
                }
                #endregion
            }
        }

        public double TriggerTime
        {
            get
            {
                return triggerTime;
            }
            set
            {
                triggerTime = value;
                RaisePropertyChanged("TriggerTime");
            }
        }
        double triggerTime = -1;

        private int _TriggerPacket;
        public int TriggerPacket
        {
            get
            {
                return _TriggerPacket;
            }
            set
            {
                _TriggerPacket = value;
                RaisePropertyChanged("TriggerPacket");
            }
        }

        private void GetChannelVisibility(eChannles channnel)
        {
            switch (channnel)
            {
                case eChannles.CH1:
                    config.IsCh1Visible = true;
                    break;
                case eChannles.CH2:
                    config.IsCh2Visible = true;
                    break;
                case eChannles.CH3:
                    config.IsCh3Visible = true;
                    break;
                case eChannles.CH4:
                    config.IsCh4Visible = true;
                    break;
                case eChannles.CH5:
                    config.IsCh5Visible = true;
                    break;
                case eChannles.CH6:
                    config.IsCh6Visible = true;
                    break;
                case eChannles.CH7:
                    config.IsCh7Visible = true;
                    break;
                case eChannles.CH8:
                    config.IsCh8Visible = true;
                    break;
                case eChannles.CH9:
                    config.IsCh9Visible = true;
                    break;
                case eChannles.CH10:
                    config.IsCh10Visible = true;
                    break;
                case eChannles.CH11:
                    config.IsCh11Visible = true;
                    break;
                case eChannles.CH12:
                    config.IsCh12Visible = true;
                    break;
                case eChannles.CH13:
                    config.IsCh13Visible = true;
                    break;
                case eChannles.CH14:
                    config.IsCh14Visible = true;
                    break;
                case eChannles.CH15:
                    config.IsCh15Visible = true;
                    break;
                case eChannles.CH16:
                    config.IsCh16Visible = true;
                    break;
                default:
                    break;
            }
        }

        private void GetChannelColor(eChannles channnel, Color SelColor)
        {
            switch (channnel)
            {
                case eChannles.CH1:
                    config.Ch1Fgd = new SolidColorBrush(SelColor).ToString();
                    break;
                case eChannles.CH2:
                    config.Ch2Fgd = new SolidColorBrush(SelColor).ToString();
                    break;
                case eChannles.CH3:
                    config.Ch3Fgd = new SolidColorBrush(SelColor).ToString();
                    break;
                case eChannles.CH4:
                    config.Ch4Fgd = new SolidColorBrush(SelColor).ToString();
                    break;
                case eChannles.CH5:
                    config.Ch5Fgd = new SolidColorBrush(SelColor).ToString();
                    break;
                case eChannles.CH6:
                    config.Ch6Fgd = new SolidColorBrush(SelColor).ToString();
                    break;
                case eChannles.CH7:
                    config.Ch7Fgd = new SolidColorBrush(SelColor).ToString();
                    break;
                case eChannles.CH8:
                    config.Ch8Fgd = new SolidColorBrush(SelColor).ToString();
                    break;
                case eChannles.CH9:
                    config.Ch9Fgd = new SolidColorBrush(SelColor).ToString();
                    break;
                case eChannles.CH10:
                    config.Ch10Fgd = new SolidColorBrush(SelColor).ToString();
                    break;
                case eChannles.CH11:
                    config.Ch11Fgd = new SolidColorBrush(SelColor).ToString();
                    break;
                case eChannles.CH12:
                    config.Ch12Fgd = new SolidColorBrush(SelColor).ToString();
                    break;
                case eChannles.CH13:
                    config.Ch13Fgd = new SolidColorBrush(SelColor).ToString();
                    break;
                case eChannles.CH14:
                    config.Ch14Fgd = new SolidColorBrush(SelColor).ToString();
                    break;
                case eChannles.CH15:
                    config.Ch15Fgd = new SolidColorBrush(SelColor).ToString();
                    break;
                case eChannles.CH16:
                    config.Ch16Fgd = new SolidColorBrush(SelColor).ToString();
                    break;
                default:
                    break;
            }
        }
    }
}
