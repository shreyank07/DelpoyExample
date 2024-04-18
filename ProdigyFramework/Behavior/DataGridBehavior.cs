using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ProdigyFramework.Behavior
{
    public class DataGridBehavior
    {
        #region AutoScrollIntoView

        public static bool GetAutoScrollIntoView(DataGrid dataGrid)
        {
            return (bool)dataGrid.GetValue(AutoScrollIntoViewProperty);
        }

        public static void SetAutoScrollIntoView(
          DataGrid dataGrid, bool value)
        {
            dataGrid.SetValue(AutoScrollIntoViewProperty, value);
        }

        public static readonly DependencyProperty AutoScrollIntoViewProperty =
            DependencyProperty.RegisterAttached(
            "AutoScrollIntoView",
            typeof(bool),
            typeof(DataGridBehavior),
            new UIPropertyMetadata(false, OnAutoScrollIntoViewWhenSelectionChanged));

        static void OnAutoScrollIntoViewWhenSelectionChanged(
          DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = depObj as DataGrid;
            if (dataGrid == null)
                return;

            if (!(e.NewValue is bool))
                return;

            if ((bool)e.NewValue)
                dataGrid.SelectionChanged += OnDataGridSelectionChanged;
            else
                dataGrid.SelectionChanged -= OnDataGridSelectionChanged;
        }

        static void OnDataGridSelectionChanged(object sender, RoutedEventArgs e)
        {
            // Only react to the SelectionChanged event raised by the DataGrid
            // Ignore all ancestors.
            if (!object.ReferenceEquals(sender, e.OriginalSource))
                return;

            DataGrid dataGrid = e.OriginalSource as DataGrid;
            if (dataGrid != null && dataGrid.SelectedItem != null)
            {
                // this is a workaround to fix the layout issue.
                // otherwise ScrollIntoView should work directly. 
                dataGrid.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new DispatcherOperationCallback(ScrollItemIntoView), dataGrid);
            }
        }

        static object ScrollItemIntoView(object sender)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid != null && dataGrid.SelectedItem != null)
            {
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);
            }
            return null;
        }

        #endregion // AutoScrollIntoView


    }
}
