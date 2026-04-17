using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;

namespace Correctum.Core.Builders
{
    public sealed class PositionBuilder
    {
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

        public PositionBuilder SetName(string value)
        {
            _name = value;
            return this;
        }

        public PositionBuilder SetSubjectType(PositionType value)
        {
            _type = value;
            return this;
        }

        public PositionBuilder SetCalcMethod(CalculationMethod value)
        {
            _calcMethod = value;
            return this;
        }

        public PositionBuilder SetTaxRate(NDSRate value)
        {
            _ndsRate = value;
            return this;
        }

        public PositionBuilder SetUnit(UnitOfMeasure value)
        {
            _uom = value;
            return this;
        }

        public PositionBuilder SetPrice(Currency value)
        {
            _price = value;
            return this;
        }

        public PositionBuilder SetQuantity(ulong value)
        {
            _quantity = value;
            return this;
        }

        public PositionBuilder SetCost(Currency value)
        {
            _cost = value;
            return this;
        }

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
