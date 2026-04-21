using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.Importers;

internal partial class Sqlite
{
    internal class View : ImporterView<View, Sqlite>
    {
        public View()
        {
            var content = new Grid();

            #region Столбцы
            content.AddColumn(GridUnitType.Auto);
            content.AddColumn(GridUnitType.Pixel, 5);
            content.AddColumn(GridUnitType.Star);
            content.AddColumn(GridUnitType.Pixel, 5);
            content.AddColumn(GridUnitType.Pixel, 20);
            #endregion

            #region Ввод пути к базе
            content.AddChild(new TextBlock { Text = "Путь к главной базе: " });
            var tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(Presenter.Path));
            content.AddChild(tb, 2);
            var btn = new Button { Content = "..." };
            btn.Click += (_, _) =>
            {
                var dialog = new OpenFileDialog();
                if (dialog.ShowDialog(Application.Current.MainWindow) == true)
                    Presenter.Path = dialog.FileName;
            };
            content.AddChild(btn, 4);
            #endregion

            Content = content;
        }
    }
}
