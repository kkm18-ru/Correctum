using RetailCorrector.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.Parsers.Taxcom
{
    internal sealed class TaxcomView : UserControl, IParserView<TaxcomView, TaxcomPresenter>
    {
        public TaxcomPresenter Presenter { get; } = new();

        public TaxcomView()
        {
            DataContext = Presenter;
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
            content.AddChild(UIGenerators.Text("Логин: "));
            content.AddChild(UIGenerators.Text("Пароль: "), y: 2);
            content.AddChild(UIGenerators.Text("Регистрационный номер: "), y: 4);
            content.AddChild(UIGenerators.Text("Начало сканирования: "), y: 6);
            content.AddChild(UIGenerators.Text("Конец сканирования: "), y: 8);
            #endregion

            #region Ввод
            content.AddChild(UIGenerators.TextBox(nameof(Presenter.Login)), 2);
            content.AddChild(UIGenerators.TextBox(nameof(Presenter.Password)), 2, 2);
            content.AddChild(UIGenerators.TextBox(nameof(Presenter.DeviceId)), 2, 4);
            content.AddChild(UIGenerators.DatePicker(nameof(Presenter.BeginDate), new DateTime(2020, 1, 1), nameof(Presenter.EndDate)), 2, 6);
            content.AddChild(UIGenerators.DatePicker(nameof(Presenter.EndDate), nameof(Presenter.BeginDate), DateTime.Today), 2, 8);
            #endregion


            Content = content;
        }
    }
}
