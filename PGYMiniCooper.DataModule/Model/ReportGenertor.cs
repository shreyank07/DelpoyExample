using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using PGYMiniCooper.DataModule.Structure;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using PGYMiniCooper.DataModule.Structure.I3CStructure;
using PGYMiniCooper.DataModule.Structure.RFFEStructure;
using PGYMiniCooper.DataModule.Structure.SPIStructure;
using PGYMiniCooper.DataModule.Structure.SPMIStructure;
using PGYMiniCooper.DataModule.Structure.UARTStructure;
using ProdigyFramework.ComponentModel;
using ProdigyFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using System.Windows.Media.Imaging;
using ProdigyFramework.Collections;
using System.Reflection;
using PGYMiniCooper.CoreModule.ViewModel;
using PGYMiniCooper.DataModule.Structure.CANStructure;
using PGYMiniCooper.DataModule.Interface;
using System.Configuration;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace PGYMiniCooper.DataModule.Model
{
    class ReportGenerator : ViewModelBase
    {
       // private AnalyzerViewModel analyzerViewModel;
        private readonly ConfigModel config = null;
        private readonly ProtocolActivityModel protocolActivity = null;
        private readonly List<IResultModel> results;
        private readonly WaveformListingModel waveformListing = null;

        ResultModel_I2C I2CResults;
        ResultModel_SPI SPIResults;
        ResultModel_UART UARTResults;
        ResultModel_SPMI SPMIResults;
        ResultModel_RFFE RFFEResults;
        ResultModel_I3C I3CResults;
        ResultModel_CAN CANResults;
        ResultModel_QSPI QSPIResults;

        public ReportGenerator(List<IResultModel> results, ConfigModel configModel, ProtocolActivityModel protocolActivity, WaveformListingModel waveformListing)
        {
            this.results = results;
            this.config = configModel;
            this.protocolActivity = protocolActivity;
            this.waveformListing = waveformListing;
        }

        protected internal eResponseFlag GenerateReportCSV(string path, bool incImage = false, int startIndex = -1, int stopIndex = -1)
        {
            return GenerateCSV(path, startIndex, stopIndex);
        }

        protected internal eResponseFlag GenerateReportPDF(string path, bool incImage = false, int startIndex = -1, int stopIndex = -1)
        {
            return GeneratePDF(path, startIndex, stopIndex);
        }



        #region pdf
        eResponseFlag GeneratePDF(string path, int startIndex = -1, int stopIndex = -1)
        {
            UARTResults = results.OfType<ResultModel_UART>().FirstOrDefault();

            //var resultI2C = ResultModel_I2C.GetInstance();
            //List<I2CFrame> resultcollectioni2c = resultI2C.ResultCollection;
            //var resultSPI = ResultModel_SPI.GetInstance();
            //ObservableCollection<SPIFrame> resultcollectionSPI = resultSPI.SPIFrameCollection;
            //var resultUART = ResultModel_UART.GetInstance();
            //ObservableCollection<UARTFrame> resultcollectionUART = resultUART.UARTFrameCollection;
            //var resultSPMI = ResultModel_SPMI.GetInstance();
            //ObservableCollection<SPMIFrameStructure> resultcollectionSPMI = resultSPMI.FrameCollection;
            //var resultRFFE = ResultModel_RFFE.GetInstance();
            //ObservableCollection<RFFEFrameStructure> resultcollectionRFFE = resultRFFE.FrameCollection;
            //var resultI3C = ResultModel_I3C.GetInstance();
            //List<FramePattern> resultcollectionI3C = resultI3C.FrameCollection;
            //var resultCAN = ResultModel_CAN.GetInstance();
            //ObservableCollection<CANFrame> resultcollectionCAN = resultCAN.CANFrameCollection;


            //if (!Directory.Exists(new FileInfo(path).Directory.FullName))
            //{
            //    return eResponseFlag.Directory_Not_Exist;
            //}


            //if (config.IsI2CSelected || config.IsAllSelected)
            //{
            //    if (resultcollectioni2c.Count() == 0)
            //    {
            //        return eResponseFlag.No_Result;
            //    }
            //}

            //if (config.IsSPISelected || config.IsAllSelected)
            //{
            //    if (resultcollectionSPI.Count() == 0)
            //    {
            //        return eResponseFlag.No_Result;
            //    }
            //}

            //if (config.IsUARTSelected || config.IsAllSelected)
            //{
            //    if (resultcollectionUART.Count() == 0)
            //    {
            //        return eResponseFlag.No_Result;
            //    }
            //}

            //if (config.IsSPMISelected || config.IsAllSelected)
            //{
            //    if (resultcollectionSPMI.Count() == 0)
            //    {
            //        return eResponseFlag.No_Result;
            //    }
            //}
            //if (config.IsRFFESelected || config.IsAllSelected)
            //{
            //    if (resultcollectionRFFE.Count() == 0)
            //    {
            //        return eResponseFlag.No_Result;
            //    }
            //}

            //if (config.IsI3CSelected || config.IsAllSelected)
            //{
            //    if (resultcollectionI3C.Count() == 0)
            //    {
            //        return eResponseFlag.No_Result;
            //    }
            //} 

            if (IsFileLocked(new FileInfo(path)))
            {
                return eResponseFlag.Task_Busy;
            }
            else
            {
                if (startIndex == -1 && stopIndex == -1)
                {
                    startIndex = 0;

                }
                else if (startIndex == 0 && stopIndex == 0)
                {
                    return eResponseFlag.Invalid_Data;
                }
                else if (startIndex <= 0 || stopIndex >= I2CResults.ResultCollection.Count() || startIndex >= stopIndex)
                {
                    return eResponseFlag.Invalid_Data;
                }
                Task.Factory.StartNew(() =>
                {
                   // string prevStatus = analyzerViewModel.s;
                   // ResultModel_I2C.GetInstance().Status = "Generating Report";

                    PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
                    Document document = CreateDocument();
                    pdfRenderer.Document = document;
                    pdfRenderer.RenderDocument();
                    pdfRenderer.PdfDocument.PageLayout = PdfPageLayout.OneColumn;
                    pdfRenderer.PdfDocument.Settings.TrimMargins.Left = 100;
                    pdfRenderer.PdfDocument.Save(new FileStream(path, FileMode.Create));

                  //  ResultModel_I2C.GetInstance().Status = "Report Generated";

                    //System.Windows.MessageBoxResult messageBoxResult = MessageBox.Show("Report Generated.\nDo you want to open the report?", "PGY-LA-EMBD", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
                    //if (messageBoxResult == System.Windows.MessageBoxResult.Yes)
                    //{
                    //    if (File.Exists(path))
                    //    {
                    //        Process.Start(path);
                    //    }
                    //}

                  //  ResultModel_I2C.GetInstance().Status = prevStatus;
                    return eResponseFlag.Success;
                }).Wait();
                return eResponseFlag.Success;


            }

        }

        #region properies
        public Document Doc;
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

        public Table Tbl;

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


        TextMeasurement tm;

        #endregion
        public Document CreateDocument()
        {

            var i2CConfig = config.ProtocolConfigList.OfType<ConfigModel_I2C>().FirstOrDefault();
            var UARTConfig = config.ProtocolConfigList.OfType<ConfigModel_UART>().FirstOrDefault();
            var spiConfig = config.ProtocolConfigList.OfType<ConfigModel_SPI>().FirstOrDefault();
            var i3cConfig = config.ProtocolConfigList.OfType<ConfigModel_I3C>().FirstOrDefault();
            var spmiConfig = config.ProtocolConfigList.OfType<ConfigModel_SPMI>().FirstOrDefault();
            var rffeconfig=config.ProtocolConfigList.OfType<ConfigModel_RFFE>().FirstOrDefault();
            var canconfig= config.ProtocolConfigList.OfType<ConfigModel_CAN>().FirstOrDefault();
            // Create a new MigraDoc document
            Doc = new Document();
            Doc.Info.Title = "PGY-LA-EMBD Test Report";
            Doc.Info.Subject = "Shows the result values for the current collection";
            Doc.Info.Author = "Prodigy Technovations";
            try
            {
                DefineStyles();
                CreateFirstPage();
                CreateSecondPage();
                if ((config.ConfigurationMode == DataModule.eConfigMode.PA_Mode || config.ConfigurationMode == DataModule.eConfigMode.Both))
                {
                    if ((!IsStandard && IsProtocolSelected) || IsStandard)
                    {
                        if (i2CConfig != null)
                        {
                            if (SelRange && StartIndex < I2CResults.ResultCollection.Count && StopIndex < I2CResults.ResultCollection.Count
                                && StartIndex < StopIndex && selRange && !IsStandard)
                            {
                                StartIndexReportI2C = StartIndex;
                                EndIndexReportI2C = StopIndex;
                            }
                            else
                            {
                                StartIndexReportI2C = 0;
                                EndIndexReportI2C = I2CResults.ResultCollection.Count - 1;
                            }
                            if (IsStandard)
                            {
                                StartIndexReportI2C = 0;
                                EndIndexReportI2C = I2CResults.ResultCollection.Count - 1;
                            }
                            if (EndIndexReportI2C - StartIndexReportI2C > 499) // limitation for no. of frames
                                EndIndexReportI2C = StartIndexReportI2C + 499;
                            CreatePage("I2C");
                            FillContent("I2C");
                        }
                        if (spiConfig != null)
                        {
                            if (SelRange && StartIndex < SPIResults.SPIFrameCollection.Count && StopIndex < SPIResults.SPIFrameCollection.Count
                                && StartIndex < StopIndex && selRange && !IsStandard)
                            {
                                StartIndexReportSPI = StartIndex;
                                EndIndexReportSPI = StopIndex;
                            }
                            else
                            {
                                StartIndexReportSPI = 0;
                                EndIndexReportSPI = SPIResults.SPIFrameCollection.Count - 1;
                            }
                            if (IsStandard)
                            {
                                StartIndexReportSPI = 0;
                                EndIndexReportSPI = SPIResults.SPIFrameCollection.Count - 1;
                            }
                            if (EndIndexReportSPI - StartIndexReportSPI > 499) // limitation for no. of frames
                                EndIndexReportSPI = StartIndexReportSPI + 499;
                            CreatePage("SPI");
                            FillContent("SPI");
                        }
                        if (UARTConfig != null)
                        {
                            if (SelRange && StartIndex < UARTResults.UARTFrameCollection.Count && StopIndex < UARTResults.UARTFrameCollection.Count
                                && StartIndex < StopIndex && selRange && !IsStandard)
                            {
                                StartIndexReportUART = StartIndex;
                                EndIndexReportUART = StopIndex;
                            }
                            else
                            {
                                StartIndexReportUART = 0;
                                EndIndexReportUART = UARTResults.UARTFrameCollection.Count - 1;
                            }
                            if (IsStandard)
                            {
                                StartIndexReportUART = 0;
                                EndIndexReportUART = UARTResults.UARTFrameCollection.Count - 1;
                            }
                            if (EndIndexReportUART - StartIndexReportUART > 499) // limitation for no. of frames
                                EndIndexReportUART = StartIndexReportUART + 499;
                            CreatePage("UART");
                            FillContent("UART");
                        }
                        if (i3cConfig != null)
                        {
                            if (SelRange && StartIndex < I3CResults.FrameCollection.Count && StopIndex < I3CResults.FrameCollection.Count
                                && StartIndex < StopIndex && selRange && !IsStandard)
                            {
                                StartIndexReportI3C = StartIndex;
                                EndIndexReportI3C = StopIndex;
                            }
                            else
                            {
                                StartIndexReportI3C = 0;
                                EndIndexReportI3C = I3CResults.FrameCollection.Count - 1;
                            }
                            if (IsStandard)
                            {
                                StartIndexReportI3C = 0;
                                EndIndexReportI3C = I3CResults.FrameCollection.Count - 1;
                            }
                            if (EndIndexReportI3C - StartIndexReportI3C > 499) // limitation for no. of frames
                                EndIndexReportI3C = StartIndexReportI3C + 499;
                            CreatePage("I3C");
                            FillContent("I3C");
                        }
                        if (spmiConfig!=null)
                        {
                            if (SelRange && StartIndex < SPMIResults.FrameCollection.Count && StopIndex < SPMIResults.FrameCollection.Count
                                && StartIndex < StopIndex && selRange && !IsStandard)
                            {
                                StartIndexReportSPMI = StartIndex;
                                EndIndexReportSPMI = StopIndex;
                            }
                            else
                            {
                                StartIndexReportSPMI = 0;
                                EndIndexReportSPMI = SPMIResults.FrameCollection.Count - 1;
                            }
                            if (IsStandard)
                            {
                                StartIndexReportSPMI = 0;
                                EndIndexReportSPMI = SPMIResults.FrameCollection.Count - 1;
                            }
                            if (EndIndexReportSPMI - StartIndexReportSPMI > 499) // limitation for no. of frames
                                EndIndexReportSPMI = StartIndexReportSPMI + 499;
                            CreatePage("SPMI");
                            FillContent("SPMI");
                        }
                        if (rffeconfig!=null)
                        {
                            if (SelRange && StartIndex < RFFEResults.FrameCollection.Count && StopIndex < RFFEResults.FrameCollection.Count
                                && StartIndex < StopIndex && selRange && !IsStandard)
                            {
                                StartIndexReportRFFE = StartIndex;
                                EndIndexReportRFFE = StopIndex;
                            }
                            else
                            {
                                StartIndexReportRFFE = 0;
                                EndIndexReportRFFE = RFFEResults.FrameCollection.Count - 1;
                            }
                            if (IsStandard)
                            {
                                StartIndexReportRFFE = 0;
                                EndIndexReportRFFE = RFFEResults.FrameCollection.Count - 1;
                            }
                            if (EndIndexReportRFFE - StartIndexReportRFFE > 499) // limitation for no. of frames
                                EndIndexReportRFFE = StartIndexReportRFFE + 499;
                            CreatePage("RFFE");
                            FillContent("RFFE");
                        }
                        if (canconfig!=null)
                        {
                            if (SelRange && StartIndex < CANResults.CANFrameCollection.Count && StopIndex < CANResults.CANFrameCollection.Count
                                && StartIndex < StopIndex && selRange && !IsStandard)
                            {
                                StartIndexReportCAN = StartIndex;
                                EndIndexReportCAN = StopIndex;
                            }
                            else
                            {
                                StartIndexReportCAN = 0;
                                EndIndexReportCAN = CANResults.CANFrameCollection.Count - 1;
                            }
                            if (IsStandard)
                            {
                                StartIndexReportCAN = 0;
                                EndIndexReportCAN = CANResults.CANFrameCollection.Count - 1;
                            }
                            if (EndIndexReportCAN - StartIndexReportCAN > 499) // limitation for no. of frames
                                EndIndexReportCAN = StartIndexReportCAN + 499;
                            CreatePage("CAN");
                            FillContent("CAN");
                        }
                    }
                }
                //else if (ConfigModel.GetInstance().ConfigurationMode == DataModule.eConfigMode.LA_Mode)
                //{
                //    if ((!IsStandard && IsLogicSelected) || IsStandard)
                //    {
                //        CreatePageLA();
                //        FillContentLA();
                //    }
                //}
                //if ((!IsStandard && IsLogicSelected) || IsStandard)
                //{
                //    CreatePageLA();
                //    FillContentLA();
                //}
                AddRemarks();
            }
            catch (Exception ex)
            {

                //MessageBox.Show("Error in Report Generation - " + ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return Doc;
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

        public void FillContentLA()
        {
            var resultCollection = waveformListing.WfmHolder.ASyncCollection;
            if (!(resultCollection.Count() > 0))
            {
                //MessageBox.Show("No collection found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
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
            int j = 0;
            for (int i = startIndex; i <= stopIndex; i++)
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
            this.Tbl.SetEdge(0, 0, j, stopIndex, Edge.Box, BorderStyle.Single, 0.75);
            Tbl.Borders.Visible = true;
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
        public void CreatePageLA()
        {
            Section section = Doc.AddSection();
            AddHeaderFooter(section);

            // Topic Heading (Report)
            Paragraph para = section.AddParagraph();
            para.AddFormattedText("Waveform List\n\n", TextFormat.Underline);
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
            Tbl.Rows.LeftIndent = 0;
            Tbl.LeftPadding = 0;
            Tbl.RightPadding = 0;
            Tbl.TopPadding = 0;
            Tbl.BottomPadding = 0;

            tm = new TextMeasurement(Tbl.Format.Font);

            Column column = Tbl.AddColumn("1.1cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column = Tbl.AddColumn("2.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            if (config.IsCh10Visible)
            {
                column = Tbl.AddColumn("1.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh9Visible)
            {
                column = Tbl.AddColumn("1.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh8Visible)
            {
                column = Tbl.AddColumn("1.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh7Visible)
            {
                column = Tbl.AddColumn("1.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh6Visible)
            {
                column = Tbl.AddColumn("1.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh5Visible)
            {
                column = Tbl.AddColumn("1.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh4Visible)
            {
                column = Tbl.AddColumn("1.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh3Visible)
            {
                column = Tbl.AddColumn("1.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh2Visible)
            {
                column = Tbl.AddColumn("1.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }
            if (config.IsCh1Visible)
            {
                column = Tbl.AddColumn("1.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;
            }

            Row row = Tbl.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = Colors.LightBlue;

            WaveformListingModel WVM = waveformListing;

            int i = 0;
            row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], "S No"));
            row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[i].MergeDown = 1;
            i++;
            row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], "Time"));
            row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[i].MergeDown = 1;
            i++;
            if (config.IsCh10Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.Ch10Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh9Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.Ch9Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh8Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.Ch8Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh7Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.Ch7Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh6Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.Ch6Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh5Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.Ch5Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh4Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.Ch4Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh3Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.Ch3Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh2Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.Ch2Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            if (config.IsCh1Visible)
            {
                row.Cells[i].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[i], WVM.Ch1Name));
                row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[i].MergeDown = 1;
                i++;
            }
            Tbl.SetEdge(0, 0, i, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        }


        public void AddHeaderFooter(Section section)
        {
            Paragraph paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddText("Page ");
            paragraph.AddPageField();
            paragraph.Format.Font.Size = 9;
            paragraph.Format.Alignment = ParagraphAlignment.Right;
        }
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
            Tbl.Rows.LeftIndent = 0;
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
                column = Tbl.AddColumn("1.3cm");
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
                row.Cells[11].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Freq"));
                row.Cells[11].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[11].MergeDown = 1;
                row.Cells[10].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[0], "Error"));
                row.Cells[10].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[10].MergeDown = 1;
             
            }
            else if (protocolType == "SPMI")
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

                var canconfig = config.ProtocolConfigList.OfType<ConfigModel_CAN>().FirstOrDefault();
                Column column = Tbl.AddColumn("1.1cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.7cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("2.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.5cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.5cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("3.7cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("1.5cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                column = Tbl.AddColumn("2.0cm");
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
                row.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[3], "ID"));
                row.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[3].MergeDown = 1;
                row.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[3], "IDE"));
                row.Cells[4].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[4].MergeDown = 1;
                row.Cells[5].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[4], "DLC"));
                row.Cells[5].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[5].MergeDown = 1;
                row.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[5], "DATA"));
                row.Cells[6].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[6].MergeDown = 1;
                row.Cells[7].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[6], "CRC"));
                row.Cells[7].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[7].MergeDown = 1;
                if(canconfig.BRS)
                {
                    row.Cells[8].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[7], "Freq/BRSFreq"));
                    row.Cells[8].Format.Alignment = ParagraphAlignment.Center;
                    row.Cells[8].MergeDown = 1;
                }
                else
                {
                    row.Cells[8].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[7], "Frequency"));
                    row.Cells[8].Format.Alignment = ParagraphAlignment.Center;
                    row.Cells[8].MergeDown = 1;
                }
      
                row.Cells[9].AddParagraph(AdjustIfTooWideToFitIn(row.Cells[9], "ErrorType"));
                row.Cells[9].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[9].MergeDown = 1;
            }
            if (protocolType == "I2C")
                Tbl.SetEdge(0, 0, 8, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
            else if (protocolType == "I3C")
                Tbl.SetEdge(0, 0, 12, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
            else if (protocolType == "SPMI" || protocolType == "RFFE")
                Tbl.SetEdge(0, 0, 9, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
            else if (protocolType == "CAN")
                Tbl.SetEdge(0, 0, 10, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
          
            else
                Tbl.SetEdge(0, 0, 7, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
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
                        //MessageBox.Show("Exception");
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
            //string version ="1.0.0.2";

            //string version = Assembly.GetEntryAssembly().GetName().Version.ToString();

            Assembly version = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo f1 = System.Diagnostics.FileVersionInfo.GetVersionInfo(version.Location);
            string ver = f1.FileVersion;
            para = row.Cells[2].AddParagraph(version == null ? "" : ver);
            row.Cells[2].MergeRight = 4;
        }
        public void CreateSecondPage()
        {
            // Each MigraDoc document needs at least one section.
            var i2CConfig = config.ProtocolConfigList.OfType<ConfigModel_I2C>().FirstOrDefault();
            var spiConfig = config.ProtocolConfigList.OfType<ConfigModel_SPI>().FirstOrDefault();
            var UARTConfig = config.ProtocolConfigList.OfType<ConfigModel_UART>().FirstOrDefault();
            var i3cConfig = config.ProtocolConfigList.OfType<ConfigModel_I3C>().FirstOrDefault();
            var spmiConfig = config.ProtocolConfigList.OfType<ConfigModel_SPMI>().FirstOrDefault();
            var rffeconfig=config.ProtocolConfigList.OfType<ConfigModel_RFFE>().FirstOrDefault();
            var canconfig = config.ProtocolConfigList.OfType<ConfigModel_CAN>().FirstOrDefault();
            Section section = Doc.AddSection();
            AddHeaderFooter(section);
            Paragraph para = section.AddParagraph();
            para.AddFormattedText("Setup Details\n\n", TextFormat.Underline);
            para.Format.Alignment = ParagraphAlignment.Left;
            para.Format.Font.Size = 12;

            if (config.ConfigurationMode == DataModule.eConfigMode.PA_Mode || config.ConfigurationMode == DataModule.eConfigMode.Both)
            {
                if (! (canconfig!=null))
                {
                    WriteVoltDetails(section);
                }
                section.AddParagraph("\n\n");
                if (UARTConfig != null)
                {
                    WriteConfigurationDetails("UART", section);
                    section.AddParagraph("\n\n");
                }
                if (spiConfig!=null)
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
                if (spmiConfig!=null)
                {
                    WriteConfigurationDetails("SPMI", section);
                    section.AddParagraph("\n\n");
                }
                if (rffeconfig!=null)
                {
                    WriteConfigurationDetails("RFFE", section);
                    section.AddParagraph("\n\n");
                }
                if (canconfig!=null)
                {
                    WriteConfigurationDetails("CAN", section);
                    section.AddParagraph("\n\n");
                }
            }
            else if (config.ConfigurationMode == DataModule.eConfigMode.LA_Mode)
            {
                WriteConfigurationDetailsLA(section);
            }
        }


        public void WriteVoltDetails(Section section)
        {
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
            var i2CConfig = config.ProtocolConfigList.OfType<ConfigModel_I2C>().FirstOrDefault();
            var spiConfig = config.ProtocolConfigList.OfType<ConfigModel_SPI>().FirstOrDefault();
            var uartConfig = config.ProtocolConfigList.OfType<ConfigModel_UART>().FirstOrDefault();
            var i3cConfig = config.ProtocolConfigList.OfType<ConfigModel_I3C>().FirstOrDefault();
            var spmiConfig = config.ProtocolConfigList.OfType<ConfigModel_SPMI>().FirstOrDefault();
            var rffeConfig=config.ProtocolConfigList.OfType<ConfigModel_RFFE>().FirstOrDefault();
            var canConfig = config.ProtocolConfigList.OfType<ConfigModel_CAN>().FirstOrDefault();
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
                row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], ((i3cConfig.ChannelIndex_SCL == DataModule.eChannles.None) ? "" : ("SCL : " +i3cConfig.ChannelIndex_SCL.ToString() + " ; ")) + ((i3cConfig.ChannelIndex_SDA == DataModule.eChannles.None) ? "" : ("SDA : " + i3cConfig.ChannelIndex_SDA.ToString()))));
            else if (protocolType == "SPMI")
                row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], ((spmiConfig.ChannelIndex_SCL == DataModule.eChannles.None) ? "" : ("CLK : " + spmiConfig.ChannelIndex_SCL.ToString() + " ; ")) + ((spmiConfig.ChannelIndex_SDA == DataModule.eChannles.None) ? "" : ("DATA : " + spmiConfig.ChannelIndex_SDA.ToString()))));
            else if (protocolType == "RFFE")
                row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], ((rffeConfig.ChannelIndex_SCL == DataModule.eChannles.None) ? "" : ("CLK : " + rffeConfig.ChannelIndex_SCL.ToString() + " ; ")) + ((rffeConfig.ChannelIndex_SDA == DataModule.eChannles.None) ? "" : ("DATA : " + rffeConfig.ChannelIndex_SDA.ToString()))));
            else if (protocolType == "CAN")
                row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], ((canConfig.ChannelIndex == DataModule.eChannles.None) ? "" : ("CH : " + canConfig.ChannelIndex.ToString() + " ; ")) ));

            if (config.ConfigurationMode == DataModule.eConfigMode.LA_Mode || config.ConfigurationMode == DataModule.eConfigMode.Both)
            {
                Row row2 = tbl1.AddRow();
                row2.Format.Alignment = ParagraphAlignment.Center;
                row2.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row2.Cells[0], "Sample Rate"));
                row2.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row2.Cells[0].Format.Font.Bold = true;
                row2.Cells[0].Shading.Color = Colors.SteelBlue;
                //volt = GetVolt(config.SignalAmpCh7_8_9_10);
                string sampleRate = "";
                sampleRate = GetSampleRate(config.SampleRateLAPA);

                row2.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row2.Cells[1], sampleRate));
                // row24.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row24.Cells[1], volt));
                row2.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                row1.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            }



            tbl1.SetEdge(0, 1, 2, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        }

        public void WriteConfigurationDetailsLA(Section section)
        {
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
        public void FillContent(string protocolType)
        {
            Tbl.AddRow();
            if (protocolType == "I2C")
            {
                var resultCollection = I2CResults.ResultCollection;
                if (!(resultCollection.Count() > 0))
                {
                    // MessageBox.Show("No I2C collection found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
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


                    row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], CUtilities.ToEngineeringNotation(resultCollection[i].StartTime - I2CResults.TriggerTime).ToString() + "s"));
                    row1.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[2], "0x" + (resultCollection[i].AddressFirst >> 1).ToString("X2")));

                    if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.I2C)
                    {
                        if (index == I2CResults.TriggerPacket + 1)
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
            else if (protocolType == "SPI")
            {
                var resultCollection = SPIResults.SPIFrameCollection;
                if (!(resultCollection.Count() > 0))
                {
                    // MessageBox.Show("No SPI collection found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
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
                    if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.SPI)
                    {
                        // TriggerPacket = SPIResultHolder.GetInstance().TriggerPacket;
                        if (index == SPIResults.TriggerPacket + 1)
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

                    // row1.Cells[0].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[0], index.ToString()));
                    row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], CUtilities.ToEngineeringNotation(resultCollection[i].StartTime - SPIResults.TriggerTime).ToString() + "s"));
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
            else if (protocolType == "UART")
            {
                var resultCollection = UARTResults.UARTFrameCollection;
                if (!(resultCollection.Count() > 0))
                {
                    //   MessageBox.Show("No UART collection found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
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
                    if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.UART)
                    {
                        // TriggerPacket = UARTResultHolder.GetInstance().TriggerPacket;
                        if (index == UARTResults.TriggerPacket + 1)
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

                    row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], CUtilities.ToEngineeringNotation(resultCollection[i].StartTime - UARTResults.TriggerTime).ToString() + "s"));
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
            else if (protocolType == "SPMI")
            {
                var resultCollection = SPMIResults.FrameCollection;
                if (!(resultCollection.Count() > 0))
                {
                    // MessageBox.Show("No SPMI collection found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return;
                }
                int index = 1;
                for (int i = StartIndexReportSPMI; i <= EndIndexReportSPMI; i++)
                {
                    Row row1 = new Row();
                    row1 = Tbl.AddRow();
                    row1.HeadingFormat = false;
                    row1.Format.Alignment = ParagraphAlignment.Center;
                    if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.SPMI)
                    {
                        if (index == SPMIResults.TriggerPacket + 1)
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

                    row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], CUtilities.ToEngineeringNotation(resultCollection[i].StartTime - SPMIResults.TriggerTime).ToString() + "s"));
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
                this.Tbl.SetEdge(0, 0, 9, (EndIndexReportSPMI - StartIndexReportSPMI + 1), Edge.Box, BorderStyle.Single, 0.75);
                Tbl.Borders.Visible = true;
            }
            else if (protocolType == "RFFE")
            {
                var resultCollection = RFFEResults.FrameCollection;
                if (!(resultCollection.Count() > 0))
                {
                    //MessageBox.Show("No RFFE collection found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return;
                }
                int index = 1;
                for (int i = StartIndexReportRFFE; i <= EndIndexReportRFFE; i++)
                {
                    Row row1 = new Row();
                    row1 = Tbl.AddRow();
                    row1.HeadingFormat = false;
                    row1.Format.Alignment = ParagraphAlignment.Center;
                    if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.RFFE)
                    {

                        if (index == RFFEResults.TriggerPacket + 1)
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

                    row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], CUtilities.ToEngineeringNotation(resultCollection[i].StartTime - RFFEResults.TriggerTime).ToString() + "s"));
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
            else if (protocolType == "I3C")
            {
                var resultCollection = I3CResults.FrameCollection;
                if (!(resultCollection.Count() > 0))
                {
                    //MessageBox.Show("No I3C collection found", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
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
                            if (index == I3CResults.TriggerPacket + 1)
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


                        if (j == 0)
                        {
                            row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], resultCollection[i].FrameType.ToString())); //frame type

                        }
                        else
                        {
                            row1.Cells[1].AddParagraph("-");
                        }


                        //}
                        //    else
                        //        row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], CUtilities.ToEngineeringNotation(resultCollection[i].PacketCollection[j].TimeStamp - SessionConfiguration.TriggerTime)) + "s");
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
            else if(protocolType=="CAN")
            {


                var resultCollection = CANResults.CANFrameCollection.ToList();
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
                        if (index == CANResults.TriggerPacket + 1)
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
                    row1.Cells[1].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[1], CUtilities.ToEngineeringNotation(resultCollection[i].StartTime - CANResults.TriggerTime).ToString() + "s"));
                    row1.Cells[2].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[2], resultCollection[i].FrameType.ToString()));
                    row1.Cells[3].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[3], ((resultCollection[i].IDDdataBytes == -1) ? "-" : ("0x" + resultCollection[i].IDDdataBytes.ToString("X2")))));
                    row1.Cells[4].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[4], ((resultCollection[i].IDEDdataBytes == -1) ? "-" : ("0x" + resultCollection[i].IDEDdataBytes.ToString("X2")))));
                    row1.Cells[5].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[5], ((resultCollection[i].DLCDdataBytes == -1) ? "-" : ("0x" + resultCollection[i].DLCDdataBytes.ToString("X2")))));
                    var canconfig = config.ProtocolConfigList.OfType<ConfigModel_CAN>().FirstOrDefault();
                    if (canconfig.BRS)
                    {
                        row1.Cells[8].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[8], CUtilities.FormatNumber(resultCollection[i].Frequency, Units.BASE_UNIT).Replace("M", "") + "/" + CUtilities.FormatNumber(resultCollection[i].BRSFrequency, Units.BASE_UNIT) + "Hz"));

                    }

                    else
                    {
                        row1.Cells[8].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[8], CUtilities.FormatNumber(resultCollection[i].Frequency, Units.BASE_UNIT) + "Hz"));
                        //row1.Cells[9].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[9], CUtilities.FormatNumber(resultCollection[i].Frequency, Units.BASE_UNIT) + "Hz"));
                    }
                
                    row1.Cells[9].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[9], resultCollection[i].ErrorType.ToString()));
                    string tempDataStr = "";
                    foreach (var dat in resultCollection[i].DataBytes)
                    {
                        tempDataStr += "0x";
                        tempDataStr += dat.ToString("X2");
                        tempDataStr += " ";
                    }
                    row1.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[6], ((resultCollection[i].DLCDdataBytes == 0) ? "-" : tempDataStr)));
                    row1.Cells[7].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[7], ((resultCollection[i].CRCDdataBytes == -1) ? "-" : ("0x" + resultCollection[i].CRCDdataBytes.ToString("X2")))));
                    //string error = "";
                    //if (resultCollection[i].ErrorType == DataModule.eErrorType.None)
                    //    error = "-";
                    //else
                    //    error = resultCollection[i].ErrorType.ToString();
                    //row1.Cells[6].AddParagraph(AdjustIfTooWideToFitIn(row1.Cells[6], error));
                    index++;
                }
                this.Tbl.SetEdge(0, 0, 10, (endIndexReportCAN - startIndexReportCAN + 1), Edge.Box, BorderStyle.Single, 0.75);
                Tbl.Borders.Visible = true;
            }
        }

        private string AdjustIfTooWideToFitIn(string v)
        {
            throw new NotImplementedException();
        }

        #endregion



        #region CSV
        eResponseFlag GenerateCSV(string path, int startIndex = -1, int stopIndex = -1)
        {
            var i2CConfig = config.ProtocolConfigList.OfType<ConfigModel_I2C>().FirstOrDefault();
            var spiConfig = config.ProtocolConfigList.OfType<ConfigModel_SPI>().FirstOrDefault();
            var UARTConfig = config.ProtocolConfigList.OfType<ConfigModel_UART>().FirstOrDefault();
            var i3cConfig = config.ProtocolConfigList.OfType<ConfigModel_I3C>().FirstOrDefault();
            var spmiConfig = config.ProtocolConfigList.OfType<ConfigModel_SPMI>().FirstOrDefault();
            var rffeconfig=config.ProtocolConfigList.OfType<ConfigModel_RFFE>().FirstOrDefault();
            //var resultI2C = ResultModel_I2C.GetInstance();
            //List<I2CFrame> resultcollectioni2c = resultI2C.ResultCollection;
            //var resultSPI = ResultModel_SPI.GetInstance();
            //ObservableCollection<SPIFrame> resultcollectionSPI = resultSPI.SPIFrameCollection;
            //var resultUART = ResultModel_UART.GetInstance();
            //ObservableCollection<UARTFrame> resultcollectionUART = resultUART.UARTFrameCollection;
            //var resultSPMI = ResultModel_SPMI.GetInstance();
            //ObservableCollection<SPMIFrameStructure> resultcollectionSPMI = resultSPMI.FrameCollection;
            //var resultRFFE = ResultModel_RFFE.GetInstance();
            //ObservableCollection<RFFEFrameStructure> resultcollectionRFFE = resultRFFE.FrameCollection;
            //var resultI3C = ResultModel_I3C.GetInstance();
            //List<FramePattern> resultcollectionI3C = resultI3C.FrameCollection;
            //var resultCAN=ResultModel_CAN.GetInstance();
            //ObservableCollection<CANFrame> resultcollectioncan = resultCAN.CANFrameCollection;


            if (IsFileLocked(new FileInfo(path)))
            {
                return eResponseFlag.Task_Busy;
            }
            else
            {
                if (startIndex == -1 && stopIndex == -1)
                {
                    startIndex = 0;
                    
                }
                else if (startIndex == 0 && stopIndex == 0)
                {
                    return eResponseFlag.Invalid_Data;
                }
                else if (startIndex <= 0 || stopIndex >= I2CResults.ResultCollection.Count() || startIndex >= stopIndex)
                {
                    return eResponseFlag.Invalid_Data;
                }
                // Task.Factory.StartNew(() =>
                //{
                StringBuilder str = new StringBuilder();
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
                    if (i2CConfig != null)
                    {
                        String[] strChannel = new String[2];
                        strChannel[0] = "I2C CLK";
                        strChannel[1] = "I2C DATA";
                        str.AppendLine(String.Join(",", strChannel));
                        String[] strChannel1 = new String[2];
                        strChannel1[0] = i2CConfig.ChannelIndex_SCL.ToString();
                        strChannel1[1] = i2CConfig.ChannelIndex_SDA.ToString();
                        str.AppendLine(String.Join(",", strChannel1));
                        str.AppendLine();
                        str.AppendLine();
                    }
                    if (spiConfig!=null)
                    {
                        String[] strChannel = new String[4];
                        strChannel[0] = "SPI CLK";
                        strChannel[1] = "SPI CS";
                        strChannel[2] = "SPI MOSI";
                        strChannel[3] = "SPI MISO";
                        str.AppendLine(String.Join(",", strChannel));
                        String[] strChannel1 = new String[4];
                        strChannel1[0] = spiConfig.ChannelIndex_CLK.ToString();
                        strChannel1[1] = spiConfig.ChannelIndex_CS.ToString();
                        strChannel1[2] = spiConfig.ChannelIndex_MOSI.ToString();
                        strChannel1[3] = spiConfig.ChannelIndex_MISO.ToString();
                        str.AppendLine(String.Join(",", strChannel1));
                        str.AppendLine();
                        str.AppendLine();
                    }
                    if (UARTConfig!=null)
                    {
                        String[] strChannel = new String[2];
                        strChannel[0] = "UAR TX";
                        strChannel[1] = "UART RX";
                        str.AppendLine(String.Join(",", strChannel));
                        String[] strChannel1 = new String[2];
                        strChannel1[0] = UARTConfig.ChannelIndex_TX.ToString();
                        strChannel1[1] = UARTConfig.ChannelIndex_RX.ToString();
                        str.AppendLine(String.Join(",", strChannel1));
                        str.AppendLine();
                        str.AppendLine();
                    }
                    if (i3cConfig != null)
                    {
                        String[] strChannel = new String[2];
                        strChannel[0] = "I3C CLK";
                        strChannel[1] = "I3C DATA";
                        str.AppendLine(String.Join(",", strChannel));
                        String[] strChannel1 = new String[2];
                        strChannel1[0] = i3cConfig.ChannelIndex_SCL.ToString();
                        strChannel1[1] = i3cConfig.ChannelIndex_SDA.ToString();
                        str.AppendLine(String.Join(",", strChannel1));
                        str.AppendLine();
                        str.AppendLine();
                    }
                    if (spmiConfig!=null)
                    {
                        String[] strChannel = new String[2];
                        strChannel[0] = "SPMI CLK";
                        strChannel[1] = "SPMI DATA";
                        str.AppendLine(String.Join(",", strChannel));
                        String[] strChannel1 = new String[2];
                        strChannel1[0] = spmiConfig.ChannelIndex_SCL.ToString();
                        strChannel1[1] = spmiConfig.ChannelIndex_SDA.ToString();
                        str.AppendLine(String.Join(",", strChannel1));
                        str.AppendLine();
                        str.AppendLine();
                    }
                    if (rffeconfig!=null)
                    {
                        String[] strChannel = new String[2];
                        strChannel[0] = "RFFE CLK";
                        strChannel[1] = "RFFE DATA";
                        str.AppendLine(String.Join(",", strChannel));
                        String[] strChannel1 = new String[2];
                        strChannel1[0] = rffeconfig.ChannelIndex_SCL.ToString();
                        strChannel1[1] = rffeconfig.ChannelIndex_SDA.ToString();
                        str.AppendLine(String.Join(",", strChannel1));
                        str.AppendLine();
                        str.AppendLine();
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

                        if (config.SelectedClock2 != eChannles.None)
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
                        IList<ProtocolActivityHolder> collection = protocolActivity.ProtocolCollection;
                        //str.Append(collection.Count());
                        //if (collection.Count == 0)
                        //    return eResponseFlag.Invalid_DataLength;
                        AddProtocolToCSV(ref str, collection);

                        str.AppendLine();
                        str.AppendLine();

                    }
                    catch (Exception ex)
                    {

                    }


                }
                else if (config.ConfigurationMode == DataModule.eConfigMode.LA_Mode)
                {
                    AddWaveformListing(ref str);
                    str.AppendLine();
                    str.AppendLine();
                }

                File.WriteAllText(path, str.ToString());
                return eResponseFlag.Success;
                // }).Wait();
                //  return eResponseFlag.Success;
            }
        }


        eResponseFlag AddProtocolToCSV(ref StringBuilder strResult, IList<ProtocolActivityHolder> protocolList)
        {
            StringBuilder str = new StringBuilder();

            StringBuilder strTemp = new StringBuilder();
             var i2CConfig = config.ProtocolConfigList.OfType<ConfigModel_I2C>().FirstOrDefault();
             var spiConfig = config.ProtocolConfigList.OfType<ConfigModel_SPI>().FirstOrDefault();
            var UARTConfig = config.ProtocolConfigList.OfType<ConfigModel_UART>().FirstOrDefault();
            var i3cConfig = config.ProtocolConfigList.OfType<ConfigModel_I3C>().FirstOrDefault();
            var spmiConfig = config.ProtocolConfigList.OfType<ConfigModel_SPMI>().FirstOrDefault();
            var rffeconfig=config.ProtocolConfigList.OfType<ConfigModel_RFFE>().FirstOrDefault();
            var canconfig=config.ProtocolConfigList.OfType<ConfigModel_CAN>().FirstOrDefault();
            //var resultI2C = ResultModel_I2C.GetInstance().ResultCollection;
            //var resultSPI = ResultModel_SPI.GetInstance().SPIFrameCollection;
            //var resultUART = ResultModel_UART.GetInstance().UARTFrameCollection;
            //var resultSPMI = ResultModel_SPMI.GetInstance().FrameCollection;
            //var resultRFFE = ResultModel_RFFE.GetInstance().FrameCollection;
            //var resultI3C = ResultModel_I3C.GetInstance().FrameCollection;
            //var resultCAN = ResultModel_CAN.GetInstance().CANFrameCollection;
            bool addI2C = true;
            bool addSPI = true;
            bool addUART = true;
            bool addSPMI = true;
            bool addRFFE = true;
            bool addI3C = true;
            bool addcan = true;

            if (i2CConfig != null)
            {
                if (I2CResults.ResultCollection == null)
                {
                    // MessageBox.Show("I2C Result not found", "Empty Collection", MessageBoxButton.OK, MessageBoxImage.Information);
                    //addI2C = false;
                    return eResponseFlag.Fail;
                }
                if (I2CResults.ResultCollection.Count == 0)
                {
                    //MessageBox.Show("I2C Result not found", "Empty Collection", MessageBoxButton.OK, MessageBoxImage.Information);
                    //addI2C = false;
                    return eResponseFlag.Fail;
                }
                else
                {
                    int index = 1;
                    str.Append("I2C PROTOCOL");
                    str.AppendLine();
                    str.AppendLine();
                    //str.Append("Trigger,");
                    str.Append("Index,");
                    str.Append("Time,");
                    str.Append("Frame,");
                    str.Append("S/Sr,");
                    str.Append("Address,");
                    str.Append("Read/Write,");
                    str.Append("A/N,");
                    str.Append("Data,");
                    str.Append("Frequency,");
                    str.Append("Error,");
                    strResult.AppendLine(String.Join(",", str));
                    foreach (var frame in I2CResults.ResultCollection)
                    {

                        // StringBuilder str1 = new StringBuilder();
                        String[] strTemp1 = new String[11];
                        strTemp1[0] = (index.ToString());


                        string S = CUtilities.ToEngineeringNotation(frame.StartTime - I2CResults.TriggerTime).Replace(",", "").ToString() + "s";
                        strTemp1[1] = S.Replace(",", " ").ToString();

                        if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.I2C)
                        {
                            if (frame.FrameIndex == I2CResults.TriggerPacket)
                            {
                                
                                strTemp1[0] = "    T       " + strTemp1[0];
                            }
                        }
                        strTemp1[2] = "I2C";
                        strTemp1[3] = frame.Start.ToString();
                        strTemp1[4] = ("0x" + (frame.AddressFirst >> 1).ToString("X2"));
                        strTemp1[5] = ((frame.AddressFirst & 0x01) == 0 ? "WR" : "RD");
                        string datStr1 = "";
                        foreach (var dat in frame.DataBytes)
                        {
                            datStr1 += "0x" + dat.ToString("X2");
                            datStr1 += " : ";
                        }
                        if (datStr1 == "")
                            datStr1 = "-";
                        else
                            datStr1 = datStr1.Substring(0, datStr1.Length - 2);
                        strTemp1[7] = (datStr1);
                        string ackString1 = "";
                        if ((frame.AckAddr & 0x0F) == 0x0F)
                            ackString1 = "Nack";
                        else
                            ackString1 = "Ack";
                        strTemp1[6] = (ackString1);
                        strTemp1[8] = (CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz");
                        strTemp1[9] = (frame.ErrorType.ToString());

                        strResult.AppendLine(String.Join(",", strTemp1));
                        //  strResult.AppendLine(String.Join(",", strTemp1));
                        index++;


                    }
                    strResult.AppendLine();
                    strResult.AppendLine();
                }

            }
            else
                addI2C = false;
            if (spiConfig != null)
            {
                if (SPIResults.SPIFrameCollection == null)
                {
                    //MessageBox.Show("SPI Result not found", "Empty Collection", MessageBoxButton.OK, MessageBoxImage.Information);
                    addSPI = false;
                }
                if (SPIResults.SPIFrameCollection.Count == 0)
                {
                    //MessageBox.Show("SPI Result not found", "Empty Collection", MessageBoxButton.OK, MessageBoxImage.Information);
                    addSPI = false;
                }
                else
                {
                    int index = 1;
                    StringBuilder strSPI = new StringBuilder();
                    strSPI.Append("SPI PROTOCOL");
                    strSPI.AppendLine();
                    strSPI.AppendLine();
                    strSPI.Append("Index,");
                    strSPI.Append("Time,");
                    strSPI.Append("Frame,");
                    strSPI.Append("MOSI,");
                    strSPI.Append("MISO,");
                    strSPI.Append("Frequency,");
                    strSPI.Append("Stop,");
                    strSPI.Append("Error,");
                    strResult.AppendLine(String.Join(",", strSPI));
                    foreach (var frame in SPIResults.SPIFrameCollection)
                    {
                        String[] strTemp1 = new String[11];
                        strTemp1[0] = index.ToString();
                        if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.SPI)
                        {
                            if (frame.FrameIndex == SPIResults.TriggerPacket)
                            {
                                strTemp1[0] = "T    " + strTemp1[0];
                            }
                        }
                        strTemp1[1] = CUtilities.ToEngineeringNotation(frame.StartTime - SPIResults.TriggerTime).Replace(",", "").ToString() + "s";
                        strTemp1[2] = "SPI";
                        string mOSIStr = "";

                        foreach (var dat in frame.MOSIDataBytes)
                        {
                            mOSIStr += "0x" + dat.ToString("X2");
                            mOSIStr += " : ";
                        }
                        if (mOSIStr == "")
                            mOSIStr = "-";
                        else
                            mOSIStr = mOSIStr.Substring(0, mOSIStr.Length - 2);
                        strTemp1[3] = mOSIStr;
                        string mISOStr = "";
                        foreach (var dat in frame.MISODataBytes)
                        {
                            mISOStr += "0x" + dat.ToString("X2");
                            mISOStr += " : ";
                        }
                        if (mISOStr == "")
                            mISOStr = "-";
                        else
                            mISOStr = mISOStr.Substring(0, mISOStr.Length - 2);
                        strTemp1[4] = mISOStr;
                        strTemp1[5] = CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz";
                        strTemp1[6] = frame.HasStop.ToString();
                        strTemp1[7] = frame.ErrorType.ToString();

                        strResult.AppendLine(String.Join(",", strTemp1));
                        index++;

                    }
                    strResult.AppendLine();
                    strResult.AppendLine();
                }
            }
            else
                addSPI = false;


            if (UARTConfig!=null)
            {
                if (UARTResults.UARTFrameCollection == null)
                {
                    //MessageBox.Show("UART Result not found", "Empty Collection", MessageBoxButton.OK, MessageBoxImage.Information);
                    addUART = false;
                }
                if (UARTResults.UARTFrameCollection.Count == 0)
                {
                    //MessageBox.Show("UART Result not found", "Empty Collection", MessageBoxButton.OK, MessageBoxImage.Information);
                    addUART = false;
                }
                else
                {
                    int index = 1;
                    StringBuilder struart = new StringBuilder();
                    struart.Append("UART PROTOCOL");
                    struart.AppendLine();
                    struart.AppendLine();
                    struart.Append("Index,");
                    struart.Append("Time,");
                    struart.Append("Frame,");
                    struart.Append("Tx Data,");
                    struart.Append("Rx Data,");
                    struart.Append("Frequency,");
                    struart.Append("Stop,");
                    struart.Append("Error,");
                    strResult.AppendLine(String.Join(",", struart));
                    foreach (var frame in UARTResults.UARTFrameCollection)
                    {

                        String[] strTemp1 = new String[11];
                        strTemp1[0] = index.ToString();
                        if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.UART)
                        {
                            if (frame.FrameIndex == UARTResults.TriggerPacket)
                            {
                                strTemp1[0] = "T    " + strTemp1[0];
                            }
                        }

                        strTemp1[1] = CUtilities.ToEngineeringNotation(frame.StartTime - UARTResults.TriggerTime).Replace(",", "").ToString() + "s";
                        strTemp1[2] = "UART";
                        strTemp1[3] = (((frame.TXDdataBytes == -1) ? "-" : ("0x" + frame.TXDdataBytes.ToString("X2"))));
                        strTemp1[4] = (((frame.RXdataBytes == -1) ? "-" : ("0x" + frame.RXdataBytes.ToString("X2"))));

                        strTemp1[5] = CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz";
                        strTemp1[6] = frame.HasStop.ToString();
                        strTemp1[7] = frame.ErrorType.ToString();

                        strResult.AppendLine(String.Join(",", strTemp1));
                        index++;

                    }
                    strResult.AppendLine();
                    strResult.AppendLine();
                }
            }
            else
                addUART = false;

            if (spmiConfig!=null)
            {
                if (SPMIResults.FrameCollection == null)
                {
                    // MessageBox.Show("SPMI Result not found", "Empty Collection", MessageBoxButton.OK, MessageBoxImage.Information);
                    addSPMI = false;
                }
                if (SPMIResults.FrameCollection.Count == 0)
                {
                    // MessageBox.Show("SPMI Result not found", "Empty Collection", MessageBoxButton.OK, MessageBoxImage.Information);
                    addSPMI = false;
                }
                else
                {
                    int index = 1;
                    StringBuilder strSPMI = new StringBuilder();
                    strSPMI.Append("SPMI PROTOCOL");
                    strSPMI.AppendLine();
                    strSPMI.AppendLine();
                    strSPMI.Append("Index,");
                    strSPMI.Append("Time,");
                    strSPMI.Append("Frame,");
                    strSPMI.Append("Slave/MID,");
                    strSPMI.Append("Command,");
                    strSPMI.Append("Reg Address,");
                    strSPMI.Append("Data,");
                    strSPMI.Append("ByteCount,");
                    strSPMI.Append("Frequency,");
                    strSPMI.Append("Error,");
                    strResult.AppendLine(String.Join(",", strSPMI));
                    foreach (var frame in SPMIResults.FrameCollection)
                    {
                        String[] strTemp1 = new String[11];

                        strTemp1[0] = index.ToString();

                        if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.SPMI)
                        {
                            if (frame.FrameIndex == SPMIResults.TriggerPacket)
                            {
                                strTemp1[0] = "T    " + strTemp1[0];
                            }
                        }
                        strTemp1[1] = CUtilities.ToEngineeringNotation(frame.StartTime - SPMIResults.TriggerTime).Replace(",", "").ToString() + "s";
                        strTemp1[2] = "SPMI";


                        strTemp1[3] = (frame.SlaveId == -1) ? "-," : ("0x" + frame.SlaveId.ToString("X2"));
                        strTemp1[4] = frame.Command.CmdType.ToString();

                        strTemp1[5] = "0x" + frame.IntAddress.ToString("X2");
                        strTemp1[6] = frame.Data == null ? "-" : String.Join(" ", frame.Data.Select(obj => String.Format("0x{0:X}", obj.Value)));
                        strTemp1[7] = (frame.ByteCount == -1) ? "-" : ("0x" + frame.ByteCount.ToString("X2"));
                        strTemp1[8] = CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz";
                        strTemp1[9] = frame.ErrorType.ToString();

                        strResult.AppendLine(String.Join(",", strTemp1));
                        index++;

                    }
                    strResult.AppendLine();
                    strResult.AppendLine();
                }
            }
            else
                addSPMI = false;

            if (rffeconfig!=null)
            {
                if (RFFEResults.FrameCollection == null)
                {
                    //MessageBox.Show("RFFE Result not found", "Empty Collection", MessageBoxButton.OK, MessageBoxImage.Information);
                    addRFFE = false;
                }
                if (RFFEResults.FrameCollection.Count == 0)
                {
                    //MessageBox.Show("RFFE Result not found", "Empty Collection", MessageBoxButton.OK, MessageBoxImage.Information);
                    addRFFE = false;
                }

                else
                {
                    int index = 1;
                    StringBuilder strRFFE = new StringBuilder();
                    strRFFE.Append("RFFE PROTOCOL");
                    strRFFE.AppendLine();
                    strRFFE.AppendLine();
                    strRFFE.Append("Index,");
                    strRFFE.Append("Time,");
                    strRFFE.Append("Frame,");
                    strRFFE.Append("Slave/MID,");
                    strRFFE.Append("Command,");
                    strRFFE.Append("Reg Address,");
                    strRFFE.Append("Data,");
                    strRFFE.Append("ByteCount,");
                    strRFFE.Append("Frequency,");
                    strRFFE.Append("Error,");
                    strResult.AppendLine(String.Join(",", strRFFE));
                    foreach (var frame in RFFEResults.FrameCollection)
                    {
                        // int index = 1;
                        String[] strTemp1 = new String[11];
                        strTemp1[0] = index.ToString();

                        if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.RFFE)
                        {
                            if (frame.FrameIndex == RFFEResults.TriggerPacket)
                            {
                                strTemp1[0] = "T    " + strTemp1[0];
                            }
                        }
                        strTemp1[1] = CUtilities.ToEngineeringNotation(frame.StartTime - RFFEResults.TriggerTime).Replace(",", "").ToString() + "s";
                        strTemp1[2] = "RFFE";

                        strTemp1[3] = (frame.SlaveId == -1) ? "-," : ("0x" + frame.SlaveId.ToString("X2"));
                        strTemp1[4] = frame.Command.CmdType.ToString();
                        strTemp1[5] = "0x" + frame.IntAddress.ToString("X2");
                        strTemp1[6] = frame.Data == null ? "-" : String.Join(" ", frame.Data.Select(obj => String.Format("0x{0:X}", obj.Value)));
                        strTemp1[7] = (frame.ByteCount == -1) ? "-" : ("0x" + frame.ByteCount.ToString("X2"));
                        strTemp1[8] = CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz";
                        strTemp1[9] = frame.ErrorType.ToString();

                        strResult.AppendLine(String.Join(",", strTemp1));

                        index++;
                    }
                    strResult.AppendLine();
                    strResult.AppendLine();
                }
            }
            else
                addRFFE = false;

            if (i3cConfig != null)
            {
                if (I3CResults.FrameCollection == null)
                {
                    //MessageBox.Show("I3C Result not found", "Empty Collection", MessageBoxButton.OK, MessageBoxImage.Information);
                    addI3C = false;
                }
                if (I3CResults.FrameCollection.Count == 0)
                {
                    //MessageBox.Show("I3C Result not found", "Empty Collection", MessageBoxButton.OK, MessageBoxImage.Information);
                    addI3C = false;
                }
                else
                {
                    int index = 1;
                    StringBuilder strI3C = new StringBuilder();
                    strI3C.Append("I3C PROTOCOL");
                    strI3C.AppendLine();
                    strI3C.AppendLine();
                    strI3C.Append("index,");
                    strI3C.Append("Time,");
                    strI3C.Append("Frame,");
                    strI3C.Append("Message,");
                    strI3C.Append("Address,");
                    strI3C.Append("Command,");
                    strI3C.Append("Data,");
                    strI3C.Append("Frequency,");
                    strI3C.Append("Error,");
                    strResult.AppendLine(String.Join(",", strI3C));

                    foreach (var frame in I3CResults.FrameCollection)
                    {
                        //int index = 1;
                        String[] strTemp1 = new String[11];
                        strTemp1[0] = index.ToString();

                        if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.I3C)
                        {
                            if (frame.FrameIndex == I3CResults.TriggerPacket)
                            {
                                strTemp1[0] = "T    " + strTemp1[0];
                            }
                        }
                        strTemp1[1] = CUtilities.ToEngineeringNotation(frame.StartTime - I3CResults.TriggerTime).Replace(",", "").ToString() + "s";
                        strTemp1[2] = "I3C";

                        strTemp1[3] = frame.FrameType.ToString();
                        // strTemp1.Append(resultI3C[frame.SaFrameType + ",");
                        StringBuilder command = new StringBuilder();
                        StringBuilder address = new StringBuilder();
                        StringBuilder data = new StringBuilder();

                        foreach (var item in frame.PacketCollection)
                        {
                            if (item is CommandMessageModel && item.PacketType == ePacketType.Command)
                                command.Append(ProtocolInfoRepository.GetCommand(((CommandMessageModel)item).Value).ToString());
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

                        strTemp1[4] = address.ToString();
                        strTemp1[5] = command.ToString();
                        strTemp1[6] = data.ToString();
                        strTemp1[7] = (CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz");
                        strTemp1[8] = (frame.ErrorType.ToString());
                        strResult.AppendLine(String.Join(",", strTemp1));
                        index++;

                    }
                    strResult.AppendLine();
                    strResult.AppendLine();
                }
            }
            else
                addI3C = false;
            if (canconfig!=null)
            {
                if (CANResults.CANFrameCollection == null)
                {
                    //MessageBox.Show("UART Result not found", "Empty Collection", MessageBoxButton.OK, MessageBoxImage.Information);
                    addcan = false;
                }
                if (CANResults.CANFrameCollection.Count == 0)
                {
                    //MessageBox.Show("UART Result not found", "Empty Collection", MessageBoxButton.OK, MessageBoxImage.Information);
                    addcan = false;
                }
                else
                {
                    int index = 1;
                    StringBuilder strcan = new StringBuilder();
                    strcan.Append("CAN PROTOCOL");
                    strcan.AppendLine();
                    strcan.AppendLine();
                    strcan.Append("Index,");//0
                    strcan.Append("Time,");//1
              
                    strcan.Append("FrameType,");//2
                    strcan.Append("FrameFormat,");//3
                    strcan.Append("ID,");//4
                    strcan.Append("IDE,");//5
                    strcan.Append("DLC,");//6
                    strcan.Append("Data,");//7
                    strcan.Append("CRC,");//8
                    if(canconfig.BRS)
                    {
                        strcan.Append("Freq/BRSFreq,");//9
                    }
                    else

                    {
                        strcan.Append("Frequency,");//9
                    }
                  
                    strcan.Append("ErrorType,");//10
                    strResult.AppendLine(String.Join(",", strcan));
                    foreach (var frame in CANResults.CANFrameCollection)
                    {

                        String[] strTemp1 = new String[11];
                        strTemp1[0] = index.ToString();
                        if (SessionConfiguration.TriggerProtcolset && TriggerModel.GetInstance().ProtocolTypeIndex == eProtocolTypeList.CAN)
                        {
                            if (frame.FrameIndex == CANResults.TriggerPacket)
                            {
                                strTemp1[0] = "T    " + strTemp1[0];
                            }
                        }

                        strTemp1[1] = CUtilities.ToEngineeringNotation(frame.StartTime - CANResults.TriggerTime).Replace(",", "").ToString() + "s";
                        strTemp1[2]=("CAN");
                        strTemp1[3]= frame.Eframeformates.ToString();
                        strTemp1[4]=(((frame.IDDdataBytes == -1) ? "-," : ("0x" + frame.IDDdataBytes.ToString("X2") )));
                        strTemp1[5]=(((frame.IDEDdataBytes == -1) ? "-," : ("0x" + frame.IDEDdataBytes.ToString("X2") )));
                        strTemp1[6]=(((frame.DLCDdataBytes == -1) ? "-," : ("0x" +frame.DLCDdataBytes.ToString("X2") )));

                        string datStr1 = "";
                        foreach (var dat in frame.DataBytes)
                        {
                            datStr1 += "0x" + dat.ToString("X2");
                            datStr1 += " : ";
                        }
                        if (datStr1 == "")
                            datStr1 = "-";
                        else
                            datStr1 = datStr1.Substring(0, datStr1.Length - 2);
                       // strTemp1[6] = (datStr1);

                      
                        strTemp1[7]=(((frame.DLCDdataBytes == 0) ? "-," : datStr1 ));
                        strTemp1[8]=((frame.CRCDdataBytes == -1) ? "-," : ("0x" + frame.CRCDdataBytes.ToString("X2") ));
                        datStr1 = "";
                        strTemp1[9]=(CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz" );
                     
                        if (canconfig.BRS)
                        {
                           // strTemp.Append(CUtilities.FormatNumber(resultCAN[frame.Sample].Frequency, Units.BASE_UNIT).Replace("M", "") + "/" + CUtilities.FormatNumber(resultCAN[frame.Sample].BRSFrequency, Units.BASE_UNIT) + "Hz" + ",");
                            strTemp1[9] = (CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT).Replace("M", "") + "/" + CUtilities.FormatNumber(frame.BRSFrequency, Units.BASE_UNIT) + "Hz" + ",");
                        }


                        else
                        {
                            strTemp1[9] = (CUtilities.FormatNumber(frame.Frequency, Units.BASE_UNIT) + "Hz");
                        }

                        strTemp1[10]=frame.ErrorType.ToString();

                        strResult.AppendLine(String.Join(",", strTemp1));
                        index++;

                    }
                    strResult.AppendLine();
                    strResult.AppendLine();
                
                    }
                }
            else
                addcan = false;

            return eResponseFlag.Success;
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



        private void AddWaveformListing(ref StringBuilder strResult)
        {        
            String[] str = new String[20];
            int k = 0;
            str[k] = "S No";
            k++;
            str[k] = "TIMESTAMP";
            k++;
                str[k] = "CH16";
                k++;
          
                str[k] = "CH15";
                k++;
          
                str[k] = "CH14";
                k++;
                str[k] = "CH13";
                k++;
          
                str[k] = "CH12";
                k++;
          
                str[k] = "CH11";
                k++;
           
                str[k] = "CH10";
                k++;
           
                str[k] = "CH9";
                k++;
          
                str[k] ="CH8";
                k++;
           
                str[k] = "CH7";
                k++;
           
                str[k] = "CH6";
                k++;
           
                str[k] ="CH5";
                k++;
            
                str[k] ="CH4";
                k++;
          
                str[k] = "CH3";
                k++;
             str[k] ="CH2";
                k++;
           
                str[k] = "CH1";
            
            strResult.AppendLine(String.Join(",", str));

            int index = 1;
            startIndex = 0;
            var dataReceiver = Ioc.Default.GetService<IDataProvider>();
            //stopIndex = result.Count - 1 > 10000000 ? 10000000 : result.Count - 1;
            //stopIndex = (int)dataReceiver.TotalWaveformPoints;// need to check for limit.

            List<DiscreteWaveForm> resul = Ioc.Default.GetService<IDataProvider>().GetWaveform(startIndex, stopIndex);
            for (int i = startIndex; i < stopIndex; i++)
            {
                String[] strTemp = new String[20];

                int j = 0;
                strTemp[j] = index.ToString();
                if (SessionConfiguration.TriggersetTView && SessionConfiguration.TriggerTime > 0)
                {
                    if (index == SessionConfiguration.TriggerIndex)
                    {
                        strTemp[j] = "T    " + strTemp[j];
                    }

                }
                j++;

                strTemp[j] = CUtilities.ToEngineeringNotation(resul[i].TimeStamp - SessionConfiguration.TriggerTime).ToString() + "s";
                j++;


                strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch16").ToString();
                    j++;
                
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch15").ToString();
                    j++;
                
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch14").ToString();
                    j++;
               
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch13").ToString();
                    j++;
              
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch12").ToString();
                    j++;
               
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch11").ToString();
                    j++;
              
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch10").ToString();
                    j++;
              
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch9").ToString();
                    j++;
                
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch8").ToString();
                    j++;
                
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch7").ToString();
                    j++;
              
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch6").ToString();
                    j++;
              
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch5").ToString();
                    j++;
               
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch4").ToString();
                    j++;
                
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch3").ToString();
                    j++;
              
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch2").ToString();
                    j++;
              
                    strTemp[j] = GetWaveformChannelByte(resul[i].ChannelValue, "Ch1").ToString();
              
                strResult.AppendLine(String.Join(",", strTemp));
                index++;
            }
        }

        private string GetVolt(DataModule.eVoltage volt)
        {
            string voltStr = "";
            switch (volt)
            {
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

        bool IsFileLocked(FileInfo file)
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
        #endregion


    }
}

