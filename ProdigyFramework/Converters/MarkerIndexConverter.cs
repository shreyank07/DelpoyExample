using ProdigyFramework.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ProdigyFramework.Converters
{
    public class MarkerIndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] == null || values[0] == DependencyProperty.UnsetValue
                || values[1] == null || values[1] == DependencyProperty.UnsetValue)
                return DependencyProperty.UnsetValue;

            var markcollection = (ObservableCollection<Markerdetails>)values[0];
            if (markcollection.Count > 0 && markcollection.Any(x => x.Index == System.Convert.ToInt32(values[1])))
                return markcollection.Where(x => x.Index == System.Convert.ToInt32(values[1])).ToList()[0].Markername;
            else
                return "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
