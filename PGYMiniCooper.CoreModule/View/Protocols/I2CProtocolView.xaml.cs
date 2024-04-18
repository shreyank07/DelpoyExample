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
using PGYMiniCooper.DataModule;
using PGYMiniCooper.CoreModule.ViewModel;
using PGYMiniCooper.DataModule.Structure.I2CStructure;
using PGYMiniCooper.DataModule.Model;

using PGYMiniCooper.DataModule.Interface;
using System.Globalization;


namespace PGYMiniCooper.CoreModule.View.Protocols
{
    /// <summary>
    /// Interaction logic for I2CProtocolView.xaml
    /// </summary>
    public partial class I2CProtocolView : UserControl
    {
        public I2CProtocolView()
        {
     
        InitializeComponent();
            comboselect1.SelectedIndex = 0; comboselect2.SelectedIndex = 0; comboselect3.SelectedIndex = 0;
            comboselect2.ItemsSource = Enum.GetValues(typeof(eTransfer)).Cast<eTransfer>();
            comboselect1.ItemsSource = Enum.GetValues(typeof(eType)).Cast<eType>();
            comboselect3.ItemsSource = Enum.GetValues(typeof(eType)).Cast<eType>();
        }
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            //Set DataTemplate
            i2cgrid.RowDetailsTemplate = this.Resources["i2cdatatemplate"] as DataTemplate;
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
            i2cgrid.RowDetailsTemplate = this.Resources["i2cdatatemplate"] as DataTemplate;
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
