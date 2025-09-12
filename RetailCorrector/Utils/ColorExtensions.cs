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
    }
}
