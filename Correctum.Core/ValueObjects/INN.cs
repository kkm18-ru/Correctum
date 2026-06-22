namespace Correctum.Core.ValueObjects
{
    /// <summary>
    /// Представляет идентификационный номер налогоплательщика (ИНН)
    /// </summary>
    public readonly struct Inn : IEquatable<Inn>, IComparable<Inn>
    {
        private readonly ulong _value;

        private Inn(ulong value) => _value = value;

        /// <summary>
        /// Преобразовывает строковое представление объекта ИНН в эквивалентную структуру <see cref="Inn"/>.
        /// </summary>
        /// <param name="s">Строка для синтаксического анализа.</param>
        /// <returns>Результат синтаксического анализа <paramref name="s"/>.</returns>
        /// <exception cref="FormatException"><paramref name="s"/> не находится в распознаваемом формате.</exception>
        public static Inn? Parse(string s) => Parse(s, null);
        /// <summary>
        /// Анализирует строку в значение.
        /// </summary>
        /// <param name="s">Строка для синтаксического анализа.</param>
        /// <param name="provider">Объект, предоставляющий сведения о форматировании, зависящее от языка и региональных параметров, о <paramref name="s"/>.</param>
        /// <returns>Результат синтаксического анализа <paramref name="s"/>.</returns>
        /// <exception cref="FormatException"><paramref name="s"/> не находится в распознаваемом формате.</exception>
        public static Inn? Parse(string s, IFormatProvider? provider) => Parse(s ?? string.Empty, out var result) switch
        {
            InnParsingStatus.Ok => result,
            InnParsingStatus.OutOfRangeLength => throw new FormatException("Длина ввода отлична от 10 и 12 символов."),
            InnParsingStatus.IsNotDigitCode => throw new FormatException("Ввод не является числовым кодом."),
            InnParsingStatus.InvalidHashSum => throw new FormatException("Контрольное значение ИНН некорректно."),
            InnParsingStatus status => throw new NotImplementedException($"Неизвестный статус код: {status}.")
        };

        /// <summary>
        /// Пытается преобразовать строковое представление объекта ИНН в эквивалентную структуру <see cref="Inn"/>.
        /// </summary>
        /// <param name="s">Строка для синтаксического анализа.</param>
        /// <param name="result">При возврате содержит результат успешного анализа <paramref name="s"/> или неопределенное значение при сбое.</param>
        /// <returns>true, если <paramref name="s"/> анализ выполнен успешно; в противном случае — значение false.</returns>
        public static bool TryParse(string? s, out Inn? result) => TryParse(s, null, out result);

        /// <summary>
        /// Пытается проанализировать строку в значение.
        /// </summary>
        /// <param name="s">Строка для синтаксического анализа.</param>
        /// <param name="provider">Объект, предоставляющий сведения о форматировании, зависящее от языка и региональных параметров, о <paramref name="s"/>.</param>
        /// <param name="result">При возврате содержит результат успешного анализа <paramref name="s"/> или неопределенное значение при сбое.</param>
        /// <returns>true, если <paramref name="s"/> анализ выполнен успешно; в противном случае — значение false.</returns>
        public static bool TryParse(string? s, IFormatProvider? provider, out Inn? result) => Parse(s ?? string.Empty, out result) == 0;

        /// <summary>
        /// Возвращает строковое представление значения этого экземпляра <see cref="Inn"/> структуры.
        /// </summary>
        /// <returns>Значение этого <see cref="Inn"/>.</returns>
        public override string ToString() => _value switch
        {
            >= 100_000_000 and <= 9_999_999_999 => _value.ToString("D10"),
            >= 10_000_000_000 and <= 999_999_999_999 => _value.ToString("D12"),
            _ => ""
        };

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is Inn inn && Equals(inn);

        /// <inheritdoc/>
        public bool Equals(Inn other) => _value.Equals(other._value);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(_value);

        /// <inheritdoc/>
        public int CompareTo(Inn other) => _value.CompareTo(other._value);

        /// <inheritdoc/>
        public static bool operator ==(Inn left, Inn right) => left.Equals(right);

        /// <inheritdoc/>
        public static bool operator !=(Inn left, Inn right) => !left.Equals(right);

        /// <inheritdoc/>
        public static bool operator <(Inn left, Inn right) => left.CompareTo(right) < 0;

        /// <inheritdoc/>
        public static bool operator <=(Inn left, Inn right) => left.CompareTo(right) <= 0;

        /// <inheritdoc/>
        public static bool operator >(Inn left, Inn right) => left.CompareTo(right) > 0;

        /// <inheritdoc/>
        public static bool operator >=(Inn left, Inn right) => left.CompareTo(right) >= 0;

        private enum InnParsingStatus { Ok, OutOfRangeLength, IsNotDigitCode, InvalidHashSum }
        private static InnParsingStatus Parse(string s, out Inn? result)
        {
            result = default;
            if(s.All(c => c == '0')) return InnParsingStatus.Ok;
            if (s.Length != 10 && s.Length != 12) return InnParsingStatus.OutOfRangeLength;
            if (!ulong.TryParse(s, out var code)) return InnParsingStatus.IsNotDigitCode;
            var temp = new byte[s.Length];
            for (var i = 0; i < temp.Length; i++)
                temp[i] = (byte)(code / (ulong)Math.Pow(10, temp.Length - i - 1) % 10);
            byte[] multiple = [3, 7, 2, 4, 10, 3, 5, 9, 4, 6, 8];
            switch (s.Length)
            {
                case 10:
                    var n10 = 0;
                    for (var i = 0; i < 9; i++)
                        n10 += temp[i] * multiple[2 + i];
                    n10 = n10 % 11 % 10;
                    if ((byte)n10 != temp[^1]) return InnParsingStatus.InvalidHashSum;
                    break;
                case 12:
                    var n11 = 0;
                    var n12 = 0;
                    for (var i = 0; i < 10; i++)
                    {
                        n11 += temp[i] * multiple[1 + i];
                        n12 += temp[i] * multiple[i];
                    }
                    n11 = n11 % 11 % 10;
                    n12 = (n12 + n11 * multiple[^1]) % 11 % 10;
                    if ((byte)n12 != temp[^1] || (byte)n11 != temp[^2]) return InnParsingStatus.InvalidHashSum;
                    break;
            }
            result = new(code);
            return InnParsingStatus.Ok;
        }
    }
}
