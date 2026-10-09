using System.Globalization;
using System.Windows.Data;

namespace Correctum.Converters.Abstractions;

public abstract class BaseViewValueConverter<TSelf> : IValueConverter
    where TSelf: BaseViewValueConverter<TSelf>
{
    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new InvalidOperationException($"{typeof(TSelf).Name} не поддерживает режимы привязки TwoWay и OneWayToSource.");
}
