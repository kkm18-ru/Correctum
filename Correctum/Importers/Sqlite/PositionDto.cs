using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Correctum.Utils;

namespace Correctum.Importers;

partial class Sqlite
{
    internal readonly struct PositionDto
    {
        public string? Name { get; init; }
        public long? Type { get; init; }
        public long? CalcMethod { get; init; }
        public long? Tax { get; init; }
        public long? Unit { get; init; }
        public long? Price { get; init; }
        public long? Quantity { get; init; }
        public long? Cost { get; init; }

        public static Position ToModel(PositionDto dto) => new()
        {
            Name = dto.Name,
            Cost = (Currency?)dto.Cost,
            Unit = (UnitOfMeasure?)dto.Unit,
            CalcMethod = (CalculationMethod?)dto.CalcMethod,
            Price = (Currency?)dto.Price,
            Quantity = (ulong?)dto.Quantity,
            TaxRate = dto.Tax is null ? null : EnumUtils.ToTaxRate((byte)dto.Tax),
            Type = (PositionType?)dto.Type
        };
    }
}
