using RetailCorrector.Data;
using RetailCorrector.Enums;
using System.Text.Json.Nodes;

namespace RetailCorrector.Parsers
{
    internal partial class OfdRu
    {
        private static Receipt ParseReceipt(JsonNode node, Guid parseId)
        {
            var items = node["Items"]!.AsArray();
            var receipt = new Receipt
            {
                ParseId = parseId,
                Created = ToDateTime(node["DocDateTime"]!.GetValue<string>()),
                DocSign = ulong.Parse(node["DecimalFiscalSign"]!.GetValue<string>()),
                TotalSum = node["TotalSumm"]!.GetValue<uint>(),
                Payment = ParsePayment(node),
                Items = new Position[items.Count],
                Type = ToOperation(node["OperationType"]!.GetValue<string>()),
                TaxationSystem = (TaxationSystem)node["TaxationType"]!.GetValue<int>()
            };
            for (var i = 0; i < items.Count; i++)
                receipt.Items[i] = ParsePosition(items[i]!);
            return receipt;
        }

        private static Position ParsePosition(JsonNode node) => new()
        {
            Name = node["Name"]!.GetValue<string>(),
            Price = node["Price"]!.GetValue<uint>(),
            Quantity = ToQuantity(node["Quantity"]!.GetValue<double>()),
            Cost = node["Total"]!.GetValue<uint>(),
            PayMethod = (PayMethod)node["CalculationMethod"]!.GetValue<int>(),
            MeasureUnit = (MeasureUnit)int.Parse(node["ProductUnitOfMeasure"]?.GetValue<string>() ?? "-1"),
            TaxRate = (TaxRate)node["NDS_Rate"]!.GetValue<int>(),
            Type = (PositionType)node["SubjectType"]!.GetValue<int>()
        };

        private static Payment ParsePayment(JsonNode node) => new()
        {
            Cash = node["CashSumm"]!.GetValue<uint>(),
            ECash = node["ECashSumm"]!.GetValue<uint>(),
            Pre = node["PrepaidSumm"]!.GetValue<uint>(),
            Post = node["CreditSumm"]!.GetValue<uint>(),
            Provision = node["ProvisionSumm"]!.GetValue<uint>(),
        };

        private static uint ToQuantity(double raw) => (uint)Math.Round(raw * 1000);

        private static Operation ToOperation(string text) => text.ToLower() switch
        {
            "income" => Operation.Income,
            "outcome" => Operation.Outcome,
            "refund income" => Operation.RefundIncome,
            "refund outcome" => Operation.RefundOutcome
        };

        private static DateTime ToDateTime(string text) =>
            DateTime.ParseExact(text, "yyyy'-'MM'-'dd'T'HH':'mm':'ss", null);
    }
}
