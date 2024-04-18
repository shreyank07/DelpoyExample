using PGYMiniCooper.CoreModule.ViewModel;
using PGYMiniCooper.CoreModule.ViewModel.ProtocolViewModel;
using PGYMiniCooper.DataModule.Model;
using PGYMiniCooper.DataModule.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PGYMiniCooper.CoreModule.View.Converters
{
    public class TriggerPatternValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            try
            {

               if (Wrapper.Trigger.TriggerModel.TriggerType == DataModule.eTriggerTypeList.Pattern)
                {
                    //if (value == null || value.ToString() == string.Empty)
                    //    return new ValidationResult(false, "value cannot be empty.");
                    if (Wrapper.Trigger.TriggerModel.PatternFormat == DataModule.ePatternFormat.Hex)
                    {
                        Regex r = new Regex(@"^[A-Fa-f0-9]*$");
                        var txtpattern = value.ToString().ToLower();
                        if (txtpattern.Contains("0x"))
                            txtpattern = txtpattern.Replace("0x", "");
                        int maxLength = -1;
                        int.TryParse(txtpattern, System.Globalization.NumberStyles.HexNumber, null, out maxLength);
                        if (!r.IsMatch(txtpattern))
                            return new ValidationResult
                           (false, "Input string format is not correct");
                        else if (maxLength > 0xFFFF)
                            return new ValidationResult
                            (false, "Maximum value can not be greater than 0xFFFF");
                    }
                    else if (Wrapper.Trigger.TriggerModel.PatternFormat == DataModule.ePatternFormat.Binary)
                    {
                        var regex = new Regex(@"^[0-1]*$");
                        var txtpattern = value.ToString().ToLower();
                        txtpattern = txtpattern.Replace("x", "0");
                        if (!regex.IsMatch(txtpattern))
                            return new ValidationResult
                           (false, "Input string format is not correct");
                    }
                    else if (Wrapper.Trigger.TriggerModel.PatternFormat == DataModule.ePatternFormat.Decimal)
                    {
                        var regex = new Regex(@"^[0-9]\d*$");
                        var txtpattern = value.ToString().ToLower();
                        if (!regex.IsMatch(txtpattern))
                            return new ValidationResult
                           (false, "Input string format is not correct");
                        else if (Convert.ToInt32(value.ToString()) > 0xFFFF)
                            return new ValidationResult
                            (false, "Maximum value can not be greater than 65535");
                    }
                    else if (Wrapper.Trigger.TriggerModel.PatternFormat == DataModule.ePatternFormat.Octal)
                    {
                        var regex = new Regex(@"^[0-9]\d*$");
                        var txtpattern = value.ToString().ToLower();

                        if (!regex.IsMatch(txtpattern))
                            return new ValidationResult
                           (false, "Input string format is not correct");
                        else if (Convert.ToInt32(value.ToString()) > 177777)
                            return new ValidationResult
                            (false, "Maximum value can not be greater than 177777");
                    }
                }
                else if (Wrapper.Trigger.TriggerModel.TriggerType == DataModule.eTriggerTypeList.Protocol)
                {
                    if (Wrapper.Trigger.TriggerModel.ProtocolTypeIndex == DataModule.eProtocolTypeList.I2C)
                    {
                        //if (value == null || value.ToString() == string.Empty)
                        //    return new ValidationResult(false, "value cannot be empty.");
                        if (Wrapper.Trigger.TriggerModel.I2CTriggerAtSelected == DataModule.eI2CTriggerAtList.Data || (Wrapper.Trigger.TriggerModel.I2CTriggerAtSelected == DataModule.eI2CTriggerAtList.Address_Data && Wrapper.Format == "data"))
                        {
                            if (Wrapper.Trigger.TriggerModel.DataPattern == DataModule.ePatternFormat.Hex)
                            {
                                var regex = new Regex(@"^[A-Fa-f0-9]*$");

                                var txtpattern = value.ToString().ToLower();
                                if (txtpattern.Contains("-"))
                                    regex = new Regex(@"^[A-Fa-f0-9]+(-[A-Fa-f0-9]+)+$");
                                if (txtpattern.Contains("0x"))
                                    txtpattern = txtpattern.Replace("0x", "");
                                int maxLength = 0xFF;

                                if (!regex.IsMatch(txtpattern))
                                    return new ValidationResult
                                   (false, "Input string format is not correct");

                                var data = txtpattern.Trim().Split('-');
                                foreach (var item in data)
                                {
                                    int.TryParse(item, System.Globalization.NumberStyles.HexNumber, null, out maxLength);
                                    if (maxLength > 0xFF)
                                        return new ValidationResult
                                        (false, "Maximum value can not be greater than 0xFF");
                                }
                            }
                            else if (Wrapper.Trigger.TriggerModel.DataPattern == DataModule.ePatternFormat.Binary)
                            {
                                var regex = new Regex(@"^[01]+$");
                                var txtpattern = value.ToString().ToLower();
                                if (txtpattern.Contains("-"))
                                    regex = new Regex(@"^[01]+(-[01]+)+$");

                                if (!regex.IsMatch(txtpattern))
                                    return new ValidationResult
                                   (false, "Input string format is not correct");

                                var data = txtpattern.Trim().Split('-');
                                foreach (var item in data)
                                {
                                    if (item.Length != 8)
                                        return new ValidationResult
                                        (false, "Length should be 8 characters");
                                }

                            }
                            else if (Wrapper.Trigger.TriggerModel.DataPattern == DataModule.ePatternFormat.Decimal)
                            {
                                var regex = new Regex(@"^[0-9]\d*$");
                                var txtpattern = value.ToString().ToLower();
                                if (txtpattern.Contains("-"))
                                    regex = new Regex(@"^[0-9]+(-[0-9]+)+$");

                                if (!regex.IsMatch(txtpattern))
                                    return new ValidationResult
                                   (false, "Input string format is not correct");

                                var data = txtpattern.Trim().Split('-');
                                foreach (var item in data)
                                {
                                    if (Convert.ToInt32(item.ToString()) > 255)
                                        return new ValidationResult
                                        (false, "Maximum value can not be greater than 255");
                                }
                            }
                            else if (Wrapper.Trigger.TriggerModel.DataPattern == DataModule.ePatternFormat.Octal)
                            {
                                var regex = new Regex(@"^[0-9]\d*$");
                                var txtpattern = value.ToString().ToLower();
                                if (txtpattern.Contains("-"))
                                    regex = new Regex(@"^[0-9]+(-[0-9]+)+$");
                                if (!regex.IsMatch(txtpattern))
                                    return new ValidationResult
                                   (false, "Input string format is not correct");

                                var data = txtpattern.Trim().Split('-');
                                foreach (var item in data)
                                {
                                    if (Convert.ToInt32(item.ToString()) > 377)
                                        return new ValidationResult
                                        (false, "Maximum value can not be greater than 377");
                                }
                            }
                        }
                        else if (Wrapper.Trigger.TriggerModel.I2CTriggerAtSelected == DataModule.eI2CTriggerAtList.Address || (Wrapper.Trigger.TriggerModel.I2CTriggerAtSelected == DataModule.eI2CTriggerAtList.Address_Data && Wrapper.Format == "address"))
                        {
                            if (Wrapper.Trigger.TriggerModel.AddressPattern == DataModule.ePatternFormat.Hex)
                            {
                                Regex r = new Regex(@"^[A-Fa-f0-9]*$");
                                var txtpattern = value.ToString().ToLower();
                                if (txtpattern.Contains("0x"))
                                    txtpattern = txtpattern.Replace("0x", "");
                                int maxLength = 0xFF;
                                int.TryParse(txtpattern, System.Globalization.NumberStyles.HexNumber, null, out maxLength);

                                if (!r.IsMatch(txtpattern))
                                    return new ValidationResult
                                   (false, "Input string format is not correct");
                                else if (maxLength > 0xFF)
                                    return new ValidationResult
                                    (false, "Maximum value can not be greater than 0xFF");
                            }
                            else if (Wrapper.Trigger.TriggerModel.AddressPattern == DataModule.ePatternFormat.Binary)
                            {
                                var regex = new Regex(@"[^01]+$");
                                var txtpattern = value.ToString().ToLower();
                                if (regex.IsMatch(txtpattern))
                                    return new ValidationResult
                                   (false, "Input string format is not correct");
                            }
                            else if (Wrapper.Trigger.TriggerModel.AddressPattern == DataModule.ePatternFormat.Decimal)
                            {
                                var regex = new Regex(@"^[0-9]\d*$");
                                var txtpattern = value.ToString().ToLower();
                                if (!regex.IsMatch(txtpattern))
                                    return new ValidationResult
                                   (false, "Input string format is not correct");
                                else if (Convert.ToInt32(value.ToString()) > 255)
                                    return new ValidationResult
                                    (false, "Maximum value can not be greater than 255");
                            }
                            else if (Wrapper.Trigger.TriggerModel.AddressPattern == DataModule.ePatternFormat.Octal)
                            {
                                var regex = new Regex(@"^[0-9]\d*$");
                                var txtpattern = value.ToString().ToLower();
                                if (!regex.IsMatch(txtpattern))
                                    return new ValidationResult
                                   (false, "Input string format is not correct");
                                else if (Convert.ToInt32(value.ToString()) > 377)
                                    return new ValidationResult
                                    (false, "Maximum value can not be greater than 377");
                            }
                        }
                    }
                    else if (Wrapper.Trigger.TriggerModel.ProtocolTypeIndex == DataModule.eProtocolTypeList.SPI)
                    {
                        //if (value == null || value.ToString() == string.Empty)
                        //    return new ValidationResult(false, "value cannot be empty.");
                        if (Wrapper.Trigger.TriggerModel.IsMOSIChecked)
                        {
                            if (Wrapper.Trigger.TriggerModel.MOSIPattern == DataModule.ePatternFormat.Hex)
                            {
                                var regex = new Regex(@"^[A-Fa-f0-9]*$");

                                var txtpattern = value.ToString().ToLower();
                                if (txtpattern.Contains("-"))
                                    regex = new Regex(@"^[A-Fa-f0-9]+(-[A-Fa-f0-9]+)+$");
                                if (txtpattern.Contains("0x"))
                                    txtpattern = txtpattern.Replace("0x", "");
                                int maxLength = 0xFF;

                                if (!regex.IsMatch(txtpattern))
                                    return new ValidationResult
                                   (false, "Input string format is not correct");

                                var data = txtpattern.Trim().Split('-');
                                foreach (var item in data)
                                {
                                    int.TryParse(item, System.Globalization.NumberStyles.HexNumber, null, out maxLength);
                                    if (maxLength > 0xFF)
                                        return new ValidationResult
                                        (false, "Maximum value can not be greater than 0xFF");
                                }
                            }
                            else if (Wrapper.Trigger.TriggerModel.MOSIPattern == DataModule.ePatternFormat.Binary)
                            {
                                var regex = new Regex(@"^[01]+$");
                                var txtpattern = value.ToString().ToLower();
                                if (txtpattern.Contains("-"))
                                    regex = new Regex(@"^[01]+(-[01]+)+$");

                                if (!regex.IsMatch(txtpattern))
                                    return new ValidationResult
                                   (false, "Input string format is not correct");

                                var data = txtpattern.Trim().Split('-');
                                foreach (var item in data)
                                {
                                    if (item.Length != 8)
                                        return new ValidationResult
                                        (false, "Length should be 8 characters");
                                }

                            }
                            else if (Wrapper.Trigger.TriggerModel.MOSIPattern == DataModule.ePatternFormat.Decimal)
                            {
                                var regex = new Regex(@"^[0-9]\d*$");
                                var txtpattern = value.ToString().ToLower();
                                if (txtpattern.Contains("-"))
                                    regex = new Regex(@"^[0-9]+(-[0-9]+)+$");

                                if (!regex.IsMatch(txtpattern))
                                    return new ValidationResult
                                   (false, "Input string format is not correct");

                                var data = txtpattern.Trim().Split('-');
                                foreach (var item in data)
                                {
                                    if (Convert.ToInt32(item.ToString()) > 255)
                                        return new ValidationResult
                                        (false, "Maximum value can not be greater than 255");
                                }
                            }
                            else if (Wrapper.Trigger.TriggerModel.MOSIPattern == DataModule.ePatternFormat.Octal)
                            {
                                var regex = new Regex(@"^[0-9]\d*$");
                                var txtpattern = value.ToString().ToLower();
                                if (txtpattern.Contains("-"))
                                    regex = new Regex(@"^[0-9]+(-[0-9]+)+$");
                                if (!regex.IsMatch(txtpattern))
                                    return new ValidationResult
                                   (false, "Input string format is not correct");

                                var data = txtpattern.Trim().Split('-');
                                foreach (var item in data)
                                {
                                    if (Convert.ToInt32(item.ToString()) > 377)
                                        return new ValidationResult
                                        (false, "Maximum value can not be greater than 377");
                                }
                            }
                        }
                        else if (Wrapper.Trigger.TriggerModel.IsMISOChecked)
                        {
                            if (Wrapper.Trigger.TriggerModel.MISOPattern == DataModule.ePatternFormat.Hex)
                            {
                                var regex = new Regex(@"^[A-Fa-f0-9]*$");

                                var txtpattern = value.ToString().ToLower();
                                if (txtpattern.Contains("-"))
                                    regex = new Regex(@"^[A-Fa-f0-9]+(-[A-Fa-f0-9]+)+$");
                                if (txtpattern.Contains("0x"))
                                    txtpattern = txtpattern.Replace("0x", "");
                                int maxLength = 0xFF;

                                if (!regex.IsMatch(txtpattern))
                                    return new ValidationResult
                                   (false, "Input string format is not correct");

                                var data = txtpattern.Trim().Split('-');
                                foreach (var item in data)
                                {
                                    int.TryParse(item, System.Globalization.NumberStyles.HexNumber, null, out maxLength);
                                    if (maxLength > 0xFF)
                                        return new ValidationResult
                                        (false, "Maximum value can not be greater than 0xFF");
                                }
                            }
                            else if (Wrapper.Trigger.TriggerModel.MISOPattern == DataModule.ePatternFormat.Binary)
                            {
                                var regex = new Regex(@"^[01]+$");
                                var txtpattern = value.ToString().ToLower();
                                if (txtpattern.Contains("-"))
                                    regex = new Regex(@"^[01]+(-[01]+)+$");

                                if (!regex.IsMatch(txtpattern))
                                    return new ValidationResult
                                   (false, "Input string format is not correct");

                                var data = txtpattern.Trim().Split('-');
                                foreach (var item in data)
                                {
                                    if (item.Length != 8)
                                        return new ValidationResult
                                        (false, "Length should be 8 characters");
                                }

                            }
                            else if (Wrapper.Trigger.TriggerModel.MISOPattern == DataModule.ePatternFormat.Decimal)
                            {
                                var regex = new Regex(@"^[0-9]\d*$");
                                var txtpattern = value.ToString().ToLower();
                                if (txtpattern.Contains("-"))
                                    regex = new Regex(@"^[0-9]+(-[0-9]+)+$");

                                if (!regex.IsMatch(txtpattern))
                                    return new ValidationResult
                                   (false, "Input string format is not correct");

                                var data = txtpattern.Trim().Split('-');
                                foreach (var item in data)
                                {
                                    if (Convert.ToInt32(item.ToString()) > 255)
                                        return new ValidationResult
                                        (false, "Maximum value can not be greater than 255");
                                }
                            }
                            else if (Wrapper.Trigger.TriggerModel.MISOPattern == DataModule.ePatternFormat.Octal)
                            {
                                var regex = new Regex(@"^[0-9]\d*$");
                                var txtpattern = value.ToString().ToLower();
                                if (txtpattern.Contains("-"))
                                    regex = new Regex(@"^[0-9]+(-[0-9]+)+$");
                                if (!regex.IsMatch(txtpattern))
                                    return new ValidationResult
                                   (false, "Input string format is not correct");

                                var data = txtpattern.Trim().Split('-');
                                foreach (var item in data)
                                {
                                    if (Convert.ToInt32(item.ToString()) > 377)
                                        return new ValidationResult
                                        (false, "Maximum value can not be greater than 377");
                                }
                            }
                        }
                    }
                    else if (Wrapper.Trigger.TriggerModel.ProtocolTypeIndex == DataModule.eProtocolTypeList.UART)
                    {
                        //if (value == null || value.ToString() == string.Empty)
                        //    return new ValidationResult(false, "value cannot be empty.");
                        if (Wrapper.Trigger.TriggerModel.UARTDataPattern == DataModule.ePatternFormat.Hex)
                        {
                            var regex = new Regex(@"^[A-Fa-f0-9]*$");

                            var txtpattern = value.ToString().ToLower();
                            if (txtpattern.Contains("-"))
                                regex = new Regex(@"^[A-Fa-f0-9]+(-[A-Fa-f0-9]+)+$");
                            if (txtpattern.Contains("0x"))
                                txtpattern = txtpattern.Replace("0x", "");
                            int maxLength = 0xFF;

                            if (!regex.IsMatch(txtpattern))
                                return new ValidationResult
                               (false, "Input string format is not correct");

                            var data = txtpattern.Trim().Split('-');
                            foreach (var item in data)
                            {
                                int.TryParse(item, System.Globalization.NumberStyles.HexNumber, null, out maxLength);
                                if (maxLength > 0xFF)
                                    return new ValidationResult
                                    (false, "Maximum value can not be greater than 0xFF");
                            }
                        }
                        else if (Wrapper.Trigger.TriggerModel.UARTDataPattern == DataModule.ePatternFormat.Binary)
                        {
                            var regex = new Regex(@"^[01]+$");
                            var txtpattern = value.ToString().ToLower();
                            if (txtpattern.Contains("-"))
                                regex = new Regex(@"^[01]+(-[01]+)+$");

                            if (!regex.IsMatch(txtpattern))
                                return new ValidationResult
                               (false, "Input string format is not correct");

                            var data = txtpattern.Trim().Split('-');
                            foreach (var item in data)
                            {
                                if (item.Length != 8)
                                    return new ValidationResult
                                    (false, "Length should be 8 characters");
                            }

                        }
                        else if (Wrapper.Trigger.TriggerModel.UARTDataPattern == DataModule.ePatternFormat.Decimal)
                        {
                            var regex = new Regex(@"^[0-9]\d*$");
                            var txtpattern = value.ToString().ToLower();
                            if (txtpattern.Contains("-"))
                                regex = new Regex(@"^[0-9]+(-[0-9]+)+$");

                            if (!regex.IsMatch(txtpattern))
                                return new ValidationResult
                               (false, "Input string format is not correct");

                            var data = txtpattern.Trim().Split('-');
                            foreach (var item in data)
                            {
                                if (Convert.ToInt32(item.ToString()) > 255)
                                    return new ValidationResult
                                    (false, "Maximum value can not be greater than 255");
                            }
                        }
                        else if (Wrapper.Trigger.TriggerModel.UARTDataPattern == DataModule.ePatternFormat.Octal)
                        {
                            var regex = new Regex(@"^[0-9]\d*$");
                            var txtpattern = value.ToString().ToLower();
                            if (txtpattern.Contains("-"))
                                regex = new Regex(@"^[0-9]+(-[0-9]+)+$");
                            if (!regex.IsMatch(txtpattern))
                                return new ValidationResult
                               (false, "Input string format is not correct");

                            var data = txtpattern.Trim().Split('-');
                            foreach (var item in data)
                            {
                                if (Convert.ToInt32(item.ToString()) > 377)
                                    return new ValidationResult
                                    (false, "Maximum value can not be greater than 377");
                            }
                        }
                    }
                    else
                    {
                        var txtpattern = value.ToString().ToLower();
                        if (Wrapper.Format.Contains("data"))
                        {
                            var regex = new Regex(@"^[A-Fa-f0-9]*$");

                            if (txtpattern.Contains("-"))
                                regex = new Regex(@"^[A-Fa-f0-9]+(-[A-Fa-f0-9]+)+$");
                            if (txtpattern.Contains("0x"))
                                txtpattern = txtpattern.Replace("0x", "");
                            int maxLength = 0xFF;

                            if (!regex.IsMatch(txtpattern))
                                return new ValidationResult
                               (false, "Input string format is not correct");

                            var data = txtpattern.Trim().Split('-');
                            foreach (var item in data)
                            {
                                int.TryParse(item, System.Globalization.NumberStyles.HexNumber, null, out maxLength);
                                if (maxLength > 0xFF)
                                    return new ValidationResult
                                    (false, "Maximum value can not be greater than 0xFF");
                            }
                        }
                        else
                        {
                            //var regex = new Regex(@"^[0-9]\d*$");

                            //if (txtpattern == string.Empty)
                            //    return new ValidationResult
                            //      (true, "");
                            //if (!regex.IsMatch(txtpattern))
                            //    return new ValidationResult
                            //   (false, "Input string format is not correct");
                            //else if (Convert.ToInt32(value.ToString()) > 255)
                            //    return new ValidationResult
                            //    (false, "Maximum value can not be greater than 255");
                            Regex r = new Regex(@"^[A-Fa-f0-9]*$");
                            txtpattern = value.ToString().ToLower();
                            if (txtpattern.Contains("0x"))
                                txtpattern = txtpattern.Replace("0x", "");
                            int maxLength = -1;
                            int.TryParse(txtpattern, System.Globalization.NumberStyles.HexNumber, null, out maxLength);
                            if (!r.IsMatch(txtpattern))
                                return new ValidationResult
                               (false, "Input string format is not correct");
                            // else if (maxLength > 0x3FF)
                            //   return new ValidationResult
                            // (false, "Maximum value can not be greater than 0x3FF");
                        }
                    }
                }
                else if (Wrapper.Trigger.TriggerModel.TriggerType == DataModule.eTriggerTypeList.Timing)
                {
                    //if (value == null || value.ToString() == string.Empty)
                    //    return new ValidationResult(false, "value cannot be empty.");
                    if (Wrapper.Trigger.TriggerModel.TimingTriggerTypeSelected == DataModule.eTimingTriggerTypeList.Pulse_Width)
                    {
                        var regex = new Regex(@"^[0-9]\d*$");
                        var txtpattern = value.ToString().ToLower();
                        if (!regex.IsMatch(txtpattern))
                            return new ValidationResult
                           (false, "Input string format is not correct");
                        else if (System.Convert.ToInt32(value) > 65535)
                            return new ValidationResult
                            (false, "Maximum value can not be greater than 65535");
                    }
                    else if (Wrapper.Trigger.TriggerModel.TimingTriggerTypeSelected == DataModule.eTimingTriggerTypeList.Delay)
                    {
                        var regex = new Regex(@"^[0-9]\d*$");
                        var txtpattern = value.ToString().ToLower();
                        if (!regex.IsMatch(txtpattern))
                            return new ValidationResult
                           (false, "Input string format is not correct");
                        else if (System.Convert.ToInt32(value) > 65535)
                            return new ValidationResult
                            (false, "Maximum value can not be greater than 65535");
                    }
                }
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Input string format is not correct");
            }

            return ValidationResult.ValidResult;
        }

        public TriggerWrapper Wrapper { get; set; }
    }

    public class TriggerWrapper : DependencyObject
    {
        public static readonly DependencyProperty TriggerProperty =
             DependencyProperty.Register("Trigger", typeof(TriggerViewModel),
             typeof(TriggerWrapper), new FrameworkPropertyMetadata(null));

        public TriggerViewModel Trigger
        {
            get { return (TriggerViewModel)GetValue(TriggerProperty); }
            set { SetValue(TriggerProperty, value); }
        }

        public static readonly DependencyProperty FormatProperty =
             DependencyProperty.Register("Format", typeof(string),
             typeof(TriggerWrapper), new FrameworkPropertyMetadata(null));

        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }
    }
}
