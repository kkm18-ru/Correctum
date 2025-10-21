using Kraig.Attributes;
using System.Globalization;
using System.Windows.Controls;

namespace RetailCorrector.Utils.ValidationRules
{
    [Singleton]
    internal partial class InnValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var text = (string)value;
            if (text.Length != 10 && text.Length != 12)
                return new ValidationResult(false, "Введённое значение имеет размер отличный от 10 или 12 символов");
            if (!ulong.TryParse(text, out var result))
                return new ValidationResult(false, "Введённое значение не является числовым кодом");
            var digits = new byte[text.Length];
            for (var i = 0; i < digits.Length; i++)
                digits[i] = (byte)((result / (ulong)Math.Pow(10, (digits.Length - i - 1))) % 10);
            byte[] multiple = [3, 7, 2, 4, 10, 3, 5, 9, 4, 6, 8];
            switch (text.Length)
            {
                case 10:
                    var n10 = 0;
                    for (var i = 0; i < 9; i++)
                        n10 += digits[i] * multiple[2 + i];
                    n10 = n10 % 11 % 10;
                    if ((byte)n10 != digits[^1])
                        return new ValidationResult(false, "Контрольное значение ИНН некорректно");
                    break;
                case 12:
                    var n11 = 0;
                    var n12 = 0;
                    for (var i = 0; i < 10; i++)
                    {
                        n11 += digits[i] * multiple[1 + i];
                        n12 += digits[i] * multiple[i];
                    }
                    n11 = n11 % 11 % 10;
                    n12 = (n12 + n11 * multiple[^1]) % 11 % 10;
                    if ((byte)n12 != digits[^1] || (byte)n11 != digits[^2])
                        return new ValidationResult(false, "Контрольное значение ИНН некорректно");
                    break;
            }
            return ValidationResult.ValidResult;
        }
    }
}
