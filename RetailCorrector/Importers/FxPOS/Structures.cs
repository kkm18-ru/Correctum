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
        public int MeasureUnit { get; set; }
        public ulong Cost { get; set; }
        public bool HasMark { get; set; }

        public Position ToPosition(Receipt receipt) => new()
        {
            Name = Name,
            Receipt = receipt,
            Price = Price,
            Quantity = Quantity,
            Cost = Cost,
            MeasureUnit = MeasureUnit switch
            {
                0 => Enums.MeasureUnit.Default,
                1 => Enums.MeasureUnit.g,
                2 => Enums.MeasureUnit.kg,
                3 => Enums.MeasureUnit.t,
                4 => Enums.MeasureUnit.cm,
                5 => Enums.MeasureUnit.dm,
                6 => Enums.MeasureUnit.m,
                7 => Enums.MeasureUnit.cm2,
                8 => Enums.MeasureUnit.dm2,
                9 => Enums.MeasureUnit.m2,
                10 => Enums.MeasureUnit.mL,
                11 => Enums.MeasureUnit.L,
                12 => Enums.MeasureUnit.m3,
                13 => Enums.MeasureUnit.kW_h,
                14 => Enums.MeasureUnit.GCal,
                15 => Enums.MeasureUnit.Day,
                16 => Enums.MeasureUnit.h,
                17 => Enums.MeasureUnit.min,
                18 => Enums.MeasureUnit.sec,
                19 => Enums.MeasureUnit.kByte,
                20 => Enums.MeasureUnit.MByte,
                21 => Enums.MeasureUnit.GByte,
                22 => Enums.MeasureUnit.TByte,
                23 => Enums.MeasureUnit.Other,
            },
            PayMethod = (PayMethod)PayMethod,
            Type = (Type, HasMark) switch
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
            TaxRate = TaxRate switch
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
