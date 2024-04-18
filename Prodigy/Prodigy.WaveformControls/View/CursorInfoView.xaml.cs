// <copyright file="CursorInfoView.xaml.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>
  
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Prodigy.Business;

namespace Prodigy.WaveformControls.View
{
    /// <summary>
    /// Interaction logic for CursorInfoView.xaml
    /// </summary>
    public partial class CursorInfoView : UserControl
    {
        public static DependencyProperty Cursor1PositionProperty = DependencyProperty.Register("Cursor1Position", typeof(double),
            typeof(CursorInfoView), new PropertyMetadata(0.0, OnCursor1PositionPropertyChanged));

        public static DependencyProperty Cursor2PositionProperty = DependencyProperty.Register("Cursor2Position", typeof(double),
            typeof(CursorInfoView), new PropertyMetadata(0.0, OnCursor2PositionPropertyChanged));

        public static DependencyProperty CursorDeltaProperty = DependencyProperty.Register("CursorDelta", typeof(double),
            typeof(CursorInfoView), new PropertyMetadata(0.0));

        public static DependencyProperty CursorTypeProperty = DependencyProperty.Register("CursorType", typeof(CursorEnum),
            typeof(CursorInfoView), new PropertyMetadata(CursorEnum.V_BAR));

        public static DependencyProperty SampleRateProperty = DependencyProperty.Register("SampleRate", typeof(double),
            typeof(CursorInfoView), new PropertyMetadata(0.0));

        public static DependencyProperty Cursor1LabelProperty = DependencyProperty.Register("Cursor1Label", typeof(string),
            typeof(CursorInfoView), new PropertyMetadata("C1"));

        public static DependencyProperty Cursor2LabelProperty = DependencyProperty.Register("Cursor2Label", typeof(string),
            typeof(CursorInfoView), new PropertyMetadata("C2"));

        public static DependencyProperty CursorDeltaLabelProperty = DependencyProperty.Register("CursorDeltaLabel", typeof(string),
            typeof(CursorInfoView), new PropertyMetadata("Δ"));

        public static DependencyProperty HUnitLabelProperty = DependencyProperty.Register("HUnitLabel", typeof(string),
            typeof(CursorInfoView), new PropertyMetadata("Unit"));

        public static DependencyProperty TriggerPositionProperty = DependencyProperty.Register("TriggerPosition", typeof(double),
            typeof(CursorInfoView), new PropertyMetadata(0.0));

        public static DependencyProperty Cursor1LabelForeColorProperty = DependencyProperty.Register("Cursor1LabelForeColor", typeof(Brush),
            typeof(CursorInfoView), new PropertyMetadata(Brushes.Black));

        public static DependencyProperty Cursor2LabelForeColorProperty = DependencyProperty.Register("Cursor2LabelForeColor", typeof(Brush),
            typeof(CursorInfoView), new PropertyMetadata(Brushes.Black));

        public static DependencyProperty CursorDeltaLabelForeColorProperty = DependencyProperty.Register("CursorDeltaLabelForeColor", typeof(Brush),
            typeof(CursorInfoView), new PropertyMetadata(Brushes.Black));

        public CursorInfoView()
        {
            this.InitializeComponent();
        }

        public double Cursor1Position
        {
            get
            {
                return (double)this.GetValue(Cursor1PositionProperty);
            }
            set
            {
                this.SetValue(Cursor1PositionProperty, value);
            }
        }

        public double Cursor2Position
        {
            get
            {
                return (double)this.GetValue(Cursor2PositionProperty);
            }
            set
            {
                this.SetValue(Cursor2PositionProperty, value);
            }
        }

        public double CursorDelta
        {
            get
            {
                return (double)this.GetValue(CursorDeltaProperty);
            }
            set
            {
                this.SetValue(CursorDeltaProperty, value);
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

        public double SampleRate
        {
            get
            {
                return (double)this.GetValue(SampleRateProperty);
            }
            set
            {
                this.SetValue(SampleRateProperty, value);
            }
        }

        public string Cursor1Label
        {
            get
            {
                return (string)this.GetValue(Cursor1LabelProperty);
            }
            set
            {
                this.SetValue(Cursor1LabelProperty, value);
            }
        }

        public string Cursor2Label
        {
            get
            {
                return (string)this.GetValue(Cursor2LabelProperty);
            }
            set
            {
                this.SetValue(Cursor2LabelProperty, value);
            }
        }

        public Brush Cursor1LabelForeColor
        {
            get
            {
                return (Brush)this.GetValue(Cursor1LabelForeColorProperty);
            }
            set
            {
                this.SetValue(Cursor1LabelForeColorProperty, value);
            }
        }

        public Brush CursorDeltaLabelForeColor
        {
            get
            {
                return (Brush)this.GetValue(CursorDeltaLabelForeColorProperty);
            }
            set
            {
                this.SetValue(CursorDeltaLabelForeColorProperty, value);
            }
        }

        public Brush Cursor2LabelForeColor
        {
            get
            {
                return (Brush)this.GetValue(Cursor2LabelForeColorProperty);
            }
            set
            {
                this.SetValue(Cursor2LabelForeColorProperty, value);
            }
        }

        public string HUnitLabel
        {
            get
            {
                return (string)this.GetValue(HUnitLabelProperty);
            }
            set
            {
                this.SetValue(HUnitLabelProperty, value);
            }
        }

        public string CursorDeltaLabel
        {
            get
            {
                return (string)this.GetValue(CursorDeltaLabelProperty);
            }
            set
            {
                this.SetValue(CursorDeltaLabelProperty, value);
            }
        }

        public double TriggerPosition
        {
            get
            {
                return (double)this.GetValue(TriggerPositionProperty);
            }
            set
            {
                this.SetValue(TriggerPositionProperty, value);
            }
        }

        private static void OnCursor1PositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                CursorInfoView view = d as CursorInfoView;
                view.CursorDelta = Math.Abs(view.Cursor2Position - view.Cursor1Position);
            }
        }

        private static void OnCursor2PositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                CursorInfoView view = d as CursorInfoView;
                view.CursorDelta = Math.Abs(view.Cursor2Position - view.Cursor1Position);
            }
        }
    }
}