namespace RetailCorrector.Data;

public readonly struct EnumLocale<T>(T key, string locale) 
    where T : struct, Enum
{
    public T Value { get; } = key;
    public string Locale { get; } = locale;
}
