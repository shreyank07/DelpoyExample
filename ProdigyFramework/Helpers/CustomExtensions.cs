using ProdigyFramework.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyFramework.Helpers
{
    public static class CustomExtensions
    {
        static CustomExtensions()
        {
        }
        public static int Calc_Crc5_Usb(byte[] inputData)
        {
            int[] crc = new int[5];
            int[] ch = new int[5]; //Hold previous for next calc
            int[] inputbyte = new int[8];
            byte SEED = 0x1F; // intialize seed value with 11111
            int icrc = 0;

            for (int i = 0; i < 5; ++i)
            {
                crc[i] = (SEED >> i) & 0x01;
            }
            foreach (byte b in inputData)
            {
                for (int i = 0; i < 8; i++)
                {
                    inputbyte[i] = (b >> (7 - i)) & 0x01;
                }

                for (int k = 0; k < 5; k = k + 1)
                {
                    ch[k] = crc[k];
                }
                crc[4] = ch[1] ^ ch[2] ^ inputbyte[2] ^ inputbyte[3] ^ inputbyte[5];
                crc[3] = ch[0] ^ ch[1] ^ ch[4] ^ inputbyte[0] ^ inputbyte[3] ^ inputbyte[4] ^ inputbyte[6];
                crc[2] = ch[0] ^ ch[3] ^ ch[4] ^ inputbyte[0] ^ inputbyte[1] ^ inputbyte[4] ^ inputbyte[5] ^ inputbyte[7];
                crc[1] = ch[1] ^ ch[3] ^ ch[4] ^ inputbyte[0] ^ inputbyte[1] ^ inputbyte[3] ^ inputbyte[6];
                crc[0] = ch[0] ^ ch[2] ^ ch[3] ^ inputbyte[1] ^ inputbyte[2] ^ inputbyte[4] ^ inputbyte[7];

            }
            for (int i = 0; i < 5; i++)
                icrc += crc[i] << i;
            return icrc;
        }
        public static string findTwoscomplement(StringBuilder str)
        {
            int n = str.Length;

            // Traverse the string to get 
            // first '1' from the last of string 
            int i;
            for (i = n - 1; i >= 0; i--)
            {
                if (str[i] == '1')
                {
                    break;
                }
            }

            // If there exists no '1' concat 1 
            // at the starting of string 
            if (i == -1)
            {
                return "1" + str;
            }

            // Continue traversal after the
            // position of first '1' 
            for (int k = i - 1; k >= 0; k--)
            {
                // Just flip the values 
                if (str[k] == '1')
                {
                    str.Remove(k, k + 1 - k).Insert(k, "0");
                }
                else
                {
                    str.Remove(k, k + 1 - k).Insert(k, "1");
                }
            }

            // return the modified string 
            return str.ToString();
        }
        public static void CopyPropertiesFrom(this object fromObject, object toObject)
        {
            var fromProperties = fromObject.GetType().GetProperties().Where(p => p.CanRead);
            var toProperties = toObject.GetType().GetProperties().Where(p => p.CanWrite);

            foreach (var fromProperty in fromProperties)
            {
                foreach (var toProperty in toProperties)
                {
                    if (fromProperty.Name == toProperty.Name && fromProperty.PropertyType == toProperty.PropertyType)
                    {
                        toProperty.SetValue(toObject, fromProperty.GetValue(fromObject));
                        break;
                    }
                }
            }
        }
        public static int CRC8_CCITT(byte[] input)
        {
            return new CRC8Calc(CRC8_POLY.CRC8_CCITT).Checksum(input);
        }

        public static int CRC7Calculation(byte[] CMDinput)
        {
            int icrc = 0;
            int[] crcbyte = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0 ,0};
            int[] inputbyte = new int[9];
            int xored;
            foreach (byte b in CMDinput)
            {
                for (int i = 0; i < 9; i++)
                {
                    inputbyte[i] = (b >> i) & 0x01;
                }
                for (int j = 8; j >= 0; j--)
                {
                    xored = crcbyte[7] ^ inputbyte[j];
                    crcbyte[7] = crcbyte[6];
                    crcbyte[6] = crcbyte[5];
                    crcbyte[5] = crcbyte[4];
                    crcbyte[4] = crcbyte[3];
                    crcbyte[3] = crcbyte[2];
                    crcbyte[2] = crcbyte[1] ^ xored;
                    crcbyte[1] = crcbyte[0] ^ xored;
                    crcbyte[0] = xored;
                }
            }
            for (int i = 0; i < 8; i++)
                icrc += (crcbyte[i] << i);
            return icrc;
        }
        public static string ConvertFormat(this int val, NumberFormat fmt)
        {
            string retVal = val.ToString();
            try
            {
                if (fmt == NumberFormat.HEX)
                {
                    retVal = String.Format("0x{0:X}", val);
                }
                else if (fmt == NumberFormat.DECIMAL)
                {
                    retVal = val.ToString();
                }
                else if (fmt == NumberFormat.BINARY)
                {
                    retVal = string.Format("b{0}", Convert.ToString(val, 2));
                }
                else if (fmt == NumberFormat.OCTAL)
                {
                    retVal = string.Format("O{0}", Convert.ToString(val, 8));
                }
                else if (fmt == NumberFormat.ASCII)
                {
                    retVal = string.Format("'{0}'", Convert.ToChar(val));
                }
            }
            catch (Exception)
            {
                //AppManager.WriteToLog("Utilities : ConvertFormat() - ", ex);
            }

            return retVal;
        }

        public static void RemoveAll<T>(this ICollection<T> source, Func<T, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source", "source is null.");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate", "predicate is null.");
            }

            source.Where(predicate).ToList().ForEach(e => source.Remove(e));
        }

        /// <summary>
        /// Formats the time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="inUnits">The in units.</param>
        /// <returns></returns>
        public static string FormatTime(this double value, Units inUnits)
        {
            bool isNegative = false;
            string retVal = "";
            try
            {
                if (value < 0)
                {
                    isNegative = true;
                    value = value * -1;
                }

                retVal = FormatNumber(value, inUnits);
                if (isNegative == true)
                {
                    retVal = string.Format("-{0}S", retVal);
                }
                else
                {
                    if (retVal == "NA")
                    {
                        return retVal;
                    }
                    else if (inUnits == Units.PERCENT)
                    {
                        retVal = string.Format("{0} %", retVal);
                    }
                    else
                    {
                        retVal = string.Format("{0}S", retVal);
                    }
                }
            }
            catch
            {
                throw;
            }
            return retVal;
        }

        public static string RoundValue(this string value, int round)
        {
            return value;
        }

        /// <summary>
        /// Formats the volt.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="inUnits">The in units.</param>
        /// <returns></returns>
        public static string FormatVolt(this double value, Units inUnits)
        {
            bool isNegative = false;
            string retVal = "";
            try
            {
                if (value < 0)
                {
                    isNegative = true;
                    value = value * -1;
                }

                retVal = FormatNumber(value, inUnits);

                if (isNegative == true)
                {
                    retVal = string.Format("-{0}V", retVal);
                }
                else
                {
                    retVal = string.Format("{0}V", retVal);
                }
            }
            catch
            {
                throw;
            }
            return retVal;
        }

        /// <summary>
        /// Formats the number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="inUnits">The in units.</param>
        /// <returns></returns>
        public static string FormatNumber(this double value, Units inUnits, int round = 4)
        {
            try
            {
                string strFormat = "0." + "".PadRight(round, '#');

                string retVal = "";
                double baseUnitValue = value;
                if (double.IsNaN(baseUnitValue))
                    return "NA";
                if (!(inUnits == Units.BASE_UNIT || inUnits == Units.SECONDS))
                {
                    baseUnitValue = ConvertToBaseUnits(value, inUnits);
                }

                retVal = baseUnitValue.ToString("#.0000E+0");

                if (retVal.Length <= 8)
                {
                    //error
                    return "0.0000";
                }

                //string pow = retVal.Substring(8);  
                int power;
                int.TryParse(retVal.Substring(8), out power);

                StringBuilder strBuilder = new StringBuilder(retVal);
                double roundOffValue = value;

                if (retVal[7] == '+')
                {
                    switch (power)
                    {
                        case 0:
                            //X.XXXX
                            strBuilder.Remove(6, retVal.Length - 6);
                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue) < value)
                                {
                                    roundOffValue = roundOffValue + 0.0001;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }

                            break;
                        case 1:
                            // XX.XXX
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(2, '.');
                            strBuilder.Remove(6, retVal.Length - 6);
                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue) < value)
                                {
                                    roundOffValue = roundOffValue + 0.001;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            break;
                        case 2:
                            //XXX.XX
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(3, '.');
                            strBuilder.Remove(6, retVal.Length - 6);
                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue) < value)
                                {
                                    roundOffValue = roundOffValue + 0.01;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            break;
                        case 3:
                            //X.XXXX
                            strBuilder.Remove(6, retVal.Length - 6);
                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue * 1000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.0001;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}K", retVal);
                            //strBuilder.Append(" K");
                            //retVal = strBuilder.ToString();

                            break;
                        case 4:
                            //XX.XXX
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(2, '.');
                            strBuilder.Remove(6, retVal.Length - 6);

                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue * 1000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.001;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}K", retVal);
                            //retVal = strBuilder.ToString();
                            break;
                        case 5:
                            //XXX.XX
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(3, '.');
                            strBuilder.Remove(6, retVal.Length - 6);

                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue * 1000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.01;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}K", retVal);
                            //strBuilder.Append(" K");
                            //retVal = strBuilder.ToString();
                            break;
                        case 6:
                            //X.XXXX
                            strBuilder.Remove(6, retVal.Length - 6);

                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue * 1E6) < value)
                                {
                                    roundOffValue = roundOffValue + 0.0001;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}M", retVal);
                            //strBuilder.Append(" M");
                            //retVal = strBuilder.ToString();
                            break;
                        case 7:
                            //XX.XXX
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(2, '.');
                            strBuilder.Remove(6, retVal.Length - 6);
                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue * 1E6) < value)
                                {
                                    roundOffValue = roundOffValue + 0.001;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}M", retVal);
                            //strBuilder.Append(" M");
                            //retVal = strBuilder.ToString();
                            break;
                        case 8:
                            //XXX.XX
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(3, '.');
                            strBuilder.Remove(6, retVal.Length - 6);
                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue * 1E6) < value)
                                {
                                    roundOffValue = roundOffValue + 0.01;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}M", retVal);
                            //strBuilder.Append(" M");
                            //retVal = strBuilder.ToString();
                            break;
                        case 9:
                            //X.XXXX
                            strBuilder.Remove(6, retVal.Length - 6);
                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue * 1E9) < value)
                                {
                                    roundOffValue = roundOffValue + 0.0001;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}G", retVal);
                            //strBuilder.Append(" G");
                            //retVal = strBuilder.ToString();
                            break;
                        case 10:
                            //XX.XXX
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(2, '.');
                            strBuilder.Remove(6, retVal.Length - 6);
                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue * 1E9) < value)
                                {
                                    roundOffValue = roundOffValue + 00.001;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}G", retVal);
                            //strBuilder.Append(" G");
                            //retVal = strBuilder.ToString();
                            break;
                        case 11:
                            //XXX.XX
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(3, '.');
                            strBuilder.Remove(6, retVal.Length - 6);
                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue * 1E9) < value)
                                {
                                    roundOffValue = roundOffValue + 0.01;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}G", retVal);
                            //strBuilder.Append(" G");
                            //retVal = strBuilder.ToString();
                            break;
                        default:

                            break;
                    }
                }
                else
                {
                    switch (power)
                    {
                        case -1:

                            // 1.2345E-1  = -0.00045 E-1 
                            strBuilder.Remove(2, 1);
                            strBuilder.Insert(4, '.');
                            strBuilder.Remove(6, retVal.Length - 6);

                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue / 1000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.01;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}m", retVal);
                            break;
                        case 0:

                            strBuilder.Remove(6, retVal.Length - 6);
                            // retVal = Math.Round(Convert.ToDouble(strBuilder), round).ToString();
                            retVal = strBuilder.ToString();

                            break;
                        case 1:
                            // 1.2345E-1  = 123.45 E-3 
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(3, '.');
                            strBuilder.Remove(6, retVal.Length - 6);

                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue / 1000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.01;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}m", retVal);

                            //strBuilder.Append(" m");
                            //retVal = strBuilder.ToString();
                            break;
                        case 2:
                            // 1.2345E-2  = 12.345 E-3
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(2, '.');
                            strBuilder.Remove(6, retVal.Length - 6);

                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue / 1000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.001;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }

                            retVal = string.Format("{0}m", retVal);
                            //strBuilder.Append(" m");
                            //retVal = strBuilder.ToString();

                            break;
                        case 3:
                            // 1.2345E-3  = 1.2345 E-3
                            strBuilder.Remove(6, retVal.Length - 6);
                            //  strBuilder.Append(" m");
                            retVal = strBuilder.ToString();

                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue / 1000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.0001;

                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            //  retVal = Convert.ToDouble(retVal).ToString(strFormat);
                            retVal = string.Format("{0}m", retVal);

                            break;
                        case 4:
                            // 1.2345E-4  = 123.45 E-6
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(3, '.');
                            strBuilder.Remove(6, retVal.Length - 6);

                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue / 1000000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.01;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}µ", retVal);

                            //strBuilder.Append(" µ");
                            //retVal = strBuilder.ToString();
                            break;
                        case 5:
                            // 1.2345E-5  = 12.345 E-6
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(2, '.');
                            strBuilder.Remove(6, retVal.Length - 6);

                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue / 1000000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.001;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}µ", retVal);

                            //strBuilder.Append(" µ");
                            //retVal = strBuilder.ToString();
                            break;
                        case 6:

                            // 1.2345E-6  = 1.2345 E-6
                            strBuilder.Remove(6, retVal.Length - 6);

                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue / 1000000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.0001;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}µ", retVal);

                            //strBuilder.Append(" µ");
                            //retVal = strBuilder.ToString();
                            break;
                        case 7:
                            // 1.2345E-7  = 123.45 nS
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(3, '.');
                            strBuilder.Remove(6, retVal.Length - 6);

                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue / 1000000000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.01;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}n", retVal);

                            //strBuilder.Append(" n");
                            //retVal = strBuilder.ToString();
                            break;
                        case 8:
                            // 1.2345E-8  = 12.345 E nS
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(2, '.');
                            strBuilder.Remove(6, retVal.Length - 6);

                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue / 1000000000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.001;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}n", retVal);

                            //strBuilder.Append(" n");
                            //retVal = strBuilder.ToString();
                            break;
                        case 9:
                            // 1.2345E-9  = 1.2345 E-9
                            strBuilder.Remove(6, retVal.Length - 6);
                            //strBuilder.Append(" n");

                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue / 1000000000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.0001;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}n", retVal);

                            //retVal = strBuilder.ToString();
                            break;
                        case 10:
                            // 1.2345E-10 = 123.45 E-12
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(3, '.');
                            strBuilder.Remove(6, retVal.Length - 6);

                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue / 1000000000000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.01;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}p", retVal);

                            //strBuilder.Append(" p");
                            //retVal = strBuilder.ToString();
                            break;
                        case 11:
                            // 1.2345E-11 = 12.345 E-12
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(2, '.');
                            strBuilder.Remove(6, retVal.Length - 6);

                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue / 1000000000000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.001;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}p", retVal);

                            //strBuilder.Append(" p");
                            //retVal = strBuilder.ToString();
                            break;
                        case 12:
                            // 1.2345E-12 = 1.2345 E-12
                            strBuilder.Remove(6, retVal.Length - 6);
                            //   strBuilder.Append(" p");
                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue / 1000000000000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.0001;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}p", retVal);

                            //retVal = strBuilder.ToString();
                            break;
                        case 13:

                            // 1.2345E-13 = 123.45 E-15
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(3, '.');
                            strBuilder.Remove(6, retVal.Length - 6);

                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue / 1000000000000000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.01;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}f", retVal);

                            //strBuilder.Append(" f");
                            //retVal = strBuilder.ToString();
                            break;
                        case 14:

                            // 1.2345E-14 = 12.345 E-15
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(2, '.');
                            strBuilder.Remove(6, retVal.Length - 6);
                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue / 1000000000000000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.001;
                                    roundOffValue = (Math.Round(roundOffValue, round));
                                    retVal = roundOffValue.ToString(strFormat);
                                }
                                else
                                {
                                    retVal = Convert.ToDouble(retVal).ToString(strFormat);
                                }
                            }
                            retVal = string.Format("{0}f", retVal);
                            //strBuilder.Append(" f");
                            //retVal = strBuilder.ToString();
                            break;
                        case 15:

                            // 1.2345E-15 = 12.345 E-15
                            strBuilder.Remove(6, retVal.Length - 6);
                            strBuilder.Append("f");
                            retVal = strBuilder.ToString();
                            break;
                        default:

                            break;
                    }
                }

                return retVal;
            }
            catch
            {
                return "0";
            }
        }

        public static string FormatNumber(this double value, int round)
        {
            string strRounded = value.ToString("e");
            int pow = int.Parse(strRounded.Split('e')[1]);
            string unit = "";

            if (pow > 0)
            {
                if (pow >= 9)
                {
                    unit = "G";
                    pow -= 9;
                }
                else if (pow >= 6)
                {
                    unit = "M";
                    pow -= 6;
                }
                else if (pow >= 3)
                {
                    unit = "K";
                    pow -= 3;
                }
            }
            else if (pow < 0)
            {
                if (pow >= -3)
                {
                    unit = "m";
                    pow += 3;
                }
                else if (pow >= -6)
                {
                    unit = "µ";
                    pow += 6;
                }
                else if (pow >= -9)
                {
                    unit = "n";
                    pow += 9;
                }
                else
                {
                    unit = "p";
                    pow += 12;
                }
            }

            strRounded = strRounded.Split('e')[0];
            strRounded = string.Format("{0}{1}", Math.Round(double.Parse(strRounded) * Math.Pow(10, pow), round).ToString(), unit);

            return strRounded;
        }

        public static double FormatStringToNumber(this string strValue)
        {
            try
            {
                double value;
                if (double.TryParse(strValue, out value))
                {
                    return value;
                }
                else if (double.TryParse(strValue.Substring(0, strValue.Length - 1), out value))
                {
                    switch (strValue[strValue.Length - 1])
                    {
                        case 'G':
                            value = value * 1E9;
                            break;
                        case 'M':
                            value = value * 1E6;
                            break;
                        case 'K':
                            value = value * 1E3;
                            break;
                        case 'm':
                            value = value * 1E-3;
                            break;
                        case 'µ':
                            value = value * 1E-6;
                            break;
                        case 'n':
                            value = value * 1E-9;
                            break;
                        case 'p':
                            value = value * 1E-12;
                            break;
                        case 'f':
                            value = value * 1E-15;
                            break;
                        default:
                            value = value;
                            break;
                    }
                }

                return value;
            }
            catch
            {
                throw;
            }
        }

        public static string DisplayMeasurementValue(double value, TypeOfMeasurement measurementType)
        {
            string strValue = "";
            switch (measurementType)
            {
                case TypeOfMeasurement.AMPLITUDE:
                case TypeOfMeasurement.HIGH:
                case TypeOfMeasurement.LOW:
                case TypeOfMeasurement.MAXIMUM:
                case TypeOfMeasurement.MINIMUM:
                case TypeOfMeasurement.PK2PK:
                case TypeOfMeasurement.RMS:
                case TypeOfMeasurement.AMPLITUDE_TREND:
                case TypeOfMeasurement.MAXIMUM_TREND:
                case TypeOfMeasurement.MINIMUM_TREND:
                case TypeOfMeasurement.RMS_TREND:
                    strValue = value.FormatVolt(Units.BASE_UNIT);
                    break;
                case TypeOfMeasurement.POVERSHOOT:
                case TypeOfMeasurement.NOVERSHOOT:
                case TypeOfMeasurement.PDUTY:
                case TypeOfMeasurement.NDUTY:
                case TypeOfMeasurement.POSITIVE_OVERSHOOT_TREND:
                case TypeOfMeasurement.NEGATIVE_OVERSHOOT_TREND:
                case TypeOfMeasurement.POSITIVE_DUTY_CYCLE_TREND:
                case TypeOfMeasurement.NEGATIVE_DUTY_CYCLE_TREND:
                    strValue = string.Format("{0}%", value.FormatNumber(Units.BASE_UNIT));
                    break;
                case TypeOfMeasurement.RISE:
                case TypeOfMeasurement.FALL:
                case TypeOfMeasurement.PERIOD:
                case TypeOfMeasurement.PWIDTH:
                case TypeOfMeasurement.NWIDTH:
                case TypeOfMeasurement.RISE_TIME_TREND:
                case TypeOfMeasurement.FALL_TIME_TREND:
                case TypeOfMeasurement.PERIOD_TREND:
                case TypeOfMeasurement.POSITIVE_WIDTH_TREND:
                case TypeOfMeasurement.NEGATIVE_WIDTH_TREND:
                    strValue = value.FormatTime(Units.BASE_UNIT);
                    break;
                case TypeOfMeasurement.FREQUENCY:
                case TypeOfMeasurement.FREQUENCY_TREND:
                    strValue = string.Format("{0}Hz", value.FormatNumber(Units.BASE_UNIT));
                    break;
                default:
                    strValue = value.FormatNumber(Units.BASE_UNIT);
                    break;
            }

            return strValue;
        }

        /// <summary>
        /// Reverses the specified string.
        /// </summary>
        /// <param name="strToReverse">The string to reverse.</param>
        /// <returns></returns>
        public static string Reverse(this string strToReverse)
        {
            string reversed = "";
            for (int i = strToReverse.Length - 1; i >= 0; i--)
            {
                reversed += strToReverse[i].ToString();
            }

            return reversed;
        }

        /// <summary>
        ///  This static funtion used for smothing the wave waveform returns smoth waveform index
        /// </summary>
        /// <param name="start1"></param>
        /// <param name="end1"></param>
        /// <param name="start2"></param>
        /// <param name="end2"></param>
        /// <param name="mid2"></param>
        /// <returns></returns>
        public static long GetEquivalent(long start1, long end1, long start2, long end2, long mid2)
        {
            double mid1;
            mid1 = ((((end2 - mid2) / (double)(end2 - start2)) * (start1 - end1)) + end1);
            return (long)mid1;
        }

        /// <summary>
        ///  This static funtion used for smothing the wave waveform returns smoth waveform index
        /// </summary>
        /// <param name="start1"></param>
        /// <param name="end1"></param>
        /// <param name="start2"></param>
        /// <param name="end2"></param>
        /// <param name="mid2"></param>
        /// <returns></returns>
        public static double GetEquivalent(double start1, double end1, double start2, double end2, double mid2)
        {
            return start1 + (((mid2 - start2) * (end1 - start1)) / (end2 - start2));
        }

        /// <summary>
        /// This extension method is broken out so you can use a similar pattern with 
        /// other MetaData elements in the future. This is your base method for each.
        /// </summary>
        /// <param name="value">Enum value.</param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return (T)attributes[0];
        }

        /// <summary>
        /// This method creates a specific call to the above method, requesting the
        /// Description MetaData attribute.
        /// </summary>
        /// <param name="value">Enum value.</param>
        /// <returns></returns>
        public static string ToName(this Enum value)
        {
            DescriptionAttribute attribute = null;
            if (value.GetType().GetField(value.ToString()).IsDefined(typeof(DescriptionAttribute), false))
                attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        /// <summary>
        /// Converts to base units.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="inUnits">The in units.</param>
        /// <returns></returns>
        private static double ConvertToBaseUnits(double value, Units inUnits)
        {
            double retVal = 0.0;
            try
            {
                switch (inUnits)
                {
                    case Units.GIGA:
                        retVal = value * 1E9;
                        break;
                    case Units.MEGA:
                        retVal = value * 1E6;
                        break;
                    case Units.KILO:
                        retVal = value * 1E3;
                        break;
                    case Units.BASE_UNIT:
                        retVal = value * 1;
                        break;
                    case Units.MILLI:
                        retVal = value * 1E-3;
                        break;
                    case Units.MICRO:
                        retVal = value * 1E-6;
                        break;
                    case Units.NANO:
                        retVal = value * 1E-9;
                        break;
                    case Units.PICO:
                        retVal = value * 1E-12;
                        break;
                    case Units.FEMTO:
                        retVal = value * 1E-15;
                        break;
                    case Units.PERCENT:
                        retVal = value * 1;
                        break;
                    default:
                        retVal = value;
                        break;
                }
            }
            catch
            {
                throw;
            }
            return retVal;
        }

        public static int GetIndex(this System.Collections.Generic.Dictionary<string, string> dictionary, string key)
        {
            for (int index = 0; index < dictionary.Count; index++)
            {
                if (dictionary.ElementAt(index).Value == key)
                    return index; // We found the item
            }

            return -1;
        }
    }
}
