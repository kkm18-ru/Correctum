using System.Globalization;
using System.Windows.Data;

namespace RetailCorrector.Converters;

public sealed class InvertBooleanConverter : IValueConverter
{
    public static readonly InvertBooleanConverter Instance = new();
    
    private InvertBooleanConverter() { }
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => targetType == typeof(bool) && value is bool @bool ? !@bool : null;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => targetType == typeof(bool) && value is bool @bool ? !@bool : null;
}