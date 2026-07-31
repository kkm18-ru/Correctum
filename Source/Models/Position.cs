using Correctum.Core.Enums;
using Correctum.Core.ValueObjects;

namespace Correctum.Core.Models
{
    /// <summary>
    /// Представляет предмет расчета кассового чека
    /// </summary> 
    public sealed class Position
    {
        public int Id { get; set; }
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
        public Currency? Price { get; set; }
        /// <summary>
        /// Количество предмета расчета
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// Стоимость предмета расчета с учетом скидок и наценок
        /// </summary>
        public Currency? Cost { get; set; }

        public Receipt Receipt { get; set; }
        public int ReceiptId { get; set; }

        public Position Clone() => new()
        {
            Name = (string?)Name?.Clone(),
            Quantity = Quantity,
            CalcMethod = CalcMethod,
            TaxRate = TaxRate,
            Type = Type,
            Unit = Unit,
            Price = Price,
            Cost = Cost
        };
    }
}
