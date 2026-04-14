using System.Buffers.Text;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RetailCorrector.ValueObjects;

public readonly struct FiscalSign : IUtf8SpanParsable<FiscalSign>, ISpanParsable<FiscalSign>
{
    private readonly uint _data;

    public FiscalSign(uint data) => _data = data;

    public static FiscalSign Parse(string? s) => Parse(s, null);
    public static FiscalSign Parse(string? s, IFormatProvider? provider) => Parse(s.AsSpan(), provider);

    public static FiscalSign Parse(ReadOnlySpan<char> s) => Parse(s, null);
    public static FiscalSign Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        Span<byte> utf8Text = stackalloc byte[Encoding.UTF8.GetByteCount(s)];
        Encoding.UTF8.GetBytes(s, utf8Text);
        return Parse(utf8Text, provider);
    }

    public static FiscalSign Parse(ReadOnlySpan<byte> utf8Text) => Parse(utf8Text, null);
    public static FiscalSign Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider) => 
        TryParse(utf8Text, provider, out var result) ? result : 
        throw new FormatException("Ввод не является, ни TLV-, ни целочисленным представлением ФПД.");


    public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out FiscalSign result) => TryParse(s, null, out result);
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out FiscalSign result) => TryParse(s.AsSpan(), provider, out result);

    public static bool TryParse(ReadOnlySpan<char> s, [MaybeNullWhen(false)] out FiscalSign result) => TryParse(s, null, out result);
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out FiscalSign result)
    {
        Span<byte> utf8Text = stackalloc byte[Encoding.UTF8.GetByteCount(s)];
        Encoding.UTF8.GetBytes(s, utf8Text);
        return TryParse(utf8Text, provider, out result);
    }

    public static bool TryParse(ReadOnlySpan<byte> utf8Text, [MaybeNullWhen(false)] out FiscalSign result) => TryParse(utf8Text, null, out result);
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, [MaybeNullWhen(false)] out FiscalSign result)
    {
        if (!utf8Text.IsEmpty)
        {
            if (uint.TryParse(utf8Text, out var data))
            {
                result = new(data);
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

    public override string ToString() => _data.ToString();
}
