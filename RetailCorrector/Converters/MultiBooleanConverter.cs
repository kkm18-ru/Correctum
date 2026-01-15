using System.Globalization;
using System.Windows.Data;

namespace RetailCorrector.Converters;

public sealed class MultiBooleanConverter: IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        foreach (var value in values)
        {
            if (value is not bool @bool)
                throw new ArgumentException();
            if (!@bool) return false;
        }

        return true;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}