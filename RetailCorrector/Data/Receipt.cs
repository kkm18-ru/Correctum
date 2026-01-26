using RetailCorrector.Enums;
using RetailCorrector.Exceptions;
using RetailCorrector.Utils;

namespace RetailCorrector.Data
{
    internal class Receipt(bool @readonly = false) : IUnionReceiptPosition
    {
        public required Guid ParseId { get; init; }
        public uint DocNumber { get; set; }
        public string RegId { get; set; }
        public string StorageId { get; set; }
        public uint SessionId { get; set; }
        public uint DocInSession { get; set; }
        public DateTime Created { get; set; }
        public uint DocSign { get; set; }
        public Operation Type { get; set; }
        public Position[] Items { get; set; }
        public Payment Payment { get; set; }
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

        public Receipt Clone()
        {
            var receipt = new Receipt
            {
                ParseId = ParseId,
                BuyerContact = $"{BuyerContact}",
                DocNumber = DocNumber,
                DocInSession = DocInSession,
                DocSign = DocSign,
                TotalSum = TotalSum,
                Created = Created,
                EditsCount = EditsCount,
                Payment = Payment,
                RegId = $"{RegId}",
                Type = Type,
                SessionId = SessionId,
                TaxationSystem = TaxationSystem,
                Items = new Position[Items.Length],
                StorageId = $"{StorageId}"
            };
            Array.Copy(Items, receipt.Items, Items.Length);
            return receipt;
        }
    }
}
