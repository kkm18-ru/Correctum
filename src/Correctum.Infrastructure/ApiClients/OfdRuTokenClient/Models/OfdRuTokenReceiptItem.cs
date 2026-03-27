using Correctum.Enums;
using Correctum.Models;

namespace Correctum.ApiClients.OfdRuTokenClient.Models;

internal readonly struct OfdRuTokenReceiptItem
{
    public string Name { get; init; }
    public ulong Price { get; init; }
    public double Quantity { get; init; }
    public ulong Total { get; init; }
    public PositionCalculationMethod CalculationMethod { get; init; }
    public string? ProductUnitOfMeasure { get; init; }
    public byte NDS_Rate { get; init; }
    public PositionType SubjectType { get; init; }

    public readonly Position ToPosition() => new()
    {
        Name = Name,
        Price = Price,
        Quantity = (ulong)Math.Round(Quantity * 1000),
        Cost = Total,
        CalculationMethod = CalculationMethod,
        TaxRate = (TaxRate)NDS_Rate,
        SubjectType = SubjectType,
        Unit = int.TryParse(ProductUnitOfMeasure, out var unit) ? null : (PositionUnit)unit
    };
}
