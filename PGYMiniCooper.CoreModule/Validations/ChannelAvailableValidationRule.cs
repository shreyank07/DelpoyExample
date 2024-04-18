using PGYMiniCooper.CoreModule.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PGYMiniCooper.CoreModule.Validations
{
    public class ChannelAvailableValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is ChannelSelectionViewModel channelSelection)
            {
                if (channelSelection.IsAvailable == false)
                    return new ValidationResult(false, "Channel already in use.");
                else
                    return ValidationResult.ValidResult;
            }
            else
                throw new InvalidCastException($"This validation rule only supports data of {typeof(ChannelSelectionViewModel)}.");
        }
    }
}
