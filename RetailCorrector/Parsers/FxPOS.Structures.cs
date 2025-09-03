using RetailCorrector.Data;
using RetailCorrector.Enums;

namespace RetailCorrector.Parsers
{
    internal partial class FxPOS
    {
        private struct Document
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public ulong Total { get; set; }
            public ulong Sign { get; set; }
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

            public static explicit operator Position(InnerPosition raw) => new()
            {
                Name = raw.Name,
                Price = raw.Price,
                Quantity = raw.Quantity,
                Cost = raw.Cost,
                MeasureUnit = raw.MeasureUnit switch
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
                PayMethod = (PayMethod)raw.PayMethod,
                Type = raw.Type switch
                {
                    1 => PositionType.Product,
                    2 => PositionType.Excise,
                    3 => PositionType.Work,
                    4 => PositionType.Service,
                    10 => PositionType.Payment,
                    12 => PositionType.Other,
                    13 => PositionType.Agent,
                    14 => PositionType.Lottery,
                    15 => PositionType.IntellectualProperty,
                    16 => PositionType.Forfeit,
                    17 => PositionType.ResortFee,
                    18 => PositionType.FromBankAgent,
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
}
