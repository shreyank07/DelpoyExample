using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PGYMiniCooper.CoreModule.View.CustomView
{
    /// <summary>
    /// This custom radio button shows a error message when the radio button IsAvailable property is false but user tries to check it
    /// </summary>
    public partial class RadioButtonWithAvailabilityGroup : RadioButton
    {
        public static readonly DependencyProperty NotAvailableErrorProperty = DependencyProperty.Register("NotAvailableError", typeof(string), typeof(RadioButtonWithAvailabilityGroup),
            new PropertyMetadata("This checkbox is not available. Please see the configuration."));

        /// <summary>
        /// Error message that the user want to show
        /// </summary>
        public string NotAvailableError
        {
            get { return (string)GetValue(NotAvailableErrorProperty); }
            set { SetValue(NotAvailableErrorProperty, value); }
        }

        public static readonly DependencyProperty IsAvailableProperty = DependencyProperty.Register("IsAvailable", typeof(bool?), typeof(RadioButtonWithAvailabilityGroup),
            new PropertyMetadata(null, new PropertyChangedCallback(OnIsAvailableChanged)));

        private static void OnIsAvailableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // nothing to do here
        }

        /// <summary>
        /// IsAvailable flag that tells the user that the radio button check in not allowed. But may be allowed when someother control turn the is avilable flag to true or null
        /// </summary>
        public bool? IsAvailable
        {
            get { return (bool?)GetValue(IsAvailableProperty); }
            set { SetValue(IsAvailableProperty, value); }
        }

        private bool uncheckedDueToError_IgnoreOnUncheckedEvent = false;

        protected override void OnUnchecked(RoutedEventArgs e)
        {
            // Ignore is unchecked as a result of error condition
            if (uncheckedDueToError_IgnoreOnUncheckedEvent)
                uncheckedDueToError_IgnoreOnUncheckedEvent = false;
            else if (IsAvailable == false && IsChecked == false)
                IsAvailable = true; // Check if same radio button is clicked -> Set is available to true

            base.OnUnchecked(e);
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            if (IsChecked == true)
            {
                // IsAvailable to check this radio button -> Set the available to false;
                if (IsAvailable == true)
                    IsAvailable = false;
                else
                {
                    uncheckedDueToError_IgnoreOnUncheckedEvent = true;

                    MessageBox.Show(NotAvailableError, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                    IsChecked = false;
                }
            }

            base.OnChecked(e);
        }

        /// <summary>
        /// Overriding toggle method to make the available state when the radiobutton state changes from true to false
        /// </summary>
        protected override void OnToggle()
        {
            bool? previousState = IsChecked;

            // Update the toggle state
            base.OnToggle();

            if (previousState == true)
            {
                IsAvailable = true;
                IsChecked = false;
            }
        }
    }
}
