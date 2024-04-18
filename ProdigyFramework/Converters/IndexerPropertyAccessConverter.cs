using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ProdigyFramework.Converters
{
    public class IndexerPropertyAccessConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(values == null || values == DependencyProperty.UnsetValue)
                return DependencyProperty.UnsetValue;

            //if (values.Length == 4 && values[0].ToString() == values[1].ToString())
            //{
            //    IList list = values[2] as IList;
            //    int index = 0;
            //    if (values[3] != null && int.TryParse(values[3].ToString(), out index))
            //    {
            //        object obj = list[index];

            //        if (!string.IsNullOrEmpty(parameter.ToString()))
            //        {
            //            var propertyInfo = obj.GetType().GetProperty(parameter.ToString());

            //            if (propertyInfo != null)
            //            {
            //                return propertyInfo.GetValue(obj, null).ToString();
            //            }
            //        }
            //    }
            //}
            string data = "";

            if (values.Length == 4 && values[0].ToString() == values[1].ToString())
            {
                IList list = values[2] as IList;
                object obj;
                int index = 0;
                if (values[3] != null && int.TryParse(values[3].ToString(), out index))
                {
                    if (list.Count == 12)
                    {
                        if (index == 1)
                        {
                            obj = list[7];
                            {
                                var propertyInfo = obj.GetType().GetProperty("Value");

                                if (propertyInfo != null)
                                {
                                    int number = System.Convert.ToInt32(propertyInfo.GetValue(obj, null).ToString());
                                    data = "0x" + System.Convert.ToString(number, 16).ToUpper();
                                }
                            }
                        }
                        else if (index == 2)
                        {
                            obj = list[9];
                            {
                                var propertyInfo = obj.GetType().GetProperty("Value");

                                if (propertyInfo != null)
                                {
                                    int number = System.Convert.ToInt32(propertyInfo.GetValue(obj, null).ToString());
                                    data = "0x" + System.Convert.ToString(number, 16).ToUpper();
                                }
                            }

                        }
                        else
                        {
                            obj = list[11];
                            if (!(obj.GetType().GetProperty("Description").GetValue(obj).ToString().Contains("Pass")))
                                data = "FAIL";
                        }
                    }
                    else
                    {
                        if (index == 1)
                        {
                            obj = list[list.Count - 3];
                            if (obj.GetType().GetProperty("Description").GetValue(obj).ToString().Contains("Frame"))
                            {
                                var propertyInfo = obj.GetType().GetProperty("Value");

                                if (propertyInfo != null)
                                {
                                    int number = System.Convert.ToInt32(propertyInfo.GetValue(obj, null).ToString());
                                    data = "0x" + System.Convert.ToString(number, 16).ToUpper();
                                }
                            }
                        }
                        if (index == 3)
                        {
                            obj = list[list.Count - 1];
                            if (!(obj.GetType().GetProperty("Description").GetValue(obj).ToString().Contains("Pass")))
                                data = "FAIL";
                        }
                    }
                }
                //if (list.Count == 12)
                //{
                //    obj = list[7];
                //    {
                //        var propertyInfo = obj.GetType().GetProperty("Value");

                //        if (propertyInfo != null)
                //        {
                //            int number = System.Convert.ToInt32(propertyInfo.GetValue(obj, null).ToString());
                //            data = "Frame Seq:" + System.Convert.ToString(number, 16).ToUpper() + "  ";
                //        }
                //    }
                //    obj = list[9];
                //    {
                //        var propertyInfo = obj.GetType().GetProperty("Value");

                //        if (propertyInfo != null)
                //        {
                //            int number = System.Convert.ToInt32(propertyInfo.GetValue(obj, null).ToString());
                //            data = data + "Credit Value:" + System.Convert.ToString(number, 16).ToUpper() + "  ";
                //        }
                //    }

                //    obj = list[11];
                //    if (!(obj.GetType().GetProperty("Description").GetValue(obj).ToString().Contains("Pass")))
                //        data = data + "CRC:FAIL ";
                //}
                //else
                //{
                //    obj = list[list.Count - 1];
                //    if (!(obj.GetType().GetProperty("Description").GetValue(obj).ToString().Contains("Pass")))
                //        data = data + "CRC:FAIL ";
                //}
            }

            return data;
            //return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
