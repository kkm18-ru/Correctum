using System.Security.Cryptography;
using System.Text;

namespace RetailCorrector.Utils;

public static class SecretHelper
{
    public static string EncodeSecret(string rawSecret, Encoding? encoding = null)
    {
        if (string.IsNullOrWhiteSpace(rawSecret)) return string.Empty;
        encoding ??= Encoding.UTF8;
        
        var length = encoding.GetMaxByteCount(rawSecret.Length);
        Span<byte> rawBytes = stackalloc byte[length];
        length = encoding.GetBytes(rawSecret, rawBytes);
        Span<byte> bytes = stackalloc byte[2048];
        ProtectedData.TryProtect(rawBytes[..length], DataProtectionScope.LocalMachine, bytes, out length);
        Span<char> chars = stackalloc char[length / 3 * 5];
        Convert.TryToBase64Chars(bytes[..length], chars, out _);
        return new string(chars.TrimEnd('\0'));
    }

    public static string DecodeSecret(string encoded, Encoding? encoding = null)
    {
        if (string.IsNullOrWhiteSpace(encoded)) return string.Empty;
        encoding ??= Encoding.UTF8;
        
        var length = encoding.GetMaxByteCount(encoded.Length);
        Span<byte> encodedBytes = stackalloc byte[length];
        Convert.TryFromBase64String(encoded, encodedBytes, out length);
        Span<byte> bytes = stackalloc byte[2048];
        ProtectedData.TryUnprotect(encodedBytes[..length], DataProtectionScope.LocalMachine, bytes, out length);
        Span<char> chars = stackalloc char[encoding.GetMaxCharCount(length)];
        encoding.GetChars(bytes[..length], chars);
        return new string(chars.TrimEnd('\0'));
    }
}