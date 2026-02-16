using RetailCorrector.Enums;

namespace RetailCorrector.Data
{
    public struct Position(string name, ulong price, ulong quantity)
    {
        public string Name { get; set; } = name;
        public PayMethod PayMethod { get; set; }
        public PositionType Type { get; set; }
        public TaxRate TaxRate { get; set; }
        public ulong Price { get; set; } = price;
        public ulong Quantity { get; set; } = quantity;
        public PositionUnit? Unit { get; set; }
        public ulong Cost { get; set; } = price * quantity / 1000;
    }
}
