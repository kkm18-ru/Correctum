using Correctum.Core.Enums;
using Correctum.Core.ValueObjects;

namespace Correctum.Core.Models
{
    /// <summary>
    /// Представляет кассовый чек
    /// </summary>
    public readonly struct Receipt
    {
        /// <summary>
        /// Внутренний идентификатор
        /// </summary>
        public uint Id { get; init; }
        /// <summary>
        /// Регистрационный номер ККТ
        /// </summary>
        public ulong? RegId { get; init; }
        /// <summary>
        /// Номер накопителя
        /// </summary>
        public ulong? StorageId { get; init; }
        /// <summary>
        /// Номер документа
        /// </summary>
        public uint? DocNumber { get; init; }
        /// <summary>
        /// Номер смены
        /// </summary>
        public uint? SessionId { get; init; }
        /// <summary>
        /// Номер чека за смену
        /// </summary>
        public uint? DocInSession { get; init; }
        /// <summary>
        /// Дата и время проведения чека
        /// </summary>
        public DateTime? Created { get; init; }
        /// <summary>
        /// Фискальный признак документа
        /// </summary>
        public FiscalSign? DocSign { get; init; }
        /// <summary>
        /// Признак расчета
        /// </summary>
        public ReceiptType? Type { get; init; }
        /// <summary>
        /// Множество предметов расчета
        /// </summary>
        public Position[]? Items { get; init; }
        /// <summary>
        /// Сумма оплаты наличным способом
        /// </summary>
        public Currency? CashSum { get; init; }
        /// <summary>
        /// Сумма оплаты безналичным способом
        /// </summary>
        public Currency? NonCashSum { get; init; }
        /// <summary>
        /// Сумма аванса
        /// </summary>
        public Currency? AdvanceSum { get; init; }
        /// <summary>
        /// Сумма оплаты кредита
        /// </summary>
        public Currency? CreditSum { get; init; }
        /// <summary>
        /// Встречное предоставление
        /// </summary>
        public Currency? ProvisionSum { get; init; }
        /// <summary>
        /// Итоговая сумма расчета
        /// </summary>
        public Currency? TotalSum { get; init; }
        /// <summary>
        /// Телефон/электронный адрес покупателя
        /// </summary>
        public string? BuyerContact { get; init; }
        /// <summary>
        /// Применяемая система налогообложения
        /// </summary>
        public TaxationSystems? TaxationSystem { get; init; }
    }
}
