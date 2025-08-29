using RetailCorrector.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.Actions.ExportReceipts
{
    internal class ExportReceiptsView : ActionView<ExportReceiptsView, ExportReceiptsPresenter>
    {
        private readonly string[] Items = [ "SQLite", "Fptr-скрипт" ];

        public ExportReceiptsView()
        {
            (Height, Width) = (22, 220);
            var grid = new Grid();
            Content = grid;

            grid.AddColumn(GridUnitType.Star, 6);
            grid.AddColumn(GridUnitType.Pixel, 5);
            grid.AddColumn(GridUnitType.Star, 5);

            grid.AddChild(new TextBlock()
            {
                Text = "Экспортировать как",
                VerticalAlignment = VerticalAlignment.Center
            });
            grid.AddChild(DrawSelector(), 2);
        }

        private ComboBox DrawSelector()
        {
            var cb = new ComboBox { ItemsSource = Items };
            cb.SetBinding(ComboBox.SelectedIndexProperty, nameof(Context.Type));
            return cb;
        }
    }
}
