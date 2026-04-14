#pragma warning disable S101

using System.Numerics;

namespace RetailCorrector.ValueObjects;

public readonly struct INN : IEqualityOperators<INN, INN, bool>, IEquatable<INN>, IParsable<INN>
{
    private readonly ulong _value;
    private INN(ulong value) => _value = value;

    public static INN Parse(string s) => Parse(s, null);
    public static INN Parse(string s, IFormatProvider? provider) => Parse(s ?? string.Empty, out var result) switch
    {
        InnParsingStatus.Ok => result,
        InnParsingStatus.OutOfRangeLength => throw new FormatException("Длина ввода отлична от 10 и 12 символов."),
        InnParsingStatus.IsNotDigitCode => throw new FormatException("Ввод не является числовым кодом."),
        InnParsingStatus.InvalidHashSum => throw new FormatException("Контрольное значение ИНН некорректно."),
        InnParsingStatus status => throw new NotImplementedException($"Неизвестный статус код: {status}.")
    };

    public static bool TryParse(string? s, out INN result) => TryParse(s, null, out result);
    public static bool TryParse(string? s, IFormatProvider? provider, out INN result) => Parse(s ?? string.Empty, out result) == 0;

    public override int GetHashCode() => _value.GetHashCode();
    public override bool Equals(object? obj) => obj is INN other && Equals(other);
    public bool Equals(INN other) => _value == other._value;
    public static bool operator ==(INN left, INN right) => left.Equals(right);
    public static bool operator !=(INN left, INN right) => !left.Equals(right);

    private enum InnParsingStatus { Ok, OutOfRangeLength, IsNotDigitCode, InvalidHashSum }
    private static InnParsingStatus Parse(string s, out INN result)
    {
        result = default;
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

