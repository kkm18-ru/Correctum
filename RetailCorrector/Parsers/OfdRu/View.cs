using RetailCorrector.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.Parsers.OfdRu
{
    internal class OfdRuView : UserControl, IParserView<OfdRuView, OfdRuPresenter>
    {
        public OfdRuPresenter Presenter { get; } = new OfdRuPresenter();

        public OfdRuView()
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
            content.AddRow(GridUnitType.Pixel, 5);
            content.AddRow(GridUnitType.Auto);
            #endregion

            content.AddChild(UIGenerators.ProgressBar(0, Presenter.CountDays, nameof(Presenter.CurrentDayNumber)), dx: 3);

            #region Текст
            content.AddChild(UIGenerators.Text("Начало сканирования: "), y: 2);
            content.AddChild(UIGenerators.Text("Конец сканирования: "), y: 4);
            content.AddChild(UIGenerators.Text("Аутентификационный токен: "), y: 6);
            content.AddChild(UIGenerators.Text("ИНН организации: "), y: 8);
            content.AddChild(UIGenerators.Text("Регистрационный номер: "), y: 10);
            #endregion

            #region Ввод
            content.AddChild(UIGenerators.DatePicker(new DateTime(2020, 1, 1), Presenter.EndDate, nameof(Presenter.BeginDate)), 2, 2);
            content.AddChild(UIGenerators.DatePicker(Presenter.BeginDate, DateTime.Today, nameof(Presenter.EndDate)), 2, 4);
            content.AddChild(UIGenerators.TextBox(nameof(Presenter.Token)), 2, 6);
            content.AddChild(UIGenerators.TextBox(nameof(Presenter.Vatin)), 2, 8);
            content.AddChild(UIGenerators.TextBox(nameof(Presenter.DeviceId)), 2, 10);
            #endregion

            Content = content;
        }
    }
}
