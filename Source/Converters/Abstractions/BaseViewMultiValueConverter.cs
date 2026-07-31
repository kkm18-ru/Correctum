using System.Globalization;
using System.Windows.Data;

namespace Correctum.Converters.Converters;

public abstract class BaseViewMultiValueConverter<TSelf> : IMultiValueConverter
    where TSelf : BaseViewMultiValueConverter<TSelf>
{
    public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new InvalidOperationException($"{typeof(TSelf).Name} не поддерживает режимы привязки TwoWay и OneWayToSource.");
}
