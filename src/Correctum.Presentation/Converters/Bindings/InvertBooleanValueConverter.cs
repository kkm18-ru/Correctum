using Avalonia.Data.Converters;
using System.Globalization;

namespace Correctum.Converters.Bindings;

public sealed class InvertBooleanValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value is not bool @bool) 
            throw new ArgumentException("Значение должно имеет тип данных System.Boolean.");
        return !@bool;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        Convert(value, targetType, parameter, culture);
}
