using RetailCorrector.Enums;
using RetailCorrector.Utils;

namespace RetailCorrector.Data
{
    internal struct Position : IUnionReceiptPosition
    {
        public string Name { get; set; }
        public PayMethod PayMethod { get; set; }
        public PositionType Type { get; set; }
        public TaxRate TaxRate { get; set; }
        public ulong Price { get; set; }
        public ulong Quantity { get; set; }
        public MeasureUnit? MeasureUnit { get; set; }
        public ulong Cost { get; set; }
        public Receipt Receipt { get; init; }
    }
}
