using System.Globalization;
using System.Windows.Data;

namespace RetailCorrector.Converters;

public sealed class IntegerAsDoubleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string text || !double.TryParse(text, out var @double)) throw new FormatException();
        var shift = (int?)parameter ?? 0;
        return (ulong)Math.Round(@double * Math.Pow(10, shift), shift);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var @int = (int?)value ?? 0;
        var shift = (int?)parameter ?? 0;
        return Math.Round(@int / Math.Pow(10, shift), shift);
    }
}