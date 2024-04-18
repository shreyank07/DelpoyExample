using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Prodigy.Business;
using Prodigy.WaveformControls;
using Prodigy.WaveformControls.Interfaces;
using Prodigy.WaveformControls.View;
using ProdigyFramework.Collections;
using ProdigyFramework.ComponentModel;
using ProdigyFramework.Extension;

namespace PGYMiniCooper.DataModule.Model
{
    /// <summary>
    /// This is the class that contains the plotting information and plots
    /// </summary>
    public class WfmPlotViewModel : ViewModelBase
    {
        private RelayCommand<IPlotInfoView> enableCommand;
        private RelayCommand initializeCommand;
        private RelayCommand<IPlotInfoView> removeCommand;
        private RelayCommand<string> cursorSelectionCommand;
        private int marker = 0;
        private RelayCommand<IPlotInfoView> markerSelectionCommand;
        //private RelayCommand<System.Windows.DragEventArgs> dropCommand;
        private bool horizontalCursorON;
        private double horizontalCursor1Position;
        private double horizontalCursor2Position;
        private bool verticalCursorON;
        private double verticalCursor1Position;
        private double verticalCursor2Position;
        private double wfmShowingStartIndex;
        private double wfmShowingStopIndex;
        private double maximumIndex;
        private string tag;
        private CursorEnum cursorType;
        private ObservableCollection<IPlotInfoView> plotCollection;
        private IPlotInfoView selectedPlot;
        private IPlotInfoView busResultInfoView;
        private bool markerCursor1ON;
        private bool markerCursor2ON;
        private bool markerCursor3ON;
        private bool markerCursor4ON;
        private double markerCursor1Position;
        private double markerCursor2Position;
        private double markerCursor3Position;
        private double markerCursor4Position;
        private PlotEvents plotEvent;

        private double minimumIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="WfmPlotViewModel" /> class.
        /// </summary>
        public WfmPlotViewModel()
        {
            this.plotEvent = PlotEvents.NONE;
            this.plotCollection = new ObservableCollection<IPlotInfoView>();
            this.CursorType = CursorEnum.OFF;
            this.IsInitialized = false;
            this.tag = string.Empty;
            markerName = false;
            isTVisible = false;
        }

        private bool isTVisible;
        public bool IsTVisible
        {
            get
            {
                return isTVisible;
            }
            set
            {
                isTVisible = value;
                RaisePropertyChanged("IsTVisible");
            }
        }
        private bool markerName;
        public bool IsMarkername
        {
            get
            {
                return markerName;
            }
            set
            {
                markerName = value;
                RaisePropertyChanged("IsMarkername");
            }
        }

        #region Relay Commands


        /// <summary>
        /// Gets the initialize command.
        /// </summary>
        /// <value>The initialize command.</value>
        public RelayCommand InitializeCommand
        {
            get
            {
                return this.initializeCommand ??
                       (this.initializeCommand = new RelayCommand(
                           () =>
                           {
                               this.IsInitialized = true;
                           }));
            }
        }

        /// <summary>
        /// Gets the remove command.
        /// </summary>
        /// <value>The remove command.</value>
        public RelayCommand<IPlotInfoView> RemoveCommand
        {
            get
            {
                return this.removeCommand ??
                       (this.removeCommand = new RelayCommand<IPlotInfoView>(
                           (plot) =>
                           {
                               if (this.PlotCollection.Any(p => p.Equals(plot)))
                               {
                                   this.PlotCollection.First(p => p.Equals(plot)).Remove();
                               }
                           }));
            }
        }

        /// <summary>
        /// Gets the enable command.
        /// </summary>
        /// <value>The enable command.</value>
        public RelayCommand<IPlotInfoView> EnableCommand
        {
            get
            {
                return this.enableCommand ??
                       (this.enableCommand = new RelayCommand<IPlotInfoView>(
                           (plot) =>
                           {
                               plot.IsShowing = !plot.IsShowing;
                           }));
            }
        }

