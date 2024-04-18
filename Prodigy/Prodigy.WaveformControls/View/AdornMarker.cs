// <copyright file="AdornMarker.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>

namespace Prodigy.WaveformControls.View
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using Prodigy.WaveformControls.Navigation;
    using Prodigy.WaveformControls.Viewport;

    public class AdornMarker : Canvas
    { 
        public static readonly DependencyProperty SelectedPlotViewProperty =
            DependencyProperty.Register("SelectedPlotView", typeof(PlotView), typeof(AdornMarker),
                new PropertyMetadata(null, new PropertyChangedCallback(SelectedViewportPropertyChanged)));

        public static readonly DependencyProperty IsDirtyProperty =
            DependencyProperty.Register("IsDirty", typeof(bool),
                typeof(AdornMarker), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(IsDirtyPropertyChanged),
                    new CoerceValueCallback(IsDirtyPropertyCoerced)));

        public static readonly DependencyProperty PlotViewProperty =
            DependencyProperty.Register("PlotView", typeof(PlotView), typeof(AdornMarker),
                new PropertyMetadata(null));

        public static readonly DependencyProperty MouseNavigationProperty =
            DependencyProperty.Register("MouseNavigation", typeof(MouseNavigation),
                typeof(AdornMarker), new FrameworkPropertyMetadata(null,new PropertyChangedCallback(MouseNavigationPropertyChanged)));

        public static DependencyProperty HorizontalZoomLimitProperty =
            DependencyProperty.Register("HorizontalZoomLimit", typeof(double), typeof(AdornMarker),
                new PropertyMetadata(double.NaN));

        public UpdateViewportEventHandler UpdateViewportEvent;

        readonly Panel dockPanel;
        readonly Paner paner;

        double width;
        double height;

        bool panAction = false;
        Point mouseDown;
        double left;
        double top;
        double positionX;
        double positionY;

        public AdornMarker()
        {
            this.ClipToBounds = true;

            Marker marker = new Marker();
            this.dockPanel = marker;
            var leftborder = marker.FindName("leftborder") as Border;
            var topborder = marker.FindName("topborder") as Border;
            var rightborder = marker.FindName("rightborder") as Border;
            var bottomborder = marker.FindName("bottomborder") as Border;

            this.paner = new Paner();
            this.paner.MouseDown += new System.Windows.Input.MouseButtonEventHandler(rect_MouseDown);
            this.paner.MouseMove += new System.Windows.Input.MouseEventHandler(rect_MouseMove);
            this.paner.MouseUp += new System.Windows.Input.MouseButtonEventHandler(rect_MouseUp);
            this.paner.SizeChanged += new SizeChangedEventHandler(paner_SizeChanged);

            leftborder.MouseDown += new System.Windows.Input.MouseButtonEventHandler(border_MouseDown);
            leftborder.MouseMove += new System.Windows.Input.MouseEventHandler(border_MouseMove);
            leftborder.MouseUp += new System.Windows.Input.MouseButtonEventHandler(rect_MouseUp);

            topborder.MouseDown += new System.Windows.Input.MouseButtonEventHandler(border_MouseDown);
            topborder.MouseMove += new System.Windows.Input.MouseEventHandler(border_MouseMove);
            topborder.MouseUp += new System.Windows.Input.MouseButtonEventHandler(rect_MouseUp);

            rightborder.MouseDown += new System.Windows.Input.MouseButtonEventHandler(border_MouseDown);
            rightborder.MouseMove += new System.Windows.Input.MouseEventHandler(border_MouseMove);
            rightborder.MouseUp += new System.Windows.Input.MouseButtonEventHandler(rect_MouseUp);

            bottomborder.MouseDown += new System.Windows.Input.MouseButtonEventHandler(border_MouseDown);
            bottomborder.MouseMove += new System.Windows.Input.MouseEventHandler(border_MouseMove);
            bottomborder.MouseUp += new System.Windows.Input.MouseButtonEventHandler(rect_MouseUp);

            this.Children.Add(this.dockPanel);
            this.Children.Add(this.paner);

            Canvas.SetLeft(this.dockPanel, 0);
            Canvas.SetTop(this.dockPanel, 0);
            this.dockPanel.MouseDown += new System.Windows.Input.MouseButtonEventHandler(rect_MouseDown);
            this.dockPanel.MouseMove += new System.Windows.Input.MouseEventHandler(rect_MouseMove);
            this.dockPanel.MouseUp += new System.Windows.Input.MouseButtonEventHandler(rect_MouseUp);
            this.dockPanel.MinWidth = 5;
            this.dockPanel.MinHeight = 5;
        }

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

        private static void SelectedViewportPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                AdornMarker view = d as AdornMarker;

                view.ClearValue(AdornMarker.IsDirtyProperty);
                if (e.NewValue != null)
                {
                    Binding b = new Binding("IsDirty");
                    b.Source = (e.NewValue as PlotView).DataContext;
                    b.Mode = BindingMode.TwoWay;
                    view.SetBinding(AdornMarker.IsDirtyProperty, b);
                }
            }
        }

        private static object IsDirtyPropertyCoerced(DependencyObject d, object baseValue)
        {
            if (d != null)
            {
                AdornMarker view = d as AdornMarker;
                view.Refresh();
            }

            return baseValue;
        }

        private static void IsDirtyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //do nothing            
        }

        private static void MouseNavigationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                AdornMarker view = d as AdornMarker;
                if ((MouseNavigation)e.NewValue != null)
                {
                    view.UpdateViewportEvent = ((MouseNavigation)e.NewValue).UpdateViewportEvent;
                }
            }
        }

        private void paner_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.paner_SetPosition();
        }

        private void border_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Border border = sender as Border;

                Point p = e.GetPosition(this);
                Vector v = p - this.mouseDown;
                this.positionX = this.left + v.X;
                this.positionY = this.top + v.Y;

                //Limit the width of rectangle to desired HorizontalZoomLimit
                double limitXpixel = this.PlotView.Viewport.DataToCursorScreen(new Point(this.HorizontalZoomLimit, 0)).X -
                                     this.PlotView.Viewport.DataToCursorScreen(new Point(0, 0)).X;

                if (this.positionX < 0)
                {
                    this.positionX = 0;
                }
                if (this.positionY < 0)
                {
                    this.positionY = 0;
                }

                if (border.Name == "leftborder")
                {
                    if (this.width - v.X > 1 && Canvas.GetLeft(this.dockPanel) != this.positionX && this.width - v.X < limitXpixel)
                    {
                        Canvas.SetLeft(this.dockPanel, this.positionX);
                        this.dockPanel.Width = this.width - v.X;
                    }
                }
                else if (border.Name == "topborder")
                {
                    if (this.height - v.Y > 1 && Canvas.GetTop(this.dockPanel) != this.positionY)
                    {
                        Canvas.SetTop(this.dockPanel, this.positionY);
                        this.dockPanel.Height = this.height - v.Y;
                    }
                }
                else if (border.Name == "rightborder")
                {
                    if (this.width + v.X > 1 && this.left + this.width + v.X < this.ActualWidth && this.width + v.X < limitXpixel)
                    {
                        this.dockPanel.Width = this.width + v.X;
                    }
                }
                else if (border.Name == "bottomborder")
                {
                    if (this.height + v.Y > 1 && this.top + this.height + v.Y < this.ActualHeight)
                    {
                        this.dockPanel.Height = this.height + v.Y;
                    }
                }
                else
                {
                    //nothing here
                }

                this.paner_SetPosition();

                e.Handled = true;
            }
        }

        private void border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Border border = sender as Border;
                e.MouseDevice.Capture(sender as IInputElement);
                this.left = Canvas.GetLeft(this.dockPanel);
                this.top = Canvas.GetTop(this.dockPanel);
                this.mouseDown = e.GetPosition(this);
                this.width = this.dockPanel.ActualWidth;
                this.height = this.dockPanel.ActualHeight;

                e.Handled = true;
            }
        }

        private void rect_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.panAction = false;
            e.MouseDevice.Capture(null);

            //Update the plot area content in the rect
            if (this.SelectedPlotView != null && this.PlotView != null)
            {
                Viewport2D viewport = this.SelectedPlotView.Viewport;
                double x1 = Canvas.GetLeft(this.dockPanel);
                double y1 = Canvas.GetTop(this.dockPanel) ;

                double x2 = x1 + this.dockPanel.ActualWidth;
                double y2 = y1 + this.dockPanel.ActualHeight;

                Point p1 = this.PlotView.Viewport.CursorScreenToData(new Point(x1 + this.ActualWidth, y1 + this.ActualHeight));
                Point p2 = this.PlotView.Viewport.CursorScreenToData(new Point(x2 + this.ActualWidth, y2 + this.ActualHeight));
                p1 = viewport.DataToCursorScreen(p1);
                p2 = viewport.DataToCursorScreen(p2);

                if (this.UpdateViewportEvent != null)
                {
                    this.UpdateViewportEvent(PlotEvents.ZOOM_IN, p1, p2);
                }

                e.Handled = true;
            }
        }

        private void rect_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed && this.panAction && !e.Handled)
            {
                Point p = e.GetPosition(this);
                Vector v = p - this.mouseDown;
                this.positionX = this.left + v.X;
                this.positionY = this.top + v.Y;

                //Limit the width of rectangle to desired HorizontalZoomLimit
                double limitXpixel = this.PlotView.Viewport.DataToCursorScreen(new Point(this.HorizontalZoomLimit, 0)).X -
                                     this.PlotView.Viewport.DataToCursorScreen(new Point(0, 0)).X;

                if (this.positionX < 0)
                {
                    this.positionX = 0;
                }
                if (this.positionY < 0)
                {
                    this.positionY = 0;
                }
                if (this.positionX > this.ActualWidth - this.dockPanel.Width && limitXpixel - this.positionX - this.dockPanel.ActualWidth < 0)
                {
                    this.positionX = this.ActualWidth - this.dockPanel.Width;
                }
                if (this.positionY > this.ActualHeight - this.dockPanel.Height)
                {
                    this.positionY = this.ActualHeight - this.dockPanel.Height;
                }
                Canvas.SetLeft(this.dockPanel, this.positionX);
                Canvas.SetTop(this.dockPanel, this.positionY);

                // paner detail set
                this.paner_SetPosition();
            }
        }

        private void rect_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed && !e.Handled)
            {
                e.MouseDevice.Capture(sender as IInputElement);
                this.panAction = true;
                this.mouseDown = e.GetPosition(this);
                this.left = Canvas.GetLeft(this.dockPanel);
                this.top = Canvas.GetTop(this.dockPanel);
            }
        }

        private void Refresh()
        {
            if (this.SelectedPlotView != null && this.PlotView != null)
            {
                Viewport2D viewport = this.SelectedPlotView.Viewport;

                Point p1 = viewport.CursorScreenToData(new Point(viewport.Output.Width / 3d, viewport.Output.Height / 3d));
                Point p2 = viewport.CursorScreenToData(new Point(2d * viewport.Output.Width / 3d, 2d * viewport.Output.Height / 3d));
                p1 = this.PlotView.Viewport.DataToCursorScreen(p1);
                p2 = this.PlotView.Viewport.DataToCursorScreen(p2);
                                
                Vector distance = p2 - p1;

                Canvas.SetLeft(this.dockPanel, p1.X - this.ActualWidth);
                Canvas.SetTop(this.dockPanel, p1.Y - this.ActualHeight);
                this.dockPanel.Width = distance.X;
                this.dockPanel.Height = distance.Y;

                this.paner_SetPosition(p1.X - this.ActualWidth, p1.Y - this.ActualHeight, distance.X, distance.Y);
            }
        }

        private void paner_SetPosition()
        {
            if (this.ActualWidth - Canvas.GetLeft(this.dockPanel) - this.dockPanel.ActualWidth >= this.paner.ActualWidth)
            {
                this.paner.Align = Alignment.RIGHT;
                Canvas.SetLeft(this.paner, Canvas.GetLeft(this.dockPanel) + this.dockPanel.ActualWidth);
            }
            else
            {
                this.paner.Align = Alignment.LEFT;
                Canvas.SetLeft(this.paner, Canvas.GetLeft(this.dockPanel) - this.paner.ActualWidth);
            }

            Canvas.SetTop(this.paner, (2d * Canvas.GetTop(this.dockPanel) + this.dockPanel.ActualHeight - this.paner.ActualHeight) / 2d);
        }

        private void paner_SetPosition(double left, double top, double width, double height)
        {
            if (this.ActualWidth - left - width >= this.paner.ActualWidth)
            {
                this.paner.Align = Alignment.RIGHT;
                Canvas.SetLeft(this.paner, left + width);
            }
            else
            {
                this.paner.Align = Alignment.LEFT;
                Canvas.SetLeft(this.paner, left - this.paner.ActualWidth);
            }

            Canvas.SetTop(this.paner, (2d * top + height - this.paner.ActualHeight) / 2d);
        }
    }
}