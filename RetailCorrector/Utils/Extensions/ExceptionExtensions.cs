namespace RetailCorrector.Utils.Extensions;

public static class ExceptionExtensions
{
    extension(ArgumentException)
    {
        public static void ThrowIfNotUInteger(string str)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(str);
            if (!ulong.TryParse(str, out _))
                throw new ArgumentException("Ввод не является целочисленным без знака.", nameof(str));
        }

        public static void ThrowIfNotInteger(string str)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(str);
            if (!long.TryParse(str, out _))
                throw new ArgumentException("Ввод не является целочисленным.", nameof(str));
        }
    }
}
