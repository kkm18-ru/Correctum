using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.Utils
{
    internal static class GridExtensions
    {
        public static void AddColumn(this Grid grid, GridUnitType type, int width = 1) =>
            grid.ColumnDefinitions.Add(new() { Width = new(width, type) });

        public static void AddRow(this Grid grid, GridUnitType type, int height = 1) =>
            grid.RowDefinitions.Add(new() { Height = new(height, type) });

        public static void AddChild(this Grid grid, UIElement element, int x = 0, int y = 0, int dx = 1, int dy = 1)
        {
            if (x > 0) Grid.SetColumn(element, x);
            if (y > 0) Grid.SetRow(element, y);
            if (dx > 1) Grid.SetColumnSpan(element, dx);
            if (dy > 1) Grid.SetRowSpan(element, dy);
            grid.Children.Add(element);
        }
    }
}
