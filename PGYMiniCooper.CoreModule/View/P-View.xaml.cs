using MaterialDesignThemes.Wpf;
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
    /// Interaction logic for P_View.xaml
    /// </summary>
    public partial class P_View : UserControl
    {
        public P_View()
        {
            InitializeComponent();
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {

            //Set DataTemplate
            i3cgrid.RowDetailsTemplate = this.Resources["i3cdatatemplate"] as DataTemplate;
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
            {
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
            }
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            i3cgrid.RowDetailsTemplate = this.Resources["i3cdatatemplate"] as DataTemplate;
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
        }

        private Detailed_I3CViewWindow i3cFrameWindow = null;
        private void btnDetailFlotingWindow_Click(object sender, RoutedEventArgs e)
        {
            if (i3cFrameWindow != null)
                return;

            i3cFrameWindow = new Detailed_I3CViewWindow();
            i3cFrameWindow.DataContext = this.DataContext;
            i3cFrameWindow.Closed += I3cFrameWindow_Closed;

            i3cFrameWindow.Show();
        }

        private void I3cFrameWindow_Closed(object sender, EventArgs e)
        {
            if(i3cFrameWindow != null)
            {
                i3cFrameWindow.Closed -= I3cFrameWindow_Closed;
                i3cFrameWindow = null;
            }
        }
    }
}
