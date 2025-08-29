using RetailCorrector.Utils;
using System.Windows;
using System.Windows.Controls;

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
                content.AddRow(GridUnitType.Pixel, 15);
                content.AddRow(GridUnitType.Pixel, 5);
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

                var progressBar = new ProgressBar();
                progressBar.SetBinding(ProgressBar.ValueProperty, nameof(Presenter.CurrentDayNumber));
                progressBar.SetBinding(ProgressBar.MaximumProperty, nameof(Presenter.CountDays));
                content.AddChild(progressBar, dx: 3);

                #region Текст
                content.AddChild(new TextBlock { Text = "Начало сканирования: " }, y: 2);
                content.AddChild(new TextBlock { Text = "Конец сканирования: " }, y: 4);
                content.AddChild(new TextBlock { Text = "Аутентификационный токен: " }, y: 6);
                content.AddChild(new TextBlock { Text = "ИНН организации: " }, y: 8);
                content.AddChild(new TextBlock { Text = "Регистрационный номер: " }, y: 10);
                #endregion

                #region Ввод
                var dp = new DatePicker { DisplayDateStart = new DateTime(2020, 1, 1) };
                dp.SetBinding(DatePicker.SelectedDateProperty, nameof(Presenter.BeginDate));
                dp.SetBinding(DatePicker.DisplayDateEndProperty, nameof(Presenter.EndDate));
                content.AddChild(dp, 2, 2);

                dp = new DatePicker { DisplayDateStart = DateTime.Today };
                dp.SetBinding(DatePicker.SelectedDateProperty, nameof(Presenter.EndDate));
                dp.SetBinding(DatePicker.DisplayDateStartProperty, nameof(Presenter.BeginDate));
                content.AddChild(dp, 2, 4);

                var tb = new TextBox();
                tb.SetBinding(TextBox.TextProperty, nameof(Presenter.Token));
                content.AddChild(tb, 2, 6);

                tb = new TextBox();
                tb.SetBinding(TextBox.TextProperty, nameof(Presenter.Vatin));
                content.AddChild(tb, 2, 8);

                tb = new TextBox();
                tb.SetBinding(TextBox.TextProperty, nameof(Presenter.DeviceId));
                content.AddChild(tb, 2, 10);
                #endregion

                Content = content;
            }
        }
    }
}
