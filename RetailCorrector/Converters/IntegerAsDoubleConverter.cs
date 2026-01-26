using System.Globalization;
using System.Windows.Data;

namespace RetailCorrector.Converters;

public sealed class IntegerAsDoubleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var @int = (ulong?)value ?? 0;
        var shift = (int?)parameter ?? 0;
        return Math.Round(@int / Math.Pow(10, shift), shift);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var @double = (double?)value ?? 0;
        var shift = (int?)parameter ?? 0;
        return (ulong)Math.Round(@double * Math.Pow(10, shift), shift);
    }
}