// <copyright file="MouseNavigation.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>
  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Prodigy.WaveformControls.View;

namespace Prodigy.WaveformControls.Navigation
{
    /// <summary>Provides common methods of mouse navigation around viewport</summary>
    public class MouseNavigation : Canvas
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseNavigation"/> class.
        /// </summary>
        public MouseNavigation()
        {
        }

        public static readonly DependencyProperty PlotEventProperty =
            DependencyProperty.Register("PlotEvent", typeof(PlotEvents),
                typeof(MouseNavigation), new FrameworkPropertyMetadata(PlotEvents.NONE));

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

        public bool MultiZoomDisabled = false;

        private AdornerLayer adornerLayer;

        protected AdornerLayer AdornerLayer
        {
            get
            {
                if (this.adornerLayer == null)
                {
                    this.adornerLayer = AdornerLayer.GetAdornerLayer(this);
                    if (this.adornerLayer != null)
                    {
                        this.adornerLayer.IsHitTestVisible = false;
                    }
                }

                return this.adornerLayer;
            }
        }

        readonly List<UIElement> Plotter = new List<UIElement>();
        double left;
        double top;

        public void AttachPlotter(UIElement plotter)
        {
            this.Plotter.Add(plotter);
            Mouse.AddMouseDownHandler(this.Parent, OnMouseDown);
            Mouse.AddMouseMoveHandler(this.Parent, OnMouseMove);
            Mouse.AddMouseUpHandler(this.Parent, OnMouseUp);
            Mouse.AddMouseWheelHandler(this.Parent, OnMouseWheel);
        }

