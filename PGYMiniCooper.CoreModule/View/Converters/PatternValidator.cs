using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows;

namespace PGYMiniCooper.CoreModule.View.Converters
{
    public class PatternValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (Wrapper.PatternFormat == DataModule.ePatternFormat.Decimal)
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
                    if (Convert.ToInt32(item.ToString()) > 1023)
                        return new ValidationResult
                        (false, "Maximum value can not be greater than 1023");
                }
            }
            else if (Wrapper.PatternFormat == DataModule.ePatternFormat.Hex)
            {
                var regex = new Regex(@"^[A-Fa-f0-9]*$");

                var txtpattern = value.ToString().ToLower();
                if (txtpattern.Contains("-"))
                    regex = new Regex(@"^[A-Fa-f0-9]+(-[A-Fa-f0-9]+)+$");
                if (txtpattern.Contains("0x"))
                    txtpattern = txtpattern.Replace("0x", "");
                int maxLength = 0x3FF;

                if (!regex.IsMatch(txtpattern))
                    return new ValidationResult
                   (false, "Input string format is not correct");

                var data = txtpattern.Trim().Split('-');
                foreach (var item in data)
                {
                    int.TryParse(item, System.Globalization.NumberStyles.HexNumber, null, out maxLength);
                    if (maxLength > 0x3FF)
                        return new ValidationResult
                        (false, "Maximum value can not be greater than 0x3FF");
                }
            }

            return ValidationResult.ValidResult;
        }

        public PatternValidatorWrapper Wrapper { get; set; }
    }

    public class PatternValidatorWrapper : DependencyObject
    {
        public static readonly DependencyProperty PatternFormatProperty =
             DependencyProperty.Register("PatternFormat", typeof(DataModule.ePatternFormat),
             typeof(PatternValidatorWrapper), new FrameworkPropertyMetadata(DataModule.ePatternFormat.Hex));

        public DataModule.ePatternFormat PatternFormat
        {
            get { return (DataModule.ePatternFormat)GetValue(PatternFormatProperty); }
            set { SetValue(PatternFormatProperty, value); }
        }
    }
}
