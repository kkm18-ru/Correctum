using Correctum.Core.Enums;
using Correctum.Core.ValueObjects;

namespace Correctum.Core.Models
{
    /// <summary>
    /// Представляет предмет расчета кассового чека
    /// </summary> 
    public readonly struct Position
    {
        /// <summary>
        /// Внутренний идентификатор
        /// </summary>
        public uint Id { get; init; }
        /// <summary>
        /// Наименование предмета расчета
        /// </summary>
        public string? Name { get; init; }
        /// <summary>
        /// Признак предмета расчета
        /// </summary>
        public PositionType? Type { get; init; }
        /// <summary>
        /// Признак способа расчета
        /// </summary>
        public CalculationMethod? CalcMethod { get; init; }
        /// <summary>
        /// Ставка НДС
        /// </summary>
        public NDSRate? TaxRate { get; init; }
        /// <summary>
        /// Мера количества предмера расчета
        /// </summary>
        public UnitOfMeasure? Unit { get; init; }
        /// <summary>
        /// Цена за единицу предмета расчета с учетом скидок и наценок
        /// </summary>
        public Currency? Price { get; init; }
        /// <summary>
        /// Количество предмета расчета
        /// </summary>
        public decimal? Quantity { get; init; }
        /// <summary>
        /// Стоимость предмета расчета с учетом скидок и наценок
        /// </summary>
        public Currency? Cost { get; init; }
        /// <summary>
        /// Внутренний идентификатор кассового чека
        /// </summary>
        public uint ReceiptId { get; init; }
    }
}
