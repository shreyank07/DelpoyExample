using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;


namespace ProdigyFramework.Converters
{
    class EnumToBinaryConvert : IValueConverter
    {
        Stack<object> stack = new Stack<object>();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object[] digits = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, "A", "B", "C", "D", "E", "F" };
            try
            {
                // Get the source value
                int number = 0;
                if (parameter is Type)
                {
                    number = (int)Enum.Parse((Type)parameter, value.ToString());
                }
                // Get the binary number system
                string binary = System.Convert.ToString(number, 2).PadLeft(8, '0');

                return binary;
                // Return result
            }
            catch (Exception)
            {
                return null;
            }
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