        /// <summary>
        /// Gets the cursor selection command.
        /// </summary>
        /// <value>The cursor selection command.</value>
        public RelayCommand<string> CursorSelectionCommand
        {
            get
            {
                return this.cursorSelectionCommand ??
                       (this.cursorSelectionCommand = new RelayCommand<string>(
                           (strCursorType) =>
                           {
                               this.CursorType = (CursorEnum)Enum.Parse(typeof(CursorEnum), strCursorType);
                               if (this.CursorType == CursorEnum.H_BAR)
                               {
                                   if (this.HCursorON)
                                   {
                                       this.HCursorON = false;
                                   }
                                   else
                                   {
                                       this.HCursorON = true;
                                   }
                               }
                               else if (this.CursorType == CursorEnum.V_BAR)
                               {
                                   if (this.VCursorON)
                                   {
                                       this.VCursorON = false;
                                   }
                                   else
                                   {
                                       this.VCursorON = true;
                                   }
                               }
                               else
                               {
                                   this.VCursorON = false;
                                   this.HCursorON = false;
                               }
                           }));
            }
        }

        /// <summary>
        /// Gets the marker selection command.
        /// </summary>
        /// <value>The marker selection command.</value>
        public RelayCommand<IPlotInfoView> MarkerSelectionCommand
        {
            get
            {
                return this.markerSelectionCommand ??
                       (this.markerSelectionCommand = new RelayCommand<IPlotInfoView>(
                           (plot) =>
                           {
                               if (plot != null)
                               {
                                   switch (this.marker)
                                   {
                                       case 1:
                                           this.MCursor1Position = plot.StartWfmIndex + ((plot.StopWfmIndex - plot.StartWfmIndex) * 1.2d / 3d);
                                           break;
                                       case 2:
                                           this.MCursor2Position = plot.StartWfmIndex + ((plot.StopWfmIndex - plot.StartWfmIndex) * 1.4d / 3d);
                                           break;
                                       case 3:
                                           this.MCursor3Position = plot.StartWfmIndex + ((plot.StopWfmIndex - plot.StartWfmIndex) * 1.6d / 3d);
                                           break;
                                       case 4:
                                           this.MCursor4Position = plot.StartWfmIndex + ((plot.StopWfmIndex - plot.StartWfmIndex) * 1.8d / 3d);
                                           break;
                                   }
                                   if (MCursor1ON == false && MCursor2ON == false && MCursor3ON == false && MCursor4ON == false)
                                       IsMarkername = false;
                                   else
                                       IsMarkername = true;
                               }
                           }));
            }
        }

        /// <summary>
        /// Gets the drop command.
        /// </summary>
        /// <value>The drop command.</value>
        //public RelayCommand<System.Windows.DragEventArgs> DropCommand
        //{
        //    get
        //    {
        //        return this.dropCommand ??
        //               (this.dropCommand = new RelayCommand<System.Windows.DragEventArgs>(
        //                   (e) =>
        //                   {
        //                       // TODO: Cleanup and optimization needed
        //                       // If an element in the panel has already handled the drop, 
        //                       // the panel should not also handle it. 
        //                       if (e.Handled == false)
        //                       {
        //                           IPlotInfoView element = (IPlotInfoView)e.Data.GetData(typeof(IPlotInfoView));

        //                           if (element != null)
        //                           {
        //                               this.AddWaveform(element);
        //                               e.Handled = true;
        //                           }
        //                       }
        //                   }));
        //    }
        //}

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the Horizontal cursor is ON.
        /// </summary>
        /// <value>The H cursor ON.</value>
        public bool HCursorON
        {
            get
            {
                return this.horizontalCursorON;
            }
            set
            {
                this.horizontalCursorON = value;
                if (value && this.SelectedPlot != null)
                {
                    //SelectedPlot = PlotCollection.Where(x => x.Channel == Prodigy.Business.WfmEnum.CH7).FirstOrDefault();
                   // this.HCursor1Position = this.SelectedPlot.VerticalScale * 0.4d + ((TrendViewModel)this.SelectedPlot).Min;
                    //this.HCursor2Position = this.SelectedPlot.VerticalScale * 0.6d + ((TrendViewModel)this.SelectedPlot).Min;
                }
                this.RaisePropertyChanged("HCursorON");
            }
        }

