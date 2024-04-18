using PGYMiniCooper.DataModule.Structure;
using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace PGYMiniCooper.CoreModule.View.Converters
{
    public class VoltageValidator : ValidationRule
    {
        public override ValidationResult Validate
         (object value, System.Globalization.CultureInfo cultureInfo)
        {
            string format = Wrapper.Format;
            if (value == null || value.ToString() == string.Empty)
                return new ValidationResult(false, "value cannot be empty.");
            else
            {
                try
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
                    else if (maxLength > 0xFFF)
                        return new ValidationResult
                        (false, "Maximum value can not be greater than 0xFFF");
                }
                catch (Exception e)
                {
                    return new ValidationResult
                                      (false, "Input string format is not correct");
                }
                finally
                {

                }
            }
            return ValidationResult.ValidResult;
        }
        public Wrapper Wrapper { get; set; }
    }
}
