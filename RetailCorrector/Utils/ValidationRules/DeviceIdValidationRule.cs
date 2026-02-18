using System.Globalization;
using System.Windows.Controls;

namespace RetailCorrector.Utils.ValidationRules
{
    internal class DeviceIdValidationRule : ValidationRule
    {
        public static DeviceIdValidationRule Instance => field ??= new DeviceIdValidationRule();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private const string InvalidMessage = "\"{Value}\" - не является корректным РНМ/серийным номером ФН: {Reason}";

        public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
        {
            var text = (string?)value ?? "";
            if (text.Length != 16)
            {
                Logger.Info(InvalidMessage, text, "размер текста отличный от 16 символов");
                return new ValidationResult(false, "Введённое значение имеет размер отличный от 16 символов");
            }
            if (!ulong.TryParse(text, out _))
            {
                Logger.Info(InvalidMessage, text, "текст не является числовым кодом");
                return new ValidationResult(false, "Введённое значение не является числовым кодом");
            }
            Logger.Info("\"{Value}\" - корректный регистрационный номер ККТ/серийный номер ФН", text);
            return ValidationResult.ValidResult;
        }
    }
}
