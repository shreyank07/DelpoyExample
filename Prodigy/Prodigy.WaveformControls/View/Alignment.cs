// <copyright file="Alignment.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>
  
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Prodigy.WaveformControls.View
{
    public enum Alignment
    {
        LEFT,
        RIGHT
    }

    /// <summary>
    /// Interaction logic for Paner.xaml
    /// </summary>
    internal partial class Paner : Grid
    {
        public static DependencyProperty AlignmentProperty = DependencyProperty.Register("Alignment", typeof(Alignment),
            typeof(Paner), new PropertyMetadata(Alignment.RIGHT,new PropertyChangedCallback(OnAlignmentPropertyChanged)));

        public Paner()
        {
            this.InitializeComponent();
        }

        public Alignment Align
        {
            get
            {
                return (Alignment)this.GetValue(AlignmentProperty);
            }
            set
            {
                this.SetValue(AlignmentProperty, value);
            }
        }

        private static void OnAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((Alignment)e.NewValue == Alignment.LEFT)
            {
                ((d as Paner).parent.RenderTransform as RotateTransform).Angle = 180;
            }
            else
            {
                ((d as Paner).parent.RenderTransform as RotateTransform).Angle = 0;
            }
        }
    }
}