using RetailCorrector.Data;
using RetailCorrector.Enums;
using System.Text.Json.Nodes;

namespace RetailCorrector.Parsers.Taxcom
{
    internal partial class TaxcomPresenter
    {
        private static IEnumerable<(string, int)> ParseReceipts(JsonArray arr)
        {
            foreach (var doc in arr)
                yield return (doc["fiscalSign"].GetValue<string>(),
                    doc["documentNumber"].GetValue<int>());
        }

        private static Receipt ParseReceipt(JsonArray arr)
        {
            var payment = new Payment();
            var receipt = new Receipt();
            var positions = new List<Position>();
            foreach (var tag in arr)
            {
                switch (tag["Tag"].GetValue<string>())
                {
                    case "1012":
                        var value = tag["Value"].GetValue<string>();
                        receipt.Created = DateTime.ParseExact(value, "yyyy'-'MM'-'dd' 'HH':'mm':'ss", null);
                        break;
                    case "1077":
                        receipt.DocSign = tag["Value"].GetValue<ulong>();
                        break;
                    case "1031":
                        value = tag["Value"].GetValue<string>().Replace(".", "");
                        payment.Cash = ulong.Parse(value);
                        break;
                    case "1081":
                        value = tag["Value"].GetValue<string>().Replace(".", "");
                        payment.ECash = ulong.Parse(value);
                        break;
                    case "1215":
                        value = tag["Value"].GetValue<string>().Replace(".", "");
                        payment.Pre = ulong.Parse(value);
                        break;
                    case "1216":
                        value = tag["Value"].GetValue<string>().Replace(".", "");
                        payment.Post = ulong.Parse(value);
                        break;
                    case "1217":
                        value = tag["Value"].GetValue<string>().Replace(".", "");
                        payment.Provision = ulong.Parse(value);
                        break;
                    case "1020":
                        value = tag["Value"].GetValue<string>().Replace(".", "");
                        receipt.TotalSum = ulong.Parse(value);
                        break;
                    case "1054":
                        value = tag["Value"].GetValue<string>();
                        receipt.Type = (Operation)byte.Parse(value);
                        break;
                    case "1059":
                        var values = tag["Value"].AsArray();
                        positions.Add(ParsePosition(values));
                        break;

                }
            }
            receipt.Payment = payment;
            receipt.Items = [.. positions];
            return receipt;
        }

        private static Position ParsePosition(JsonArray arr)
        {
            var pos = new Position();
            foreach (var posTag in arr)
            {
                switch (posTag["Tag"].GetValue<string>())
                {
                    case "1030":
                        pos.Name = posTag["Value"].GetValue<string>();
                        break;
                    case "1079":
                        pos.Price = ulong.Parse(posTag["Value"].GetValue<string>().Replace(".", ""));
                        break;
                    case "1023":
                        pos.Quantity = (ulong)(double.Parse(posTag["Value"].GetValue<string>()) * 1000);
                        break;
                    case "1043":
                        pos.Cost = ulong.Parse(posTag["Value"].GetValue<string>().Replace(".", ""));
                        break;
                    case "1199":
                        pos.TaxRate = (TaxRate)byte.Parse(posTag["Value"].GetValue<string>());
                        break;
                    case "1214":
                        pos.PayMethod = (PayMethod)byte.Parse(posTag["Value"].GetValue<string>());
                        break;
                    case "1212":
                        pos.Type = (PositionType)byte.Parse(posTag["Value"].GetValue<string>());
                        break;
                }
            }
            return pos;
        }
    }
}
