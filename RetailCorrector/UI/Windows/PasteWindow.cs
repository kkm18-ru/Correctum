using RetailCorrector.Attributes;
using RetailCorrector.Data;
using RetailCorrector.Enums;
using RetailCorrector.UI.Controls;
using RetailCorrector.Utils;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.UI.Windows
{
    internal partial class PasteWindow : Types.DialogWindow
    {
        [NotifyChanged] public partial string Text { get; set; }

        public PasteWindow(string text)
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
                ItemsSource = new[] { "Платформа ОФД" },
                IsEnabled = false,
                SelectedIndex = 0
            };
            content.AddChild(cb);
        }

        private void Submit(object? sender, RoutedEventArgs args)
        {
            Workspace.Instance.ParserHistory.Add(new("Вставка из Платформа ОФД"));
            var receipt = new Receipt
            {
                ParseId = Workspace.Instance.ParserHistory[^1].Id,
                Payment = new Payment()
            };
            var items = new List<Position>();
            var lines = Text.Replace("\r", "").Split("\n");
            object item = null;

            for(var i = 0; i < lines.Length; i++)
            {
                var match = Regex.Match(lines[i], @"^ (\|  )*\+- \[(?<code>\d{4})\] [a-zA-Z0-9]+( +(?<value>[^ ]*))?$");
                if (!match.Success) continue;
                if(item is not null && !match.Groups[1].Success)
                {
                    if (item is Position pos) 
                        items.Add(pos);
                    item = null;
                }
                var value = match.Groups["value"].Value.Replace('·', ' ');
                switch (match.Groups["code"].Value)
                {
                    case "1054":
                        receipt.Type = (Operation)int.Parse(value);
                        break;
                    case "1077":
                        receipt.DocSign = ulong.Parse(value);
                        break;
                    case "1012":
                        receipt.Created = DateTimeOffset.FromUnixTimeSeconds(long.Parse(value)).DateTime;
                        break;
                    case "1020":
                        receipt.TotalSum = ulong.Parse(value);
                        break;
                    case "1031":
                        {
                            var payment = receipt.Payment;
                            payment.Cash = ulong.Parse(value);
                            receipt.Payment = payment;
                        }
                        break;
                    case "1081":
                        {
                            var payment = receipt.Payment;
                            payment.ECash = ulong.Parse(value);
                            receipt.Payment = payment;
                        }
                        break;
                    case "1215":
                        {
                            var payment = receipt.Payment;
                            payment.Pre = ulong.Parse(value);
                            receipt.Payment = payment;
                        }
                        break;
                    case "1216":
                        {
                            var payment = receipt.Payment;
                            payment.Post = ulong.Parse(value);
                            receipt.Payment = payment;
                        }
                        break;
                    case "1217":
                        {
                            var payment = receipt.Payment;
                            payment.Provision = ulong.Parse(value);
                            receipt.Payment = payment;
                        }
                        break;
                    case "1059":
                        item = new Position();
                        break;
                    case "2108":
                        {
                            if (item is not Position pos) continue;
                            pos.MeasureUnit = (MeasureUnit)int.Parse(value);
                            item = pos;
                        }
                        break;
                    case "1030":
                        {
                            if (item is not Position pos) continue;
                            pos.Name = value;
                            item = pos;
                        }
                        break;
                    case "1079":
                        {
                            if (item is not Position pos) continue;
                            pos.Price = ulong.Parse(value);
                            item = pos;
                        }
                        break;
                    case "1023":
                        {
                            if (item is not Position pos) continue;
                            pos.Quantity = (ulong)(double.Parse(value) * 1000);
                            item = pos;
                        }
                        break;
                    case "1043":
                        {
                            if (item is not Position pos) continue;
                            pos.Cost = ulong.Parse(value);
                            item = pos;
                        }
                        break;
                    case "1199":
                        {
                            if (item is not Position pos) continue;
                            pos.TaxRate = (TaxRate)int.Parse(value);
                            item = pos;
                        }
                        break;
                    case "1214":
                        {
                            if (item is not Position pos) continue;
                            pos.PayMethod = (PayMethod)int.Parse(value);
                            item = pos;
                        }
                        break;
                    case "1212":
                        {
                            if (item is not Position pos) continue;
                            pos.Type = (PositionType)int.Parse(value);
                            item = pos;
                        }
                        break;
                }
            }

            receipt.Items = [..items];
            Workspace.Instance.Receipts.Add(receipt);
            ReceiptPanel.UpdateReceiptList();
            Close();
        }
    }
}