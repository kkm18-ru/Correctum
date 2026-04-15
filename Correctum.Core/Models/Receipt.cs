using Correctum.Core.Enums;
using Correctum.Core.ValueObjects;

namespace Correctum.Core.Models
{
    public struct Receipt
    {
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
        public TaxationSystem TaxationSystem { get; set; }

        #region Meta-Data
        public Guid Id
        {
            get
            {
                if (field == Guid.Empty)
                    field = Guid.CreateVersion7();
                return field;
            }
        }
        #endregion
    }
}
