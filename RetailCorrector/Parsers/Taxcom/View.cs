using RetailCorrector.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.Parsers.Taxcom
{
    internal sealed class TaxcomView : ParserView<TaxcomView, TaxcomPresenter>
    {
        public TaxcomView()
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
            content.AddChild(new TextBlock { Text = "Логин: " }, y: 4);
            content.AddChild(new TextBlock { Text = "Пароль: " }, y: 6);
            content.AddChild(new TextBlock { Text = "Регистрационный номер: " }, y: 8);
            #endregion

            #region Ввод
            var dp = new DatePicker { DisplayDateStart = new DateTime(2020, 1, 1) };
            dp.SetBinding(DatePicker.SelectedDateProperty, nameof(Presenter.BeginDate));
            dp.SetBinding(DatePicker.DisplayDateEndProperty, nameof(Presenter.EndDate));
            content.AddChild(dp, 2);

            dp = new DatePicker { DisplayDateStart = DateTime.Today };
            dp.SetBinding(DatePicker.SelectedDateProperty, nameof(Presenter.EndDate));
            dp.SetBinding(DatePicker.DisplayDateStartProperty, nameof(Presenter.BeginDate));
            content.AddChild(dp, 2, 2);

            var tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(Presenter.Login));
            content.AddChild(tb, 2, 4);

            tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(Presenter.Password));
            content.AddChild(tb, 2, 6);

            tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(Presenter.DeviceId));
            content.AddChild(tb, 2, 8);
            #endregion

            Content = content;
        }
    }
}
