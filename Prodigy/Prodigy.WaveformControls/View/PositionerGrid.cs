// <copyright file="PositionerGrid.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>

namespace Prodigy.WaveformControls.View
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Prodigy.WaveformControls.View;

    public class PositionerGrid : Grid
    {
        public static readonly DependencyProperty PlotterProperty =
            DependencyProperty.Register("Plotter", typeof(PlotterView), typeof(PositionerGrid),
                new PropertyMetadata(null,new PropertyChangedCallback(PlotterPropertyChanged)));

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double),
                typeof(PositionerView), new FrameworkPropertyMetadata(10.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionerGrid" /> class.
        /// </summary>
        public PositionerGrid()
        {
        }

        public PlotterView Plotter
        {
            get
            {
                return (PlotterView)this.GetValue(PlotterProperty);
            }
            set
            {
                this.SetValue(PlotterProperty, value);
            }
        }

        public string DisplayMemberPath { get; set; }

        public double FontSize
        {
            get
            {
                return (double)this.GetValue(FontSizeProperty);
            }
            set
            {
                this.SetValue(FontSizeProperty, value);
            }
        }

        private static void PlotterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                PositionerGrid view = d as PositionerGrid;
                if (e.NewValue != null)
                {
                    view.Plotter.ChartCollection.CollectionChanged +=
                        new System.Collections.Specialized.NotifyCollectionChangedEventHandler(view.OnChartCollectionChanged);
                }
            }
        }

        private void OnChartCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (IPlot item in e.NewItems)
                    {
                        this.AddPositionerView(item);
                    }
                }
                else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    foreach (IPlot item in e.OldItems)
                    {
                        this.RemovePositionerView(item);
                    }
                }
                else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    this.Children.OfType<PositionerView>().ToList().ForEach(c => this.Children.Remove(c));
                }
            }
            catch
            {
            }
        }

        private void RemovePositionerView(IPlot item)
        {
            var positioners = this.Children.OfType<PositionerView>();
            if (positioners.Any((p) => p.DataContext.Equals((item as PlotView).DataContext)))
            {
                PositionerView pos = positioners.First((p) => p.DataContext.Equals((item as PlotView).DataContext));
                this.Children.Remove(pos);
            }
        }

        private void AddPositionerView(IPlot item)
        {
            try
            {
                ///////////////////////////////////////
                PositionerView pos = new PositionerView();
                this.Children.Add(pos);
                //Set dataContext
                pos.DataContext = (item as PlotView).DataContext;

                pos.BeginInit();

                //Bind SelectedPlotItem
                Binding binding;

                //Bind Vertical Offset
                binding = new Binding("VerticalOffset");
                binding.UpdateSourceTrigger = UpdateSourceTrigger.Default;
                binding.Mode = BindingMode.TwoWay;
                pos.SetBinding(PositionerView.VOffsetProperty, binding);

                //Bind Vertical Scale
                binding = new Binding("VerticalScale");
                binding.UpdateSourceTrigger = UpdateSourceTrigger.Default;
                binding.Mode = BindingMode.TwoWay;
                pos.SetBinding(PositionerView.VScaleProperty, binding);

                //Bind Caption
                binding = new Binding("Channel");
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                pos.SetBinding(PositionerView.CaptionProperty, binding);

                //Bind  Visible
                binding = new Binding("IsShowing");
                binding.Converter = new BooleanToVisibilityConverter();
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                pos.SetBinding(PositionerView.VisibilityProperty, binding);

                //Bind  isdirty
                binding = new Binding("IsDirty");
                binding.Converter = new BooleanToVisibilityConverter();
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                pos.SetBinding(PositionerView.IsDirtyProperty, binding);

                //Bind Selectable
                binding = new Binding("IsEnabled");
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                binding.Mode = BindingMode.OneWay;
                pos.SetBinding(PositionerView.IsSelectableProperty, binding);

                //Bind  Tag
                binding = new Binding(this.DisplayMemberPath);
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                binding.Mode = BindingMode.TwoWay;
                pos.SetBinding(PositionerView.TagProperty, binding);

                //bind FontSize
                System.Windows.Data.Binding b = new System.Windows.Data.Binding("FontSize");
                b.Source = this;
                pos.SetBinding(PositionerView.FontSizeProperty, b);

                binding = new Binding("Viewport");
                binding.Source = item;
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                pos.SetBinding(PositionerView.ViewportProperty, binding);

                //Bind SelectedPlotItem
                binding = new Binding("SelectedChart");
                binding.Source = this.Plotter;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                binding.Mode = BindingMode.TwoWay;
                pos.SetBinding(PositionerView.SelectedPlotItemProperty, binding);

                pos.SetResourceReference(PositionerView.ForeColorProperty, pos.Caption);
                pos.Margin = new Thickness(0, this.ActualHeight / 2, 0, 0);
                pos.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                pos.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

                pos.EndInit();
            }
            catch
            {
            }
        }
    }
}