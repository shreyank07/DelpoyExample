using PGYMiniCooper.CoreModule.ViewModel;
using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ProtocolSelectListBox.xaml
    /// </summary>
    public partial class ProtocolSelectListBox : UserControl
    {
        private ObservableCollection<Node> _nodeList;
        public ProtocolSelectListBox()
        {
            InitializeComponent();
            _nodeList = new ObservableCollection<Node>();
        }

        #region Dependency Properties

        public static readonly DependencyProperty ItemsSourceProperty =
             DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<ChannelSelectionViewModel>), typeof(ProtocolSelectListBox), new FrameworkPropertyMetadata(null,
        new PropertyChangedCallback(ProtocolSelectListBox.OnItemsSourceChanged)));

        public static readonly DependencyProperty SelectedItemsProperty =
         DependencyProperty.Register("SelectedItem", typeof(ChannelSelectionViewModel), typeof(ProtocolSelectListBox), new FrameworkPropertyMetadata(null,
     new PropertyChangedCallback(ProtocolSelectListBox.OnSelectedItemsChanged)));

        public static DependencyProperty TextProperty =
           DependencyProperty.Register("Text", typeof(string), typeof(ProtocolSelectListBox), new FrameworkPropertyMetadata(null,
     new PropertyChangedCallback(ProtocolSelectListBox.OnTextChanged)));

        public static DependencyProperty DefaultTextProperty =
            DependencyProperty.Register("DefaultText", typeof(bool), typeof(ProtocolSelectListBox), new FrameworkPropertyMetadata(false,
     new PropertyChangedCallback(ProtocolSelectListBox.OnVisibleChanged)));

        public static DependencyProperty ErrorProperty =
           DependencyProperty.Register("IsErrorOccur", typeof(bool), typeof(ProtocolSelectListBox), new FrameworkPropertyMetadata(false,
    new PropertyChangedCallback(ProtocolSelectListBox.OnErrorChanged)));

        public ObservableCollection<ChannelSelectionViewModel> ItemsSource
        {
            get { return (ObservableCollection<ChannelSelectionViewModel>)GetValue(ItemsSourceProperty); }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public eChannles SelectedItem
        {
            get { return (eChannles)GetValue(SelectedItemsProperty); }
            set
            {
                SetValue(SelectedItemsProperty, value);
            }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool DefaultText
        {
            get { return (bool)GetValue(DefaultTextProperty); }
            set { SetValue(DefaultTextProperty, value); }
        }

        public bool IsErrorOccur
        {
            get { return (bool)GetValue(ErrorProperty); }
            set { SetValue(ErrorProperty, value); }
        }
        #endregion

        #region Events
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProtocolSelectListBox control = (ProtocolSelectListBox)d;
            if (control != null)
                control.DisplayInControl();
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProtocolSelectListBox control = (ProtocolSelectListBox)d;
            if (control != null)
            {
                control.SelectNodes();
                //  control.SetText();
            }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProtocolSelectListBox control = (ProtocolSelectListBox)d;
            if (control != null)
            {
                control.SetText();
            }
        }

        private static void OnErrorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProtocolSelectListBox control = (ProtocolSelectListBox)d;
            if (control != null)
            {
                control.Error();
            }
        }
        private static void OnVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProtocolSelectListBox control = (ProtocolSelectListBox)d;
            if (control != null)
            {
                control.SetVisible();
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            RadioButton clickedBox = (RadioButton)sender;
            if (clickedBox.IsChecked == true)
                clickedBox.IsChecked = false;

            //CheckBox clickedBox = (CheckBox)sender;
            //SetSelectedItems(clickedBox);

        }
        #endregion


        #region Methods
        private void SelectNodes()
        {
            foreach (Node node in _nodeList)
            {
                node.IsSelected = false;
            }
            if (SelectedItem != null)
            {
                Node node = _nodeList.FirstOrDefault(i => i.Title == SelectedItem.ToString());
                if (node != null)
                    node.IsSelected = true;
            }
        }

        private void SetSelectedItems(CheckBox cb)
        {
        
            //eChannles myStatus;
            //Label clickedBox = (Label)cb.Content;
            //if (Enum.TryParse(_nodeList.Where(x => x.Title == clickedBox.Content.ToString()).LastOrDefault().Title, out myStatus) && cb.IsChecked == true)
            //{
            //    SelectedItem = myStatus;
            //}
            //else
            //    SelectedItem = eChannles.None;
            //foreach (Node node in _nodeList)
            //{
            //    if (SelectedItem.ToString() != node.Title)
   
            //    node.IsSelected = false;
                
            //}
        }

        private void DisplayInControl()
        {
            _nodeList.Clear();
            //if (this.ItemsSource.Count > 0)
            //  _nodeList.Add(new Node("All"));
            if (this.ItemsSource != null)
            {
                foreach (ChannelSelectionViewModel keyValue in this.ItemsSource)
                {
                    Node node = new Node(keyValue.ToString());
                    if (node.Title != "None")
                        _nodeList.Add(node);
                }
            }
            MultiSelectList.ItemsSource = _nodeList;
        }

        private void Error()
        {
            if(IsErrorOccur)
            {
             //   MessageBox.Show("Error");
            }
        }

        private void SetVisible()
        {
            //if (this.DefaultText == true)
            //    staticchannelList.Visibility = Visibility.Visible;
            //else
            //    staticchannelList.Visibility = Visibility.Collapsed;
        }
        private void SetText()
        {
            gName.Content = this.Text;
            //if (this.DefaultText == true)
            //    staticchannelList.Visibility = Visibility.Visible;
            //else
            //    staticchannelList.Visibility = Visibility.Collapsed;
        
        }


        #endregion

        private void chk2_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void chk2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RadioButton clickedBox = (RadioButton)sender;

            if (clickedBox.IsChecked == true)
            {
                clickedBox.IsChecked = false;
                e.Handled = true;
            }
            else
            {
                clickedBox.IsChecked = true;
            }
        }
    }
}
