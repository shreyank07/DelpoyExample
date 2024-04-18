using PGYMiniCooper.DataModule;
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

namespace PGYMiniCooper.CoreModule.View.Protocols
{
    /// <summary>
    /// Interaction logic for RFFEProtocolView.xaml
    /// </summary>
    public partial class RFFEProtocolView : UserControl
    {
        public RFFEProtocolView()
        {
            InitializeComponent();
            CommandRFFE.ItemsSource = Enum.GetValues(typeof(eRFFECMDTYPE)).Cast<eRFFECMDTYPE>();
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            //Set DataTemplate
            rffegrid.RowDetailsTemplate = this.Resources["rffedatatemplate"] as DataTemplate;
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            rffegrid.RowDetailsTemplate = this.Resources["rffedatatemplate"] as DataTemplate;
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
        }
    }
}
