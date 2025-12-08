using System.Text.RegularExpressions;
using RetailCorrector.Data;
using RetailCorrector.Enums;

namespace RetailCorrector.Deserializers;

internal sealed partial class PlatformOfd : IReceiptDeserializer
{
    public static PlatformOfd Instance => field ??= new PlatformOfd();
    public string Name => "Платформа ОФД";

    public void Deserialize(string raw, ref Receipt receipt)
    {
        var items = new List<Position>();
        var paysums = new List<PaymentSum>(2);
        var taxsums = new List<TaxSum>(3);
        var lines = raw.Replace("\r", "").Split("\n");
        object? item = null;

        foreach (var line in lines)
        {
            var match = SyntaxRegex().Match(line);
            if (!match.Success) continue;
            if (item is not null && !match.Groups[1].Success)
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
                    paysums.Add(new PaymentSum(PaymentType.Cash, ulong.Parse(value)));
                    break;
                case "1081":
                    paysums.Add(new PaymentSum(PaymentType.NonCash, ulong.Parse(value)));
                    break;
                case "1215":
                    paysums.Add(new PaymentSum(PaymentType.Advance, ulong.Parse(value)));
                    break;
                case "1216":
                    paysums.Add(new PaymentSum(PaymentType.Credit, ulong.Parse(value)));
                    break;
                case "1217":
                    paysums.Add(new PaymentSum(PaymentType.Other, ulong.Parse(value)));
                    break;
                case "1102":
                    taxsums.Add(new TaxSum(TaxRate.Tax20, ulong.Parse(value)));
                    break;
                case "1103":
                    taxsums.Add(new TaxSum(TaxRate.Tax10, ulong.Parse(value)));
                    break;
                case "1104":
                    taxsums.Add(new TaxSum(TaxRate.Tax0, ulong.Parse(value)));
                    break;
                case "1105":
                    taxsums.Add(new TaxSum(TaxRate.Without, ulong.Parse(value)));
                    break;
                case "1106":
                    taxsums.Add(new TaxSum(TaxRate.Calc20, ulong.Parse(value)));
                    break;
                case "1107":
                    taxsums.Add(new TaxSum(TaxRate.Calc10, ulong.Parse(value)));
                    break;
                    // todo add 5,7,107,105
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
        receipt.PaymentSums = [.. paysums.Where(p => p.Sum > 0)];
    }

    [GeneratedRegex(@"^ (\|  )*\+- \[(?<code>\d{4})\] [a-zA-Z0-9]+( +(?<value>[^ ]*))?$")]
    private static partial Regex SyntaxRegex();
}