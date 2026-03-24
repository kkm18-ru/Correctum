using Avalonia;
using Avalonia.Data.Converters;
using System.Globalization;

namespace Correctum.Converters.Bindings;

internal class UnionBooleansConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        foreach(var value in values)
        {
            if (value == AvaloniaProperty.UnsetValue)
                return false;
            if (value is not bool @bool) 
                throw new ArgumentException("Один из значений не является логическим (Boolean)");
            if (!@bool) return false;
        }
        return true;
    }
}
