using RetailCorrector.Enums;

namespace RetailCorrector.Importers;

internal partial class Sqlite
{
    private readonly struct Document
    {
        public ulong id { get; init; }
        public DateTime date { get; init; }
        public Operation type { get; init; }
        public string sign { get; init; }
        public string devid { get; init; }
        public string drive { get; init; }
        public uint shift { get; init; }
        public uint doc { get; init; }
        public uint req { get; init; }
        public ulong cash { get; init; }
        public ulong ecash { get; init; }
        public ulong pre { get; init; }
        public ulong post { get; init; }
        public ulong other { get; init; }
        public ulong total { get; init; }
        public string buyer { get; init; }
    }

    private readonly struct InnerPosition
    {
        public string name { get; init; }
        public PayMethod payment { get; init; }
        public PositionType type { get; init; }
        public byte tax { get; init; }
        public MeasureUnit unit { get; init; }
        public ulong price { get; init; }
        public ulong quantity { get; init; }
        public ulong total { get; init; }
    }
}
