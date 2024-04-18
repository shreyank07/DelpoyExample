// <copyright file="CursorView.xaml.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>
  
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Prodigy.Business;

namespace Prodigy.WaveformControls.View
{
    /// <summary>
    /// Interaction logic for CursorView.xaml
    /// </summary>
    public partial class CursorView : UserControl
    {
        public static readonly DependencyProperty BackColorProperty =
            DependencyProperty.Register("BackColor", typeof(Brush),
                typeof(CursorView), new FrameworkPropertyMetadata(Brushes.Red));

        public static readonly DependencyProperty CursorTypeProperty =
            DependencyProperty.Register("CursorType", typeof(CursorEnum),
                typeof(CursorView), new FrameworkPropertyMetadata(CursorEnum.OFF));

        public static readonly DependencyProperty ForeColorProperty =
            DependencyProperty.Register("ForeColor", typeof(Brush),
                typeof(CursorView), new FrameworkPropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string),
                typeof(CursorView), new FrameworkPropertyMetadata("Cursor"));

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(double),
                typeof(CursorView), new FrameworkPropertyMetadata(0.0, OnPositionPropertyChanged));

        //WfmIndex or Voltage value
        public static readonly DependencyProperty CursorPositionProperty =
            DependencyProperty.Register("CursorPosition", typeof(double),
                typeof(CursorView), new FrameworkPropertyMetadata(0.0, CursorPositionPropertyChanged));

        public static readonly DependencyProperty SelectedPlotViewProperty =
            DependencyProperty.Register("SelectedPlotView", typeof(PlotView), typeof(CursorView),
                new PropertyMetadata(null, new PropertyChangedCallback(SelectedViewportPropertyChanged)));

        public static readonly DependencyProperty IsDirtyProperty =
            DependencyProperty.Register("IsDirty", typeof(bool),
                typeof(CursorView), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(IsDirtyPropertyChanged),
                    new CoerceValueCallback(IsDirtyPropertyCoerced)));

        private bool cursorMoving;

        private Point mouseDownPosition;

        public CursorView()
        {
            this.InitializeComponent();
        }

        public Brush BackColor
        {
            get
            {
                return (Brush)this.GetValue(BackColorProperty);
            }
            set
            {
                this.SetValue(BackColorProperty, value);
            }
        }

        public CursorEnum CursorType
        {
            get
            {
                return (CursorEnum)this.GetValue(CursorTypeProperty);
            }
            set
            {
                this.SetValue(CursorTypeProperty, value);
            }
        }

        public Brush ForeColor
        {
            get
            {
                return (Brush)this.GetValue(ForeColorProperty);
            }
            set
            {
                this.SetValue(ForeColorProperty, value);
            }
        }

        public string Caption
        {
            get
            {
                return this.GetValue(CaptionProperty).ToString();
            }
            set
            {
                this.SetValue(CaptionProperty, value);
            }
        }

        public double Position
        {
            get
            {
                return (double)this.GetValue(PositionProperty);
            }
            set
            {
                this.SetValue(PositionProperty, value);
            }
        }

        public double CursorPosition
        {
            get
            {
                return (double)this.GetValue(CursorPositionProperty);
            }
            set
            {
                this.SetValue(CursorPositionProperty, value);
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

        private static void OnPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CursorView view = (CursorView)d;
            view.Position = (double)e.NewValue;

            if (view.Parent != null)
            {
                FrameworkElement parent = view.Parent as FrameworkElement;
                if (view.CursorType == CursorEnum.V_BAR)
                {
                    Thickness margin = view.Margin;
                    margin.Left = parent.ActualWidth * view.Position;
                    margin.Top = 0;
                    view.Margin = margin;

                    view.Height = parent.ActualHeight;
                }
                else if (view.CursorType == CursorEnum.H_BAR)
                {
                    Thickness margin = view.Margin;
                    margin.Left = 0;
                    margin.Top = parent.ActualHeight * view.Position;
                    view.Margin = margin;

                    view.Width = parent.ActualWidth;
                }
            }

            view.Refresh();
        }

        private static void CursorPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Changes the pixel position of the cursor
            if (d != null)
            {
                CursorView view = d as CursorView;
                view.ReSetCursorPosition();
            }
        }

        private static void SelectedViewportPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //
            if (d != null)
            {
                CursorView view = d as CursorView;

                view.ClearValue(CursorView.IsDirtyProperty);
                if (e.NewValue != null)
                {
                    Binding b = new Binding("IsDirty");
                    b.Source = (e.NewValue as PlotView).DataContext;
                    b.Mode = BindingMode.TwoWay;
                    view.SetBinding(CursorView.IsDirtyProperty, b);
                }
            }
        }

        private static object IsDirtyPropertyCoerced(DependencyObject d, object baseValue)
        {
            if (d != null)
            {
                CursorView view = d as CursorView;
                view.ReSetCursorPosition();
            }

            return baseValue;
        }

        private static void IsDirtyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //do nothing
            if (d != null)
            {
                CursorView view = d as CursorView;
            }
        }

        private void Path_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Cross;
        }

        private void Path_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.path.CaptureMouse();
                this.mouseDownPosition = e.GetPosition(this);
                this.cursorMoving = true;
                e.Handled = true;
            }
        }

        private void Path_MouseUp(object sender, MouseEventArgs e)
        {
            this.path.ReleaseMouseCapture();
            this.cursorMoving = false;
        }

        private void Path_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.cursorMoving)
                {
                    Point loc1 = e.GetPosition(this.Parent as FrameworkElement);
                    double width = (this.Parent as FrameworkElement).ActualWidth;
                    double height = (this.Parent as FrameworkElement).ActualHeight;
                    Thickness margin = this.Margin;
                    if (this.CursorType == CursorEnum.V_BAR)
                    {
                        margin.Left = loc1.X - this.mouseDownPosition.X;
                        if (margin.Left < 0)
                        {
                            margin.Left = 0;
                        }
                        if (margin.Left > width)
                        {
                            margin.Left = width - 2;
                        }

                        if (this.SelectedPlotView == null)
                        {
                            this.Margin = margin;
                        }
                        else
                        {
                            //change the cursor positions
                            Point p = this.SelectedPlotView.Viewport.ScreenToData(new Point(margin.Left + width, 0));
                            this.CursorPosition = p.X;
                        }
                    }
                    else if (this.CursorType == CursorEnum.H_BAR)
                    {
                        margin.Top = loc1.Y - this.mouseDownPosition.Y;
                        if (margin.Top < 0)
                        {
                            margin.Top = 0;
                        }
                        if (margin.Top > height)
                        {
                            margin.Top = height - 2;
                        }

                        if (this.SelectedPlotView == null)
                        {
                            this.Margin = margin;
                        }
                        else
                        {
                            //change the cursor positions
                            Point p = this.SelectedPlotView.Viewport.ScreenToData(new Point(0, margin.Top + height));
                            this.CursorPosition = p.Y;
                        }
                    }

                    e.Handled = true;
                }
            }
        }

        private void Path_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = null;
        }

        private void CustomCursor_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Parent != null)
            {
                if (this.CursorType == CursorEnum.V_BAR)
                {
                    this.Height = e.NewSize.Height;
                }
                else if (this.CursorType == CursorEnum.H_BAR)
                {
                    this.Width = e.NewSize.Width;
                }

                this.Refresh();
            }
        }

        private void Refresh()
        {
            //////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////Refresh Geometry of the Cursor
            //////////////////////////////////////////////////////////////////////////////////////////////////
            PathFigure pathFigure;
            PolyLineSegment segment;
            //update the geomety
            pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(0, 0);
            segment = new PolyLineSegment();

            if (this.CursorType == CursorEnum.V_BAR)
            {
                Thickness margin = this.lbl.Margin;
                margin.Left = 0;
                this.lbl.Margin = margin;

                segment.Points.Add(new Point(0, this.ActualHeight));
                segment.Points.Add(new Point(2, this.ActualHeight));
                segment.Points.Add(new Point(2, this.lbl.ActualHeight));
                segment.Points.Add(new Point(this.lbl.ActualWidth, this.lbl.ActualHeight));
                segment.Points.Add(new Point(this.lbl.ActualWidth, 0));

                pathFigure.Segments.Add(segment);
                this.path.Fill = this.BackColor;
                this.path.Data = Geometry.Parse(pathFigure.ToString());
            }
            else if (this.CursorType == CursorEnum.H_BAR)
            {
                Thickness margin = this.lbl.Margin;
                margin.Left = this.ActualWidth - this.lbl.ActualWidth;
                this.lbl.Margin = margin;

                segment.Points.Add(new Point(0, 2));
                segment.Points.Add(new Point(this.ActualWidth - this.lbl.ActualHeight, 2));
                segment.Points.Add(new Point(this.ActualWidth - this.lbl.ActualHeight, this.lbl.ActualWidth));
                segment.Points.Add(new Point(this.ActualWidth, this.lbl.ActualWidth));
                segment.Points.Add(new Point(this.ActualWidth, 0));
                segment.Points.Add(new Point(0, 0));

                pathFigure.Segments.Add(segment);
                this.path.Fill = this.BackColor;
                this.path.Data = Geometry.Parse(pathFigure.ToString());
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////Refresh the position of the cursor according to previus setting
            //////////////////////////////////////////////////////////////////////////////////////////////////

            this.ReSetCursorPosition();
        }
  
        private void ReSetCursorPosition()
        {
            try
            {
                if (this.SelectedPlotView != null)
                {
                    double width = (this.Parent as FrameworkElement).ActualWidth;
                    double height = (this.Parent as FrameworkElement).ActualHeight;
                    Thickness t = this.Margin;
                    if (this.CursorType == CursorEnum.V_BAR)
                    {
                        //change the cursor positions
                        Point p = this.SelectedPlotView.Viewport.DataToScreen(new Point(this.CursorPosition, 0));

                        if (double.IsNaN(p.X) || double.IsInfinity(p.X))
                        {
                            p.X = 0;
                        }
                        t.Left = p.X - width;
                    }
                    else if (this.CursorType == CursorEnum.H_BAR)
                    {
                        //change the cursor positions
                        Point p = this.SelectedPlotView.Viewport.DataToScreen(new Point(0, this.CursorPosition));

                        if (double.IsNaN(p.Y) || double.IsInfinity(p.X))
                        {
                            p.Y = 0;
                        }
                        t.Top = p.Y - height;
                    }

                    this.Margin = t;
                }
            }
            catch
            {
            }
        }

        private void CustomCursor_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Parent != null && this.Visibility == System.Windows.Visibility.Visible && this.SelectedPlotView != null)
            {
                this.Refresh();
                this.ReSetCursorPosition();
            }
        }
    }
}