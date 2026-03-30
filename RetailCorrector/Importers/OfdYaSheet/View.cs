using Microsoft.Win32;
using System.Windows.Controls;

namespace RetailCorrector.Importers;

partial class OfdYaSheet
{
    internal class View : ImporterView<View, OfdYaSheet>
    {
        public View()
        {
            var grid = new Grid();
            grid.AddColumn(System.Windows.GridUnitType.Auto);
            grid.AddColumn(System.Windows.GridUnitType.Pixel, 5);
            grid.AddColumn(System.Windows.GridUnitType.Star);
            grid.AddColumn(System.Windows.GridUnitType.Pixel, 5);
            grid.AddColumn(System.Windows.GridUnitType.Pixel, 22);

            var lbl = new TextBlock { Text = "Путь к файлу:" };
            Grid.SetColumn(lbl, 0);
            grid.AddChild(lbl);

            var tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(Path));
            grid.AddChild(tb, 2);

            var btn = new Button();
            btn.Click += (_, _) =>
            {
                var fd = new OpenFileDialog
                {
                    Filter = "Excel-таблица|*.xlsx",
                    Multiselect = false
                };
                if(fd.ShowDialog() ?? false)
                    Presenter.Path = fd.FileName;
            };
            grid.AddChild(btn, 4);

            Content = grid;
        }
    }
}
