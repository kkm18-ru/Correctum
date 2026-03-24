using Correctum.Enums;
using Correctum.Models;
using System.Text.Json.Serialization;

namespace Correctum.ApiClients.FirstOfdClient;

[method: JsonConstructor]
internal readonly struct FirstOfdTicketItem(string name, ulong price, decimal quantity, ulong sum, int ndsRate, int calculationSubjectSign)
{
    public string Name { get; } = name;
    public ulong Price { get; } = price;
    public decimal Quantity { get; } = quantity;
    public ulong Total { get; } = sum;
    public int NdsRate { get; } = ndsRate;
    public int CalculationSubject { get; } = calculationSubjectSign;
    public int? Unit { get; }
    public int? Type { get; }

    public Position ToPosition() => new()
    {
        Name = Name,
        Cost = Total,
        CalculationMethod = (PositionCalculationMethod)CalculationSubject,
        Unit = (PositionUnit?)Unit,
        TaxRate = (TaxRate)NdsRate,
        Price = Price,
        SubjectType = (PositionType?)Type ?? PositionType.Unknown,
        Quantity = (ulong)Math.Round(Quantity * 1000),
    };
}
