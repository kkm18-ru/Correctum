using Correctum.Core.Enums;
using Correctum.Core.ValueObjects;
using Correctum.ValueObjects;

namespace Correctum.Core.Models
{
    /// <summary>
    /// Представляет кассовый чек
    /// </summary>
    public sealed class Receipt
    {
        /// <summary>
        /// Внутренний идентификатор
        /// </summary>
        public int Id { get; set; }
        public bool Marked { get; set; }
        /// <summary>
        /// Регистрационный номер ККТ
        /// </summary>
        public string? RegId { get; set; }
        /// <summary>
        /// Номер накопителя
        /// </summary>
        public string? StorageId { get; set; }
        /// <summary>
        /// Номер документа
        /// </summary>
        public uint? DocNumber { get; set; }
        /// <summary>
        /// Номер смены
        /// </summary>
        public uint? SessionId { get; set; }
        /// <summary>
        /// Номер чека за смену
        /// </summary>
        public uint? DocInSession { get; set; }
        /// <summary>
        /// Дата и время проведения чека
        /// </summary>
        public DateTime? Created { get; set; }
        /// <summary>
        /// Фискальный признак документа
        /// </summary>
        public FiscalSign? DocSign { get; set; }
        /// <summary>
        /// Признак расчета
        /// </summary>
        public ReceiptType? Type { get; set; }
        /// <summary>
        /// Множество предметов расчета
        /// </summary>
        public List<Position> Items { get; set; } = [];
        /// <summary>
        /// Сумма оплаты наличным способом
        /// </summary>
        public Currency? CashSum { get; set; }
        /// <summary>
        /// Сумма оплаты безналичным способом
        /// </summary>
        public Currency? NonCashSum { get; set; }
        /// <summary>
        /// Сумма аванса
        /// </summary>
        public Currency? AdvanceSum { get; set; }
        /// <summary>
        /// Сумма оплаты кредита
        /// </summary>
        public Currency? CreditSum { get; set; }
        /// <summary>
        /// Встречное предоставление
        /// </summary>
        public Currency? ProvisionSum { get; set; }
        /// <summary>
        /// Итоговая сумма расчета
        /// </summary>
        public Currency? TotalSum { get; set; }
        /// <summary>
        /// Телефон/электронный адрес покупателя
        /// </summary>
        public string? BuyerContact { get; set; }
        /// <summary>
        /// Применяемая система налогообложения
        /// </summary>
        public TaxationSystems? TaxationSystem { get; set; }
        public string? RetailPlace { get; set; }
        public string? RetailAddress { get; set; }
        public string? SenderAddress { get; set; }
        public string? OperatorName { get; set; }
        public Inn? OperatorInn { get; set; }
        public bool IsReadOnly { get; set; }

        public Receipt Clone() => new()
        {
            Items = [..Items.Select(i => i.Clone())],
            TotalSum = TotalSum,
            CashSum = CashSum,
            NonCashSum = NonCashSum,
            AdvanceSum = AdvanceSum,
            CreditSum = CreditSum,
            ProvisionSum = ProvisionSum,
            BuyerContact = (string?)BuyerContact?.Clone(),
            Created = Created,
            DocInSession = DocInSession,
            DocNumber = DocNumber,
            DocSign = DocSign,
            OperatorName = (string?)OperatorName?.Clone(),
            RetailPlace = (string?)RetailPlace?.Clone(),
            RetailAddress = (string?)RetailAddress?.Clone(),
            SenderAddress = (string?)SenderAddress?.Clone(),
            StorageId = (string?)StorageId?.Clone(),
            RegId = (string?)RegId?.Clone(),
            OperatorInn = OperatorInn,
            SessionId = SessionId,
            TaxationSystem = TaxationSystem,
            Type = Type
        };
    }
}
