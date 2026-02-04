using RetailCorrector.Data;
using RetailCorrector.Enums;

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
                0 => PositionUnit.Piece,
                1 => PositionUnit.Gram,
                2 => PositionUnit.Kilogram,
                3 => PositionUnit.Ton,
                4 => PositionUnit.Centimeter,
                5 => PositionUnit.Decimeter,
                6 => PositionUnit.Meter,
                7 => PositionUnit.SquareCentimeter,
                8 => PositionUnit.SquareDecimeter,
                9 => PositionUnit.SquareMeter,
                10 => PositionUnit.Milliliter,
                11 => PositionUnit.Liter,
                12 => PositionUnit.CubicMeter,
                13 => PositionUnit.KilowattHour,
                14 => PositionUnit.Gigacalories,
                15 => PositionUnit.Days,
                16 => PositionUnit.Hours,
                17 => PositionUnit.Minutes,
                18 => PositionUnit.Seconds,
                19 => PositionUnit.Kilobyte,
                20 => PositionUnit.Megabyte,
                21 => PositionUnit.Gigabyte,
                22 => PositionUnit.Terabyte,
                23 => PositionUnit.Other,
            },
            PayMethod = (PayMethod)raw.PayMethod,
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
                0 => Enums.TaxRate.Without,
                5 => Enums.TaxRate.Tax5,
                7 => Enums.TaxRate.Tax7,
                10 => Enums.TaxRate.Tax10,
                18 => Enums.TaxRate.Tax20,
                20 => Enums.TaxRate.Tax20,
            }
        };
    }
}
