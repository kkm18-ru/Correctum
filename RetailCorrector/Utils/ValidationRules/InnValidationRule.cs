using System.Globalization;
using System.Windows.Controls;

namespace RetailCorrector.Utils.ValidationRules
{
    internal class InnValidationRule : ValidationRule
    {
        public static InnValidationRule Instance => field ??= new InnValidationRule();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private const string InvalidMessage = "\"{Value}\" - не является корректным ИНН: {Reason}";

        public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
        {
            var text = (string?)value ?? "";
            if (text.Length != 10 && text.Length != 12)
            {
                Logger.Info(InvalidMessage, text, "размер текста отличный от 10 или 12 символов");
                return new ValidationResult(false, "Введённое значение имеет размер отличный от 10 или 12 символов");
            }
            if (!ulong.TryParse(text, out var result))
            {
                Logger.Info(InvalidMessage, text, "текст не является числовым кодом");
                return new ValidationResult(false, "Введённое значение не является числовым кодом");
            }
            var digits = new byte[text.Length];
            for (var i = 0; i < digits.Length; i++)
                digits[i] = (byte)((result / (ulong)Math.Pow(10, (digits.Length - i - 1))) % 10);
            byte[] multiple = [3, 7, 2, 4, 10, 3, 5, 9, 4, 6, 8];
            switch (text.Length)
            {
                case 10:
                    Logger.Info("\"{Value}\" - это ИНН юридического лица", text);
                    var n10 = 0;
                    for (var i = 0; i < 9; i++)
                        n10 += digits[i] * multiple[2 + i];
                    n10 = n10 % 11 % 10;
                    Logger.Debug("Для ИНН {Value} рассчитано контрольное значение: {Hash}", text, n10);
                    if ((byte)n10 != digits[^1])
                    {
                        Logger.Info(InvalidMessage, text, "контрольное значение некорректно");
                        return new ValidationResult(false, "Контрольное значение ИНН некорректно");
                    }
                    break;
                case 12:
                    Logger.Info("\"{Value}\" - это ИНН физического лица", text);
                    var n11 = 0;
                    var n12 = 0;
                    for (var i = 0; i < 10; i++)
                    {
                        n11 += digits[i] * multiple[1 + i];
                        n12 += digits[i] * multiple[i];
                    }
                    n11 = n11 % 11 % 10;
                    n12 = (n12 + n11 * multiple[^1]) % 11 % 10;
                    Logger.Debug("Для ИНН {Value} рассчитано контрольное значение: {Hash}", text, $"{n11}{n12}");
                    if ((byte)n12 != digits[^1] || (byte)n11 != digits[^2])
                    {
                        Logger.Info(InvalidMessage, text, "контрольное значение некорректно");
                        return new ValidationResult(false, "Контрольное значение ИНН некорректно");
                    }
                    break;
            }
            Logger.Info("\"{Value}\" - корректный ИНН", text);
            return ValidationResult.ValidResult;
        }
    }
}
