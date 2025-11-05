using System.Globalization;
using System.Windows.Controls;

namespace RetailCorrector.Utils.ValidationRules
{
    internal partial class DeviceIdValidationRule : ValidationRule
    {
        private static DeviceIdValidationRule _instance = null!;
        public static DeviceIdValidationRule Instance => _instance ??= new DeviceIdValidationRule();

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var text = (string)value;
            if (text.Length != 16)
                return new ValidationResult(false, "Введённое значение имеет размер отличный от 16 символов");
            if (!ulong.TryParse(text, out _))
                return new ValidationResult(false, "Введённое значение не является числовым кодом");
            return ValidationResult.ValidResult;
        }
    }
}
