namespace Prodigy.WaveformControls.View
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Prodigy.Business;
    using Prodigy.WaveformControls;
    using Prodigy.WaveformControls.Extensions;
    using Prodigy.WaveformControls.Interfaces;
    using Prodigy.WaveformControls.Viewport;

    public class DigitalPlotView : PlotView
    {
        public static DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(ObservableDictionary<Digital, List<Point>>), typeof(DigitalPlotView),
                new PropertyMetadata(null,new PropertyChangedCallback(OnDataPropertyChanged),new CoerceValueCallback(OnDataPropertyCoerce)));

        public DigitalPlotView(FrameworkElement plotter)
            : base(plotter)
        { 
        }

        public ObservableDictionary<Digital, List<Point>> Data
        {
            get
            {
                return (ObservableDictionary<Digital, List<Point>>)this.GetValue(DataProperty);
            }
            set
            {
                this.SetValue(DataProperty, value);
            }
        }

        protected override void Refresh()
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
                            //Write the logic to draw the Digital waveform

                            List<Point> pnts = new List<Point>();
                            var pEnumerater = this.Data.GetEnumerator();
                            while (pEnumerater.MoveNext())
                            {
                                var points = pEnumerater.Current.Value;

                                Point currentPoint;
                                Point nextPoint;
                                int totalPts = points.Count();
                                pnts.Clear();
                                for (int i = 0; i < totalPts; i++)
                                {
                                    if (i >= totalPts)
                                    {
                                        currentPoint = this.Viewport.DataToScreen(points.ElementAt(i));
                                        nextPoint = this.Viewport.DataToScreen(points.ElementAt(i + 1));
                                        pnts.Add(currentPoint);
                                        pnts.Add(new Point(nextPoint.X, currentPoint.Y));
                                    }
                                    else
                                    {
                                        pnts.Add(this.Viewport.DataToScreen(points.ElementAt(i)));
                                    }
                                }

                                this.bitmap.DrawPolyline(pnts.ToArray(), Colors.White);
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

        private static object OnDataPropertyCoerce(DependencyObject d, object baseValue)
        {
            if (d != null)
            {
                DigitalPlotView view = d as DigitalPlotView;
                view.Viewport.Visible.X1 = view.StartIndex;
                view.Viewport.Visible.X2 = view.StopIndex;
                view.Refresh();
            }

            return baseValue;
        }

        private static void OnDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}