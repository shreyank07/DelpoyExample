using PGYMiniCooper.CoreModule.ViewModel;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PGYMiniCooper.CoreModule.View
{
    /// <summary>
    /// Interaction logic for ProdigyPlotView.xaml
    /// </summary>
    public partial class ProtocolPlotView : UserControl
    {
        public ProtocolPlotView()
        {
            InitializeComponent();
        }

        public void plotter_gridlines_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            plotter_gridlines.Width = e.NewSize.Width;
            plotter_gridlines.Height = e.NewSize.Height;
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
