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
    /// Interaction logic for DeviceConfiguration.xaml
    /// </summary>
    public partial class DeviceConfiguration : UserControl
    {
        public DeviceConfiguration()
        {
            InitializeComponent();
        }

        private void interfaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsEnabledComboBoxBinding_Update();
        }

        private void interface_Checked(object sender, RoutedEventArgs e)
        {
            IsEnabledComboBoxBinding_Update();
        }

        private void interface_Unchecked(object sender, RoutedEventArgs e)
        {
            IsEnabledComboBoxBinding_Update();
        }

        private void IsEnabledComboBoxBinding_Update()
        {
            //var cmb0BE = i2cSCL.GetBindingExpression(ComboBox.SelectedIndexProperty);
            //var cmb1BE = i2cSDA.GetBindingExpression(ComboBox.SelectedIndexProperty);
            //var cmb2BE = uartTx.GetBindingExpression(ComboBox.SelectedIndexProperty);
            //var cmb3BE = uartRx.GetBindingExpression(ComboBox.SelectedIndexProperty);
            //var cmb4BE = spi_cs.GetBindingExpression(ComboBox.SelectedIndexProperty);
            //var cmb5BE = spi_miso.GetBindingExpression(ComboBox.SelectedIndexProperty);
            //var cmb6BE = spi_mosi.GetBindingExpression(ComboBox.SelectedIndexProperty);
            //var cmb7BE = spi_ws.GetBindingExpression(ComboBox.SelectedIndexProperty);

            //Validation.ClearInvalid(cmb0BE);
            //Validation.ClearInvalid(cmb1BE);
            //Validation.ClearInvalid(cmb2BE);
            //Validation.ClearInvalid(cmb3BE);
            //Validation.ClearInvalid(cmb4BE);
            //Validation.ClearInvalid(cmb5BE);
            //Validation.ClearInvalid(cmb6BE);
            //Validation.ClearInvalid(cmb7BE);

            //cmb0BE.UpdateSource();
            //cmb1BE.UpdateSource();
            //cmb2BE.UpdateSource();
            //cmb3BE.UpdateSource();
            //cmb4BE.UpdateSource();
            //cmb5BE.UpdateSource();
            //cmb6BE.UpdateSource();
            //cmb7BE.UpdateSource();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            IsEnabledComboBoxBinding_Update();
        }
    }
}
