using RetailCorrector.Utils;
using RetailCorrector.Utils.Integrations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;

namespace RetailCorrector.UI.Windows;

internal class SettingsWindow : Types.DialogWindow
{
    public SettingsWindow()
    {
        Title = "Настройки";
        (Width, Height) = (450, 200);
        DataContext = Configuration.Instance;
        var tab = new TabControl();
        
        tab.Items.Add(CreateHttpTab());
        tab.Items.Add(CreateFptr());
        tab.Items.Add(CreateSqlTab());
        tab.Items.Add(CreateImportTab());
        tab.Items.Add(CreateExportTab());
        tab.Items.Add(CreateFptrTab());
        
        Content = tab;
    }

    private static TabItem CreateFptr()
    {
        var tab = new TabItem { Header = "Fptr" };
        var grid = new Grid();
        tab.Content = grid;

        grid.AddColumn(GridUnitType.Star);
        grid.AddColumn(GridUnitType.Pixel, 5);
        grid.AddColumn(GridUnitType.Star);

        grid.AddRow(GridUnitType.Auto);
        grid.AddRow(GridUnitType.Pixel, 5);
        grid.AddRow(GridUnitType.Auto);

        grid.AddChild(CreateText("Кол-во чеков до передачи в ОФД"));
        var nb = new IntegerUpDown { Minimum = 10, Maximum = 5000, Increment = 10 };
        nb.SetBinding(IntegerUpDown.ValueProperty, Bindings.FptrCountReceiptsBeforeDelay);
        grid.AddChild(nb, 2);

        grid.AddChild(CreateText("Время ожидания отправки чеков"), y: 2);
        nb = new IntegerUpDown { Minimum = 10, Maximum = 60 };
        nb.SetBinding(IntegerUpDown.ValueProperty, Bindings.FptrDelay);
        grid.AddChild(nb, 2, 2);

        return tab;
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
        nb.SetBinding(IntegerUpDown.ValueProperty, Bindings.SqliteLimit);
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
        nb.SetBinding(IntegerUpDown.ValueProperty, Bindings.HttpPageSize);
        grid.AddChild(nb, 2);

        grid.AddChild(CreateText("Интервал между HTTP-запросами"), y: 2);
        nb = new IntegerUpDown { Minimum = 1 };
        nb.SetBinding(IntegerUpDown.ValueProperty, Bindings.HttpDelay);
        grid.AddChild(nb, 2, 2);

        grid.AddChild(CreateText("Количество попыток запроса"), y: 4);
        nb = new IntegerUpDown { Minimum = 1 };
        nb.SetBinding(IntegerUpDown.ValueProperty, Bindings.HttpTriesCount);
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
        nb.SetBinding(IntegerUpDown.ValueProperty, Bindings.ExportFptrFileSize);
        grid.AddChild(nb, 2);

        return tab;
    }

    private static TabItem CreateImportTab()
    {
        var tab = new TabItem { Header = "Импорт" };
        var grid = new Grid();
        tab.Content = grid;

        grid.AddColumn(GridUnitType.Star);
        grid.AddColumn(GridUnitType.Pixel, 5);
        grid.AddColumn(GridUnitType.Star);

        grid.AddRow(GridUnitType.Auto);

        grid.AddChild(CreateText("Ключ интегратора Контур.ОФД"));
        var tb = new TextBox();
        tb.SetBinding(TextBox.TextProperty, Bindings.ImportKonturIntegrator);
        grid.AddChild(tb, 2);

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

file static class Bindings
{
    public readonly static Binding FptrDelay =
        new(nameof(Configuration.Fptr.Delay))
        { Source = Configuration.Instance.Fptr };

    public readonly static Binding FptrCountReceiptsBeforeDelay =
        new(nameof(Configuration.Fptr.CountReceiptsBeforeDelay))
        { Source = Configuration.Instance.Fptr };

    public readonly static Binding SqliteLimit =
        new(nameof(Configuration.Sqlite.Limit))
        { Source = Configuration.Instance.Sqlite };

    public readonly static Binding HttpTriesCount =
        new(nameof(Configuration.HttpClient.TriesCount))
        { Source = Configuration.Instance.HttpClient };

    public readonly static Binding HttpPageSize =
        new(nameof(Configuration.HttpClient.PageSize))
        { Source = Configuration.Instance.HttpClient };

    public readonly static Binding HttpDelay =
        new(nameof(Configuration.HttpClient.RequestDelay))
        { Source = Configuration.Instance.HttpClient };

    public readonly static Binding ExportFptrFileSize =
        new(nameof(Configuration.Exporters.Fptr.DocsPerFile))
        { Source = Configuration.Instance.Exporters.Fptr };

    public readonly static Binding ImportKonturIntegrator =
        new(nameof(Configuration.Importes.KonturIntegratorKey))
        { Source = Configuration.Instance.Importes };
}