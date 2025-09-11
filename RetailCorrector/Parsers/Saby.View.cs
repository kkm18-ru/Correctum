using RetailCorrector.UI.Controls;
using RetailCorrector.Utils;
using RetailCorrector.Utils.ValidationRules;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.Parsers;

internal partial class Saby
{
    internal class View : ParserView<View, Saby>
    {
        public View()
        {
            DataContext = new Saby();
            var grid = new Grid();
            Content = grid;

            grid.AddColumn(GridUnitType.Star);
            grid.AddColumn(GridUnitType.Pixel, 5);
            grid.AddColumn(GridUnitType.Star);

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

            grid.AddChild(CreateLabel("Логин"));
            var tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(Login));
            grid.AddChild(tb, 2);

            grid.AddChild(CreateLabel("Пароль"), y: 2);
            tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(Password));
            grid.AddChild(tb, 2, 2);

            grid.AddChild(CreateLabel("Начало сканирования"), y: 4);
            var dp = new DatePicker();
            dp.SetBinding(DatePicker.SelectedDateProperty, nameof(FromDate));
            grid.AddChild(dp, 2, 4);

            grid.AddChild(CreateLabel("Конец сканирования"), y: 6);
            dp = new DatePicker();
            dp.SetBinding(DatePicker.SelectedDateProperty, nameof(ToDate));
            grid.AddChild(dp, 2, 6);

            grid.AddChild(CreateLabel("ИНН организации"), y: 8);
            tb = new NotifyTextBox();
            tb.SetBinding(TextBox.TextProperty, new ValidatedBinding(nameof(INN), InnValidationRule.Instance));
            grid.AddChild(tb, 2, 8);

            grid.AddChild(CreateLabel("Регистрационный номер ККТ"), y: 10);
            tb = new NotifyTextBox();
            tb.SetBinding(TextBox.TextProperty, new ValidatedBinding(nameof(Device), DeviceIdValidationRule.Instance)); 
            grid.AddChild(tb, 2, 10);

            grid.AddChild(CreateLabel("Заводской номер ФН"), y: 12);
            tb = new NotifyTextBox();
            tb.SetBinding(TextBox.TextProperty, new ValidatedBinding(nameof(Storage), DeviceIdValidationRule.Instance)); 
            grid.AddChild(tb, 2, 12);
        }

        private static TextBlock CreateLabel(string text) => new()
        {
            Text = $"{text}: ",
            VerticalAlignment = VerticalAlignment.Center
        };
    }
}