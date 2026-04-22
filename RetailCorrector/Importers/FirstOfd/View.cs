using RetailCorrector.Converters;
using RetailCorrector.UI.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;

namespace RetailCorrector.Importers;

partial class FirstOfd
{
    internal class View : ImporterView<View, FirstOfd>
    {
        public View()
        {
            var invertBoolConverter = new InvertBooleanConverter();

            var grid = new Grid();
            Content = grid;

            grid.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new() { Width = new(5, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Star) });

            grid.RowDefinitions.Add(new() { Height = new(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new() { Height = new(5, GridUnitType.Pixel) });
            grid.RowDefinitions.Add(new() { Height = new(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new() { Height = new(5, GridUnitType.Pixel) });
            grid.RowDefinitions.Add(new() { Height = new(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new() { Height = new(5, GridUnitType.Pixel) });
            grid.RowDefinitions.Add(new() { Height = new(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new() { Height = new(5, GridUnitType.Pixel) });
            grid.RowDefinitions.Add(new() { Height = new(1, GridUnitType.Auto) });

            var lbl = new TextBlock { Text = "Логин:", VerticalAlignment = VerticalAlignment.Center };
            Grid.SetRow(lbl, 0);
            grid.Children.Add(lbl);

            lbl = new TextBlock { Text = "Пароль:", VerticalAlignment = VerticalAlignment.Center };
            Grid.SetRow(lbl, 2);
            grid.Children.Add(lbl);

            lbl = new TextBlock { Text = "Период:", VerticalAlignment = VerticalAlignment.Center };
            Grid.SetRow(lbl, 6);
            grid.Children.Add(lbl);

            lbl = new TextBlock { Text = "ККТ:", VerticalAlignment = VerticalAlignment.Center };
            Grid.SetRow(lbl, 8);
            grid.Children.Add(lbl);

            var input = new TextBox();
            input.SetBinding(TextBox.IsEnabledProperty, new Binding(nameof(Authorizing)) { Converter = invertBoolConverter });
            input.SetBinding(TextBox.TextProperty, new Binding(nameof(Login))
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            Grid.SetColumn(input, 2);
            Grid.SetRow(input, 0);
            grid.Children.Add(input);

            var passwd = new PasswordBox();
            passwd.PasswordChanged += (s, _) => Presenter.Password = ((PasswordBox)s).Password;
            passwd.SetBinding(PasswordBox.IsEnabledProperty, new Binding(nameof(Authorizing)) { Converter = invertBoolConverter });
            Grid.SetColumn(passwd, 2);
            Grid.SetRow(passwd, 2);
            grid.Children.Add(passwd);

            var btn = new Button { Content = "Авторизоваться" };
            btn.SetBinding(Button.CommandProperty, nameof(AuthCommand));
            Grid.SetColumn(btn, 2);
            Grid.SetRow(btn, 4);
            grid.Children.Add(btn);

            var drp = new DateRangePicker();
            drp.SetBinding(DateRangePicker.DateRangeProperty, nameof(DateRange));
            drp.SetBinding(DateRangePicker.IsEnabledProperty, nameof(IsAuthorized));
            Grid.SetColumn(drp, 2);
            Grid.SetRow(drp, 6);
            grid.Children.Add(drp);

            var ccb = new CheckComboBox();
            ccb.SetBinding(CheckComboBox.ItemsSourceProperty, nameof(Devices));
            ccb.SetBinding(CheckComboBox.SelectedItemsOverrideProperty, nameof(SelectedDevices));
            ccb.SetBinding(CheckComboBox.IsEnabledProperty, nameof(IsAuthorized));
            Grid.SetColumn(ccb, 2);
            Grid.SetRow(ccb, 8);
            grid.Children.Add(ccb);
        }
    }
}