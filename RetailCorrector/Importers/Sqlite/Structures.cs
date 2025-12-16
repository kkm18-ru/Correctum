using RetailCorrector.Enums;

namespace RetailCorrector.Importers;

internal partial class Sqlite
{
    private struct Document
    {
        public ulong id { get; set; }
        public DateTime date { get; set; }
        public Operation type { get; set; }
        public string sign { get; set; }
        public string devid { get; set; }
        public string drive { get; set; }
        public uint shift { get; set; }
        public uint doc { get; set; }
        public uint req { get; set; }
        public ulong cash { get; set; }
        public ulong ecash { get; set; }
        public ulong pre { get; set; }
        public ulong post { get; set; }
        public ulong other { get; set; }
        public ulong total { get; set; }
        public string buyer { get; set; }
    }

    private struct InnerPosition
    {
        public string name { get; set; }
        public PayMethod payment { get; set; }
        public PositionType type { get; set; }
        public TaxRate tax { get; set; }
        public MeasureUnit unit { get; set; }
        public ulong price { get; set; }
        public ulong quantity { get; set; }
        public ulong total { get; set; }
    }
}
