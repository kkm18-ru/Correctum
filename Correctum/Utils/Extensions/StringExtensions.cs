namespace Correctum.Utils.Extensions;

internal static class StringExtensions
{
    extension(string)
    {
        public static string Conjugate(long digit, string single, string few, string many)
        {
            digit = Math.Abs(digit);
            if (digit > 10 && digit < 20) return many;
            return (digit % 10) switch
            {
                1 => single,
                >= 2 and <= 4 => few,
                _ => many,
            };
        }

        public static string Conjugate(long digit, string single, string many)
        {
            digit = Math.Abs(digit);
            return digit % 10 == 1 && digit != 11 ? single : many;
        }
    }
}
