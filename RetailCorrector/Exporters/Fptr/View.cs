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
                    if (uiParent.RowDefinitions.Count == 19)
                    {
                        if (FiscalFormat == 2) return;
                        uiParent.RowDefinitions.RemoveRange(uiParent.RowDefinitions.Count-2, 2);
                        uiParent.Children.RemoveAt(uiParent.Children.Count - 1);
                    }
                    else
                    {
                        if (FiscalFormat != 2) return;
                        var y = AddRow(uiParent);

                        var ch = new CheckBox
                        {
                            Content = "Исправляются старые чеки?"
                        };
                        ch.SetBinding(CheckBox.IsCheckedProperty, new Binding(nameof(FixOldReceipts)) { Source = this });
                        uiParent.AddChild(ch, y: y, dx: 5);
                    }
                }
            };
        }

        private static int AddRow(Grid content)
        {
            content.AddRow(GridUnitType.Pixel, 5);
            content.AddRow(GridUnitType.Auto);
            return content.RowDefinitions.Count - 1;
        }

        public void InitVisual(Grid content)
        {
            uiParent = content;

            var y = AddRow(content);
            var lbl = CreateLabel("Номер акта");
            content.AddChild(lbl, y: y);
            var cb = new ComboBox
            {
                ItemsSource = new[] { "1.2", "1.1", "1.05" },
            };
            cb.SetBinding(ComboBox.SelectedIndexProperty, new Binding(nameof(FiscalFormat)) { Source = this });
            content.AddChild(cb, 2, y, 3);
            
            y = AddRow(content);
            var ch = new CheckBox { Content = "Печатать ли чек?" };
            cb.SetBinding(CheckBox.IsCheckedProperty, new Binding(nameof(Print)) { Source = this });
            content.AddChild(ch, y: y, dx: 5);

            y = AddRow(content);
            lbl = CreateLabel("Вид коррекции");
            content.AddChild(lbl, y: y);
            cb = new ComboBox
            {
                ItemsSource = new[] { "Самостоятельная", "По предписанию" },
            };
            cb.SetBinding(ComboBox.SelectedIndexProperty, new Binding(nameof(CorrType)) { Source = this });
            content.AddChild(cb, 2, y, 3);

            y = AddRow(content);
            lbl = CreateLabel("Номер акта");
            lbl.ToolTip = new ToolTip
            {
                Content = """
                          Данный параметр нужно использовать только в двух случаях:
                          1) Коррекция выполняется по требованию ФНС
                          2) Коррекция выполняется на кассе с ФФД 1.05, так как чек ранее не был сформирован 
                          """
            };
            content.AddChild(lbl, y: y);
            var input = new TextBox();
            input.SetBinding(TextBox.TextProperty, new Binding(nameof(ActId)) { Source = this });
            content.AddChild(input, 2, y, 3);

            y = AddRow(content);
            var check = new CheckBox
            {
                Content = "Применить рассчет для сети \"Интернет\""
            };
            check.SetBinding(CheckBox.IsCheckedProperty, new Binding(nameof(IsInternet)) { Source = this });
            content.AddChild(check, y: y, dx: 5);

            y = AddRow(content);
            content.AddChild(CreateLabel("Адрес сайта"), y: y);
            input = new TextBox();
            input.SetBinding(TextBox.IsEnabledProperty, new Binding(nameof(IsInternet)) { Source = this });
            input.SetBinding(TextBox.TextProperty, new Binding(nameof(SiteAddress)) { Source = this });
            content.AddChild(input, 2, y, 3);

            if (FiscalFormat == 2)
            {
                y = AddRow(content);

                ch = new CheckBox
                {
                    Content = "Исправляются старые чеки?"
                };
                ch.SetBinding(CheckBox.IsCheckedProperty, new Binding(nameof(FixOldReceipts)) { Source = this });
                content.AddChild(ch, y: y, dx: 5);
            }
        }

        private static TextBlock CreateLabel(string text) => new()
        {
            VerticalAlignment = VerticalAlignment.Center,
            Text = $"{text}: "
        };
    }
}
