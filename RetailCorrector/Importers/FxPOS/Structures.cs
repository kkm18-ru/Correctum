using Correctum.Core.Models;
using Correctum.Core.Enums;

namespace RetailCorrector.Importers;

internal partial class FxPOS
{
    private struct Document
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public ulong Total { get; set; }
        public uint Sign { get; set; }
        public int Type { get; set; }
    }

    private struct InnerPosition
    {
        public string Name { get; set; }
        public int PayMethod { get; set; }
        public int Type { get; set; }
        public int TaxRate { get; set; }
        public ulong Price { get; set; }
        public ulong Quantity { get; set; }
        public int Unit { get; set; }
        public ulong Cost { get; set; }
        public bool HasMark { get; set; }

        public static explicit operator Position(InnerPosition raw) => new()
        {
            Name = raw.Name,
            Price = raw.Price,
            Quantity = raw.Quantity,
            Cost = raw.Cost,
            Unit = raw.Unit switch
            {
                0 => UnitOfMeasure.Piece,
                1 => UnitOfMeasure.Gram,
                2 => UnitOfMeasure.Kilogram,
                3 => UnitOfMeasure.Ton,
                4 => UnitOfMeasure.Centimeter,
                5 => UnitOfMeasure.Decimeter,
                6 => UnitOfMeasure.Meter,
                7 => UnitOfMeasure.SquareCentimeter,
                8 => UnitOfMeasure.SquareDecimeter,
                9 => UnitOfMeasure.SquareMeter,
                10 => UnitOfMeasure.Milliliter,
                11 => UnitOfMeasure.Liter,
                12 => UnitOfMeasure.CubicMeter,
                13 => UnitOfMeasure.KilowattHour,
                14 => UnitOfMeasure.Gigacalories,
                15 => UnitOfMeasure.Days,
                16 => UnitOfMeasure.Hours,
                17 => UnitOfMeasure.Minutes,
                18 => UnitOfMeasure.Seconds,
                19 => UnitOfMeasure.Kilobyte,
                20 => UnitOfMeasure.Megabyte,
                21 => UnitOfMeasure.Gigabyte,
                22 => UnitOfMeasure.Terabyte,
                23 => UnitOfMeasure.Other,
            },
            PayMethod = (CalculationMethod)raw.PayMethod,
            Type = (raw.Type, raw.HasMark) switch
            {
                (1, false) => PositionType.Product,
                (1, true) => PositionType.Mark,
                (2, false) => PositionType.Excise,
                (2, true) => PositionType.ExciseMark,
                (3, _) => PositionType.Work,
                (4, _) => PositionType.Service,
                (10, _) => PositionType.Payment,
                (12, _) => PositionType.Other,
                (13, _) => PositionType.Agent,
                (14, _) => PositionType.Lottery,
                (15, _) => PositionType.IntellectualProperty,
                (16, _) => PositionType.Forfeit,
                (17, _) => PositionType.ResortFee,
                (18, _) => PositionType.FromBankAgent,
            },
            TaxRate = raw.TaxRate switch
            {
                0 => Correctum.Core.Enums.TaxRate.Without,
                5 => Correctum.Core.Enums.TaxRate.Tax5,
                7 => Correctum.Core.Enums.TaxRate.Tax7,
                10 => Correctum.Core.Enums.TaxRate.Tax10,
                18 => Correctum.Core.Enums.TaxRate.Tax20,
                20 => Correctum.Core.Enums.TaxRate.Tax20,
            }
        };
    }
}
