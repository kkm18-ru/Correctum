using RetailCorrector.Enums;
using RetailCorrector.Utils;

namespace RetailCorrector.Data
{
    internal struct Position(string name, ulong price, ulong quantity) : IUnionReceiptPosition
    {
        public string Name { get; set; } = name;
        public PayMethod PayMethod { get; set; }
        public PositionType Type { get; set; }
        public TaxRate TaxRate { get; set; }
        public ulong Price { get; set; } = price;
        public ulong Quantity { get; set; } = quantity;
        public MeasureUnit? MeasureUnit { get; set; }
        public ulong Cost { get; set; } = price * quantity / 1000;
    }
}
