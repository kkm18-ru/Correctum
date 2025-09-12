using RetailCorrector.Utils;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.Actions
{
    internal partial class ExportReceipts
    {
        internal class View : ActionView<View, ExportReceipts>
        {
            private readonly Grid content = new();
            public View()
            {
                content.RowDefinitions.Clear();
                content.Children.Clear();
                Content = content;

                content.AddColumn(GridUnitType.Pixel, 115);
                content.AddColumn(GridUnitType.Pixel, 5);
                content.AddColumn(GridUnitType.Pixel, 180);

                content.AddRow(GridUnitType.Auto);

                content.AddChild(new TextBlock()
                {
                    Text = "Экспортировать как",
                    VerticalAlignment = VerticalAlignment.Center
                });
                content.AddChild(DrawSelector(), 2);
                Context.Exporter.InitVisual(content);
            }

            private ComboBox DrawSelector()
            {
                var cb = new ComboBox
                {
                    ItemsSource = DisplayNames,
                };
                cb.SetBinding(ComboBox.SelectedIndexProperty, nameof(Context.ExporterId));
                cb.SelectionChanged += (_, _) =>
                {
                    if (content.RowDefinitions.Count > 1)
                    {
                        content.RowDefinitions.RemoveRange(1, content.RowDefinitions.Count - 1);
                        content.Children.RemoveRange(2, content.Children.Count - 2);
                    }
                    Context.Exporter.InitVisual(content);
                };
                return cb;
            }
        }
    }
}