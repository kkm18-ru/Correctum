using RetailCorrector.Enums;
using RetailCorrector.Exceptions;

namespace RetailCorrector.Data
{
    internal struct Receipt(bool @readonly = false)
    {
        public required Guid ParseId { get; init; }
        public int DocNumber { get; set; }
        public DateTime Created { get; set; }
        public ulong DocSign { get; set; }
        public Operation Type { get; set; }
        public Position[] Items { get; set; }
        public PaymentSum[] PaymentSums { get; set; }
        public ulong TotalSum { get; set; }
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
