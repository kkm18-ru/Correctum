using RetailCorrector.Exports;
using RetailCorrector.Utils;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.Actions
{
    internal partial class ExportReceipts
    {
        internal class View : ActionView<View, ExportReceipts>
        {
            public View()
            {
                (Height, Width) = (22, 300);
                var grid = new Grid();
                Content = grid;

                grid.AddColumn(GridUnitType.Pixel, 115);
                grid.AddColumn(GridUnitType.Pixel, 5);
                grid.AddColumn(GridUnitType.Star);

                grid.AddChild(new TextBlock()
                {
                    Text = "Экспортировать как",
                    VerticalAlignment = VerticalAlignment.Center
                });
                grid.AddChild(DrawSelector(), 2);
            }

            private ComboBox DrawSelector()
            {
                var cb = new ComboBox
                {
                    ItemsSource = ExportReceipts.Exporters,
                    DisplayMemberPath = nameof(IExport.Name)
                };
                cb.SetBinding(ComboBox.SelectedValueProperty, nameof(Context.Exporter));
                return cb;
            }
        }
    }
}