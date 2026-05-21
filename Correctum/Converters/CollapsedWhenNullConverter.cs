using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Correctum.Converters
{
    internal class CollapsedWhenNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is null ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null!;
    }
}
