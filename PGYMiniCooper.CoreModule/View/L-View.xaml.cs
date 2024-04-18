using PGYMiniCooper.CoreModule.ViewModel;
using ProdigyFramework.Converters;
using ProdigyFramework.Model;
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
    /// Interaction logic for L_View.xaml
    /// </summary>
    public partial class L_View : UserControl
    {
        public L_View()
        {
            InitializeComponent();
            AttachLoadingAdorner();
        }

        private void AttachLoadingAdorner()
        {
            LoadingAdorner loading = new LoadingAdorner(dg);
            loading.FontSize = 16;
            loading.OverlayedText = "Loading...";
            loading.Typeface = new Typeface(new FontFamily("Roboto"), FontStyles.Italic,
                FontWeights.Bold, FontStretch);
            Binding bind = new Binding("WaveformListingVM.WfmHolder.IsLoading");
            bind.Converter = new BoolToVisibilityConverter();
            loading.SetBinding(LoadingAdorner.VisibilityProperty, bind);
            AdornerLayer.GetAdornerLayer(dg).Add(loading);
        }

        private void dummyElement_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
