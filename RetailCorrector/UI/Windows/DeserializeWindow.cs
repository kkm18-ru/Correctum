using RetailCorrector.Attributes;
using RetailCorrector.Data;
using RetailCorrector.UI.Controls;
using RetailCorrector.Utils;
using System.Windows;
using System.Windows.Controls;
using RetailCorrector.Deserializers;

namespace RetailCorrector.UI.Windows
{
    internal partial class DeserializeWindow : Types.DialogWindow
    {
        [NotifyChanged] public partial string Text { get; set; }
        [NotifyChanged] public partial int DeserializerId { get; set; }

        private IReceiptDeserializer CurrDeserializer => Deserializers[DeserializerId]; 
        private string CurrDeserializerName => DisplayNames[DeserializerId]; 

        public DeserializeWindow(string text)
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
                ItemsSource = DisplayNames
            };
            cb.SetBinding(ComboBox.SelectedIndexProperty, nameof(DeserializerId));
            content.AddChild(cb);
        }

        private void Submit(object? sender, RoutedEventArgs args)
        {
            Workspace.Instance.ParserHistory.Add(new ParseInfo($"Вставка из {CurrDeserializerName}"));
            var receipt = new Receipt
            {
                ParseId = Workspace.Instance.ParserHistory[^1].Id,
                Payment = new Payment()
            };
            CurrDeserializer.Deserialize(Text, ref receipt);
            Workspace.Instance.Receipts.Add(receipt);
            ReceiptPanel.UpdateReceiptList();
            Close();
        }
    }
}