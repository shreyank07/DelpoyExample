// <copyright file="PlotterView.xaml.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>
  
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Prodigy.WaveformControls.Navigation;
using Prodigy.WaveformControls.View;
using Prodigy.WaveformControls.Viewport;
using Prodigy.WaveformControls.Interfaces;
using System.Runtime.CompilerServices;

namespace Prodigy.WaveformControls.View
{
    /// <summary>
    /// Interaction logic for PlotterView.xaml
    /// </summary>
    public partial class PlotterView : UserControl
    {
        public PlotterView()
        {
            this.InitializeComponent();
        }

        public readonly static DependencyProperty PlotCollectionProperty =
            DependencyProperty.Register("PlotCollection", typeof(ObservableCollection<IPlotInfoView>), typeof(PlotterView),
                new PropertyMetadata(new ObservableCollection<IPlotInfoView>(), OnCollectionChanged));

        private static void OnCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var obj = (d as PlotterView);

                obj.CollectionChanged();
            }
        }

        void CollectionChanged()
        {
            if (this.PlotCollection != null)
            {
                this.PlotCollection.CollectionChanged +=
                    new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnCollectionChanged);
            }
        }

        public ObservableCollection<IPlotInfoView> PlotCollection
        {
            get
            {
                return this.GetValue(PlotCollectionProperty) as ObservableCollection<IPlotInfoView>;
            }
            set
            {
                this.SetValue(PlotCollectionProperty, value);
            }
        }

        public readonly static DependencyProperty PlotProperty =
            DependencyProperty.Register("Plot", typeof(IPlotInfoView), typeof(PlotterView),
                new PropertyMetadata(null, PlotPropertyChanged));

        private static void PlotPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var obj = (d as PlotterView);
                if (e.NewValue != null && e.OldValue != null)
                {
                    obj.RemoveGraph((IPlotInfoView)e.OldValue);
                    obj.AddGraph((IPlotInfoView)e.NewValue);
                }
                else if (e.NewValue != null && e.OldValue == null)
                {
                    obj.AddGraph((IPlotInfoView)e.NewValue);
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    obj.RemoveGraph((IPlotInfoView)e.OldValue);
                }
            }
        }

        public IPlotInfoView Plot
        {
            get
            {
                return this.GetValue(PlotCollectionProperty) as IPlotInfoView;
            }
            set
            {
                this.SetValue(PlotCollectionProperty, value);
            }
        }

        public string BusDataSourceString { get; set; }

        private void AddBus(IPlotInfoView view)
        {
            try
            {
                BusView chart = new BusView(this.plotCanvas);

                chart.BeginInit();

                chart.DataContext = view;
                chart.Viewport = new Viewport2D(new ViewportArea(-10000, 20000, -1, 1),
                    new ViewportArea(0, this.plotCanvas.ActualWidth, 0, this.plotCanvas.ActualHeight));

                Binding b;

                b = new Binding(nameof(IPlotInfoView.Channel));
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                chart.SetBinding(BusView.ChannelProperty, b);

                b = new Binding(nameof(IPlotInfoView.StartWfmIndex));
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                chart.SetBinding(BusView.StartIndexProperty, b);

                b = new Binding(nameof(IPlotInfoView.StopWfmIndex));
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                chart.SetBinding(BusView.StopIndexProperty, b);

                b = new Binding("VerticalOffset");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                chart.SetBinding(BusView.OffsetProperty, b);

                b = new Binding("VerticalScale");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                chart.SetBinding(PlotView.ScaleProperty, b);

                b = new Binding(this.BusDataSourceString);
                b.UpdateSourceTrigger = UpdateSourceTrigger.Default;
                chart.SetBinding(BusView.DataProperty, b);

                b = new Binding("IsShowing");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                chart.SetBinding(BusView.IsShowingProperty, b);

                b = new Binding("IsDirty");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                chart.SetBinding(BusView.IsDirtyProperty, b);

                this.prevStart = this.StartIndex;
                this.prevStop = this.StopIndex;

                chart.MinIndex = this.MinIndex;
                chart.MaxIndex = this.MaxIndex;

                chart.StartIndex = this.StartIndex;
                chart.StopIndex = this.StopIndex;

                this.ChartCollection.Add(chart);
                this.plotCanvas.Children.Add(chart);

                //Clear Undo Operations
                this.ChartCollection.All(c => { c.UndoClear(); return true; });

                if (AutoScaleAndOffset)
                {
                    // Added charts scale and offsets are calculated automatically so that all charts are visible on the screen
                    // TODO: implement the auto scale offset for all the chart, so that all fits the screen.

                    var plots = this.plotCanvas.Children.OfType<PlotView>();

                    double scale = 10D / this.PlotCollection.Count;
                    for (int i = 0; i < this.PlotCollection.Count; i++)
                    {
                        var plotData = this.PlotCollection[i];
                        var c = plots.FirstOrDefault(p => p.DataContext == plotData);

                        c.BeginUpdate();
                        //1 chart scale plot covers 8 div
                        //2 chart scale plot covers 4 div

                        c.DataScale = scale * 0.9D;

                        // Warning: Somehow this code is working - figure out right way to do it
                        c.DataOffset = -5d - c.DataScale / 2d + scale * (this.PlotCollection.Count - i) - scale * 0.5;

                        c.EndUpdate();
                    }
                }
                else
                {
                    // Added chart decides the scale and offset
                    chart.Viewport.DataVerticalOffset = view.VerticalOffset;
                    chart.Viewport.DataVerticalScale = view.VerticalScale;
                }

                chart.IsShowing = true;
                chart.EndInit();
            }
            catch
            {
            }
        }

        public void RemoveBus(IPlotInfoView view)
        {
            IPlot chart = this.ChartCollection.Where(c => c.Channel == view.Channel).First(c => c is BusView);

            this.ChartCollection.Remove(chart);
            this.plotCanvas.Children.Remove(chart as UIElement);
        }

        private void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (IPlotInfoView item in e.NewItems)
                    {
                        if (item.ChartType == TypeOfChart.SINE)
                        {
                            this.AddGraph(item);
                        }
                        else if (item.ChartType == TypeOfChart.RECTANGULAR)
                        {
                            if (item is IDigitalPlotInfoView)
                            {
                                this.AddGraph(item as IDigitalPlotInfoView);
                            }
                            else
                            {
                                this.AddGraph(item);
                            }
                        }
                        else if (item.ChartType == TypeOfChart.BUS)
                        {
                            this.AddBus(item);
                        }
                    }
                }
                else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    foreach (IPlotInfoView item in e.OldItems)
                    {
                        if (item.ChartType == TypeOfChart.SINE)
                        {
                            this.RemoveGraph(item);
                        }
                        else if (item.ChartType == TypeOfChart.RECTANGULAR)
                        {
                            this.RemoveGraph(item);
                        }
                        else if (item.ChartType == TypeOfChart.BUS)
                        {
                            this.RemoveBus(item);
                        }
                    }
                }
                else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    foreach (IPlotInfoView item in PlotCollection)
                    {
                        if (item.ChartType == TypeOfChart.SINE)
                        {
                            this.RemoveGraph(item);
                        }
                        else if (item.ChartType == TypeOfChart.RECTANGULAR)
                        {
                            this.RemoveGraph(item);
                        }
                        else if (item.ChartType == TypeOfChart.BUS)
                        {
                            this.RemoveBus(item);
                        }
                    }
                }

                if (this.PlotCollection.Count == 0)
                {
                    this.IsMouseEnabled = false;
                    this.PlotEvent = PlotEvents.NONE;
                }
                else
                {
                    this.IsMouseEnabled = true;
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static readonly DependencyProperty IsMouseEnabledProperty =
            DependencyProperty.Register("IsMouseEnabled", typeof(bool), typeof(PlotterView),
                new PropertyMetadata(false,new PropertyChangedCallback(IsMouseEnabledPropertyChanged)));

        public bool IsMouseEnabled
        {
            get
            {
                return (bool)this.GetValue(IsMouseEnabledProperty);
            }
            set
            {
                this.SetValue(IsMouseEnabledProperty, value);
            }
        }

        private static void IsMouseEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                PlotterView view = d as PlotterView;
                if ((bool)e.NewValue == true)
                {
                    MouseNavigation mouseNavigation = new MouseNavigation();
                    mouseNavigation.MultiZoomDisabled = view.MultiZoomDisabled;
                    Binding b = new Binding("PlotEvent");
                    b.Source = view;
                    b.Mode = BindingMode.TwoWay;
                    mouseNavigation.SetBinding(MouseNavigation.PlotEventProperty, b);                    
                    view.parent.Children.Add(mouseNavigation);
                    mouseNavigation.AttachPlotter(view.plotCanvas);
                    Canvas.SetLeft(view.plotCanvas, -view.parent.ActualWidth);
                    Canvas.SetTop(view.plotCanvas, -view.parent.ActualHeight);

                    mouseNavigation.UpdateViewportEvent += new UpdateViewportEventHandler(view.UpdateViewport);
                    view.MouseNavigation = mouseNavigation;
                }
                else
                {
                    if (view.parent.Children.OfType<MouseNavigation>().Count() != 0)
                    {
                        view.MouseNavigation.DetachPlotter();
                        view.MouseNavigation.UpdateViewportEvent -= new UpdateViewportEventHandler(view.UpdateViewport);
                        view.parent.Children.Remove(view.MouseNavigation);
                        view.MouseNavigation = null;
                    }
                }
            }
        }

        public static readonly DependencyProperty MouseNavigationProperty =
            DependencyProperty.Register("MouseNavigation", typeof(MouseNavigation),
                typeof(PlotterView), new FrameworkPropertyMetadata(null));

        public MouseNavigation MouseNavigation
        {
            get
            {
                return (MouseNavigation)this.GetValue(MouseNavigationProperty);
            }
            set
            {
                this.SetValue(MouseNavigationProperty, value);
            }
        }

        private bool _MultiZoomDisabled = false;

        public bool MultiZoomDisabled
        {
            get
            {
                return this._MultiZoomDisabled;
            }
            set
            {
                this._MultiZoomDisabled = value;
                if (this.MouseNavigation != null)
                {
                    this.MouseNavigation.MultiZoomDisabled = value;
                }
            }
        }

        public static readonly DependencyProperty PlotEventProperty =
            DependencyProperty.Register("PlotEvent", typeof(PlotEvents),
                typeof(PlotterView), new FrameworkPropertyMetadata(PlotEvents.NONE,new PropertyChangedCallback(OnPlotEventPropertyChanged)));

        private static void OnPlotEventPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                PlotterView view = d as PlotterView;
                System.Windows.Input.Cursor cursor = null;
                try
                {
                    switch (view.PlotEvent)
                    {
                        case PlotEvents.FIT:
                            cursor = ((FrameworkElement)Application.Current.Resources["fitCursor"]).Cursor;
                            break;
                        case PlotEvents.PAN:
                            cursor = ((FrameworkElement)Application.Current.Resources["panCursor"]).Cursor;
                            break;
                        case PlotEvents.UNDO:
                            cursor = ((FrameworkElement)Application.Current.Resources["undoCursor"]).Cursor;
                            break;
                        case PlotEvents.ZOOM_IN:
                            cursor = ((FrameworkElement)Application.Current.Resources["zoomInCursor"]).Cursor;
                            break;
                        case PlotEvents.ZOOM_OUT:
                            cursor = ((FrameworkElement)Application.Current.Resources["zoomOutCursor"]).Cursor;
                            break;
                    }
                }
                catch
                {
                    cursor = null;
                }

                view.parent.Cursor = cursor;
            }
        }

        public PlotEvents PlotEvent
        {
            get
            {
                return (PlotEvents)this.GetValue(PlotEventProperty);
            }
            set
            {
                this.SetValue(PlotEventProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedChartProperty =
            DependencyProperty.Register("SelectedChart", typeof(IPlotInfoView), typeof(PlotterView),
                new PropertyMetadata(null, new PropertyChangedCallback(SelectedChartPropertyChanged)));

        private static void SelectedChartPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                PlotterView view = d as PlotterView;
                IPlotInfoView iplot = e.NewValue as IPlotInfoView;
                if (view.ChartCollection.Count > 0 && iplot != null)
                {
                    if (view.ChartCollection.Any(c => c.Channel == iplot.Channel))
                    {
                        IPlot plot = view.ChartCollection.First(c => c.Channel == iplot.Channel);
                        if (plot is PlotView)
                        {
                            view.SelectedPlotView = plot as PlotView;
                        }
                    }
                }
            }
        }

        public IPlotInfoView SelectedChart
        {
            get
            {
                return (IPlotInfoView)this.GetValue(SelectedChartProperty);
            }
            set
            {
                this.SetValue(SelectedChartProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedPlotViewProperty =
            DependencyProperty.Register("SelectedPlotView", typeof(PlotView), typeof(PlotterView),
                new PropertyMetadata(null,new PropertyChangedCallback(SelectedViewportPropertyChanged)));

        private static void SelectedViewportPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlotterView view = d as PlotterView;
            if (view != null)
            {
                foreach (var p in view.plotCanvas.Children.OfType<PlotView>())
                {
                    Panel.SetZIndex(p, 0);
                }

                if (e.NewValue != null)
                {
                    Panel.SetZIndex((e.NewValue as PlotView), 1);
                }
            }
        }

        public PlotView SelectedPlotView
        {
            get
            {
                return (PlotView)this.GetValue(SelectedPlotViewProperty);
            }
            set
            {
                this.SetValue(SelectedPlotViewProperty, value);
            }
        }

        public void AddGraph(IPlotInfoView view)
        {
            try
            {
                PlotView chart = new PlotView(this.plotCanvas);

                chart.BeginInit();
                chart.DataContext = view;
                chart.Viewport = new Viewport2D(new ViewportArea(-10000, 20000, -1, 1),
                    new ViewportArea(0, this.plotCanvas.ActualWidth, 0, this.plotCanvas.ActualHeight));

                Binding b;

                b = new Binding("Channel");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                chart.SetBinding(PlotView.ChannelProperty, b);

                b = new Binding("StartWfmIndex");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                chart.SetBinding(PlotView.StartIndexProperty, b);

                b = new Binding("StopWfmIndex");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                chart.SetBinding(PlotView.StopIndexProperty, b);

                b = new Binding("VerticalOffset");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                chart.SetBinding(PlotView.OffsetProperty, b);

                b = new Binding("VerticalScale");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                chart.SetBinding(PlotView.ScaleProperty, b);

                b = new Binding(this.PointsDataSource);
                b.UpdateSourceTrigger = UpdateSourceTrigger.Default;
                chart.SetBinding(PlotView.PointsProperty, b);

                b = new Binding("IsShowing");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                chart.SetBinding(PlotView.IsShowingProperty, b);

                b = new Binding("IsDirty");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                chart.SetBinding(PlotView.IsDirtyProperty, b);

                this.prevStart = this.StartIndex;
                this.prevStop = this.StopIndex;

                chart.MinIndex = this.MinIndex;
                chart.MaxIndex = this.MaxIndex;

                chart.StartIndex = this.StartIndex;
                chart.StopIndex = this.StopIndex;

                this.ChartCollection.Add(chart);
                this.plotCanvas.Children.Add(chart);

                //Clear Undo Operations
                this.ChartCollection.All(c => { c.UndoClear(); return true; });

                if (AutoScaleAndOffset)
                {
                    // Added charts scale and offsets are calculated automatically so that all charts are visible on the screen
                    // TODO: implement the auto scale offset for all the chart, so that all fits the screen.

                    var plots = this.plotCanvas.Children.OfType<PlotView>();

                    double scale = 10D / this.PlotCollection.Count;
                    for (int i = 0; i < this.PlotCollection.Count; i++)
                    {
                        var plotData = this.PlotCollection[i];
                        var c = plots.FirstOrDefault(p => p.DataContext == plotData);

                        c.BeginUpdate();

                        //1 chart scale plot covers 9 div
                        //2 chart scale plot covers 4.5 div

                        c.DataScale = scale * 0.9D;

                        // Warning: Somehow this code is working - figure out right way to do it
                        c.DataOffset = -5d - c.DataScale / 2d + scale * (this.PlotCollection.Count - i) - scale * 0.5;

                        c.EndUpdate();
                    }
                }
                else
                {
                    // Added chart decides the scale and offset
                    chart.Viewport.DataVerticalOffset = view.VerticalOffset;
                    chart.Viewport.DataVerticalScale = view.VerticalScale;
                }

                this.SelectedChart = view;
                this.SelectedPlotView = chart;
                chart.IsShowing = true;
                
                chart.EndInit();
            }
            catch
            {
            }
        }

        public void AddGraph(IDigitalPlotInfoView view)
        {
            try
            {
                view.SetIsShowing();

                this.InvalidateMeasure();
                DigitalPlotView chart = new DigitalPlotView(this.plotCanvas);
                chart.DataContext = view;
                chart.Viewport = new Viewport2D(new ViewportArea(-10000, 20000, 0, 0),
                    new ViewportArea(0, this.plotCanvas.ActualWidth, 0, this.plotCanvas.ActualHeight));

                chart.BeginInit();

                Binding b;

                b = new Binding("Channel");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                chart.SetBinding(PlotView.ChannelProperty, b);

                b = new Binding("StartWfmIndex");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                chart.SetBinding(PlotView.StartIndexProperty, b);

                b = new Binding("StopWfmIndex");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                chart.SetBinding(PlotView.StopIndexProperty, b);

                b = new Binding("VerticalOffset");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                chart.SetBinding(PlotView.OffsetProperty, b);

                b = new Binding("VerticalScale");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                chart.SetBinding(PlotView.ScaleProperty, b);

                b = new Binding(this.PointsDataSource);
                b.UpdateSourceTrigger = UpdateSourceTrigger.Default;
                chart.SetBinding(DigitalPlotView.DataProperty, b);

                b = new Binding("IsShowing");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                chart.SetBinding(PlotView.IsShowingProperty, b);

                b = new Binding("IsDirty");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                chart.SetBinding(PlotView.IsDirtyProperty, b);

                chart.EndInit();

                this.prevStart = this.StartIndex;
                this.prevStop = this.StopIndex;

                chart.MinIndex = this.MinIndex;
                chart.MaxIndex = this.MaxIndex;
                chart.StartIndex = this.StartIndex;
                chart.StopIndex = this.StopIndex;

                this.ChartCollection.Add(chart);
                this.plotCanvas.Children.Add(chart);

                //Clear Undo Operations
                this.ChartCollection.All(c => { c.UndoClear(); return true; });

                if (AutoScaleAndOffset)
                {
                    // Added charts scale and offsets are calculated automatically so that all charts are visible on the screen
                    // TODO: implement the auto scale offset for all the chart, so that all fits the screen.
                    //Prodigy.Business.Extensions.CustomExtensions.GetEquivalent(0, this.plotCanvas.ActualHeight, 1, -1, this.plotCanvas.ActualHeight / 2);
                    var plots = this.plotCanvas.Children.OfType<PlotView>();
                    for (int i = 0; i < this.PlotCollection.Count; i++)
                    {
                        var plotData = this.PlotCollection[i];
                        var c = plots.FirstOrDefault(p => p.DataContext == plotData);
                        //1 chart scale plot covers 8 div
                        //2 chart scle plot covers 4 div
                        c.DataScale = 8D * c.Scale / this.PlotCollection.Count;
                        c.DataOffset = (5D - i * 0.2D) * c.Scale - (i + 1) * c.DataScale;
                    }
                }
                else
                {
                    // Added chart decides the scale and offset
                    chart.Viewport.DataVerticalOffset = view.VerticalOffset;
                    chart.Viewport.DataVerticalScale = view.VerticalScale;
                }
                this.Refresh();

                //if (this.SelectedPlotView != null)
                //{
                //    chart.StartIndex = this.SelectedPlotView.StartIndex;
                //    chart.StopIndex = this.SelectedPlotView.StopIndex;
                //    chart.Viewport.Visible = DataTransform.ViewportToData
                //        (this.SelectedPlotView.Viewport, chart.Viewport.Visible, this.SelectedPlotView.Scale, view.VerticalScale);
                //    this.Refresh();
                //}
                //else
                //{
                //    this.UpdateViewport(PlotEvents.FIT, new Point(0, 0), new Point(0, 0));
                //}

                this.SelectedChart = view;
                this.SelectedPlotView = chart;
            }
            catch
            {
            }
        }

        public static readonly DependencyProperty PointsDataSourceProperty =
            DependencyProperty.Register("PointsDataSource", typeof(string), typeof(PlotterView),
                new PropertyMetadata(null));

        public string PointsDataSource
        {
            get
            {
                return (string)this.GetValue(PointsDataSourceProperty);
            }
            set
            {
                this.SetValue(PointsDataSourceProperty, value);
            }
        }
        
        public void RemoveGraph(IPlotInfoView view)
        {
            IPlot chart = this.ChartCollection.Where(c => c.Channel == view.Channel).First(c => c is PlotView);

            this.ChartCollection.Remove(chart);
            this.plotCanvas.Children.Remove(chart as PlotView);

            if (this.ChartCollection.Where(c => !(c is BusView)).Count() > 0)
            {
                this.SelectedChart = (this.ChartCollection.First(c => !(c is BusView)) as Image).DataContext as IPlotInfoView;
            }
            else
            {
                this.SelectedChart = null;
                this.SelectedPlotView = null;
                this.StartIndex = 0;
                this.StopIndex = this.MaxIndex;
            }
        }

        public System.Collections.ObjectModel.ObservableCollection<IPlot> ChartCollection =
            new System.Collections.ObjectModel.ObservableCollection<IPlot>();

        public static DependencyProperty StartIndexProperty =
            DependencyProperty.Register("StartIndex", typeof(double), typeof(PlotterView),
                new PropertyMetadata(0.0));

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

        public static DependencyProperty StopIndexProperty =
            DependencyProperty.Register("StopIndex", typeof(double), typeof(PlotterView),
                new PropertyMetadata(0.0));

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

        public static DependencyProperty MinIndexProperty =
            DependencyProperty.Register("MinIndex", typeof(double), typeof(PlotterView),
                new PropertyMetadata(0d, new PropertyChangedCallback(MinIndexPropertyChanged), new CoerceValueCallback(MinIndexPropertyCoerce)));

        private static object MinIndexPropertyCoerce(DependencyObject d, object baseValue)
        {
            // IsDirty property is used to update the viewport
            //if (d != null)
            //{
            //    plotterview view = d as plotterview;
            //    view.isdirty = true;
            //}

            return baseValue;
        }

        private static void MinIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                PlotterView view = d as PlotterView;
                //if (e.NewValue != e.OldValue)
                //    view.UpdateViewport(PlotEvents.NONE, new Point(), new Point());
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

        public static DependencyProperty MaxIndexProperty =
            DependencyProperty.Register("MaxIndex", typeof(double), typeof(PlotterView),
                new PropertyMetadata(0d, new PropertyChangedCallback(MaxIndexPropertyChanged), new CoerceValueCallback(MaxIndexPropertyCoerce)));

        private static object MaxIndexPropertyCoerce(DependencyObject d, object baseValue)
        {
            //if (d != null)
            //{
            //    PlotterView view = d as PlotterView;
            //    view.IsDirty = true;
            //}

            return baseValue;
        }

        private static void MaxIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                PlotterView view = d as PlotterView;
                //if (e.NewValue != e.OldValue)
                //    view.UpdateViewport(PlotEvents.NONE, new Point(), new Point());
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
        //
        public static DependencyProperty VisibleMaxIndexProperty =
           DependencyProperty.Register("DisplayLimited", typeof(bool), typeof(PlotterView),
               new PropertyMetadata(false, new PropertyChangedCallback(VisibleMaxIndexPropertyChanged), new CoerceValueCallback(VisibleMaxIndexPropertyCoerce)));

        private static object VisibleMaxIndexPropertyCoerce(DependencyObject d, object baseValue)
        {
            if (d != null)
            {
                PlotterView view = d as PlotterView;
                view.IsDirty = true;
            }

            return baseValue;
        }

        private static void VisibleMaxIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                PlotterView view = d as PlotterView;
                //view.UpdateViewport(PlotEvents.NONE, new Point(), new Point());
            }
        }

        public bool DisplayLimited
        {
            get
            {
                return (bool)this.GetValue(VisibleMaxIndexProperty);
            }
            set
            {
                this.SetValue(VisibleMaxIndexProperty, value);
            }
        }
        
        //

        public void FIT()
        {
            this.UpdateViewport(PlotEvents.FIT, new Point(0, 0), new Point(0, 0));
        }

        public void PlotSelectedView(Point p1, Point p2)
        {
            this.UpdateViewport(PlotEvents.NONE, p1, p2);
        }

        private Prodigy.WaveformControls.Viewport.ViewportRestrictions _Restriction;

        #region Need Update on viewport restriction and other functions

        private readonly object LockObject = new object();

        /// <summary>
        /// Updates the viewport.
        /// This Updation only causes the X 
        /// calculate the viewport restrictions
        /// apply viewport restrictions
        /// horizontal restriction 0 to recordlength
        /// </summary>
        /// <param name="plotEvent">The plot event.</param>
        /// <param name="visibleStart">The visible start.</param>
        /// <param name="visibleEnd">The visible end.</param>
        public void UpdateViewport(PlotEvents plotEvent, Point visibleStart, Point visibleEnd)
        {
            lock (LockObject)
            {
                if (this._Restriction == null)
                {
                    this._Restriction = new Prodigy.WaveformControls.Viewport.ViewportRestrictions(new ViewportArea(0, 0, 0, 0));
                }
                this._Restriction.Restriction.X1 = this.MinIndex;
                this._Restriction.Restriction.X2 = this.MaxIndex;

                Point p1;
                Point p2;
                if (this.SelectedPlotView == null)
                {
                    p1 = new Point(Prodigy.Business
                                          .Extensions
                                          .CustomExtensions
                                          .GetEquivalent(this.StartIndex, this.StopIndex, 0, this.parent.ActualWidth * 3d, visibleStart.X),
                        0);
                    p2 = new Point(Prodigy.Business
                                          .Extensions
                                          .CustomExtensions
                                          .GetEquivalent(this.StartIndex, this.StopIndex, 0, this.parent.ActualWidth * 3d, visibleEnd.X),
                        0);
                }
                else
                {
                    p1 = this.SelectedPlotView.Viewport.CursorScreenToData(visibleStart);
                    p2 = this.SelectedPlotView.Viewport.CursorScreenToData(visibleEnd);
                }
                Vector change = p2 - p1;

                if (plotEvent == PlotEvents.ZOOM_OUT)
                {
                    double tmp = 0;
                    tmp = 2d * this.StartIndex - this.StopIndex;
                    this.StopIndex = 2d * this.StopIndex - this.StartIndex;
                    this.StartIndex = tmp;

                    if (this.StartIndex < this.MinIndex)
                    {
                        this.StartIndex = this.MinIndex;
                    }
                    if (this.StopIndex > this.MaxIndex)
                    {
                        this.StopIndex = this.MaxIndex;
                    }
                    if (this.IsOptimized)
                    {
                        if (this.StopIndex - this.StartIndex >= this.HorizontalZoomLimit)
                        {
                            //zoomout ended
                            this.StartIndex = this.prevStart;
                            this.StopIndex = this.StartIndex + this.HorizontalZoomLimit;
                        }
                    }
                }
                else if (plotEvent == PlotEvents.PAN)
                {
                    this.StartIndex -= change.X;
                    this.StopIndex -= change.X;

                    //viewport restrictions
                    double tmp = 0;
                    if (this.StartIndex < this.MinIndex)
                    {
                        tmp = this.StartIndex;
                        this.StartIndex = this.MinIndex;
                        this.StopIndex += (this.MinIndex - tmp);
                    }
                    if (this.StopIndex > this.MaxIndex)
                    {
                        tmp = this.StopIndex;
                        this.StartIndex -= (tmp - this.MaxIndex);
                        this.StopIndex = this.MaxIndex;
                    }
                }
                else if (plotEvent == PlotEvents.ZOOM_IN)
                {
                    if (p2.X < p1.X)
                    {
                        this.StartIndex = p2.X;
                        this.StopIndex = p1.X;
                    }
                    else
                    {
                        this.StartIndex = p1.X;
                        this.StopIndex = p2.X;
                    }
                }
                else if (plotEvent == PlotEvents.FIT)
                {
                    if (IsOptimized)
                    {
                        double zoomFromLeft = (this.HorizontalZoomLimit - (this.StopIndex - this.StartIndex)) / 2d;

                        if (zoomFromLeft < 0)
                        {
                            this.StartIndex = (this.StopIndex - this.StartIndex) / 2d;
                            this.StopIndex = this.StartIndex;
                            zoomFromLeft = this.HorizontalZoomLimit / 2d;
                        }

                        this.StartIndex -= zoomFromLeft;
                        this.StopIndex += zoomFromLeft;

                        if (this.StartIndex < this.MinIndex)
                            this.StartIndex = this.MinIndex;
                        if (this.StopIndex > this.MaxIndex)
                            this.StopIndex = this.MaxIndex;
                    }
                    else
                    {
                        this.StartIndex = MinIndex;
                        this.StopIndex = MaxIndex;
                    }
                }
                else if (plotEvent == PlotEvents.RESET)
                {
                    if (this.StartIndex == 0 && this.StopIndex == 0)
                    {
                        this.StartIndex = this.MinIndex;
                        this.StopIndex = this.StartIndex + this.HorizontalZoomLimit;
                    }
                }
                else if (plotEvent == PlotEvents.NONE)
                {
                    //no need to change                
                }

                this.prevStart = this.StartIndex;
                this.prevStop = this.StopIndex;

                ViewportArea visible = new ViewportArea(this.StartIndex, this.StopIndex, 0, 0);
                visible = this._Restriction.Apply(visible);
                this.StartIndex = visible.X1;
                this.StopIndex = visible.X2;

                Canvas.SetLeft(this.plotCanvas, -this.parent.ActualWidth);
                Canvas.SetTop(this.plotCanvas, -this.parent.ActualHeight);

                foreach (IPlot c in this.ChartCollection)
                {
                    c.MinIndex = this.MinIndex;
                    c.MaxIndex = this.MaxIndex;

                    if (plotEvent == PlotEvents.FIT || plotEvent == PlotEvents.RESET)
                    {
                        c.StartIndex = 2d * this.StartIndex - this.StopIndex;
                        c.StopIndex = 2d * this.StopIndex - this.StartIndex;
                    }

                    if (this.IsOptimized)
                    {
                        (c as PlotView).HorizontalZoomLimit = this.HorizontalZoomLimit;
                    }
                    //if (StartIndex != StopIndex || PlotEvent == PlotEvents.ZOOM_OUT)
                    c.UpdateViewport(plotEvent, visibleStart, visibleEnd);
                }

                if (this.PlotEvent == PlotEvents.UNDO)
                {
                    if (this.SelectedPlotView != null)
                    {
                        this.StartIndex = this.SelectedPlotView.Viewport.CursorScreenToData(new Point(0, 0)).X;
                        this.StopIndex = this.SelectedPlotView.Viewport.CursorScreenToData(new Point(3d * this.ActualWidth, 0)).X;
                    }
                }

                if (this.PlotEvent == PlotEvents.RESET)
                {
                    this.PlotEvent = PlotEvents.NONE;
                }

                this.IsDirty = false;
                this.Refresh();
            }
        }

        #endregion

        /// <summary>
        /// Refreshes the UI.
        /// </summary>
        private void Refresh()
        {
            foreach (var c in this.ChartCollection)
            {
                c.IsDirty = true;
            }
        }

        public static DependencyProperty IsDirtyProperty =
            DependencyProperty.Register("IsDirty", typeof(bool), typeof(PlotterView),
                new PropertyMetadata(false, new PropertyChangedCallback(IsDirtyPropertyChanged), new CoerceValueCallback(IsDirtyPropertyCoerced)));

        private static object IsDirtyPropertyCoerced(DependencyObject d, object baseValue)
        {
            if (d != null)
            {
                if ((bool)baseValue)
                {
                    PlotterView view = d as PlotterView;
                    view.UpdateViewport(PlotEvents.RESET, new Point(), new Point());
                }
            }
            return baseValue;
        }

        private static void IsDirtyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Intentionally left blank
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
        
        public static DependencyProperty IsOptimizedProperty =
            DependencyProperty.Register("IsOptimized", typeof(bool), typeof(PlotterView),
                new PropertyMetadata(false));

        public bool IsOptimized
        {
            get
            {
                return (bool)this.GetValue(IsOptimizedProperty);
            }
            set
            {
                this.SetValue(IsOptimizedProperty, value);
            }
        }

        public static DependencyProperty HorizontalZoomLimitProperty =
            DependencyProperty.Register("HorizontalZoomLimit", typeof(double), typeof(PlotterView),
                new PropertyMetadata(1e6));

        public double HorizontalZoomLimit
        {
            get
            {
                return (double)this.GetValue(HorizontalZoomLimitProperty);
            }
            set
            {
                this.SetValue(HorizontalZoomLimitProperty, value);
            }
        }

        public static DependencyProperty AutoScaleAndOffsetProperty =
            DependencyProperty.Register("AutoScaleAndOffset", typeof(bool), typeof(PlotterView),
                new PropertyMetadata(false));

        /// <summary>
        /// Plots offset and scale are automatically calculated based on number of waveforms. Default value is false.
        /// </summary>
        public bool AutoScaleAndOffset
        {
            get
            {
                return (bool)this.GetValue(AutoScaleAndOffsetProperty);
            }
            set
            {
                this.SetValue(AutoScaleAndOffsetProperty, value);
            }
        }

        double prevStart;
        double prevStop;

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.plotCanvas.Width = 3d * e.NewSize.Width;
            this.plotCanvas.Height = 3d * e.NewSize.Height;
            Canvas.SetLeft(this.plotCanvas, -e.NewSize.Width);
            Canvas.SetTop(this.plotCanvas, -e.NewSize.Height);

            foreach (var chart in ChartCollection)
                chart.OnSizeChanged(sender, e);

            // Reset the layout when the size changes
            this.UpdateViewport(PlotEvents.RESET, new Point(), new Point());
        }

        public void MoveHCursor1Here(Point point)
        {
        }

        public void MoveHCursor2Here(Point point)
        {
        }

        public void MoveVCursor1Here(Point point)
        {
        }

        public void MoveVCursor2Here(Point point)
        {
        }
    }
}