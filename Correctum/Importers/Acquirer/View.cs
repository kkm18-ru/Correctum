using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Correctum.Core.Enums;

namespace Correctum.Importers;

partial class Acquirer
{
    internal class View : ImporterView<View, Acquirer>
    {
        public View()
        {
            var grid = new Grid();
            Content = grid;
            
            grid.AddColumn(GridUnitType.Auto);
            grid.AddColumn(GridUnitType.Pixel, 5);
            grid.AddColumn(GridUnitType.Star);
            grid.AddColumn(GridUnitType.Pixel, 5);
            grid.AddColumn(GridUnitType.Pixel, 20);
            
            grid.AddRow(GridUnitType.Auto);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto);
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto);

            grid.AddChild(new TextBlock{Text = "Путь к файлу:"});
            var tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(FilePath));
            grid.AddChild(tb, 2);
            var open = new OpenFileDialog
            {
                Filter = "Операции по эквайрингу (*.csv)|*.csv",
                CheckFileExists = true,
                Multiselect = false,
                ShowReadOnly = true
            };
            var btn = new Button{Content = "..."};
            btn.Click += (s, e) =>
            {
                var ok = open.ShowDialog();
                if (ok == true)
                    Presenter.FilePath = open.FileName;
            };
            grid.AddChild(btn, 4);
            
            grid.AddChild(new TextBlock{ Text = "Разделение:" },y:2);
            var cb = new ComboBox { ItemsSource = new[] { ';', ',' } };
            cb.SetBinding(ComboBox.SelectedValueProperty, nameof(Separator));
            grid.AddChild(cb, 2, 2, 3);
            
            var check = new CheckBox { Content = "Имеются заголовки?" };
            check.SetBinding(CheckBox.IsCheckedProperty, nameof(HasHeaders));
            grid.AddChild(check, y: 4, dx: 5);

            btn = new Button{Content = "Прочитать первую строку"};
            btn.Click += (_, _) =>
            {
                var line = File.ReadLine(Presenter.FilePath);
                var columns = line.Split(Presenter.Separator);
                Presenter.Headers.Clear();
                for (var i = 0; i < columns.Length; i++)
                {
                    var header = new StringBuilder(Helper.GenerateColumnName((uint)i + 1));
                    if (Presenter.HasHeaders)
                        header.Append($" ({columns[i].Trim('"')})");
                    Presenter.Headers.Add(header.ToString());
                }
            };
            grid.AddChild(btn, 2, 6, 3);

            grid.AddChild(new TextBlock { Text = "Столбец операции" }, y: 8);
            cb = new ComboBox();
            cb.SetBinding(ComboBox.ItemsSourceProperty, nameof(Headers));
            cb.SetBinding(ComboBox.SelectedIndexProperty, nameof(OperationReceipt));
            grid.AddChild(cb, 2, 8, 3);

            grid.AddChild(new TextBlock { Text = "Столбец даты" }, y: 10);
            cb = new ComboBox();
            cb.SetBinding(ComboBox.ItemsSourceProperty, nameof(Headers));
            cb.SetBinding(ComboBox.SelectedIndexProperty, nameof(DateReceipt));
            grid.AddChild(cb, 2, 10, 3);
            
            grid.AddChild(new TextBlock { Text = "Столбец итог" }, y: 12);
            cb = new ComboBox();
            cb.SetBinding(ComboBox.ItemsSourceProperty, nameof(Headers));
            cb.SetBinding(ComboBox.SelectedIndexProperty, nameof(TotalReceipt));
            grid.AddChild(cb, 2, 12, 3);
            
            grid.AddChild(new TextBlock { Text = "Формат даты" }, y: 14);
            tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(DateFormat));
            grid.AddChild(tb, 2, 14, 3);
            
            grid.AddChild(new TextBlock { Text = "Наименование позиции" }, y: 16);
            tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(ItemName));
            grid.AddChild(tb, 2, 16, 3);
            
            grid.AddChild(new TextBlock { Text = "НДС" }, y: 18);
            cb = new ComboBox { ItemsSource = Enum.GetValues<NDSRate>()};
            cb.SetBinding(ComboBox.SelectedValueProperty, nameof(ItemVat));
            grid.AddChild(cb, 2, 18, 3);
            
            grid.AddChild(new TextBlock { Text = "Паттерн прихода" }, y: 20);
            tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(IncomePattern));
            grid.AddChild(tb, 2, 20, 3);
            
            grid.AddChild(new TextBlock { Text = "Паттерн возврата" }, y: 22);
            tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(RefundPattern));
            grid.AddChild(tb, 2, 22, 3);
        }
    }
}

file static class Helper
{
    public static string GenerateColumnName(uint value)
    {
        if (value == 0) return "";
        var power = value switch
        {
            1 => (int)value--,
            > 1 => (int)Math.Ceiling(Math.Log(value--, 26)),
            _ => throw new ArgumentException(null, nameof(value))
        };
        var builder = new StringBuilder(power);
        for (var i = power - 1; i >= 0; i--)
            builder.Append((char)((i == 0 ? 'A' : '@') + value / Math.Pow(26, i) % 26));
        return builder.ToString();
    }
} 