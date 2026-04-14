using RetailCorrector.Enums;
using RetailCorrector.Exceptions;
using RetailCorrector.ValueObjects;

namespace RetailCorrector.Data
{
    public struct Receipt(bool @readonly = false)
    {
        public required Guid ParseId { get; init; }
        public uint DocNumber { get; set; }
        public string RegId { get; set; }
        public string StorageId { get; set; }
        public uint SessionId { get; set; }
        public uint DocInSession { get; set; }
        public DateTime Created { get; set; }
        public FiscalSign? DocSign { get; set; }
        public Operation Type { get; set; }
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

        public int EditsCount
        {
            get;
            set
            {
                if (value == field) return;
                if (IsReadOnly && value != 0)
                    throw new ReceiptReadOnlyException();
                if (value < field)
                    throw new NotImplementedException("Отмена действий в данный момент невозможна");
                field = value;
            }
        }

        public bool IsReadOnly { get; } = @readonly;
        #endregion
    }
}
