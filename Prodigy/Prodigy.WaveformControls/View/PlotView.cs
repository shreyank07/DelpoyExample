// <copyright file="PlotView.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>

namespace Prodigy.WaveformControls.View
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Prodigy.Business;
    using Prodigy.WaveformControls.Extensions;
    using Prodigy.WaveformControls.Viewport;
    using Prodigy.WaveformControls;
    using Prodigy.WaveformControls.Interfaces;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Basic Graph
    /// </summary>
    public class PlotView : Image, IPlot
    {
        public static readonly DependencyProperty ViewportProperty = DependencyProperty.Register("Viewport", typeof(Viewport.Viewport2D), typeof(PlotView),
            new PropertyMetadata(new Viewport.Viewport2D(new Prodigy.WaveformControls.Viewport.ViewportArea(0, 0, 0, 0), new Prodigy.WaveformControls.Viewport.ViewportArea(0, 0, 0, 0))));

        public static DependencyProperty ChannelProperty =
            DependencyProperty.Register("Channel", typeof(WfmEnum), typeof(PlotView),
                new PropertyMetadata(WfmEnum.CH1));

        public static DependencyProperty StartIndexProperty =
            DependencyProperty.Register("StartIndex", typeof(double), typeof(PlotView),
                new PropertyMetadata(0.0, StartIndexPropertyChanged));

        public static DependencyProperty StopIndexProperty =
            DependencyProperty.Register("StopIndex", typeof(double), typeof(PlotView),
                new PropertyMetadata(0.0, StopIndexPropertyChanged));

        public static DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(PlotView),
                new PropertyMetadata(double.NaN, ScalePropertyChanged));

        public static DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(PlotView),
                new PropertyMetadata(0.0, OffsetPropertyChanged));

        public static DependencyProperty DataScaleProperty =
            DependencyProperty.Register("DataScale", typeof(double), typeof(PlotView),
                new PropertyMetadata(1.0, DataScalePropertyChanged));

        public static DependencyProperty DataOffsetProperty =
            DependencyProperty.Register("DataOffset", typeof(double), typeof(PlotView),
                new PropertyMetadata(0.0, DataOffsetPropertyChanged));

        public static DependencyProperty IsDirtyProperty =
            DependencyProperty.Register("IsDirty", typeof(bool), typeof(PlotView),
                new PropertyMetadata(false));

        public static DependencyProperty IsShowingProperty =
            DependencyProperty.Register("IsShowing", typeof(bool), typeof(PlotView),
                new PropertyMetadata(true, IsShowingPropertyChanged));

        public static DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(IEnumerable<Point>), typeof(PlotView),
                new PropertyMetadata(new List<Point>(), PointsPropertyChanged, new CoerceValueCallback(PointsPropertyCoerced)));

        public static DependencyProperty MinIndexProperty =
            DependencyProperty.Register("MinIndex", typeof(double), typeof(PlotView),
                new PropertyMetadata(0d, new PropertyChangedCallback(MinIndexPropertyChanged)));

        public static DependencyProperty MaxIndexProperty =
            DependencyProperty.Register("MaxIndex", typeof(double), typeof(PlotView),
                new PropertyMetadata(0d, new PropertyChangedCallback(MaxIndexPropertyChanged)));

        public double HorizontalZoomLimit = double.NaN;

        internal List<UndoItem> Undo = new List<UndoItem>();

        protected WriteableBitmap bitmap;

        protected bool _IsUndoEnabled = false;

        IEnumerable<Point> points = new List<Point>();

        private Prodigy.WaveformControls.Viewport.ViewportRestrictions _Restriction;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlotView" /> class.
        /// </summary>
        /// <param name="plotter">The plotter.</param>
        public PlotView(FrameworkElement plotter)
        {
            if (plotter.IsEnabled)
            {
                this.Width = plotter.ActualWidth;
                this.Height = plotter.ActualHeight;
                if (plotter.ActualHeight == 0 || plotter.ActualWidth == 0)
                {
                    this.bitmap = BitmapFactory.New(10, 10);
                }
                else
                {
                    this.bitmap = BitmapFactory.New((int)(plotter.ActualWidth), (int)(plotter.ActualHeight));
                }
            }

            this.Viewport = new Viewport2D(new ViewportArea(-10000, 20000, 0, 0),
                new ViewportArea(0, this.ActualWidth, 0, this.ActualHeight));
        }

        protected bool isGettingUpdated = false;

        internal void BeginUpdate()
        {
            isGettingUpdated = true;
        }

        internal void EndUpdate()
        {
            isGettingUpdated = false;
        }

        public Viewport.Viewport2D Viewport
        {
            get
            {
                return (Viewport.Viewport2D)this.GetValue(ViewportProperty);
            }
            set
            {
                this.SetValue(ViewportProperty, value);
            }
        }

        public WfmEnum Channel
        {
            get
            {
                return (WfmEnum)this.GetValue(ChannelProperty);
            }
            set
            {
                this.SetValue(ChannelProperty, value);
            }
        }

        public double StartIndex
        {
            get
            {
                return (double)this.GetValue(StartIndexProperty);
            }
            set
            {
                this.SetValue(StartIndexProperty, value);
            }
        }

        public double StopIndex
        {
            get
            {
                return (double)this.GetValue(StopIndexProperty);
            }
            set
            {
                this.SetValue(StopIndexProperty, value);
            }
        }

        /// <summary>
        /// Scale of the Parent graph
        /// </summary>
        public double Scale
        {
            get
            {
                return (double)this.GetValue(ScaleProperty);
            }
            set
            {
                this.SetValue(ScaleProperty, value);
            }
        }

        /// <summary>
        /// Offset of the parent graph
        /// </summary>
        public double Offset
        {
            get
            {
                return (double)this.GetValue(OffsetProperty);
            }
            set
            {
                this.SetValue(OffsetProperty, value);
            }
        }

        /// <summary>
        /// Scale the original data
        /// </summary>
        public double DataScale
        {
            get { return (double)this.GetValue(DataScaleProperty); }
            set { this.SetValue(DataScaleProperty, value); }
        }

        /// <summary>
        /// Offset the original data
        /// </summary>
        public double DataOffset
        {
            get { return (double)this.GetValue(DataOffsetProperty); }
            set { this.SetValue(DataOffsetProperty, value); }
        }

        public bool IsDirty
        {
            get
            {
                return (bool)this.GetValue(IsDirtyProperty);
            }
            set
            {
                this.SetValue(IsDirtyProperty, value);
            }
        }

        public bool IsShowing
        {
            get
            {
                return (bool)this.GetValue(IsShowingProperty);
            }
            set
            {
                this.SetValue(IsShowingProperty, value);
            }
        }

        public IEnumerable<Point> Points
        {
            get
            {
                return (IEnumerable<Point>)this.GetValue(PointsProperty);
            }
            set
            {
                this.SetValue(PointsProperty, value);
            }
        }

        public double MinIndex
        {
            get
            {
                return (double)this.GetValue(MinIndexProperty);
            }
            set
            {
                this.SetValue(MinIndexProperty, value);
            }
        }

        public double MaxIndex
        {
            get
            {
                return (double)this.GetValue(MaxIndexProperty);
            }
            set
            {
                this.SetValue(MaxIndexProperty, value);
            }
        }

        public virtual void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Width = 3d * e.NewSize.Width;
            this.Height = 3d * e.NewSize.Height;
            this.bitmap = BitmapFactory.New((int)(this.Width), (int)(this.Height));
            this.Viewport.Output = new ViewportArea(0, this.Width, 0, this.Height);
        }

        public void DoUndo()
        {
            UndoItem undoitem;
            if (this.Undo.Count > 1)
            {
                //Create UndoItem with Current Data
                undoitem = new UndoItem(
                    this.Viewport.Visible.X1, this.Viewport.Visible.X2,
                    this.Viewport.Visible.Y1, this.Viewport.Visible.Y2,
                    this.Offset, this.Scale,
                    this.DataOffset, this.DataScale);
                //Check if the previous Undo operation is same as current data
                //if same ignore the last undo item.
                if (this.Undo[this.Undo.Count - 1].Equals(undoitem))
                {
                    this.Undo.RemoveAt(this.Undo.Count - 1);
                }
            }
            if (this.Undo.Count > 0)
            {
                this._IsUndoEnabled = true;

                //Get the last undo item
                undoitem = this.Undo[this.Undo.Count - 1];

                this.StartIndex = this.Viewport.Visible.X1 = undoitem.X1;
                this.StopIndex = this.Viewport.Visible.X2 = undoitem.X2;
                this.Viewport.Visible.Y1 = undoitem.Y1;
                this.Viewport.Visible.Y2 = undoitem.Y2;
                this.Scale = undoitem.Scale;
                this.Offset = undoitem.Offset;
                this.DataOffset = undoitem.DataOffset;
                this.DataScale = undoitem.DataScale;

                //Remove the last last undo item
                this.Undo.Remove(undoitem);
            }
        }

        public void UpdateViewport(PlotEvents plotEvent, Point visibleStart, Point visibleEnd)
        {
            double screenStartIndex = this.StartIndex;
            double screenEndIndex = this.StopIndex;

            if (this._Restriction == null)
                this._Restriction = new Prodigy.WaveformControls.Viewport.ViewportRestrictions(new ViewportArea(0, 0, 0, 0));

            //if (plotEvent == PlotEvents.NONE)
            //{
            //    //no need to change
            //}
            //else
            //{
                screenStartIndex = this.Viewport.CursorScreenToData(new Point(this.Width / 3d, 0)).X;
                screenEndIndex = this.Viewport.CursorScreenToData(new Point(this.Width * 2d / 3d, 0)).X;
            //}

            this._Restriction.Restriction.X1 = this.MinIndex;
            this._Restriction.Restriction.X2 = this.MaxIndex;

            Point p1;
            Point p2;
            p1 = this.Viewport.CursorScreenToData(visibleStart);
            p2 = this.Viewport.CursorScreenToData(visibleEnd);

            Vector change = p2 - p1;

            if (plotEvent == PlotEvents.ZOOM_OUT)
            {
                double prevStart = screenStartIndex;
                double tmp = 0;
                tmp = 2d * screenStartIndex - screenEndIndex;
                screenEndIndex = 2d * screenEndIndex - screenStartIndex;
                screenStartIndex = tmp;

                if (screenStartIndex < this.MinIndex)
                {
                    screenStartIndex = this.MinIndex;
                }
                if (screenEndIndex > this.MaxIndex)
                {
                    screenEndIndex = this.MaxIndex;
                }

                if (!double.IsNaN(this.HorizontalZoomLimit))
                {
                    if (screenEndIndex - screenStartIndex >= this.HorizontalZoomLimit)
                    {
                        screenStartIndex = prevStart;
                        screenEndIndex = screenStartIndex + this.HorizontalZoomLimit;
                    }
                }
            }
            else if (plotEvent == PlotEvents.PAN)
            {
                screenStartIndex -= change.X;
                screenEndIndex -= change.X;

                //viewport restrictions
                double tmp = 0;
                if (screenStartIndex < this.MinIndex)
                {
                    tmp = screenStartIndex;
                    screenStartIndex = this.MinIndex;
                    screenEndIndex += (this.MinIndex - tmp);
                }
                if (screenEndIndex > this.MaxIndex)
                {
                    tmp = screenEndIndex;
                    screenStartIndex -= (tmp - this.MaxIndex);
                    screenEndIndex = this.MaxIndex;
                }
            }
            else if (plotEvent == PlotEvents.ZOOM_IN)
            {
                if (p2.X < p1.X)
                {
                    screenStartIndex = p2.X;
                    screenEndIndex = p1.X;
                }
                else
                {
                    screenStartIndex = p1.X;
                    screenEndIndex = p2.X;
                }
            }
            else if (plotEvent == PlotEvents.FIT)
            {
                if (!double.IsNaN(this.HorizontalZoomLimit))
                {
                    double zoomFromLeft = (this.HorizontalZoomLimit - (screenEndIndex - screenStartIndex)) / 2d;

                    if (zoomFromLeft < 0)
                    {
                        screenStartIndex = (screenEndIndex - screenStartIndex) / 2d;
                        screenEndIndex = screenStartIndex;
                        zoomFromLeft = this.HorizontalZoomLimit / 2d;
                    }

                    screenStartIndex -= zoomFromLeft;
                    screenEndIndex += zoomFromLeft;

                    if (screenStartIndex < this.MinIndex)
                        screenStartIndex = this.MinIndex;
                    if (screenEndIndex > this.MaxIndex)
                        screenEndIndex = this.MaxIndex;
                }
                else
                {
                    screenStartIndex = MinIndex;
                    screenEndIndex = MaxIndex;
                }
            }
            else if (plotEvent == PlotEvents.RESET)
            {
                this.Viewport.Visible.X1 = this.StartIndex;
                this.Viewport.Visible.X2 = this.StopIndex;

                screenStartIndex = this.Viewport.CursorScreenToData(new Point(this.Width / 3d, 0)).X;
                screenEndIndex = this.Viewport.CursorScreenToData(new Point(this.Width * 2d / 3d, 0)).X;
            }
            else
            {
                //no need to change
            }

            Point startPt = visibleStart;
            Point endPt = visibleEnd;
            this._Restriction.Restriction.Y1 = this.Scale * 5d + this.Offset;
            this._Restriction.Restriction.Y2 = -this.Scale * 5d + this.Offset;

            p1 = this.Viewport.CursorScreenToData(startPt);
            p2 = this.Viewport.CursorScreenToData(endPt);
            change = p2 - p1;

            double y1;
            double y2;
            if (plotEvent == PlotEvents.ZOOM_OUT)
            {
                y1 = this.Viewport.Visible.Y1;
                y2 = this.Viewport.Visible.Y2;
            }
            else if (plotEvent == PlotEvents.PAN)
            {
                double center = (this.Viewport.Visible.Y1 + this.Viewport.Visible.Y2) / 2d;
                double volt = (this.Viewport.Visible.Y2 - this.Viewport.Visible.Y1) / 6d;
                y1 = center + volt - change.Y;
                y2 = center - volt - change.Y;
            }
            else if (plotEvent == PlotEvents.ZOOM_IN)
            {
                if (p2.Y < p1.Y)
                {
                    y1 = p2.Y;
                    y2 = p1.Y;
                }
                else
                {
                    y1 = p1.Y;
                    y2 = p2.Y;
                }
            }
            else if (plotEvent == PlotEvents.FIT)
            {
                y1 = this.Scale * 5d + this.Offset;
                y2 = -this.Scale * 5d + this.Offset;
            }
            else
            {
                y1 = this.Viewport.Visible.Y1;
                y2 = this.Viewport.Visible.Y2;
            }

            ViewportArea visible = new ViewportArea(screenStartIndex, screenEndIndex, y1, y2);
            if (y2 > y1)
            {
                visible.Y1 = y2;
                visible.Y2 = y1;
            }

            visible = this._Restriction.Apply(visible);

            this.Viewport.Visible.X1 = 2d * visible.X1 - visible.X2;
            this.Viewport.Visible.X2 = 2d * visible.X2 - visible.X1;
            this.Viewport.Visible.Y1 = 2d * visible.Y1 - visible.Y2;
            this.Viewport.Visible.Y2 = 2d * visible.Y2 - visible.Y1;

            this.StartIndex = this.Viewport.Visible.X1;
            this.StopIndex = this.Viewport.Visible.X2;

            //Undo
            if (plotEvent == PlotEvents.UNDO)
            {
                this.DoUndo();
            }
        }

        public void UndoClear()
        {
            this.Undo.Clear();
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        protected virtual void Refresh()
        {
            if (this.Viewport.Visible.IsValid() && !this._IsUndoEnabled)
            {
                UndoItem undoitem = new UndoItem(
                    this.Viewport.Visible.X1, this.Viewport.Visible.X2,
                    this.Viewport.Visible.Y1, this.Viewport.Visible.Y2,
                    this.Offset, this.Scale,
                    this.DataOffset, this.DataScale);
                if (this.Undo.Count >= 10)
                {
                    if (!this.Undo.Last().Equals(undoitem))
                    {
                        this.Undo.RemoveAt(0);
                        this.Undo.Add(undoitem);
                    }
                }
                else
                {
                    if (this.Undo.Count == 0)
                    {
                        this.Undo.Add(undoitem);
                    }
                    else if (!this.Undo.Last().Equals(undoitem))
                    {
                        this.Undo.Add(undoitem);
                    }
                }
            }

            this._IsUndoEnabled = false;

            //use viewport to generate points at current coordinates
            //update polyline.Points
            if (this.IsShowing)
            {
                TypeOfChart chartType = (this.DataContext as IPlotInfoView).ChartType;
                List<Point> pnts = new List<Point>();
                    if (chartType == TypeOfChart.SINE)
                    {
                        var pEnumerater = this.points.GetEnumerator();
                        while (pEnumerater.MoveNext())
                        {
                            pnts.Add(this.Viewport.DataToScreen(pEnumerater.Current));
                        }
                    }
                    else if (chartType == TypeOfChart.RECTANGULAR)
                    {
                        Point currentPoint;
                        Point nextPoint;
                        int totalPts = this.points.Count();
                        for (int i = 0; i < totalPts; i++)
                        {
                            if (i >= totalPts)
                            {
                                currentPoint = this.Viewport.DataToScreen(this.points.ElementAt(i));
                                nextPoint = this.Viewport.DataToScreen(this.points.ElementAt(i + 1));
                                pnts.Add(currentPoint);
                                pnts.Add(new Point(nextPoint.X, currentPoint.Y));
                            }
                            else
                            {
                                pnts.Add(this.Viewport.DataToScreen(this.points.ElementAt(i)));
                            }
                        }
                    }

                    if (pnts.Count > 1)
                    {
                        this.UpdateViewLayout(pnts);
                    }
            }
            else
            {
                this.UpdateViewLayout();
            }
        }

        private static void StartIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                PlotView view = (PlotView)d;
                if (double.Equals(e.NewValue, e.OldValue) == false)
                {
                    view.Viewport.Visible.X1 = (double)e.NewValue;
                }
            }
        }

        private static void StopIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                PlotView view = (PlotView)d;
                if (double.Equals(e.NewValue, e.OldValue) == false)
                {
                    view.Viewport.Visible.X2 = (double)e.NewValue;
                }
            }
        }

        private static void OffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                PlotView view = (PlotView)d;
                if (double.Equals(e.NewValue, e.OldValue) == false)
                {
                    view.Viewport.Visible.Y1 = -5D * view.Scale + (double)e.NewValue;
                    view.Viewport.Visible.Y2 = 5D * view.Scale + (double)e.NewValue;

                    if (view.IsInitialized && view.isGettingUpdated == false)
                        view.IsDirty = true;
                }
            }
        }

        private static void ScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                PlotView view = (PlotView)d;
                if(double.Equals(e.NewValue, e.OldValue) == false)
                {
                    view.Viewport.Visible.Y1 = -5D * (double)e.NewValue + view.Offset;
                    view.Viewport.Visible.Y2 = 5D * (double)e.NewValue + view.Offset;

                    if (view.IsInitialized && view.isGettingUpdated == false)
                        view.IsDirty = true;
                }
            }
        }

        private static void DataOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                PlotView view = (PlotView)d;
                if (double.Equals(e.NewValue, e.OldValue) == false)
                {
                    view.Viewport.DataVerticalOffset = (double)e.NewValue;

                    if (view.IsInitialized && view.isGettingUpdated == false)
                        view.IsDirty = true;
                }
            }
        }

        private static void DataScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                PlotView view = (PlotView)d;
                if (double.Equals(e.NewValue, e.OldValue) == false)
                {
                    view.Viewport.DataVerticalScale = (double)e.NewValue;

                    if (view.IsInitialized && view.isGettingUpdated == false)
                        view.IsDirty = true;
                }
            }
        }

        private static void IsShowingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                PlotView view = d as PlotView;

                if (view.IsInitialized && view.isGettingUpdated == false)
                    view.IsDirty = true;
            }
        }

        private static void PointsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Keep this Empty
        }

        private static object PointsPropertyCoerced(DependencyObject d, object baseValue)
        {
            if (d != null && baseValue != null)
            {
                PlotView view = (PlotView)d;

                if (view.DataContext != null)
                {
                    view.points = (IEnumerable<Point>)baseValue;
                    view.Viewport.Visible.X1 = view.StartIndex;
                    view.Viewport.Visible.X2 = view.StopIndex;

                    view.Refresh();// do not change
                }
            }

            return baseValue;
        }

        private static void MinIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                // Nothing to do here: IsDity property updated the plot.
            }
        }

        private static void MaxIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                // Nothing to do here: IsDity property updated the plot.
            }
        }

        private void UpdateViewLayout(IEnumerable<Point> points = null)
        {
            if (this.bitmap != null)
            {
                lock (this.bitmap)
                {
                this.bitmap.Clear();

                if (points != null)
                {
                    Color c = (Application.Current.Resources[this.Channel.ToString()] as SolidColorBrush).Color;
                    this.bitmap.Lock();
                    this.bitmap.DrawPolyline(points.ToArray(), c);
                    this.bitmap.Unlock();
                }
            	}
            }

            //CreateThumbnail(@"E:\bitmap.png", bitmap);

            this.Source = this.bitmap;
            //this.Margin = new Thickness(0);
        }
    }
}