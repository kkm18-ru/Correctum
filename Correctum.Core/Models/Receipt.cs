using Correctum.Core.Enums;
using Correctum.Core.ValueObjects;

namespace Correctum.Core.Models
{
    /// <summary>
    /// Представляет кассовый чек
    /// </summary>
    public struct Receipt()
    {
        /// <summary>
        /// Внутренний идентификатор
        /// </summary>
        public Guid Id { get; } = Guid.CreateVersion7();
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
        public DateTime Created { get; set; }
        /// <summary>
        /// Фискальный признак документа
        /// </summary>
        public FiscalSign? DocSign { get; set; }
        /// <summary>
        /// Признак расчета
        /// </summary>
        public ReceiptType Type { get; set; }
        /// <summary>
        /// Множество предметов расчета
        /// </summary>
        public Position[] Items { get; set; }
        /// <summary>
        /// Информация об оплате чека
        /// </summary>
        public Payment Payment { get; set; }
        /// <summary>
        /// Итоговая сумма расчета
        /// </summary>
        public Currency TotalSum { get; set; }
        /// <summary>
        /// Телефон/электронный адрес покупателя
        /// </summary>
        public string? BuyerContact { get; set; }
        /// <summary>
        /// Применяемая система налогообложения
        /// </summary>
        public TaxationSystems TaxationSystem { get; set; }
    }
}
