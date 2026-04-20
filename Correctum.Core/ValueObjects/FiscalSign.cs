using System.Buffers.Text;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Correctum.Core.ValueObjects
{
    /// <summary>
    /// Представляет фискальный признак документа
    /// </summary>
    public readonly struct FiscalSign : IUtf8SpanParsable<FiscalSign>, ISpanParsable<FiscalSign>, IFormattable
    {
        private readonly uint _value;

        private FiscalSign(uint value) => _value = value;

        /// <summary>
        /// Преобразовывает строковое представление объекта ФПД в эквивалентную структуру <see cref="FiscalSign"/>.
        /// </summary>
        /// <param name="s">Строка для синтаксического анализа.</param>
        /// <returns>Результат синтаксического анализа <paramref name="s"/>.</returns>
        /// <exception cref="FormatException"><paramref name="s"/> не находится в распознаваемом формате.</exception>
        public static FiscalSign Parse(string? s) => Parse(s, null);
        /// <summary>
        /// Анализирует строку в значение.
        /// </summary>
        /// <param name="s">Строка для синтаксического анализа.</param>
        /// <param name="provider">Объект, предоставляющий сведения о форматировании, зависящее от языка и региональных параметров, о <paramref name="s"/>.</param>
        /// <returns>Результат синтаксического анализа <paramref name="s"/>.</returns>
        /// <exception cref="FormatException"><paramref name="s"/> не находится в распознаваемом формате.</exception>
        public static FiscalSign Parse(string? s, IFormatProvider? provider) => Parse(s.AsSpan(), provider);

        /// <summary>
        /// Преобразовывает диапазон символов только для чтения, представляющий ФПД в эквивалентную структуру <see cref="FiscalSign"/>.
        /// </summary>
        /// <param name="s">Диапазон символов для синтаксического анализа.</param>
        /// <returns>Результат синтаксического анализа <paramref name="s"/>.</returns>
        /// <exception cref="FormatException"><paramref name="s"/> не находится в распознаваемом формате.</exception>
        public static FiscalSign Parse(ReadOnlySpan<char> s) => Parse(s, null);
        /// <summary>
        /// Анализирует диапазон символов в значение.
        /// </summary>
        /// <param name="s">Диапазон символов для синтаксического анализа.</param>
        /// <param name="provider">Объект, предоставляющий сведения о форматировании, зависящее от языка и региональных параметров, о <paramref name="s"/>.</param>
        /// <returns>Результат синтаксического анализа <paramref name="s"/>.</returns>
        /// <exception cref="FormatException"><paramref name="s"/> не находится в распознаваемом формате.</exception>
        public static FiscalSign Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        {
            Span<byte> utf8Text = stackalloc byte[Encoding.UTF8.GetByteCount(s)];
            Encoding.UTF8.GetBytes(s, utf8Text);
            return Parse(utf8Text, provider);
        }

        /// <summary>
        /// Преобразовывает диапазон символов UTF-8, представляющий ФПД в эквивалентную структуру <see cref="FiscalSign"/>.
        /// </summary>
        /// <param name="utf8Text">Диапазон символов UTF-8 для синтаксического анализа.</param>
        /// <returns>Результат синтаксического анализа <paramref name="utf8Text"/>.</returns>
        /// <exception cref="FormatException"><paramref name="utf8Text"/> не находится в распознаваемом формате.</exception>
        public static FiscalSign Parse(ReadOnlySpan<byte> utf8Text) => Parse(utf8Text, null);
        /// <summary>
        /// Анализирует диапазон символов UTF-8 в значение.
        /// </summary>
        /// <param name="utf8Text">Диапазон символов UTF-8 для синтаксического анализа.</param>
        /// <param name="provider">Объект, предоставляющий сведения о форматировании, зависящее от языка и региональных параметров, о <paramref name="utf8Text"/>.</param>
        /// <returns>Результат синтаксического анализа <paramref name="utf8Text"/>.</returns>
        /// <exception cref="FormatException"><paramref name="utf8Text"/> не находится в распознаваемом формате.</exception>
        public static FiscalSign Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider) => 
            TryParse(utf8Text, provider, out var result) ? result : 
            throw new FormatException("Ввод не является, ни TLV-, ни целочисленным представлением ФПД.");


        /// <summary>
        /// Пытается преобразовать строковое представление объекта ФПД в эквивалентную структуру <see cref="FiscalSign"/>.
        /// </summary>
        /// <param name="s">Строка для синтаксического анализа.</param>
        /// <param name="result">При возврате содержит результат успешного анализа <paramref name="s"/> или неопределенное значение при сбое.</param>
        /// <returns>true, если <paramref name="s"/> анализ выполнен успешно; в противном случае — значение false.</returns>
        public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out FiscalSign result) => TryParse(s, null, out result);

        /// <summary>
        /// Пытается проанализировать строку в значение.
        /// </summary>
        /// <param name="s">Строка для синтаксического анализа.</param>
        /// <param name="provider">Объект, предоставляющий сведения о форматировании, зависящее от языка и региональных параметров, о <paramref name="s"/>.</param>
        /// <param name="result">При возврате содержит результат успешного анализа <paramref name="s"/> или неопределенное значение при сбое.</param>
        /// <returns>true, если <paramref name="s"/> анализ выполнен успешно; в противном случае — значение false.</returns>
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out FiscalSign result) => TryParse(s.AsSpan(), provider, out result);

        /// <summary>
        /// Пытается преобразовать диапазон символов только для чтения, представляющий ФПД в эквивалентную структуру <see cref="FiscalSign"/>.
        /// </summary>
        /// <param name="s">Диапазон символов для анализа.</param>
        /// <param name="result">При возврате содержит результат успешного анализа <paramref name="s"/> или неопределенное значение при сбое.</param>
        /// <returns>true, если <paramref name="s"/> анализ выполнен успешно; в противном случае — значение false.</returns>
        public static bool TryParse(ReadOnlySpan<char> s, [MaybeNullWhen(false)] out FiscalSign result) => TryParse(s, null, out result);
        /// <summary>
        /// Пытается проанализировать диапазон символов в значение.
        /// </summary>
        /// <param name="s">Диапазон символов для анализа.</param>
        /// <param name="provider">Объект, предоставляющий сведения о форматировании, зависящее от языка и региональных параметров, о <paramref name="s"/>.</param>
        /// <param name="result">При возврате содержит результат успешного анализа <paramref name="s"/> или неопределенное значение при сбое.</param>
        /// <returns>true, если <paramref name="s"/> анализ выполнен успешно; в противном случае — значение false.</returns>
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out FiscalSign result)
        {
            Span<byte> utf8Text = stackalloc byte[Encoding.UTF8.GetByteCount(s)];
            Encoding.UTF8.GetBytes(s, utf8Text);
            return TryParse(utf8Text, provider, out result);
        }

        /// <summary>
        /// Пытается преобразовать диапазон символов UTF-8, представляющий ФПД в эквивалентную структуру <see cref="FiscalSign"/>.
        /// </summary>
        /// <param name="utf8Text">Диапазон символов UTF-8 для анализа.</param>
        /// <param name="result">При возврате содержит результат успешного анализа <paramref name="utf8Text"/> или неопределенное значение при сбое.</param>
        /// <returns>true, если <paramref name="utf8Text"/> анализ выполнен успешно; в противном случае — значение false.</returns>
        public static bool TryParse(ReadOnlySpan<byte> utf8Text, [MaybeNullWhen(false)] out FiscalSign result) => TryParse(utf8Text, null, out result);
        /// <summary>
        /// Пытается проанализировать диапазон символов UTF-8 в значение.
        /// </summary>
        /// <param name="utf8Text">Диапазон символов UTF-8 для анализа.</param>
        /// <param name="provider">Объект, предоставляющий сведения о форматировании, зависящее от языка и региональных параметров, о <paramref name="utf8Text"/>.</param>
        /// <param name="result">При возврате содержит результат успешного анализа <paramref name="utf8Text"/> или неопределенное значение при сбое.</param>
        /// <returns>true, если <paramref name="utf8Text"/> анализ выполнен успешно; в противном случае — значение false.</returns>
        public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, [MaybeNullWhen(false)] out FiscalSign result)
        {
            if (!utf8Text.IsEmpty)
            {
                if (uint.TryParse(utf8Text, out var data))
                {
                    result = data;
                    return true;
                }
                if (Encoding.UTF8.GetCharCount(utf8Text) == 8 && Base64.IsValid(utf8Text))
                {
                    Span<byte> bytes = stackalloc byte[6];
                    if (Base64.DecodeFromUtf8(utf8Text, bytes, out _, out _) == System.Buffers.OperationStatus.Done)
                    {
                        bytes.Reverse();
                        result = new(BitConverter.ToUInt32(bytes));
                        return true;
                    }
                }
            }
            result = default;
            return false;
        }

        /// <summary>
        /// Преобразует целочисленное число без знака в число <see cref="FiscalSign"/>.
        /// </summary>
        /// <param name="value">Преобразуемое целое число без знака.</param>
        public static implicit operator FiscalSign(uint value) => new(value);

        /// <summary>
        /// Возвращает строковое представление значения этого экземпляра <see cref="FiscalSign"/> структуры.
        /// </summary>
        /// <returns>Значение этого <see cref="FiscalSign"/>.</returns>
        public override string ToString() => ToString("D", null);

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            switch (format)
            {
                case "r" or "R":
                    Span<byte> bytes = stackalloc byte[6];
                    BitConverter.TryWriteBytes(bytes, _value);
                    bytes[4] = 0b00000100;
                    bytes[5] = 0b00110001;
                    bytes.Reverse();

                    Span<char> chars = stackalloc char[8];
                    Convert.TryToBase64Chars(bytes, chars, out _);

                    var textSign = new string(chars);
                    return textSign;
                case "d" or "D":
                    return _value.ToString();
                default: return "";
            }
        }
    }
}