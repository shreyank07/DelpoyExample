// <copyright file="TriggerMarkerView.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>

namespace Prodigy.WaveformControls.View
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using Prodigy.WaveformControls.Viewport;

    public class TriggerMarkerView : Canvas
    {
        public static readonly DependencyProperty SelectedPlotViewProperty =
            DependencyProperty.Register("SelectedPlotView", typeof(PlotView), typeof(TriggerMarkerView),
                new PropertyMetadata(null, new PropertyChangedCallback(SelectedViewportPropertyChanged)));

        public static readonly DependencyProperty IsDirtyProperty =
            DependencyProperty.Register("IsDirty", typeof(bool),
                typeof(TriggerMarkerView), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(IsDirtyPropertyChanged),
                    new CoerceValueCallback(IsDirtyPropertyCoerced)));

        public static readonly DependencyProperty MarkerHeightProperty =
            DependencyProperty.Register("MarkerHeight", typeof(double),
                typeof(TriggerMarkerView), new FrameworkPropertyMetadata(5D));

        public static readonly DependencyProperty MarkerWidthProperty =
            DependencyProperty.Register("MarkerWidth", typeof(double),
                typeof(TriggerMarkerView), new FrameworkPropertyMetadata(5D));

        public static readonly DependencyProperty TriggerIndexProperty =
            DependencyProperty.Register("TriggerIndex", typeof(double),
                typeof(TriggerMarkerView), new FrameworkPropertyMetadata(0D));

        readonly TextBlock shape;

        public TriggerMarkerView()
        {
            this.shape = new TextBlock();
            this.shape.Text = "T";
            this.shape.FontSize = 20;
            this.shape.FontFamily = new FontFamily("Impact");
            this.shape.Background = Brushes.Transparent;
            this.shape.Foreground = Brushes.DarkRed;
            this.Children.Add(this.shape);
            Canvas.SetLeft(this.shape, 0);
            Canvas.SetBottom(this.shape, 0);
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

        public double MarkerHeight
        {
            get
            {
                return (double)this.GetValue(MarkerHeightProperty);
            }
            set
            {
                this.SetValue(MarkerHeightProperty, value);
            }
        }

        public double MarkerWidth
        {
            get
            {
                return (double)this.GetValue(MarkerWidthProperty);
            }
            set
            {
                this.SetValue(MarkerWidthProperty, value);
            }
        }

        public double TriggerIndex
        {
            get
            {
                return (double)this.GetValue(TriggerIndexProperty);
            }
            set
            {
                this.SetValue(TriggerIndexProperty, value);
            }
        }

        private static void SelectedViewportPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                TriggerMarkerView view = d as TriggerMarkerView;

                view.ClearValue(TriggerMarkerView.IsDirtyProperty);
                if (e.NewValue != null)
                {
                    Binding b = new Binding("IsDirty");
                    b.Source = (e.NewValue as PlotView).DataContext;
                    b.Mode = BindingMode.TwoWay;
                    view.SetBinding(TriggerMarkerView.IsDirtyProperty, b);
                    view.Refresh();
                }
                //25th Feb 2015 @Girish Chandra Mohanta
                //Refresh the trigger point when wave form is not there
                else
                {
                    view.Refresh();
                }
            }
        }

        private static object IsDirtyPropertyCoerced(DependencyObject d, object baseValue)
        {
            if (d != null)
            {
                TriggerMarkerView view = d as TriggerMarkerView;
                view.Refresh();
            }

            return baseValue;
        }

        private static void IsDirtyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //do nothing            
        }

        private void Refresh()
        {
            if (this.SelectedPlotView != null)
            {
                Viewport2D viewport = this.SelectedPlotView.Viewport;
                Point p = viewport.DataToCursorScreen(new Point(this.TriggerIndex, 0));

                Canvas.SetTop(this.shape, this.ActualHeight - this.shape.ActualHeight);
                if (double.IsInfinity(p.X))
                {
                    Canvas.SetLeft(this.shape, 0);
                    this.shape.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Canvas.SetLeft(this.shape, p.X - this.ActualWidth - this.shape.ActualWidth / 2d);
                    this.shape.Visibility = Visibility.Visible;
                }
            }
            //25 th Feb 2015 @Girish Chandra Mohanta
            //Reset the trigger point when wave form is not there
            else
            {
                Canvas.SetTop(this.shape, this.ActualHeight - this.shape.ActualHeight);
                Canvas.SetLeft(this.shape, 0);
            }
        }
    }
}