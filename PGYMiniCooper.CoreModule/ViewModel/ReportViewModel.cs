using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Drawing.BarCodes;
using PdfSharp.Pdf;

using PGYMiniCooper.CoreModule.View;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using PGYMiniCooper.CoreModule.ViewModel.ProtocolViewModel;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Interface;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.CANStructure;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using PGYMiniCooper.DataModule.Structure.I3CStructure;
using PGYMiniCooper.DataModule.Structure.QSPIStructure;
using PGYMiniCooper.DataModule.Structure.RFFEStructure;
using PGYMiniCooper.DataModule.Structure.SPIStructure;
using PGYMiniCooper.DataModule.Structure.SPMIStructure;
using PGYMiniCooper.DataModule.Structure.UARTStructure;

using ProdigyFramework.Behavior;
using ProdigyFramework.Collections;
using ProdigyFramework.ComponentModel;
using ProdigyFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using RelayCommand = ProdigyFramework.Behavior.RelayCommand;

namespace PGYMiniCooper.CoreModule.ViewModel
{
    public class ReportViewModel : ViewModelBase
    {
        private AnalyzerViewModel analyzerViewModel;
        private readonly ConfigurationViewModel configuration;
        private readonly ProtocolActivityViewModel protocolActivity;
        private readonly WaveformListingViewModel waveformListing;
        private readonly ResultsViewModel results;
        //public event Action<string> ReportStatusUpdated;
        public delegate void OnReportStatus(string status);
        public static event OnReportStatus ReportStatusUpdated;
        public ReportViewModel(ConfigurationViewModel configuration, ProtocolActivityViewModel protocolActivity, WaveformListingViewModel waveformListing, ResultsViewModel results)
        {
            //this.analyzerViewModel = analyzerViewModel;
            this.configuration = configuration;
            this.protocolActivity = protocolActivity;
            this.waveformListing = waveformListing;
            this.results = results;
        }

        bool CanexecuteReport(object o)
        {
            if (results.ProtocolResults.Count() != 0)
                return true;

            return false;
        }

        public bool CanExecute
        {
            get
            {
                //if (ResultHolder.GetInstance().FrameCollection.Count != 0 || SPIResultHolder.GetInstance().SPICollection.Count != 0)
                //    return true;

                //return false;
                return true;
            }
        }

        private async void ExecuteRunDialog(object o)
        {
            ImageList = new ObservableCollection<ImageStructure>();
            string capture = Path.GetFileNameWithoutExtension(SessionConfiguration.TraceFilePath);
            string folder = MiniCooperDirectoryInfo.ImagePath + @"\" + capture;
            if (Directory.Exists(folder))
            {
                foreach (string F in System.IO.Directory.GetFiles(folder, "*.png"))
                {
                    ImageList.Add(new ImageStructure()
                    {
                        ImagePath = F
                    });
                }
            }
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new ReportView
            {
                DataContext = this
            };

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);

            if (result != null && result.ToString().ToLower() == "true")
            {
                // SelRange = true;
                GeneratePDF();
            }
            //check the result...
            Console.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL"));
        }

