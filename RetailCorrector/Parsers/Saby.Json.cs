using RetailCorrector.Data;
using RetailCorrector.Enums;
using System.Text.Json.Nodes;

namespace RetailCorrector.Parsers;

internal partial class Saby
{
    private Position ToPosition(JsonNode? node) => new()
    {
        Name = node!["name"]!.GetValue<string>(),
        Cost = node["sum"]!.GetValue<ulong>(),
        Quantity = (ulong)(node["quantity"]!.GetValue<double>() * 1000),
        Price = node["price"]!.GetValue<ulong>(),
        MeasureUnit = (MeasureUnit)node["itemsQuantityMeasure"]!.GetValue<int>(),
        PayMethod = (PayMethod)node["paymentType"]!.GetValue<int>(),
        TaxRate = (TaxRate)node["nds"]!.GetValue<int>(),
        Type = (PositionType)node["productType"]!.GetValue<int>()
    };

    private Payment ToPayment(JsonNode node) => new()
    {
        Cash = node["cashTotalSum"]!.GetValue<ulong>(),
        ECash = node["ecashTotalSum"]!.GetValue<ulong>(),
        Pre = node["prepaidSum"]!.GetValue<ulong>(),
        Post = node["creditSum"]!.GetValue<ulong>(),
        Provision = node["provisionSum"]!.GetValue<ulong>()
    };

    private Receipt ToReceipt(JsonNode node) => new()
    {
        ParseId = parseInfo.Id,
        DocSign = node["fiscalSign"]!.GetValue<ulong>(),
        Type = (Operation)node["operationType"]!.GetValue<int>(),
        Payment = ToPayment(node),
        Items = [.. node["items"]!.AsArray().Select(ToPosition)]
    };
}