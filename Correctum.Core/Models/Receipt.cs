using Correctum.Core.Enums;
using Correctum.Core.ValueObjects;

namespace Correctum.Core.Models
{
    /// <summary>
    /// Представляет кассовый чек
    /// </summary>
    public readonly struct Receipt()
    {
        /// <summary>
        /// Внутренний идентификатор
        /// </summary>
        public Guid Id { get; } = Guid.CreateVersion7();
        /// <summary>
        /// Регистрационный номер ККТ
        /// </summary>
        public string? RegId { get; init; }
        /// <summary>
        /// Номер накопителя
        /// </summary>
        public string? StorageId { get; init; }
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
        public DateTime Created { get; init; }
        /// <summary>
        /// Фискальный признак документа
        /// </summary>
        public FiscalSign? DocSign { get; init; }
        /// <summary>
        /// Признак расчета
        /// </summary>
        public ReceiptType Type { get; init; }
        /// <summary>
        /// Множество предметов расчета
        /// </summary>
        public Position[] Items { get; init; }
        /// <summary>
        /// Информация об оплате чека
        /// </summary>
        public Payment Payment { get; init; }
        /// <summary>
        /// Итоговая сумма расчета
        /// </summary>
        public Currency TotalSum { get; init; }
        /// <summary>
        /// Телефон/электронный адрес покупателя
        /// </summary>
        public string? BuyerContact { get; init; }
        /// <summary>
        /// Применяемая система налогообложения
        /// </summary>
        public TaxationSystems TaxationSystem { get; init; }
    }
}
