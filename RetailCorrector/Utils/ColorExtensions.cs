using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RetailCorrector.Utils
{
    internal static class ColorExtensions
    {
        public static Color Subtraction(this Color color, byte r, byte g, byte b)
        {
            var _r = color.R >= r ? color.R - r : color.R;
            var _g = color.G >= g ? color.G - g : color.G;
            var _b = color.B >= b ? color.B - b : color.B;
            return Color.FromRgb((byte)_r, (byte)_g, (byte)_b);
        }
        public static Color AddOrSubtraction(this Color color, byte r, byte g, byte b)
        {
            var _r = color.R <= byte.MaxValue - r ? color.R + r : color.R - r;
            var _g = color.G <= byte.MaxValue - g ? color.G + g : color.G - g;
            var _b = color.B <= byte.MaxValue - b ? color.B + b : color.B - b;
            return Color.FromRgb((byte)_r, (byte)_g, (byte)_b);
        }
        private static Color WhiteOrBlack(this Color bg)
        {
            var k = 0.299 * bg.R + 0.587 * bg.G + 0.114 * bg.B;
            return k < 100 ? Colors.White : Colors.Black;
        }

        public class TextColorConverter : IValueConverter
        {
            private readonly Color _errColor = Colors.Pink;

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is not SolidColorBrush scb) return _errColor;
                return new SolidColorBrush(scb.Color.WhiteOrBlack());
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => _errColor;
        }
    }
}
