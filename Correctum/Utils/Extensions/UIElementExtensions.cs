using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Correctum.Utils.Extensions;

internal static class UIElementExtensions
{
    extension(Grid grid)
    {
        public void AddColumn(GridUnitType type, int width = 1) =>
            grid.ColumnDefinitions.Add(new() { Width = new(width, type) });

        public void AddRow(GridUnitType type, int height = 1) =>
            grid.RowDefinitions.Add(new() { Height = new(height, type) });

        public void AddChild(UIElement element, int x = 0, int y = 0, int dx = 1, int dy = 1)
        {
            if (x > 0) Grid.SetColumn(element, x);
            if (y > 0) Grid.SetRow(element, y);
            if (dx > 1) Grid.SetColumnSpan(element, dx);
            if (dy > 1) Grid.SetRowSpan(element, dy);
            grid.Children.Add(element);
        }
    }

    extension(Color color)
    {
        public Color Subtraction(byte r, byte g, byte b)
        {
            var _r = color.R >= r ? color.R - r : color.R;
            var _g = color.G >= g ? color.G - g : color.G;
            var _b = color.B >= b ? color.B - b : color.B;
            return Color.FromRgb((byte)_r, (byte)_g, (byte)_b);
        }

        public Color AddOrSubtraction(byte r, byte g, byte b)
        {
            var _r = color.R <= byte.MaxValue - r ? color.R + r : color.R - r;
            var _g = color.G <= byte.MaxValue - g ? color.G + g : color.G - g;
            var _b = color.B <= byte.MaxValue - b ? color.B + b : color.B - b;
            return Color.FromRgb((byte)_r, (byte)_g, (byte)_b);
        }
    }
}
