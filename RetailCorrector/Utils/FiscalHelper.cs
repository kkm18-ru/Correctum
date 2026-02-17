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

    public static string ToStringSign(uint sign)
    {
        Span<byte> bytes = stackalloc byte[6];
        BitConverter.TryWriteBytes(bytes, sign);
        bytes[4] = 0b00000100; 
        bytes[5] = 0b00110001; 
        bytes.Reverse();
        
        Span<char> chars = stackalloc char[8];
        Convert.TryToBase64Chars(bytes, chars, out _);

        return new string(chars);
    }
}