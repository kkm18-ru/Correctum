namespace RetailCorrector.Utils
{
    internal static class LocaleHelper
    {
        public static string ConjugateWord(long digit, string single, string few, string many)
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
        public static string ConjugateWord(long digit, string single, string many)
        {
            digit = Math.Abs(digit);
            return digit % 10 == 1 && digit != 11 ? single : many;
        }
    }
}
