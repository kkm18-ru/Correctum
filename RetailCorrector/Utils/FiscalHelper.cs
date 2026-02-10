using System.Buffers.Text;
using System.Text;

namespace RetailCorrector.Utils;

public static class FiscalHelper
{
    public static uint ToDecimalSign(string? sign)
    {
        if (sign?.Length != 8) return 0;
        Span<byte> utf8Bytes = stackalloc byte[8];
        Encoding.UTF8.GetBytes(sign, utf8Bytes);
        Span<byte> bytes = stackalloc byte[6];
        Base64.DecodeFromUtf8(utf8Bytes, bytes, out _, out _);
        bytes.Reverse();
        return BitConverter.ToUInt32(bytes);
    }
}