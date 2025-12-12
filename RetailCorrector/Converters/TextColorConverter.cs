using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RetailCorrector.Converters;

public class TextColorConverter : IValueConverter
{
    private readonly Color _errColor = Colors.Pink;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not SolidColorBrush scb) return _errColor;
        var k = 0.299 * scb.Color.R + 0.587 * scb.Color.G + 0.114 * scb.Color.B;
        var fg = k < 100 ? Colors.White : Colors.Black;
        return new SolidColorBrush(fg);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => _errColor;
}
