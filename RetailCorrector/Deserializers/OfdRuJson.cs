using RetailCorrector.Data;
using RetailCorrector.Enums;
using System.Text.Json.Nodes;

namespace RetailCorrector.Deserializers
{
    internal sealed partial class OfdRuJson : IReceiptDeserializer
    {
        public static OfdRuJson Instance => field ??= new OfdRuJson();
        public string Name => "ОФД.ру (JSON)";

        const string DATE_FORMAT = "yyyy-MM-ddTHH:mm:ss";

        public void Deserialize(string raw, ref Receipt receipt)
        {
            var json = JsonNode.Parse(raw);
            var doc = json!["Document"]!;
            var items = doc["Items"]!.AsArray();

            receipt.DocSign = ulong.Parse(doc["DecimalFiscalSign"]!.GetValue<string>());
            receipt.Created = DateTime.ParseExact(doc["DateTime"]!.GetValue<string>(), DATE_FORMAT, null);
            receipt.Type = (Operation)doc["OperationType"]!.GetValue<int>();
            receipt.TotalSum = doc["Amount_Total"]!.GetValue<ulong>();
            receipt.PaymentSums = [.. new PaymentSum[] 
            {
                new(PaymentType.Cash, doc["Amount_Cash"]!.GetValue<ulong>()),
                new(PaymentType.NonCash, doc["Amount_ECash"]!.GetValue<ulong>()),
                new(PaymentType.Advance, doc["Amount_Advance"]!.GetValue<ulong>()),
                new(PaymentType.Credit, doc["Amount_Loan"]!.GetValue<ulong>()),
                new(PaymentType.Other, doc["Amount_Granting"]!.GetValue<ulong>())
            }.Where(p => p.Sum > 0)];
            receipt.Items = new Position[items.Count];
            for(var i = 0; i< receipt.Items.Length; i++)
            {
                receipt.Items[i] = new Position
                {
                    Name = items[i]!["Name"]!.GetValue<string>(),
                    Price = items[i]!["Price"]!.GetValue<ulong>(),
                    Quantity = (ulong)(items[i]!["Quantity"]!.GetValue<double>() * 1000),
                    Cost = items[i]!["Total"]!.GetValue<ulong>(),
                    MeasureUnit = (MeasureUnit)items[i]!["ProductUnitOfMeasure"]!.GetValue<int>(),
                    PayMethod = (PayMethod)items[i]!["CalculationMethod"]!.GetValue<int>(),
                    TaxRate = (TaxRate)items[i]!["NDS_Rate"]!.GetValue<int>(),
                    Type = (PositionType)items[i]!["SubjectType"]!.GetValue<int>(),
                };
            }
        }
    }
}
