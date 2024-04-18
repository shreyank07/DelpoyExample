using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Model;
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
    /// Interaction logic for CANProtocolView.xaml
    /// </summary>
    public partial class CANProtocolView : UserControl
    {
        public CANProtocolView()
        {
            InitializeComponent();
            frameType.SelectedIndex = 0; 
            frameType.ItemsSource = Enum.GetValues(typeof(eFrameTypeCAN)).Cast<eFrameTypeCAN>();
          
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            //Set DataTemplate
            cangrid.RowDetailsTemplate = this.Resources["candatatemplate"] as DataTemplate;
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
           cangrid.RowDetailsTemplate = this.Resources["candatatemplate"] as DataTemplate;
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
