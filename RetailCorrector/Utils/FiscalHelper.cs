namespace RetailCorrector.Utils;

public static class FiscalHelper
{
    public static uint ToDecimalSign(string? sign)
    {
        if (sign?.Length != 8) return 0;
        Span<byte> bytes = stackalloc byte[6];
        Convert.TryFromBase64String(sign, bytes, out _);
        bytes.Reverse();
        return BitConverter.ToUInt32(bytes);
    }
}