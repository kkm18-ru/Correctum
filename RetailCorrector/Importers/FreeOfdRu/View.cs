using RetailCorrector.Utils;
using RetailCorrector.Utils.Integrations.Clients;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace RetailCorrector.Importers
{
    partial class FreeOfdRu
    {
        internal class View : ImporterView<View, FreeOfdRu>
        {
            public View()
            {
                var grid = new Grid();
                
                grid.AddColumn(GridUnitType.Auto);
                grid.AddColumn(GridUnitType.Pixel, 5);
                grid.AddColumn(GridUnitType.Star);

                grid.AddRow(GridUnitType.Star);
                grid.AddRow(GridUnitType.Pixel, 5);
                grid.AddRow(GridUnitType.Star);
                grid.AddRow(GridUnitType.Pixel, 5);
                grid.AddRow(GridUnitType.Star);
                grid.AddRow(GridUnitType.Pixel, 5);
                grid.AddRow(GridUnitType.Star);
                grid.AddRow(GridUnitType.Pixel, 5);
                grid.AddRow(GridUnitType.Star);
                grid.AddRow(GridUnitType.Pixel, 5);
                grid.AddRow(GridUnitType.Star);


                grid.AddChild(new TextBlock { Text = "Логин", VerticalAlignment = VerticalAlignment.Center }, y: 0);
                grid.AddChild(new TextBlock { Text = "Пароль", VerticalAlignment = VerticalAlignment.Center }, y: 2);
                grid.AddChild(new TextBlock { Text = "ККТ", VerticalAlignment = VerticalAlignment.Center }, y: 6);
                grid.AddChild(new TextBlock { Text = "Начало", VerticalAlignment = VerticalAlignment.Center }, y: 8);
                grid.AddChild(new TextBlock { Text = "Конец", VerticalAlignment = VerticalAlignment.Center }, y: 10);

                var tbLogin = new TextBox();
                tbLogin.SetBinding(TextBox.TextProperty, nameof(Login));
                tbLogin.SetBinding(TextBox.IsEnabledProperty, nameof(CanAuthorize));
                grid.AddChild(tbLogin, 2, 0);

                var tbPswd = new PasswordBox();
                tbPswd.PasswordChanged += (s, _) => Presenter.Password = ((PasswordBox)s).Password;
                tbPswd.SetBinding(PasswordBox.IsEnabledProperty, nameof(CanAuthorize));
                grid.AddChild(tbPswd, 2, 2);

                var btnAuth = new Button { Content = "Авторизоваться" };
                btnAuth.Click += OnAuthClicked;
                btnAuth.SetBinding(Button.IsEnabledProperty, nameof(CanAuthorize));
                grid.AddChild(btnAuth, 2, 4);

                var devices = new CheckComboBox { IsEditable = true, DisplayMemberPath = nameof(FreeOfdRuDevice.RegNumber) };
                devices.SetBinding(CheckComboBox.SelectedItemsOverrideProperty, nameof(SelectedDevices));
                devices.SetBinding(CheckComboBox.ItemsSourceProperty, nameof(Devices));
                devices.SetBinding(CheckComboBox.IsEnabledProperty, nameof(Authorized));
                grid.AddChild(devices, 2, 6);

                var beginDate = new DatePicker { DisplayDateStart = Constants.OfdBeginDate, SelectedDateFormat = DatePickerFormat.Short };
                beginDate.SetBinding(DatePicker.SelectedDateProperty, nameof(Begin));
                beginDate.SetBinding(DatePicker.DisplayDateEndProperty, nameof(End));
                beginDate.SetBinding(DatePicker.IsEnabledProperty, nameof(Authorized));
                grid.AddChild(beginDate, 2, 8);

                var endDate = new DatePicker { DisplayDateEnd = DateTime.Today, SelectedDateFormat = DatePickerFormat.Short };
                endDate.SetBinding(DatePicker.SelectedDateProperty, nameof(End));
                endDate.SetBinding(DatePicker.DisplayDateStartProperty, nameof(Begin));
                endDate.SetBinding(DatePicker.IsEnabledProperty, nameof(Authorized));
                grid.AddChild(endDate, 2, 10);

                Content = grid;
            }
            private async void OnAuthClicked(object? sender, RoutedEventArgs args)
            {
                try
                {
                    await Dispatcher.InvokeAsync(() => Presenter.Authorizing = true);

                    Presenter.Devices.Clear();
                    await Presenter.Client.AuthAsync(Presenter.Login, Presenter.Password);
                    await foreach (var d in Presenter.Client.FetchDevicesAsync())
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
}
