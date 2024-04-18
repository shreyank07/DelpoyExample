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
    /// Interaction logic for Detailed_I3CView.xaml
    /// </summary>
    public partial class Detailed_I3CView : UserControl
    {
        public Detailed_I3CView()
        {
            InitializeComponent();

            mainWindowDataContext.DataContext = Application.Current.MainWindow.DataContext;
        }
    }
}
