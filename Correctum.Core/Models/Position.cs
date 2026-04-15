using Correctum.Core.Enums;
using Correctum.Core.ValueObjects;

namespace Correctum.Core.Models
{
    /// <summary>
    /// Представляет предмет расчета кассового чека
    /// </summary> 
    public struct Position
    {
        /// <summary>
        /// Наименование предмета расчета
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Признак предмета расчета
        /// </summary>
        public PositionType? Type { get; set; }
        /// <summary>
        /// Признак способа расчета
        /// </summary>
        public CalculationMethod? CalcMethod { get; set; }
        /// <summary>
        /// Ставка НДС
        /// </summary>
        public NDSRate? TaxRate { get; set; }
        /// <summary>
        /// Мера количества предмера расчета
        /// </summary>
        public UnitOfMeasure? Unit { get; set; }
        /// <summary>
        /// Цена за единицу предмета расчета с учетом скидок и наценок
        /// </summary>
        public Currency Price { get; set; }
        /// <summary>
        /// Количество предмета расчета
        /// </summary>
        public ulong Quantity { get; set; }
        /// <summary>
        /// Стоимость предмета расчета с учетом скидок и наценок
        /// </summary>
        public Currency Cost { get; set; }
    }
}
