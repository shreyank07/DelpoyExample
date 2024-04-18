using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyFramework.Helpers
{
    public static class CUtilities
    {
        public static string GetUSBBytes(byte[] bytes)
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


        /// <summary>
        /// Converts hex string to ascii
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static string HexString2Ascii(string hexString)
        {
            int hexval = Convert.ToInt16(hexString, 16);
            char c = (char)hexval;
            string strascii = ".";
            if (!char.IsWhiteSpace(c) && !char.IsControl(c))
                strascii = c.ToString();
            return strascii;
        }

        public static string FormatString(byte[] data)
        {
            int linecount = 0;
            string retval = "";
            try
            {
                if (data.Length > 0)
                {
                    string[] dataascii = new string[2];
                    int j = 1;
                    for (int z = 0; z < data.Length; z++)
                    {
                        dataascii[0] = dataascii[0] + data[z].ToString("X2") + " ";
                       // dataascii[1] = dataascii[1] + HexString2Ascii(data[z].ToString("X2")) + " ";

                        if ((j % 16) == 0)
                        {
                            dataascii[0] += Environment.NewLine;
                            dataascii[1] = "";
                        }
                        j += 1;
                    }
                    string[] lines = dataascii[0].Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string line in lines)
                    {
                        retval += "[ " +linecount.ToString("X4") + " ]" + "  " + line + Environment.NewLine;
                        linecount += 16;
                    }
                }
            }
            catch (Exception ee) { throw ee; }
            return retval;
        }

        /// <summary>
        /// Converts the format.
        /// </summary>
        /// <param name="fmt">The FMT.</param>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        public static string ConvertFormat(NumberFormat fmt, int val)
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
                    retVal = "b" + Convert.ToString(val, 2);
                }
                else if (fmt == NumberFormat.OCTAL)
                {
                    retVal = "O" + Convert.ToString(val, 8);
                }
                else if (fmt == NumberFormat.ASCII)
                {
                    retVal = "'" + Convert.ToChar(val) + "'";
                }
            }
            catch (Exception ex)
            {
                //AppManager.WriteToLog("Utilities : ConvertFormat() - ", ex);
            }

            return retVal;
        }

        /// <summary>
        /// Converts to base units.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="inUnits">The in units.</param>
        /// <returns></returns>
        public static double ConvertToBaseUnits(double value, Units inUnits)
        {
            if (double.IsNaN(value) == true)
            {
                return 0.0;
            }
            double retVal = 0.0;
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

                default:
                    retVal = value;
                    break;

            }
            return retVal;
        }

        /// <summary>
        /// Formats the volt.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="inUnits">The in units.</param>
        /// <returns></returns>
        public static string FormatVolt(double value, Units inUnits)
        {
            if (double.IsNaN(value) == true)
            {
                return "NA";
            }
            bool isNegative = false;
            if (value < 0)
            {
                isNegative = true;
                value = value * -1;
            }

            string retVal = FormatVoltNumber(value, inUnits);

            if (isNegative == true)
            {
                retVal = "-" + retVal + "V";
            }
            else
            {
                retVal = retVal + "V";
            }

            return retVal;
        }

        /// <summary>
        /// Formats the time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="inUnits">The in units.</param>
        /// <returns></returns>
        public static string FormatTime(double value, Units inUnits)
        {
            if (double.IsNaN(value) == true)
            {
                return "NA";
            }
            bool isNegative = false;
            if (value < 0)
            {
                isNegative = true;
                value = value * -1;
            }
            string retVal = FormatNumber(value, inUnits);
            if (isNegative == true)
            {

                retVal = "-" + retVal + "S";
            }
            else
            {
                retVal = retVal + "S";
            }
            return retVal;
        }

        /// <summary>
        /// Formats the number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="inUnits">The in units.</param>
        /// <returns></returns>
        public static string FormatNumber(double value, Units inUnits)
        {
            try
            {

                string retVal = "";
                double baseUnitValue = value;
                if (!(inUnits == Units.BASE_UNIT || inUnits == Units.SECONDS))
                {
                    baseUnitValue = ConvertToBaseUnits(value, inUnits);
                }

                retVal = baseUnitValue.ToString("#.0000E+0");
                if (retVal.Length <= 8)
                {
                    //error
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
                                    retVal = roundOffValue.ToString("0.0000");
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
                                    retVal = roundOffValue.ToString("00.000");
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
                                    retVal = roundOffValue.ToString("000.00");
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
                                    retVal = roundOffValue.ToString("0.0000");
                                }
                            }
                            retVal = retVal + " K";
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
                                    retVal = roundOffValue.ToString("00.000");
                                }
                            }
                            retVal = retVal + " K";
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
                                    retVal = roundOffValue.ToString("000.00");
                                }
                            }
                            retVal = retVal + " K";
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
                                    retVal = roundOffValue.ToString("0.0000");
                                }
                            }
                            retVal = retVal + " M";
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
                                    retVal = roundOffValue.ToString("00.000");
                                }
                            }
                            retVal = retVal + " M";
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
                                    retVal = roundOffValue.ToString("000.00");
                                }
                            }
                            retVal = retVal + " M";
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
                                    retVal = roundOffValue.ToString("0.0000");
                                }
                            }
                            retVal = retVal + " G";
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
                                    retVal = roundOffValue.ToString("00.000");
                                }
                            }
                            retVal = retVal + " G";
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
                                    retVal = roundOffValue.ToString("000.00");
                                }
                            }
                            retVal = retVal + " G";
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
                        case 0:

                            strBuilder.Remove(6, retVal.Length - 6);
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
                                    retVal = roundOffValue.ToString("000.00");
                                }
                            }
                            retVal = retVal + " m";

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
                                    retVal = roundOffValue.ToString("00.000");
                                }
                            }
                            retVal = retVal + " m";
                            //strBuilder.Append(" m");
                            //retVal = strBuilder.ToString();

                            break;
                        case 3:
                            // 1.2345E-3  = 1.2345 E-3
                            strBuilder.Remove(6, retVal.Length - 6);
                            // strBuilder.Append(" m");
                            retVal = strBuilder.ToString();

                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue / 1000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.0001;
                                    retVal = roundOffValue.ToString("0.0000");
                                }
                            }
                            retVal = retVal + " m";

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
                                    retVal = roundOffValue.ToString("000.00");
                                }
                            }
                            retVal = retVal + " u";

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
                                    retVal = roundOffValue.ToString("00.000");
                                }
                            }
                            retVal = retVal + " u";

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
                                    retVal = roundOffValue.ToString("0.0000");
                                }
                            }
                            retVal = retVal + " u";

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
                                    retVal = roundOffValue.ToString("000.00");
                                }
                            }
                            retVal = retVal + " n";

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
                                    retVal = roundOffValue.ToString("00.000");
                                }
                            }
                            retVal = retVal + " n";


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
                                    retVal = roundOffValue.ToString("0.0000");
                                }
                            }
                            retVal = retVal + " n";


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
                                    retVal = roundOffValue.ToString("000.00");
                                }
                            }
                            retVal = retVal + " p";


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
                                    retVal = roundOffValue.ToString("00.000");
                                }
                            }
                            retVal = retVal + " p";

                            //strBuilder.Append(" p");
                            //retVal = strBuilder.ToString();
                            break;

                        case 12:
                            // 1.2345E-12 = 1.2345 E-12
                            strBuilder.Remove(6, retVal.Length - 6);
                            strBuilder.Append(" p");
                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {

                                if ((roundOffValue / 1000000000000) < value)
                                {
                                    roundOffValue = roundOffValue + 0.0001;
                                    retVal = roundOffValue.ToString("0.0000");
                                }
                            }
                            retVal = retVal + " p";

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
                                    retVal = roundOffValue.ToString("000.00");
                                }
                            }
                            retVal = retVal + " f";

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
                                    retVal = roundOffValue.ToString("00.000");
                                }
                            }
                            retVal = retVal + " f";
                            //strBuilder.Append(" f");
                            //retVal = strBuilder.ToString();
                            break;

                        case 15:

                            // 1.2345E-15 = 12.345 E-15
                            strBuilder.Remove(6, retVal.Length - 6);
                            strBuilder.Append(" f");
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

        public static string FormatNumber(string t3, double value)
        {
            try
            {   
                Units inUnits=Units.BASE_UNIT;
                bool isNegative = false;
                if (value < 0)
                {
                    isNegative = true;
                    value = Math.Abs(value);
                }
                string retVal = "";
                double baseUnitValue = value;
                if (!(inUnits == Units.BASE_UNIT || inUnits == Units.SECONDS))
                {
                    baseUnitValue = ConvertToBaseUnits(value, inUnits);
                }

                retVal = baseUnitValue.ToString("#.0000000E+0");
                if (retVal.Length <= 8)
                {
                    //error
                }

                //string pow = retVal.Substring(8);  
                int power;
                int.TryParse(retVal.Substring(11), out power);

                StringBuilder strBuilder = new StringBuilder(retVal);
                double roundOffValue = value;

                if (retVal[10] == '+')
                {
                    switch (power)
                    {
                        case 0:
                            //X.XXXX
                            strBuilder.Remove(9, retVal.Length - 9);
                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue) < value)
                                {
                                    roundOffValue = roundOffValue + 0.0001;
                                    retVal = roundOffValue.ToString("0.0000000");
                                }
                            }
                            retVal = (retVal.Length < 10) ? retVal.PadRight(10, '0')+"s" : retVal+"s";
                            break;
                        case 1:
                            // XX.XXX
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(2, '.');
                            strBuilder.Remove(9, retVal.Length - 9);
                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue) < value)
                                {
                                    roundOffValue = roundOffValue + 0.001;
                                    retVal = roundOffValue.ToString("00.0000000");
                                }
                            }
                            retVal = (retVal.Length < 10) ? retVal.PadRight(10, '0')+"s" : retVal+"s";
                            break;
                        case 2:
                            //XXX.XX
                            strBuilder.Remove(1, 1);
                            strBuilder.Insert(3, '.');
                            strBuilder.Remove(9, retVal.Length - 9);
                            retVal = strBuilder.ToString();
                            if (double.TryParse(retVal, out roundOffValue))
                            {
                                if ((roundOffValue) < value)
                                {
                                    roundOffValue = roundOffValue + 0.01;
                                    retVal = roundOffValue.ToString("000.0000000");
                                }
                            }
                            retVal = (retVal.Length < 10) ? retVal.PadRight(10, '0')+"s" : retVal+"s";
                            break;
                        case 3:
                            retVal = strBuilder.ToString();
                            retVal = roundOffValue.ToString()+"s";

                            break;
                        case 4:
                            //XX.XXX
                            //strBuilder.Remove(1, 1);
                            //strBuilder.Insert(2, '.');
                            //strBuilder.Remove(6, retVal.Length - 6);

                            retVal = strBuilder.ToString();
                            retVal = (Math.Round(Convert.ToDouble(retVal) * 10 * 1E-4, 0)).ToString();
                            //if (double.TryParse(retVal, out roundOffValue))
                            //{
                            //    if ((roundOffValue * 1000) < value)
                            //    {
                            //        roundOffValue = roundOffValue + 0.001;
                            //retVal = roundOffValue.ToString();
                            //    }
                            //}
                            retVal = retVal + "ks";
                            //retVal = strBuilder.ToString();
                            break;
                        case 5:
                            //XXX.XX
                            //strBuilder.Remove(1, 1);
                            //strBuilder.Insert(3, '.');
                            //strBuilder.Remove(6, retVal.Length - 6);

                            retVal = strBuilder.ToString();
                            retVal = (Math.Round(Convert.ToDouble(retVal) * 100 * 1E-5, 0)).ToString();
                            //if (double.TryParse(retVal, out roundOffValue))
                            //{
                            //    if ((roundOffValue * 1000) < value)
                            //    {
                            //  roundOffValue = roundOffValue + 0.01;
                            //retVal = roundOffValue.ToString();
                            //    }
                            //}
                            retVal = retVal + "ks";
                            //strBuilder.Append(" K");
                            //retVal = strBuilder.ToString();
                            break;
                        case 6:
                            //X.XXXX
                            //  strBuilder.Remove(6, retVal.Length - 6);

                            retVal = strBuilder.ToString();
                            retVal = (Math.Round(Convert.ToDouble(retVal) * 1E-6, 0)).ToString();
                            //if (double.TryParse(retVal, out roundOffValue))
                            //{
                            //    if ((roundOffValue * 1E6) < value)
                            //    {
                            // roundOffValue = roundOffValue + 0.0001;
                            //retVal = roundOffValue.ToString();
                            //    }
                            //}
                            retVal = retVal + "Ms";
                            //strBuilder.Append(" M");
                            //retVal = strBuilder.ToString();
                            break;
                        case 7:
                            //XX.XXX
                            //strBuilder.Remove(1, 1);
                            //strBuilder.Insert(2, '.');
                            //strBuilder.Remove(6, retVal.Length - 6);
                            //retVal = strBuilder.ToString();
                            //if (double.TryParse(retVal, out roundOffValue))
                            //{
                            //    if ((roundOffValue * 1E6) < value)
                            //    {
                            //        roundOffValue = roundOffValue + 0.001;
                            //        retVal = roundOffValue.ToString("00.000");
                            //    }
                            //}
                            //retVal = retVal + "M";
                            //strBuilder.Append(" M");
                            //retVal = strBuilder.ToString();
                            retVal = strBuilder.ToString();
                            retVal = (Math.Round(Convert.ToDouble(retVal) * 10 * 1E-7, 0)).ToString();
                            retVal = retVal + "Ms";
                            break;
                        case 8:
                            //XXX.XX
                            //strBuilder.Remove(1, 1);
                            //strBuilder.Insert(3, '.');
                            //strBuilder.Remove(6, retVal.Length - 6);
                            //retVal = strBuilder.ToString();
                            //if (double.TryParse(retVal, out roundOffValue))
                            //{
                            //    if ((roundOffValue * 1E6) < value)
                            //    {
                            //        roundOffValue = roundOffValue + 0.01;
                            //        retVal = roundOffValue.ToString("000.00");
                            //    }
                            //}
                            //retVal = retVal + "M";
                            //strBuilder.Append(" M");
                            //retVal = strBuilder.ToString();
                            retVal = strBuilder.ToString();
                            retVal = (Math.Round(Convert.ToDouble(retVal) * 100 * 1E-8, 0)).ToString();
                            retVal = retVal + "Ms";
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
                                    retVal = roundOffValue.ToString("0.0000");
                                }
                            }
                            retVal = retVal + "Gs";
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
                                    retVal = roundOffValue.ToString("00.000");
                                }
                            }
                            retVal = retVal + "Gs";
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
                                    retVal = roundOffValue.ToString("000.00");
                                }
                            }
                            retVal = retVal + "Gs";
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
                        case 0:

                            strBuilder.Remove(6, retVal.Length - 6);
                            retVal = strBuilder.ToString()+"s";

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
                                    retVal = roundOffValue.ToString("000.00");
                                }
                            }
                            retVal = retVal + "ms";

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
                                    retVal = roundOffValue.ToString("00.000");
                                }
                            }
                            retVal = retVal + "ms";
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
                                    retVal = roundOffValue.ToString("0.0000");
                                }
                            }
                            retVal = retVal + "ms";

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
                                    retVal = roundOffValue.ToString("000.00");
                                }
                            }
                            retVal = retVal + "µs";

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
                                    retVal = roundOffValue.ToString("00.000");
                                }
                            }
                            retVal = retVal + "µs";

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
                                    retVal = roundOffValue.ToString("0.0000");
                                }
                            }
                            retVal = retVal + "µs";

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
                                    retVal = roundOffValue.ToString("000.00");
                                }
                            }
                            retVal = retVal + "ns";

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
                                    retVal = roundOffValue.ToString("00.000");
                                }
                            }
                            retVal = retVal + "ns";


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
                                    retVal = roundOffValue.ToString("0.0000");
                                }
                            }
                            retVal = retVal + "ns";


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
                                    retVal = roundOffValue.ToString("000.00");
                                }
                            }
                            retVal = retVal + "ps";


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
                                    retVal = roundOffValue.ToString("00.000");
                                }
                            }
                            retVal = retVal + "ps";

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
                                    retVal = roundOffValue.ToString("0.0000");
                                }
                            }
                            retVal = retVal + "ps";

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
                                    retVal = roundOffValue.ToString("000.00");
                                }
                            }
                            retVal = retVal + "fs";

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
                                    retVal = roundOffValue.ToString("00.000");
                                }
                            }
                            retVal = retVal + "fs";
                            //strBuilder.Append(" f");
                            //retVal = strBuilder.ToString();
                            break;

                        case 15:

                            // 1.2345E-15 = 12.345 E-15
                            strBuilder.Remove(6, retVal.Length - 6);
                            strBuilder.Append("fs");
                            retVal = strBuilder.ToString();
                            break;

                        default:

                            break;
                    }

                }

                if (isNegative)
                    retVal = "-" + retVal;
                return retVal;
            }
            catch
            {
                return "0";
            }
        }

        /// <summary>
        /// Formats the volt number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="inUnits">The in units.</param>
        /// <returns></returns>
        public static string FormatVoltNumber(double value, Units inUnits)
        {
            string retVal = "";
            double baseUnitValue = value;
            if (!(inUnits == Units.BASE_UNIT || inUnits == Units.SECONDS))
            {
                baseUnitValue = ConvertToBaseUnits(value, inUnits);
            }

            retVal = baseUnitValue.ToString("#.00E+0");
            if (retVal.Length <= 6)
            {
                //error
            }

            //string pow = retVal.Substring(8);  
            int power;
            int.TryParse(retVal.Substring(6), out power);

            StringBuilder strBuilder = new StringBuilder(retVal);

            if (retVal[5] == '+')
            {
                switch (power)
                {
                    case 0:

                        strBuilder.Remove(4, retVal.Length - 4);
                        retVal = strBuilder.ToString();

                        break;
                    case 1:
                        strBuilder.Remove(1, 1);
                        strBuilder.Insert(2, '.');
                        strBuilder.Remove(4, retVal.Length - 4);
                        retVal = strBuilder.ToString();


                        break;
                    case 2:
                        strBuilder.Remove(1, 1);
                        strBuilder.Insert(3, '.');
                        strBuilder.Remove(4, retVal.Length - 4);
                        retVal = strBuilder.ToString();

                        break;
                    case 3:
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" K");
                        retVal = strBuilder.ToString();

                        break;
                    case 4:
                        strBuilder.Remove(1, 1);
                        strBuilder.Insert(2, '.');
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" K");
                        retVal = strBuilder.ToString();
                        break;
                    case 5:
                        strBuilder.Remove(1, 1);
                        strBuilder.Insert(3, '.');
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" K");
                        retVal = strBuilder.ToString();
                        break;
                    case 6:
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" M");
                        retVal = strBuilder.ToString();
                        break;
                    case 7:
                        strBuilder.Remove(1, 1);
                        strBuilder.Insert(2, '.');
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" M");
                        retVal = strBuilder.ToString();
                        break;
                    case 8:
                        strBuilder.Remove(1, 1);
                        strBuilder.Insert(3, '.');
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" M");
                        retVal = strBuilder.ToString();
                        break;
                    case 9:
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" G");
                        retVal = strBuilder.ToString();
                        break;
                    case 10:
                        strBuilder.Remove(1, 1);
                        strBuilder.Insert(2, '.');
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" G");
                        retVal = strBuilder.ToString();
                        break;
                    case 11:
                        strBuilder.Remove(1, 1);
                        strBuilder.Insert(3, '.');
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" G");
                        retVal = strBuilder.ToString();
                        break;
                    default:

                        break;
                }

            }
            else
            {



                switch (power)
                {
                    case 0:

                        strBuilder.Remove(4, retVal.Length - 4);
                        retVal = strBuilder.ToString();

                        break;
                    case 1:
                        // 1.2345E-1  = 123.45 E-3 
                        strBuilder.Remove(1, 1);
                        strBuilder.Insert(3, '.');
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" m");
                        retVal = strBuilder.ToString();
                        break;
                    case 2:
                        // 1.2345E-2  = 12.345 E-3
                        strBuilder.Remove(1, 1);
                        strBuilder.Insert(2, '.');
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" m");
                        retVal = strBuilder.ToString();

                        break;
                    case 3:
                        // 1.2345E-3  = 1.2345 E-3
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" m");
                        retVal = strBuilder.ToString();

                        break;
                    case 4:
                        // 1.2345E-4  = 123.45 E-6
                        strBuilder.Remove(1, 1);
                        strBuilder.Insert(3, '.');
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" µ");
                        retVal = strBuilder.ToString();
                        break;
                    case 5:
                        // 1.2345E-5  = 12.345 E-6
                        strBuilder.Remove(1, 1);
                        strBuilder.Insert(2, '.');
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" µ");
                        retVal = strBuilder.ToString();
                        break;
                    case 6:

                        // 1.2345E-6  = 1.2345 E-6
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" µ");
                        retVal = strBuilder.ToString();
                        break;
                    case 7:
                        // 1.2345E-7  = 123.45 nS
                        strBuilder.Remove(1, 1);
                        strBuilder.Insert(3, '.');
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" n");
                        retVal = strBuilder.ToString();
                        break;
                    case 8:
                        // 1.2345E-8  = 12.345 E nS
                        strBuilder.Remove(1, 1);
                        strBuilder.Insert(2, '.');
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" n");
                        retVal = strBuilder.ToString();
                        break;
                    case 9:
                        // 1.2345E-9  = 1.2345 E-9
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" n");
                        retVal = strBuilder.ToString();
                        break;
                    case 10:
                        // 1.2345E-10 = 123.45 E-12
                        strBuilder.Remove(1, 1);
                        strBuilder.Insert(3, '.');
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" p");
                        retVal = strBuilder.ToString();
                        break;
                    case 11:
                        // 1.2345E-11 = 12.345 E-12
                        strBuilder.Remove(1, 1);
                        strBuilder.Insert(2, '.');
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" p");
                        retVal = strBuilder.ToString();
                        break;

                    case 12:
                        // 1.2345E-12 = 1.2345 E-12
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" p");
                        retVal = strBuilder.ToString();
                        break;
                    case 13:

                        // 1.2345E-13 = 123.45 E-15
                        strBuilder.Remove(1, 1);
                        strBuilder.Insert(3, '.');
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" f");
                        retVal = strBuilder.ToString();
                        break;

                    case 14:

                        // 1.2345E-14 = 12.345 E-15
                        strBuilder.Remove(1, 1);
                        strBuilder.Insert(2, '.');
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" f");
                        retVal = strBuilder.ToString();
                        break;

                    case 15:

                        // 1.2345E-15 = 12.345 E-15
                        strBuilder.Remove(4, retVal.Length - 4);
                        strBuilder.Append(" f");
                        retVal = strBuilder.ToString();
                        break;

                    default:
                        break;
                }

            }
            return retVal;
        }

        /// <summary>
        /// Convert a number to engineering notation
        /// </summary>
        /// <param name="d">Number to converted to string value of engineering notation</param>
        /// <param name="decimalCount">Number of decimals digits to round to</param>
        /// <returns>the string with that can be displayed to user</returns>
        public static string ToEngineeringNotation(this double d, int decimalCount = 3)
        {
            double exponent = Math.Log10(Math.Abs(d));
            string decimalFormat = "N6";
            if (decimalCount == 5)
                decimalFormat = "N5";
            else if (decimalCount == 4)
                decimalFormat = "N4";
            else if (decimalCount == 3)
                decimalFormat = "N3";
            else if (decimalCount == 2)
                decimalFormat = "N2";
            if (Math.Abs(d) >= 1)
            {
                switch ((int)Math.Floor(exponent))
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        return d.ToString(decimalFormat);
                    case 5:
                        return (d / 1e3).ToString(decimalFormat) + "k";
                    case 6:
                    case 7:
                    case 8:
                        return (d / 1e6).ToString(decimalFormat) + "M";
                    case 9:
                    case 10:
                    case 11:
                        return (d / 1e9).ToString(decimalFormat) + "G";
                    case 12:
                    case 13:
                    case 14:
                        return (d / 1e12).ToString(decimalFormat) + "T";
                    case 15:
                    case 16:
                    case 17:
                        return (d / 1e15).ToString(decimalFormat) + "P";
                    case 18:
                    case 19:
                    case 20:
                        return (d / 1e18).ToString(decimalFormat) + "E";
                    case 21:
                    case 22:
                    case 23:
                        return (d / 1e21).ToString(decimalFormat) + "Z";
                    case 24:
                    case 25:
                    case 26:
                        return (d / 1e24).ToString(decimalFormat) + "Y";
                    default:
                        return d.ToString(decimalFormat);
                }
            }
            else if (Math.Abs(d) > 0)
            {
                switch ((int)Math.Floor(exponent))
                {
                    case -1:
                    case -2:
                    case -3:
                        return (d * 1e3).ToString(decimalFormat) + "m";
                    case -4:
                    case -5:
                    case -6:
                        return (d * 1e6).ToString(decimalFormat) + "u";
                    case -7:
                    case -8:
                    case -9:
                        return (d * 1e9).ToString(decimalFormat) + "n";
                    case -10:
                    case -11:
                    case -12:
                        return (d * 1e12).ToString(decimalFormat) + "p";
                    case -13:
                    case -14:
                    case -15:
                        return (d * 1e15).ToString(decimalFormat) + "f";
                    case -16:
                    case -17:
                    case -18:
                        return (d * 1e18).ToString(decimalFormat) + "a";
                    case -19:
                    case -20:
                    case -21:
                        return (d * 1e21).ToString(decimalFormat) + "z";
                    case -22:
                    case -23:
                    case -24:
                        return (d * 1e24).ToString(decimalFormat) + "y";
                    default:
                        return d.ToString(decimalFormat);
                }
            }
            else
            {
                return "0";
            }
        }


        public static string ToPlotTimestampNotation(this double d)
        {
            double exponent = Math.Log10(Math.Abs(d));
            if (Math.Abs(d) >= 1)
            {
                switch ((int)Math.Floor(exponent))
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        return d.ToString("N6") + "s";
                    case 5:
                        return (d / 1e3).ToString() + "ks";
                    case 6:
                    case 7:
                    case 8:
                        return (d / 1e6).ToString() + "Ms";
                    case 9:
                    case 10:
                    case 11:
                        return (d / 1e9).ToString() + "Gs";
                    case 12:
                    case 13:
                    case 14:
                        return (d / 1e12).ToString() + "Ts";
                    case 15:
                    case 16:
                    case 17:
                        return (d / 1e15).ToString() + "Ps";
                    case 18:
                    case 19:
                    case 20:
                        return (d / 1e18).ToString() + "Es";
                    case 21:
                    case 22:
                    case 23:
                        return (d / 1e21).ToString() + "Zs";
                    default:
                        return (d / 1e24).ToString() + "Ys";
                }
            }
            else if (Math.Abs(d) > 0)
            {
                switch ((int)Math.Floor(exponent))
                {
                    case -1:
                    case -2:
                    case -3:
                        return (d * 1e3).ToString("N3") + "ms";
                    case -4:
                    case -5:
                    case -6:
                        return (d * 1e6).ToString("N3") + "us";
                    case -7:
                    case -8:
                    case -9:
                        return (d * 1e9).ToString("N3") + "ns";
                    case -10:
                    case -11:
                    case -12:
                        return (d * 1e12).ToString("N3") + "ps";
                    case -13:
                    case -14:
                    case -15:
                        return (d * 1e15).ToString("N3") + "fs";
                    case -16:
                    case -17:
                    case -18:
                        return (d * 1e15).ToString() + "as";
                    case -19:
                    case -20:
                    case -21:
                        return (d * 1e15).ToString() + "zs";
                    default:
                        return (d * 1e15).ToString() + "ys";
                }
            }
            else
            {
                return "0";
            }
        }

    }
    public enum NumberFormat
    {
        HEX = 0,
        BINARY = 1,
        DECIMAL = 2,
        OCTAL = 3,
        ASCII = 4,
        SIGNED = 5
    }
    public enum Units { GIGA, MEGA, KILO, BASE_UNIT, SECONDS, VOLTS, MILLI, MICRO, NANO, PICO, FEMTO, PERCENT };
}
