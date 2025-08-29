namespace RetailCorrector.Validations
{
    /// <summary>
    /// Результат проверки корректности вводимого значения
    /// </summary>
    /// <param name="message">Текст сообщения об ошибке</param>
    internal readonly struct ValidationResult(string? message = null)
    {
        /// <summary>
        /// Текст сообщения об ошибке
        /// </summary>
        public string? MessageError { get; } = message;
        /// <summary>
        /// true, если текст сообщения об ошибке отсутствует, иначе false
        /// </summary>
        public readonly bool IsValid => MessageError is null;

        public override string ToString() => MessageError ?? "Значение корректно!";

        public static implicit operator ValidationResult(string? message) => new(message);
        public static implicit operator bool(ValidationResult result) => result.IsValid;
        public static implicit operator string(ValidationResult result) => result.ToString();
    }
}
