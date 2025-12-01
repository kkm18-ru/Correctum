using RetailCorrector.Utils.Extensions;
using RetailCorrector.Utils.Integrations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RetailCorrector.Parsers;

partial class FirstOfd
{
    internal class View : ParserView<View, FirstOfd>
    {
        public View()
        {
            var grid = new Grid();

            grid.AddColumn(GridUnitType.Auto);
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

            grid.AddChild(new Label()
                .SetContent("API-ключ: "));
            grid.AddChild(new TextBox()
                .SetTextBinding(nameof(Presenter.ApiKey)), 2);

            grid.AddChild(new Button()
                .SetContent("Авторизоваться")
                .SetCommand(Presenter.AuthCommand), 2, 2);

            grid.AddChild(new Label()
                .SetContent("Организация: "), y: 4);
            grid.AddChild(new ComboBox()
                .SetItemsBinding(nameof(Presenter.Organizations))
                .SetPaths(nameof(FirstOfdOrganisation.Name), nameof(FirstOfdOrganisation.Id))
                .SetSelectedValueBinding(nameof(Presenter.OrganizationId)), 2, 4);

            grid.AddChild(new Label()
                .SetContent("Кассовый аппарат: "), y: 6);
            grid.AddChild(new ComboBox()
                .SetItemsBinding(nameof(Presenter.KKTs))
                .SetPaths(nameof(FirstOfdKKT.RegNumber))
                .SetSelectedValueBinding(nameof(Presenter.KKT)), 2, 6);

            grid.AddChild(new Label()
                .SetContent("Фискальный накопитель: "), y: 8);
            grid.AddChild(new ComboBox()
                .SetItemsBinding(nameof(Presenter.Drives))
                .SetSelectedValueBinding(nameof(Presenter.DriveId)), 2, 8);

            grid.AddChild(new Label()
                .SetContent("Начало периода: "), y: 10);
            grid.AddChild(new DatePicker()
                .SetRange(begin: new DateTime(2016, 06, 01))
                .SetRangeBinding(endPath: nameof(Presenter.EndDate))
                .SetDateBinding(nameof(Presenter.BeginDate)), 2, 10);

            grid.AddChild(new Label()
                .SetContent("Окончание периода: "), y: 12);
            grid.AddChild(new DatePicker()
                .SetRange(end: DateTime.Now)
                .SetRangeBinding(beginPath: nameof(Presenter.BeginDate))
                .SetDateBinding(nameof(Presenter.EndDate)), 2, 12);

            Content = grid;
        }
    }
}

file static class UIHelpers
{
    public static T SetContent<T>(this T element, object content)
        where T : ContentControl
    {
        element.Content = content;
        return element;
    }

    public static TextBox SetTextBinding(this TextBox element, string path)
    {
        element.SetBinding(TextBox.TextProperty, path);
        return element;
    }

    public static ComboBox SetItemsBinding(this ComboBox element, string path)
    {
        element.SetBinding(ComboBox.ItemsSourceProperty, path);
        return element;
    }

    public static ComboBox SetSelectedValueBinding(this ComboBox element, string path)
    {
        element.SetBinding(ComboBox.SelectedValueProperty, path);
        return element;
    }

    public static DatePicker SetDateBinding(this DatePicker element, string path)
    {
        element.SetBinding(DatePicker.SelectedDateProperty, path);
        return element;
    }

    public static DatePicker SetRangeBinding(this DatePicker element, string beginPath = "", string endPath = "")
    {
        if (!string.IsNullOrWhiteSpace(beginPath))
            element.SetBinding(DatePicker.DisplayDateStartProperty, beginPath);
        if (!string.IsNullOrWhiteSpace(endPath))
            element.SetBinding(DatePicker.DisplayDateEndProperty, endPath);
        return element;
    }

    public static DatePicker SetRange(this DatePicker element, DateTime? begin = null, DateTime? end = null)
    {
        if (begin is null) element.DisplayDateStart = begin;
        if (end is null) element.DisplayDateEnd = end;
        return element;
    }

    public static ComboBox SetPaths(this ComboBox element, string pathDisplayName, string pathSelect = "")
    {
        element.DisplayMemberPath = pathDisplayName;
        if(!string.IsNullOrWhiteSpace(pathSelect))
            element.SelectedValuePath = pathSelect;
        return element;
    }

    public static Button SetCommand(this Button element, ICommand command)
    {
        element.Command = command;
        return element;
    }
}