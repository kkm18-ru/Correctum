#if DEBUG
using Xceed.Wpf.Toolkit;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace RetailCorrector.Parsers
{
    internal partial class Mock
    {
        internal class View : ParserView<View, Mock>
        {
            public View()
            {
                DataContext = new Mock();
                var grid = new Grid();
                Content = grid;

                grid.AddColumn(GridUnitType.Star);
                grid.AddColumn(GridUnitType.Pixel, 5);
                grid.AddColumn(GridUnitType.Star);

                grid.AddRow(GridUnitType.Pixel, 20);
                //grid.AddRow(GridUnitType.Pixel, 5);

                grid.AddChild(GenerateLabel("Количество чеков"));
                grid.AddChild(GenerateRangeUI(), 2);
            }

            private static Canvas GenerateRangeUI()
            {
                var panel = new Canvas();
                var converter = new RangeWidthConverter();

                var min = new IntegerUpDown { Minimum = 1 };
                min.SetBinding(IntegerUpDown.ValueProperty, nameof(MinCountDocs));
                min.SetBinding(IntegerUpDown.MaximumProperty, nameof(MaxCountDocs));
                min.SetBinding(IntegerUpDown.WidthProperty, new Binding(nameof(ActualWidth)) { Source = panel, Converter = converter });
                Canvas.SetLeft(min, 0);
                panel.Children.Add(min);

                var max = new IntegerUpDown { Maximum = 5000 };
                max.SetBinding(IntegerUpDown.ValueProperty, nameof(MaxCountDocs));
                max.SetBinding(IntegerUpDown.MinimumProperty, nameof(MinCountDocs));
                max.SetBinding(IntegerUpDown.WidthProperty, new Binding(nameof(ActualWidth)) { Source = panel, Converter = converter });
                Canvas.SetRight(max, 0);
                panel.Children.Add(max);

                return panel;
            }

            private static TextBlock GenerateLabel(string text) => new()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Text = $"{text}: ",
            };
        }
    }

    file sealed class RangeWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is not double v || v <= 5 ? 0 : (v - 5) / 2;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 0;
    }
}
#endif
