using System.Globalization;
using System.Windows.Data;

namespace Correctum.Converters;

public sealed class MultiBooleanConverter: IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) =>
        values.All(v => v is bool @bool && @bool);

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}