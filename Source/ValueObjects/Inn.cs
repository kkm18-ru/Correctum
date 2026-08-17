using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Correctum.ValueObjects
{
    public readonly struct Inn : ISpanParsable<Inn>, IEquatable<Inn>
    {
        public bool IsCompany => _isCompany;

        private readonly uint _data1;
        private readonly byte _data2;
        private readonly bool _isCompany;

        private readonly static byte[] multiples =
            [8, 6, 4, 9, 5, 3, 10, 4, 2, 7, 3];

        private Inn(bool isCompany, uint data1, byte data2)
        {
            _data1 = data1;
            _data2 = data2;
            _isCompany = isCompany;
        }

        public bool Equals(Inn other) => 
            _data1.Equals(other._data1) && 
            _data2.Equals(other._data2) && 
            _isCompany.Equals(other._isCompany);

        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj is Inn other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(_data1, _data2, _isCompany);

        public override string ToString()
        {
            Span<byte> bytes = stackalloc byte[8];
            var builder = new StringBuilder(_isCompany ? 10 : 12);
            BinaryPrimitives.WriteUInt32BigEndian(bytes, _data1);
            bytes[4] = _data2;
            var raw = BinaryPrimitives.ReadUInt64LittleEndian(bytes);
            builder.Append(raw);
            while (builder.Capacity != builder.Length)
                builder.Insert(0, '0');
            return builder.ToString();
        }


        public static bool TryValidate(ReadOnlySpan<char> text)
        {
            if (text is not { Length: 10 or 12 }) return false;
            Span<byte> digits = stackalloc byte[text.Length];
            for(var offset = 0; offset < text.Length; offset++)
            {
                if (text[offset] is < '0' or > '9') return false;
                digits[text.Length - offset - 1] = (byte)(text[offset] - '0');
            }

            return digits.Length == 10 && 
                CompanyCheckDigitValidate(digits) || 
                PersonalCheckDigitsValidate(digits);
        }

        public static void Validate(ReadOnlySpan<char> text)
        {
            if (text is not { Length: 10 or 12 })
                throw new FormatException("ИНН состоит из 10/12 символов.");
            Span<byte> digits = stackalloc byte[text.Length];
            for(var offset = 0; offset < text.Length; offset++)
            {
                if (text[offset] is < '0' or > '9') 
                    throw new FormatException("ИНН является цифровым кодом.");
                digits[text.Length - offset - 1] = (byte)(text[offset] - '0');
            }

            if(!(digits.Length == 10 && 
                CompanyCheckDigitValidate(digits) ||
                PersonalCheckDigitsValidate(digits))) 
                throw new FormatException("Некорректные контрольные цифры.");
        }

        private static bool PersonalCheckDigitsValidate(Span<byte> innDigits)
        {
            int n11 = 0, n12 = 0;
            for (var i = 1; i < innDigits.Length; i++)
            {
                if (i > 1)
                    n11 += innDigits[i] * multiples[i - 2];
                n12 += innDigits[i] * multiples[i - 1];
            }
            return 
                (n12 % 11 % 10) == innDigits[0] && 
                (n11 % 11 % 10) == innDigits[1];                
        }

        private static bool CompanyCheckDigitValidate(Span<byte> innDigits)
        {
            int n10 = 0;
            for (var i = 1; i < innDigits.Length; i++)
                n10 += innDigits[i] * multiples[i - 1];
            return (n10 % 11 % 10) == innDigits[0];
        }


        public static Inn Parse(ReadOnlySpan<char> s) => Parse(s, CultureInfo.InvariantCulture);
        public static Inn Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        {
            Validate(s);
            var numb = ulong.Parse(s, provider);
            Span<byte> bytes = stackalloc byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(bytes, numb);
            return new Inn(s.Length == 10, BinaryPrimitives.ReadUInt32BigEndian(bytes), bytes[4]);
        }

        public static Inn Parse(string s, IFormatProvider? provider) => Parse((ReadOnlySpan<char>)s, provider);

        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Inn result)
        {
            result = default;
            if (!TryValidate(s)) return false;
            if (!ulong.TryParse(s, provider, out var numb)) return false;
            Span<byte> bytes = stackalloc byte[8];
            if (!BinaryPrimitives.TryWriteUInt64LittleEndian(bytes, numb)) return false;
            result = new Inn(s.Length == 10, BinaryPrimitives.ReadUInt32BigEndian(bytes), bytes[4]);
            return true;
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Inn result) =>
            TryParse((ReadOnlySpan<char>)s, provider, out result);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Inn left, Inn right) => left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Inn left, Inn right) => !left.Equals(right);
    }
}
