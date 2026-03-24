using Avalonia.Data.Converters;
using System.Globalization;

namespace Correctum.Converters.Bindings;

public sealed class CBSelectedIndexAsBooleanConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) // VM2View
    {
        if (value is not bool v) throw new ArgumentException(null, nameof(value));
        return v ? 1 : 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) // View2VM
    {
        if (value is not int v) throw new ArgumentException(null, nameof(value));
        return v != 0;
    }
}
