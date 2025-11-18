using RetailCorrector.Data;
using RetailCorrector.Enums;
using System.Text.Json.Nodes;

namespace RetailCorrector.Deserializers
{
    internal sealed partial class OfdRuJson : IReceiptDeserializer
    {
        private static OfdRuJson _instance = null!;
        public static OfdRuJson Instance => _instance ??= new OfdRuJson();
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
            receipt.Payment = new Payment
            {
                Cash = doc["Amount_Cash"]!.GetValue<ulong>(),
                ECash = doc["Amount_ECash"]!.GetValue<ulong>(),
                Pre = doc["Amount_Advance"]!.GetValue<ulong>(),
                Post = doc["Amount_Loan"]!.GetValue<ulong>(),
                Provision = doc["Amount_Granting"]!.GetValue<ulong>(),
            };
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
