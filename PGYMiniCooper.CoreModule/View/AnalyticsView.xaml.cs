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
    /// Interaction logic for AnalyticsView.xaml
    /// </summary>
    public partial class AnalyticsView : UserControl
    {
        public AnalyticsView()
        {
            InitializeComponent();
        }

        private void all_click(object sender, RoutedEventArgs e)
        {
            rffegrid2.Visibility = Visibility.Visible;
            rffegrid1.Visibility = Visibility.Collapsed;
            rffegrid.Visibility = Visibility.Collapsed;

        }

        private void Read_Click(object sender, RoutedEventArgs e)
        {
            rffegrid2.Visibility = Visibility.Collapsed;
            rffegrid.Visibility = Visibility.Collapsed;
            rffegrid1.Visibility = Visibility.Visible;
            e.Handled = true;

        }

        private void write_click(object sender, RoutedEventArgs e)
        {
            rffegrid2.Visibility = Visibility.Collapsed;

            rffegrid.Visibility = Visibility.Visible;
            rffegrid1.Visibility = Visibility.Collapsed;
            e.Handled = true;

        }

        private void remove_click(object sender, RoutedEventArgs e)
        {


            rffegrid.Visibility = Visibility.Collapsed;
            rffegrid1.Visibility = Visibility.Collapsed;
            rffegrid2.Visibility = Visibility.Collapsed;

        }

        private void all_click1(object sender, RoutedEventArgs e)
        {
            spmigrid2.Visibility = Visibility.Visible;
            spmigrid1.Visibility = Visibility.Collapsed;
            spmigrid.Visibility = Visibility.Collapsed;
            e.Handled = true;
        }

        private void Read_Click1(object sender, RoutedEventArgs e)
        {
            spmigrid2.Visibility = Visibility.Collapsed;
            spmigrid.Visibility = Visibility.Collapsed;
            spmigrid1.Visibility = Visibility.Visible;
            e.Handled = true;

        }

        private void write_click1(object sender, RoutedEventArgs e)
        {
            spmigrid2.Visibility = Visibility.Collapsed;
            spmigrid.Visibility = Visibility.Visible;
            spmigrid1.Visibility = Visibility.Collapsed;
            e.Handled = true;
        }

        private void remove_click1(object sender, RoutedEventArgs e)
        {

            spmigrid.Visibility = Visibility.Collapsed;
            spmigrid1.Visibility = Visibility.Collapsed;
            spmigrid2.Visibility = Visibility.Collapsed;
        }
        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            var selectedItem = (TreeViewItem)sender;
            var HeaderName = selectedItem.Name;

            if (HeaderName == "SDR")
            {
                GridSDR.Visibility = Visibility.Visible;
                GridHDR_DDR.Visibility = Visibility.Collapsed;
                GridHDR_TSL.Visibility = Visibility.Collapsed;
                GridHDR_TSP.Visibility = Visibility.Collapsed;
                DataGridBorder.Visibility = Visibility.Visible;

            }
            else
            if (HeaderName == "HDR_DDR")
            {
                GridSDR.Visibility = Visibility.Collapsed;
                GridHDR_DDR.Visibility = Visibility.Visible;
                GridHDR_TSL.Visibility = Visibility.Collapsed;
                GridHDR_TSP.Visibility = Visibility.Collapsed;
                DataGridBorder.Visibility = Visibility.Visible;
            }
            else
            if (HeaderName == "HDR_TSL")
            {
                GridSDR.Visibility = Visibility.Collapsed;
                GridHDR_DDR.Visibility = Visibility.Collapsed;
                GridHDR_TSL.Visibility = Visibility.Visible;
                GridHDR_TSP.Visibility = Visibility.Collapsed;
                DataGridBorder.Visibility = Visibility.Visible;
            }
            else
            if (HeaderName == "HDR_TSP")
            {
                GridSDR.Visibility = Visibility.Collapsed;
                GridHDR_DDR.Visibility = Visibility.Collapsed;
                GridHDR_TSL.Visibility = Visibility.Collapsed;
                GridHDR_TSP.Visibility = Visibility.Visible;
                DataGridBorder.Visibility = Visibility.Visible;
            }
        }

        private void DataGridRemove(object sender, MouseButtonEventArgs e)
        {

            DataGridBorder.Visibility = Visibility.Collapsed;

        }

    }
}
