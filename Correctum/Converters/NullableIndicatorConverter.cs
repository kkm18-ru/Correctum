using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Correctum.Converters;

public sealed class NullableIndicatorConverter : IValueConverter
{
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new InvalidOperationException("Конвертер не поддерживает режимы привязки TwoWay и OneWayToSource.");

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null) return Brushes.Black;
        return parameter is string colorId ? new SolidColorBrush(Color.FromHex(colorId)) : null!;
    }
}
