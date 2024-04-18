using PGYMiniCooper.CoreModule.ViewModel;
using PGYMiniCooper.CoreModule.ViewModel.Interfaces;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace PGYMiniCooper.CoreModule.View.Converters
{
    public class ProtocolConfigCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is IList list)
            {
                if (list.OfType<IConfigViewModel>().Any())
                {
                    return list.OfType<IConfigViewModel>()
                        .SkipWhile(c=>c is ConfigViewModel_Individual || c is ConfigViewModel_Group)
                        .Count(c => object.Equals(c.ProtocolType, parameter));
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
