using RetailCorrector.Enums;
using RetailCorrector.Exceptions;

namespace RetailCorrector.Data
{
    internal struct Receipt(bool @readonly = false)
    {
        public required Guid ParseId { get; set; }
        public DateTime Created { get; set; }
        public ulong DocSign { get; set; }
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
                if (_id == Guid.Empty)
                    _id = Guid.CreateVersion7();
                return _id;
            }
        }
        private Guid _id;
        public int EditsCount
        {
            get => _editsCount;
            set
            {
                if (value == _editsCount) return;
                if (IsReadOnly && value != 0)
                    throw new ReceiptReadOnlyException();
                if (value < _editsCount)
                    throw new NotImplementedException("Отмена действий в данный момент невозможна");
                _editsCount = value;
            }
        }
        private int _editsCount = 0;
        public bool IsReadOnly { get; } = @readonly;
        #endregion
    }
}
