using Correctum.Converters.Converters;
using System.Globalization;
using System.Windows.Media;

namespace Correctum.Converters;

public sealed class NullableIndicatorConverter : BaseViewValueConverter<NullableIndicatorConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null) return Brushes.Black;
        return parameter is string colorId ? new SolidColorBrush(Color.FromHex(colorId)) : null!;
    }
}
