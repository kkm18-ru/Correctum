using Correctum.Core.Enums;
using Correctum.Core.ValueObjects;

namespace Correctum.Core.Models
{
    public struct Position
    {
        public string Name { get; set; }
        public PayMethod PayMethod { get; set; }
        public PositionType Type { get; set; }
        public TaxRate TaxRate { get; set; }
        public Currency Price { get; set; }
        public ulong Quantity { get; set; }
        public PositionUnit? Unit { get; set; }
        public Currency Cost { get; set; }
    }
}
