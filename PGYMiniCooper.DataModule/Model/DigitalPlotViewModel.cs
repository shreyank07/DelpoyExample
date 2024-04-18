
using PGYMiniCooper.DataModule.Structure;
using Prodigy.Business;
using Prodigy.WaveformControls;
using Prodigy.WaveformControls.Interfaces;
using ProdigyFramework.Collections;
using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Model
{

    /// <summary>
    /// Waveform plot Class
    /// </summary>
    public class DigitalPlotViewModel : ViewModelBase, Prodigy.WaveformControls.Interfaces.IPlotInfoView
    {
        private double currentVal = 0;
        private readonly double startTime;
        private readonly long totalTime;

        private readonly WfmEnum wfmType;

        private double triggerPosition;

        private double verticalScale;
        private double verticalOffset;
        private double cursor1Position;
        private double cursor2Position;
        private bool isEnabled;

        private TypeOfChart chartType;

        private IList<System.Windows.Point> point;

        private List<System.Windows.Point> wfmCompletePoints = new List<System.Windows.Point>();

        private bool processing = false;

        private double previousStartWfmIndex;
        private double previousStopWfmIndex;
        private double startWfmIndex;
        private double stopWfmIndex;

        private bool isShowing;

        private string _tag;

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalPlotViewModel" /> class.
        /// </summary>
        public DigitalPlotViewModel()
        {
            this.StartWfmIndex = 0;
            this.StopWfmIndex = this.WfmLength;
            this.verticalOffset = 0;
            this.verticalScale = 1d;
            this.chartType = TypeOfChart.SINE;
            _tag = Channel.ToString();
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalPlotViewModel" /> class.
        /// </summary>
        /// <param name="wfm">The WFM.</param>
        public DigitalPlotViewModel(WfmEnum channel,
            long starttimeInNanoSecond,
            long stopTimeInNanoSecond,
            eChannles customChannel,
            int channelIndex,
            GetPointsDataHandler pointsMethod)
        {
            this.wfmType = channel;
            this.CustomChannel = customChannel;
            this.ChannelIndex = channelIndex;
            this.GetPointsData = pointsMethod;

            this.isEnabled = false;
            this.startTime = this.startWfmIndex = this.StartWfmIndex = starttimeInNanoSecond;
            this.totalTime = (long)(this.stopWfmIndex = this.StopWfmIndex = stopTimeInNanoSecond);

            ////default offset center screen
            ////_VerticalOffset = -wfm.MaxValue + (wfm.MaxValue - wfm.MinValue) / 2d;
            this.verticalOffset = 0;

            ////default scale waveform in 2 divisions
            ////_VerticalScale = (wfm.MaxValue - wfm.MinValue) / 2d;
            this.verticalScale = 1;

            //if (this.WfmCompletePoints == null || this.WfmCompletePoints.Count == 0)
            {
                this.startWfmIndex = this.startTime;
                this.stopWfmIndex = this.totalTime;
                //this.WfmCompletePoints = this.GetPointsData(CustomChannel, ChannelIndex).ToList();
            }

            this.chartType = TypeOfChart.RECTANGULAR;
            //this.chartType = TypeOfChart.RECTANGULAR;

            _tag = Channel.ToString();
        }

        /// <summary>
        /// Channel Name description
        /// </summary>
        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
                this.RaisePropertyChanged("Tag");
            }
        }

        public delegate List<System.Windows.Point> GetPointsDataHandler(eChannles channel, int channelIndex, double startIndex, double stopIndex);

        public event GetPointsDataHandler GetPointsData;

        /// <summary>
        /// Occurs when [on remove].
        /// </summary>
        public event RemovePlotEventHandler OnRemove;

        /// <summary>
        /// Byte Information
        /// </summary>
        public enum ByteFormat
        {
            /// <summary>
            /// Represents 1 byte per point
            /// </summary>
            Byte1Format = 1,

            /// <summary>
            /// Represents 2 byte per point
            /// </summary>
            Byte2Format = 2,

            /// <summary>
            /// Represents 4 byte per point
            /// </summary>
            Byte4Format = 4
        }

        /// <summary>
        /// Gets or sets the V cursor1 position.
        /// </summary>
        /// <value>The V cursor1 position.</value>
        public double VCursor1Position
        {
            get
            {
                return this.cursor1Position;
            }
            set
            {
                this.cursor1Position = value;
                this.RaisePropertyChanged("Cursor1Position");
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
                return this.cursor2Position;
            }
            set
            {
                this.cursor2Position = value;
                this.RaisePropertyChanged("Cursor2Position");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the 
        /// Plot is shown
        /// </summary>
        /// <value>The is showing.</value>
        public bool IsShowing
        {
            get
            {
                return this.isShowing;
            }
            set
            {
                this.isShowing = value;
                this.RaisePropertyChanged("IsShowing");
            }
        }

        /// <summary>
        /// Gets or sets the length of the WFM.
        /// </summary>
        /// <value>The length of the WFM.</value>
        public long WfmLength
        {
            get
            {
                return this.totalTime;
            }
            set
            {
                this.RaisePropertyChanged("WfmLength");
            }
        }

        /// <summary>
        /// Gets or sets the start index of the WFM.
        /// </summary>
        /// <value>The start index of the WFM.</value>
        public double StartWfmIndex 
        {
            get { return startWfmIndex; }
            set
            {
                startWfmIndex = value;
                RaisePropertyChanged(nameof(StartWfmIndex));
            }
        }

        /// <summary>
        /// Gets or sets the index of the stop WFM.
        /// </summary>
        /// <value>The index of the stop WFM.</value>
        public double StopWfmIndex
        {
            get { return stopWfmIndex; }
            set
            {
                stopWfmIndex = value;
                RaisePropertyChanged(nameof(StopWfmIndex));
            }
        }

        /// <summary>
        /// Gets or sets the vertical scale.
        /// </summary>
        /// <value>The vertical scale.</value>
        public double VerticalScale
        {
            get
            {
                return this.verticalScale;
            }
            set
            {
                this.verticalScale = value;
                this.RaisePropertyChanged("VerticalScale");
            }
        }

        /// <summary>
        /// Gets or sets the vertical offset.
        /// </summary>
        /// <value>The vertical offset.</value>
        public double VerticalOffset
        {
            get
            {
                return this.verticalOffset;
            }
            set
            {
                this.verticalOffset = value;
                this.RaisePropertyChanged("VerticalOffset");
            }
        }

        /// <summary>
        /// Gets or sets the point.
        /// </summary>
        /// <value>The point.</value>
        public IList<System.Windows.Point> Point
        {
            get
            {
                return this.point;
            }
            set
            {
                this.point = value;
                this.RaisePropertyChanged("Point");
            }
        }

        /// <summary>
        /// Gets or sets the WFM complete points.
        /// </summary>
        /// <value>The WFM complete points.</value>
        public List<System.Windows.Point> WfmCompletePoints
        {
            get
            {
                return this.wfmCompletePoints;
            }
            set
            {
                this.wfmCompletePoints = value;
                this.RaisePropertyChanged("WfmCompletePoints");
            }
        }

        private bool isDirty = false;

        /// <summary>
        /// Gets or sets a value indicating whether the 
        /// IsDirty On set to true re-evaluates the results and updates them.
        /// </summary>
        /// <value>The is dirty.</value>
        public bool IsDirty
        {
            get
            {
                return isDirty;
            }
            set
            {
                isDirty = false;
                if (value && IsShowing)
                    UpdateWaveformPoints();
            }
        }

        private void UpdateWaveformPoints()
        {
            if (!processing)
            {
                this.processing = true;
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    try
                    {
                        // check if we already have enough samples
                        if ((this.point == null || this.point.Count <= 2) || (this.startWfmIndex < previousStartWfmIndex || this.stopWfmIndex > previousStopWfmIndex) || (this.previousStartWfmIndex == 0 && this.previousStopWfmIndex == 0))
                        {
                            this.previousStartWfmIndex = this.startWfmIndex;
                            this.previousStopWfmIndex = this.stopWfmIndex;

                            // Get Updated points data
                            this.point = this.GetPointsData(CustomChannel, ChannelIndex, this.startWfmIndex, this.StopWfmIndex);
                        }
                    }
                    finally
                    {
                        this.RaisePropertyChanged("Point");
                        this.RaisePropertyChanged("IsDirty");
                        this.processing = false;
                    }
                });
            }
        }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>The channel.</value>
        public WfmEnum Channel
        {
            get
            {
                return this.wfmType;
            }
        }

        private eChannles customchannel;
        public eChannles CustomChannel
        {
            get
            {
                return this.customchannel;
            }
            set
            {
                this.customchannel = value;
            }
        }

        private int channelindex = 0;
        public int ChannelIndex
        {
            get
            {
                return channelindex;
            }
            set
            {
                channelindex = value;
            }
        }
        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        public string Source
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the sample rate.
        /// </summary>
        /// <value>The sample rate.</value>
        public double SampleRate
        {
            get
            {
                return 1E9;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the is cursor visible.
        /// </summary>
        /// <value>The is cursor visible.</value>
        public bool IsCursorVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the  is enabled.
        /// </summary>
        /// <value>The is enabled.</value>
        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                this.isEnabled = value;
                this.RaisePropertyChanged("IsEnabled");
            }
        }

        /// <summary>
        /// Gets the horizontal scale.
        /// </summary>
        /// <value>The horizontal scale.</value>
        public double HorizontalScale
        {
            get
            {
                return this.WfmLength / (10d * this.SampleRate);
            }
        }

        /// <summary>
        /// Gets the type of the chart.
        /// </summary>
        /// <value>The type of the chart.</value>
        public TypeOfChart ChartType
        {
            get
            {
                return this.chartType;
            }
            set
            {
                this.chartType = value;
                this.RaisePropertyChanged("ChartType");
            }
        }

        /// <summary>
        /// Gets the trigger position.
        /// </summary>
        /// <value>The trigger position.</value>
        public double TriggerPosition
        {
            get
            {
                return this.triggerPosition;
            }
            set
            {
                this.triggerPosition = value;
                RaisePropertyChanged("TriggerPosition");
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this.stopWfmIndex = this.WfmLength;

            ////if (startWfmIndex == 0 && stopWfmIndex == WfmLength)
            ////{
            this.WfmCompletePoints = new List<System.Windows.Point>();
            this.Point = this.WfmCompletePoints;
            ////}
            ////else
            ////{
            ////    WfmCompletePoints = GetPointsData(0, WfmLength);
            ////    Datas = GetPointsData();
            ////}
            this.RaisePropertyChanged("WfmLength");
            this.RaisePropertyChanged("HorizontalScale");
            this.RaisePropertyChanged("SampleRate");
            this.RaisePropertyChanged("TriggerPosition");
        }

        /// <summary>
        /// Sets the is showing.
        /// </summary>
        public void SetIsShowing()
        {
            this.IsShowing = true;
        }

        /// <summary>
        /// Removes this instance.
        /// </summary>
        public void Remove()
        {
            this.IsEnabled = false;
            this.IsShowing = false;
            if (this.OnRemove != null)
            {
                this.OnRemove(this, new Prodigy.WaveformControls.RemovePlotEventArgs(this));
                this.OnRemove = null;
            }
        }
    }
}
