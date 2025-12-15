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
                    receipt.DocSign = uint.Parse(value);
                    break;
                case "1037":
                    receipt.RegId = value;
                    break;
                case "1055":
                    receipt.TaxationSystem = (TaxationSystem)uint.Parse(value);
                    break;
                case "1041":
                    receipt.StorageId = value;
                    break;
                case "1038":
                    receipt.SessionId = uint.Parse(value);
                    break;
                case "1040":
                    receipt.DocNumber = uint.Parse(value);
                    break;
                case "1042":
                    receipt.DocInSession = uint.Parse(value);
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
                    pos.Quantity = (ulong)(double.Parse(value.Replace('.',',')) * 1000);
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
    }

    [GeneratedRegex(@"^ (\|  )*\+- \[(?<code>\d{4})\] [a-zA-Z0-9]+( +(?<value>[^ ]*))?$")]
    private static partial Regex SyntaxRegex();
}