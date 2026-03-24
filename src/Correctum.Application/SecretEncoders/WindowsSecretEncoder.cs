using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace Correctum.SecretEncoders;

[SupportedOSPlatform("windows")]
public class WindowsSecretEncoder : SecretEncoder
{
    protected override string InnerEncode(string raw, Encoding encoding)
    {
        try
        {
            Span<byte> rawBytes = stackalloc byte[encoding.GetMaxByteCount(raw.Length)];
            var length = encoding.GetBytes(raw, rawBytes);
            Span<byte> bytes = stackalloc byte[2048];
            if (!ProtectedData.TryProtect(rawBytes[..length], DataProtectionScope.LocalMachine, bytes, out length))
                return string.Empty;
            Span<char> chars = stackalloc char[length / 3 * 5];
            Convert.TryToBase64Chars(bytes[..length], chars, out _);
            var encoded = new string(chars.TrimEnd('\0'));
            return encoded;
        }
        catch (Exception e)
        {
            return string.Empty;
        }
    }

    protected override string InnerDecode(string encoded, Encoding encoding)
    {
        try
        {
            Span<byte> encodedBytes = stackalloc byte[encoding.GetMaxByteCount(encoded.Length)];
            Convert.TryFromBase64String(encoded, encodedBytes, out var len);
            Span<byte> bytes = stackalloc byte[2048];
            if (!ProtectedData.TryUnprotect(encodedBytes[..len], DataProtectionScope.LocalMachine, bytes, out len))
                return string.Empty;
            Span<char> chars = stackalloc char[encoding.GetMaxCharCount(len)];
            var rawSecret = new string(chars.TrimEnd('\0'));
            return rawSecret;
        }
        catch (Exception e)
        {
            return string.Empty;
        }
    }
}