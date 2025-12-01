using RetailCorrector.Utils.Extensions;
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

            grid.AddChild(new Label()
                .SetContent("API-ключ: "));
            grid.AddChild(new TextBox()
                .SetTextBinding(nameof(Presenter.ApiKey)), 2);

            grid.AddChild(new Button()
                .SetContent("Авторизоваться")
                .SetCommand(Presenter.AuthCommand), 2, 2);

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

    public static Button SetCommand(this Button element, ICommand command)
    {
        element.Command = command;
        return element;
    }
}