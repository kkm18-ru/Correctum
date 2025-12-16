using RetailCorrector.UI.Controls;
using RetailCorrector.Utils;
using RetailCorrector.Utils.ValidationRules;
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
            grid.AddRow(GridUnitType.Auto); // dateFrom
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto); // dateTo
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto); // inn
            grid.AddRow(GridUnitType.Pixel, 5);
            grid.AddRow(GridUnitType.Auto); // deviceId

            grid.AddChild(CreateLabel("Логин"));
            var tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(Login));
            grid.AddChild(tb, 2);

            grid.AddChild(CreateLabel("Пароль"), y: 2);
            tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, nameof(Password));
            grid.AddChild(tb, 2, 2);

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

            grid.AddChild(CreateLabel("Начало сканирования"), y: 6);
            var dp = new DatePicker
            {
                DisplayDateStart = Constants.OfdBeginDate,
                SelectedDateFormat = DatePickerFormat.Short
            };
            dp.SetBinding(DatePicker.SelectedDateProperty, nameof(FromDate));
            dp.SetBinding(DatePicker.DisplayDateEndProperty, nameof(ToDate));
            grid.AddChild(dp, 2, 6);

            grid.AddChild(CreateLabel("Конец сканирования"), y: 8);
            dp = new DatePicker
            {
                DisplayDateEnd = DateTime.Today,
                SelectedDateFormat = DatePickerFormat.Short
            };
            dp.SetBinding(DatePicker.SelectedDateProperty, nameof(ToDate));
            dp.SetBinding(DatePicker.DisplayDateStartProperty, nameof(FromDate));
            grid.AddChild(dp, 2, 8);

            grid.AddChild(CreateLabel("ИНН организации"), y: 10);
            tb = new NotifyTextBox();
            tb.SetBinding(TextBox.TextProperty, new ValidatedBinding(nameof(Inn), InnValidationRule.Instance));
            tb.SetBinding(TextBox.IsEnabledProperty, nameof(Authorized));
            grid.AddChild(tb, 2, 10);

            grid.AddChild(CreateLabel("Регистрационный номер ККТ"), y: 12);
            var cb = new CheckComboBox { IsEditable = true };
            cb.SetBinding(CheckComboBox.ItemsSourceProperty, nameof(Devices));
            cb.SetBinding(CheckComboBox.SelectedItemsOverrideProperty, nameof(SelectedDevices));
            cb.SetBinding(CheckComboBox.IsEnabledProperty, nameof(Authorized));
            grid.AddChild(cb, 2, 12);
        }

        private static TextBlock CreateLabel(string text) => new()
        {
            Text = $"{text}: ",
            VerticalAlignment = VerticalAlignment.Center
        };
    }
}