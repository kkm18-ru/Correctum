using RetailCorrector.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.Parsers.OfdRu
{
    internal class OfdRuView : ParserView<OfdRuView, OfdRuPresenter>
    {
        public OfdRuView()
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

            content.AddChild(UIGenerators.ProgressBar(nameof(Presenter.CurrentDayNumber), nameof(Presenter.CountDays)), dx: 3);

            #region Текст
            content.AddChild(UIGenerators.Text("Начало сканирования: "), y: 2);
            content.AddChild(UIGenerators.Text("Конец сканирования: "), y: 4);
            content.AddChild(UIGenerators.Text("Аутентификационный токен: "), y: 6);
            content.AddChild(UIGenerators.Text("ИНН организации: "), y: 8);
            content.AddChild(UIGenerators.Text("Регистрационный номер: "), y: 10);
            #endregion

            #region Ввод
            content.AddChild(UIGenerators.DatePicker(nameof(Presenter.BeginDate),new DateTime(2020, 1, 1), nameof(Presenter.EndDate)), 2, 2);
            content.AddChild(UIGenerators.DatePicker(nameof(Presenter.EndDate), nameof(Presenter.BeginDate), DateTime.Today), 2, 4);
            content.AddChild(UIGenerators.TextBox(nameof(Presenter.Token)), 2, 6);
            content.AddChild(UIGenerators.TextBox(nameof(Presenter.Vatin)), 2, 8);
            content.AddChild(UIGenerators.TextBox(nameof(Presenter.DeviceId)), 2, 10);
            #endregion

            Content = content;
        }
    }
}
