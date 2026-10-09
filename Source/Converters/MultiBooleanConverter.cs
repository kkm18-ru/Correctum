using Correctum.Converters.Abstractions;
using System.Globalization;

namespace Correctum.Converters;

public sealed class MultiBooleanConverter: BaseViewMultiValueConverter<MultiBooleanConverter>
{
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) =>
        values.All(v => v is bool @bool && @bool);
}