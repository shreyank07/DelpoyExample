// <copyright file="WfmPlotCtrl.xaml.cs" company="Prodigy Technovations Pvt. Ltd.">
//     Copyright (C)  2014
// </copyright>
  
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Prodigy.WaveformControls.View;
using Prodigy.WaveformControls.Interfaces;

namespace Prodigy.WaveformControls
{
    public partial class WfmPlotCtrl : UserControl
    {
        private bool timeInfoVisible;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public WfmPlotCtrl()
        {
            this.InitializeComponent();
        }
        
        public void MoveVCursor2Here_Click(object sender, RoutedEventArgs e)
        {
            //ContextMenu m = new ContextMenu();
            //m.com
            //plotter.MoveVCursor2Here(sender, e);
        }

        public void MoveVCursor1Here_Click(object sender, RoutedEventArgs e)
        {
            //plotter.MoveVCursor1Here(sender, e);
        }

        public void MoveHCursor2Here_Click(object sender, RoutedEventArgs e)
        {
            //plotter.MoveHCursor1Here(sender, e);
        }

        public void MoveHCursor1Here_Click(object sender, RoutedEventArgs e)
        {
            //plotter.MoveHCursor1Here(sender, e);
        }

        public void menu_ZoomOff_Click(object sender, RoutedEventArgs e)
        {
            this.plotter.FIT();
        }

        public void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            (sender as Grid).Children.OfType<CursorView>().All(c =>
            {
                c.RaiseEvent(e);
                return true;
            });
        }        

        private void userControl_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(IPlotInfoView).ToString()))
            {
                // These Effects values are used in the drag source's 
                // GiveFeedback event handler to determine which cursor to display. 
                e.Effects = DragDropEffects.Link;
            }
        }

        private void menu_Remove_Click(object sender, RoutedEventArgs e)
        {
        }

        private void leftTopPloygon_MouseEnter(object sender, MouseEventArgs e)
        {
        }

        private void leftTopPloygon_MouseLeave(object sender, MouseEventArgs e)
        {
        }

        private void leftTopPloygon_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}