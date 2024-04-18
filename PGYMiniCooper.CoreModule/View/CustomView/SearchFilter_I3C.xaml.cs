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

namespace PGYMiniCooper.CoreModule.View.CustomView
{
    /// <summary>
    /// Interaction logic for SearchFilter_I3C.xaml
    /// </summary>
    public partial class SearchFilter_I3C : UserControl
    {
        public SearchFilter_I3C()
        {
            InitializeComponent();
            Command.SelectedItem = eMajorFrame.NA;
            Command.ItemsSource = Enum.GetValues(typeof(eMajorFrame)).Cast<eMajorFrame>();
        }
    }
}
