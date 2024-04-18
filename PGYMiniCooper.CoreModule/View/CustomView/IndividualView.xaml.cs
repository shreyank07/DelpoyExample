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

namespace PGYMiniCooper.CoreModule.View.CustomView
{
    /// <summary>
    /// Interaction logic for IndividualView.xaml
    /// </summary>
    public partial class IndividualView : UserControl
    {
        public IndividualView()
        {
            InitializeComponent();
        }

        private void ChannelLabel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
