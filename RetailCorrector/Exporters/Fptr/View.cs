using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RetailCorrector.Exporters
{
    internal partial class Fptr
    {
        private Grid uiParent;

        private Fptr()
        {
            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(FiscalFormat))
                {
                    if (uiParent.RowDefinitions.Count == 11)
                    {
                        if (FiscalFormat == 2) return;
                        uiParent.RowDefinitions.RemoveRange(9, 2);
                        uiParent.Children.RemoveRange(10, 2);
                    }
                    else
                    {
                        if (FiscalFormat != 2) return;
                        uiParent.AddRow(GridUnitType.Pixel, 5);
                        uiParent.AddRow(GridUnitType.Auto); // OldCorrReason 10

                        var ch = new CheckBox
                        {
                            Content = "Исправляются старые чеки?"
                        };
                        ch.SetBinding(CheckBox.IsCheckedProperty, new Binding(nameof(FixOldReceipts)) { Source = this });
                        uiParent.AddChild(ch, y: 14, dx: 3);
                    }
                }
            };
        }

        public void InitVisual(Grid content)
        {
            uiParent = content;

            content.AddRow(GridUnitType.Pixel, 5);
            content.AddRow(GridUnitType.Auto); // FiscalFormat 4
            content.AddRow(GridUnitType.Pixel, 5);
            content.AddRow(GridUnitType.Auto); // Print 6
            content.AddRow(GridUnitType.Pixel, 5);
            content.AddRow(GridUnitType.Auto); // CorrType 8
            content.AddRow(GridUnitType.Pixel, 5);
            content.AddRow(GridUnitType.Auto); // ActId 10
            content.AddRow(GridUnitType.Pixel, 5);
            content.AddRow(GridUnitType.Auto); // IsInternet 12
            content.AddRow(GridUnitType.Pixel, 5);
            content.AddRow(GridUnitType.Auto); // HttpAddress 14

            var lbl = CreateLabel("Номер акта");
            content.AddChild(lbl, y: 2);
            var cb = new ComboBox
            {
                ItemsSource = new[] { "1.2", "1.1", "1.05" },
            };
            cb.SetBinding(ComboBox.SelectedIndexProperty, new Binding(nameof(FiscalFormat)) { Source = this });
            content.AddChild(cb, 2, 2);

            var ch = new CheckBox { Content = "Печатать ли чек?" };
            cb.SetBinding(CheckBox.IsCheckedProperty, new Binding(nameof(Print)) { Source = this });
            content.AddChild(ch, y: 4, dx: 3);

            lbl = CreateLabel("Вид коррекции");
            content.AddChild(lbl, y: 6);
            cb = new ComboBox
            {
                ItemsSource = new[] { "Самостоятельная", "По предписанию" },
            };
            cb.SetBinding(ComboBox.SelectedIndexProperty, new Binding(nameof(CorrType)) { Source = this });
            content.AddChild(cb, 2, 6);

            lbl = CreateLabel("Номер акта");
            lbl.ToolTip = new ToolTip
            {
                Content = """
                          Данный параметр нужно использовать только в двух случаях:
                          1) Коррекция выполняется по требованию ФНС
                          2) Коррекция выполняется на кассе с ФФД 1.05, так как чек ранее не был сформирован 
                          """
            };
            content.AddChild(lbl, y: 8);
            var input = new TextBox();
            input.SetBinding(TextBox.TextProperty, new Binding(nameof(ActId)) { Source = this });
            content.AddChild(input, 2, 8);

            var check = new CheckBox
            {
                Content = "Применить рассчет для сети \"Интернет\""
            };
            check.SetBinding(CheckBox.IsCheckedProperty, new Binding(nameof(IsInternet)) { Source = this });
            content.AddChild(check, y: 10, dx: 3);

            content.AddChild(CreateLabel("Адрес сайта"), y: 12);
            input = new TextBox();
            input.SetBinding(TextBox.IsEnabledProperty, new Binding(nameof(IsInternet)) { Source = this });
            input.SetBinding(TextBox.TextProperty, new Binding(nameof(SiteAddress)) { Source = this });
            content.AddChild(input, 2, 12);

            if (FiscalFormat == 2)
            {
                content.AddRow(GridUnitType.Pixel, 5);
                content.AddRow(GridUnitType.Auto); // OldCorrReason 16

                ch = new CheckBox
                {
                    Content = "Исправляются старые чеки?"
                };
                ch.SetBinding(CheckBox.IsCheckedProperty, new Binding(nameof(FixOldReceipts)) { Source = this });
                content.AddChild(ch, y: 14, dx: 3);
            }
        }

        private static TextBlock CreateLabel(string text) => new()
        {
            VerticalAlignment = VerticalAlignment.Center,
            Text = $"{text}: "
        };
    }
}
