using PGYMiniCooper.DataModule;
using System;
using System.Collections;
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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class QSPIProtocolView : UserControl
    {
        //private SelectionChangedEventHandler qSPIdatagrid_SelectionChanged;

        public QSPIProtocolView()
        {
            InitializeComponent();
            CommandQSPI.SelectedItem = eQSPICommands.Any_Command;
            CommandQSPI.ItemsSource = Enum.GetValues(typeof(eQSPICommands)).Cast<eQSPICommands>();
        }
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            //Set DataTemplate
            qspigrid.RowDetailsTemplate = this.Resources["qspidatatemplate"] as DataTemplate;
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
            qspigrid.RowDetailsTemplate = this.Resources["qspidatatemplate"] as DataTemplate;
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

        private void qSPIdatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        
        public static List<T> GetLogicalChildCollection<T>(object parent) where T : DependencyObject
        {
            List<T> logicalCollection = new List<T>();
            GetLogicalChildCollection(parent as DependencyObject, logicalCollection);
            return logicalCollection;
        }

        private static void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection) where T : DependencyObject
        {
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            foreach (object child in children)
            {
                if (child is DependencyObject)
                {
                    DependencyObject depChild = child as DependencyObject;
                    if (child is T)
                    {
                        logicalCollection.Add(child as T);
                    }
                    GetLogicalChildCollection(depChild, logicalCollection);
                }
            }
        }


        private Detailed_QSPIViewWindow qspiFrameWindow = null;
        private void btnDetailFlotingWindow_Click(object sender, RoutedEventArgs e)
        {
            if (qspiFrameWindow != null)
                return;

            qspiFrameWindow = new Detailed_QSPIViewWindow();
            qspiFrameWindow.DataContext = this.DataContext;
            qspiFrameWindow.Closed += QSPIFrameWindow_Closed;

            qspiFrameWindow.Show();
        }

        private void QSPIFrameWindow_Closed(object sender, EventArgs e)
        {
            if (qspiFrameWindow != null)
            {
                qspiFrameWindow.Closed -= QSPIFrameWindow_Closed;
                qspiFrameWindow = null;
            }
        }
    }
}


