// <copyright file="PositionerView.xaml.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>
  
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Prodigy.WaveformControls.View
{
    /// <summary>
    /// Interaction logic for PositionerView.xaml
    /// </summary>
    internal partial class PositionerView : UserControl
    {
        public static readonly DependencyProperty VOffsetProperty =
            DependencyProperty.Register("VOffset", typeof(double),
                typeof(PositionerView), new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnVOffsetPropertyChanged));

        public static readonly DependencyProperty VScaleProperty =
            DependencyProperty.Register("VScale", typeof(double),
                typeof(PositionerView), new FrameworkPropertyMetadata(1.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnVScalePropertyChanged));

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string),
                typeof(PositionerView), new FrameworkPropertyMetadata("Positioner",new PropertyChangedCallback(CaptionPropertyChanged)));

        public static readonly DependencyProperty TagProperty =
            DependencyProperty.Register("Tag", typeof(string),
                typeof(PositionerView), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty TagEditOnProperty =
            DependencyProperty.Register("TagEditOn", typeof(bool),
                typeof(PositionerView), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty ForeColorProperty =
            DependencyProperty.Register("ForeColor", typeof(Brush),
                typeof(PositionerView), new FrameworkPropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty BackColorProperty =
            DependencyProperty.Register("BackColor", typeof(Brush),
                typeof(PositionerView), new FrameworkPropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(double),
                typeof(PositionerView), new FrameworkPropertyMetadata(0.0));

        public static readonly DependencyProperty SelectedPlotItemProperty =
            DependencyProperty.Register("SelectedPlotItem", typeof(object),
                typeof(PositionerView), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty IsDirtyProperty =
            DependencyProperty.Register("IsDirty", typeof(bool),
                typeof(PositionerView), new FrameworkPropertyMetadata(false,new PropertyChangedCallback(IsDirtyPropertyChanged),
                    new CoerceValueCallback(IsDirtyPropertyCoerced)));

        public static readonly DependencyProperty ViewportProperty = DependencyProperty.Register("Viewport", typeof(Viewport.Viewport2D), typeof(PositionerView),
            new PropertyMetadata(null,
                new PropertyChangedCallback(OutputViewportPropertyChanged)));

        public static readonly DependencyProperty IsSelectableProperty = DependencyProperty.Register("IsSelectable", typeof(bool), typeof(PositionerView),
            new PropertyMetadata(false));

        private bool isMoving;
        private Point mouseDownPosition;

        internal PositionerView()
        {
            this.InitializeComponent();
        }

        public double VScale
        {
            get
            {
                return (double)this.GetValue(VScaleProperty);
            }
            set
            {
                this.SetValue(VScaleProperty, value);
            }
        }

        public double VOffset
        {
            get
            {
                return (double)this.GetValue(VOffsetProperty);
            }
            set
            {
                this.SetValue(VOffsetProperty, value);
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

        public string Tag
        {
            get 
            {
                return ((this.GetValue(TagProperty) == null) ? this.Caption : this.GetValue(TagProperty)).ToString();
            }
            set
            {
                this.SetValue(TagProperty, value);
            }
        }

        public bool TagEditOn
        {
            get
            {
                return (bool)this.GetValue(TagEditOnProperty);
            }
            set
            {
                this.SetValue(TagEditOnProperty, value);
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

        public object SelectedPlotItem
        {
            get
            {
                return this.GetValue(SelectedPlotItemProperty);
            }
            set
            {
                this.SetValue(SelectedPlotItemProperty, value);
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

        public bool IsSelectable
        {
            get
            {
                return (bool)this.GetValue(IsSelectableProperty);
            }
            set
            {
                this.SetValue(IsSelectableProperty, value);
            }
        }

        public void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.TagEditOn = true;
            this.txtTagEditBox.Focus();
        }

        public void Refresh()
        {
            try
            {
                if (this.Parent == null)
                {
                    return;
                }

                Thickness margin = this.Margin;
                double height = (this.Parent as FrameworkElement).ActualHeight;
                
                Point p = this.Viewport.DataToScreen(new Point(0, 0));
                
                margin.Top = p.Y - height;
                if (!double.IsNaN(margin.Top) && !double.IsNegativeInfinity(margin.Top) && !double.IsPositiveInfinity(margin.Top))
                {
                    this.Margin = margin;
                }
            }
            catch
            {
            }
        }

        public void positioner_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Parent != null)
            {
                Thickness margin = this.Margin;
                double height = (this.Parent as FrameworkElement).ActualHeight;

                Point p = this.Viewport.DataToScreen(new Point(0, 0));
                margin.Top = p.Y + height;
            }
        }

        private static void OnVOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PositionerView view = d as PositionerView;
            if (view.Parent != null)
            {
                Thickness margin = view.Margin;
                double height = (view.Parent as FrameworkElement).ActualHeight;
                double current = Convert.ToDouble(e.NewValue);
                //Point p = view.Viewport.DataToScreen(new Point(0, current - VOffset));
                //margin.Top = p.Y - height;
                //view.Margin = margin;
            }
        }

        private static void OnVScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PositionerView view = d as PositionerView;
            if (view.Parent != null)
            {
                Thickness margin = view.Margin;
                double height = (view.Parent as FrameworkElement).ActualHeight;
                double current = Convert.ToDouble(e.NewValue);
                //Point p = view.Viewport.DataToScreen(new Point(0, current - VOffset));
                //margin.Top = p.Y - height;
                //view.Margin = margin;
            }
        }

        private static void CaptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                PositionerView view = d as PositionerView;
                view.Tag = (string)e.NewValue;
            }
        }

        private static object IsDirtyPropertyCoerced(DependencyObject d, object baseValue)
        {
            if (d != null)
            {
                PositionerView view = d as PositionerView;
                if (view.Viewport != null)
                {
                    view.Refresh();
                }
            }

            return baseValue;
        }

        private static void IsDirtyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //do nothing
        }

        private static void OutputViewportPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                PositionerView view = d as PositionerView;
                Thickness t = view.Margin;
                if (e.NewValue != null)
                {
                    // Point p = view.Viewport.DataToScreen(new Point(0, view.VOffset * view.VScale));
                    Point p = view.Viewport.DataToCursorScreen(new Point(0, view.VOffset));
                    t.Top = p.Y - (view.Parent as FrameworkElement).ActualHeight;

                    view.Margin = t;
                }
            }
        }

        private void myEllipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.ClickCount == 1)
                {
                    if (this.IsSelectable == true)
                    {
                        this.SelectedPlotItem = this.DataContext;
                    }

                    this.lbl.CaptureMouse();
                    this.mouseDownPosition = e.GetPosition(this);
                    this.isMoving = true;
                    e.Handled = true;
                }
                else if (e.ClickCount == 2)
                {
                    this.TagEditOn = true;
                    this.txtTagEditBox.Focus();
                }
            }
        }

        private void myEllipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.isMoving)
                {
                    Point loc1 = e.GetPosition(this.Parent as FrameworkElement);
                    Thickness margin = this.Margin;

                    margin.Top = loc1.Y - this.mouseDownPosition.Y;
                    if (margin.Top < 0)
                    {
                        margin.Top = 0;
                    }
                    if (margin.Top > (this.Parent as FrameworkElement).ActualHeight - this.ActualHeight)
                    {
                        margin.Top = (this.Parent as FrameworkElement).ActualHeight - this.ActualHeight;
                    }

                    this.Margin = margin;
                    e.Handled = true;
                }
            }
        }
        private void myEllipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.lbl.ReleaseMouseCapture();
            this.isMoving = false;
            double height = (this.Parent as FrameworkElement).ActualHeight;
            Point p = this.Viewport.CursorScreenToData(new Point(0, this.Margin.Top + height));
            this.VOffset = p.Y;
            e.Handled = true;
        }

        private void myEllipse_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void myEllipse_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = null;
        }

        private void txtTagEditBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.txtTagEditBox.Text != null && this.txtTagEditBox.Text != "")
            {
                this.Tag = this.txtTagEditBox.Text;
            }
        }

        private void txtTagEditBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.TagEditOn = false;
        }

        private void positioner_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is PositionerView)
            {
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    this.context.IsOpen = true;
                }
            }
        }

        private void txtTagEditBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.TagEditOn = false;
            }
        }
    }
}