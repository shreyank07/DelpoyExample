using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyFramework.Extension
{
    public static class FrameworkExtensions
    {
        public static IEnumerable<Type> GetTypesWith<TAttribute>(bool inherit)
                              where TAttribute : System.Attribute
        {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
                   from t in a.GetTypes()
                   where t.IsDefined(typeof(TAttribute), inherit)
                   select t;
        }

        unsafe static partial class FasterBitConverter
        {
            #region from/to hex
            // assigned int values for bytes (0-255)
            static readonly int[] toHexTable = new int[] {
            3145776, 3211312, 3276848, 3342384, 3407920, 3473456, 3538992, 3604528, 3670064, 3735600,
            4259888, 4325424, 4390960, 4456496, 4522032, 4587568, 3145777, 3211313, 3276849, 3342385,
            3407921, 3473457, 3538993, 3604529, 3670065, 3735601, 4259889, 4325425, 4390961, 4456497,
            4522033, 4587569, 3145778, 3211314, 3276850, 3342386, 3407922, 3473458, 3538994, 3604530,
            3670066, 3735602, 4259890, 4325426, 4390962, 4456498, 4522034, 4587570, 3145779, 3211315,
            3276851, 3342387, 3407923, 3473459, 3538995, 3604531, 3670067, 3735603, 4259891, 4325427,
            4390963, 4456499, 4522035, 4587571, 3145780, 3211316, 3276852, 3342388, 3407924, 3473460,
            3538996, 3604532, 3670068, 3735604, 4259892, 4325428, 4390964, 4456500, 4522036, 4587572,
            3145781, 3211317, 3276853, 3342389, 3407925, 3473461, 3538997, 3604533, 3670069, 3735605,
            4259893, 4325429, 4390965, 4456501, 4522037, 4587573, 3145782, 3211318, 3276854, 3342390,
            3407926, 3473462, 3538998, 3604534, 3670070, 3735606, 4259894, 4325430, 4390966, 4456502,
            4522038, 4587574, 3145783, 3211319, 3276855, 3342391, 3407927, 3473463, 3538999, 3604535,
            3670071, 3735607, 4259895, 4325431, 4390967, 4456503, 4522039, 4587575, 3145784, 3211320,
            3276856, 3342392, 3407928, 3473464, 3539000, 3604536, 3670072, 3735608, 4259896, 4325432,
            4390968, 4456504, 4522040, 4587576, 3145785, 3211321, 3276857, 3342393, 3407929, 3473465,
            3539001, 3604537, 3670073, 3735609, 4259897, 4325433, 4390969, 4456505, 4522041, 4587577,
            3145793, 3211329, 3276865, 3342401, 3407937, 3473473, 3539009, 3604545, 3670081, 3735617,
            4259905, 4325441, 4390977, 4456513, 4522049, 4587585, 3145794, 3211330, 3276866, 3342402,
            3407938, 3473474, 3539010, 3604546, 3670082, 3735618, 4259906, 4325442, 4390978, 4456514,
            4522050, 4587586, 3145795, 3211331, 3276867, 3342403, 3407939, 3473475, 3539011, 3604547,
            3670083, 3735619, 4259907, 4325443, 4390979, 4456515, 4522051, 4587587, 3145796, 3211332,
            3276868, 3342404, 3407940, 3473476, 3539012, 3604548, 3670084, 3735620, 4259908, 4325444,
            4390980, 4456516, 4522052, 4587588, 3145797, 3211333, 3276869, 3342405, 3407941, 3473477,
            3539013, 3604549, 3670085, 3735621, 4259909, 4325445, 4390981, 4456517, 4522053, 4587589,
            3145798, 3211334, 3276870, 3342406, 3407942, 3473478, 3539014, 3604550, 3670086, 3735622,
            4259910, 4325446, 4390982, 4456518, 4522054, 4587590
        };

            public static string ToHexString(byte[] source, int bytes)
            {
                return ToHexString(source, bytes, false);
            }

            // hexIndicator: use prefix ("0x") or not
            public static string ToHexString(byte[] source, int btyes, bool hexIndicator)
            {
                // freeze toHexTable position in memory
                fixed (int* hexRef = toHexTable)
                // freeze source position in memory
                fixed (byte* sourceRef = source)
                {
                    // take first parsing position of source - allow inline pointer positioning
                    byte* s = sourceRef;
                    // calculate result length
                    int resultLen = btyes << 1; //(source.Length << 1);
                                                // use prefix ("Ox")
                    if (hexIndicator)
                        // adapt result length
                        resultLen += 2;
                    // initialize result string with any character expect '\0'
                    string result = new string(' ', resultLen);
                    // take the first character address of result
                    fixed (char* resultRef = result)
                    {
                        // pairs of characters explain the endianess of toHexTable
                        // move on by pairs of characters (2 x 2 bytes) - allow inline pointer positioning
                        int* pair = (int*)resultRef;
                        // use prefix ("Ox") ?
                        if (hexIndicator)
                            // set first pair value
                            *pair++ = 7864368;
                        // more to go
                        while (*pair != 0)
                            // set the value of the current pair and move to next pair and source byte
                            *pair++ = hexRef[*s++];
                        return result;
                    }
                }
            }

            // values for '\0' to 'f' where 255 indicates invalid input character
            // starting from '\0' and not from '0' costs 48 bytes
            // but results 0 subtructions and less if conditions
            static readonly byte[] fromHexTable = new byte[] {
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 0, 1,
            2, 3, 4, 5, 6, 7, 8, 9, 255, 255,
            255, 255, 255, 255, 255, 10, 11, 12, 13, 14,
            15, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 10, 11, 12,
            13, 14, 15
        };

            // same as above but valid values are multiplied by 16
            static readonly byte[] fromHexTable16 = new byte[] {
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 0, 16,
            32, 48, 64, 80, 96, 112, 128, 144, 255, 255,
            255, 255, 255, 255, 255, 160, 176, 192, 208, 224,
            240, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 160, 176, 192,
            208, 224, 240
        };

            public static byte[] FromHexString(string source)
            {
                // return an empty array in case of null or empty source
                if (string.IsNullOrEmpty(source))
                    return new byte[0]; // you may change it to return null
                if (source.Length % 2 == 1) // source length must be even
                    throw new ArgumentException();
                int
                    index = 0, // start position for parsing source
                    len = source.Length >> 1; // initial length of result
                                              // take the first character address of source
                fixed (char* sourceRef = source)
                {
                    if (*(int*)sourceRef == 7864368) // source starts with "0x"
                    {
                        if (source.Length == 2) // source must not be just a "0x")
                            throw new ArgumentException();
                        index += 2; // start position (bypass "0x")
                        len -= 1; // result length (exclude "0x")
                    }
                    byte add = 0; // keeps a fromHexTable value
                    byte[] result = new byte[len]; // initialization of result for known length
                                                   // freeze fromHexTable16 position in memory
                    fixed (byte* hiRef = fromHexTable16)
                    // freeze fromHexTable position in memory
                    fixed (byte* lowRef = fromHexTable)
                    // take the first byte address of result
                    fixed (byte* resultRef = result)
                    {
                        // take first parsing position of source - allow inremental memory position
                        char* s = (char*)&sourceRef[index];
                        // take first byte position of result - allow incremental memory position
                        byte* r = resultRef;
                        // source has more characters to parse
                        while (*s != 0)
                        {
                            // check for non valid characters in pairs
                            // you may split it if you don't like its readbility
                            if (
                                // check for character > 'f'
                                *s > 102 ||
                                // assign source value to current result position and increment source position
                                // and check if is a valid character
                                (*r = hiRef[*s++]) == 255 ||
                                // check for character > 'f'
                                *s > 102 ||
                                // assign source value to "add" parameter and increment source position
                                // and check if is a valid character
                                (add = lowRef[*s++]) == 255
                                )
                                throw new ArgumentException();
                            // set final value of current result byte and move pointer to next byte
                            *r++ += add;
                        }
                        return result;
                    }
                }
            }
            #endregion
        }

        public static string GetHexString(this byte[] bytes)
        {
            return FasterBitConverter.ToHexString(bytes, bytes.Length);
        }

        public static byte[] GetBytes(this string hexString)
        {
            return FasterBitConverter.FromHexString(hexString);
        }

        public static string GetHexString(this byte[] bytes, string byteSeparator)
        {
            string hexString = FasterBitConverter.ToHexString(bytes, bytes.Length);
            int count = hexString.Length / 2;
            StringBuilder retString = new StringBuilder(count + hexString.Length);
            for (int i = 0; i < count; i++)
            {
                retString.Append(hexString.Substring(i * 2, 2));
                retString.Append(byteSeparator);
            }

            return retString.ToString();
        }

        public static byte[] GetBytes(this string hexString, string byteSeparator)
        {
            string str = hexString.Replace(byteSeparator, "");
            if (str.Length % 2 == 1) str += '0';
            return FasterBitConverter.FromHexString(str);
        }

        public static string GetUSBBytes(this byte[] bytes)
        {
            StringBuilder retString = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 0)
            {
                retString.AppendLine(string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15}",
                    bytes[i++].ToString("X2"), bytes[i++].ToString("X2"), bytes[i++].ToString("X2"), bytes[i++].ToString("X2"), bytes[i++].ToString("X2"),
                    bytes[i++].ToString("X2"), bytes[i++].ToString("X2"), bytes[i++].ToString("X2"), bytes[i++].ToString("X2"), bytes[i++].ToString("X2"),
                    bytes[i++].ToString("X2"), bytes[i++].ToString("X2"), bytes[i++].ToString("X2"), bytes[i++].ToString("X2"), bytes[i++].ToString("X2"), bytes[i++].ToString("X2")));
            }
            return retString.ToString();
        }
        
    }

    public enum NumberFormat
    {
        Hex = 0,
        Decimal = 1,
        Binary = 2,
        Octal = 3,
        ASCII = 4,
        SIGNED = 5
    }

    public enum UnitPrefixes
    {
        GIGA = 9,
        MEGA = 6,
        KILO = 3,
        MILLI = -3,
        MICRO = -6,
        NANO = -9,
        PICO = -12,
        FEMTO = -15,
        BASE_PREFIX = 1
    }

    public enum Units
    {
        VOLTS,
        TIME,
        PERCENTAGE,
        UNITINTERVAL
    }

    public static class NumberFormaterExtension
    {
        static NumberFormaterExtension()
        {
            _UnitPrefixesSymbols.Add(UnitPrefixes.GIGA, "G");
            _UnitPrefixesSymbols.Add(UnitPrefixes.MEGA, "M");
            _UnitPrefixesSymbols.Add(UnitPrefixes.KILO, "K");
            _UnitPrefixesSymbols.Add(UnitPrefixes.BASE_PREFIX, "");
            _UnitPrefixesSymbols.Add(UnitPrefixes.MILLI, "m");
            _UnitPrefixesSymbols.Add(UnitPrefixes.MICRO, "μ");
            _UnitPrefixesSymbols.Add(UnitPrefixes.NANO, "n");
            _UnitPrefixesSymbols.Add(UnitPrefixes.PICO, "p");
            _UnitPrefixesSymbols.Add(UnitPrefixes.FEMTO, "f");
        }

        static Dictionary<UnitPrefixes, string> _UnitPrefixesSymbols = new Dictionary<UnitPrefixes, string>();

        public static string ConvertFormat(this int val, NumberFormat numberFormat)
        {
            string retVal = val.ToString();
            try
            {
                if (numberFormat == NumberFormat.Hex)
                {
                    retVal = String.Format("0x{0:X}", val);
                }
                else if (numberFormat == NumberFormat.Decimal)
                {
                    retVal = val.ToString();
                }
                else if (numberFormat == NumberFormat.Binary)
                {
                    retVal = string.Format("b{0}", Convert.ToString(val, 2));
                }
                else if (numberFormat == NumberFormat.Octal)
                {
                    retVal = string.Format("O{0}", Convert.ToString(val, 8));
                }
                else if (numberFormat == NumberFormat.ASCII)
                {
                    retVal = string.Format("'{0}'", Convert.ToChar(val));
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }

            return retVal;
        }

        /// <summary>
        /// Formats the time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="inUnits">The in units.</param>
        /// <returns></returns>
        public static string FormatTime(this double value, UnitPrefixes unitPrefix)
        {
            bool isNegative = false;
            string retVal = string.Empty;
            try
            {
                if (value < 0)
                {
                    isNegative = true;
                    value = value * -1;
                }

                retVal = FormatNumber(value, unitPrefix);
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
        /// <param name="unitPrefix">The in units.</param>
        /// <returns></returns>
        public static string FormatVolt(this double value, UnitPrefixes unitPrefix)
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

                retVal = FormatNumber(value, unitPrefix);

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
        public static string FormatNumber(this double value, UnitPrefixes unitPrefix, int round = 4)
        {
            try
            {
                string retVal = "";
                double baseUnitValue = value;
                if (unitPrefix != UnitPrefixes.BASE_PREFIX)
                {
                    baseUnitValue = ConvertToBaseUnits(value, unitPrefix);
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
                                }
                            }
                            //  retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    //  retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
                                    retVal = roundOffValue.ToString().PadRight(round, '0');
                                }
                                else
                                {
                                    retVal = Math.Round(Convert.ToDouble(retVal), round).ToString();
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
            UnitPrefixes unitPrefix = UnitPrefixes.BASE_PREFIX;
            if (pow > 0)
            {
                if (pow >= 9)
                {
                    unitPrefix = UnitPrefixes.GIGA;
                }
                else if (pow >= 6)
                {
                    unitPrefix = UnitPrefixes.MEGA;
                }
                else if (pow >= 3)
                {
                    unitPrefix = UnitPrefixes.KILO;
                }
            }
            else if (pow < 0)
            {
                if (pow >= -3)
                {
                    unitPrefix = UnitPrefixes.MILLI;
                }
                else if (pow >= -6)
                {
                    unitPrefix = UnitPrefixes.MICRO;
                }
                else if (pow >= -9)
                {
                    unitPrefix = UnitPrefixes.NANO;
                }
                else if (pow >= -12)
                {
                    unitPrefix = UnitPrefixes.PICO;
                }
                else if (pow >= -15)
                {
                    unitPrefix = UnitPrefixes.FEMTO;
                }
            }

            if (unitPrefix != UnitPrefixes.BASE_PREFIX)
                pow -= (int)unitPrefix;

            strRounded = strRounded.Split('e')[0];
            strRounded = string.Format("{0}{1}", Math.Round(double.Parse(strRounded) * Math.Pow(10, pow), round).ToString(),
                _UnitPrefixesSymbols[unitPrefix]);

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
                    }
                }

                return value;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Converts to base units.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="unitPrefix">The in units.</param>
        /// <returns></returns>
        private static double ConvertToBaseUnits(double value, UnitPrefixes unitPrefix)
        {
            return value * (double)unitPrefix;
        }

        public static bool IsFileInUse(string path)
        {
            FileStream stream = null;
            try
            {
                if (File.Exists(path))
                    stream = new FileInfo(path).Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }
    }
}
