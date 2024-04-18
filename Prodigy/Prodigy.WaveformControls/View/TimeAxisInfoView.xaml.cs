// <copyright file="TimeAxisInfoView.xaml.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>
  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Prodigy.Business;
using Prodigy.Business.Extensions;

namespace Prodigy.WaveformControls.View
{
    /// <summary>
    /// Interaction logic for TimeAxisInfoView.xaml
    /// </summary>
    public partial class TimeAxisInfoView : UserControl
    {
        public static DependencyProperty ItemCountProperty = DependencyProperty.Register("ItemCount", typeof(int), typeof(TimeAxisInfoView), new PropertyMetadata(0, OnItemCountPropertyChanged));

        public static DependencyProperty WfmShowingStartIndexProperty = DependencyProperty.Register("WfmShowingStartIndex", typeof(long), typeof(TimeAxisInfoView), new PropertyMetadata(0L, OnWfmShowingStartIndexPropertyChanged));

        public static DependencyProperty WfmShowingStopIndexProperty = DependencyProperty.Register("WfmShowingStopIndex", typeof(long), typeof(TimeAxisInfoView), new PropertyMetadata(0L, OnWfmShowingStopIndexPropertyChanged));

        public static DependencyProperty TriggerPositionProperty = DependencyProperty.Register("TriggerPosition", typeof(long), typeof(TimeAxisInfoView), new PropertyMetadata(0L, OnTriggerPositionPropertyChanged));

        public static DependencyProperty SampleRateProperty = DependencyProperty.Register("SampleRate", typeof(double), typeof(TimeAxisInfoView), new PropertyMetadata(0.0, OnSampleRatePropertyChanged));

        public TimeAxisInfoView()
        {
            this.InitializeComponent();
        }

        public int ItemCount
        {
            get
            {
                return (int)this.GetValue(ItemCountProperty);
            }
            set
            {
                this.SetValue(ItemCountProperty, value);
            }
        }

        public long WfmShowingStartIndex
        {
            get
            {
                return (long)this.GetValue(WfmShowingStartIndexProperty);
            }
            set
            {
                this.SetValue(WfmShowingStartIndexProperty, value);
            }
        }

        public long WfmShowingStopIndex
        {
            get
            {
                return (long)this.GetValue(WfmShowingStopIndexProperty);
            }
            set
            {
                this.SetValue(WfmShowingStopIndexProperty, value);
            }
        }

        public long TriggerPosition
        {
            get
            {
                return (long)this.GetValue(TriggerPositionProperty);
            }
            set
            {
                this.SetValue(TriggerPositionProperty, value);
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

        private static void OnTriggerPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                TimeAxisInfoView view = d as TimeAxisInfoView;
                view.Refresh();
            }
        }

        private static void OnItemCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                TimeAxisInfoView view = d as TimeAxisInfoView;
                view.canvas.Children.Clear();
                double count = (int)e.NewValue;
                double left = 0d;
                for (int i = 0; i < count; i++)
                {
                    left = view.canvas.ActualWidth * (((double)(1 + i)) / (1 + count));

                    TextBlock txt = new TextBlock();
                    txt.Tag = i;
                    txt.Text = ((Prodigy.Business.Extensions.CustomExtensions.GetEquivalent(
                        Convert.ToDouble(view.WfmShowingStartIndex),
                        Convert.ToDouble(view.WfmShowingStopIndex),
                        0d,
                        view.canvas.ActualWidth, left) - Convert.ToDouble(view.TriggerPosition)) / view.SampleRate).FormatNumber(Units.BASE_UNIT) + "s";

                    System.Windows.Data.Binding b = new System.Windows.Data.Binding("Foreground");
                    b.Mode = System.Windows.Data.BindingMode.OneWay;
                    b.Source = view;
                    txt.SetBinding(TextBlock.ForegroundProperty, b);

                    view.canvas.Children.Add(txt);
                    Canvas.SetLeft(txt, left - txt.ActualWidth / 2d);
                }
            }
        }

        private static void OnWfmShowingStartIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                TimeAxisInfoView view = d as TimeAxisInfoView;
                view.Refresh();
            }
        }
  
        private static void OnWfmShowingStopIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                TimeAxisInfoView view = d as TimeAxisInfoView;
                view.Refresh();
            }
        }

        private static void OnSampleRatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                TimeAxisInfoView view = d as TimeAxisInfoView;
                view.Refresh();
            }
        }

        private void Refresh()
        {
            IEnumerable<TextBlock> labels = this.canvas.Children.OfType<TextBlock>();
            double index = 0;
            double left = 0;
            double count = labels.Count();
            foreach (TextBlock txt in labels.OrderBy((l) => l.Tag))
            {
                left = this.canvas.ActualWidth * ((1 + index++) / (1 + count));
                txt.Text = ((Prodigy.Business.Extensions.CustomExtensions.GetEquivalent(
                    Convert.ToDouble(this.WfmShowingStartIndex),
                    Convert.ToDouble(this.WfmShowingStopIndex),
                    0d,
                    this.canvas.ActualWidth, left) - Convert.ToDouble(this.TriggerPosition)) / this.SampleRate).FormatNumber(Units.BASE_UNIT) + "s";

                Canvas.SetLeft(txt, left - txt.ActualWidth / 2d);
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Refresh();
        }
    }
}