        public void DetachPlotter()
        {
            this.Plotter.Clear();
            Mouse.RemoveMouseDownHandler(this.Parent, OnMouseDown);
            Mouse.RemoveMouseMoveHandler(this.Parent, OnMouseMove);
            Mouse.RemoveMouseUpHandler(this.Parent, OnMouseUp);
            Mouse.RemoveMouseWheelHandler(this.Parent, OnMouseWheel);
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                Point mousePos = e.GetPosition(this);
                int delta = -e.Delta;
                this.MouseWheelZoom(mousePos, delta);

                e.Handled = true;
            }
        }

        #if DEBUG
        public override string ToString()
        {
            if (!String.IsNullOrEmpty(this.Name))
            {
                return this.Name;
            }
            return base.ToString();
        }

        #endif

        bool adornerAdded;
        RectangleSelectionAdorner selectionAdorner;

        private void AddSelectionAdorner()
        {
            if (!this.adornerAdded)
            {
                AdornerLayer layer = this.AdornerLayer;
                if (layer != null)
                {
                    this.selectionAdorner = new RectangleSelectionAdorner(this) { Border = this.zoomRect };

                    layer.Add(this.selectionAdorner);
                    this.adornerAdded = true;
                }
            }
        }

        private void RemoveSelectionAdorner()
        {
            AdornerLayer layer = this.AdornerLayer;
            if (layer != null)
            {
                layer.Remove(this.selectionAdorner);
                this.adornerAdded = false;
            }
        }

        private void UpdateSelectionAdorner()
        {
            this.selectionAdorner.Border = this.zoomRect;
            this.selectionAdorner.InvalidateVisual();
        }

        Rect? zoomRect = null;
        private const double wheelZoomSpeed = 1.2;
        private bool shouldKeepRatioWhileZooming;

        private readonly PlotEvents _PlotEvent = PlotEvents.NONE;

        private Point startPoint;
        private Point endPoint;

        protected virtual bool ShouldStartPanning(MouseButtonEventArgs e)
        {
            return e.ChangedButton == MouseButton.Left && this.PlotEvent == PlotEvents.PAN;
        }

        protected virtual bool ShouldStartZoom(MouseButtonEventArgs e)
        {
            return e.ChangedButton == MouseButton.Left && this.PlotEvent == PlotEvents.ZOOM_IN;
        }

        protected virtual void StartPanning(MouseButtonEventArgs e)
        {
            this.startPoint = e.GetPosition(this.Parent as UIElement);
            this.CaptureMouse();
        }

        protected virtual void StartZoom(MouseButtonEventArgs e)
        {
            this.startPoint = e.GetPosition(this.Parent as UIElement);
            if (this.MultiZoomDisabled)
            {
                this.startPoint.Y = 0d;
            }

            //if (Viewport.Output.Contains(zoomStartPoint))
            {
                this.AddSelectionAdorner();
                this.CaptureMouse();
                this.shouldKeepRatioWhileZooming = Keyboard.Modifiers == ModifierKeys.Shift;
            }
        }

        private bool canAction;

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Handled == false)
            {
                this.canAction = true;
                this.left = Canvas.GetLeft(this.Plotter[0]);
                this.top = Canvas.GetTop(this.Plotter[0]);

                // dragging
                bool shouldStartDrag = this.ShouldStartPanning(e);
                if (shouldStartDrag)
                {
                    this.StartPanning(e);
                }

                // zooming
                bool shouldStartZoom = this.ShouldStartZoom(e);
                if (shouldStartZoom)
                {
                    this.StartZoom(e);
                }

                ((IInputElement)this.Parent).Focus();
            }
        }

        readonly System.Windows.Media.TranslateTransform t = new System.Windows.Media.TranslateTransform(0,0); 

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!this.canAction)
            {
                return;
            }

            this.endPoint = e.GetPosition(this.Parent as UIElement);
            // dragging
            if (this.PlotEvent == PlotEvents.PAN && e.LeftButton == MouseButtonState.Pressed)
            { 
                Vector shift = this.endPoint - this.startPoint;

                // preventing unnecessary changes, if actually visible hasn't change.
                if (shift.X != 0 || shift.Y != 0)
                {
                    //set min/max limit for canvas left
                    if (this.left + shift.X > 0)
                    {
                        shift.X = - this.left;
                    }
                    if (this.left + shift.X < -(this.Plotter[0] as FrameworkElement).ActualWidth * (2.0 / 3.0))
                    {
                        shift.X = -(this.Plotter[0] as FrameworkElement).ActualWidth * (2.0 / 3.0) - this.left;
                    }

                    //set min/max limit for canvas top
                    if (this.top + shift.Y > 0)
                    {
                        shift.Y = -this.top;
                    }
                    if (this.top + shift.Y < -(this.Plotter[0] as FrameworkElement).ActualHeight * (2.0 / 3.0))
                    {
                        shift.Y = -(this.Plotter[0] as FrameworkElement).ActualHeight * (2.0 / 3.0) - this.top;
                    }

                    foreach (var p in this.Plotter)
                    {
                        Canvas.SetLeft(p, this.left + shift.X);
                        Canvas.SetTop(p, this.top + shift.Y);
                    }
                }
            }
            // zooming
            else if (this.PlotEvent == PlotEvents.ZOOM_IN && e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.MultiZoomDisabled)
                {
                    this.endPoint.Y = (this.Parent as FrameworkElement).ActualWidth;
                }

                if (this.endPoint.X < 0)
                {
                    this.endPoint.X = 0;
                }
                if (this.endPoint.Y < 0)
                {
                    this.endPoint.Y = 0;
                }
                if (this.endPoint.X > (this.Parent as FrameworkElement).ActualWidth)
                {
                    this.endPoint.X = (this.Parent as FrameworkElement).ActualWidth;
                }
                if (this.endPoint.Y > (this.Parent as FrameworkElement).ActualHeight)
                {
                    this.endPoint.Y = (this.Parent as FrameworkElement).ActualHeight;
                }

                this.UpdateZoomRect(endPoint);
            }
        }

        private void UpdateZoomRect(Point zoomEndPoint)
        {
            Rect tmpZoomRect = new Rect(this.startPoint, zoomEndPoint);
            this.zoomRect = tmpZoomRect;
            this.UpdateSelectionAdorner();
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            //Point endPoint = e.GetPosition(this.Parent as UIElement);            
            if (this.UpdateViewportEvent != null && this.canAction)
            {
                this.canAction = false;                              
                
                Rect rect = new Rect(this.startPoint, this.endPoint);
                if (this.PlotEvent == PlotEvents.ZOOM_IN)
                {
                    if ((this.endPoint - this.startPoint).X > 1 && (this.endPoint - this.startPoint).Y > 1)
                    {
                        this.startPoint.X -= this.left;
                        this.startPoint.Y -= this.top;
                        this.endPoint.X -= this.left;
                        this.endPoint.Y -= this.top;
                        this.UpdateViewportEvent(this.PlotEvent, this.startPoint, this.endPoint);
                    }
                    else if ((this.startPoint - this.endPoint).X > 1 && (this.endPoint - this.startPoint).Y > 1)
                    {
                        this.startPoint.X -= this.left;
                        this.startPoint.Y -= this.top;
                        this.endPoint.X -= this.left;
                        this.endPoint.Y -= this.top;
                        this.UpdateViewportEvent(this.PlotEvent, this.endPoint, this.startPoint);
                    }
                }
                else
                {
                    this.startPoint.X -= this.left;
                    this.startPoint.Y -= this.top;
                    this.endPoint.X -= this.left;
                    this.endPoint.Y -= this.top;
                    this.UpdateViewportEvent(this.PlotEvent, this.startPoint, this.endPoint);
                }

                foreach (var p in this.Plotter)
                {
                    Canvas.SetLeft(p, this.left);
                    Canvas.SetTop(p, this.top);
                }
            }

            this.OnParentMouseUp(e);
        }

        public UpdateViewportEventHandler UpdateViewportEvent;

        protected virtual void OnParentMouseUp(MouseButtonEventArgs e)
        {
            if (this.PlotEvent == PlotEvents.PAN && e.ChangedButton == MouseButton.Left)
            {
                this.StopPanning(e);
            }
            else if (this.PlotEvent == PlotEvents.ZOOM_IN && e.ChangedButton == MouseButton.Left)
            {
                this.StopZooming();
            }
        }

        protected virtual void StopZooming()
        {
            if (this.zoomRect.HasValue)
            {
                this.zoomRect = null;
                this.ReleaseMouseCapture();
                this.RemoveSelectionAdorner();
            }
        }

        protected virtual void StopPanning(MouseButtonEventArgs e)
        {
            this.ReleaseMouseCapture();
            this.ClearValue(CursorProperty);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (this.PlotEvent != PlotEvents.ZOOM_IN)
            {
                this.RemoveSelectionAdorner();
            }
            this.ReleaseMouseCapture();
            base.OnLostFocus(e);
        }

        private void MouseWheelZoom(Point mousePos, int wheelRotationDelta)
        {
            //Point zoomTo = mousePos.ScreenToViewport(Viewport.Transform);
            //double zoomSpeed = Math.Abs(wheelRotationDelta / Mouse.MouseWheelDeltaForOneLine);
            //zoomSpeed *= wheelZoomSpeed;
            //if (wheelRotationDelta < 0)
            //{
            //    zoomSpeed = 1 / zoomSpeed;
            //}
            //Viewport.Visible = Viewport.Visible.Zoom(zoomTo, zoomSpeed);
        }
    }
}