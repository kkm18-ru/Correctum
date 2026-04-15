using Correctum.Core.Enums;
using Correctum.Core.ValueObjects;

namespace Correctum.Core.Models
{
    public struct Position
    {
        public string Name { get; set; }
        public CalculationMethod PayMethod { get; set; }
        public PositionType Type { get; set; }
        public NDSRate TaxRate { get; set; }
        public Currency Price { get; set; }
        public ulong Quantity { get; set; }
        public UnitOfMeasure? Unit { get; set; }
        public Currency Cost { get; set; }
    }
}
