using System.Windows;
using System.Windows.Controls;
using RetailCorrector.Exceptions;
using RetailCorrector.UI.Controls;
using RetailCorrector.Utils;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace RetailCorrector.Importers;

partial class Taxcom
{
    internal class View : ImporterView<View, Taxcom>
    {
        public View()
        {
            var grid = new Grid();

            grid.AddColumn(GridUnitType.Auto);
            grid.AddColumn(GridUnitType.Pixel, 5);
            grid.AddColumn(GridUnitType.Star);

            grid.AddRow(GridUnitType.Auto); // login
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto); // passwd
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto); // agreement
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto); // btnAuth
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto); // device
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto); // dateRange

            grid.AddChild(new TextBlock { Text = "Логин: " }, 0, 0);
            grid.AddChild(new TextBlock { Text = "Пароль: " }, 0, 2);
            grid.AddChild(new TextBlock { Text = "Номер договора: " }, 0, 4);
            grid.AddChild(new TextBlock { Text = "ККТ: " }, 0, 8);
            grid.AddChild(new TextBlock { Text = "Период выгрузки: " }, 0, 10);

            var tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(Login));
            tb.SetBinding(IsEnabledProperty, nameof(CanAuthorized));
            grid.AddChild(tb, 2, 0);

            var pb = new PasswordBox();
            pb.PasswordChanged += (s, _) => Presenter.Password = ((PasswordBox)s).Password;
            pb.SetBinding(IsEnabledProperty, nameof(CanAuthorized));
            grid.AddChild(pb, 2, 2);

            tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(Agreement));
            tb.SetBinding(IsEnabledProperty, nameof(CanAuthorized));
            grid.AddChild(tb, 2, 4);

            var btn = new Button { Content = "Авторизоваться" };
            btn.SetBinding(IsEnabledProperty, nameof(CanAuthorized));
            btn.Click += Auth;
            grid.AddChild(btn, 2, 6);

            var ccb = new CheckComboBox();
            ccb.SetBinding(CheckComboBox.SelectedItemsOverrideProperty, nameof(SelectedDevices));
            ccb.SetBinding(CheckComboBox.ItemsSourceProperty, nameof(Devices));
            ccb.SetBinding(CheckComboBox.IsEnabledProperty, nameof(Authorized));
            grid.AddChild(ccb, 2, 8);

            var drp = new DateRangePicker();
            drp.SetBinding(DateRangePicker.DateRangeProperty, nameof(DateRange));
            drp.SetBinding(DateRangePicker.IsEnabledProperty, nameof(Authorized));
            grid.AddChild(drp, 2, 10);

            Content = grid;
        }

        private async void Auth(object? sender, RoutedEventArgs args)
        {
            try
            {
                await Dispatcher.InvokeAsync(() => Presenter.SelectedDevices.Clear());
                await Dispatcher.InvokeAsync(() => Presenter.Devices.Clear());
                
                await Dispatcher.InvokeAsync(() => Presenter.Authorizing = true);
                await Presenter._client.AuthAsync(Presenter.Login, Presenter.Password, Presenter.Agreement);
                await Dispatcher.InvokeAsync(() => Presenter.Authorized = true);
                
                await foreach (var outlet in Presenter._client.FetchOutletIdsAsync())
                    await foreach (var device in Presenter._client.FetchDevicesAsync(outlet))
                        await Dispatcher.InvokeAsync(() => Presenter.Devices.Add(device));
            }
            catch (Exception e)
            {
                await Dispatcher.InvokeAsync(() => Presenter.Authorized = false);
                if (e is TaxcomException te)
                    MessageBox.Show(te.Message, te.Summary, 0, MessageBoxImage.Error);
                else MessageBox.Show(e.ToString(), null, 0, MessageBoxImage.Error);
            }
            finally
            {
                await Dispatcher.InvokeAsync(() => Presenter.Authorizing = false);
            }
        }
    }
}