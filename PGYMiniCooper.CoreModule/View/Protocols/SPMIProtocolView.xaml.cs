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
    /// Interaction logic for SPMIProtocolView.xaml
    /// </summary>
    public partial class SPMIProtocolView : UserControl
    {
        public SPMIProtocolView()
        {
            InitializeComponent();
           // CommandSPMI.SelectedItem = eSPMICMDTYPE.DEFAULT;
            CommandSPMI.ItemsSource = Enum.GetValues(typeof(eSPMICMDTYPE)).Cast<eSPMICMDTYPE>();
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            //Set DataTemplate
            spmigrid.RowDetailsTemplate = this.Resources["spmidatatemplate"] as DataTemplate;
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
            spmigrid.RowDetailsTemplate = this.Resources["spmidatatemplate"] as DataTemplate;
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
        }
        private void Srbitcheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (Srbitcheckbox.IsChecked == true)
            {
                Srbit.Visibility = Visibility.Visible;
            }
            else if (Srbitcheckbox.IsChecked == false)
            {
                Srbit.Visibility = Visibility.Collapsed;
            }
        }

        private void Checked(object sender, RoutedEventArgs e)
        {
            if (AbitCheckbox.IsChecked == true)
            {
                Abit.Visibility = Visibility.Visible;
            }
            else if (AbitCheckbox.IsChecked == false)
            {
                Abit.Visibility = Visibility.Collapsed;
            }

        }


    }
}
