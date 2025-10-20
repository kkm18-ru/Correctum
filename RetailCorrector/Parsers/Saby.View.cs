using RetailCorrector.UI.Controls;
using RetailCorrector.Utils;
using RetailCorrector.Utils.ValidationRules;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

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

            grid.AddChild(CreateLabel("�����"));
            var tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(Login));
            grid.AddChild(tb, 2);

            grid.AddChild(CreateLabel("������"), y: 2);
            tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(Password));
            grid.AddChild(tb, 2, 2);

            grid.AddChild(CreateLabel("������ ������������"), y: 4);
            var dp = new DateTimePicker
            {
                Minimum = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                Format = DateTimeFormat.ShortDate,
                CultureInfo = CultureInfo.GetCultureInfo("ru-RU"),
                TimePickerVisibility = Visibility.Collapsed,
                ShowButtonSpinner = false,
                ShowDropDownButton = false
            };
            dp.SetBinding(DateTimePicker.ValueProperty, nameof(FromDate));
            dp.SetBinding(DateTimePicker.MaximumProperty, nameof(ToDate));
            grid.AddChild(dp, 2, 4);

            grid.AddChild(CreateLabel("����� ������������"), y: 6);
            dp = new DateTimePicker
            {
                Maximum = DateTime.Today,
                Format = DateTimeFormat.ShortDate,
                CultureInfo = CultureInfo.GetCultureInfo("ru-RU"),
                TimePickerVisibility = Visibility.Collapsed,
                ShowButtonSpinner = false,
                ShowDropDownButton = false
            };
            dp.SetBinding(DateTimePicker.ValueProperty, nameof(ToDate));
            dp.SetBinding(DateTimePicker.MinimumProperty, nameof(FromDate));
            grid.AddChild(dp, 2, 6);

            grid.AddChild(CreateLabel("��� �����������"), y: 8);
            tb = new NotifyTextBox();
            tb.SetBinding(TextBox.TextProperty, new ValidatedBinding(nameof(Inn), InnValidationRule.Instance));
            grid.AddChild(tb, 2, 8);

            grid.AddChild(CreateLabel("��������������� ����� ���"), y: 10);
            tb = new NotifyTextBox();
            tb.SetBinding(TextBox.TextProperty, new ValidatedBinding(nameof(Device), DeviceIdValidationRule.Instance)); 
            grid.AddChild(tb, 2, 10);

            grid.AddChild(CreateLabel("��������� ����� ��"), y: 12);
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