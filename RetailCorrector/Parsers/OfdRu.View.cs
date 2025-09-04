using RetailCorrector.Utils;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace RetailCorrector.Parsers
{
    internal partial class OfdRu {
        internal class View : ParserView<View, OfdRu>
        {
            public View()
            {
                var content = new Grid();

                #region Столбцы
                content.AddColumn(GridUnitType.Auto);
                content.AddColumn(GridUnitType.Pixel, 5);
                content.AddColumn(GridUnitType.Star);
                #endregion

                #region Строки
                content.AddRow(GridUnitType.Auto);
                content.AddRow(GridUnitType.Pixel, 5);
                content.AddRow(GridUnitType.Auto);
                content.AddRow(GridUnitType.Pixel, 5);
                content.AddRow(GridUnitType.Auto);
                content.AddRow(GridUnitType.Pixel, 5);
                content.AddRow(GridUnitType.Auto);
                content.AddRow(GridUnitType.Pixel, 5);
                content.AddRow(GridUnitType.Auto);
                #endregion

                #region Текст
                content.AddChild(new TextBlock { Text = "Начало сканирования: " });
                content.AddChild(new TextBlock { Text = "Конец сканирования: " }, y: 2);
                content.AddChild(new TextBlock { Text = "Аутентификационный токен: " }, y: 4);
                content.AddChild(new TextBlock { Text = "ИНН организации: " }, y: 6);
                content.AddChild(new TextBlock { Text = "Регистрационный номер: " }, y: 8);
                #endregion

                #region Ввод
                var dp = new DateTimePicker
                {
                    Minimum = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                    Format = DateTimeFormat.ShortDate,
                    CultureInfo = CultureInfo.GetCultureInfo("ru-RU"),
                    TimePickerVisibility = Visibility.Collapsed,
                    ShowButtonSpinner = false,
                    ShowDropDownButton = false
                };
                dp.SetBinding(DateTimePicker.ValueProperty, nameof(Presenter.BeginDate));
                dp.SetBinding(DateTimePicker.MaximumProperty, nameof(Presenter.EndDate));
                content.AddChild(dp, 2);

                dp = new DateTimePicker
                { 
                    Minimum = DateTime.Today,
                    Format = DateTimeFormat.ShortDate,
                    CultureInfo = CultureInfo.GetCultureInfo("ru-RU"),
                    TimePickerVisibility = Visibility.Collapsed,
                    ShowButtonSpinner = false,
                    ShowDropDownButton = false
                };
                dp.SetBinding(DateTimePicker.ValueProperty, nameof(Presenter.EndDate));
                dp.SetBinding(DateTimePicker.MinimumProperty, nameof(Presenter.BeginDate));
                content.AddChild(dp, 2, 2);

                var tb = new TextBox();
                tb.SetBinding(TextBox.TextProperty, nameof(Presenter.Token));
                content.AddChild(tb, 2, 4);

                tb = new TextBox();
                tb.SetBinding(TextBox.TextProperty, nameof(Presenter.Vatin));
                content.AddChild(tb, 2, 6);

                tb = new TextBox();
                tb.SetBinding(TextBox.TextProperty, nameof(Presenter.DeviceId));
                content.AddChild(tb, 2, 8);
                #endregion

                Content = content;
            }
        }
    }
}
