using RetailCorrector.Utils;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace RetailCorrector.Parsers;

partial class PlatformaOFD
{
    internal class View : ParserView<View, PlatformaOFD>
    {
        public View()
        {
            var grid = new Grid();
            Content = grid;

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

            grid.AddChild(new TextBlock { Text = "Идентификатор сессии" });
            var tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(SessionId));
            tb.SetBinding(IsEnabledProperty, nameof(CanAuthorize));
            grid.AddChild(tb, 2);

            grid.AddChild(new TextBlock { Text = "Токен доступа" }, y: 2);
            tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(AccessToken));
            tb.SetBinding(IsEnabledProperty, nameof(CanAuthorize));
            grid.AddChild(tb, 2, 2);

            var btn = new Button { Content = "Авторизоваться" };
            btn.Click += OnAuthClicked;
            btn.SetBinding(IsEnabledProperty, nameof(CanAuthorize));
            grid.AddChild(btn, 2, 4);

            grid.AddChild(new TextBlock { Text = "ККТ" }, y: 6);
            var cb = new CheckComboBox();
            cb.SetBinding(CheckComboBox.ItemsSourceProperty, nameof(Devices));
            cb.SetBinding(CheckComboBox.SelectedItemsOverrideProperty, nameof(SelectedDevices));
            cb.SetBinding(IsEnabledProperty, nameof(Authorized));
            grid.AddChild(cb, 2, 6);

            grid.AddChild(new TextBlock { Text = "Начало периода" }, y: 8);
            var dp = new DatePicker { DisplayDateStart = Constants.OfdBeginDate };
            dp.SetBinding(DatePicker.DisplayDateEndProperty, nameof(EndDate));
            dp.SetBinding(DatePicker.SelectedDateProperty, nameof(BeginDate));
            dp.SetBinding(IsEnabledProperty, nameof(Authorized));
            grid.AddChild(dp, 2, 8);

            grid.AddChild(new TextBlock { Text = "Конец периода" }, y: 10);
            dp = new DatePicker { DisplayDateEnd = DateTime.Now };
            dp.SetBinding(DatePicker.DisplayDateStartProperty, nameof(BeginDate));
            dp.SetBinding(DatePicker.SelectedDateProperty, nameof(EndDate));
            dp.SetBinding(IsEnabledProperty, nameof(Authorized));
            grid.AddChild(dp, 2, 10);
        }

        private async void OnAuthClicked(object? sender, RoutedEventArgs args)
        {
            try
            {
                await Dispatcher.InvokeAsync(() => Presenter.Authorizing = true);

                Presenter.Devices.Clear();
                Presenter.Client.Auth(Presenter.AccessToken, Presenter.SessionId);
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
