using RetailCorrector.UI.Controls;
using RetailCorrector.Converters;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using RetailCorrector.Exceptions.Saby;
using MessageBox = System.Windows.MessageBox;
using Xceed.Wpf.Toolkit;

namespace RetailCorrector.Importers;

internal partial class Saby
{
    internal class View : ImporterView<View, Saby>
    {
        public View()
        {
            DataContext = new Saby();
            var grid = new Grid();
            Content = grid;

            grid.AddColumn(GridUnitType.Star);
            grid.AddColumn(GridUnitType.Pixel, 5);
            grid.AddColumn(GridUnitType.Star);

            grid.AddRow(GridUnitType.Auto); // login
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto); // passwd
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto); // auth
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto); // dateRange
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto); // inn
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto); // deviceId

            grid.AddChild(CreateLabel("Логин"));
            var tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(Login));
            grid.AddChild(tb, 2);

            grid.AddChild(CreateLabel("Пароль"), y: 2);
            var pb = new PasswordBox();
            pb.PasswordChanged += (s, _) => Presenter.Password = ((PasswordBox)s).Password;
            grid.AddChild(pb, 2, 2);

            var btn = new Button
            {
                Content = "Авторизоваться"
            };
            btn.SetBinding(Button.IsEnabledProperty, nameof(CanAuthorize));
            btn.Click += async (_, _) =>
            {
                try
                {
                    await Dispatcher.InvokeAsync(() => Presenter.Authorizing = true);
                    await Presenter._client.AuthAsync(Presenter.Login, Presenter.Password).ConfigureAwait(false);
                    await Dispatcher.InvokeAsync(() => Presenter.Authorized = true);
                }
                catch (Exception e)
                {
                    await Dispatcher.InvokeAsync(() => Presenter.Authorized = false);
                    var (message, caption) = e switch
                    {
                        SabyAuthException s => (s.Description, s.Title),
                        _ => (e.Message, "Произошла ошибка...")
                    };
                    MessageBox.Show(message, caption, 0, MessageBoxImage.Error);
                }
                finally
                {
                    await Dispatcher.InvokeAsync(() => Presenter.Authorizing = false);
                }
            };
            grid.AddChild(btn, y: 4, dx: 3);

            grid.AddChild(CreateLabel("Период выгрузки"), y: 6);
            var drp = new DateRangePicker();
            drp.SetBinding(DateRangePicker.DateRangeProperty, nameof(DateRange));
            drp.SetBinding(DateRangePicker.IsEnabledProperty, nameof(Authorized));
            grid.AddChild(drp, 2, 6);

            grid.AddChild(CreateLabel("ИНН организации"), y: 8);
            tb = new NotifyTextBox();
            tb.SetBinding(TextBox.TextProperty, new Binding(nameof(Inn))
            {
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus,
                Mode = BindingMode.TwoWay,
                Converter = new InnConverter()
            });
            tb.SetBinding(TextBox.IsEnabledProperty, nameof(Authorized));
            grid.AddChild(tb, 2, 8);

            grid.AddChild(CreateLabel("Регистрационный номер ККТ"), y: 10);
            var cb = new CheckComboBox { IsEditable = true };
            cb.SetBinding(CheckComboBox.ItemsSourceProperty, nameof(Devices));
            cb.SetBinding(CheckComboBox.SelectedItemsOverrideProperty, nameof(SelectedDevices));
            cb.SetBinding(CheckComboBox.IsEnabledProperty, nameof(Authorized));
            grid.AddChild(cb, 2, 10);
        }

        private static TextBlock CreateLabel(string text) => new()
        {
            Text = $"{text}: ",
            VerticalAlignment = VerticalAlignment.Center
        };
    }
}