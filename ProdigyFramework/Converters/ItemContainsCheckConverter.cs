using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ProdigyFramework.Converters
{
    public class ItemContainsCheckConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2) return false;
            if (values[0] == null || values[1] == null || values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return DependencyProperty.UnsetValue;

            var item = values[0];
            IList collection = (IList)values[1];
            return collection.Contains(item);

            //Collections.DataWrapper item = values[0] as Collections.DataWrapper;
            //IList collection = (IList)values[1];
            //if (collection.Count > 0)
            //{
            //    var item0 = collection[0] as Collections.DataWrapper;
            //    return item.Index >= item0.Index && item.Index < (item0.Index + collection.Count);
            //}
            //else
            //{
            //    return DependencyProperty.UnsetValue; ;
            //}
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
