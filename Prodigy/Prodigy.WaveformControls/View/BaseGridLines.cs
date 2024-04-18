// <copyright file="BaseGridLines.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>

namespace Prodigy.WaveformControls.View
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public class BaseGridLines : Canvas
    {
        public static readonly DependencyProperty PlotViewProperty =
            DependencyProperty.Register("PlotView", typeof(PlotView), typeof(BaseGridLines),
                new PropertyMetadata(null, new PropertyChangedCallback(PlotViewPropertyChanged)));

        public static DependencyProperty IsDirtyProperty =
            DependencyProperty.Register("IsDirty", typeof(bool), typeof(BaseGridLines),
                new PropertyMetadata(false, new PropertyChangedCallback(IsDirtyPropertyChanged), new CoerceValueCallback(IsDirtyPropertyCoerced)));

        public static DependencyProperty VerticalDivisionCountProperty = DependencyProperty.Register("VerticalDivisionCount", typeof(int),
            typeof(BaseGridLines), new PropertyMetadata(10));

        public static DependencyProperty HorizontalDivisionCountProperty = DependencyProperty.Register("HorizontalDivisionCount", typeof(int),
            typeof(BaseGridLines), new PropertyMetadata(10));

        public BaseGridLines()
        {
        }

        public PlotView PlotView
        {
            get
            {
                return (PlotView)this.GetValue(PlotViewProperty);
            }
            set
            {
                this.SetValue(PlotViewProperty, value);
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

        public int VerticalDivisionCount
        {
            get
            {
                return (int)this.GetValue(VerticalDivisionCountProperty);
            }
            set
            {
                this.SetValue(VerticalDivisionCountProperty, value);
            }
        }

        public int HorizontalDivisionCount
        {
            get
            {
                return (int)this.GetValue(HorizontalDivisionCountProperty);
            }
            set
            {
                this.SetValue(HorizontalDivisionCountProperty, value);
            }
        }

        private static void PlotViewPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                BaseGridLines view = d as BaseGridLines;
                //update path geomety according to zoom level
                view.Refresh();
            }
        }

        private static object IsDirtyPropertyCoerced(DependencyObject d, object baseValue)
        {
            //25 th Feb 2015 @Girish Chandra Mohanta
            //Refresh the original grid when wave form is not there
            if (d != null)
            {
                BaseGridLines view = d as BaseGridLines;
                view.Refresh();
            }
            return false;
        }

        private static void IsDirtyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private void Refresh()
        {
            double ratio = 1;
            if (this.PlotView != null)
            {
                ratio = (-this.PlotView.Viewport.DataToCursorScreen(new Point(0, this.PlotView.Scale * 5d)).Y +
                         this.PlotView.Viewport.DataToCursorScreen(new Point(0, -this.PlotView.Scale * 5d)).Y) / this.ActualHeight;

                foreach (Path p in this.Children.OfType<Path>())
                {
                    if (!(p.RenderTransform is TransformGroup))
                    {
                        TransformGroup g = new TransformGroup();
                        g.Children.Add(new TranslateTransform());
                        g.Children.Add(new ScaleTransform(1, 1, 0, 0));
                        p.RenderTransform = g;
                    }

                    ScaleTransform scale = ((TransformGroup)p.RenderTransform).Children.OfType<ScaleTransform>().First();
                    TranslateTransform translate = ((TransformGroup)p.RenderTransform).Children.OfType<TranslateTransform>().First();

                    translate.Y = -Prodigy.Business.Extensions.CustomExtensions.GetEquivalent(0, this.ActualHeight, this.PlotView.Viewport.Visible.Y1, this.PlotView.Viewport.Visible.Y2,
                        this.PlotView.Viewport.Visible.Y1);
                    scale.ScaleX = 1;
                    scale.ScaleY = ratio;
                }
            }
            //25 th Feb 2015 @Girish Chandra Mohanta
            //Refresh the original grid when wave form is not there
            else
            {
                foreach (Path p in this.Children.OfType<Path>())
                {
                    if (!(p.RenderTransform is TransformGroup))
                    {
                        TransformGroup g = new TransformGroup();
                        g.Children.Add(new TranslateTransform());
                        g.Children.Add(new ScaleTransform(1, 1, 0, 0));
                        p.RenderTransform = g;
                    }

                    ScaleTransform scale = ((TransformGroup)p.RenderTransform).Children.OfType<ScaleTransform>().First();
                    TranslateTransform translate = ((TransformGroup)p.RenderTransform).Children.OfType<TranslateTransform>().First();

                    translate.Y = 0d;
                    scale.ScaleX = 1;
                    scale.ScaleY = ratio;
                }
            }
        }
    }
}