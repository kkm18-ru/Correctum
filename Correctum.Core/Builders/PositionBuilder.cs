using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;

namespace Correctum.Core.Builders
{
    /// <summary>
    /// Построитель предмета расчёта (позиции чека) с fluent-интерфейсом.
    /// </summary>
    public sealed class PositionBuilder
    {
        /// <summary>
        /// Возвращает связанный построитель чека, если позиция была создана через <see cref="ReceiptBuilder.AddItem"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Выбрасывается, если построитель позиции создан вне контекста построителя чека.
        /// </exception>
        public ReceiptBuilder? Receipt
        {
            get => field ?? 
                throw new InvalidOperationException("Свойство Receipt недоступно, поскольку сборщик предмета расчёта был инициализирован вне контекста сборщика кассового чека.");
        }

        private string? _name;
        private PositionType? _type;
        private CalculationMethod? _calcMethod;
        private NDSRate? _ndsRate;
        private UnitOfMeasure? _uom;
        private Currency? _price;
        private ulong? _quantity;
        private Currency? _cost;

        /// <summary>
        /// Инициализирует пустой построитель позиции.
        /// </summary>
        public PositionBuilder()
        {
            _name = null;
            _type = null;
            _calcMethod = null;
            _ndsRate = null;
            _uom = null;
            _price = null;
            _quantity = null;
            _cost = null;
            Receipt = null;
        }

        /// <summary>
        /// Инициализирует построитель значениями существующей позиции.
        /// </summary>
        /// <param name="position">Исходная позиция чека.</param>
        public PositionBuilder(Position position)
        {
            _name = position.Name;
            _type = position.Type;
            _calcMethod = position.CalcMethod;
            _ndsRate = position.TaxRate;
            _uom = position.Unit;
            _price = position.Price;
            _quantity = position.Quantity;
            _cost = position.Cost;
            Receipt = null;
        }

        /// <summary>
        /// Инициализирует построитель позиции, связанный с построителем чека.
        /// </summary>
        /// <param name="receiptBuilder">Построитель чека, к которому будет добавлена позиция.</param>
        internal PositionBuilder(ReceiptBuilder receiptBuilder)
        {
            _name = null;
            _type = null;
            _calcMethod = null;
            _ndsRate = null;
            _uom = null;
            _price = null;
            _quantity = null;
            _cost = null;
            Receipt = receiptBuilder;
        }

        /// <summary>
        /// Устанавливает наименование предмета расчёта.
        /// </summary>
        /// <param name="value">Наименование позиции.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public PositionBuilder SetName(string? value)
        {
            _name = value;
            return this;
        }

        /// <summary>
        /// Устанавливает тип предмета расчёта.
        /// </summary>
        /// <param name="value">Тип позиции.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public PositionBuilder SetSubjectType(PositionType? value)
        {
            _type = value;
            return this;
        }

        /// <summary>
        /// Устанавливает способ расчёта по позиции.
        /// </summary>
        /// <param name="value">Способ расчёта.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public PositionBuilder SetCalcMethod(CalculationMethod? value)
        {
            _calcMethod = value;
            return this;
        }

        /// <summary>
        /// Устанавливает ставку НДС для позиции.
        /// </summary>
        /// <param name="value">Ставка НДС.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public PositionBuilder SetTaxRate(NDSRate? value)
        {
            _ndsRate = value;
            return this;
        }

        /// <summary>
        /// Устанавливает единицу измерения позиции.
        /// </summary>
        /// <param name="value">Единица измерения.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public PositionBuilder SetUnit(UnitOfMeasure? value)
        {
            _uom = value;
            return this;
        }

        /// <summary>
        /// Устанавливает цену единицы позиции.
        /// </summary>
        /// <param name="value">Цена в копейках.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public PositionBuilder SetPrice(Currency? value)
        {
            _price = value;
            return this;
        }

        /// <summary>
        /// Устанавливает количество единиц позиции.
        /// </summary>
        /// <param name="value">Количество.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public PositionBuilder SetQuantity(ulong? value)
        {
            _quantity = value;
            return this;
        }

        /// <summary>
        /// Устанавливает итоговую стоимость позиции.
        /// </summary>
        /// <param name="value">Стоимость в копейках.</param>
        /// <returns>Текущий экземпляр построителя.</returns>
        public PositionBuilder SetCost(Currency? value)
        {
            _cost = value;
            return this;
        }

        /// <summary>
        /// Создаёт экземпляр <see cref="Position"/> из установленных значений.
        /// </summary>
        /// <returns>Построенная позиция чека.</returns>
        public Position Build() => new()
        {
            Name = _name,
            Type = _type,
            CalcMethod = _calcMethod,
            TaxRate = _ndsRate,
            Unit = _uom,
            Price = _price,
            Quantity = _quantity,
            Cost = _cost,
        };
    }
}
