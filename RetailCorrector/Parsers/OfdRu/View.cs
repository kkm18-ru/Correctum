using RetailCorrector.UI.Controls;
using RetailCorrector.Utils;
using RetailCorrector.Utils.Extensions;
using RetailCorrector.Utils.ValidationRules;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.Parsers
{
    internal partial class OfdRu
    {
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
                var dp = new DatePicker
                {
                    DisplayDateStart = Constants.OfdBeginDate,
                    SelectedDateFormat = DatePickerFormat.Short
                };
                dp.SetBinding(DatePicker.SelectedDateProperty, nameof(Presenter.BeginDate));
                dp.SetBinding(DatePicker.DisplayDateEndProperty, nameof(Presenter.EndDate));
                content.AddChild(dp, 2);

                dp = new DatePicker
                {
                    DisplayDateEnd = DateTime.Today,
                    SelectedDateFormat = DatePickerFormat.Short
                };
                dp.SetBinding(DatePicker.SelectedDateProperty, nameof(Presenter.EndDate));
                dp.SetBinding(DatePicker.DisplayDateStartProperty, nameof(Presenter.BeginDate));
                content.AddChild(dp, 2, 2);

                var tb = new TextBox();
                tb.SetBinding(TextBox.TextProperty, nameof(Presenter.Token));
                content.AddChild(tb, 2, 4);

                tb = new NotifyTextBox();
                var bind = new ValidatedBinding(nameof(Presenter.Vatin), InnValidationRule.Instance);
                tb.SetBinding(TextBox.TextProperty, bind);
                content.AddChild(tb, 2, 6);

                var cb = new ComboBox 
                { 
                    IsEditable = true 
                };
                cb.SetBinding(ComboBox.SelectedValueProperty, nameof(Presenter.DeviceId));
                content.AddChild(cb, 2, 8);

                Presenter.TokenChanged += () =>
                {
                    cb.Items.Clear();
                    foreach (var d in Presenter.GetDevices())
                        cb.Items.Add(d);
                };
                #endregion

                Content = content;
            }
        }
    }
}
