// <copyright file="BusView.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>

namespace Prodigy.WaveformControls.View
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Prodigy.WaveformControls.Extensions;
    using Prodigy.WaveformControls.Interfaces;

    /// <summary>
    /// This Class defines the Bus Structure
    /// </summary>
    public class BusView : PlotView
    {
        public static DependencyProperty DataProperty =
            DependencyProperty.Register("DataProperty", typeof(IEnumerable<IBusData>), typeof(BusView),
                new PropertyMetadata(null,new PropertyChangedCallback(OnDataPropertyChanged),new CoerceValueCallback(OnDataPropertyCoerce)));

        public static DependencyProperty BusHeightProperty =
            DependencyProperty.Register("BusHeight", typeof(double), typeof(BusView),
                new PropertyMetadata(24d));

        public BusView(FrameworkElement plotter) : base(plotter)
        { 
        }

        public double BusHeight
        {
            get
            {
                return (double)this.GetValue(BusHeightProperty);
            }
            set
            {
                this.SetValue(BusHeightProperty, value);
            }
        }

        public IEnumerable<IBusData> Data
        {
            get
            {
                return (IEnumerable<IBusData>)this.GetValue(DataProperty);
            }
            set
            {
                this.SetValue(DataProperty, value);
            }
        }

        protected override void Refresh()
        {
            UndoItem undoitem = new UndoItem(
                this.Viewport.Visible.X1, this.Viewport.Visible.X2,
                this.Viewport.Visible.Y1, this.Viewport.Visible.Y2,
                this.Offset, this.Scale,
                this.DataOffset, this.DataScale);
            if (this.Viewport.Visible.IsValid() && !this._IsUndoEnabled)
            {
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

            if (this.bitmap != null)
            {
                lock (this.bitmap)
                {
                    this.bitmap.Clear();

                    this._IsUndoEnabled = false;

                    if (this.IsShowing)
                    {
                        Color c = (Application.Current.Resources[this.Channel.ToString()] as SolidColorBrush).Color;
                        try
                        {
                            this.bitmap.Lock();
                            this.BusHeight = this.Viewport.DataToScreen(new Point(0, -0.5)).Y - this.Viewport.DataToScreen(new Point(0, 0.5)).Y;
                            Style textblockdefaultstyle = Application.Current.FindResource(typeof(Label)) as Style;
                            Label t = new Label();
                            t.Style = textblockdefaultstyle;
                            t.FontFamily = new FontFamily("Roboto");
                            t.FontSize = 9.5;

                            double yOffset = this.Viewport.DataToScreen(new Point(0, 0.5)).Y;

                            //Write the logic to draw the bus daigram
                            foreach (IBusData busdata in this.data)
                            {
                                double startPx = this.Viewport.DataToScreen(new Point(busdata.StartIndex, 0)).X;
                                double stopPx = this.Viewport.DataToScreen(new Point(busdata.StopIndex, 0)).X;

                                // Bus lies outside the drawable area
                                if (startPx > 0 && startPx < this.ActualWidth && stopPx > 0 && stopPx < this.ActualWidth &&
                                    (yOffset - this.BusHeight / 2d) > 0 && (yOffset + BusHeight / 2d) < this.ActualHeight)
                                {
                                    Point[] polygon = new Point[7];
                                    polygon[0] = new Point(startPx, yOffset);
                                    polygon[1] = new Point(startPx + 0.05d * (stopPx - startPx), yOffset - this.BusHeight / 2d);
                                    polygon[2] = new Point(stopPx - 0.05 * (stopPx - startPx), yOffset - this.BusHeight / 2d);
                                    polygon[3] = new Point(stopPx, yOffset);
                                    polygon[4] = new Point(stopPx - 0.05 * (stopPx - startPx), yOffset + this.BusHeight / 2d);
                                    polygon[5] = new Point(startPx + 0.05d * (stopPx - startPx), yOffset + this.BusHeight / 2d);
                                    polygon[6] = new Point(startPx, yOffset);

                                    this.bitmap.DrawPolyline(polygon, c);
                                    var text = new FormattedText(busdata.ToString(), new CultureInfo("en-us"), FlowDirection.LeftToRight,
                                        new Typeface(t.FontFamily, t.FontStyle, t.FontWeight, t.FontStretch),
                                        t.FontSize, busdata.Brush, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                                    if (polygon[2].X - polygon[1].X > text.Width && polygon[5].Y - polygon[1].Y > text.Height)
                                    {
                                        this.bitmap.DrawText(text, new Point((polygon[1].X + polygon[2].X - text.Width) / 2d, yOffset - text.Height / 2d));
                                    }
                                }
                            }

                            this.bitmap.Unlock();
                        }
                        catch
                        {
                            this.bitmap.Unlock();
                        }
                    }
                }
            }

            this.Source = this.bitmap;
        }

        /// <summary>
        /// Issue - By the time dependency property is updated we try to update the bus bitmap. This causes issue with bus generation.
        /// </summary>
        private IEnumerable<IBusData> data = new List<IBusData>();
        private static object OnDataPropertyCoerce(DependencyObject d, object baseValue)
        {
            if (d != null && baseValue != null)
            {
                BusView view = (BusView)d;
                if (view.DataContext != null)
                {
                    view.data = (IEnumerable<IBusData>)baseValue;
                    view.Viewport.Visible.X1 = view.StartIndex;
                    view.Viewport.Visible.X2 = view.StopIndex;
                    view.Refresh();// do not change
                }
            }

            return baseValue;
        }

        private static void OnDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}