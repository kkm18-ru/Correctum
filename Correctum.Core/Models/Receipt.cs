using Correctum.Core.Enums;
using Correctum.Core.ValueObjects;

namespace Correctum.Core.Models
{
    public struct Receipt()
    {
        public Guid Id { get; } = Guid.CreateVersion7();
        public required Guid ParseId { get; init; }
        public uint DocNumber { get; set; }
        public string RegId { get; set; }
        public string StorageId { get; set; }
        public uint SessionId { get; set; }
        public uint DocInSession { get; set; }
        public DateTime Created { get; set; }
        public FiscalSign? DocSign { get; set; }
        public ReceiptType Type { get; set; }
        public Position[] Items { get; set; }
        public Payment Payment { get; set; }
        public Currency TotalSum { get; set; }
        public string BuyerContact { get; set; }
        public TaxationSystems TaxationSystem { get; set; }
    }
}
