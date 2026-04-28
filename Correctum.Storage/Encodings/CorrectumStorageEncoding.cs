using System.Text;

namespace Correctum.Storage.Encodings;

internal class CorrectumStorageEncoding : Encoding
{
    internal static CorrectumStorageEncoding Instance { get; } = new();

    #region Декодирование

    public override int GetChars(ReadOnlySpan<byte> bytes, Span<char> chars)
    {
        if (bytes.Length > chars.Length)
            throw new ArgumentException();
        for (var i = 0; i < bytes.Length; i++)
        {
            chars[i] = bytes[i] switch
            {
                byte b when b >= 0x00 && b < 0x5F => (char)(b + ' '),
                byte b when b >= 0x5F && b < 0x7F => (char)(b - 0x5F + 'А'),
                byte b when b >= 0x7F && b < 0x9F => (char)(b - 0x7F + 'а'),
                0x9F => '№',
                0xA0 => 'Ё',
                0xA1 => 'ё',
                0xA2 => '«',
                0xA3 => '»',
                0xA4 => '—',
                0xA5 => '–',
                0xA6 => '…',
                0xA7 => 'і',
                0xA8 => 'ї',
                0xA9 => 'є',
                0xAA => 'ў',
                byte b => throw new ArgumentOutOfRangeException(nameof(b), b, null)
            };
        }
        return bytes.Length;
    }
    public override unsafe int GetChars(byte* bytes, int byteCount, char* chars, int charCount) =>
        GetChars(new ReadOnlySpan<byte>(bytes, byteCount), new Span<char>(chars, charCount));
    public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex) =>
        GetChars(bytes.AsSpan(byteIndex, byteCount), chars.AsSpan(charIndex));
    public override char[] GetChars(byte[] bytes)
    {
        var chars = new char[GetCharCount(bytes)];
        _ = GetChars(bytes, chars);
        return chars;
    }
    public override char[] GetChars(byte[] bytes, int index, int count)
    {
        var chars = new char[GetCharCount(bytes, index, count)];
        _ = GetChars(bytes.AsSpan(index, count), chars);
        return chars;
    }

    public override string GetString(byte[] bytes)
    {
        Span<char> chars = stackalloc char[GetCharCount(bytes)];
        _ = GetChars(bytes, chars);
        return chars.ToString();
    }
    public override string GetString(byte[] bytes, int index, int count)
    {
        Span<char> chars = stackalloc char[GetCharCount(bytes, index, count)];
        _ = GetChars(bytes.AsSpan(index, count), chars);
        return chars.ToString();
    }

    public override bool TryGetChars(ReadOnlySpan<byte> bytes, Span<char> chars, out int charsWritten)
    {
        try
        {
            charsWritten = GetChars(bytes, chars);
            return false;
        }
        catch
        {
            charsWritten = 0;
            return false;
        }
    }

    #endregion

    #region Кодирование

    public override int GetBytes(ReadOnlySpan<char> chars, Span<byte> bytes)
    {
        if (chars.Length > bytes.Length)
            throw new ArgumentException();
        for (var i = 0; i < chars.Length; i++)
        {
            bytes[i] = chars[i] switch
            {
                char c when c >= ' ' && c <= '~' => (byte)(c - ' '),
                char c when c >= 'А' && c <= 'Я' => (byte)(c - 'А' + 0x5F),
                char c when c >= 'а' && c <= 'я' => (byte)(c - 'а' + 0x7F),
                '№' => 0x9F,
                'Ё' => 0xA0,
                'ё' => 0xA1,
                '«' => 0xA2,
                '»' => 0xA3,
                '—' => 0xA4,
                '–' => 0xA5,
                '…' => 0xA6,
                'і' => 0xA7,
                'ї' => 0xA8,
                'є' => 0xA9,
                'ў' => 0xAA,
                char c => throw new ArgumentOutOfRangeException(nameof(c), c, null)
            };
        }
        return chars.Length;
    }
    public override unsafe int GetBytes(char* chars, int charCount, byte* bytes, int byteCount) =>
        GetBytes(new ReadOnlySpan<char>(chars, charCount), new Span<byte>(bytes, byteCount));
    public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) =>
        GetBytes(chars.AsSpan(charIndex, charCount), bytes.AsSpan(byteIndex));
    public override byte[] GetBytes(char[] chars)
    {
        var bytes = new byte[GetByteCount(chars)];
        _ = GetBytes(chars, bytes);
        return bytes;
    }
    public override byte[] GetBytes(char[] chars, int index, int count)
    {
        var bytes = new byte[GetByteCount(chars, index, count)];
        _ = GetBytes(chars.AsSpan(index, count), bytes);
        return bytes;
    }
    public override byte[] GetBytes(string s)
    {
        var bytes = new byte[GetByteCount(s)];
        _ = GetBytes(s, bytes);
        return bytes;
    }
    public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex) =>
        GetBytes(s.AsSpan(charIndex, charCount), bytes.AsSpan(byteIndex));

    public override bool TryGetBytes(ReadOnlySpan<char> chars, Span<byte> bytes, out int bytesWritten)
    {
        try
        {
            bytesWritten = GetBytes(chars, bytes);
            return false;
        }
        catch
        {
            bytesWritten = 0;
            return false;
        }
    }

    #endregion

    #region "Рассчет" длин массивов

    public override unsafe int GetByteCount(char* chars, int count) => count;
    public override int GetByteCount(char[] chars) => chars.Length;
    public override int GetByteCount(ReadOnlySpan<char> chars) => chars.Length;
    public override int GetByteCount(string s) => s.Length;
    public override int GetByteCount(char[] chars, int index, int count) => count;

    public override int GetCharCount(byte[] bytes, int index, int count) => count;
    public override unsafe int GetCharCount(byte* bytes, int count) => count;
    public override int GetCharCount(ReadOnlySpan<byte> bytes) => bytes.Length;
    public override int GetCharCount(byte[] bytes) => bytes.Length;

    public override int GetMaxByteCount(int charCount) => charCount;
    public override int GetMaxCharCount(int byteCount) => byteCount;

    #endregion

}