        string pathCsv = "";
        private void GenerateReportCsvDialog()
        {
            
           Task.Run(() =>
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV file (*.csv)|*.csv";
                pathCsv = "";
                if (saveFileDialog.ShowDialog() == true && saveFileDialog.FileName != null)
                {
                    pathCsv = saveFileDialog.FileName;
                    if (!Directory.Exists(new FileInfo(pathCsv).Directory.FullName))
                    {
                        MessageBox.Show("Directory not found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        return;
                    }
                    else if (IsFileLocked(new FileInfo(pathCsv)))
                    {
                        MessageBox.Show("File is in use", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        return;
                    }
                
                ThreadFunctionCsv();
                    //Thread thread = new Thread(new ThreadStart(ThreadFunctionCsv));
                    //thread.IsBackground = true;
                    //thread.Start();
                }
            });
           
        }

        private void ThreadFunctionCsv()
        {
            // string prevStatus = analyzerViewModel.Status;
            ReportStatusUpdated?.Invoke("Generating Report");
            Thread.Sleep(10000);

            GenerateCSV(pathCsv);
            ReportStatusUpdated?.Invoke("Report Generated");
            Thread.Sleep(3000);
            // analyzerViewModel.Status = prevStatus;
        }

        public void GenerateCSV(string path)
        {

            StringBuilder str = new StringBuilder();
            ConfigModel config = configuration.Config;
            var i2cResult = results.ProtocolResults.OfType<ResultViewModel_I2C>().FirstOrDefault();
            var spiResult = results.ProtocolResults.OfType<ResultViewModel_SPI>().FirstOrDefault();
            var uartResult = results.ProtocolResults.OfType<ResultViewModel_UART>().FirstOrDefault();
            var spmiResult = results.ProtocolResults.OfType<ResultViewModel_SPMI>().FirstOrDefault();
            var rffeResult = results.ProtocolResults.OfType<ResultViewModel_RFFE>().FirstOrDefault();
            var qspiResult = results.ProtocolResults.OfType<ResultViewModel_QSPI>().FirstOrDefault();
            var canResult = results.ProtocolResults.OfType<ResultViewModel_CAN>().FirstOrDefault();

            #region Volt
            String[] strVolt = new String[8];
            strVolt[0] = "Voltage - Ch1:Ch2";
            strVolt[1] = "Voltage - Ch3:Ch4";
            strVolt[2] = "Voltage - Ch5:Ch6";
            strVolt[3] = "Voltage - Ch7:Ch8";
            strVolt[4] = "Voltage - Ch9:Ch10";
            strVolt[5] = "Voltage - Ch11:Ch12";
            strVolt[6] = "Voltage - Ch13:Ch14";
            strVolt[7] = "Voltage - Ch15:Ch16";
            str.AppendLine(String.Join(",", strVolt));
            String[] strVolt1 = new String[8];
            strVolt1[0] = GetVolt(config.SignalAmpCh1_2);
            strVolt1[1] = GetVolt(config.SignalAmpCh3_4);
            strVolt1[2] = GetVolt(config.SignalAmpCh5_6);
            strVolt1[3] = GetVolt(config.SignalAmpCh7_8);
            strVolt1[4] = GetVolt(config.SignalAmpCh9_10);
            strVolt1[5] = GetVolt(config.SignalAmpCh11_12);
            strVolt1[6] = GetVolt(config.SignalAmpCh13_14);
            strVolt1[7] = GetVolt(config.SignalAmpCh15_16);
            str.AppendLine(String.Join(",", strVolt1));
            str.AppendLine();
            str.AppendLine();
            #endregion
            #region Channels
            if (config.ConfigurationMode == DataModule.eConfigMode.PA_Mode || config.ConfigurationMode == DataModule.eConfigMode.Both)
            {
                int i = 1;



                foreach (var configViewModel in configuration.ProtocolConfiguration)
                {

                    if (configViewModel is ConfigViewModel_I3C i3CConfig)
                    {
                        if (i3CConfig != null)
                        {
                            String[] strChannel = new String[2];
                            strChannel[0] = "I3C CLK_" + i;
                            strChannel[1] = "I3C DATA_" + i;
                            str.AppendLine(String.Join(",", strChannel));
                            String[] strChannel1 = new String[2];
                            strChannel1[0] = i3CConfig.ChannelIndex_SCL.ToString();
                            strChannel1[1] = i3CConfig.ChannelIndex_SDA.ToString();
                            str.AppendLine(String.Join(",", strChannel1));
                            str.AppendLine();
                            str.AppendLine();
                            i++;
                        }

                    }

                    if (configViewModel is ConfigViewModel_I2C i2CConfig)
                    {
                        if (i2CConfig != null)
                        {
                            String[] strChannel = new String[2];
                            strChannel[0] = "I2C CLK_" + i;
                            strChannel[1] = "I2C DATA_" + i;
                            str.AppendLine(String.Join(",", strChannel));
                            String[] strChannel1 = new String[2];
                            strChannel1[0] = i2CConfig.ChannelIndex_SCL.ToString();
                            strChannel1[1] = i2CConfig.ChannelIndex_SDA.ToString();
                            str.AppendLine(String.Join(",", strChannel1));
                            str.AppendLine();
                            str.AppendLine();
                            i++;
                        }

                    }

                    if (configViewModel is ConfigViewModel_SPI spiConfig)
                    {
                        if (spiConfig != null)
                        {
                            String[] strChannel = new String[4];
                            strChannel[0] = "SPI CLK_" + i;
                            strChannel[1] = "SPI CS_" + i;
                            strChannel[2] = "SPI MOSI_" + i;
                            strChannel[3] = "SPI MISO_" + i;
                            str.AppendLine(String.Join(",", strChannel));
                            String[] strChannel1 = new String[4];
                            strChannel1[0] = spiConfig.ChannelIndex_CLK.ToString();
                            strChannel1[1] = spiConfig.ChannelIndex_CS.ToString();
                            strChannel1[2] = spiConfig.ChannelIndex_MOSI.ToString();
                            strChannel1[3] = spiConfig.ChannelIndex_MISO.ToString();
                            str.AppendLine(String.Join(",", strChannel1));
                            str.AppendLine();
                            str.AppendLine();
                            i++;
                        }

                    }
                    if (configViewModel is ConfigViewModel_UART uartConfig)
                    {
                        if (uartConfig != null)
                        {
                            String[] strChannel = new String[2];
                            strChannel[0] = "UART TX_" + i;
                            strChannel[1] = "UART RX_" + i;
                            str.AppendLine(String.Join(",", strChannel));
                            String[] strChannel1 = new String[2];
                            strChannel1[0] = uartConfig.ChannelIndex_TX.ToString();
                            strChannel1[1] = uartConfig.ChannelIndex_RX.ToString();
                            str.AppendLine(String.Join(",", strChannel1));
                            str.AppendLine();
                            str.AppendLine();
                            i++;
                        }

                    }


                    if (configViewModel is ConfigViewModel_QSPI qspiConfig)
                    {
                        if (qspiConfig != null)
                        {
                            String[] strChannel = new String[6];
                            strChannel[0] = "QSPI CLK_" + i;
                            strChannel[1] = "QSPI CS_" + i;
                            strChannel[2] = "QSPI D0_" + i;
                            strChannel[3] = "QSPI D1_" + i;
                            strChannel[4] = "QSPI D2_" + i;
                            strChannel[5] = "QSPI D3_" + i;
                            str.AppendLine(String.Join(",", strChannel));
                            String[] strChannel1 = new String[6];
                            strChannel1[0] = qspiConfig.ChannelIndex_CLK.ToString();
                            strChannel1[1] = qspiConfig.ChannelIndex_CS.ToString();
                            strChannel1[2] = qspiConfig.ChannelIndex_D0.ToString();
                            strChannel1[3] = qspiConfig.ChannelIndex_D1.ToString();
                            strChannel1[4] = qspiConfig.ChannelIndex_D2.ToString();
                            strChannel1[5] = qspiConfig.ChannelIndex_D3.ToString();
                            str.AppendLine(String.Join(",", strChannel1));
                            str.AppendLine();
                            str.AppendLine();
                            i++;
                        }

                    }



                    if (configViewModel is ConfigViewModel_CAN canConfig)
                    {
                        if (canConfig != null)
                        {
                            String[] strChannel = new String[1];
                            strChannel[0] = "CAN CH_" + i;
                           
                            str.AppendLine(String.Join(",", strChannel));
                            String[] strChannel1 = new String[1];
                            strChannel1[0] = canConfig.ChannelIndex.ToString();
                         
                            str.AppendLine(String.Join(",", strChannel1));
                            str.AppendLine();
                            str.AppendLine();
                            i++;
                        }

                    }


                    if(configViewModel is ConfigViewModel_SPMI spmiConfig)
                    {
                        if(spmiConfig!=null)
                        {
                            String[] strChannel = new String[2];
                            strChannel[0] = "SPMI CLK_" + i;
                            strChannel[1] = "SPMI DATA_" + i;
                            str.AppendLine(String.Join(",", strChannel));
                            String[] strChannel1 = new String[2];
                            strChannel1[0] = spmiConfig.ChannelIndex_SCL.ToString();
                            strChannel1[1] = spmiConfig.ChannelIndex_SDA.ToString();
                            str.AppendLine(String.Join(",", strChannel1));
                            str.AppendLine();
                            str.AppendLine();
                            i++;
                        }
                    }


                    if (configViewModel is ConfigViewModel_RFFE rffeConfig)
                    {
                        if (rffeConfig != null)
                        {
                            String[] strChannel = new String[2];
                            strChannel[0] = "RFFE CLK_" + i;
                            strChannel[1] = "RFFE DATA_" + i;
                            str.AppendLine(String.Join(",", strChannel));
                            String[] strChannel1 = new String[2];
                            strChannel1[0] = rffeConfig.ChannelIndex_SCL.ToString();
                            strChannel1[1] = rffeConfig.ChannelIndex_SDA.ToString();
                            str.AppendLine(String.Join(",", strChannel1));
                            str.AppendLine();
                            str.AppendLine();
                            i++;
                        }
                    }
                }
            


        

                
                if (config.ConfigurationMode == DataModule.eConfigMode.LA_Mode || config.ConfigurationMode == DataModule.eConfigMode.Both)
                {
                    #region Sample Rate
                    string strSR = "Sample Rate";
                    str.AppendLine(strSR);
                    string sampleRate = "-";
                    if (config.ConfigurationMode == DataModule.eConfigMode.LA_Mode)
                    {
                        sampleRate = GetSampleRate(config.SampleRateLAPA);
                    }
                    else if (config.ConfigurationMode == DataModule.eConfigMode.Both)
                        sampleRate = GetSampleRate(config.SampleRateLAPA);
                    str.AppendLine(sampleRate);
                    str.AppendLine();
                    str.AppendLine();
                    #endregion

                    if (config.ConfigurationMode == DataModule.eConfigMode.LA_Mode)
                    {
                        #region Clk1
                        if (config.GeneralPurposeMode == DataModule.eGeneralPurpose.State)
                        {
                            string strClk1 = "Clk1 Channel";
                            str.AppendLine(strClk1);
                            string Clk1Channel = config.SelectedClock1.ToString();
                            str.AppendLine(Clk1Channel);
                            str.AppendLine();
                            str.AppendLine();
                        }
                        #endregion
                        #region Clk1 Groups
                        String[] strClk1Grp = new String[4];
                        strClk1Grp[0] = "Clk1 Grp1 Channels";
                        strClk1Grp[1] = "Clk1 Grp2 Channels";
                        strClk1Grp[2] = "Clk1 Grp3 Channels";
                        str.AppendLine(String.Join(",", strClk1Grp));

                        String[] strClk1Grp_ = new String[4];
                        if (config.SelectedCLK1_GRP1 != null)
                        {
                            if (config.SelectedCLK1_GRP1.Count != 0)
                            {
                                foreach (var chn in config.SelectedCLK1_GRP1)
                                {
                                    strClk1Grp_[0] += chn + " : ";
                                }
                                strClk1Grp_[0] = strClk1Grp_[0].Substring(0, strClk1Grp_[0].Length - 2);
                            }
                            else
                                strClk1Grp_[0] = "-";
                        }
                        else
                            strClk1Grp_[0] = "-";

                        if (config.SelectedCLK1_GRP2 != null)
                        {
                            if (config.SelectedCLK1_GRP2.Count != 0)
                            {
                                foreach (var chn in config.SelectedCLK1_GRP2)
                                {
                                    strClk1Grp_[1] += chn + " : ";
                                }
                                strClk1Grp_[1] = strClk1Grp_[1].Substring(0, strClk1Grp_[1].Length - 2);
                            }
                            else
                                strClk1Grp_[1] = "-";
                        }
                        else
                            strClk1Grp_[1] = "-";

                        if (config.SelectedCLK1_GRP3 != null)
                        {
                            if (config.SelectedCLK1_GRP3.Count != 0)
                            {
                                foreach (var chn in config.SelectedCLK1_GRP3)
                                {
                                    strClk1Grp_[2] += chn + " : ";
                                }
                                strClk1Grp_[2] = strClk1Grp_[2].Substring(0, strClk1Grp_[2].Length - 2);
                            }
                            else
                                strClk1Grp_[2] = "-";
                        }
                        else
                            strClk1Grp_[2] = "-";

                        str.AppendLine(String.Join(",", strClk1Grp_));
                        str.AppendLine();
                        str.AppendLine();
                        #endregion

                        if (config.HasTwoClockSources)
                        {
                            #region Clk2
                            string strClk2 = "Clk2 Channel";
                            str.AppendLine(strClk2);
                            string Clk2Channel = config.SelectedClock2.ToString();
                            str.AppendLine(Clk2Channel);
                            str.AppendLine();
                            str.AppendLine();
                            #endregion
                            #region Clk2 Groups
                            String[] strClk2Grp = new String[4];
                            strClk2Grp[0] = "Clk2 Grp1 Channels";
                            strClk2Grp[1] = "Clk2 Grp2 Channels";
                            strClk2Grp[2] = "Clk2 Grp3 Channels";
                            str.AppendLine(String.Join(",", strClk2Grp));

                            String[] strClk2Grp_ = new String[4];
                            if (config.SelectedCLK2_GRP1 != null)
                            {
                                if (config.SelectedCLK2_GRP1.Count != 0)
                                {
                                    foreach (var chn in config.SelectedCLK2_GRP1)
                                    {
                                        strClk2Grp_[0] += chn + " : ";
                                    }
                                    strClk2Grp_[0] = strClk2Grp_[0].Substring(0, strClk2Grp_[0].Length - 2);
                                }
                                else
                                    strClk2Grp_[0] = "-";
                            }

                            if (config.SelectedCLK2_GRP2 != null)
                            {
                                if (config.SelectedCLK2_GRP2.Count != 0)
                                {
                                    foreach (var chn in config.SelectedCLK2_GRP2)
                                    {
                                        strClk2Grp_[1] += chn + " : ";
                                    }
                                    strClk2Grp_[1] = strClk2Grp_[1].Substring(0, strClk2Grp_[1].Length - 2);
                                }
                                else
                                    strClk2Grp_[1] = "-";
                            }
                            else
                                strClk2Grp_[1] = "-";

                            if (config.SelectedCLK2_GRP3 != null)
                            {
                                if (config.SelectedCLK2_GRP3.Count != 0)
                                {
                                    foreach (var chn in config.SelectedCLK2_GRP3)
                                    {
                                        strClk2Grp_[2] += chn + " : ";
                                    }
                                    strClk2Grp_[2] = strClk2Grp_[2].Substring(0, strClk2Grp_[2].Length - 2);
                                }
                                else
                                    strClk2Grp_[2] = "-";
                            }
                            else
                                strClk2Grp_[2] = "-";

                            str.AppendLine(String.Join(",", strClk2Grp_));
                            str.AppendLine();
                            str.AppendLine();
                            #endregion
                        }
                    }
                }
                #endregion
                if (config.ConfigurationMode == DataModule.eConfigMode.PA_Mode || config.ConfigurationMode == DataModule.eConfigMode.Both)
                {
                    try
                    {
                        IList<ProtocolActivityHolder> collection = protocolActivity.ProtocolActivity.ProtocolCollection;
                        AddProtocolToCSV(ref str);
                        str.AppendLine();
                        str.AppendLine();
                    }
                    catch (Exception ex)
                    {

                    }


                    //if (i2cResult != null)
                    //{
                    //    StartIndexReportI2C = 0;
                    //    if (i2cResult.FrameCollection != null)
                    //        EndIndexReportI2C = i2cResult.FrameCollection.Count - 1;
                    //    else
                    //        EndIndexReportI2C = 0;
                    //    AddToI2C_CSV(ref str);
                    //    str.AppendLine();
                    //    str.AppendLine();
                    //}
                    //if (spiResult != null)
                    //{
                    //    StartIndexReportSPI = 0;
                    //    if (spiResult.SPIFrameCollection != null)
                    //        EndIndexReportSPI = spiResult.SPIFrameCollection.Count - 1;
                    //    else
                    //        EndIndexReportSPI = 0;
                    //    AddToSPI_CSV(ref str);
                    //    str.AppendLine();
                    //    str.AppendLine();
                    //}
                    //if (uartResult != null)
                    //{
                    //    StartIndexReportUART = 0;
                    //    if (uartResult.WareHouse.UARTFrameCollection != null)
                    //        EndIndexReportUART = uartResult.WareHouse.UARTFrameCollection.Count - 1;
                    //    else
                    //        EndIndexReportUART = 0;
                    //    AddToUART_CSV(uartResult, ref str);
                    //    str.AppendLine();
                    //    str.AppendLine();
                    //}
                }
                else if (config.ConfigurationMode == DataModule.eConfigMode.LA_Mode || config.ConfigurationMode == DataModule.eConfigMode.Both)
                {
                    AddWaveformListing(ref str);
                    str.AppendLine();
                    str.AppendLine();
                }
                File.WriteAllText(path, str.ToString());
            }
            //Task.Factory.StartNew(() =>
            //{

            //});
            System.Windows.MessageBoxResult messageBoxResult = MessageBox.Show("CSV Report Generated.\nDo you want to open the report?", "PGY-LA-EMBD", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
            if (messageBoxResult == System.Windows.MessageBoxResult.Yes)
            {
                if (File.Exists(path))
                {
                    Process.Start(path);
                }
            }
        }

       


        private void AddProtocolToCSV(ref StringBuilder strResult)
        {
            ConfigModel config = configuration.Config;
            foreach (var result in results.ProtocolResults)
            {
                if (result.Config.ProtocolType == eProtocol.I3C)
                {
                    StringBuilder str = new StringBuilder();
                    str.AppendLine();
                    str.Append("Time,");

                    str.Append("Frame,");
                    str.Append("Message,");
                    str.Append("Address,");
                    str.Append("Command,");
                    str.Append("Data,");
                    str.Append("Frequency,");
                    str.Append("Error,");




                    str.Length--;
                    strResult.AppendLine(str.ToString());

                    var resultCollection = results.WareHouse.ResultsI3C_Combined.ToList().OrderBy(f => f.StartTime);

                    string datStr = "";


                    int index = 1;

                    String[] strTemp1 = new String[11];


                    int n = 0;

                    foreach (var frame in resultCollection.OfType<FramePattern>())
                    {

                        {
                            StringBuilder strTemp = new StringBuilder();



                            strTemp.Append(strTemp1[0] + CUtilities.ToEngineeringNotation(frame.StartTime - SessionConfiguration.TriggerTime).Replace(",", "").ToString() + "s" + ",");


                            strTemp.Append(frame.ProtocolName + ",");
                            strTemp.Append(frame.FrameType + ",");

                            datStr = "";
                            StringBuilder command = new StringBuilder();
                            StringBuilder address = new StringBuilder();
                            StringBuilder data = new StringBuilder();
                            foreach (var item in frame.PacketCollection)
                            {
                                if (item is CommandMessageModel && item.PacketType == ePacketType.Command)
                                    command.Append(ProtocolInfoRepository.GetCommand(((CommandMessageModel)item).Value).ToString() + "-");

                                else if (item is AddressMessageModel && item.PacketType == ePacketType.Address)
                                {
                                    address.Append("0x" + (item as AddressMessageModel).Value.ToString("X2") + "-");
                                }
                                else if (item is DataMessageModel && item.PacketType == ePacketType.Data)
                                {
                                    data.Append("0x" + (item as DataMessageModel).Value.ToString("X2") + "-");
                                }
                                else if (item is PIDDAAMessageModel && item.PacketType == ePacketType.Data)
                                {
                                    data.Append("0x" + (item as PIDDAAMessageModel).Value.ToString("X4") + "-");
                                }
                                else if (item is HDRMessageModel)
                                {
                                    if (item.PacketType == ePacketType.HDR_Command)
                                        data.Append("0x" + (item as HDRMessageModel).Value.ToString("X4") + "-");
                                    else if (item.PacketType == ePacketType.HDR_Data || item.PacketType == ePacketType.HDR_CRC)
                                        data.Append("0x" + (item as HDRMessageModel).Value.ToString("X4") + "-");
                                }
                            }
                            if (address.Length > 0)
                                address.Remove(address.Length - 1, 1).ToString();
                            else
                                address.Append("-");
                            if (command.Length > 0)
                                command.Remove(command.Length - 1, 1).ToString();
                            else
                                command.Append("-");
                            if (data.Length > 0)
                                data.Remove(data.Length - 1, 1).ToString();
                            else
                                data.Append("-");

                            strTemp.Append(address + ",");
                            strTemp.Append(command + ",");
                            strTemp.Append(data + ",");
                            strTemp.Append(CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz" + ",");
                            strTemp.Append(frame.ErrorType.ToString() + ",");




                            strTemp.Length--;
                            strResult.AppendLine(strTemp.ToString());

                        }
                    }

                }

                if (result.Config.ProtocolType == eProtocol.I2C)
                {
                    StringBuilder str = new StringBuilder();
                    str.AppendLine();
                    str.Append("Time,");

                    str.Append("Frame,");
                    str.Append("S/Sr,");
                    str.Append("Address,");
                    str.Append("Read/Write,");
                    str.Append("A/N,");
                    str.Append("Data,");
                    str.Append("Frequency,");
                    str.Append("Error,");
                    string ackString = "";
                    str.Length--;
                    strResult.AppendLine(str.ToString());

                    string datStr = "";
                    var I2C_result = result as ResultViewModel_I2C;
                    var resultI2C = I2C_result.FrameCollection;


                    foreach (var frame in resultI2C.OfType<I2CFrame>())
                    {
                        StringBuilder strTemp = new StringBuilder();
                        strTemp.Append(CUtilities.ToEngineeringNotation(frame.StartTime - SessionConfiguration.TriggerTime).Replace(",", "").ToString() + "s" + ",");
                        strTemp.Append("I2C,");
                        strTemp.Append(frame.Start.ToString() + ",");
                        strTemp.Append("0x" + (frame.AddressFirst >> 1).ToString("X2") + ",");
                        strTemp.Append((frame.AddressFirst & 0x01) == 0 ? "WR," : "RD,");
                        if ((frame.AckAddr & 0x0F) == 0x0F)
                            ackString = "Nack";
                        else
                            ackString = "Ack";
                        strTemp.Append(ackString + ",");
                        datStr = "";
                        foreach (var dat in frame.DataBytes)
                        {
                            datStr += "0x" + dat.ToString("X2");
                            datStr += " : ";
                        }
                        if (datStr == "")
                            datStr = "-";
                        else
                            datStr = datStr.Substring(0, datStr.Length - 2);
                        strTemp.Append(datStr + ",");
                        strTemp.Append(CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz" + ",");
                        strTemp.Append(frame.ErrorType.ToString() + ",");

                        strTemp.Length--;
                        strResult.AppendLine(strTemp.ToString());

                    }
                }
              
                if (result.Config.ProtocolType == eProtocol.SPI)
                {

                    StringBuilder str = new StringBuilder();
                    str.AppendLine();
                    str.Append("Time,");
                    str.Append("Frame,");
                    str.Append("MOSI,");
                    str.Append("MISO,");
                    str.Append("Frequency,");
                    str.Append("Stop,");
                    str.Append("Error,");

                    str.Length--;
                    strResult.AppendLine(str.ToString());

                    string datStr = "";
                    var SPI_result = result as ResultViewModel_SPI;
                    var resultSPI = SPI_result.SPIFrameCollection;
                    string mOSIStr = "";
                    string mISOStr = "";

                    foreach (var frame in resultSPI.OfType<SPIFrame>())
                    {
                        StringBuilder strTemp = new StringBuilder();
                        strTemp.Append(CUtilities.ToEngineeringNotation(frame.StartTime - SessionConfiguration.TriggerTime).Replace(",", "").ToString() + "s" + ",");
                        strTemp.Append("SPI,");
                        mOSIStr = "";
                        foreach (var dat in frame.MOSIDataBytes)
                        {
                            mOSIStr += "0x" + dat.ToString("X2");
                            mOSIStr += " : ";
                        }
                        if (mOSIStr == "")
                            mOSIStr = "-";
                        else
                            mOSIStr = mOSIStr.Substring(0, mOSIStr.Length - 2);
                        strTemp.Append(mOSIStr + ",");
                        mISOStr = "";
                        foreach (var dat in frame.MISODataBytes)
                        {
                            mISOStr += "0x" + dat.ToString("X2");
                            mISOStr += " : ";
                        }
                        if (mISOStr == "")
                            mISOStr = "-";
                        else
                            mISOStr = mISOStr.Substring(0, mISOStr.Length - 2);
                        strTemp.Append(mISOStr + ",");
                        strTemp.Append(CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz" + ",");
                        strTemp.Append(frame.HasStop.ToString() + ",");
                        strTemp.Append(frame.ErrorType.ToString() + ",");

                        strTemp.Length--;
                        strResult.AppendLine(strTemp.ToString());
                    }
                }

           
                if (result.Config.ProtocolType == eProtocol.UART)
                {
                    StringBuilder str = new StringBuilder();
                    str.AppendLine();
                    str.Append("Time,");
                    str.Append("Frame,");
                    str.Append("Tx Data,");
                    str.Append("Rx Data,");
                    str.Append("Frequency,");
                    str.Append("Stop,");
                    str.Append("Error,");

                    str.Length--;
                    strResult.AppendLine(str.ToString());

                    var UART_result = result as ResultViewModel_UART;
                    var resultUART = UART_result.WareHouse.UARTFrameCollection;

                    foreach (var frame in resultUART.OfType<UARTFrame>())
                    {
                        StringBuilder strTemp = new StringBuilder();
                        strTemp.Append(CUtilities.ToEngineeringNotation(frame.StartTime - SessionConfiguration.TriggerTime).Replace(",", "").ToString() + "s" + ",");
                        strTemp.Append("UART,");
                        strTemp.Append(((frame.TXDdataBytes == -1) ? "-," : ("0x" + frame.TXDdataBytes.ToString("X2") + ",")));
                        strTemp.Append(((frame.RXdataBytes == -1) ? "-," : ("0x" + frame.RXdataBytes.ToString("X2") + ",")));
                        strTemp.Append(CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz" + ",");
                        strTemp.Append(frame.HasStop.ToString() + ",");
                        strTemp.Append(frame.ErrorType.ToString() + ",");
                        strTemp.Length--;
                        strResult.AppendLine(strTemp.ToString());
                    }
                }

                if(result.Config.ProtocolType==eProtocol.QSPI)
                {
                    StringBuilder str = new StringBuilder();

                    str.Append("Time,");
                    str.Append("Frame,");
                    str.Append("Command,");
                    str.Append("Address,");
                    str.Append("Data,");
                    str.Append("Frequency,");
                    str.Append("ErrorType,");
                    str.Length--;
                    strResult.AppendLine(str.ToString());


                    var QSPI_result = result as ResultViewModel_QSPI;
                    var resultQSPI = QSPI_result.WareHouse.QSPIFrameCollection;

                    foreach (var frame in resultQSPI.OfType<QSPIFrame>())
                    {
                        StringBuilder strTemp = new StringBuilder();
                        strTemp.Append(CUtilities.ToEngineeringNotation(frame.StartTime - SessionConfiguration.TriggerTime).Replace(",", "").ToString() + "s" + ",");
                        strTemp.Append("QSPI,");



                        string datStr = "";
                        StringBuilder command = new StringBuilder();
                        StringBuilder address = new StringBuilder();
                        StringBuilder data = new StringBuilder();

                        foreach (var currentPacket in frame.PacketCollection)
                        {


                            if (currentPacket is CommandMessageModel && currentPacket.PacketType == ePacketType.Command)
                            {
                                command.Append(ProtocolInfoRepository.GetQSPICommandInfo((currentPacket as CommandMessageModel).PacketValue).QSPICommands.ToString());
                            }

                            else if (currentPacket is AddressMessageModel && currentPacket.PacketType == ePacketType.Address)
                            {
                                address.Append("0x" + (currentPacket as AddressMessageModel).PacketValue.ToString("X2") + "-");
                            }

                            else if (currentPacket is DataMessageModel && currentPacket.PacketType == ePacketType.Data)
                            {
                                data.Append("0x" + (currentPacket as DataMessageModel).PacketValue.ToString("X2") + "-");
                            }

                        }
                        if (address.Length > 0)
                            address.Remove(address.Length - 1, 1).ToString();
                        else
                            address.Append("-");

                        if (data.Length > 0)
                            data.Remove(data.Length - 1, 1).ToString();
                        else
                            data.Append("-");



                        strTemp.Append(command + ",");
                        strTemp.Append(address + ",");
                        strTemp.Append(data + ",");

                        strTemp.Append(CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz" + ",");
                        strTemp.Append(frame.ErrorType.ToString() + ",");

                        strTemp.Length--;
                        strResult.AppendLine(strTemp.ToString());
                    }
                    }

                if (result.Config.ProtocolType == eProtocol.CAN)
                {
                    StringBuilder str = new StringBuilder();
                    str.AppendLine();
                    str.Append("Time,");

                    str.Append("Frame,");
                    str.Append("Frameformat,");
                    str.Append("ID,");
                    str.Append("IDE,");
                    str.Append("DLC,");
                    str.Append("Data,");
                    str.Append("CRC,");
                    str.Append("FrameType,");

                    foreach (var configViewModel in configuration.ProtocolConfiguration)
                    {

                        if (configViewModel is ConfigViewModel_CAN canconfig)
                        {

                            if (canconfig.BRS)
                            {
                                str.Append("Freq/BRSFreq,");

                            }
                            else
                            {
                                str.Append("Frequency,");
                            }


                            str.Append("ErrorType,");
                            str.Length--;
                            strResult.AppendLine(str.ToString());
                            string datStr = "";

                            var CAN_result = result as ResultViewModel_CAN;
                            var resultCAN = CAN_result.ResultCollection;

                            foreach (var frame in resultCAN.OfType<CANFrame>())
                            {
                                StringBuilder strTemp = new StringBuilder();
                                strTemp.Append(CUtilities.ToEngineeringNotation(frame.StartTime - SessionConfiguration.TriggerTime).Replace(",", "").ToString() + "s" + ",");
                                strTemp.Append("CAN,");

                                strTemp.Append(frame.Eframeformates + ",");
                                strTemp.Append(((frame.IDDdataBytes == -1) ? "-," : ("0x" + frame.IDDdataBytes.ToString("X2") + ",")));
                                strTemp.Append(((frame.IDEDdataBytes == -1) ? "-," : ("0x" + frame.IDEDdataBytes.ToString("X2") + ",")));
                                strTemp.Append(((frame.DLCDdataBytes == -1) ? "-," : ("0x" + frame.DLCDdataBytes.ToString("X2") + ",")));
                                datStr = "";
                                foreach (var dat in frame.DataBytes)
                                {
                                    datStr += "0x" + dat.ToString("X2");
                                    datStr += " : ";
                                }
                                if (datStr == "")
                                    datStr = "-";
                                else
                                    datStr = datStr.Substring(0, datStr.Length - 2);
                                strTemp.Append(((frame.DLCDdataBytes == 0) ? "-," : datStr + ","));
                                strTemp.Append(((frame.CRCDdataBytes == -1) ? "-," : ("0x" + frame.CRCDdataBytes.ToString("X2") + ",")));

                                datStr = "";
                                strTemp.Append(frame.FrameType + ",");
                                if (canconfig.BRS)
                                {
                                    strTemp.Append(CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT).Replace("M", "") + "/" + CUtilities.FormatNumber(frame.BRSFrequency, Units.BASE_UNIT) + "Hz" + ",");
                                }


                                else
                                {
                                    strTemp.Append(CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz" + ",");
                                }

                                strTemp.Append((frame.ErrorType + ","));

                                strTemp.Length--;
                                strResult.AppendLine(strTemp.ToString());
                            }
                        }
                    }
                }

                if (result.Config.ProtocolType == eProtocol.SPMI)
                {

                    StringBuilder str = new StringBuilder();
                    str.AppendLine();
                    str.Append("SI.no,");
                    str.Append("Time,");
                    str.Append("Frame,");
                    str.Append("A Bit,");
                    str.Append("Sr Bit,");
                    str.Append("Slave,");
                    str.Append("MID,");
                    str.Append("Command,");
                    str.Append("Reg Address,");
                    str.Append("Data,");
                    str.Append("ByteCount,");
                    str.Append("Frequency,");
                    str.Append("Error,");
                    str.Length--;
                    string datStr = "";
                    strResult.AppendLine(str.ToString());
                    var SPMI_result = result as ResultViewModel_SPMI;
                    var resultSPMI = SPMI_result.SPMIFrameCollection;

                    int index = 1;
                    foreach (var frame in resultSPMI.OfType<SPMIFrameStructure>())
                    {
                      
                        StringBuilder strTemp = new StringBuilder();
                        strTemp.Append(index.ToString()+",");
                        strTemp.Append(CUtilities.ToEngineeringNotation(frame.StartTime - SessionConfiguration.TriggerTime).Replace(",", "").ToString() + "s" + ",");
                        
                        strTemp.Append("SPMI,");
                        strTemp.Append(frame.HasA_bit.ToString()+",");
                        strTemp.Append(frame.HasSr_bit.ToString() + ",");
                        strTemp.Append(((frame.SlaveIdBind == -1) ? "-," : ("0x" + frame.SlaveIdBind.ToString("X2") + ",")));
                        strTemp.Append(((frame.MIDBind == -1) ? "-," : ("0x" + frame.MIDBind.ToString("X2") + ",")));
                        strTemp.Append(((frame.Command.CmdType.ToString() + ",")));
                        strTemp.Append((("0x" + frame.IntAddress.ToString("X2") + ",")));
                        strTemp.Append(frame.Data == null ? "-," : String.Join(" ", frame.Data.Select(obj => String.Format("0x{0:X}", obj.Value))) + ",");
                        strTemp.Append(((frame.ByteCount == -1) ? "-," : ("0x" + frame.ByteCount.ToString("X2") + ",")));
                        strTemp.Append(CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz" + ",");
                        strTemp.Append((frame.SPMIErrorType.Count > 0 ? "Error" : "Pass") + ",");
                        
                        strTemp.Length--;
                        strResult.AppendLine(strTemp.ToString());
                        index++;
                    }
                   
                }

                if (result.Config.ProtocolType == eProtocol.RFFE)
                {

                    StringBuilder str = new StringBuilder();
                    str.AppendLine();
                    str.Append("Time,");
                    str.Append("Frame,");
                    str.Append("Slave/MID,");
                    str.Append("Command,");
                    str.Append("Reg Address,");
                    str.Append("Data,");
                    str.Append("ByteCount,");
                    str.Append("Frequency,");
                    str.Append("Error,");
                    str.Length--;
                    string datStr = "";
                    strResult.AppendLine(str.ToString());
                    var RFFE_result = result as ResultViewModel_RFFE;
                    var resultRFFE = RFFE_result.RFFEFrameCollection;


                    foreach (var frame in resultRFFE.OfType<RFFEFrameStructure>())
                    {

                        StringBuilder strTemp = new StringBuilder();
                        strTemp.Append(CUtilities.ToEngineeringNotation(frame.StartTime - SessionConfiguration.TriggerTime).Replace(",", "").ToString() + "s" + ",");

                        strTemp.Append("RFFE,");
                        strTemp.Append(((frame.SlaveId == -1) ? "-," : ("0x" + frame.SlaveId.ToString("X2") + ",")));
                        strTemp.Append(((frame.Command.CmdType.ToString() + ",")));
                        strTemp.Append((("0x" + frame.IntAddress.ToString("X2") + ",")));
                        strTemp.Append(frame.Data == null ? "-," : String.Join(" ", frame.Data.Select(obj => String.Format("0x{0:X}", obj.Value))) + ",");
                        strTemp.Append(((frame.ByteCount == -1) ? "-," : ("0x" + frame.ByteCount.ToString("X2") + ",")));
                        strTemp.Append(CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz" + ",");
                        strTemp.Append(frame.ErrorType.ToString() + ",");

                        strTemp.Length--;
                        strResult.AppendLine(strTemp.ToString());
                    }
                }

            }
            
        }

 
        private void AddWaveformListing(ref StringBuilder strResult)
        {
            var result = waveformListing.WfmHolder.ASyncCollection;
            if (result == null)
                MessageBox.Show("Result not found", "Empty Collection", MessageBoxButton.OK, MessageBoxImage.Information);
            if (result.Count == 0)
                MessageBox.Show("Result not found", "Empty Collection", MessageBoxButton.OK, MessageBoxImage.Information);

            ConfigModel config = configuration.Config;
            WaveformListingViewModel WVM = waveformListing;
            String[] str = new String[18];
            int k = 0;
            str[k] = "S No";
            k++;
            str[k] = "TIMESTAMP";
            k++;
            if (config.IsCh16Visible)
            {
                str[k] = WVM.WfmList.Ch16Name;
                k++;
            }
            if (config.IsCh15Visible)
            {
                str[k] = WVM.WfmList.Ch15Name;
                k++;
            }
            if (config.IsCh14Visible)
            {
                str[k] = WVM.WfmList.Ch14Name;
                k++;
            }
            if (config.IsCh13Visible)
            {
                str[k] = WVM.WfmList.Ch13Name;
                k++;
            }
            if (config.IsCh12Visible)
            {
                str[k] = WVM.WfmList.Ch12Name;
                k++;
            }
            if (config.IsCh11Visible)
            {
                str[k] = WVM.WfmList.Ch11Name;
                k++;
            }
            if (config.IsCh10Visible)
            {
                str[k] = WVM.WfmList.Ch10Name;
                k++;
            }
            if (config.IsCh9Visible)
            {
                str[k] = WVM.WfmList.Ch9Name;
                k++;
            }
            if (config.IsCh8Visible)
            {
                str[k] = WVM.WfmList.Ch8Name;
                k++;
            }
            if (config.IsCh7Visible)
            {
                str[k] = WVM.WfmList.Ch7Name;
                k++;
            }
            if (config.IsCh6Visible)
            {
                str[k] = WVM.WfmList.Ch6Name;
                k++;
            }
            if (config.IsCh5Visible)
            {
                str[k] = WVM.WfmList.Ch5Name;
                k++;
            }
            if (config.IsCh4Visible)
            {
                str[k] = WVM.WfmList.Ch4Name;
                k++;
            }
            if (config.IsCh3Visible)
            {
                str[k] = WVM.WfmList.Ch3Name;
                k++;
            }
            if (config.IsCh2Visible)
            {
                str[k] = WVM.WfmList.Ch2Name;
                k++;
            }
            if (config.IsCh1Visible)
            {
                str[k] = WVM.WfmList.Ch1Name;
            }
            strResult.AppendLine(String.Join(",", str));

            int index = 1;
            startIndex = 0;
            stopIndex = result.Count - 1 > 10000000 ? 10000000 : result.Count - 1; // need to check for limit.
            List<DiscreteWaveForm> resul = Ioc.Default.GetService<IDataProvider>().GetWaveform(startIndex, stopIndex + 1);
            for (int i = startIndex; i <= stopIndex; i++)
            {
                String[] strTemp = new String[18];
                int j = 0;
                strTemp[j] = index.ToString();
                j++;
                strTemp[j] = CUtilities.ToEngineeringNotation(resul[i].TimeStamp - SessionConfiguration.TriggerTime).ToString() + "s";
                j++;
                if (config.IsCh16Visible)
                {
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch16").ToString();
                    j++;
                }
                if (config.IsCh15Visible)
                {
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch15").ToString();
                    j++;
                }
                if (config.IsCh14Visible)
                {
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch14").ToString();
                    j++;
                }
                if (config.IsCh13Visible)
                {
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch13").ToString();
                    j++;
                }
                if (config.IsCh12Visible)
                {
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch12").ToString();
                    j++;
                }
                if (config.IsCh11Visible)
                {
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch11").ToString();
                    j++;
                }
                if (config.IsCh10Visible)
                {
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch10").ToString();
                    j++;
                }
                if (config.IsCh9Visible)
                {
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch9").ToString();
                    j++;
                }
                if (config.IsCh8Visible)
                {
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch8").ToString();
                    j++;
                }
                if (config.IsCh7Visible)
                {
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch7").ToString();
                    j++;
                }
                if (config.IsCh6Visible)
                {
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch6").ToString();
                    j++;
                }
                if (config.IsCh5Visible)
                {
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch5").ToString();
                    j++;
                }
                if (config.IsCh4Visible)
                {
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch4").ToString();
                    j++;
                }
                if (config.IsCh3Visible)
                {
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch3").ToString();
                    j++;
                }
                if (config.IsCh2Visible)
                {
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch2").ToString();
                    j++;
                }
                if (config.IsCh1Visible)
                {
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch1").ToString();
                }
                strResult.AppendLine(String.Join(",", strTemp));
                index++;
            }
        }

        private void GenerateReportDialog(string param)
        {
            IsStandard = false;
            if (param == "Standard")
                IsStandard = true;
            ImageList = new ObservableCollection<ImageStructure>();
            string capture = Path.GetFileNameWithoutExtension(SessionConfiguration.TraceFilePath);
            string folder = MiniCooperDirectoryInfo.ImagePath + @"\" + capture;
            if (Directory.Exists(folder))
            {
                foreach (string F in System.IO.Directory.GetFiles(folder, "*.png"))
                {
                    ImageList.Add(new ImageStructure()
                    {
                        ImagePath = F
                    });
                }
            }
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            //var view = new ReportView
            //{
            //    DataContext = this
            //};

            //show the dialog
            //var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);

            //if (result != null && result.ToString().ToLower() == "true")
            //{
            //    // SelRange = true;
            //    GeneratePDF(o);
            //}
            GeneratePDF();
            //check the result...
            //Console.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL"));
        }
        string path;
        public void GeneratePDF()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF file (*.pdf)|*.pdf";
            if (saveFileDialog.ShowDialog() == true)
            {
                path = saveFileDialog.FileName;
                if (IsFileLocked(new FileInfo(path)))
                {
                    MessageBox.Show("File is in use", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return;
                }
                if (!Directory.Exists(new FileInfo(path).Directory.FullName))
                {
                    MessageBox.Show("Directory not found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return;
                }
                // Create a renderer for PDF that uses Unicode font encoding
                try
                {
                    Thread thread = new Thread(new ThreadStart(ThreadFunction));
                    thread.IsBackground = true;
                    thread.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in Report Rendering - " + ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void ThreadFunction()
        {
            string prevStatus = results.Status;
            //results.Status = "Generating Report";
            ReportStatusUpdated?.Invoke("Generating Report");
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
            Document document = CreateDocument();
            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();
            pdfRenderer.PdfDocument.PageLayout = PdfPageLayout.OneColumn;
            pdfRenderer.PdfDocument.Settings.TrimMargins.Left = 100;
            pdfRenderer.PdfDocument.Save(new FileStream(path, FileMode.Create));

            ReportStatusUpdated?.Invoke("Report Generated");

            System.Windows.MessageBoxResult messageBoxResult = MessageBox.Show("PDF Report Generated.\nDo you want to open the report?", "PGY-LA-EMBD", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
            if (messageBoxResult == System.Windows.MessageBoxResult.Yes)
            {
                if (File.Exists(path))
                {
                    Process.Start(path);
                }
            }

            //analyzerViewModel.Status= prevStatus;
        }
        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            var i2CConfig = configuration.Config.ProtocolConfigList.OfType<ConfigModel_I2C>().FirstOrDefault();
            var spiConfig = configuration.Config.ProtocolConfigList.OfType<ConfigModel_SPI>().FirstOrDefault();
            var UARTConfig = configuration.Config.ProtocolConfigList.OfType<ConfigModel_UART>().FirstOrDefault();
            var i3cConfig = configuration.Config.ProtocolConfigList.OfType<ConfigModel_I3C>().FirstOrDefault();
            var spmiConfig = configuration.Config.ProtocolConfigList.OfType<ConfigModel_SPMI>().FirstOrDefault();
            var rffeConfig = configuration.Config.ProtocolConfigList.OfType<ConfigModel_RFFE>().FirstOrDefault();
            var qspiconfig = configuration.Config.ProtocolConfigList.OfType<ConfigModel_QSPI>().FirstOrDefault();
            var canconfig = configuration.Config.ProtocolConfigList.OfType<ConfigModel_CAN>().FirstOrDefault();
            Doc = new Document();
            Doc.Info.Title = "PGY-LA-EMBD Test Report";
            Doc.Info.Subject = "Shows the result values for the current collection";
            Doc.Info.Author = "Prodigy Technovations";
            try
            {
                DefineStyles();
                CreateFirstPage();
                CreateSecondPage();
                if ((configuration.Config.ConfigurationMode == DataModule.eConfigMode.PA_Mode || configuration.Config.ConfigurationMode == DataModule.eConfigMode.Both))
                {
                    if ((!IsStandard && IsProtocolSelected) || IsStandard)
                    {
                        foreach (var result in results.ProtocolResults)

                        {
                            if (result.Config.ProtocolType == eProtocol.I3C)
                            {
                                var resul = result as ResultViewModel_I3C;
                                var resultCollection = resul.ResultCollection.Count;

                                if (SelRange && StartIndex < resultCollection && StopIndex < resultCollection
                                     && StartIndex < StopIndex && selRange && !IsStandard)
                                {
                                    StartIndexReportI3C = StartIndex;
                                    EndIndexReportI3C = StopIndex;
                                }
                                else
                                {
                                    StartIndexReportI3C = 0;
                                    EndIndexReportI3C = resultCollection - 1;
                                }
                                if (IsStandard)
                                {
                                    StartIndexReportI3C = 0;
                                    EndIndexReportI3C = resultCollection - 1;
                                }
                                if (EndIndexReportI3C - StartIndexReportI3C > 499) // limitation for no. of frames
                                    EndIndexReportI3C = StartIndexReportI3C + 499;
                                CreatePage("I3C");
                                FillContent("I3C");
                            }
                            else
                                if (result.Config.ProtocolType == eProtocol.I2C)
                            {
                                var i2cResult = (ResultViewModel_I2C)result;

                                if (SelRange && StartIndex < i2cResult.WareHouse.ResultCollection.Count && StopIndex < i2cResult.WareHouse.ResultCollection.Count
                                && StartIndex < StopIndex && selRange && !IsStandard)
                                {
                                    StartIndexReportI2C = StartIndex;
                                    EndIndexReportI2C = StopIndex;
                                }
                                else
                                {
                                    StartIndexReportI2C = 0;
                                    EndIndexReportI2C = i2cResult.WareHouse.ResultCollection.Count - 1;
                                }
                                if (IsStandard)
                                {
                                    StartIndexReportI2C = 0;
                                    EndIndexReportI2C = i2cResult.WareHouse.ResultCollection.Count - 1;
                                }
                                if (EndIndexReportI2C - StartIndexReportI2C > 499) // limitation for no. of frames
                                    EndIndexReportI2C = StartIndexReportI2C + 499;
                                CreatePage("I2C");
                                FillContent("I2C");
                            }
                            else
                                if (result.Config.ProtocolType == eProtocol.SPI)
                            {
                                var spiResult = (ResultViewModel_SPI)result;

                                if (SelRange && StartIndex < spiResult.WareHouse.SPIFrameCollection.Count && StopIndex < spiResult.WareHouse.SPIFrameCollection.Count
                                    && StartIndex < StopIndex && selRange && !IsStandard)
                                {
                                    StartIndexReportSPI = StartIndex;
                                    EndIndexReportSPI = StopIndex;
                                }
                                else
                                {
                                    StartIndexReportSPI = 0;
                                    EndIndexReportSPI = spiResult.WareHouse.SPIFrameCollection.Count - 1;
                                }
                                if (IsStandard)
                                {
                                    StartIndexReportSPI = 0;
                                    EndIndexReportSPI = spiResult.WareHouse.SPIFrameCollection.Count - 1;
                                }
                                if (EndIndexReportSPI - StartIndexReportSPI > 499) // limitation for no. of frames
                                    EndIndexReportSPI = StartIndexReportSPI + 499;
                                CreatePage("SPI");
                                FillContent("SPI");
                            }
                            else
                                if (result.Config.ProtocolType == eProtocol.UART)
                            {
                                var uartResult = (ResultViewModel_UART)result;

                                if (SelRange && StartIndex < uartResult.WareHouse.UARTFrameCollection.Count && StopIndex < uartResult.WareHouse.UARTFrameCollection.Count
                                    && StartIndex < StopIndex && selRange && !IsStandard)
                                {
                                    StartIndexReportUART = StartIndex;
                                    EndIndexReportUART = StopIndex;
                                }
                                else
                                {
                                    StartIndexReportUART = 0;
                                    EndIndexReportUART = uartResult.WareHouse.UARTFrameCollection.Count - 1;
                                }
                                if (IsStandard)
                                {
                                    StartIndexReportUART = 0;
                                    EndIndexReportUART = uartResult.WareHouse.UARTFrameCollection.Count - 1;
                                }
                                if (EndIndexReportUART - StartIndexReportUART > 499) // limitation for no. of frames
                                    EndIndexReportUART = StartIndexReportUART + 499;
                                CreatePage("UART");
                                FillContent("UART");
                            }
                            else
                                if (result.Config.ProtocolType == eProtocol.SPMI)
                            {
                                var spmiResult = (ResultViewModel_SPMI)result;

                                if (SelRange && StartIndex < spmiResult.WareHouse.FrameCollection.Count && StopIndex < spmiResult.WareHouse.FrameCollection.Count
                                    && StartIndex < StopIndex && selRange && !IsStandard)
                                {
                                    StartIndexReportSPMI = StartIndex;
                                    EndIndexReportSPMI = StopIndex;
                                }
                                else
                                {
                                    StartIndexReportSPMI = 0;
                                    EndIndexReportSPMI = spmiResult.WareHouse.FrameCollection.Count - 1;
                                }
                                if (IsStandard)
                                {
                                    StartIndexReportSPMI = 0;
                                    EndIndexReportSPMI = spmiResult.WareHouse.FrameCollection.Count - 1;
                                }
                                if (EndIndexReportSPMI - StartIndexReportSPMI > 499) // limitation for no. of frames
                                    EndIndexReportSPMI = StartIndexReportSPMI + 499;
                                CreatePage("SPMI");
                                FillContent("SPMI");
                            }
                            else
                                if (result.Config.ProtocolType == eProtocol.QSPI)
                            {
                                //var qspiResult = (ResultViewModel_QSPI)result;
                                var resul = result as ResultViewModel_QSPI;
                                var resultCollection = resul.QSPIFrameCollection.Count;
                                if (SelRange && StartIndex < resultCollection && StopIndex < resultCollection
                                && StartIndex < StopIndex && selRange && !IsStandard)
                                {
                                    StartIndexReportQSPI = StartIndex;
                                    EndIndexReportQSPI = StopIndex;
                                }
                                else
                                {
                                    StartIndexReportQSPI = 0;
                                    EndIndexReportQSPI = resultCollection - 1;
                                }
                                if (IsStandard)
                                {
                                    StartIndexReportQSPI = 0;
                                    EndIndexReportQSPI =resultCollection - 1;
                                }
                                if (EndIndexReportQSPI - StartIndexReportQSPI > 499) // limitation for no. of frames
                                    EndIndexReportQSPI = StartIndexReportQSPI + 499;
                                CreatePage("QSPI");
                                FillContent("QSPI");
                            }
                            else
                                if (result.Config.ProtocolType == eProtocol.RFFE)
                            {
                                var rffeResult = (ResultViewModel_RFFE)result;

                                if (SelRange && StartIndex < rffeResult.WareHouse.FrameCollection.Count && StopIndex < rffeResult.WareHouse.FrameCollection.Count
                                    && StartIndex < StopIndex && selRange && !IsStandard)
                                {
                                    StartIndexReportRFFE = StartIndex;
                                    EndIndexReportRFFE = StopIndex;
                                }
                                else
                                {
                                    StartIndexReportRFFE = 0;
                                    EndIndexReportRFFE = rffeResult.WareHouse.FrameCollection.Count - 1;
                                }
                                if (IsStandard)
                                {
                                    StartIndexReportRFFE = 0;
                                    EndIndexReportRFFE = rffeResult.WareHouse.FrameCollection.Count - 1;
                                }
                                if (EndIndexReportRFFE - StartIndexReportRFFE > 499) // limitation for no. of frames
                                    EndIndexReportRFFE = StartIndexReportRFFE + 499;
                                CreatePage("RFFE");
                                FillContent("RFFE");

                            }
                            else
                                if (result.Config.ProtocolType == eProtocol.CAN)
                            {
                                var canResult = (ResultViewModel_CAN)result;

                                if (SelRange && StartIndex < canResult.WareHouse.CANFrameCollection.Count && StopIndex < canResult.WareHouse.CANFrameCollection.Count
                                    && StartIndex < StopIndex && selRange && !IsStandard)
                                {
                                    StartIndexReportCAN = StartIndex;
                                    EndIndexReportCAN = StopIndex;
                                }
                                else
                                {
                                    StartIndexReportCAN = 0;
                                    EndIndexReportCAN = canResult.WareHouse.CANFrameCollection.Count - 1;
                                }
                                if (IsStandard)
                                {
                                    StartIndexReportCAN = 0;
                                    EndIndexReportCAN = canResult.WareHouse.CANFrameCollection.Count - 1;
                                }
                                if (EndIndexReportCAN - StartIndexReportCAN > 499) // limitation for no. of frames
                                    EndIndexReportCAN = StartIndexReportCAN + 499;
                                CreatePage("CAN");
                                FillContent("CAN");
                            }

                        }
                    }
                }

                //if ((configuration.Config.ConfigurationMode == DataModule.eConfigMode.PA_Mode || configuration.Config.ConfigurationMode == DataModule.eConfigMode.Both))
                //{
                //    if ((!IsStandard && IsProtocolSelected) || IsStandard)
                //    {
                //        if (i2CConfig!=null )
                //        {
                //            if (SelRange && StartIndex < ResultModel_I2C.GetInstance().ResultCollection.Count && StopIndex < ResultModel_I2C.GetInstance().ResultCollection.Count
                //                && StartIndex < StopIndex && selRange && !IsStandard)
                //            {
                //                StartIndexReportI2C = StartIndex;
                //                EndIndexReportI2C = StopIndex;
                //            }
                //            else
                //            {
                //                StartIndexReportI2C = 0;
                //                EndIndexReportI2C = ResultModel_I2C.GetInstance().ResultCollection.Count - 1;
                //            }
                //            if (IsStandard)
                //            {
                //                StartIndexReportI2C = 0;
                //                EndIndexReportI2C = ResultModel_I2C.GetInstance().ResultCollection.Count - 1;
                //            }
                //            if (EndIndexReportI2C - StartIndexReportI2C > 499) // limitation for no. of frames
                //                EndIndexReportI2C = StartIndexReportI2C + 499;
                //            CreatePage("I2C");
                //            FillContent("I2C");
                //        }
                //        if (spiConfig!=null )
                //        {
                //            if (SelRange && StartIndex < ResultModel_SPI.GetInstance().SPIFrameCollection.Count && StopIndex < ResultModel_SPI.GetInstance().SPIFrameCollection.Count
                //                && StartIndex < StopIndex && selRange && !IsStandard)
                //            {
                //                StartIndexReportSPI = StartIndex;
                //                EndIndexReportSPI = StopIndex;
                //            }
                //            else
                //            {
                //                StartIndexReportSPI = 0;
                //                EndIndexReportSPI = ResultModel_SPI.GetInstance().SPIFrameCollection.Count - 1;
                //            }
                //            if (IsStandard)
                //            {
                //                StartIndexReportSPI = 0;
                //                EndIndexReportSPI = ResultModel_SPI.GetInstance().SPIFrameCollection.Count - 1;
                //            }
                //            if (EndIndexReportSPI - StartIndexReportSPI > 499) // limitation for no. of frames
                //                EndIndexReportSPI = StartIndexReportSPI + 499;
                //            CreatePage("SPI");
                //            FillContent("SPI");
                //        }
                //        if (UARTConfig!=null)
                //        {
                //            if (SelRange && StartIndex < ResultModel_UART.GetInstance().UARTFrameCollection.Count && StopIndex < ResultModel_UART.GetInstance().UARTFrameCollection.Count
                //                && StartIndex < StopIndex && selRange && !IsStandard)
                //            {
                //                StartIndexReportUART = StartIndex;
                //                EndIndexReportUART = StopIndex;
                //            }
                //            else
                //            {
                //                StartIndexReportUART = 0;
                //                EndIndexReportUART = ResultModel_UART.GetInstance().UARTFrameCollection.Count - 1;
                //            }
                //            if (IsStandard)
                //            {
                //                StartIndexReportUART = 0;
                //                EndIndexReportUART = ResultModel_UART.GetInstance().UARTFrameCollection.Count - 1;
                //            }
                //            if (EndIndexReportUART - StartIndexReportUART > 499) // limitation for no. of frames
                //                EndIndexReportUART = StartIndexReportUART + 499;
                //            CreatePage("UART");
                //            FillContent("UART");
                //        }
                //        if (i3cConfig!=null)
                //        {


                //            foreach (var i3cConfig1 in configuration.Config.ProtocolConfigList.OfType<ConfigModel_I3C>())
                //            {

                //               // //TODO:update code here
                //               // if (SelRange && StartIndex < I3CResults.FrameCollection.Count && StopIndex < I3CResults.FrameCollection.Count
                //               //&& StartIndex < StopIndex && selRange && !IsStandard)
                //               // {
                //               //     StartIndexReportI3C = StartIndex;
                //               //     EndIndexReportI3C = StopIndex;
                //               // }
                //               // else
                //               // {
                //               //     StartIndexReportI3C = 0;
                //               //     EndIndexReportI3C = I3CResults.FrameCollection.Count - 1;
                //               // }
                //               // if (IsStandard)
                //               // {
                //               //     StartIndexReportI3C = 0;
                //               //     EndIndexReportI3C = I3CResults.FrameCollection.Count - 1;
                //               // }
                //               // if (EndIndexReportI3C - StartIndexReportI3C > 499) // limitation for no. of frames
                //               //     EndIndexReportI3C = StartIndexReportI3C + 499;
                //               // CreatePage("I3C");
                //               // FillContent("I3C");

                //            }


                //        }
                //        if (spmiConfig!=null)
                //        {
                //            if (SelRange && StartIndex < ResultModel_SPMI.GetInstance().FrameCollection.Count && StopIndex < ResultModel_SPMI.GetInstance().FrameCollection.Count
                //                && StartIndex < StopIndex && selRange && !IsStandard)
                //            {
                //                StartIndexReportSPMI = StartIndex;
                //                EndIndexReportSPMI = StopIndex;
                //            }
                //            else
                //            {
                //                StartIndexReportSPMI = 0;
                //                EndIndexReportSPMI = ResultModel_SPMI.GetInstance().FrameCollection.Count - 1;
                //            }
                //            if (IsStandard)
                //            {
                //                StartIndexReportSPMI = 0;
                //                EndIndexReportSPMI = ResultModel_SPMI.GetInstance().FrameCollection.Count - 1;
                //            }
                //            if (EndIndexReportSPMI - StartIndexReportSPMI > 499) // limitation for no. of frames
                //                EndIndexReportSPMI = StartIndexReportSPMI + 499;
                //            CreatePage("SPMI");
                //            FillContent("SPMI");
                //        }
                //        if (rffeConfig!=null)
                //        {
                //            if (SelRange && StartIndex < ResultModel_RFFE.GetInstance().FrameCollection.Count && StopIndex < ResultModel_RFFE.GetInstance().FrameCollection.Count
                //                && StartIndex < StopIndex && selRange && !IsStandard)
                //            {
                //                StartIndexReportRFFE = StartIndex;
                //                EndIndexReportRFFE = StopIndex;
                //            }
                //            else
                //            {
                //                StartIndexReportRFFE = 0;
                //                EndIndexReportRFFE = ResultModel_RFFE.GetInstance().FrameCollection.Count - 1;
                //            }
                //            if (IsStandard)
                //            {
                //                StartIndexReportRFFE = 0;
                //                EndIndexReportRFFE = ResultModel_RFFE.GetInstance().FrameCollection.Count - 1;
                //            }
                //            if (EndIndexReportRFFE - StartIndexReportRFFE > 499) // limitation for no. of frames
                //                EndIndexReportRFFE = StartIndexReportRFFE + 499;
                //            CreatePage("RFFE");
                //            FillContent("RFFE");
                //        }


                //        if (canconfig!=null )
                //        {
                //            if (SelRange && StartIndex < ResultModel_CAN.GetInstance().CANFrameCollection.Count && StopIndex < ResultModel_CAN.GetInstance().CANFrameCollection.Count
                //                && StartIndex < StopIndex && selRange && !IsStandard)
                //            {
                //                StartIndexReportCAN = StartIndex;
                //                EndIndexReportCAN = StopIndex;
                //            }
                //            else
                //            {
                //                StartIndexReportCAN = 0;
                //                EndIndexReportCAN = ResultModel_CAN.GetInstance().CANFrameCollection.Count - 1;
                //            }
                //            if (IsStandard)
                //            {
                //                StartIndexReportCAN = 0;
                //                EndIndexReportCAN = ResultModel_CAN.GetInstance().CANFrameCollection.Count - 1;
                //            }
                //            if (EndIndexReportCAN - StartIndexReportCAN > 499) // limitation for no. of frames
                //                EndIndexReportCAN = StartIndexReportCAN + 499;
                //            CreatePage("CAN");
                //            FillContent("CAN");
                //        }

                //        //QSPI
                //        if (qspiconfig!=null)
                //        {
                //            if (SelRange && StartIndex < ResultModel_QSPI.GetInstance().QSPIFrameCollection.Count && StopIndex < ResultModel_QSPI.GetInstance().QSPIFrameCollection.Count
                //                && StartIndex < StopIndex && selRange && !IsStandard)
                //            {
                //                StartIndexReportQSPI = StartIndex;
                //                EndIndexReportQSPI= StopIndex;
                //            }
                //            else
                //            {
                //                StartIndexReportQSPI = 0;
                //                EndIndexReportQSPI = ResultModel_QSPI.GetInstance().QSPIFrameCollection.Count - 1;
                //            }
                //            if (IsStandard)
                //            {
                //                StartIndexReportQSPI = 0;
                //                EndIndexReportQSPI= ResultModel_QSPI.GetInstance().QSPIFrameCollection.Count - 1;
                //            }
                //            if (EndIndexReportQSPI - StartIndexReportQSPI > 499) // limitation for no. of frames
                //                EndIndexReportQSPI = StartIndexReportQSPI+ 499;
                //            CreatePage("QSPI");
                //            FillContent("QSPI");
                //        }
                //    }
                //}
                //else if (ConfigModel.GetInstance().ConfigurationMode == DataModule.eConfigMode.LA_Mode)
                //{
                //    if ((!IsStandard && IsLogicSelected) || IsStandard)
                //    {
                //        CreatePageLA();
                //        FillContentLA();
                //    }
                //}
                if ((!IsStandard && IsLogicSelected) || IsStandard)
                {
                    CreatePageLA();
                    FillContentLA();
                }
                AddRemarks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Report Generation - " + ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return Doc;
        }

        private void DefineStyles()
        {
            // Get the predefined style Normal.
            MigraDoc.DocumentObjectModel.Style style = this.Doc.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";


            style = this.Doc.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = this.Doc.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = this.Doc.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            // style.Font.Name = "Times New Roman";
            style.Font.Size = 9;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            // Create a new style called Reference based on style Normal
            style = this.Doc.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
            style.ParagraphFormat.PageBreakBefore = true;
        }

        public void CreateFirstPage()
        {
            // Each MigraDoc document needs at least one section.
            double A4_WIDTH = PdfSharp.Drawing.XUnit.FromCentimeter(21).Point;
            Section section = Doc.AddSection();

            try
            {
                #region PGY Logo
                BitmapImage img = new BitmapImage(new Uri("pack://application:,,,/PGYMiniCooper.CoreModule;component/Resources/Logo.png", UriKind.Absolute));
                PngBitmapEncoder pbe = new PngBitmapEncoder();
                pbe.Frames.Add(BitmapFrame.Create(img));
                using (FileStream fs = new FileStream("PGY_Logo.png", FileMode.Create))
                {
                    pbe.Save(fs);
                }
                Image image = section.Headers.Primary.AddImage("PGY_Logo.png");
                image.Height = "1cm";
                image.LockAspectRatio = true;
                image.RelativeVertical = RelativeVertical.Line;
                image.RelativeHorizontal = RelativeHorizontal.Margin;
                image.Top = ShapePosition.Top;
                image.Left = ShapePosition.Right;
                image.WrapFormat.Style = WrapStyle.Through;
                #endregion

                if (LogoFilePath != "")
                {
                    try
                    {
                        image = section.Headers.Primary.AddImage(LogoFilePath);
                        image.Height = "1cm";
                        image.LockAspectRatio = true;
                        image.RelativeVertical = RelativeVertical.Line;
                        image.RelativeHorizontal = RelativeHorizontal.Margin;
                        image.Top = ShapePosition.Top;
                        image.Left = ShapePosition.Left;
                        image.WrapFormat.Style = WrapStyle.Through;
                        LogoFilePath = "";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Exception");
                    }
                }
            }
            catch (Exception ex)
            {

            }
            AddHeaderFooter(section);

            Paragraph para = section.AddParagraph();
            para.AddFormattedText("PGY-LA-EMBD Test Report", new Font("Calibri", 18));
            para.Format.Font.Bold = true;
            para.Format.Alignment = ParagraphAlignment.Center;

            Table objTable;
            objTable = section.AddTable();
            objTable.Format.Font.Name = "Calibri";
            objTable.Format.Font.Size = 14;
            objTable.TopPadding = 5;
            objTable.BottomPadding = 5;

            objTable.Borders.Color = Colors.Transparent;
            objTable.Borders.Visible = true;


            objTable.AddColumn(A4_WIDTH * 0.35);
            objTable.AddColumn(A4_WIDTH * 0.02);
            objTable.AddColumn(A4_WIDTH * 0.2);
            objTable.AddColumn(A4_WIDTH * 0.05);
            objTable.AddColumn(A4_WIDTH * 0.2);
            objTable.AddColumn(A4_WIDTH * 0.02);
            objTable.AddColumn(A4_WIDTH * 0.2);
            Row row = objTable.AddRow();

            //Fill Content of first page           

            row = objTable.AddRow();
            para = section.AddParagraph();
            row.Cells[0].Format.Alignment = ParagraphAlignment.Justify;
            para = row.Cells[0].AddParagraph("Organisation Name");
            para.Format.Font.Italic = true;
            para.Format.Font.Bold = true;

            para = row.Cells[1].AddParagraph(":");
            //para.Format.Font.Italic = true;
            para.Format.Font.Bold = true;

            para = row.Cells[2].AddParagraph(OrganisationName == null ? "" : OrganisationName);
            row.Cells[2].MergeRight = 4;

            row = objTable.AddRow();
            para = row.Cells[0].AddParagraph("Project Name");
            para.Format.Font.Italic = true;
            para.Format.Font.Bold = true;

            para = row.Cells[1].AddParagraph(":");
            //para.Format.Font.Italic = true;
            para.Format.Font.Bold = true;

            para = row.Cells[2].AddParagraph(ProjectName == null ? "" : ProjectName);
            row.Cells[2].MergeRight = 4;
            // 3rd row DUT Type
            //row = objTable.AddRow();
            //para = row.Cells[0].AddParagraph("Test Name");
            //para.Format.Font.Italic = true;
            //para.Format.Font.Bold = true;

            //para = row.Cells[1].AddParagraph(":");
            ////para.Format.Font.Italic = true;
            //para.Format.Font.Bold = true;

            //para = row.Cells[2].AddParagraph(TestName == null ? "" : TestName);
            //row.Cells[2].MergeRight = 4;
            string date = DateTime.Now.ToLongDateString() + " : " + DateTime.Now.ToLongTimeString();
            row = objTable.AddRow();
            //First row only date is there..
            para = row.Cells[0].AddParagraph("Date");
            para.Format.Font.Bold = true;
            para.Format.Font.Italic = true;

            para = row.Cells[1].AddParagraph(":");
            para.Format.Font.Bold = true;
            //para.Format.Font.Italic = true;

            row.Cells[2].AddParagraph(date);
            row.Cells[2].MergeRight = 4;


            // 4th row Number of channels
            //row = objTable.AddRow();
            //para = row.Cells[0].AddParagraph("Description");
            //para.Format.Font.Italic = true;
            //para.Format.Font.Bold = true;

            //para = row.Cells[1].AddParagraph(":");
            ////para.Format.Font.Italic = true;
            //para.Format.Font.Bold = true;

            //para = row.Cells[2].AddParagraph(Description == null ? "" : Description);
            //row.Cells[2].MergeRight = 4;
            row = objTable.AddRow();
            para = row.Cells[0].AddParagraph("Prepared By");
            para.Format.Font.Italic = true;
            para.Format.Font.Bold = true;

            para = row.Cells[1].AddParagraph(":");
            //para.Format.Font.Italic = true;
            para.Format.Font.Bold = true;

            para = row.Cells[2].AddParagraph(DesignerName == null ? "" : DesignerName);
            row.Cells[2].MergeRight = 4;

            //Software Version
            row = objTable.AddRow();
            para = row.Cells[0].AddParagraph("Software Version");
            para.Format.Font.Italic = true;
            para.Format.Font.Bold = true;

            para = row.Cells[1].AddParagraph(":");
            //para.Format.Font.Italic = true;
            para.Format.Font.Bold = true;
            string version = Application.Current.MainWindow.GetType().Assembly.GetName().Version.ToString();
            para = row.Cells[2].AddParagraph(version == null ? "" : version);
            row.Cells[2].MergeRight = 4;
        }

        public void CreateSecondPage()
        {
            // Each MigraDoc document needs at least one section.
            Section section = Doc.AddSection();
            AddHeaderFooter(section);
            Paragraph para = section.AddParagraph();
            para.AddFormattedText("Setup Details\n\n", TextFormat.Underline);
            para.Format.Alignment = ParagraphAlignment.Left;
            para.Format.Font.Size = 12;

            ConfigModel config = configuration.Config;
            var i2CConfig = config.ProtocolConfigList.OfType<ConfigModel_I2C>().FirstOrDefault();
            var spiConfig = config.ProtocolConfigList.OfType<ConfigModel_SPI>().FirstOrDefault();
            var uartConfig = config.ProtocolConfigList.OfType<ConfigModel_UART>().FirstOrDefault();
            var i3cConfig = config.ProtocolConfigList.OfType<ConfigModel_I3C>().FirstOrDefault();
            var spmiConfig = config.ProtocolConfigList.OfType<ConfigModel_SPMI>().FirstOrDefault();
            var rffeconfig = config.ProtocolConfigList.OfType<ConfigModel_RFFE>().FirstOrDefault();
            var qspiconfig = config.ProtocolConfigList.OfType<ConfigModel_QSPI>().FirstOrDefault();
            var canconfig = config.ProtocolConfigList.OfType<ConfigModel_CAN>().FirstOrDefault();

            if (config.ConfigurationMode == DataModule.eConfigMode.PA_Mode || config.ConfigurationMode == DataModule.eConfigMode.Both)
            {
                if (!(canconfig != null))
                {
                    WriteVoltDetails(section);
                }
                section.AddParagraph("\n\n");
                if (uartConfig != null)
                {
                    WriteConfigurationDetails("UART", section);
                    section.AddParagraph("\n\n");
                }
                if (spiConfig != null)
                {
                    WriteConfigurationDetails("SPI", section);
                    section.AddParagraph("\n\n");
                }
                if (i2CConfig != null)
                {
                    WriteConfigurationDetails("I2C", section);
                    section.AddParagraph("\n\n");
                }

                if (i3cConfig != null)
                {
                    WriteConfigurationDetails("I3C", section);
                    section.AddParagraph("\n\n");
                }
                if (spmiConfig != null)
                {
                    WriteConfigurationDetails("SPMI", section);
                    section.AddParagraph("\n\n");
                }
                if (rffeconfig != null)
                {
                    WriteConfigurationDetails("RFFE", section);
                    section.AddParagraph("\n\n");
                }
                if (canconfig != null)
                {
                    WriteConfigurationDetails("CAN", section);
                    section.AddParagraph("\n\n");
                }
                //QSPI
                if (qspiconfig != null)
                {
                    WriteConfigurationDetails("QSPI", section);
                    section.AddParagraph("\n\n");
                }
            }
            if (config.ConfigurationMode == DataModule.eConfigMode.LA_Mode || config.ConfigurationMode == DataModule.eConfigMode.Both)
            {
                WriteConfigurationDetailsLA(section);
            }
        }

        public void WriteVoltDetails(Section section)
        {
            ConfigModel config = configuration.Config;
            //Table creation
            Table tbl1;
            tbl1 = section.AddTable();
            tbl1.Style = "Table";
            tbl1.Borders.Visible = true;
            tbl1.Format.Font.Name = "Calibri";
            tbl1.Format.Font.Size = 12;
            tbl1.Borders.Color = Colors.Black;
            tbl1.Borders.Width = 0.25;
            tbl1.Borders.Left.Width = 0.5;
            tbl1.Borders.Right.Width = 0.5;
            tbl1.Rows.LeftIndent = 0;
            tbl1.LeftPadding = 0;
            tbl1.RightPadding = 0;
            tbl1.TopPadding = 0;
            tbl1.BottomPadding = 0;

            tm = new TextMeasurement(tbl1.Format.Font);

            //Column definition
            Column column = tbl1.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column = tbl1.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            //Header Row
            Row HeaderRow = tbl1.AddRow();
            HeaderRow.HeadingFormat = true;
            HeaderRow.Format.Alignment = ParagraphAlignment.Center;
            HeaderRow.Format.Font.Bold = true;
            HeaderRow.Shading.Color = Colors.SkyBlue;
            HeaderRow.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(HeaderRow.Cells[0], "Voltage Configuration"));
            HeaderRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            HeaderRow.Cells[0].MergeRight = 1;

            string volt = "";
            Row row2 = tbl1.AddRow();
            row2.Format.Alignment = ParagraphAlignment.Center;
            row2.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row2.Cells[0], "Voltage - Ch1, Ch2"));
            row2.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row2.Cells[0].Format.Font.Bold = true;
            row2.Cells[0].Shading.Color = Colors.SteelBlue;
            volt = GetVolt(config.SignalAmpCh1_2);
            row2.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row2.Cells[1], volt));
            row2.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            Row row21 = tbl1.AddRow();
            row21.Format.Alignment = ParagraphAlignment.Center;
            row21.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row21.Cells[0], "Voltage - Ch3, Ch4"));
            row21.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row21.Cells[0].Format.Font.Bold = true;
            row21.Cells[0].Shading.Color = Colors.SteelBlue;
            volt = GetVolt(config.SignalAmpCh3_4);
            row21.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row21.Cells[1], volt));
            row21.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            Row row22 = tbl1.AddRow();
            row22.Format.Alignment = ParagraphAlignment.Center;
            row22.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row22.Cells[0], "Voltage - Ch5, Ch6"));
            row22.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row22.Cells[0].Format.Font.Bold = true;
            row22.Cells[0].Shading.Color = Colors.SteelBlue;
            volt = GetVolt(config.SignalAmpCh5_6);
            row22.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row22.Cells[1], volt));
            row22.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            Row row23 = tbl1.AddRow();
            row23.Format.Alignment = ParagraphAlignment.Center;
            row23.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row23.Cells[0], "Voltage - Ch7, Ch8"));
            row23.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row23.Cells[0].Format.Font.Bold = true;
            row23.Cells[0].Shading.Color = Colors.SteelBlue;
            volt = GetVolt(config.SignalAmpCh7_8);
            row23.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row23.Cells[1], volt));
            row23.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            Row row24 = tbl1.AddRow();
            row24.Format.Alignment = ParagraphAlignment.Center;
            row24.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row24.Cells[0], "Voltage - Ch9, Ch10"));
            row24.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row24.Cells[0].Format.Font.Bold = true;
            row24.Cells[0].Shading.Color = Colors.SteelBlue;
            volt = GetVolt(config.SignalAmpCh9_10);
            row24.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row24.Cells[1], volt));
            row24.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            Row row25 = tbl1.AddRow();
            row25.Format.Alignment = ParagraphAlignment.Center;
            row25.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row25.Cells[0], "Voltage - Ch11, Ch12"));
            row25.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row25.Cells[0].Format.Font.Bold = true;
            row25.Cells[0].Shading.Color = Colors.SteelBlue;
            volt = GetVolt(config.SignalAmpCh11_12);
            row25.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row25.Cells[1], volt));
            row25.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            Row row26 = tbl1.AddRow();
            row26.Format.Alignment = ParagraphAlignment.Center;
            row26.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row26.Cells[0], "Voltage - Ch13, Ch14"));
            row26.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row26.Cells[0].Format.Font.Bold = true;
            row26.Cells[0].Shading.Color = Colors.SteelBlue;
            volt = GetVolt(config.SignalAmpCh13_14);
            row26.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row26.Cells[1], volt));
            row26.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            Row row27 = tbl1.AddRow();
            row27.Format.Alignment = ParagraphAlignment.Center;
            row27.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row27.Cells[0], "Voltage - Ch15, Ch16"));
            row27.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row27.Cells[0].Format.Font.Bold = true;
            row27.Cells[0].Shading.Color = Colors.SteelBlue;
            volt = GetVolt(config.SignalAmpCh15_16);
            row27.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row27.Cells[1], volt));
            row27.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            tbl1.SetEdge(0, 1, 2, 8, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        }

        public void WriteConfigurationDetails(string protocolType, Section section)
        {
            ConfigModel config = configuration.Config;
            var i2CConfig = config.ProtocolConfigList.OfType<ConfigModel_I2C>().FirstOrDefault();
            var spiConfig = config.ProtocolConfigList.OfType<ConfigModel_SPI>().FirstOrDefault();
            var uartConfig = config.ProtocolConfigList.OfType<ConfigModel_UART>().FirstOrDefault();
            var i3cConfig = config.ProtocolConfigList.OfType<ConfigModel_I3C>().FirstOrDefault();
            var spmiConfig = config.ProtocolConfigList.OfType<ConfigModel_SPMI>().FirstOrDefault();
            var rffeconfig = config.ProtocolConfigList.OfType<ConfigModel_RFFE>().FirstOrDefault();
            var qspiconfig = config.ProtocolConfigList.OfType<ConfigModel_QSPI>().FirstOrDefault();
            var canconfig = config.ProtocolConfigList.OfType<ConfigModel_CAN>().FirstOrDefault();

            //Table creation
            Table tbl1;
            tbl1 = section.AddTable();
            tbl1.Style = "Table";
            tbl1.Borders.Visible = true;
            tbl1.Format.Font.Name = "Calibri";
            tbl1.Format.Font.Size = 12;
            tbl1.Borders.Color = Colors.Black;
            tbl1.Borders.Width = 0.25;
            tbl1.Borders.Left.Width = 0.5;
            tbl1.Borders.Right.Width = 0.5;
            tbl1.Rows.LeftIndent = 0;
            tbl1.LeftPadding = 0;
            tbl1.RightPadding = 0;
            tbl1.TopPadding = 0;
            tbl1.BottomPadding = 0;

            tm = new TextMeasurement(tbl1.Format.Font);

            //Column definition
            Column column = tbl1.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column = tbl1.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            //Header Row
            Row HeaderRow = tbl1.AddRow();
            HeaderRow.HeadingFormat = true;
            HeaderRow.Format.Alignment = ParagraphAlignment.Center;
            HeaderRow.Format.Font.Bold = true;
            HeaderRow.Shading.Color = Colors.SkyBlue;
            HeaderRow.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(HeaderRow.Cells[0], protocolType));
            HeaderRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            HeaderRow.Cells[0].MergeRight = 1;

            //Filling content
            Row row1 = tbl1.AddRow();
            row1.Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], "Channels Assigned"));
            row1.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[0].Format.Font.Bold = true;
            row1.Cells[0].Shading.Color = Colors.SteelBlue;

            if (protocolType == "UART")
                row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], ((uartConfig.ChannelIndex_RX == DataModule.eChannles.None) ? "" : ("Rx : " + uartConfig.ChannelIndex_RX.ToString() + " ; ")) + ((uartConfig.ChannelIndex_TX == DataModule.eChannles.None) ? "" : ("Tx : " + uartConfig.ChannelIndex_TX.ToString()))));
            else if (protocolType == "SPI")
                row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], ((spiConfig.ChannelIndex_CLK == DataModule.eChannles.None) ? "" : ("CLK : " + spiConfig.ChannelIndex_CLK.ToString() + " ; ")) + ((spiConfig.ChannelIndex_CS == DataModule.eChannles.None) ? "" : ("CS : " + spiConfig.ChannelIndex_CS.ToString() + " ; ")) +
                                                                                 ((spiConfig.ChannelIndex_MISO == DataModule.eChannles.None) ? "" : ("MISO : " + spiConfig.ChannelIndex_MISO.ToString() + " ; ")) + ((spiConfig.ChannelIndex_MOSI == DataModule.eChannles.None) ? "" : ("MOSI : " + spiConfig.ChannelIndex_MOSI.ToString()))));
            else if (protocolType == "I2C")
                row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], ((i2CConfig.ChannelIndex_SCL == DataModule.eChannles.None) ? "" : ("SCL : " + i2CConfig.ChannelIndex_SCL.ToString() + " ; ")) + ((i2CConfig.ChannelIndex_SDA == DataModule.eChannles.None) ? "" : ("SDA : " + i2CConfig.ChannelIndex_SDA.ToString()))));

            else if (protocolType == "I3C")
                row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], ((i3cConfig.ChannelIndex_SCL == DataModule.eChannles.None) ? "" : ("SCL : " + i3cConfig.ChannelIndex_SCL.ToString() + " ; ")) + ((i3cConfig.ChannelIndex_SDA == DataModule.eChannles.None) ? "" : ("SDA : " + i3cConfig.ChannelIndex_SDA.ToString()))));
            else if (protocolType == "SPMI")
                row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], ((spmiConfig.ChannelIndex_SCL == DataModule.eChannles.None) ? "" : ("CLK : " + spmiConfig.ChannelIndex_SCL.ToString() + " ; ")) + ((spmiConfig.ChannelIndex_SDA == DataModule.eChannles.None) ? "" : ("DATA : " + spmiConfig.ChannelIndex_SDA.ToString()))));
            else if (protocolType == "RFFE")
                row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], ((rffeconfig.ChannelIndex_SCL == DataModule.eChannles.None) ? "" : ("CLK : " + rffeconfig.ChannelIndex_SCL.ToString() + " ; ")) + ((rffeconfig.ChannelIndex_SDA == DataModule.eChannles.None) ? "" : ("DATA : " + rffeconfig.ChannelIndex_SDA.ToString()))));

            //QSPI
            else if (protocolType == "QSPI")
                row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], ((qspiconfig.ChannelIndex_CLK == DataModule.eChannles.None) ? "" : ("CLK : " + qspiconfig.ChannelIndex_CLK.ToString() + " ; ")) + ((qspiconfig.ChannelIndex_CS == DataModule.eChannles.None) ? "" : ("CS : " + qspiconfig.ChannelIndex_CS.ToString() + " ; ")) +
                                                                                 ((qspiconfig.ChannelIndex_D0 == DataModule.eChannles.None) ? "" : ("D0 : " + qspiconfig.ChannelIndex_D0.ToString() + " ; ")) + ((qspiconfig.ChannelIndex_D1 == DataModule.eChannles.None) ? "" : ("D1: " + qspiconfig.ChannelIndex_D1.ToString() + " ; ")) +
                                                                               ((qspiconfig.ChannelIndex_D2 == DataModule.eChannles.None) ? "" : ("D2 : " + qspiconfig.ChannelIndex_D2.ToString() + " ; ")) + ((qspiconfig.ChannelIndex_D3 == DataModule.eChannles.None) ? "" : ("D3 : " + qspiconfig.ChannelIndex_D3.ToString()))));
            row1.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            if (protocolType == "CAN")
                row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], ((canconfig.ChannelIndex == DataModule.eChannles.None) ? "" : ("CAN CH : " + canconfig.ChannelIndex.ToString() + " ; "))));

            row1.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            tbl1.SetEdge(0, 1, 2, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        }

        public void WriteConfigurationDetailsLA(Section section)
        {
            ConfigModel config = configuration.Config;
            //Table creation
            Table tbl1;
            tbl1 = section.AddTable();
            tbl1.Style = "Table";
            tbl1.Borders.Visible = true;
            tbl1.Format.Font.Name = "Calibri";
            tbl1.Format.Font.Size = 12;
            tbl1.Borders.Color = Colors.Black;
            tbl1.Borders.Width = 0.25;
            tbl1.Borders.Left.Width = 0.5;
            tbl1.Borders.Right.Width = 0.5;
            tbl1.Rows.LeftIndent = 0;
            tbl1.LeftPadding = 0;
            tbl1.RightPadding = 0;
            tbl1.TopPadding = 0;
            tbl1.BottomPadding = 0;

            tm = new TextMeasurement(tbl1.Format.Font);

            //Column definition
            Column column = tbl1.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column = tbl1.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            //Header Row
            Row HeaderRow = tbl1.AddRow();
            HeaderRow.HeadingFormat = true;
            HeaderRow.Format.Alignment = ParagraphAlignment.Center;
            HeaderRow.Format.Font.Bold = true;
            HeaderRow.Shading.Color = Colors.SkyBlue;
            HeaderRow.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(HeaderRow.Cells[0], "LA Configuration"));
            HeaderRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            HeaderRow.Cells[0].MergeRight = 1;

            //Filling content
            #region Clk1
            if (config.GeneralPurposeMode == DataModule.eGeneralPurpose.State)
            {
                Row row11 = tbl1.AddRow();
                row11.Format.Alignment = ParagraphAlignment.Center;
                row11.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row11.Cells[0], "Clk1 Channel"));
                row11.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row11.Cells[0].Format.Font.Bold = true;
                row11.Cells[0].Shading.Color = Colors.SteelBlue;
                string Clk1Chnl = config.SelectedClock1.ToString();
                row11.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row11.Cells[1], Clk1Chnl));
                row11.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            }
            #endregion
            #region Clk1 Grp1
            Row row1 = tbl1.AddRow();
            row1.Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], config.Clk1Grp1Text + " Channels"));
            row1.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[0].Format.Font.Bold = true;
            row1.Cells[0].Shading.Color = Colors.SteelBlue;
            string Clk1Grp1Chnls = "";
            if (config.SelectedCLK1_GRP1 != null)
            {
                if (config.SelectedCLK1_GRP1.Count != 0)
                {
                    foreach (var chn in config.SelectedCLK1_GRP1)
                    {
                        Clk1Grp1Chnls += (chn == DataModule.eChannles.None) ? "-" : chn.ToString();
                        Clk1Grp1Chnls += " : ";
                    }
                    Clk1Grp1Chnls = Clk1Grp1Chnls.Substring(0, Clk1Grp1Chnls.Length - 2);
                }
                else
                    Clk1Grp1Chnls = "-";
            }
            else
                Clk1Grp1Chnls = "-";
            row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], Clk1Grp1Chnls));
            row1.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            #endregion
            #region Clk1 Grp2
            Row row2 = tbl1.AddRow();
            row2.Format.Alignment = ParagraphAlignment.Center;
            row2.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row2.Cells[0], config.Clk1Grp2Text + " Channels"));
            row2.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row2.Cells[0].Format.Font.Bold = true;
            row2.Cells[0].Shading.Color = Colors.SteelBlue;
            string Clk1Grp2Chnls = "";
            if (config.SelectedCLK1_GRP2 != null)
            {
                if (config.SelectedCLK1_GRP2.Count != 0)
                {
                    foreach (var chn in config.SelectedCLK1_GRP2)
                    {
                        Clk1Grp2Chnls += (chn == DataModule.eChannles.None) ? "-" : chn.ToString();
                        Clk1Grp2Chnls += " : ";
                    }
                    Clk1Grp2Chnls = Clk1Grp2Chnls.Substring(0, Clk1Grp2Chnls.Length - 2);
                }
                else
                    Clk1Grp2Chnls = "-";
            }
            else
                Clk1Grp2Chnls = "-";
            row2.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row2.Cells[1], Clk1Grp2Chnls));
            row2.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            #endregion
            #region Clk1 Grp3
            Row row3 = tbl1.AddRow();
            row3.Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row3.Cells[0], config.Clk1Grp3Text + " Channels"));
            row3.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[0].Format.Font.Bold = true;
            row3.Cells[0].Shading.Color = Colors.SteelBlue;
            string Clk1Grp3Chnls = "";
            if (config.SelectedCLK1_GRP3 != null)
            {
                if (config.SelectedCLK1_GRP3.Count != 0)
                {
                    foreach (var chn in config.SelectedCLK1_GRP3)
                    {
                        Clk1Grp3Chnls += (chn == DataModule.eChannles.None) ? "-" : chn.ToString();
                        Clk1Grp3Chnls += " : ";
                    }
                    Clk1Grp3Chnls = Clk1Grp3Chnls.Substring(0, Clk1Grp3Chnls.Length - 2);
                }
                else
                    Clk1Grp3Chnls = "-";
            }
            else
                Clk1Grp3Chnls = "-";
            row3.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row3.Cells[1], Clk1Grp3Chnls));
            row3.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            #endregion
            if (config.HasTwoClockSources)
            {
                #region Clk2
                Row row21 = tbl1.AddRow();
                row21.Format.Alignment = ParagraphAlignment.Center;
                row21.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row21.Cells[0], "Clk2 Channel"));
                row21.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row21.Cells[0].Format.Font.Bold = true;
                row21.Cells[0].Shading.Color = Colors.SteelBlue;
                string Clk2Chnl = config.SelectedClock2.ToString();
                row21.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row21.Cells[1], Clk2Chnl));
                row21.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                #endregion
                #region Clk2 Grp1
                Row row4 = tbl1.AddRow();
                row4.Format.Alignment = ParagraphAlignment.Center;
                row4.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row4.Cells[0], config.Clk2Grp1Text + " Channels"));
                row4.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row4.Cells[0].Format.Font.Bold = true;
                row4.Cells[0].Shading.Color = Colors.SteelBlue;
                string Clk2Grp1Chnls = "";
                if (config.SelectedCLK2_GRP1 != null)
                {
                    if (config.SelectedCLK2_GRP1.Count != 0)
                    {
                        foreach (var chn in config.SelectedCLK2_GRP1)
                        {
                            Clk2Grp1Chnls += (chn == DataModule.eChannles.None) ? "-" : chn.ToString();
                            Clk2Grp1Chnls += " : ";
                        }
                        Clk2Grp1Chnls = Clk2Grp1Chnls.Substring(0, Clk2Grp1Chnls.Length - 2);
                    }
                    else
                        Clk2Grp1Chnls = "-";
                }
                else
                    Clk2Grp1Chnls = "-";
                row4.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row4.Cells[1], Clk2Grp1Chnls));
                row4.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                #endregion
                #region Clk2 Grp2
                Row row5 = tbl1.AddRow();
                row5.Format.Alignment = ParagraphAlignment.Center;
                row5.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row5.Cells[0], config.Clk2Grp2Text + " Channels"));
                row5.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row5.Cells[0].Format.Font.Bold = true;
                row5.Cells[0].Shading.Color = Colors.SteelBlue;
                string Clk2Grp2Chnls = "";
                if (config.SelectedCLK2_GRP2 != null)
                {
                    if (config.SelectedCLK2_GRP2.Count != 0)
                    {
                        foreach (var chn in config.SelectedCLK2_GRP2)
                        {
                            Clk2Grp2Chnls += (chn == DataModule.eChannles.None) ? "-" : chn.ToString();
                            Clk2Grp2Chnls += " : ";
                        }
                        Clk2Grp2Chnls = Clk2Grp2Chnls.Substring(0, Clk2Grp2Chnls.Length - 2);
                    }
                    else
                        Clk2Grp2Chnls = "-";
                }
                else
                    Clk2Grp2Chnls = "-";
                row5.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row5.Cells[1], Clk2Grp2Chnls));
                row5.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                #endregion
                #region Clk2 Grp3
                Row row6 = tbl1.AddRow();
                row6.Format.Alignment = ParagraphAlignment.Center;
                row6.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row6.Cells[0], config.Clk2Grp3Text + " Channels"));
                row6.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row6.Cells[0].Format.Font.Bold = true;
                row6.Cells[0].Shading.Color = Colors.SteelBlue;
                string Clk2Grp3Chnls = "";
                if (config.SelectedCLK2_GRP3 != null)
                {
                    if (config.SelectedCLK2_GRP3.Count != 0)
                    {
                        foreach (var chn in config.SelectedCLK2_GRP3)
                        {
                            Clk2Grp3Chnls += (chn == DataModule.eChannles.None) ? "-" : chn.ToString();
                            Clk2Grp3Chnls += " : ";
                        }
                        Clk2Grp3Chnls = Clk2Grp3Chnls.Substring(0, Clk2Grp3Chnls.Length - 2);
                    }
                    else
                        Clk2Grp3Chnls = "-";
                }
                else
                    Clk2Grp3Chnls = "-";
                row6.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row6.Cells[1], Clk2Grp3Chnls));
                row6.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                #endregion
            }

            Row row7 = tbl1.AddRow();
            row7.Format.Alignment = ParagraphAlignment.Center;
            row7.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row7.Cells[0], "Sample Rate"));
            row7.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row7.Cells[0].Format.Font.Bold = true;
            row7.Cells[0].Shading.Color = Colors.SteelBlue;
            string sampleRate = "";
            if (config.ConfigurationMode == DataModule.eConfigMode.LA_Mode)
            {
                if (config.GeneralPurposeMode == DataModule.eGeneralPurpose.Timing)
                    sampleRate = GetSampleRate(config.SampleRateLAPA);
                else
                    sampleRate = "-";
            }
            else if (config.ConfigurationMode == DataModule.eConfigMode.Both)
                sampleRate = GetSampleRate(config.SampleRateLAPA);
            row7.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row7.Cells[1], sampleRate));
            row7.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            //if (config.HasTwoClockSources)
            //    tbl1.SetEdge(0, 1, 2, 9, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
            //else
            //    tbl1.SetEdge(0, 1, 2, 5, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        }

        private string GetVolt(DataModule.eVoltage volt)
        {
            string voltStr = "";
            switch (volt)
            {
                case DataModule.eVoltage._1_0V:
                    voltStr = "1.0V";
                    break;
                case DataModule.eVoltage._1_2V:
                    voltStr = "1.2V";
                    break;
                case DataModule.eVoltage._1_8V:
                    voltStr = "1.8V";
                    break;
                case DataModule.eVoltage._2_5V:
                    voltStr = "2.5V";
                    break;
                case DataModule.eVoltage._3_3V:
                    voltStr = "3.3V";
                    break;
                case DataModule.eVoltage._5_0V:
                    voltStr = "5.0V";
                    break;
                default:
                    voltStr = "-";
                    break;
            }
            return voltStr;
        }

        private string GetSampleRate(DataModule.eSampleRate sampleRate)
        {
            string sample = "";
            switch (sampleRate)
            {
                case DataModule.eSampleRate.SR_125:
                    sample = "125 MS/s";
                    break;
                case DataModule.eSampleRate.SR_142:
                    sample = "142 MS/s";
                    break;
                case DataModule.eSampleRate.SR_166:
                    sample = "166 MS/s";
                    break;
                case DataModule.eSampleRate.SR_200:
                    sample = "200 MS/s";
                    break;
                case DataModule.eSampleRate.SR_250:
                    sample = "250 MS/s";
                    break;
                case DataModule.eSampleRate.SR_333:
                    sample = "333 MS/s";
                    break;
                case DataModule.eSampleRate.SR_500:
                    sample = "500 MS/s";
                    break;
                case DataModule.eSampleRate.SR_1000:
                    sample = "1000 MS/s";
                    break;
                default:
                    sample = "1000 MS/s";
                    break;
            }
            return sample;
        }

        //Section section;

        public void CreatePage(string protocolType)
        {
            // Each MigraDoc document needs at least one section.
            Section section = Doc.AddSection();
            AddHeaderFooter(section);

            // Topic Heading (Report)
            Paragraph para = section.AddParagraph();
            para.AddFormattedText(protocolType + " Protocol List\n\n", TextFormat.Underline);
            para.Format.Font.Size = 12;
            para.Format.Alignment = ParagraphAlignment.Left;

            // Create the item table
            Tbl = section.AddTable();
            Tbl.Style = "Table";
            Tbl.Borders.Visible = true;
            Tbl.Format.Font.Name = "Calibri";
            Tbl.Format.Font.Size = 8;
            Tbl.Borders.Color = Colors.Black;
            Tbl.Borders.Width = 0.25;
            Tbl.Borders.Left.Width = 0.5;
            Tbl.Borders.Right.Width = 0.5;
            Tbl.Rows.LeftIndent = -35;
            Tbl.LeftPadding = 0;
            Tbl.RightPadding = 0;
            Tbl.TopPadding = 0;
            Tbl.BottomPadding = 0;

            tm = new TextMeasurement(Tbl.Format.Font);

            if (protocolType == "I2C")
            {
                Column column = Tbl.AddColumn("1.1cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("2.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("4.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("2.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.7cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("2.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                // Create the header of the table
                Row row = Tbl.AddRow();
                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Center;
                row.Format.Font.Bold = true;
                row.Shading.Color = Colors.LightBlue;
                row.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "S No"));
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[0].MergeDown = 1;
                row.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[1], "Time"));
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].MergeDown = 1;
                row.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[2], "Addr"));
                row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[2].MergeDown = 1;
                row.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[3], "R/W"));
                row.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[3].MergeDown = 1;
                row.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[4], "Data"));
                row.Cells[4].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[4].MergeDown = 1;
                row.Cells[5].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[5], "Ack/Nack"));
                row.Cells[5].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[5].MergeDown = 1;
                row.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[6], "Frequency"));
                row.Cells[6].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[6].MergeDown = 1;
                row.Cells[7].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[7], "Error"));
                row.Cells[7].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[7].MergeDown = 1;
            }
            else if (protocolType == "SPI")
            {
                Column column = Tbl.AddColumn("1.1cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("2.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("4.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("4.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.7cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.1cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("2.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                Row row = Tbl.AddRow();
                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Center;
                row.Format.Font.Bold = true;
                row.Shading.Color = Colors.LightBlue;
                row.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "S No"));
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[0].MergeDown = 1;
                row.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[1], "Time"));
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].MergeDown = 1;
                row.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[2], "MOSI Data"));
                row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[2].MergeDown = 1;
                row.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[3], "MISO Data"));
                row.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[3].MergeDown = 1;
                row.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[4], "Frequency"));
                row.Cells[4].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[4].MergeDown = 1;
                row.Cells[5].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[5], "Stop"));
                row.Cells[5].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[5].MergeDown = 1;
                row.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[6], "Error"));
                row.Cells[6].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[6].MergeDown = 1;
            }
            else if (protocolType == "UART")
            {
                Column column = Tbl.AddColumn("1.1cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("2.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("4.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("4.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.7cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.1cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("2.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                Row row = Tbl.AddRow();
                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Center;
                row.Format.Font.Bold = true;
                row.Shading.Color = Colors.LightBlue;
                row.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "S No"));
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[0].MergeDown = 1;
                row.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[1], "Time"));
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].MergeDown = 1;
                row.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[2], "Tx Data"));
                row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[2].MergeDown = 1;
                row.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[3], "Rx Data"));
                row.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[3].MergeDown = 1;
                row.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[4], "Frequency"));
                row.Cells[4].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[4].MergeDown = 1;
                row.Cells[5].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[5], "Stop"));
                row.Cells[5].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[5].MergeDown = 1;
                row.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[6], "Error"));
                row.Cells[6].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[6].MergeDown = 1;
            }
            else if (protocolType == "I3C")
            {
                Column column = Tbl.AddColumn("1.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("2.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.8cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.3cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("2.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.1cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("2.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("0.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.5cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.1cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("2.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.5cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                //column = Tbl.AddColumn("1.5cm");
                //column.Format.Alignment = ParagraphAlignment.Center;

                // Create the header of the table
                Row row = Tbl.AddRow();
                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Center;
                row.Format.Font.Bold = true;
                row.Shading.Color = Colors.LightBlue;
                row.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "S No"));
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[0].MergeDown = 1;
                row.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Meesage Type"));
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].MergeDown = 1;
                row.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Time"));
                row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[2].MergeDown = 1;
                row.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Mode"));
                row.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[3].MergeDown = 1;
                row.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Packet Info"));
                row.Cells[4].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[4].MergeDown = 1;
                row.Cells[5].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Start"));
                row.Cells[5].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[5].MergeDown = 1;
                row.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Addr_RW_A"));
                row.Cells[6].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[6].MergeDown = 1;
                row.Cells[7].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "CCC_T"));
                row.Cells[7].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[7].MergeDown = 1;
                row.Cells[8].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Data_T/P"));
                row.Cells[8].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[8].MergeDown = 1;
                row.Cells[9].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Stop"));
                row.Cells[9].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[9].MergeDown = 1;
                row.Cells[10].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Error"));
                row.Cells[10].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[10].MergeDown = 1;
                row.Cells[11].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Freq"));
                row.Cells[11].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[11].MergeDown = 1;

            }
            else if (protocolType == "SPMI")
            {
                Column column = Tbl.AddColumn("1.1cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.6cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("3.3cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.5cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("2.8cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.1cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                // Create the header of the table
                Row row = Tbl.AddRow();
                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Center;
                row.Format.Font.Bold = true;
                row.Shading.Color = Colors.LightBlue;
                row.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "S No"));
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[0].MergeDown = 1;
                row.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[1], "Time"));
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].MergeDown = 1;
                row.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[2], "A Bit"));
                row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[2].MergeDown = 1;
                row.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[3], "Sr Bit"));
                row.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[3].MergeDown = 1;
                row.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[2], "SlaveID"));
                row.Cells[4].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[4].MergeDown = 1;
                row.Cells[5].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[2], "MID"));
                row.Cells[5].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[5].MergeDown = 1;
                row.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[3], "Command"));
                row.Cells[6].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[6].MergeDown = 1;
                row.Cells[7].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[6], "Reg Address"));
                row.Cells[7].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[7].MergeDown = 1;
                row.Cells[8].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[7], "Data"));
                row.Cells[8].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[8].MergeDown = 1;
                row.Cells[9].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[5], "Byte Count"));
                row.Cells[9].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[9].MergeDown = 1;
                row.Cells[10].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[4], "Frequency"));
                row.Cells[10].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[10].MergeDown = 1;
                row.Cells[11].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[8], "Error"));
                row.Cells[11].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[11].MergeDown = 1;
            }
            else if (protocolType == "RFFE")
            {
                Column column = Tbl.AddColumn("1.1cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.8cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.3cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("3.3cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.5cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("2.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.1cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                // Create the header of the table
                Row row = Tbl.AddRow();
                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Center;
                row.Format.Font.Bold = true;
                row.Shading.Color = Colors.LightBlue;
                row.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "S No"));
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[0].MergeDown = 1;
                row.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[1], "Time"));
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].MergeDown = 1;
                row.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[2], "Slave ID"));
                row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[2].MergeDown = 1;
                row.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[3], "Command"));
                row.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[3].MergeDown = 1;
                row.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[6], "Reg Address"));
                row.Cells[4].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[4].MergeDown = 1;
                row.Cells[5].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[7], "Data"));
                row.Cells[5].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[5].MergeDown = 1;
                row.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[5], "Byte Count"));
                row.Cells[6].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[6].MergeDown = 1;
                row.Cells[7].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[4], "Frequency"));
                row.Cells[7].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[7].MergeDown = 1;
                row.Cells[8].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[8], "Error"));
                row.Cells[8].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[8].MergeDown = 1;
            }
            else if (protocolType == "CAN")
            {
                var canconfig = configuration.Config.ProtocolConfigList.OfType<ConfigModel_CAN>().FirstOrDefault();
                Column column = Tbl.AddColumn("1.1cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.7cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.8cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.5cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.5cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("3.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.6cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.6cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.7cm");
                column.Format.Alignment = ParagraphAlignment.Center;


                Row row = Tbl.AddRow();
                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Center;
                row.Format.Font.Bold = true;
                row.Shading.Color = Colors.LightBlue;
                row.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "S No"));
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[0].MergeDown = 1;
                row.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[1], "Time"));
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].MergeDown = 1;
                row.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[2], "FrameType"));
                row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[2].MergeDown = 1;
                row.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[3], "FrameFormat"));
                row.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[3].MergeDown = 1;
                row.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[4], "ID"));
                row.Cells[4].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[4].MergeDown = 1;
                row.Cells[5].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[5], "IDE"));
                row.Cells[5].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[5].MergeDown = 1;
                row.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[6], "DLC"));
                row.Cells[6].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[6].MergeDown = 1;
                row.Cells[7].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[7], "DATA"));
                row.Cells[7].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[7].MergeDown = 1;
                row.Cells[8].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[8], "CRC"));
                row.Cells[8].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[8].MergeDown = 1;
                if (canconfig.BRS)
                {
                    row.Cells[9].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[9], "Freq/BRSFreq"));
                    row.Cells[9].Format.Alignment = ParagraphAlignment.Center;
                    row.Cells[9].MergeDown = 1;

                }
                else

                {
                    row.Cells[9].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[9], "Freq"));
                    row.Cells[9].Format.Alignment = ParagraphAlignment.Center;
                    row.Cells[9].MergeDown = 1;
                }

                row.Cells[10].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[10], "ErrorType"));
                row.Cells[10].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[10].MergeDown = 1;

            }

            //QSPI
            if (protocolType == "QSPI")
            {
                Column column = Tbl.AddColumn("2.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("2.6cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.3cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("2.1cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.1cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("2.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("0.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                Row row = Tbl.AddRow();
                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Center;
                row.Format.Font.Bold = true;
                row.Shading.Color = Colors.LightBlue;
                row.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Frame Type"));
                row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[1].MergeDown = 1;

                row.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Time"));
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[0].MergeDown = 1;

                row.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Packet Info"));
                row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[2].MergeDown = 1;

                row.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "CCC"));
                row.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[3].MergeDown = 1;

                row.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Address"));
                row.Cells[4].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[4].MergeDown = 1;

                row.Cells[5].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Data"));
                row.Cells[5].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[5].MergeDown = 1;

                row.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Stop"));
                row.Cells[6].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[6].MergeDown = 1;

                row.Cells[7].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "ErrorType "));
                row.Cells[7].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[7].MergeDown = 1;

                row.Cells[8].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Frequency"));
                row.Cells[8].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[8].MergeDown = 1;

                Tbl.SetEdge(0, 0, 8, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
            }
            if (protocolType == "I2C")
                Tbl.SetEdge(0, 0, 8, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
            else if (protocolType == "I3C" ||protocolType=="SPMI")
                Tbl.SetEdge(0, 0, 12, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
            else if ( protocolType == "RFFE")
                Tbl.SetEdge(0, 0, 9, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
            else if (protocolType == "UART" || protocolType == "SPI")
                Tbl.SetEdge(0, 0, 7, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
            else if (protocolType == "CAN" ) 
                Tbl.SetEdge(0, 0, 11, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
            else 
                Tbl.SetEdge(0, 0, 9, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        }

        //private void CreateTable(ref Table Tbl1, string protocolType)
        //{
        //    // Create the item table
        //    Tbl1 = section.AddTable();
        //    Tbl1.Style = "Table";
        //    Tbl1.Borders.Visible = true;
        //    Tbl1.Format.Font.Name = "Calibri";
        //    Tbl1.Format.Font.Size = 8;
        //    Tbl1.Borders.Color = Colors.Black;
        //    Tbl1.Borders.Width = 0.25;
        //    Tbl1.Borders.Left.Width = 0.5;
        //    Tbl1.Borders.Right.Width = 0.5;
        //    Tbl1.Rows.LeftIndent = 0;
        //    Tbl1.LeftPadding = 0;
        //    Tbl1.RightPadding = 0;
        //    Tbl1.TopPadding = 0;
        //    Tbl1.BottomPadding = 0;

        //    tm = new TextMeasurement(Tbl1.Format.Font);

        //    if (protocolType == "I2C")
        //    {
        //        Column column = Tbl1.AddColumn("1.1cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("3.0cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("2.0cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("2.2cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("2.2cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("2.0cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("2.2cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("2.0cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //    }
        //    else if (protocolType == "SPI")
        //    {
        //        Column column = Tbl1.AddColumn("1.1cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("3.0cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("2.2cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("2.2cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("3.0cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("2.0cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("2.2cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //    }
        //    else if (protocolType == "UART")
        //    {
        //        Column column = Tbl1.AddColumn("1.1cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("3.0cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("2.2cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("2.2cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("3.0cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("2.0cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //        column = Tbl1.AddColumn("2.2cm");
        //        column.Format.Alignment = ParagraphAlignment.Center;
        //    }
        //    //if (protocolType == "I2C")
        //    //    Tbl1.SetEdge(0, 0, 8, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        //    //else
        //    //    Tbl1.SetEdge(0, 0, 7, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        //}

        public void FillContent(string protocolType)
        {
            Tbl.AddRow();

            foreach (var result in results.ProtocolResults)

            {
                if (result.Config.ProtocolType == eProtocol.I2C)
                {
                    var i2cResult = (ResultViewModel_I2C)result;

                    var resultCollection = i2cResult.WareHouse.ResultCollection;
                    if (!(resultCollection.Count() > 0))
                    {
                        MessageBox.Show("No I2C collection found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        return;
                    }
                    int index = 1;
                    for (int i = StartIndexReportI2C; i <= EndIndexReportI2C; i++)
                    {
                        Row row1 = new Row();
                        //if (i % 1000 == 0 && i!=0)
                        //{
                        //    this.Tbl.SetEdge(0, 0, 8, 1000, Edge.Box, BorderStyle.Single, 0.75);
                        //    Tbl = new Table();
                        //    CreateTable(ref Tbl, protocolType);
                        //    Tbl.AddRow();
                        //}
                        row1 = Tbl.AddRow();
                        row1.HeadingFormat = false;
                        row1.Format.Alignment = ParagraphAlignment.Center;

                        //  row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], index.ToString()));


                        if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.I2C)
                        {
                            if (index == i2cResult.WareHouse.TriggerPacket + 1)
                            {
                                //string s= row1.Cells[0].ToString();
                                // s.Replace(s, "");

                                row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], "T    " + index.ToString()));
                                row1.Cells[0].Format.Font.Bold = true;
                                row1.Cells[0].Shading.Color = Colors.Yellow;
                            }



                            else
                            {
                                row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], index.ToString()));

                            }
                        }
                        else
                            row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], index.ToString()));
                        row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], CUtilities.ToEngineeringNotation(resultCollection[i].StartTime - i2cResult.WareHouse.TriggerTime).ToString() + "s"));
                        row1.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[2], "0x" + (resultCollection[i].AddressFirst >> 1).ToString("X2")));
                        string RW = "";
                        if ((resultCollection[i].AddressFirst & 0x1) == 0)
                            RW = "WR";
                        else
                            RW = "RD";
                        row1.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[3], RW));
                        string tempDataStr = "";
                        foreach (var dat in resultCollection[i].DataBytes)
                        {
                            tempDataStr += "0x";
                            tempDataStr += dat.ToString("X2");
                            tempDataStr += " ";
                        }
                        row1.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[4], tempDataStr));
                        string ackString = "";
                        if ((resultCollection[i].AckAddr & 0x0F) == 0x0F)
                            ackString = "Nack";
                        else
                            ackString = "Ack";
                        row1.Cells[5].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[5], ackString));
                        row1.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[6], CUtilities.FormatNumber(resultCollection[i].Frequency, Units.BASE_UNIT) + "Hz"));
                        string error = "";
                        if (resultCollection[i].ErrorType == DataModule.eErrorType.None)
                            error = "-";
                        else
                            error = resultCollection[i].ErrorType.ToString();
                        row1.Cells[7].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[7], error));
                        index++;
                    }
                    this.Tbl.SetEdge(0, 0, 8, (EndIndexReportI2C - StartIndexReportI2C + 1), Edge.Box, BorderStyle.Single, 0.75);
                    Tbl.Borders.Visible = true;
                }
                else if (result.Config.ProtocolType == eProtocol.SPI)
                {
                    var spiResult = (ResultViewModel_SPI)result;
                    var resultCollection = spiResult.SPIFrameCollection;
                    if (!(resultCollection.Count() > 0))
                    {
                        MessageBox.Show("No SPI collection found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        return;
                    }
                    int index = 1;
                    for (int i = StartIndexReportSPI; i <= EndIndexReportSPI; i++)
                    {
                        Row row1 = new Row();
                        //if (i % 1000 == 0 && i != 0)
                        //{
                        //    this.Tbl.SetEdge(0, 0, 7, 1000, Edge.Box, BorderStyle.Single, 0.75);
                        //    Tbl = new Table();
                        //    CreateTable(ref Tbl, protocolType);
                        //    Tbl.AddRow();
                        //}
                        row1 = Tbl.AddRow();
                        row1.HeadingFormat = false;
                        row1.Format.Alignment = ParagraphAlignment.Center;

                        //row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], index.ToString()));

                        if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.SPI)
                        {
                            // TriggerPacket = SPIResultHolder.GetInstance().TriggerPacket;
                            if (index == spiResult.WareHouse.TriggerPacket + 1)
                            {
                                row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], "T    " + index.ToString()));
                                row1.Cells[0].Format.Font.Bold = true;
                                row1.Cells[0].Shading.Color = Colors.Yellow;
                            }



                            else
                            {
                                row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], index.ToString()));

                            }
                        }
                        else
                            row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], index.ToString()));
                        row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], CUtilities.ToEngineeringNotation(resultCollection[i].StartTime - spiResult.WareHouse.TriggerTime).ToString() + "s"));
                        string mosiDataStr = "", misoDataStr = "";
                        if (resultCollection[i].MOSIDataBytes.Count == 0)
                            row1.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[2], "-"));
                        else
                        {
                            foreach (var mosiDat in resultCollection[i].MOSIDataBytes)
                            {
                                mosiDataStr += "0x";
                                mosiDataStr += mosiDat.ToString("X2");
                                mosiDataStr += " ";
                            }
                            row1.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[2], mosiDataStr));
                        }

                        if (resultCollection[i].MISODataBytes.Count == 0)
                            row1.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[3], "-"));
                        else
                        {
                            foreach (var misoDat in resultCollection[i].MISODataBytes)
                            {
                                misoDataStr += "0x";
                                misoDataStr += misoDat.ToString("X2");
                                misoDataStr += " ";
                            }
                            row1.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[3], misoDataStr));
                        }

                        row1.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[4], CUtilities.FormatNumber(resultCollection[i].Frequency, Units.BASE_UNIT) + "Hz"));
                        row1.Cells[5].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[5], resultCollection[i].HasStop.ToString()));
                        string error = "";
                        if (resultCollection[i].ErrorType == DataModule.eErrorType.None)
                            error = "-";
                        else
                            error = resultCollection[i].ErrorType.ToString();
                        row1.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[6], error));
                        index++;
                    }
                    this.Tbl.SetEdge(0, 0, 7, (EndIndexReportSPI - StartIndexReportSPI + 1), Edge.Box, BorderStyle.Single, 0.75);
                    Tbl.Borders.Visible = true;
                }
                else if (result.Config.ProtocolType == eProtocol.UART)
                {
                    var uartResult = (ResultViewModel_UART)result;

                    var resultCollection = uartResult.WareHouse.UARTFrameCollection;
                    if (!(resultCollection.Count() > 0))
                    {
                        MessageBox.Show("No UART collection found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        return;
                    }
                    int index = 1;
                    for (int i = StartIndexReportUART; i <= EndIndexReportUART; i++)
                    {
                        Row row1 = new Row();
                        //if (i % 1000 == 0 && i != 0)
                        //{
                        //    this.Tbl.SetEdge(0, 0, 7, 1000, Edge.Box, BorderStyle.Single, 0.75);
                        //    Tbl = new Table();
                        //    CreateTable(ref Tbl, protocolType);
                        //    Tbl.AddRow();
                        //}
                        row1 = Tbl.AddRow();
                        row1.HeadingFormat = false;
                        row1.Format.Alignment = ParagraphAlignment.Center;

                        // row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], index.ToString()));

                        if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.UART)
                        {
                            // TriggerPacket = UARTResultHolder.GetInstance().TriggerPacket;
                            if (index == uartResult.WareHouse.TriggerPacket + 1)
                            {
                                row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], "T    " + index.ToString()));
                                row1.Cells[0].Format.Font.Bold = true;
                                row1.Cells[0].Shading.Color = Colors.Yellow;
                            }



                            else
                            {
                                row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], index.ToString()));

                            }
                        }
                        else
                            row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], index.ToString()));
                        row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], CUtilities.ToEngineeringNotation(resultCollection[i].StartTime - uartResult.WareHouse.TriggerTime).ToString() + "s"));
                        row1.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[2], ((resultCollection[i].TXDdataBytes == -1) ? "-" : ("0x" + resultCollection[i].TXDdataBytes.ToString("X2")))));
                        row1.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[2], ((resultCollection[i].RXdataBytes == -1) ? "-" : ("0x" + resultCollection[i].RXdataBytes.ToString("X2")))));

                        row1.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[4], CUtilities.FormatNumber(resultCollection[i].Frequency, Units.BASE_UNIT) + "Hz"));
                        row1.Cells[5].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[5], resultCollection[i].HasStop.ToString()));
                        string error = "";
                        if (resultCollection[i].ErrorType == DataModule.eErrorType.None)
                            error = "-";
                        else
                            error = resultCollection[i].ErrorType.ToString();
                        row1.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[6], error));
                        index++;
                    }
                    this.Tbl.SetEdge(0, 0, 7, (EndIndexReportUART - StartIndexReportUART + 1), Edge.Box, BorderStyle.Single, 0.75);
                    Tbl.Borders.Visible = true;
                }
                else if (result.Config.ProtocolType == eProtocol.SPMI)
                {
                    var spmiResult = (ResultViewModel_SPMI)result;

                    var resultCollection = spmiResult.WareHouse.FrameCollection;
                    if (!(resultCollection.Count() > 0))
                    {
                        MessageBox.Show("No SPMI collection found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        return;
                    }
                    int index = 1;
                    for (int i = StartIndexReportSPMI; i <= EndIndexReportSPMI; i++)
                    {
                        Row row1 = new Row();
                        row1 = Tbl.AddRow();
                        row1.HeadingFormat = false;
                        row1.Format.Alignment = ParagraphAlignment.Center;

                        //  row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], index.ToString()));
                        if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.SPMI)
                        {
                            if (index == spmiResult.WareHouse.TriggerPacket + 1)
                            {
                                row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], "T    " + index.ToString()));
                                row1.Cells[0].Format.Font.Bold = true;
                                row1.Cells[0].Shading.Color = Colors.Yellow;
                            }



                            else
                            {
                                row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], index.ToString()));

                            }
                        }
                        else
                            row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], index.ToString()));
                        row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], CUtilities.ToEngineeringNotation(resultCollection[i].StartTime - spmiResult.WareHouse.TriggerTime).ToString() + "s"));
                        row1.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[2], resultCollection[i].HasA_bit.ToString()));
                        row1.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[3], resultCollection[i].HasSr_bit.ToString()));
                        row1.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[4], (resultCollection[i].SlaveIdBind == -1 ? "-" : "0x" + resultCollection[i].SlaveIdBind.ToString("X2"))));
                        row1.Cells[5].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[5], (resultCollection[i].MIDBind == -1 ? "-" : "0x" + resultCollection[i].MIDBind.ToString("X2"))));
                        row1.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[6], resultCollection[i].Command.CmdType.ToString()));
                        row1.Cells[7].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[7], ("0x" + resultCollection[i].IntAddress.ToString("X"))));
                        row1.Cells[8].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[8], resultCollection[i].Data == null ? "" : String.Join(" ", resultCollection[i].Data.Select(obj => String.Format("0x{0:X}", obj.Value)))));
                        row1.Cells[9].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[9], resultCollection[i].ByteCount == -1 ? "-" : ("0x" + resultCollection[i].ByteCount.ToString("X"))));
                        row1.Cells[10].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[10], CUtilities.FormatNumber(resultCollection[i].Frequency, Units.BASE_UNIT) + "Hz"));
                        string error = "";
                        //if (resultCollection[i].ErrorType == DataModule.eErrorType.None)
                        //    error = "-";
                        //else
                            error = resultCollection[i].SPMIErrorType.Count > 0 ? "Error" : "Pass";
                        row1.Cells[11].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[11], error));
                        index++;
                    }
                    this.Tbl.SetEdge(0, 0, 12, (EndIndexReportSPMI - StartIndexReportSPMI + 1), Edge.Box, BorderStyle.Single, 0.75);
                    Tbl.Borders.Visible = true;
                }
                else if (result.Config.ProtocolType == eProtocol.RFFE)
                {
                    var rffeResult = (ResultViewModel_RFFE)result;

                    var resultCollection = rffeResult.WareHouse.FrameCollection;
                    if (!(resultCollection.Count() > 0))
                    {
                        MessageBox.Show("No RFFE collection found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        return;
                    }

                    int index = 1;
                    for (int i = StartIndexReportRFFE; i <= EndIndexReportRFFE; i++)
                    {
                        Row row1 = new Row();
                        row1 = Tbl.AddRow();
                        row1.HeadingFormat = false;
                        row1.Format.Alignment = ParagraphAlignment.Center;

                        //  row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], index.ToString()));
                        if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.RFFE)
                        {

                            if (index == rffeResult.WareHouse.TriggerPacket + 1)
                            {
                                row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], "T    " + index.ToString()));
                                row1.Cells[0].Format.Font.Bold = true;
                                row1.Cells[0].Shading.Color = Colors.Yellow;
                            }



                            else
                            {
                                row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], index.ToString()));

                            }
                        }
                        else
                            row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], index.ToString()));
                        row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], CUtilities.ToEngineeringNotation(resultCollection[i].StartTime - rffeResult.WareHouse.TriggerTime).ToString() + "s"));
                        row1.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[2], ("0x" + resultCollection[i].SlaveId.ToString("X2"))));
                        row1.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[3], resultCollection[i].Command.CmdType.ToString()));
                        row1.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[4], ("0x" + resultCollection[i].IntAddress.ToString("X"))));
                        row1.Cells[5].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[5], resultCollection[i].Data == null ? "" : String.Join(" ", resultCollection[i].Data.Select(obj => String.Format("0x{0:X}", obj.Value)))));
                        row1.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[6], resultCollection[i].ByteCount == -1 ? "-" : ("0x" + resultCollection[i].ByteCount.ToString("X"))));
                        row1.Cells[7].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[7], CUtilities.FormatNumber(resultCollection[i].Frequency, Units.BASE_UNIT) + "Hz"));
                        string error = "";
                        if (resultCollection[i].ErrorType == DataModule.eErrorType.None)
                            error = "-";
                        else
                            error = resultCollection[i].ErrorType.ToString();
                        row1.Cells[8].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[8], error));
                        index++;
                    }
                    this.Tbl.SetEdge(0, 0, 9, (EndIndexReportRFFE - StartIndexReportRFFE + 1), Edge.Box, BorderStyle.Single, 0.75);
                    Tbl.Borders.Visible = true;
                }
                else if (result.Config.ProtocolType == eProtocol.I3C)
                {

                    var I3c_Result = result as ResultViewModel_I3C;


                    //TODO:update code here
                    var resultCollection = I3c_Result.ResultCollection;//ResultModel_I3C.GetInstance().FrameCollection;
                    if (!(resultCollection.Count > 0))
                    {
                        MessageBox.Show("No I3C collection found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        return;
                    }
                    int index = 1;
                    int n = 0;
                    // Tbl.AddRow();
                    for (int i = StartIndexReportI3C; i <= EndIndexReportI3C; i++)
                    {
                        n++;
                        for (int j = 0; j < resultCollection[i].PacketCollection.Count(); j++)
                        {
                            Row row1 = new Row();
                            row1 = Tbl.AddRow();
                            row1.HeadingFormat = false;
                            row1.Format.Alignment = ParagraphAlignment.Center;
                            if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.I3C)
                            {

                            }
                            else
                                row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], index.ToString()));

                            if (j == 0)
                            {
                                row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], resultCollection[i].FrameType.ToString())); //frame type

                            }
                            else
                            {
                                row1.Cells[1].AddParagraph("-");
                            }
                            row1.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[2], CUtilities.ToEngineeringNotation(resultCollection[i].PacketCollection[j].TimeStamp - SessionConfiguration.TriggerTime)) + "s"); //time stamp
                            row1.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[3], resultCollection[i].PacketCollection[j].ProtocolMode.ToString())); //mode
                            if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.Command && resultCollection[i].PacketCollection[j] is CommandMessageModel)
                                row1.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[4], ProtocolInfoRepository.GetCommand(((CommandMessageModel)resultCollection[i].PacketCollection[j]).Value).ToString())); //packet info
                            else
                                row1.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[4], resultCollection[i].PacketCollection[j].PacketType.ToString())); //packet info
                            if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.Address)
                            {
                                row1.Cells[5].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[5], (resultCollection[i].PacketCollection[j] as AddressMessageModel).Start.ToString())); //start
                            }
                            else
                            {
                                row1.Cells[5].AddParagraph("-");
                            }
                            if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.Address)
                            {
                                row1.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[6], "0x" + (resultCollection[i].PacketCollection[j] as AddressMessageModel).Value.ToString("X2") + " " + (resultCollection[i].PacketCollection[j] as AddressMessageModel).TransferType.ToString().Substring(0, 1) + " " + (resultCollection[i].PacketCollection[j] as AddressMessageModel).AckType.ToString().Substring(0, 1))); //address  
                            }
                            else
                            {
                                row1.Cells[6].AddParagraph("-");
                            }

                            if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.Command)
                            {
                                row1.Cells[7].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[7], "0x" + (resultCollection[i].PacketCollection[j] as CommandMessageModel).Value.ToString("X2") + " " + (resultCollection[i].PacketCollection[j] as CommandMessageModel).ParityBit.ToString())); //ccc  
                            }
                            else
                            {
                                if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.HDR_Command)
                                {
                                    row1.Cells[7].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[7], (resultCollection[i].PacketCollection[j] as HDRMessageModel).Value.ToString("X5") + " " + (resultCollection[i].PacketCollection[j] as HDRMessageModel).ParityBit.ToString())); //hdr ccc
                                }
                                else
                                {
                                    row1.Cells[7].AddParagraph("-");
                                }
                            }

                            if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.Data)
                            {
                                if (resultCollection[i].PacketCollection[j] is DataMessageModel)
                                {
                                    row1.Cells[8].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[8], "0x" + (resultCollection[i].PacketCollection[j] as DataMessageModel).Value.ToString("X2") + " " + (resultCollection[i].PacketCollection[j] as DataMessageModel).TransmitBit.ToString())); //data
                                }
                                else if (resultCollection[i].PacketCollection[j] is PIDDAAMessageModel)
                                {
                                    row1.Cells[8].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[8], "0x" + (resultCollection[i].PacketCollection[j] as PIDDAAMessageModel).Value.ToString("X2") + " " + (resultCollection[i].PacketCollection[j] as PIDDAAMessageModel).TransmitBit.ToString())); //data
                                }
                                else
                                {
                                    row1.Cells[8].AddParagraph("-");
                                }
                            }
                            else
                            {
                                if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.HDR_Data)
                                {
                                    row1.Cells[8].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[8], "0x" + (resultCollection[i].PacketCollection[j] as HDRMessageModel).Value.ToString("X5") + " " + (resultCollection[i].PacketCollection[j] as HDRMessageModel).ParityBit.ToString())); //hdr data
                                }
                                else if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.HDR_CRC)
                                {
                                    row1.Cells[8].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[8], "0x" + (resultCollection[i].PacketCollection[j] as HDRMessageModel).Value.ToString("X5"))); //hdr crc
                                }
                                else
                                {
                                    row1.Cells[8].AddParagraph("-");
                                }
                            }
                            if (resultCollection[i].PacketCollection[j].Stop == false)
                            {
                                row1.Cells[9].AddParagraph("-");
                            }
                            else
                            {
                                row1.Cells[9].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[9], resultCollection[i].PacketCollection[j].Stop.ToString())); //stop
                            }
                            if (resultCollection[i].PacketCollection[j].ErrorType == eErrorType.None)
                            {
                                row1.Cells[10].AddParagraph("-");
                            }
                            else
                            {
                                row1.Cells[10].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[10], resultCollection[i].PacketCollection[j].ErrorType.ToString())); //error
                            }
                            double strvalue = System.Convert.ToDouble(resultCollection[i].PacketCollection[j].Frequency);

                            row1.Cells[11].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[11], strvalue == 0 ? "-" : CUtilities.FormatNumber(strvalue, Units.BASE_UNIT) + "Hz")); //frequency
                            index++;
                        }
                    }
                    this.Tbl.SetEdge(0, 0, 12, (EndIndexReportI3C - StartIndexReportI3C + 1), Edge.Box, BorderStyle.Single, 0.75);
                    Tbl.Borders.Visible = true;
                }
                else if (result.Config.ProtocolType == eProtocol.CAN)
                {
                    var canResult = result as ResultViewModel_CAN;

                    var resultCollection = canResult.ResultCollection;
                    if (!(resultCollection.Count() > 0))
                    {
                        MessageBox.Show("No CAN collection found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        return;
                    }
                    int index = 1;
                    for (int i = StartIndexReportCAN; i <= EndIndexReportCAN; i++)
                    {
                        Row row1 = new Row();
                        //if (i % 1000 == 0 && i != 0)
                        //{
                        //    this.Tbl.SetEdge(0, 0, 7, 1000, Edge.Box, BorderStyle.Single, 0.75);
                        //    Tbl = new Table();
                        //    CreateTable(ref Tbl, protocolType);
                        //    Tbl.AddRow();
                        //}
                        row1 = Tbl.AddRow();
                        row1.HeadingFormat = false;
                        row1.Format.Alignment = ParagraphAlignment.Center;


                        if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.CAN)
                        {
                            // TriggerPacket = UARTResultHolder.GetInstance().TriggerPacket;
                            if (index == canResult.WareHouse.TriggerPacket + 1)
                            {
                                row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], "T    " + index.ToString()));
                                row1.Cells[0].Format.Font.Bold = true;
                                row1.Cells[0].Shading.Color = Colors.Yellow;
                            }



                            else
                            {
                                row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], index.ToString()));

                            }
                        }
                        else
                            row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], index.ToString()));
                        row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], CUtilities.ToEngineeringNotation(resultCollection[i].StartTime - canResult.WareHouse.TriggerTime).ToString() + "s"));
                        row1.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[2], resultCollection[i].FrameType.ToString()));
                        row1.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[3], resultCollection[i].Eframeformates.ToString()));
                        row1.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[4], ((resultCollection[i].IDDdataBytes == -1) ? "-" : ("0x" + resultCollection[i].IDDdataBytes.ToString("X2")))));
                        row1.Cells[5].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[5], ((resultCollection[i].IDEDdataBytes == -1) ? "-" : ("0x" + resultCollection[i].IDEDdataBytes.ToString("X2")))));
                        row1.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[6], ((resultCollection[i].DLCDdataBytes == -1) ? "-" : ("0x" + resultCollection[i].DLCDdataBytes.ToString("X2")))));
                        var canconfig = configuration.Config.ProtocolConfigList.OfType<ConfigModel_CAN>().FirstOrDefault();
                        if (canconfig.BRS)
                        {
                            row1.Cells[9].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[9], CUtilities.FormatNumber(resultCollection[i].Frequency, Units.BASE_UNIT).Replace("M", "") + "/" + CUtilities.FormatNumber(resultCollection[i].BRSFrequency, Units.BASE_UNIT) + "Hz"));

                        }

                        else
                        {
                            row1.Cells[9].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[9], CUtilities.FormatNumber(resultCollection[i].Frequency, Units.BASE_UNIT) + "Hz"));
                        }

                        //row1.Cells[9].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[9], CUtilities.FormatNumber(resultCollection[i].Frequency, Units.BASE_UNIT) + "Hz"));
                        row1.Cells[10].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[10], resultCollection[i].ErrorType.ToString()));
                        string tempDataStr = "";
                        foreach (var dat in resultCollection[i].DataBytes)
                        {
                            tempDataStr += "0x";
                            tempDataStr += dat.ToString("X2");
                            tempDataStr += " ";
                        }
                        row1.Cells[7].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[7], ((resultCollection[i].DLCDdataBytes == 0) ? "-" : tempDataStr)));
                        row1.Cells[8].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[8], ((resultCollection[i].CRCDdataBytes == -1) ? "-" : ("0x" + resultCollection[i].CRCDdataBytes.ToString("X2")))));
                        //string error = "";
                        //if (resultCollection[i].ErrorType == DataModule.eErrorType.None)
                        //    error = "-";
                        //else
                        //    error = resultCollection[i].ErrorType.ToString();
                        //row1.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[6], error));
                        index++;
                    }
                    this.Tbl.SetEdge(0, 0, 11, (endIndexReportCAN - startIndexReportCAN + 1), Edge.Box, BorderStyle.Single, 0.75);
                    Tbl.Borders.Visible = true;
                }

                else if (protocolType == "QSPI")
                {

                    var QSPI_Result = result as ResultViewModel_QSPI;


                    //TODO:update code here
                    var resultCollection = QSPI_Result.QSPIFrameCollection;//ResultModel_I3C.GetInstance().FrameCollection;
                    if (!(resultCollection.Count > 0))
                    {
                        MessageBox.Show("No QSPI collection found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        return;
                    }

                    ////var resultCollection = QSPIResultHolder.GetInstance().QSPIFrameCollection;
                    //ObservableCollection<QSPIFrame> resultCollection;
                    //if (!(resultCollection > 0))
                    //{
                    //    MessageBox.Show("No QSPI collection found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    //    return;
                    //}

                    int index = 1;
                    int n = 0;
                    //Tbl.AddRow();

                    for (int i = StartIndexReportQSPI; i <= EndIndexReportQSPI; i++)
                    {
                        n++;
                        for (int j = 0; j < resultCollection[i].PacketCollection.Count(); j++)
                        {
                            Row row1 = new Row();
                            row1 = Tbl.AddRow();
                            row1.HeadingFormat = false;
                            row1.Format.Alignment = ParagraphAlignment.Center;
                            if (j == 0)
                            {
                                row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], resultCollection[i].QSPICommandType.ToString())); //frame type
                            }
                            else
                            {
                                row1.Cells[1].AddParagraph("-");
                            }
                            row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], CUtilities.ToEngineeringNotation(resultCollection[i].PacketCollection[j].TimeStamp)) + "s"); //time stamp

                            row1.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[2], resultCollection[i].PacketCollection[j].PacketType.ToString())); //packet info

                            if (j == 0)


                                if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.Command)
                                {
                                    row1.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[3], "0x" + (resultCollection[i].PacketCollection[j] as CommandMessageModel).PacketValue.ToString("X2"))); //ccc  
                                }
                                else
                                {
                                    row1.Cells[3].AddParagraph("-");
                                }

                            if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.Address)
                            {
                                row1.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[4], "0x" + (resultCollection[i].PacketCollection[j] as AddressMessageModel).PacketValue.ToString("X2"))); //address  
                            }
                            else
                            {
                                row1.Cells[4].AddParagraph("-");
                            }

                            if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.Data)
                            {
                                if (resultCollection[i].PacketCollection[j] is DataMessageModel)
                                {
                                    row1.Cells[5].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[5], "0x" + (resultCollection[i].PacketCollection[j] as DataMessageModel).PacketValue.ToString("X2"))); //data
                                }
                                else
                                {
                                    row1.Cells[5].AddParagraph("-");
                                }
                            }
                            else
                                row1.Cells[5].AddParagraph("-");


                            if (resultCollection[i].PacketCollection[j].Stop == false)
                            {
                                row1.Cells[6].AddParagraph("-");
                            }
                            else
                            {
                                row1.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[7], resultCollection[i].PacketCollection[j].Stop.ToString())); //stop
                            }
                            if (resultCollection[i].PacketCollection[j].ErrorType == eErrorType.None)
                            {
                                row1.Cells[7].AddParagraph("-");
                            }
                            else
                            {
                                row1.Cells[7].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[7], resultCollection[i].PacketCollection[j].ErrorType.ToString())); //error
                            }
                            double strvalue = System.Convert.ToDouble(resultCollection[i].PacketCollection[j].Frequency);
                            row1.Cells[8].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[8], strvalue == 0 ? "-" : CUtilities.FormatNumber(strvalue, Units.BASE_UNIT) + "Hz")); //frequency
                        }
                    }
                    this.Tbl.SetEdge(0, 0, 8, (EndIndexReportQSPI - StartIndexReportQSPI + 1), Edge.Box, BorderStyle.Single, 0.75);

                    Tbl.Borders.Visible = true;
                }
            }
        }
        public void CreatePageLA()
        {
            Section section = Doc.AddSection();
            AddHeaderFooter(section);

            // Topic Heading (Report)
            Paragraph para = section.AddParagraph();
            para.AddFormattedText("Waveform List\n\n", TextFormat.Underline);
            para.Format.Font.Size = 9;
            para.Format.Alignment = ParagraphAlignment.Left;

            // Create the item table
            Tbl = section.AddTable();
            Tbl.Style = "Table";
            Tbl.Borders.Visible = true;
            Tbl.Format.Font.Name = "Calibri";
            Tbl.Format.Font.Size = 8;
            Tbl.Borders.Color = Colors.Black;
            Tbl.Borders.Width = 0.25;
            Tbl.Borders.Left.Width = 0.5;
            Tbl.Borders.Right.Width = 0.5;
            Tbl.Rows.LeftIndent = 0;
            Tbl.LeftPadding = 0;
            Tbl.RightPadding = 0;
            Tbl.TopPadding = 0;
            Tbl.BottomPadding = 0;

            tm = new TextMeasurement(Tbl.Format.Font);

            ConfigModel config = configuration.Config;

            Column column = Tbl.AddColumn("1.1cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column = Tbl.AddColumn("2.0cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            if (config.IsCh16Visible)
            {
                column = Tbl.AddColumn("0.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh15Visible)
            {
                column = Tbl.AddColumn("0.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh14Visible)
            {
                column = Tbl.AddColumn("0.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh13Visible)
            {
                column = Tbl.AddColumn("0.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh12Visible)
            {
                column = Tbl.AddColumn("0.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh11Visible)
            {
                column = Tbl.AddColumn("0.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh10Visible)
            {
                column = Tbl.AddColumn("0.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh9Visible)
            {
                column = Tbl.AddColumn("0.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh8Visible)
            {
                column = Tbl.AddColumn("0.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh7Visible)
            {
                column = Tbl.AddColumn("0.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh6Visible)
            {
                column = Tbl.AddColumn("0.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh5Visible)
            {
                column = Tbl.AddColumn("0.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh4Visible)
            {
                column = Tbl.AddColumn("0.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh3Visible)
            {
                column = Tbl.AddColumn("0.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh2Visible)
            {
                column = Tbl.AddColumn("0.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh1Visible)
            {
                column = Tbl.AddColumn("0.9cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }

            Row row = Tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = Colors.LightBlue;

            WaveformListingViewModel WVM = waveformListing;

            int i = 0;
            row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], "S No"));
            row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[i].MergeDown = 1;
            i++;
            row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], "Time"));
            row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[i].MergeDown = 1;
            i++;
            if (config.IsCh16Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.WfmList.Ch16Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh15Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.WfmList.Ch15Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh14Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.WfmList.Ch14Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh13Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.WfmList.Ch13Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh12Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.WfmList.Ch12Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh11Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.WfmList.Ch11Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh10Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.WfmList.Ch10Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh9Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.WfmList.Ch9Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh8Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.WfmList.Ch8Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh7Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.WfmList.Ch7Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh6Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.WfmList.Ch6Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh5Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.WfmList.Ch5Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh4Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.WfmList.Ch4Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh3Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.WfmList.Ch3Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh2Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.WfmList.Ch2Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh1Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.WfmList.Ch1Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            Tbl.SetEdge(0, 0, i, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        }

        public void FillContentLA()
        {
            var resultCollection = waveformListing.WfmHolder.ASyncCollection;
            if (!(resultCollection.Count() > 0))
            {
                MessageBox.Show("No collection found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            if ((startIndex == 0 && stopIndex == 0) || (stopIndex < startIndex) ||
                (startIndex < 0) || (startIndex > (waveformListing.WfmHolder.ASyncCollection.Count - 1)) ||
                (stopIndex < 0) || (stopIndex > (waveformListing.WfmHolder.ASyncCollection.Count - 1)) || !SelRange || IsStandard)
            {
                startIndex = 0;
                stopIndex = waveformListing.WfmHolder.ASyncCollection.Count() - 1;
            }
            if ((stopIndex - startIndex) > 499) // limitaions in no. of frames.
                stopIndex = startIndex + 499;
            List<DiscreteWaveForm> resul = Ioc.Default.GetService<IDataProvider>().GetWaveform(startIndex, stopIndex + 1);
            int index = 1;
            Tbl.AddRow();
            ConfigModel config = configuration.Config;
            int j = 0;
            for (int i = 0; i <= (stopIndex - startIndex); i++)
            {
                Row row1 = new Row();
                row1 = Tbl.AddRow();
                row1.HeadingFormat = false;
                row1.Format.Alignment = ParagraphAlignment.Center;
                j = 0;
                row1.Cells[j].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[j], index.ToString()));
                j++;
                row1.Cells[j].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[j], CUtilities.ToEngineeringNotation(resul[i].TimeStamp - SessionConfiguration.TriggerTime).ToString() + "s"));
                j++;
                if (config.IsCh16Visible)
                {
                    row1.Cells[j].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[j], GetWaveformChannelByte(resul[i].ChannelValue, "Ch16").ToString()));
                    j++;
                }
                if (config.IsCh15Visible)
                {
                    row1.Cells[j].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[j], GetWaveformChannelByte(resul[i].ChannelValue, "Ch15").ToString()));
                    j++;
                }
                if (config.IsCh14Visible)
                {
                    row1.Cells[j].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[j], GetWaveformChannelByte(resul[i].ChannelValue, "Ch14").ToString()));
                    j++;
                }
                if (config.IsCh13Visible)
                {
                    row1.Cells[j].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[j], GetWaveformChannelByte(resul[i].ChannelValue, "Ch13").ToString()));
                    j++;
                }
                if (config.IsCh12Visible)
                {
                    row1.Cells[j].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[j], GetWaveformChannelByte(resul[i].ChannelValue, "Ch12").ToString()));
                    j++;
                }
                if (config.IsCh11Visible)
                {
                    row1.Cells[j].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[j], GetWaveformChannelByte(resul[i].ChannelValue, "Ch11").ToString()));
                    j++;
                }
                if (config.IsCh10Visible)
                {
                    row1.Cells[j].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[j], GetWaveformChannelByte(resul[i].ChannelValue, "Ch10").ToString()));
                    j++;
                }
                if (config.IsCh9Visible)
                {
                    row1.Cells[j].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[j], GetWaveformChannelByte(resul[i].ChannelValue, "Ch9").ToString()));
                    j++;
                }
                if (config.IsCh8Visible)
                {
                    row1.Cells[j].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[j], GetWaveformChannelByte(resul[i].ChannelValue, "Ch8").ToString()));
                    j++;
                }
                if (config.IsCh7Visible)
                {
                    row1.Cells[j].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[j], GetWaveformChannelByte(resul[i].ChannelValue, "Ch7").ToString()));
                    j++;
                }
                if (config.IsCh6Visible)
                {
                    row1.Cells[j].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[j], GetWaveformChannelByte(resul[i].ChannelValue, "Ch6").ToString()));
                    j++;
                }
                if (config.IsCh5Visible)
                {
                    row1.Cells[j].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[j], GetWaveformChannelByte(resul[i].ChannelValue, "Ch5").ToString()));
                    j++;
                }
                if (config.IsCh4Visible)
                {
                    row1.Cells[j].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[j], GetWaveformChannelByte(resul[i].ChannelValue, "Ch4").ToString()));
                    j++;
                }
                if (config.IsCh3Visible)
                {
                    row1.Cells[j].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[j], GetWaveformChannelByte(resul[i].ChannelValue, "Ch3").ToString()));
                    j++;
                }
                if (config.IsCh2Visible)
                {
                    row1.Cells[j].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[j], GetWaveformChannelByte(resul[i].ChannelValue, "Ch2").ToString()));
                    j++;
                }
                if (config.IsCh1Visible)
                {
                    row1.Cells[j].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[j], GetWaveformChannelByte(resul[i].ChannelValue, "Ch1").ToString()));
                    j++;
                }
                index++;
            }
            this.Tbl.SetEdge(0, 0, j, (stopIndex - startIndex), Edge.Box, BorderStyle.Single, 0.75);
            Tbl.Borders.Visible = true;
        }

        private int GetWaveformChannelByte(UInt16 value, string parameter)
        {
            int retVal = 0;
            UInt16 val = (UInt16)value;
            switch (parameter.ToString())
            {
                case "Ch10":
                    retVal = ((val & 0x200) >> 9) == 1 ? 1 : 0;
                    break;
                case "Ch9":
                    retVal = ((val & 0x100) >> 8) == 1 ? 1 : 0;
                    break;
                case "Ch8":
                    retVal = ((val & 0x80) >> 7) == 1 ? 1 : 0;
                    break;
                case "Ch7":
                    retVal = ((val & 0x40) >> 6) == 1 ? 1 : 0;
                    break;
                case "Ch6":
                    retVal = ((val & 0x20) >> 5) == 1 ? 1 : 0;
                    break;
                case "Ch5":
                    retVal = ((val & 0x10) >> 4) == 1 ? 1 : 0;
                    break;
                case "Ch4":
                    retVal = ((val & 0x8) >> 3) == 1 ? 1 : 0;
                    break;
                case "Ch3":
                    retVal = ((val & 0x4) >> 2) == 1 ? 1 : 0;
                    break;
                case "Ch2":
                    retVal = ((val & 0x2) >> 1) == 1 ? 1 : 0;
                    break;
                case "Ch1":
                    retVal = (val & 0x1) == 1 ? 1 : 0;
                    break;
                case "Ch11":
                    retVal = ((val & 0x400) >> 10) == 1 ? 1 : 0;
                    break;
                case "Ch12":
                    retVal = ((val & 0x800) >> 11) == 1 ? 1 : 0;
                    break;
                case "Ch13":
                    retVal = ((val & 0x1000) >> 12) == 1 ? 1 : 0;
                    break;
                case "Ch14":
                    retVal = ((val & 0x2000) >> 13) == 1 ? 1 : 0;
                    break;
                case "Ch15":
                    retVal = ((val & 0x4000) >> 14) == 1 ? 1 : 0;
                    break;
                case "Ch16":
                    retVal = ((val & 0x8000) >> 15) == 1 ? 1 : 0;
                    break;
                default:
                    retVal = 0;
                    break;
            }
            return retVal;
        }
        public void AddRemarks()
        {
            Section section = Doc.AddSection();
            Paragraph para = section.AddParagraph();
            para.AddFormattedText("Remarks", new Font("Calibri", 16));
            para.Format.Font.Underline = MigraDoc.DocumentObjectModel.Underline.Single;
            para.Format.Alignment = ParagraphAlignment.Left;

            section.AddParagraph("\n");

            para = section.AddParagraph();
            para.AddFormattedText(Remarks, new Font("Calibri", 12));
            para.Format.Alignment = ParagraphAlignment.Left;
        }
        public void AddHeaderFooter(Section section)
        {
            Paragraph paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddText("Page ");
            paragraph.AddPageField();
            paragraph.Format.Font.Size = 9;
            paragraph.Format.Alignment = ParagraphAlignment.Right;
        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Create, FileAccess.Write, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        private string AdjustIfTooWideToFitIn(Cell cell, string text)
        {
            Column column = cell.Column;
            Unit availableWidth = column.Width - column.Table.Borders.Width - cell.Borders.Width;

            var tooWideWords = text.Split(" ".ToCharArray()).Distinct().Where(s => TooWide(s, availableWidth));

            var adjusted = new StringBuilder(text);
            foreach (string word in tooWideWords)
            {
                var replacementWord = MakeFit(word, availableWidth);
                adjusted.Replace(word, replacementWord);
            }

            return adjusted.ToString();
        }

        private bool TooWide(string word, Unit width)
        {
            float f = tm.MeasureString(word, UnitType.Point).Width;
            return f > width.Point;
        }

        private string MakeFit(string word, Unit width)
        {
            var adjustedWord = new StringBuilder();
            var current = string.Empty;
            foreach (char c in word)
            {
                if (TooWide(current + c, width))
                {
                    adjustedWord.Append(current);
                    adjustedWord.Append(Chars.CR);
                    current = c.ToString();
                }
                else
                {
                    current += c;
                }
            }
            adjustedWord.Append(current);

            return adjustedWord.ToString();
        }

        ICommand runDialogCommand;
        public ICommand RunDialogCommand
        {
            get
            {
                if (runDialogCommand == null)
                    runDialogCommand = new RelayCommand(ExecuteRunDialog, CanexecuteReport);
                return runDialogCommand;
            }
        }

        ICommand generateReportCommand;
        public ICommand GenerateReportCommand
        {
            get
            {
                //return generateReportCommand ?? (generateReportCommand = new CommandHandler(() => GenerateReportDialog(), () => CanExecute));
                return new RelayCommand<string>(GenerateReportDialog);
            }
        }

        ICommand generateReportCsvCommand;
        public ICommand GenerateReportCsvCommand
        {
            get
            {
                return generateReportCsvCommand ?? (generateReportCsvCommand = new CommandHandler(() => GenerateReportCsvDialog(), () => CanExecute));
            }
        }


        private string reportPath = MiniCooperDirectoryInfo.ReportPath;
        public string ReportPath
        {
            get
            {
                return reportPath;
            }
            set
            {
                reportPath = value;
                RaisePropertyChanged("ReportPath");
            }
        }

        private ICommand generateReportHtmCommand;
        public ICommand GenerateReportHtmCommand
        {
            get
            {
                if (generateReportHtmCommand == null)
                    generateReportHtmCommand = new RelayCommand(GenerateHTM);
                return generateReportHtmCommand;
            }
        }


        StringBuilder sb = new StringBuilder();

        public void GenerateHTM(object obj)
        {
            //sb = new StringBuilder();
            try
            {
                Task.Factory.StartNew(() =>
                {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "htm file (*.htm)|*.htm";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        path = saveFileDialog.FileName;
                        sb = new StringBuilder();
                        sb.Clear();
                        sb.AppendLine("<html>");
                        sb.AppendLine("<head>");
                        var assembly = Assembly.GetExecutingAssembly();
                        string cssResource = string.Empty;
                        string[] resNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                        foreach (string resName in resNames)
                        {
                            if (resName.Contains("Report.css"))
                            {
                                cssResource = resName;
                                break;
                            }
                        }
                        if (cssResource != string.Empty)
                        {
                            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(cssResource))

                            using (var reader = new StreamReader(stream))
                            {
                                sb.AppendLine("<style>");
                                while (!reader.EndOfStream)
                                {
                                    sb.AppendLine(reader.ReadLine());
                                }
                                sb.AppendLine("</style >");
                            }

                            sb.AppendLine("</head>");
                            Complete();
                            sb.AppendLine("</html>");
                            if (!ReportPath.Contains(".htm"))
                            {
                                ReportStatusUpdated?.Invoke("Generating Report");
                                string str2 = DateTime.Now.ToString("ddMMyyyyHHmmss");
                                //string filename = MiniCooperDirectoryInfo.AppPath + @"\Report" + @"\Protocol" + str2 + ".htm";
                                
                                ReportPath = path;
                                File.WriteAllText(ReportPath, sb.ToString());
                                ReportStatusUpdated?.Invoke("Report Generated");
                                MessageBox.Show("Report Generated successfully.. ", MiniCooperDirectoryInfo.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Information);
                                Process.Start(ReportPath);
                            }
                            else
                            {
                                File.WriteAllText(ReportPath, sb.ToString());
                                MessageBox.Show("Report Generated successfully.. ", MiniCooperDirectoryInfo.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Information);
                                Process.Start(ReportPath);
                            }
                        }
                    }
                });
            }
            catch (Exception e)
            {
                //  ApplicationDebugManager.Instance.WriteToLog("ReportModel_GenerateHTM", e);
            }
        }


        public void Complete()
        {
            sb.AppendLine("<div class='align'>");
            Header();
            FirstTableHtm();
            ConfigurationTable();
            //MeasurementTableMht();
            FillMeasurementTableHtm();
            //    CapturedImages();
            //    sb.AppendLine("</div>");
        }


        public void FillMeasurementTableHtm()
        {
            try
            {




                foreach (var result in results.ProtocolResults)

                {
                    int k = 1;
                    if (result.Config.ProtocolType == eProtocol.I3C)
                    {
                        sb.AppendLine("<h2><u> " + result.Config.Name.ToString() + "_Protocol List </b></u></h2>");
                        sb.AppendLine("<table class='MeasurementTable'>");
                        sb.AppendLine("<tr><th>Sl.No</th><th>Message Type</th><th>Time</th><th>Mode</th><th>Packet Info</th><th>Start</th><th>Address_R_W_A</th><th>CCC_T</th><th>Data_T/P</th><th>Stop</th><th>Error</th><th>Frequency</th></tr>");

                        var I3c_Result = result as ResultViewModel_I3C;



                        //TODO:update code here
                        var resultCollection = I3c_Result.ResultCollection;//ResultModel_I3C.GetInstance().FrameCollection;
                        StartIndexReportI3C = 0;
                        EndIndexReportI3C = resultCollection.Count - 1;
                        //if (!(resultCollection.Count > 0))
                        //{
                        //    MessageBox.Show("No I3C collection found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        //    return;
                        //}
                        int index = 1;
                        int n = 0;
                        // Tbl.AddRow();
                        for (int i = StartIndexReportI3C; i <= EndIndexReportI3C; i++)
                        {
                            n++;
                            for (int j = 0; j < resultCollection[i].PacketCollection.Count(); j++)
                            {
                               

                                if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.I3C)
                                {
                                }
                                else
                                {
                                    sb.AppendLine("<td>" + index.ToString() + "</td>");
                                }
                                if (j == 0)
                                {
                                    sb.AppendLine("<td>" + resultCollection[i].FrameType.ToString() + "</td>");
                                }
                                else
                                {
                                    sb.AppendLine("<td>" + "-" + "</td>");
                                }
                                sb.AppendLine("<td>" + CUtilities.ToEngineeringNotation(resultCollection[i].PacketCollection[j].TimeStamp - SessionConfiguration.TriggerTime) + "s" + "</td>");

                                //time stamp
                                sb.AppendLine("<td>" + resultCollection[i].PacketCollection[j].ProtocolMode.ToString() + "</td>");

                                if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.Command && resultCollection[i].PacketCollection[j] is CommandMessageModel)
                                {
                                    sb.AppendLine("<td>" + ProtocolInfoRepository.GetCommand(((CommandMessageModel)resultCollection[i].PacketCollection[j]).Value).ToString() + "</td>");
                                }//packet info
                                else
                                {
                                    sb.AppendLine("<td>" + resultCollection[i].PacketCollection[j].PacketType.ToString() + "</td>");
                                }
                                if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.Address)
                                {
                                    sb.AppendLine("<td>" + (resultCollection[i].PacketCollection[j] as AddressMessageModel).Start.ToString() + "</td>");
                                }
                                else
                                {
                                    sb.AppendLine("<td>" + "-" + "</td>");
                                }
                                if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.Address)
                                {
                                    sb.AppendLine("<td>" + "0x" + (resultCollection[i].PacketCollection[j] as AddressMessageModel).Value.ToString("X2") + " " + (resultCollection[i].PacketCollection[j] as AddressMessageModel).TransferType.ToString().Substring(0, 1) + " " + (resultCollection[i].PacketCollection[j] as AddressMessageModel).AckType.ToString().Substring(0, 1) + "</td>");
                                }
                                else
                                {
                                    sb.AppendLine("<td>" + "-" + "</td>");
                                }

                                if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.Command)
                                {
                                    sb.AppendLine("<td>" + "0x" + (resultCollection[i].PacketCollection[j] as CommandMessageModel).Value.ToString("X2") + " " + (resultCollection[i].PacketCollection[j] as CommandMessageModel).ParityBit.ToString() + "</td>");
                                }
                                else
                                {
                                    if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.HDR_Command)
                                    {
                                        sb.AppendLine("<td>" + (resultCollection[i].PacketCollection[j] as HDRMessageModel).Value.ToString("X5") + " " + (resultCollection[i].PacketCollection[j] as HDRMessageModel).ParityBit.ToString() + "</td>");
                                    }
                                    else
                                    {
                                        sb.AppendLine("<td>" + "-" + "</td>");
                                    }
                                }

                                if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.Data)
                                {
                                    if (resultCollection[i].PacketCollection[j] is DataMessageModel)
                                    {
                                        sb.AppendLine("<td>" + "0x" + (resultCollection[i].PacketCollection[j] as DataMessageModel).Value.ToString("X2") + " " + (resultCollection[i].PacketCollection[j] as DataMessageModel).TransmitBit.ToString() + "</td>");
                                    }
                                    else if (resultCollection[i].PacketCollection[j] is PIDDAAMessageModel)
                                    {
                                        sb.AppendLine("<td>" + "0x" + (resultCollection[i].PacketCollection[j] as PIDDAAMessageModel).Value.ToString("X2") + " " + (resultCollection[i].PacketCollection[j] as PIDDAAMessageModel).TransmitBit.ToString() + "</td>");
                                    }
                                    else
                                    {
                                        sb.AppendLine("<td>" + "-" + "</td>");
                                    }
                                }
                                else
                                {
                                    if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.HDR_Data)
                                    {
                                        sb.AppendLine("<td>" + "0x" + (resultCollection[i].PacketCollection[j] as HDRMessageModel).Value.ToString("X5") + " " + (resultCollection[i].PacketCollection[j] as HDRMessageModel).ParityBit.ToString() + "</td>");
                                    }
                                    else if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.HDR_CRC)
                                    {
                                        sb.AppendLine("<td>" + "0x" + (resultCollection[i].PacketCollection[j] as HDRMessageModel).Value.ToString("X5") + "</td>");
                                    }
                                    else
                                    {
                                        sb.AppendLine("<td>" + "-" + "</td>");
                                    }
                                }
                                if (resultCollection[i].PacketCollection[j].Stop == false)
                                {
                                    sb.AppendLine("<td>" + "-" + "</td>");
                                }
                                else
                                {
                                    sb.AppendLine("<td>" + resultCollection[i].PacketCollection[j].Stop.ToString() + "</td>");
                                }
                                if (resultCollection[i].PacketCollection[j].ErrorType == eErrorType.None)
                                {
                                    sb.AppendLine("<td>" + "-" + "</td>");
                                }
                                else
                                {
                                    sb.AppendLine("<td>" + resultCollection[i].PacketCollection[j].ErrorType.ToString() + "</td>");
                                }
                                double strvalue = System.Convert.ToDouble(resultCollection[i].PacketCollection[j].Frequency);

                                sb.AppendLine("<td>" + (strvalue == 0 ? "-" : CUtilities.FormatNumber(strvalue, Units.BASE_UNIT) + "Hz").ToString() + "</td>");
                                sb.AppendLine("</tr>");

                                index++;
                            }
                        }
                        sb.AppendLine("</table>");
                    }




                    if (result.Config.ProtocolType == eProtocol.I2C)
                    {
                        sb.AppendLine("<h2><u> " + result.Config.Name.ToString() + "_Protocol List </b></u></h2>");
                        sb.AppendLine("<table class='MeasurementTable'>");
                        sb.AppendLine("<tr><th>S.No</th><th>Time</th><th>Frame</th><th>S/Sr</th><th>Address</th><th>Read/Write</th><th>A/N</th><th>Data</th>" +
                            "<th>Frequency</th><th>Error</th></tr>");

                        var I2C_result = result as ResultViewModel_I2C;
                        var resultI2C = I2C_result.FrameCollection;

                        string datStr = "";

                        string ackString = "";

                        int index = 1;
                        foreach (var frame in resultI2C.OfType<I2CFrame>())
                        {

                            sb.AppendLine("<td>" + index + "</td>");
                            sb.AppendLine("<td>" + CUtilities.ToEngineeringNotation(frame.StartTime - SessionConfiguration.TriggerTime) + "s" + "</td>");
                            sb.Append("<td>" + "I2C" + "</td>");
                            sb.Append("<td>" + frame.Start.ToString() + "</td>");
                            sb.Append("<td>" + "0x" + (frame.AddressFirst >> 1).ToString("X2") + "</td>");
                            var val = (frame.AddressFirst & 0x01) == 0 ? "WR" : "RD";
                            sb.Append("<td>" + val + "</td>");
                            if ((frame.AckAddr & 0x0F) == 0x0F)
                                ackString = "Nack";
                            else
                                ackString = "Ack";
                            sb.Append("<td>" + ackString + "</td>");
                            datStr = "";
                            foreach (var dat in frame.DataBytes)
                            {
                                datStr += "0x" + dat.ToString("X2");
                                datStr += " : ";
                            }
                            if (datStr == "")
                                datStr = "-";
                            else
                                datStr = datStr.Substring(0, datStr.Length - 2);
                            sb.Append("<td>" + datStr + "</td>");
                            sb.Append("<td>" + CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz" + "</td>");
                            sb.Append("<td>" + frame.ErrorType.ToString() + "</td>");

                            sb.AppendLine("</tr>");

                            index++;

                        }

                        sb.AppendLine("</table>");
                    }

                    if (result.Config.ProtocolType == eProtocol.SPI)
                    {
                        sb.AppendLine("<h2><u> " + result.Config.Name.ToString() + "_Protocol List </u></h2>");
                        sb.AppendLine("<table class='MeasurementTable'>");
                        sb.AppendLine("<tr><th>S.No</th><th>Time</th><th>Frame</th><th>MOSI</th><th>MISO</th><th>Frequency</th><th>Stop</th><th>Error</th></tr>");

                        string datStr = "";
                        var SPI_result = result as ResultViewModel_SPI;
                        var resultSPI = SPI_result.SPIFrameCollection;
                        string mOSIStr = "";
                        string mISOStr = "";

                        int index = 1;
                        foreach (var frame in resultSPI.OfType<SPIFrame>())
                        {

                            sb.AppendLine("<td>" + index + "</td>");
                            sb.AppendLine("<td>" + CUtilities.ToEngineeringNotation(frame.StartTime - SessionConfiguration.TriggerTime) + "s" + "</td>");
                            sb.Append("<td>" + "SPI" + "</td>");
                            mOSIStr = "";
                            foreach (var dat in frame.MOSIDataBytes)
                            {
                                mOSIStr += "0x" + dat.ToString("X2");
                                mOSIStr += " : ";
                            }
                            if (mOSIStr == "")
                                mOSIStr = "-";
                            else
                                mOSIStr = mOSIStr.Substring(0, mOSIStr.Length - 2);
                            sb.Append("<td>" + mOSIStr + "</td>");
                            mISOStr = "";
                            foreach (var dat in frame.MISODataBytes)
                            {
                                mISOStr += "0x" + dat.ToString("X2");
                                mISOStr += " : ";
                            }
                            if (mISOStr == "")
                                mISOStr = "-";
                            else
                                mISOStr = mISOStr.Substring(0, mISOStr.Length - 2);
                            sb.Append("<td>" + mISOStr + "</td>");
                            sb.Append("<td>" + CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz" + "</td>");
                            sb.Append("<td>" + frame.HasStop.ToString() + "</td>");
                            sb.Append("<td>" + frame.ErrorType.ToString() + "</td>");
                            sb.AppendLine("</tr>");

                            index++;

                        }

                        sb.AppendLine("</table>");
                    }


                    if (result.Config.ProtocolType == eProtocol.UART)
                    {
                        sb.AppendLine("<h2><u>" + result.Config.Name.ToString() + "_Protocol List </u></h2>");
                        sb.AppendLine("<table class='MeasurementTable'>");
                        sb.AppendLine("<tr><th>S.No</th><th>Time</th><th>Frame</th><th>TX Data</th><th>RX Data</th><th>Frequency</th><th>Stop</th><th>Error</th></tr>");

                        var UART_result = result as ResultViewModel_UART;
                        var resultUART = UART_result.WareHouse.UARTFrameCollection;

                        int index = 1;
                        foreach (var frame in resultUART.OfType<UARTFrame>())
                        {
                            sb.AppendLine("<td>" + index + "</td>");
                            sb.AppendLine("<td>" + CUtilities.ToEngineeringNotation(frame.StartTime - SessionConfiguration.TriggerTime) + "s" + "</td>");
                            sb.Append("<td>" + "UART" + "</td>");
                            sb.Append("<td>" + ((frame.TXDdataBytes == -1) ? "-" : ("0x" + frame.TXDdataBytes.ToString("X2"))) + "</td>");
                            sb.Append("<td>" + ((frame.RXdataBytes == -1) ? "-" : ("0x" + frame.RXdataBytes.ToString("X2"))) + "</td>");
                            sb.Append("<td>" + CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz" + "</td>");
                            sb.Append("<td>" + frame.HasStop.ToString() + "</td>");
                            sb.Append("<td>" + frame.ErrorType.ToString() + "</td>");
                            sb.AppendLine("</tr>");

                            index++;
                        }

                        sb.AppendLine("</table>");

                    }

                    if(result.Config.ProtocolType==eProtocol.QSPI)
                    {


                        sb.AppendLine("<h2><u> " + result.Config.Name.ToString() + "_Protocol List </b></u></h2>");
                        sb.AppendLine("<table class='MeasurementTable'>");
                        sb.AppendLine("<tr><th>Sl.No</th><th>Frame Type</th><th>Time</th><th>Packet Info</th><th>Command</th><th>Address</th><th>Data</th><th>Stop</th><th>Error</th><th>Frequency</th></tr>");

                        var QSPI_Result = result as ResultViewModel_QSPI;



                        //TODO:update code here
                        var resultCollection = QSPI_Result.QSPIFrameCollection;
                        StartIndexReportQSPI = 0;
                        EndIndexReportQSPI = resultCollection.Count - 1;

                        int index = 1;

                        for (int i = StartIndexReportQSPI; i <= EndIndexReportQSPI; i++)
                        {
                          
                            for (int j = 0; j < resultCollection[i].PacketCollection.Count(); j++)
                            {


                                if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.I3C)
                                {
                                }
                                else
                                {
                                    sb.AppendLine("<td>" + index.ToString() + "</td>");
                                }

                                if (j == 0)
                                {
                                sb.AppendLine("<td>"+(( resultCollection[i].QSPICommandType.ToString()))+"</td>"); //frame type
                                }
                                else
                                {
                                    sb.AppendLine("<td>" + "-" + "</td>");
                                }
                                sb.AppendLine("<td>" + CUtilities.ToEngineeringNotation(resultCollection[i].PacketCollection[j].TimeStamp - SessionConfiguration.TriggerTime) + "s" + "</td>");


                                sb.AppendLine("<td>" + ( resultCollection[i].PacketCollection[j].PacketType.ToString())+"</td>"); //packet info

                                if (j == 0)
                                {

                                    if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.Command)
                                    {
                                        sb.AppendLine("<td>" + ("0x" + (resultCollection[i].PacketCollection[j] as CommandMessageModel).PacketValue.ToString("X2")) + "</td>"); //ccc  
                                    }
                                }
                                else
                                {
                                    sb.AppendLine("<td>" + "-" + "</td>");
                                }
                                
                                if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.Address)
                                {
                                    sb.AppendLine("<td>" + ("0x" + (resultCollection[i].PacketCollection[j] as AddressMessageModel).PacketValue.ToString("X2")) + "</td>") ; //address  
                                }
                                else
                                {
                                    sb.AppendLine("<td>" + "-" + "</td>");
                                }

                                if (resultCollection[i].PacketCollection[j].PacketType == ePacketType.Data)
                                {
                                    if (resultCollection[i].PacketCollection[j] is DataMessageModel)
                                    {
                                        sb.AppendLine("<td>" + ( "0x" + (resultCollection[i].PacketCollection[j] as DataMessageModel).PacketValue.ToString("X2"))+"</td>"); //data
                                    }
                                    else
                                    {
                                        sb.AppendLine("<td>" + "-" + "</td>");
                                    }
                                }
                                else
                                    sb.AppendLine("<td>" + "-" + "</td>");


                                if (resultCollection[i].PacketCollection[j].Stop == false)
                                {
                                    sb.AppendLine("<td>" + "-" + "</td>");
                                }
                                else
                                {
                                    sb.AppendLine("<td>" + ( resultCollection[i].PacketCollection[j].Stop.ToString())+"</td>"); //stop
                                }
                                if (resultCollection[i].PacketCollection[j].ErrorType == eErrorType.None)
                                {
                                    sb.AppendLine("<td>" + "-" + "</td>");
                                }
                                else
                                {
                                    sb.AppendLine("<td>" + (resultCollection[i].PacketCollection[j].ErrorType.ToString()) + "</td>"); //error
                                }
                                double strvalue = System.Convert.ToDouble(resultCollection[i].PacketCollection[j].Frequency);
                                sb.AppendLine("<td>" + (strvalue == 0 ? "-" : CUtilities.FormatNumber(strvalue, Units.BASE_UNIT) + "Hz") + "</td>"); //frequency
                                sb.AppendLine("</tr>");
                                index++;
                            }
                        }
                        sb.AppendLine("</table>");
                    }

                    if(result.Config.ProtocolType==eProtocol.CAN)
                    {

                        sb.AppendLine("<h2><u> " + result.Config.Name.ToString() + "_Protocol List </b></u></h2>");
                        sb.AppendLine("<table class='MeasurementTable'>");
                        sb.AppendLine("<tr><th>S.No</th><th>Time</th><th>FrameType</th><th>FrameFormat</th><th>ID</th><th>IDE</th><th>DLC</th><th>Data</th><th>CRC</th><th>Frequency</th><th>Error</th></tr>");

                        var CAN_result = result as ResultViewModel_CAN;
                        var resultCAN = CAN_result.ResultCollection;

                        string datStr = "";

                        string ackString = "";

                        int index = 1;
                        foreach (var frame in resultCAN.OfType<CANFrame>())
                        {





                            sb.AppendLine("<td>" + index + "</td>");
                            sb.AppendLine("<td>" + CUtilities.ToEngineeringNotation(frame.StartTime - SessionConfiguration.TriggerTime) + "s" + "</td>");
                            sb.AppendLine("<td>" +frame.FrameType.ToString() + "</td>");
                            sb.AppendLine("<td>" + frame.Eframeformates.ToString() + "</td>");
                            sb.AppendLine("<td>"+((frame.IDDdataBytes == -1) ? "-" : ("0x" + frame.IDDdataBytes.ToString("X2")))+"</td>");
                            sb.AppendLine("<td>" + ((frame.IDEDdataBytes == -1) ? "-" : ("0x" + frame.IDEDdataBytes.ToString("X2")))+"</td>");
                            sb.AppendLine("<td>" + ((frame.DLCDdataBytes == -1) ? "-" : ("0x" + frame.DLCDdataBytes.ToString("X2")))+"</td>");

                        

                            //row1.Cells[9].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[9], CUtilities.FormatNumber(resultCollection[i].Frequency, Units.BASE_UNIT) + "Hz"));
                            
                            string tempDataStr = "";
                            foreach (var dat in frame.DataBytes)
                            {
                                tempDataStr += "0x";
                                tempDataStr += dat.ToString("X2");
                                tempDataStr += " ";
                            }
                            sb.AppendLine("<td>" + ((frame.DLCDdataBytes == 0) ? "-" : tempDataStr)+"</td>");
                            sb.AppendLine("<td>" + ((frame.CRCDdataBytes == -1) ? "-" : ("0x" + frame.CRCDdataBytes.ToString("X2")))+"</td>");
                            foreach (var configViewModel in configuration.ProtocolConfiguration)
                            {

                                if (configViewModel is ConfigViewModel_CAN canconfig)
                                {

                                    if (canconfig.BRS)
                                    {

                                        sb.AppendLine("<td>" + CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT).Replace("M", "") + "/" + CUtilities.FormatNumber(frame.BRSFrequency, Units.BASE_UNIT) + "Hz" + "</td>");

                                    }

                                    else
                                    {
                                        sb.AppendLine("<td>" + CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz" + "</td>");
                                    }
                                }
                            }
                            sb.AppendLine("<td>" + frame.ErrorType.ToString() + "</td>");
                            //string error = "";
                            //if (resultCollection[i].ErrorType == DataModule.eErrorType.None)
                            //    error = "-";
                            //else
                            //    error = resultCollection[i].ErrorType.ToString();
                            //row1.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[6], error));
                            sb.AppendLine("</tr>");

                            index++;


                        }
                        sb.AppendLine("</table>");
                    }

                    if (result.Config.ProtocolType == eProtocol.SPMI)
                    {
                        sb.AppendLine("<h2><u> " + result.Config.Name.ToString() + "_Protocol List </b></u></h2>");
                        sb.AppendLine("<table class='MeasurementTable'>");
                        sb.AppendLine("<tr><th>S.No</th><th>Time</th><th>Frame</th><th>A Bit</th><th>Sr Bit</th><th>Slave</th><th>MID</th><th>Command</th><th>Reg/Address</th><th>Data</th><th>ByteCount</th><th>Frequency</th><th>Error</th></tr>");

                        var SPMI_result = result as ResultViewModel_SPMI;
                        var resultSPMI = SPMI_result.SPMIFrameCollection;

                        string datStr = "";

                        string ackString = "";

                        int index = 1;
                        foreach (var frame in resultSPMI.OfType<SPMIFrameStructure>())
                        {

                            sb.AppendLine("<td>" + index + "</td>");
                            sb.AppendLine("<td>" + CUtilities.ToEngineeringNotation(frame.StartTime - SessionConfiguration.TriggerTime) + "s" + "</td>");
                            sb.Append("<td>" + "SPMI" + "</td>");
                            sb.Append("<td>" + frame.HasA_bit.ToString() + "</td>");
                            sb.Append("<td>" + frame.HasSr_bit.ToString() + "</td>");
                            if(frame.SlaveIdBind==-1)
                                sb.Append("<td> - </td>");
                            else
                            sb.Append("<td>" + "0x" + frame.SlaveIdBind.ToString("X2") + "</td>");
                            if (frame.MIDBind == -1)
                                sb.Append("<td> - </td>");
                            else
                                sb.Append("<td>" + "0x" + frame.MIDBind.ToString("X2") + "</td>");
                            sb.Append("<td>" + frame.Command.CmdType.ToString() + "</td>");
                            sb.Append("<td>" + "0x" + frame.IntAddress.ToString("X2") + "</td>");
                            if (frame.Data == null)
                            {
                                sb.Append("<td> - </td>");
                            }
                            else
                            {
                                string datas = string.Empty;
                                foreach (var item in frame.Data)
                                {
                                    datas += "0x"+ item.Value.ToString("X2");
                                    datas += " ";
                                }
                                sb.Append("<td>" + datas  + "</td>");
                            }

                            if (frame.ByteCount == -1)
                            {
                                sb.Append("<td> - </td>");
                            }
                            else
                            {
                                string bytestring = string.Empty;

                                bytestring += "0x" +frame.ByteCount.ToString("X2");
                                
                                sb.Append("<td>" + bytestring + "</td>");
                            }
                          
                            sb.Append("<td>" + CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz" + "</td>");
                            sb.Append("<td>" + (frame.SPMIErrorType.Count > 0 ? "Error" : "Pass" )+ "</td>");

                            sb.AppendLine("</tr>");

                            index++;

                        }

                        sb.AppendLine("</table>");
                    }

                    if (result.Config.ProtocolType == eProtocol.RFFE)
                    {
                        sb.AppendLine("<h2><u> " + result.Config.Name.ToString() + "_Protocol List </b></u></h2>");
                        sb.AppendLine("<table class='MeasurementTable'>");
                        sb.AppendLine("<tr><th>S.No</th><th>Time</th><th>Frame</th><th>Slave/MID</th><th>Command</th><th>Reg/Address</th><th>Data</th><th>ByteCount</th><th>Frequency</th><th>Error</th></tr>");

                        var RFFE_result = result as ResultViewModel_RFFE;
                        var resultRFFE = RFFE_result.RFFEFrameCollection;

                        string datStr = "";

                        string ackString = "";

                        int index = 1;
                        foreach (var frame in resultRFFE.OfType<RFFEFrameStructure>())
                        {

                            sb.AppendLine("<td>" + index + "</td>");
                            sb.AppendLine("<td>" + CUtilities.ToEngineeringNotation(frame.StartTime - SessionConfiguration.TriggerTime) + "s" + "</td>");
                            sb.Append("<td>" + "RFFE" + "</td>");
                            sb.Append("<td>" + "0x" + frame.SlaveId.ToString("X2") + "</td>");
                            sb.Append("<td>" + frame.Command.CmdType.ToString() + "</td>");
                            sb.Append("<td>" + "0x" + frame.IntAddress.ToString("X2") + "</td>");
                            if (frame.Data == null)
                            {
                                sb.Append("<td> - </td>");
                            }
                            else
                            {
                                string datas = string.Empty;
                                foreach (var item in frame.Data)
                                {
                                    datas += "0x" + item.Value.ToString("X2");
                                    datas += " ";
                                }
                                sb.Append("<td>" + datas + "</td>");
                            }

                            if (frame.ByteCount == -1)
                            {
                                sb.Append("<td> - </td>");
                            }
                            else
                            {
                                string bytestring = string.Empty;

                                bytestring += "0x" + frame.ByteCount.ToString("X2");

                                sb.Append("<td>" + bytestring + "</td>");
                            }

                            sb.Append("<td>" + CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz" + "</td>");
                            sb.Append("<td>" + frame.ErrorType.ToString() + "</td>");

                            sb.AppendLine("</tr>");

                            index++;

                        }

                        sb.AppendLine("</table>");
                    }
                }
            }

            catch (Exception e)
            {

            }
        }

        public void MeasurementTableMht()
        {
            //    sb.AppendLine("<h2><u>Measurement List</u></h2>");
            //    sb.AppendLine("<table class='MeasurementTable'>");
            //    sb.AppendLine("<tr><th>Time</th><th>Frame</th><th>Message</th><th>Address</th><th>Command</th><th>Data</th><th>Frequency</th><th>Error</th></tr>");
        }

        public void Header()
        {
            try
            {
                var usCulture = new System.Globalization.CultureInfo("en-US");
                string date = DateTime.Now.ToString(usCulture.DateTimeFormat);
                string logo = string.Empty;
                string[] resNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                foreach (string resName in resNames)
                {
                    if (resName.Contains("Logo.png"))
                    {
                        logo = resName;
                        break;
                    }
                }
                if (logo != string.Empty)
                {
                    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(logo))

                    using (var reader = new StreamReader(stream))
                    {
                        if (stream == null)
                            return;
                        string result = reader.ReadToEnd();
                        if (!Directory.Exists(MiniCooperDirectoryInfo.AppPath + @"\logo\"))
                            Directory.CreateDirectory(MiniCooperDirectoryInfo.AppPath + @"\logo\");
                        if (!File.Exists(MiniCooperDirectoryInfo.AppPath + @"\logo\Logo.png"))
                        {
                            Properties.Resources.Prodigy_Logo.Save(MiniCooperDirectoryInfo.AppPath + @"\logo\Logo.png");
                        }
                        var logoimage = "data:image / jpg; base64," + Convert.ToBase64String(File.ReadAllBytes(MiniCooperDirectoryInfo.AppPath + @"\logo\Logo.png"));
                        sb.AppendLine("<div class='HeaderBox'>" + "<img class='ProdigyLogo' src= " + "'" + logoimage + "'" + " > ");
                    }

                }
                sb.AppendLine("<h3 class='ReportHeader'>" + MiniCooperDirectoryInfo.ApplicationName + " Test Report </h3>");
                sb.AppendLine("</div>");
            }
            catch (Exception e)
            {
                //  ApplicationDebugManager.Instance.WriteToLog("ReportModel_Header", e);
            }
        }

        public void ConfigurationTable()
        {
            ConfigModel config1 = configuration.Config;
            sb.AppendLine("<h2><u>Setup Details :</u></h2>");
            sb.AppendLine("<table class='ConfigurationTable'>");
            sb.AppendLine("<tr><th>Voltage Settings</th></tr>");
            sb.AppendLine("</table>");


            sb.AppendLine("<table class='ConfigurationTable'>");
            sb.AppendLine(" <tr><td><b>Voltage - Ch1:Ch2</b></td><td>" + GetVolt(config1.SignalAmpCh1_2).ToString() + "</td></tr>");
            sb.AppendLine("<tr><td><b>Voltage - Ch3:Ch4</b></td><td>" + GetVolt(config1.SignalAmpCh3_4).ToString() + "</td></tr>");
            sb.AppendLine("<tr><td><b>Voltage - Ch5:Ch6</b></td><td>" + GetVolt(config1.SignalAmpCh5_6).ToString() + "</td></tr>");
            sb.AppendLine("<tr><td><b>Voltage - Ch7:Ch8</b></td><td>" + GetVolt(config1.SignalAmpCh7_8).ToString() + "</td></tr>");
            sb.AppendLine("<tr><td><b>Voltage - Ch9:Ch10</b></td><td>" + GetVolt(config1.SignalAmpCh9_10).ToString() + "</td></tr>");
            sb.AppendLine("<tr><td><b>Voltage - Ch11:Ch12</b></td><td>" + GetVolt(config1.SignalAmpCh11_12).ToString() + "</td></tr>");
            sb.AppendLine("<tr><td><b>Voltage - Ch13:Ch14</b></td><td>" + GetVolt(config1.SignalAmpCh13_14).ToString() + "</td></tr>");
            sb.AppendLine("<tr><td><b>Voltage - Ch15:Ch16</b></td><td>" + GetVolt(config1.SignalAmpCh15_16) + "</td></tr>");


            sb.AppendLine("</table>");

            sb.AppendLine("<table class='ConfigurationTable'>");
            sb.AppendLine("<h2><u>Channel Configuration :</u></h2>");


            foreach (var config in configuration.ProtocolConfiguration)
            {

                if (config is ConfigViewModel_I3C i3CConfig)
                {
                    if (i3CConfig != null)
                    {
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_CLK</b></td><td>" + i3CConfig.ChannelIndex_SCL + "</td></tr>");
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_DATA</b></td><td>" + i3CConfig.ChannelIndex_SDA + "</td></tr>");
                    }

                }


                if (config is ConfigViewModel_I2C i2CConfig)
                {
                    if (i2CConfig != null)
                    {
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_CLK</b></td><td>" + i2CConfig.ChannelIndex_SCL + "</td></tr>");
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_DATA</b></td><td>" + i2CConfig.ChannelIndex_SDA + "</td></tr>");
                    }

                }

                if (config is ConfigViewModel_SPI spiConfig)
                {
                    if (spiConfig != null)
                    {
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_CLK</b></td><td>" + spiConfig.ChannelIndex_CLK + "</td></tr>");
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_CS</b></td><td>" + spiConfig.ChannelIndex_CS + "</td></tr>");
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_MISO</b></td><td>" + spiConfig.ChannelIndex_MISO + "</td></tr>");
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_MOSI</b></td><td>" + spiConfig.ChannelIndex_MOSI + "</td></tr>");
                    }

                }

                if (config is ConfigViewModel_UART uartConfig)
                {
                    if (uartConfig != null)
                    {
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_RX</b></td><td>" + uartConfig.ChannelIndex_RX + "</td></tr>");
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_TX</b></td><td>" + uartConfig.ChannelIndex_TX + "</td></tr>");

                    }

                }



                if (config is ConfigViewModel_QSPI QSPIConfig)
                {
                    if (QSPIConfig != null)
                    {
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_CLK</b></td><td>" + QSPIConfig.ChannelIndex_CLK + "</td></tr>");
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_CS</b></td><td>" + QSPIConfig.ChannelIndex_CS + "</td></tr>");
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_D0</b></td><td>" + QSPIConfig.ChannelIndex_D0 + "</td></tr>");
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_D1</b></td><td>" + QSPIConfig.ChannelIndex_D1 + "</td></tr>");
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_D2</b></td><td>" + QSPIConfig.ChannelIndex_D2 + "</td></tr>");
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_D3</b></td><td>" + QSPIConfig.ChannelIndex_D3 + "</td></tr>");

                    }

                }

                if (config is ConfigViewModel_CAN CANConfig)
                {
                    if (CANConfig != null)
                    {
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_CH</b></td><td>" + CANConfig.ChannelIndex + "</td></tr>");

                    }
                }

                if (config is ConfigViewModel_SPMI spmiConfig)
                {
                    if (spmiConfig != null)
                    {
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_CLK</b></td><td>" + spmiConfig.ChannelIndex_SCL + "</td></tr>");
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_Data</b></td><td>" + spmiConfig.ChannelIndex_SDA + "</td></tr>");
                    }
                }


                if (config is ConfigViewModel_RFFE rffeConfig)
                {
                    if (rffeConfig != null)
                    {
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_CLK</b></td><td>" + rffeConfig.ChannelIndex_SCL + "</td></tr>");
                        sb.AppendLine("<tr><td><b>" + config.Name.ToString() + "_Data</b></td><td>" + rffeConfig.ChannelIndex_SDA + "</td></tr>");
                    }
                }

            }
            sb.AppendLine("</table>");
        }

        public void FirstTableHtm()
        {
            var usCulture = new System.Globalization.CultureInfo("en-US");
            string date = DateTime.Now.ToString(usCulture.DateTimeFormat);
            sb.AppendLine("<h2><u>Project Details</u></h2>");
            sb.AppendLine("<table class='ProjectTable'>");
            sb.AppendLine("<tr><td><pre><b>Organisation Name</b> </td><td>" + ReplaceProperty(organisationName) + "</td></tr>");
            sb.AppendLine("<tr><td><pre><b>Project Name</b></td><td>" + ReplaceProperty(ProjectName) + "</td></tr>");
            sb.AppendLine("<tr><td><pre><b>Test Name</b></td><td>" + ReplaceProperty(TestName) + "</td></tr>");
            sb.AppendLine("<tr><td><pre><b>Description</b></td><td>" + ReplaceProperty(Description) + "</td></tr>");
            sb.AppendLine("<tr><td><pre><b>Prepared By</b></td><td>" + ReplaceProperty(DesignerName) + "</td></tr>");
            sb.AppendLine("<tr><td><pre><b>Date</b></td><td>" + date + "</td></tr>");
            sb.AppendLine("</table>");
        }
        string ReplaceProperty(string Propertyname)
        {
            var returnProperty = Propertyname;
            if (!(Propertyname.Contains("<") && Propertyname.Contains(">")))
                return returnProperty;

            if (Propertyname.Contains("<"))
                returnProperty = returnProperty.Replace("<", "&lt;");

            if (Propertyname.Contains("?"))
                returnProperty = returnProperty.Replace(">", "&gt;");

            return returnProperty;
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            Console.WriteLine("You can intercept the closing event, and cancel here.");
        }

        #region Data Members
        public Document Doc;

        public Table Tbl;

        TextMeasurement tm;
        ObservableCollection<ImageStructure> imageList;
        public ObservableCollection<ImageStructure> ImageList
        {
            get
            {
                return imageList;
            }

            set
            {
                imageList = value;
                RaisePropertyChanged("ImageList");
            }
        }

        private string logoFilePath = "";

        public string LogoFilePath
        {
            get
            {
                return logoFilePath;
            }
            set
            {
                logoFilePath = value;
                RaisePropertyChanged("LogoFilePath");
            }
        }

        private string organisationName = "<Prodigy Technovations>";
        public string OrganisationName
        {
            get
            {
                return organisationName;
            }
            set
            {
                organisationName = value;
                RaisePropertyChanged("OrganisationName");
            }
        }

        private string projectName = "<ProjectName>";
        public string ProjectName
        {
            get
            {
                return projectName;
            }
            set
            {
                projectName = value;
                RaisePropertyChanged("ProjectName");
            }
        }

        private string testName = "<TestName>";
        public string TestName
        {
            get
            {
                return testName;
            }
            set
            {
                testName = value;
                RaisePropertyChanged("TestName");
            }
        }

        private string description = "<Description>";
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                RaisePropertyChanged("Description");
            }
        }

        private string remarks = "";
        public string Remarks
        {
            get
            {
                return remarks;
            }
            set
            {
                remarks = value;
                RaisePropertyChanged("Remarks");
            }
        }

        private string designerName = "<DesignerName>";
        public string DesignerName
        {
            get
            {
                return designerName;
            }
            set
            {
                designerName = value;
                RaisePropertyChanged("DesignerName");
            }
        }

        private bool selRange = false;
        public bool SelRange
        {
            get
            {
                return selRange;
            }
            set
            {
                selRange = value;
                RaisePropertyChanged("SelRange");
            }
        }

        private bool isStandard = false;
        public bool IsStandard
        {
            get
            {
                return isStandard;
            }
            set
            {
                isStandard = value;
                RaisePropertyChanged("IsStandard");
            }
        }

        private bool useInfo = true;
        public bool UseInfo
        {
            get
            {
                return useInfo;
            }
            set
            {
                useInfo = value;
                RaisePropertyChanged("UseInfo");
            }
        }

        private bool isAllSelected = true;
        public bool IsAllSelected
        {
            get
            {
                return isAllSelected;
            }
            set
            {
                isAllSelected = value;
                if (value)
                {
                    IsMultiSelected = true;
                    IsProtocolSelected = true;
                    IsLogicSelected = true;
                    IsTimingSelected = true;
                    IsAnalyticsSelected = true;
                }
                RaisePropertyChanged("IsAllSelected");
            }
        }

        private bool isMultiSelected = true;
        public bool IsMultiSelected
        {
            get
            {
                return isMultiSelected;
            }
            set
            {
                isMultiSelected = value;
                RaisePropertyChanged("IsMultiSelected");
            }
        }

        private bool isProtocolSelected = true;
        public bool IsProtocolSelected
        {
            get
            {
                return isProtocolSelected;
            }
            set
            {
                isProtocolSelected = value;
                RaisePropertyChanged("IsProtocolSelected");
            }
        }

        private bool isLogicSelected = true;
        public bool IsLogicSelected
        {
            get
            {
                return isLogicSelected;
            }
            set
            {
                isLogicSelected = value;
                RaisePropertyChanged("IsLogicSelected");
            }
        }

        private bool isTimingSelected = true;
        public bool IsTimingSelected
        {
            get
            {
                return isTimingSelected;
            }
            set
            {
                isTimingSelected = value;
                RaisePropertyChanged("IsTimingSelected");
            }
        }

        private bool isAnalyticsSelected = true;
        public bool IsAnalyticsSelected
        {
            get
            {
                return isAnalyticsSelected;
            }
            set
            {
                isAnalyticsSelected = value;
                RaisePropertyChanged("IsAnalyticsSelected");
            }
        }

        private int startIndexReportI2C = 0;
        public int StartIndexReportI2C
        {
            get
            {
                return startIndexReportI2C;
            }
            set
            {
                startIndexReportI2C = value;
                RaisePropertyChanged("StartIndexReportI2C");
            }
        }

        private int endIndexReportI2C = 10;
        public int EndIndexReportI2C
        {
            get
            {
                return endIndexReportI2C;
            }
            set
            {
                endIndexReportI2C = value;
                RaisePropertyChanged("EndIndexReportI2C");
            }
        }

        private int startIndexReportSPI = 0;
        public int StartIndexReportSPI
        {
            get
            {
                return startIndexReportSPI;
            }
            set
            {
                startIndexReportSPI = value;
                RaisePropertyChanged("StartIndexReportSPI");
            }
        }

        private int endIndexReportSPI = 10;
        public int EndIndexReportSPI
        {
            get
            {
                return endIndexReportSPI;
            }
            set
            {
                endIndexReportSPI = value;
                RaisePropertyChanged("EndIndexReportSPI");
            }
        }

        private int startIndexReportUART = 0;
        public int StartIndexReportUART
        {
            get
            {
                return startIndexReportUART;
            }
            set
            {
                startIndexReportUART = value;
                RaisePropertyChanged("StartIndexReportUART");
            }
        }

        private int endIndexReportUART = 10;
        public int EndIndexReportUART
        {
            get
            {
                return endIndexReportUART;
            }
            set
            {
                endIndexReportUART = value;
                RaisePropertyChanged("EndIndexReportUART");
            }
        }

        private int startIndexReportSPMI = 0;
        public int StartIndexReportSPMI
        {
            get
            {
                return startIndexReportSPMI;
            }
            set
            {
                startIndexReportSPMI = value;
                RaisePropertyChanged("StartIndexReportSPMI");
            }
        }

        private int endIndexReportSPMI = 10;
        public int EndIndexReportSPMI
        {
            get
            {
                return endIndexReportSPMI;
            }
            set
            {
                endIndexReportSPMI = value;
                RaisePropertyChanged("EndIndexReportSPMI");
            }
        }

        private int startIndexReportRFFE = 0;
        public int StartIndexReportRFFE
        {
            get
            {
                return startIndexReportRFFE;
            }
            set
            {
                startIndexReportRFFE = value;
                RaisePropertyChanged("StartIndexReportRFFE");
            }
        }

        private int endIndexReportRFFE = 10;
        public int EndIndexReportRFFE
        {
            get
            {
                return endIndexReportRFFE;
            }
            set
            {
                endIndexReportRFFE = value;
                RaisePropertyChanged("EndIndexReportRFFE");
            }
        }

        private int startIndexReportI3C = 0;
        public int StartIndexReportI3C
        {
            get
            {
                return startIndexReportI3C;
            }
            set
            {
                startIndexReportI3C = value;
                RaisePropertyChanged("StartIndexReportI3C");
            }
        }

        private int endIndexReportI3C = 10;
        public int EndIndexReportI3C
        {
            get
            {
                return endIndexReportI3C;
            }
            set
            {
                endIndexReportI3C = value;
                RaisePropertyChanged("EndIndexReportI3C");
            }
        }

        private int startIndexReportCAN = 0;
        public int StartIndexReportCAN
        {
            get
            {
                return startIndexReportCAN;
            }
            set
            {
                startIndexReportCAN = value;
                RaisePropertyChanged("StartIndexReportCAN");
            }
        }

        private int endIndexReportCAN = 10;
        public int EndIndexReportCAN
        {
            get
            {
                return endIndexReportCAN;
            }
            set
            {
                endIndexReportCAN = value;
                RaisePropertyChanged("EndIndexReportCAN");
            }
        }




        //QSPI


        private int startIndexReportQSPI = 0;
        public int StartIndexReportQSPI
        {
            get
            {
                return startIndexReportQSPI;
            }
            set
            {
                startIndexReportQSPI = value;
                RaisePropertyChanged("StartIndexReportQSPI");
            }
        }

        private int endIndexReportQSPI = 10;
        public int EndIndexReportQSPI
        {
            get
            {
                return endIndexReportQSPI;
            }
            set
            {
                endIndexReportQSPI = value;
                RaisePropertyChanged("EndIndexReportQSPI");
            }
        }

        private int startIndex = 0;
        public int StartIndex
        {
            get
            {
                return startIndex;
            }
            set
            {
                startIndex = value;
                RaisePropertyChanged("StartIndex");
            }
        }

        private int stopIndex = 10;
        public int StopIndex
        {
            get
            {
                return stopIndex;
            }
            set
            {
                stopIndex = value;
                RaisePropertyChanged("StopIndex");
            }
        }

        private bool isI2CSelected;

        public bool IsI2CSelected
        {
            get
            {
                return isI2CSelected;
            }
            set
            {
                isI2CSelected = value;
                RaisePropertyChanged("IsI2CSelected");
            }
        }

        private bool isSPISelected;

        public bool IsSPISelected
        {
            get
            {
                return isSPISelected;
            }
            set
            {
                isSPISelected = value;
                RaisePropertyChanged("IsSPISelected");
            }
        }

        private bool isUARTSelected;

        public bool IsUARTSelected
        {
            get
            {
                return isUARTSelected;
            }
            set
            {
                isUARTSelected = value;
                RaisePropertyChanged("IsUARTSelected");
            }
        }

        private bool isALLSelected;

        public bool IsALLSelected
        {
            get
            {
                return isALLSelected;
            }
            set
            {
                isALLSelected = value;
                RaisePropertyChanged("IsALLSelected");
            }
        }
        #endregion

    }

    public class CommandHandler : ICommand
    {
        private Action _action;
        private Func<bool> _canExecute;

        /// <summary>
        /// Creates instance of the command handler
        /// </summary>
        /// <param name="action">Action to be executed by the command</param>
        /// <param name="canExecute">A bolean property to containing current permissions to execute the command</param>
        public CommandHandler(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Wires CanExecuteChanged event 
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Forcess checking if execute is allowed
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute.Invoke();
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }

    public class ImageStructure : ViewModelBase
    {
        bool isIncluded;
        string imagePath;
        string description;

        public bool IsIncluded
        {
            get
            {
                return isIncluded;
            }

            set
            {
                isIncluded = value;
                RaisePropertyChanged("IsIncluded");
            }
        }

        public string ImagePath
        {
            get
            {
                return imagePath;
            }

            set
            {
                imagePath = value;
                RaisePropertyChanged("ImagePath");
            }
        }

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
                RaisePropertyChanged("Description");
            }
        }
    }
}
