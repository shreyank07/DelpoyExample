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
using Xceed.Wpf.Toolkit;

namespace PGYMiniCooper.CoreModule.View.CustomView
{
    /// <summary>
    /// Interaction logic for ChannelLabel.xaml
    /// </summary>
  
           public partial class ChannelLabel : System.Windows.Controls.UserControl
    {
    
        public ChannelLabel()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register("Text", typeof(string), typeof(ChannelLabel), new FrameworkPropertyMetadata(null,
     new PropertyChangedCallback(ChannelLabel.OnTextChanged)));

    

        public static readonly DependencyProperty TextBackgroundProperty =
            DependencyProperty.Register("TextBackground", typeof(Brush), typeof(ChannelLabel), new FrameworkPropertyMetadata(null,
     new PropertyChangedCallback(ChannelLabel.OnBackgroundChanged)));


        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChannelLabel control = (ChannelLabel)d;
            if (control != null)
            {
                control.SetBackground();
            }
        }
        public Brush TextBackground
        {
            get { return (Brush)GetValue(TextBackgroundProperty); }
            set { SetValue(TextBackgroundProperty, value); }
        }
        private void SetBackground()
        {
            colPicker.Background = TextBackground;
            gName.Background = this.TextBackground;
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChannelLabel control = (ChannelLabel)d;
            if (control != null)
            {
          
                control.SetText();
            }
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private void SetText()
        {
            gName.Text = this.Text;
           
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            TextBackground = colPicker.Background;
        }
    }
}
