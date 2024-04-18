using PGYMiniCooper.DataModule;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PGYMiniCooper.CoreModule.View.Converters
{
    public class ErrorDescriptionConverter : IValueConverter
    {
        public string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
                return attrib.Description;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Enum myEnum = (Enum)value;
            //string description = GetEnumDescription(myEnum);
            //return description;
            return GetDescription((List<eErrorType>)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }

        public string GetDescription(List<eErrorType> val)
        {
            string strarghex = "";
            string description = "";
            if (val.Count == 0)
                return strarghex;
            for (int b = 0; b < val.Count; b++)
            {
                Enum myEnum = (Enum)val[b];
                description += myEnum + " : " + GetEnumDescription(myEnum) + "\n";
            }
            return description;
        }
    }
}
