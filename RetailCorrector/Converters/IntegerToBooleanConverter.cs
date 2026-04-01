using System.Globalization;
using System.Windows.Data;

namespace RetailCorrector.Converters;

public sealed class IntegerToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not (long or ulong or int or uint or short or ushort or sbyte or byte or float or double or decimal))
            throw new FormatException($"Ожидается численное значение (Получено {value.GetType().Name})");
        return value != (object)0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value is not bool @bool)
            throw new FormatException($"Ожидается значение типа Boolean (Получено {value.GetType().Name})");
        return @bool ? 1 : 0;
    }
}
