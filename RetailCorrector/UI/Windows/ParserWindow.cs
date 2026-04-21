using RetailCorrector.Parsers;
using RetailCorrector.UI.Controls;
using RetailCorrector.Utils;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.UI.Windows
{
    internal partial class ParserWindow : Types.DialogWindow, INotifyPropertyChanged
    {
        public string Text
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                PropertyChanged?.Invoke(this, TextNotifier);
            }
        } = "";
        public IParser Current
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                PropertyChanged?.Invoke(this, CurrentNotifier);
            }
        } = null!;

        private static readonly PropertyChangedEventArgs TextNotifier = new(nameof(Text));
        private static readonly PropertyChangedEventArgs CurrentNotifier = new(nameof(Current));

        public event PropertyChangedEventHandler? PropertyChanged;

        public ParserWindow(string text)
        {
            Title = "Преобразование строки в чек";
            DataContext = this;
            Height = 550;
            Width = 400;
            Text = text;

            var content = new Grid
            {
                Margin = new Thickness(5)
            };
            
            content.AddRow(GridUnitType.Pixel, 21);
            content.AddRow(GridUnitType.Pixel, 5);
            content.AddRow(GridUnitType.Star);
            
            content.AddColumn(GridUnitType.Star);
            content.AddColumn(GridUnitType.Pixel, 5);
            content.AddColumn(GridUnitType.Pixel, 85);
            
            Content = content;

            var tb = new TextBox
            {
                AcceptsReturn = true,
                AcceptsTab = true
            };
            tb.SetBinding(TextBox.TextProperty, nameof(Text));
            content.AddChild(tb, y: 2, dx: 3);

            var btn = new Button
            {
                Content = "Обработать",
            };
            btn.Click += Submit;
            content.AddChild(btn, 2);

            var cb = new ComboBox
            {
                ItemsSource = ModuleRegistry.Parsers,
                DisplayMemberPath = "Name"
            };
            cb.SetBinding(ComboBox.SelectedItemProperty, nameof(Current));
            content.AddChild(cb);
        }

        private void Submit(object? sender, RoutedEventArgs args)
        {
            if (Current.Parse(Text, out var receipt))
                Storage.Instance.AddReceipt(receipt);
            else 
                MessageBox.Show("Не удалось спарсить чек. Подробная запись в журнале.",
                    "", 0, MessageBoxImage.Error);
            Close();
        }
    }
}