namespace RetailCorrector.Tests;

public static class FiscalHelper
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    
    public static uint ToDecimalSign(string? sign)
    {
        var digitSign = 0U;
        if (sign?.Length == 8)
        {
            Span<byte> bytes = stackalloc byte[6];
            Convert.TryFromBase64String(sign, bytes, out _);
            bytes.Reverse();
            digitSign = BitConverter.ToUInt32(bytes);
        }
        Logger.Debug("{TextValue} преобразовано в {DigitValue}", sign, digitSign);
        return digitSign;
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

        var textSign = new string(chars);
        Logger.Debug("{DigitValue} преобразовано в {TextValue}", sign, textSign);
        return textSign;
    }
}