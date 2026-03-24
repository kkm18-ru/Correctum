using Correctum.Enums;
using Correctum.Models;
using System.Text.Json.Serialization;

namespace Correctum.ApiClients.SabyClient;

[method: JsonConstructor]
internal readonly struct SabyReceiptItem
    (string name, ulong price, decimal quantity, ulong sum,
    int? itemsQuantityMeasure, int? nds, int? productType, int? paymentType)
{
    public string Name { get; } = name;
    public ulong Price { get; } = price;
    public decimal Quantity { get; } = quantity;
    public ulong Cost { get; } = sum;
    public int? Unit { get; } = itemsQuantityMeasure;
    public int? NdsRate { get; } = nds;
    public int? SubjectType { get; } = productType;
    public int? CalcMethod { get; } = paymentType;

    public Position ToPosition() => new()
    {
        Name = Name,
        Price = Price,
        Cost = Cost,
        Quantity = (ulong)Math.Round(Quantity * 1000),
        Unit = (PositionUnit?)Unit,
        SubjectType = (PositionType?)SubjectType ?? PositionType.Unknown,
        CalculationMethod = (PositionCalculationMethod?)CalcMethod 
            ?? PositionCalculationMethod.Unknown,
        TaxRate = NdsRate switch // todo check
        {
            1 => TaxRate.Tax20,
            2 => TaxRate.Tax10,
            3 => TaxRate.Calc20,
            4 => TaxRate.Calc10,
            5 => TaxRate.Tax0,
            6 => TaxRate.Without,
            7 => TaxRate.Tax5,
            8 => TaxRate.Tax7,
            9 => TaxRate.Calc5,
            10 => TaxRate.Calc7,
            11 => TaxRate.Tax22,
            12 => TaxRate.Calc22,
            _ => TaxRate.Unknown
        }
    };
}