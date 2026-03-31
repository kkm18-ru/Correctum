using System.Globalization;
using System.Windows.Data;

namespace RetailCorrector.Converters;

public sealed class InvertBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool @bool) 
            throw new FormatException($"Ожидается значение типа Boolean (Получено {value.GetType().Name})");
        return !@bool;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        Convert(value, targetType, parameter, culture);
}
