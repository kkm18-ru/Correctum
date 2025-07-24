namespace RetailCorrector.Data
{
    internal struct IndustryData
    {
        public byte GosId { get; set; }
        public DateOnly DocDate { get; set; }
        public long DocNumber { get; set; }
        public string Value { get; set; }
    }
}
