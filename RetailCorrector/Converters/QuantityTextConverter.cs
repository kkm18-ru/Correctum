using Correctum.Core.Enums;
using RetailCorrector.Utils;
using System.Globalization;
using System.Windows.Data;

namespace RetailCorrector.Converters
{
    public sealed class QuantityTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2) throw new ArgumentException(null, nameof(values));
            if (values[0] is decimal quantity && values[1] is UnitOfMeasure unit)
                return $"{quantity:F3} {EnumLocales.PositionUnits.GetLocale(unit)}";
            else return "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
