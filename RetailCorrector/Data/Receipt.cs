using RetailCorrector.Enums;

namespace RetailCorrector.Data
{
    internal struct Receipt
    {
        public DateTime Created { get; set; }
        public ulong DocSign { get; set; }
        public Operation Type { get; set; }
        public IndustryData[] IndustryData { get; set; }
        public Position[] Items { get; set; }
        public Payment Payment { get; set; }
        public ulong TotalSum { get; set; }
    }
}
