using RetailCorrector.Utils;
using RetailCorrector.Utils.Integrations.Clients;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace RetailCorrector.Parsers;

internal partial class OfdRu
{
    internal class View : ParserView<View, OfdRu>
    {
        public View()
        {
            Content = new Grid().AddColumns(2)
                .AddRow("Аутентификационный токен: ", new TextBox()
                    .AddBinding(TextBox.TextProperty, nameof(Token))
                    .AddBinding(TextBox.IsEnabledProperty, nameof(CanAuthorize)))

                .AddRow("", new Button { Content = "Авторизоваться" }
                    .AddBinding(Button.IsEnabledProperty, nameof(CanAuthorize))
                    .SetClickHandler(OnAuthClicked))

                .AddRow("Регистрационный номер: ", new CheckComboBox
                    { IsEditable = true, DisplayMemberPath = nameof(OfdRuDevice.KktRegId) }
                    .AddBinding(CheckComboBox.SelectedItemsOverrideProperty, nameof(SelectedDevices))
                    .AddBinding(CheckComboBox.ItemsSourceProperty, nameof(Devices))
                    .AddBinding(CheckComboBox.IsEnabledProperty, nameof(Authorized)))

                .AddRow("Начало сканирования: ", new DatePicker()
                    .SetBeginDate(Constants.OfdBeginDate).SetDateFormat(DatePickerFormat.Short)
                    .AddBinding(DatePicker.SelectedDateProperty, nameof(BeginDate))
                    .AddBinding(DatePicker.DisplayDateEndProperty, nameof(EndDate))
                    .AddBinding(DatePicker.IsEnabledProperty, nameof(Authorized)))

                .AddRow("Конец сканирования: ", new DatePicker()
                    .SetEndDate(DateTime.Today).SetDateFormat(DatePickerFormat.Short)
                    .AddBinding(DatePicker.SelectedDateProperty, nameof(EndDate))
                    .AddBinding(DatePicker.DisplayDateStartProperty, nameof(BeginDate))
                    .AddBinding(DatePicker.IsEnabledProperty, nameof(Authorized)));
        }

        private async void OnAuthClicked(object? sender, RoutedEventArgs args)
        {
            try
            {
                await Dispatcher.InvokeAsync(() => Presenter.Authorizing = true);

                Presenter.Devices.Clear();
                await Presenter.Client.SetAuthTokenAsync(Presenter.Token);
                foreach (var d in await Presenter.Client.FetchDevicesAsync())
                    Presenter.Devices.Add(d);

                await Dispatcher.InvokeAsync(() => Presenter.Authorized = true);
            }
            catch (Exception e)
            {
                await Dispatcher.InvokeAsync(() => Presenter.Authorized = false);
                System.Windows.MessageBox.Show(e.Message, "Произошла ошибка...", 0, MessageBoxImage.Error);
            }
            finally
            {
                await Dispatcher.InvokeAsync(() => Presenter.Authorizing = false);
            }
        }
    }
}

file static class Utils
{
    public static Grid AddColumns(this Grid grid, int countColumns)
    {
        for (var i = 0; i < countColumns * 2 - 1; i++)
        {
            var type = i % 2 == 1 ? GridUnitType.Pixel : GridUnitType.Star;
            var value = i % 2 == 1 ? 5 : 1;
            grid.AddColumn(type, value);
        }
        return grid;
    }

    public static Grid AddRow(this Grid grid, string text, UIElement elem)
    {
        if(grid.RowDefinitions.Count % 2 == 1) 
            grid.AddRow(GridUnitType.Pixel, 5);
        grid.AddRow(GridUnitType.Auto);
        var y = grid.RowDefinitions.Count - 1;
        grid.AddChild(new TextBlock { Text = text }, 0, y);
        grid.AddChild(elem, 2, y);
        return grid;
    }

    public static T AddBinding<T>(this T elem, DependencyProperty prop, string path)
        where T : FrameworkElement
    {
        elem.SetBinding(prop, path);
        return elem;
    }

    public static DatePicker SetDateFormat(this DatePicker elem, DatePickerFormat format)
    {
        elem.SelectedDateFormat = format;
        return elem;
    }

    public static DatePicker SetBeginDate(this DatePicker elem, DateTime value)
    {
        elem.DisplayDateStart = value;
        return elem;
    }

    public static DatePicker SetEndDate(this DatePicker elem, DateTime value)
    {
        elem.DisplayDateEnd = value;
        return elem;
    }

    public static Button SetClickHandler(this Button btn, RoutedEventHandler handler)
    {
        btn.Click += handler;
        return btn;
    }
}
