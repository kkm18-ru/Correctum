using RetailCorrector.Utils;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace RetailCorrector.UI.Windows
{
    internal class SettingsWindow : Types.DialogWindow
    {
        public SettingsWindow()
        {
            Title = "Настройки";
            (Width, Height) = (450, 200);
            DataContext = Configuration.Instance;
            var tab = new TabControl();
            
            tab.Items.Add(CreateHttpTab());
            tab.Items.Add(CreateSqlTab());
            tab.Items.Add(CreateExportTab());
            tab.Items.Add(CreateFptrTab());
            
            Content = tab;
        }

        private static TabItem CreateSqlTab()
        {
            var tab = new TabItem { Header = "SQL" };
            var grid = new Grid();
            tab.Content = grid;

            grid.AddColumn(GridUnitType.Star);
            grid.AddColumn(GridUnitType.Pixel, 5);
            grid.AddColumn(GridUnitType.Star);

            grid.AddRow(GridUnitType.Auto);

            grid.AddChild(CreateText("Количество документов в запросе"));
            var nb = new IntegerUpDown { Minimum = 1 };
            nb.SetBinding(IntegerUpDown.ValueProperty, nameof(Configuration.SqlLimit));
            grid.AddChild(nb, 2);

            return tab;
        }

        private static TabItem CreateHttpTab()
        {
            var tab = new TabItem { Header = "HTTP" };
            var grid = new Grid();
            tab.Content = grid;

            grid.AddColumn(GridUnitType.Star);
            grid.AddColumn(GridUnitType.Pixel, 5);
            grid.AddColumn(GridUnitType.Star);

            grid.AddRow(GridUnitType.Auto);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto);

            grid.AddChild(CreateText("Количество документов в ответе"));
            var nb = new IntegerUpDown { Minimum = 1 };
            nb.SetBinding(IntegerUpDown.ValueProperty, nameof(Configuration.HttpPageSize));
            grid.AddChild(nb, 2);

            grid.AddChild(CreateText("Интервал между HTTP-запросами"), y: 2);
            nb = new IntegerUpDown { Minimum = 1 };
            nb.SetBinding(IntegerUpDown.ValueProperty, nameof(Configuration.HttpRequestDelay));
            grid.AddChild(nb, 2, 2);

            grid.AddChild(CreateText("Количество попыток запроса"), y: 4);
            nb = new IntegerUpDown { Minimum = 1 };
            nb.SetBinding(IntegerUpDown.ValueProperty, nameof(Configuration.HttpTriesCount));
            grid.AddChild(nb, 2, 4);

            return tab;
        }

        private static TabItem CreateExportTab()
        {
            var tab = new TabItem { Header = "Экспорт" };
            var grid = new Grid();
            tab.Content = grid;

            grid.AddColumn(GridUnitType.Star);
            grid.AddColumn(GridUnitType.Pixel, 5);
            grid.AddColumn(GridUnitType.Star);

            grid.AddRow(GridUnitType.Auto);

            grid.AddChild(CreateText("Количество документов в скрипте"));
            var nb = new IntegerUpDown { Minimum = 1, Maximum = 150 };
            nb.SetBinding(IntegerUpDown.ValueProperty, nameof(Configuration.FptrExportDocsPerFile));
            grid.AddChild(nb, 2);

            return tab;
        }

        private static TabItem CreateFptrTab()
        {
            var tab = new TabItem { Header = "Atol-драйвер" };
            var grid = new Grid();
            tab.Content = grid;

            grid.AddColumn(GridUnitType.Star);
            grid.AddColumn(GridUnitType.Pixel, 5);
            grid.AddColumn(GridUnitType.Star);

            grid.AddRow(GridUnitType.Auto);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto);

            grid.AddChild(CreateText("Максимальное время ожидания перезагрузки ККТ (в мс)"));
            var nb = new IntegerUpDown { Minimum = 1, Maximum = 10000 };
            nb.SetBinding(IntegerUpDown.ValueProperty, nameof(Configuration.FptrWaitRebootTimeout));
            grid.AddChild(nb, 2);

            grid.AddChild(CreateText("Транспортный порт Atol (EoU)"), y: 2);
            var cb = new ComboBox
            {
                ItemsSource = EoU.GetAvailablePorts(),
                Text = $"COM{EoU.CurrentPort}",
                IsEditable = true,
            };
            cb.IsEnabled = cb.Items.Count > 0;
            cb.SelectionChanged += (s, e) => EoU.CurrentPort = byte.Parse(((string)cb.SelectedValue)[3..]);
            grid.AddChild(cb, 2, 2);

            return tab;
        }

        private static TextBlock CreateText(string text) => new()
        {
            Text = text,
            VerticalAlignment = VerticalAlignment.Center,
        };
    }
}
