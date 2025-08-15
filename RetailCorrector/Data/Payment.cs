namespace RetailCorrector.Data
{
    internal struct Payment
    {
        public ulong Cash { get; set; }
        public ulong ECash { get; set; }
        public ulong Pre { get; set; }
        public ulong Post { get; set; }
        public ulong Provision { get; set; }
    }
}
