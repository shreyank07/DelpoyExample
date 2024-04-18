using PGYMiniCooper.CoreModule.ViewModel;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PGYMiniCooper.CoreModule.View.Converters
{
    public class ProtocolConfigExitsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is IList list)
            {
                if (list.OfType<IConfigViewModel>().Any())
                {
                    foreach (var config in list.OfType<IConfigViewModel>().SkipWhile(c => c is ConfigViewModel_Individual || c is ConfigViewModel_Group))
                    {
                        if (object.Equals(config.ProtocolType, parameter))
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
