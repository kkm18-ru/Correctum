using RetailCorrector.Enums;

namespace RetailCorrector.Data
{
    internal struct Position
    {
        public string Name { get; set; }
        public PositionCode Code { get; set; }
        public IndustryData[] IndustryData { get; set; }
        public PayMethod PayMethod { get; set; }
        public PositionType Type { get; set; }
        public TaxRate TaxRate { get; set; }
        public ulong Price { get; set; }
        public ulong Quantity { get; set; }
        public MeasureUnit? MeasureUnit { get; set; }
        public ulong Cost { get; set; }
    }
}
