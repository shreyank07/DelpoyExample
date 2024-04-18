using PGYMiniCooper.DataModule;
using ProdigyFramework.ComponentModel;
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
    /// Interaction logic for MultiSelectComboBox.xaml
    /// </summary>
    public partial class MultiSelectComboBox : UserControl
    {
        private ObservableCollection<Node> _nodeList;
        public MultiSelectComboBox()
        {
            InitializeComponent();
            _nodeList = new ObservableCollection<Node>();
        }

        #region Dependency Properties

        public static readonly DependencyProperty ItemsSourceProperty =
             DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<eChannles>), typeof(MultiSelectComboBox), new FrameworkPropertyMetadata(null,
        new PropertyChangedCallback(MultiSelectComboBox.OnItemsSourceChanged)));

        public static readonly DependencyProperty SelectedItemsProperty =
         DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<eChannles>), typeof(MultiSelectComboBox), new FrameworkPropertyMetadata(null,
     new PropertyChangedCallback(MultiSelectComboBox.OnSelectedItemsChanged)));

        public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register("Text", typeof(string), typeof(MultiSelectComboBox), new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty DefaultTextProperty =
            DependencyProperty.Register("DefaultText", typeof(string), typeof(MultiSelectComboBox), new UIPropertyMetadata(string.Empty));



        public ObservableCollection<eChannles> ItemsSource
        {
            get { return (ObservableCollection<eChannles>)GetValue(ItemsSourceProperty); }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public ObservableCollection<eChannles> SelectedItems
        {
            get { return (ObservableCollection<eChannles>)GetValue(SelectedItemsProperty); }
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

        public string DefaultText
        {
            get { return (string)GetValue(DefaultTextProperty); }
            set { SetValue(DefaultTextProperty, value); }
        }
        #endregion

        #region Events
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectComboBox control = (MultiSelectComboBox)d;
            if (control != null)
                control.DisplayInControl();
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectComboBox control = (MultiSelectComboBox)d;
            if (control != null)
            {
                control.SelectNodes();
                control.SetText();
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            //CheckBox clickedBox = (CheckBox)sender;

            //if (clickedBox.Content == "All")
            //{
            //    if (clickedBox.IsChecked.Value)
            //    {
            //        foreach (Node node in _nodeList)
            //        {
            //            node.IsSelected = true;
            //        }
            //    }
            //    else
            //    {
            //        foreach (Node node in _nodeList)
            //        {
            //            node.IsSelected = false;
            //        }
            //    }

            //}
            //else
            //{
            //    int _selectedCount = 0;
            //    foreach (Node s in _nodeList)
            //    {
            //        if (s.IsSelected && s.Title != "All")
            //            _selectedCount++;
            //    }
            //    if (_selectedCount == _nodeList.Count - 1)
            //        _nodeList.FirstOrDefault(i => i.Title == "All").IsSelected = true;
            //    else
            //        _nodeList.FirstOrDefault(i => i.Title == "All").IsSelected = false;
            //}
            SetSelectedItems();
            SetText();

        }
        #endregion


        #region Methods
        private void SelectNodes()
        {
            if (SelectedItems != null)
            {
                foreach (eChannles keyValue in SelectedItems)
                {
                    Node node = _nodeList.FirstOrDefault(i => i.Title == keyValue.ToString());
                    if (node != null)
                        node.IsSelected = true;
                }
            }
        }

        private void SetSelectedItems()
        {
            if (SelectedItems == null)
                SelectedItems = new ObservableCollection<eChannles>();
            SelectedItems.Clear();
            foreach (Node node in _nodeList)
            {
                if (node.IsSelected && node.Title != "All")
                {
                    if (this.ItemsSource.Count > 0)
                    {
                        eChannles myStatus;
                        if (Enum.TryParse(node.Title, out myStatus))
                            SelectedItems.Add(myStatus);
                    }
                }
            }
        }

        private void DisplayInControl()
        {
            _nodeList.Clear();
            //if (this.ItemsSource.Count > 0)
            //  _nodeList.Add(new Node("All"));
            if (this.ItemsSource != null)
            {
                foreach (eChannles keyValue in this.ItemsSource)
                {
                    Node node = new Node(keyValue.ToString());
                    _nodeList.Add(node);
                }
            }
            MultiSelectCombo.ItemsSource = _nodeList;
        }

        private void SetText()
        {
            if (this.SelectedItems != null)
            {
                StringBuilder displayText = new StringBuilder();
                foreach (Node s in _nodeList)
                {
                    if (s.IsSelected == true && s.Title == "All")
                    {
                        displayText = new StringBuilder();
                        displayText.Append("All");
                        break;
                    }
                    else if (s.IsSelected == true && s.Title != "All")
                    {
                        displayText.Append(s.Title);
                        displayText.Append(',');
                    }
                }
                this.Text = displayText.ToString().TrimEnd(new char[] { ',' });
            }
            // set DefaultText if nothing else selected
            if (string.IsNullOrEmpty(this.Text))
            {
                this.Text = this.DefaultText;
            }
        }


        #endregion
    }

    public class Node : ViewModelBase
    {

        private string _title;
        private bool _isSelected;

        public Node()
        {

        }
        #region ctor
        public Node(string title)
        {
            Title = title;
        }
        #endregion

        #region Properties
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }
        #endregion
        

    }
}
