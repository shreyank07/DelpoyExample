using PGYMiniCooper.DataModule;
using PGYMiniCooper.DataModule.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
    /// Interaction logic for MultiSelectListBox.xaml
    /// </summary>
    public partial class MultiSelectListBox : System.Windows.Controls.UserControl
    {
        private ObservableCollection<Node> _nodeList;
        public MultiSelectListBox()
        {
            InitializeComponent();
            _nodeList = new ObservableCollection<Node>();
        }

        #region Dependency Properties

        public static readonly DependencyProperty ItemsSourceProperty =
             DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<eChannles>), typeof(MultiSelectListBox), new FrameworkPropertyMetadata(null,
        new PropertyChangedCallback(MultiSelectListBox.OnItemsSourceChanged)));

        public static readonly DependencyProperty SelectedItemsProperty =
         DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<eChannles>), typeof(MultiSelectListBox), new FrameworkPropertyMetadata(null,
     new PropertyChangedCallback(MultiSelectListBox.OnSelectedItemsChanged)));

        public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register("Text", typeof(string), typeof(MultiSelectListBox), new FrameworkPropertyMetadata(null,
     new PropertyChangedCallback(MultiSelectListBox.OnTextChanged)));

        public static readonly DependencyProperty DefaultTextProperty =
            DependencyProperty.Register("DefaultText", typeof(bool), typeof(MultiSelectListBox), new FrameworkPropertyMetadata(false,
     new PropertyChangedCallback(MultiSelectListBox.OnVisibleChanged)));

        public static readonly DependencyProperty TextBackgroundProperty =
            DependencyProperty.Register("TextBackground", typeof(Brush), typeof(MultiSelectListBox), new FrameworkPropertyMetadata(null,
     new PropertyChangedCallback(MultiSelectListBox.OnBackgroundChanged)));



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

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == TextProperty)
            {
                
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

        public Brush TextBackground
        {
            get { return (Brush)GetValue(TextBackgroundProperty); }
            set { SetValue(TextBackgroundProperty, value); }
        }

        private static string defaultName = "ClkGrp";
        #endregion

        #region Events
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectListBox control = (MultiSelectListBox)d;
            if (control != null)
                control.DisplayInControl();
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectListBox control = (MultiSelectListBox)d;
            if (control != null)
            {
                control.SelectNodes();
              //  control.SetText();
            }
            control.currentControl = control;
            var oldVal = e.OldValue as INotifyCollectionChanged;
            var newVal = e.NewValue as INotifyCollectionChanged;

            if (oldVal != null)
            {
                oldVal.CollectionChanged -= control.OnCollectionChanged;
            }

            if (newVal != null)
            {
                newVal.CollectionChanged += control.OnCollectionChanged;
            }
        }

        MultiSelectListBox currentControl;
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //ConfigModel config = ConfigModel.GetInstance();
            if (SelectedItems != null)
            {
                if (SelectedItems.Count != 0)
                {
                    //if (currentControl.Name == "Clk1Grp1")
                    //{
                    //    if (config.SelectedClock1 == SelectedItems[0])
                    //    {
                    //        System.Windows.Forms.MessageBox.Show("Channel Already set", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //        if (config.SelectedCLK1_GRP1.Count > 0)
                    //            config.SelectedCLK1_GRP1.RemoveAt(0);
                    //        if (SelectedItems.Count > 0)
                    //            SelectedItems.RemoveAt(0);
                    //    }
                    //}
                }
            }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
       {
            //MultiSelectListBox control = (MultiSelectListBox)d;
            //ConfigModel config = ConfigModel.GetInstance();
            //if (control != null)
            //{
            //    if (control.Name == "Clk1Grp1")
            //    {
            //        config.Clk1Grp1Text = control.Text;
            //        defaultName = "CLK1_GRP1";
            //    }
            //    else if (control.Name == "Clk1Grp2")
            //    {
            //        config.Clk1Grp2Text = control.Text;
            //        defaultName = "CLK1_GRP2";
            //    }
            //    else if (control.Name == "Clk1Grp3")
            //    {
            //        config.Clk1Grp3Text = control.Text;
            //        defaultName = "CLK1_GRP3";
            //    }
            //    else if (control.Name == "Clk2Grp1")
            //    {
            //        config.Clk2Grp1Text = control.Text;
            //        defaultName = "CLK2_GRP1";
            //    }
            //    else if (control.Name == "Clk2Grp2")
            //    {
            //        config.Clk2Grp2Text = control.Text;
            //        defaultName = "CLK2_GRP2";
            //    }
            //    else if (control.Name == "Clk2Grp3")
            //    {
            //        config.Clk2Grp3Text = control.Text;
            //        defaultName = "CLK2_GRP3";
            //    }
            //    control.SetText();
            //}
        }

        private static void OnVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectListBox control = (MultiSelectListBox)d;
            if (control != null)
            {
                control.SetVisible();
            }
        }
        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectListBox control = (MultiSelectListBox)d;
            if (control != null)
            {
               control.SetBackground();
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
            //SetText();

        }
        #endregion


        #region Methods
        private void SelectNodes()
        {
            foreach (Node node in _nodeList)
            {
                node.IsSelected = false;
            }
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
            //ConfigModel config = ConfigModel.GetInstance();
            //foreach (Node node in _nodeList)
            //{
            //    if (node.IsSelected && node.Title != "All")
            //    {
            //        if (this.ItemsSource.Count > 0)
            //        {
            //            eChannles myStatus;
            //            if (Enum.TryParse(node.Title, out myStatus))
            //            {
            //                if (config.SelectedClock1 == myStatus && config.GeneralPurposeMode == eGeneralPurpose.State)
            //                {
            //                    node.IsSelected = false;
            //                    System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                    return;
            //                }
            //                if (config.HasTwoClockSources && config.SelectedClock2 == myStatus && config.GeneralPurposeMode == eGeneralPurpose.State)
            //                {
            //                    node.IsSelected = false;
            //                    System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                    return;
            //                }
            //                #region CLK1_GRP1
            //                if (currentControl.Name == "Clk1Grp1")
            //                {
            //                    foreach(var chn in config.SelectedCLK1_GRP2)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                    foreach (var chn in config.SelectedCLK1_GRP3)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                    if (config.HasTwoClockSources)
            //                    {
            //                        foreach (var chn in config.SelectedCLK2_GRP1)
            //                        {
            //                            if (chn == myStatus)
            //                            {
            //                                node.IsSelected = false;
            //                                System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                                return;
            //                            }
            //                        }
            //                        foreach (var chn in config.SelectedCLK2_GRP2)
            //                        {
            //                            if (chn == myStatus)
            //                            {
            //                                node.IsSelected = false;
            //                                System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                                return;
            //                            }
            //                        }
            //                        foreach (var chn in config.SelectedCLK2_GRP3)
            //                        {
            //                            if (chn == myStatus)
            //                            {
            //                                node.IsSelected = false;
            //                                System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                                return;
            //                            }
            //                        }
            //                    }
            //                }
            //                #endregion
            //                #region CLK1_GRP2
            //                else if (currentControl.Name == "Clk1Grp2")
            //                {
            //                    foreach (var chn in config.SelectedCLK1_GRP1)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                    foreach (var chn in config.SelectedCLK1_GRP3)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                    if (config.HasTwoClockSources)
            //                    {
            //                        foreach (var chn in config.SelectedCLK2_GRP1)
            //                        {
            //                            if (chn == myStatus)
            //                            {
            //                                node.IsSelected = false;
            //                                System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                                return;
            //                            }
            //                        }
            //                        foreach (var chn in config.SelectedCLK2_GRP2)
            //                        {
            //                            if (chn == myStatus)
            //                            {
            //                                node.IsSelected = false;
            //                                System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                                return;
            //                            }
            //                        }
            //                        foreach (var chn in config.SelectedCLK2_GRP3)
            //                        {
            //                            if (chn == myStatus)
            //                            {
            //                                node.IsSelected = false;
            //                                System.Windows.Forms.MessageBox.Show("Channel Already set", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                                return;
            //                            }
            //                        }
            //                    }
            //                }
            //                #endregion
            //                #region CLK1_GRP3
            //                else if (currentControl.Name == "Clk1Grp3")
            //                {
            //                    foreach (var chn in config.SelectedCLK1_GRP1)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                    foreach (var chn in config.SelectedCLK1_GRP2)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                    if (config.HasTwoClockSources)
            //                    {
            //                        foreach (var chn in config.SelectedCLK2_GRP1)
            //                        {
            //                            if (chn == myStatus)
            //                            {
            //                                node.IsSelected = false;
            //                                System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                                return;
            //                            }
            //                        }
            //                        foreach (var chn in config.SelectedCLK2_GRP2)
            //                        {
            //                            if (chn == myStatus)
            //                            {
            //                                node.IsSelected = false;
            //                                System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                                return;
            //                            }
            //                        }
            //                        foreach (var chn in config.SelectedCLK2_GRP3)
            //                        {
            //                            if (chn == myStatus)
            //                            {
            //                                node.IsSelected = false;
            //                                System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                                return;
            //                            }
            //                        }
            //                    }
            //                }
            //                #endregion
            //                #region CLK2_GRP1
            //                else if (currentControl.Name == "Clk2Grp1")
            //                {
            //                    foreach (var chn in config.SelectedCLK1_GRP1)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                    foreach (var chn in config.SelectedCLK1_GRP2)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                    foreach (var chn in config.SelectedCLK1_GRP3)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                    foreach (var chn in config.SelectedCLK2_GRP2)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                    foreach (var chn in config.SelectedCLK2_GRP3)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                }
            //                #endregion
            //                #region CLK2_GRP2
            //                else if (currentControl.Name == "Clk2Grp2")
            //                {
            //                    foreach (var chn in config.SelectedCLK1_GRP1)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                    foreach (var chn in config.SelectedCLK1_GRP2)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                    foreach (var chn in config.SelectedCLK1_GRP3)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                    foreach (var chn in config.SelectedCLK2_GRP1)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                    foreach (var chn in config.SelectedCLK2_GRP3)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                }
            //                #endregion
            //                #region CLK2_GRP3
            //                else if (currentControl.Name == "Clk2Grp3")
            //                {
            //                    foreach (var chn in config.SelectedCLK1_GRP1)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                    foreach (var chn in config.SelectedCLK1_GRP2)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                    foreach (var chn in config.SelectedCLK1_GRP3)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                    foreach (var chn in config.SelectedCLK2_GRP1)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                    foreach (var chn in config.SelectedCLK2_GRP2)
            //                    {
            //                        if (chn == myStatus)
            //                        {
            //                            node.IsSelected = false;
            //                            System.Windows.Forms.MessageBox.Show("Channel Already Assigned", "Channel Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //                            return;
            //                        }
            //                    }
            //                }
            //                #endregion
            //                SelectedItems.Add(myStatus);
            //            }
            //        }
            //    }
            //}
            //foreach(var sel in SelectedItems)
            //{
            //    switch(sel)
            //    {
            //        case eChannles.CH1:
            //            ConfigModel.GetInstance().IsCh1Visible = true;
            //            break;
            //        case eChannles.CH2:
            //            ConfigModel.GetInstance().IsCh2Visible = true;
            //            break;
            //        case eChannles.CH3:
            //            ConfigModel.GetInstance().IsCh3Visible = true;
            //            break;
            //        case eChannles.CH4:
            //            ConfigModel.GetInstance().IsCh4Visible = true;
            //            break;
            //        case eChannles.CH5:
            //            ConfigModel.GetInstance().IsCh5Visible = true;
            //            break;
            //        case eChannles.CH6:
            //            ConfigModel.GetInstance().IsCh6Visible = true;
            //            break;
            //        case eChannles.CH7:
            //            ConfigModel.GetInstance().IsCh7Visible = true;
            //            break;
            //        case eChannles.CH8:
            //            ConfigModel.GetInstance().IsCh8Visible = true;
            //            break;
            //        case eChannles.CH9:
            //            ConfigModel.GetInstance().IsCh9Visible = true;
            //            break;
            //        case eChannles.CH10:
            //            ConfigModel.GetInstance().IsCh10Visible = true;
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }

        private void DisplayInControl()
        {
            //_nodeList.Clear();
            ////if (this.ItemsSource.Count > 0)
            ////  _nodeList.Add(new Node("All"));
            //if (this.ItemsSource != null)
            //{
            //    foreach (eChannles keyValue in this.ItemsSource)
            //    {
            //        Node node = new Node(keyValue.ToString());
            //        if (node.Title != "None")
            //            _nodeList.Add(node);
            //    }
            //}
            //MultiSelectList.ItemsSource = _nodeList;
         
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
            gName.Text = this.Text;
            //if (String.IsNullOrEmpty(this.Text))
            //    gName.Text = defaultName;

            //if (this.DefaultText == true)
            //    staticchannelList.Visibility = Visibility.Visible;
            //else
            //    staticchannelList.Visibility = Visibility.Collapsed;
            //if (this.SelectedItems != null)
            //{
            //    StringBuilder displayText = new StringBuilder();
            //    foreach (Node s in _nodeList)
            //    {
            //        if (s.IsSelected == true && s.Title == "All")
            //        {
            //            displayText = new StringBuilder();
            //            displayText.Append("All");
            //            break;
            //        }
            //        else if (s.IsSelected == true && s.Title != "All")
            //        {
            //            displayText.Append(s.Title);
            //            displayText.Append(',');
            //        }
            //    }
            //    this.Text = displayText.ToString().TrimEnd(new char[] { ',' });
            //}
            //// set DefaultText if nothing else selected
            //if (string.IsNullOrEmpty(this.Text))
            //{
            //    this.Text = this.DefaultText;
            //}
        }
        private void SetBackground()
        {
            TextBdr.Background = this.TextBackground;
            gName.Background = this.TextBackground;
        }


            #endregion
        }
}
