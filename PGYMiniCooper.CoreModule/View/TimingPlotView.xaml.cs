using PGYMiniCooper.CoreModule.ViewModel;
using PGYMiniCooper.DataModule.Structure;
using Prodigy.WaveformControls.View;
using ProdigyFramework.Converters;
using ProdigyFramework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PGYMiniCooper.CoreModule.View
{
    /// <summary>
    /// Interaction logic for TimingPlotView.xaml
    /// </summary>
    public partial class TimingPlotView : UserControl
    {
        public TimingPlotView()
        {
            InitializeComponent();
            AttachLoadingAdorner();
            SessionConfiguration.OnAnimate += UpdateStoryBoard;
        }
        void UpdateStoryBoard(bool val)
        {
            this.Dispatcher.Invoke(() =>
            {
                var template = myscroll.Template;
                var myControl = (Rectangle)template.FindName("ellipse", myscroll);
                if (myControl != null)
                {
                    Storyboard myStoryboard = myControl.TryFindResource("Wave") as Storyboard;
                    if (val)
                    {
                        myStoryboard.Begin(myControl);
                        myControl.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        myStoryboard.Stop(myControl);
                        myControl.Visibility = Visibility.Collapsed;
                    }
                }
            });
        }
        public void plotter_gridlines_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            plotter_gridlines.Width = e.NewSize.Width;
            plotter_gridlines.Height = e.NewSize.Height;
        }
        private void AttachLoadingAdorner()
        {
            LoadingAdorner loading = new LoadingAdorner(maingrid);
            loading.FontSize = 16;
            loading.OverlayedText = "Waveform Loading...";
            loading.Typeface = new Typeface(new FontFamily("Roboto"), FontStyles.Italic,
                FontWeights.Bold, FontStretch);
            Binding bind = new Binding("IsLoading");
            bind.Converter = new BoolToVisibilityConverter();
            loading.SetBinding(LoadingAdorner.VisibilityProperty, bind);
            AdornerLayer.GetAdornerLayer(maingrid).Add(loading);
        }
        public void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            (sender as Grid).Children.OfType<CursorView>().All(c =>
            {
                c.RaiseEvent(e);
                return true;
            });
        }

        public void menu_ZoomOff_Click(object sender, RoutedEventArgs e)
        {
            plotter.FIT();
        }

        private void plotter_gridlines_Loaded(object sender, RoutedEventArgs e)
        {
            plotter_gridlines.IsDirty = true;
        }
    }
}