        /// <summary>
        /// Gets or sets the Horizontal cursor1 position.
        /// </summary>
        /// <value>The Horizontal cursor1 position.</value>
        public double HCursor1Position
        {
            get
            {
                return this.horizontalCursor1Position;
            }
            set
            {
                this.horizontalCursor1Position = value;
                this.RaisePropertyChanged("HCursor1Position");
            }
        }

        /// <summary>
        /// Gets or sets the H cursor2 position.
        /// </summary>
        /// <value>The H cursor2 position.</value>
        public double HCursor2Position
        {
            get
            {
                return this.horizontalCursor2Position;
            }
            set
            {
                this.horizontalCursor2Position = value;
                this.RaisePropertyChanged("HCursor2Position");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Vertical cursor ON.
        /// </summary>
        /// <value>The Vertical cursor ON.</value>
        public bool VCursorON
        {
            get
            {
                return this.verticalCursorON;
            }
            set
            {
                this.verticalCursorON = value;
                if (value)
                {
                    this.VCursor1Position = this.WfmShowingStartIndex + ((this.WfmShowingStopIndex - this.WfmShowingStartIndex) / 3d);
                    this.VCursor2Position = this.WfmShowingStartIndex + ((this.WfmShowingStopIndex - this.WfmShowingStartIndex) / 1.5);
                    //IsMarkername = true;
                }
                //if (value == false && MCursor1ON == false && MCursor2ON == false && MCursor3ON == false && MCursor4ON == false)
                  //  IsMarkername = false;
                this.RaisePropertyChanged("VCursorON");
            }
        }

        /// <summary>
        /// Gets or sets the V cursor1 position.
        /// </summary>
        /// <value>The V cursor1 position.</value>
        public double VCursor1Position
        {
            get
            {
                return this.verticalCursor1Position;
            }
            set
            {
                this.verticalCursor1Position = value;
                this.RaisePropertyChanged("VCursor1Position");
            }
        }

        /// <summary>
        /// Gets or sets the V cursor2 position.
        /// </summary>
        /// <value>The V cursor2 position.</value>
        public double VCursor2Position
        {
            get
            {
                return this.verticalCursor2Position;
            }
            set
            {
                this.verticalCursor2Position = value;
                this.RaisePropertyChanged("VCursor2Position");
            }
        }

        /// <summary>
        /// Gets or sets the start index of the WFM showing.
        /// </summary>
        /// <value>The start index of the WFM showing.</value>
        public double WfmShowingStartIndex
        {
            get
            {
                return this.wfmShowingStartIndex;
            }
            set
            {
                this.wfmShowingStartIndex = value;
                this.RaisePropertyChanged("WfmShowingStartIndex");
            }
        }

        /// <summary>
        /// Gets or sets the index of the WFM showing stop.
        /// </summary>
        /// <value>The index of the WFM showing stop.</value>
        public double WfmShowingStopIndex
        {
            get
            {
                return this.wfmShowingStopIndex;
            }
            set
            {
                this.wfmShowingStopIndex = value;
                this.RaisePropertyChanged("WfmShowingStopIndex");
            }
        }

        /// <summary>
        /// Gets or sets the maximum index.
        /// </summary>
        /// <value>The maximum index.</value>
        public double MaximumIndex
        {
            get
            {
                return this.maximumIndex;
            }
            set
            {
                this.maximumIndex = value;
                this.RaisePropertyChanged("MaximumIndex");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the is initialized.
        /// </summary>
        /// <value>The is initialized.</value>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public string Tag
        {
            get
            {
                return this.tag;
            }
            set
            {
                this.tag = value;
                this.RaisePropertyChanged("Tag");
            }
        }

        /// <summary>
        /// Gets or sets the type of the cursor.
        /// </summary>
        /// <value>The type of the cursor.</value>
        public CursorEnum CursorType
        {
            get
            {
                return this.cursorType;
            }
            set
            {
                this.cursorType = value;
                this.RaisePropertyChanged("CursorType");
            }
        }

        /// <summary>
        /// Gets or sets the plot collection.
        /// </summary>
        /// <value>The plot collection.</value>
        public ObservableCollection<IPlotInfoView> PlotCollection
        {
            get
            {
                return this.plotCollection;
            }
            set
            {
                this.plotCollection = value;
                this.RaisePropertyChanged("PlotCollection");
            }
        }

        /// <summary>
        /// Gets or sets the selected plot.
        /// </summary>
        /// <value>The selected plot.</value>
        public IPlotInfoView SelectedPlot
        {
            get
            {
                return this.selectedPlot;
            }
            set
            {
                this.selectedPlot = value;
                this.RaisePropertyChanged("SelectedPlot");
            }
        }

        /// <summary>
        /// Gets or sets the bus result info view.
        /// </summary>
        /// <value>The bus result info view.</value>
        public IPlotInfoView BusResultInfoView
        {
            get
            {
                return this.busResultInfoView;
            }
            set
            {
                this.busResultInfoView = value;
                if (value != null)
                {
                    if (value.IsShowing || this.PlotCollection.Any((plot) => plot.Channel == value.Channel))
                    {
                        IPlotInfoView removePlot = this.PlotCollection.First((plot) => plot.Channel == value.Channel);
                        removePlot.Remove();
                        removePlot.OnRemove += new RemovePlotEventHandler(this.Remove);
                        this.Remove(this, new Prodigy.WaveformControls.RemovePlotEventArgs(removePlot));
                    }
                    this.PlotCollection.Add(value);
                }
                this.RaisePropertyChanged("BusResultInfoView");
            }
        }

        #region Markers definition

        /// <summary>
        /// Gets or sets a value indicating whether the M cursor1 ON.
        /// </summary>
        /// <value>The M cursor1 ON.</value>
        public bool MCursor1ON
        {
            get
            {
                return this.markerCursor1ON;
            }
            set
            {
                if (value)
                {
                    this.marker = 1;
                    IsMarkername = true;
                }
                this.markerCursor1ON = value;
                this.RaisePropertyChanged("MCursor1ON");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the M cursor2 ON.
        /// </summary>
        /// <value>The M cursor2 ON.</value>
        public bool MCursor2ON
        {
            get
            {
                return this.markerCursor2ON;
            }
            set
            {
                if (value)
                {
                    this.marker = 2;
                    IsMarkername = true;
                }
                this.markerCursor2ON = value;
                this.RaisePropertyChanged("MCursor2ON");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the M cursor3 ON.
        /// </summary>
        /// <value>The M cursor3 ON.</value>
        public bool MCursor3ON
        {
            get
            {
                return this.markerCursor3ON;
            }
            set
            {
                if (value)
                {
                    this.marker = 3;
                    IsMarkername = true;
                }
                this.markerCursor3ON = value;
                this.RaisePropertyChanged("MCursor3ON");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the M cursor4 ON.
        /// </summary>
        /// <value>The M cursor4 ON.</value>
        public bool MCursor4ON
        {
            get
            {
                return this.markerCursor4ON;
            }
            set
            {
                if (value)
                {
                    this.marker = 4;
                    IsMarkername = true;
                }
                this.markerCursor4ON = value;
                this.RaisePropertyChanged("MCursor4ON");
            }
        }

        /// <summary>
        /// Gets or sets the M cursor1 position.
        /// </summary>
        /// <value>The M cursor1 position.</value>
        public double MCursor1Position
        {
            get
            {
                return this.markerCursor1Position;
            }
            set
            {
                this.markerCursor1Position = value;
                this.RaisePropertyChanged("MCursor1Position");
            }
        }

        /// <summary>
        /// Gets or sets the M cursor2 position.
        /// </summary>
        /// <value>The M cursor2 position.</value>
        public double MCursor2Position
        {
            get
            {
                return this.markerCursor2Position;
            }
            set
            {
                this.markerCursor2Position = value;
                this.RaisePropertyChanged("MCursor2Position");
            }
        }

        /// <summary>
        /// Gets or sets the M cursor3 position.
        /// </summary>
        /// <value>The M cursor3 position.</value>
        public double MCursor3Position
        {
            get
            {
                return this.markerCursor3Position;
            }
            set
            {
                this.markerCursor3Position = value;
                this.RaisePropertyChanged("MCursor3Position");
            }
        }

        /// <summary>
        /// Gets or sets the M cursor4 position.
        /// </summary>
        /// <value>The M cursor4 position.</value>
        public double MCursor4Position
        {
            get
            {
                return this.markerCursor4Position;
            }
            set
            {
                this.markerCursor4Position = value;
                this.RaisePropertyChanged("MCursor4Position");
            }
        }

        #endregion

        #endregion
      
        /// <summary>
        /// Gets or sets the plot event.
        /// </summary>
        /// <value>The plot event.</value>
        public PlotEvents PlotEvent
        {
            get
            {
                return this.plotEvent;
            }
            set
            {
                this.plotEvent = value;
                MouseEventExtension.MouseEvent(MouseEventExtension.MouseEventFlags.LeftDown);
                MouseEventExtension.MouseEvent(MouseEventExtension.MouseEventFlags.LeftUp);
                this.RaisePropertyChanged("PlotEvent");
            }
        }

        private double triggerPosition;

        public double TriggerPosition
        {
            get { return this.triggerPosition; } 
            set
            {
                this.triggerPosition = value;
                RaisePropertyChanged(nameof(TriggerPosition));
            }
        }

        private double sampleRate = 1000000000;
        public double SampleRate
        {
            get
            {
                return sampleRate;
            }
            set
            {
                sampleRate = value;
                RaisePropertyChanged("SampleRate");
            }
        }

        #region Methods

        /// <summary>
        /// Adds the waveform.
        /// </summary>
        /// <param name="wfm">The WFM.</param>
        public void AddWaveform(IPlotInfoView wfm)
        {
            if (wfm.IsShowing || this.PlotCollection.Any((plot) => plot.Channel == wfm.Channel))
            {
                IPlotInfoView removePlot = this.PlotCollection.First((plot) => plot.Channel == wfm.Channel);
                removePlot.Remove();
                //this.PlotCollection.Remove(removePlot);
                removePlot.OnRemove += new RemovePlotEventHandler(this.Remove);
                this.Remove(this, new Prodigy.WaveformControls.RemovePlotEventArgs(removePlot));
            }


            //this.WfmShowingStartIndex = (long)(wfm.StartWfmIndex);
            //this.WfmShowingStopIndex = (long)(wfm.WfmLength);

            //this.MinimumIndex = (long)(wfm.StartWfmIndex);
            //this.MaximumIndex = (long)(wfm.WfmLength);

            this.PlotCollection.Add(wfm);

            if (this.SelectedPlot == null)
            {
                this.SelectedPlot = wfm;
            }
        }


        /// <summary>
        /// Removes the specified plot info view.
        /// </summary>
        /// <param name="plotInfoView">The plot info view.</param>
        public void Remove(object sender, RemovePlotEventArgs e)
        {
            try
            {
                //if (e.Plot is DigitalPlotViewModel)
                {
                    if (this.PlotCollection.Any(wfm => object.ReferenceEquals(wfm, e.Plot)))
                    {
                        this.PlotCollection.Remove(e.Plot);
                    }
                }
                //else if (e.Plot is ProtocolBusPlotViewModel)
                //{
                //    if (this.PlotCollection.Any(wfm => object.ReferenceEquals(wfm, e.Plot)))
                //    {
                //        this.PlotCollection.Remove(e.Plot);
                //    }
                //}
            }
            catch (Exception ex)
            {
                // TODO: add proper message dialog handling
                Ioc.Default.GetService<IMessenger>().Send(ex);
            }
        }

        #endregion

        private bool isDirty = false;
        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                isDirty = value;
                RaisePropertyChanged("IsDirty");
            }
        }

        private bool displayLimited = true;
        public bool DisplayLimited
        {
            get { return displayLimited; }
            set
            {
                displayLimited = value;
                RaisePropertyChanged("DisplayLimited");
            }
        }

        public double MinimumIndex
        {
            get
            {
                return this.minimumIndex;
            }
            set
            {
                this.minimumIndex = value;
                RaisePropertyChanged("MinimumIndex");
            }
        }

        private double horizontalZoomLimit;

        public double HorizontalZoomLimit
        {
            get { return this.horizontalZoomLimit; }
            set
            {
                this.horizontalZoomLimit = value;
                RaisePropertyChanged(nameof(HorizontalZoomLimit));
            }
        }
    }
}
