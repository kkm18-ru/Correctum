namespace RetailCorrector.ValueObjects;

public readonly struct Currency
{
    public readonly bool IsZero => _data == 0;

    private readonly ulong _data;

    private Currency(ulong data) => _data = data;

    public static implicit operator Currency(ulong value) => new(value);
    public static implicit operator Currency(long value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        return new((ulong)value);
    }

    public static implicit operator Currency(decimal value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        return new((ulong)Math.Round(value * 100));
    }

    public static implicit operator Currency(double value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        return new((ulong)Math.Round(value * 100));
    }

    public static explicit operator double(Currency curr) => curr._data / 100D;
    public static explicit operator decimal(Currency curr) => curr._data / 100M;
}
