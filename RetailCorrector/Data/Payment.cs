namespace RetailCorrector.Data
{
    internal struct Payment(ulong cash, ulong ecash, ulong pre, ulong post, ulong other)
    {
        public ulong Cash { get; set; } = cash;
        public ulong ECash { get; set; } = ecash;
        public ulong Pre { get; set; } = pre;
        public ulong Post { get; set; } = post;
        public ulong Provision { get; set; } = other;
    }
}
