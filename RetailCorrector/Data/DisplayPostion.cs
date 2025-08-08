using RetailCorrector.Enums;
using System.Diagnostics;

namespace RetailCorrector.Data
{
    [DebuggerDisplay("{Price}  {Name}")]
    internal readonly struct DisplayPostion(Position origin)
    {
        public string Name { get; } = origin.Name;
        public string Type { get; } = origin.Type switch
        {
            PositionType.Product                => "Т",
            PositionType.Excise                 => "АТ",
            PositionType.Work                   => "Р",
            PositionType.Service                => "У",
            PositionType.Gambling               => "СА",
            PositionType.GamblingWin            => "ВА",
            PositionType.Lottery                => "СЛ",
            PositionType.LotteryWin             => "ВЛ",
            PositionType.IntellectualProperty   => "РИД",
            PositionType.Payment                => "П",
            PositionType.Agent                  => "АВ",
            PositionType.Forfeit                => "В",
            PositionType.Other                  => "ИПР",
            PositionType.PropertyLaw            => "ИМУЩЕСТВЕННОЕ ПРАВО",
            PositionType.NonOperatingIncome     => "ВНЕРЕАЛИЗАЦИОННЫЙ ДОХОД",
            PositionType.OtherPayment           => "ИНЫЕ ПЛАТЕЖИ И ВЗНОСЫ",
            PositionType.TraceFee               => "ТОРГОВЫЙ СБОР",
            PositionType.ResortFee              => "КУРОРТНЫЙ СБОР",
            PositionType.Deposit                => "ЗАЛОГ",
            PositionType.Outcome                => "РАСХОД",
            PositionType.PensionInsuranceIE     => "ВЗНОСЫ НА ОПС ИП",
            PositionType.PensionInsurance       => "ВЗНОСЫ НА ОПС",
            PositionType.MedicalInsuranceIE     => "ВЗНОСЫ НА ОМС ИП",
            PositionType.MedicalInsurance       => "ВЗНОСЫ НА ОМС",
            PositionType.SocialInsurance        => "ВЗНОСЫ НА ОСС",
            PositionType.Casino                 => "ПК",
            PositionType.FromBankAgent          => "ВЫДАЧА ДС",            
            PositionType.ExciseNonMark          => "АТНМ",
            PositionType.ExciseMark             => "АТМ",
            PositionType.NonMark                => "ТНМ",
            PositionType.Mark                   => "ТМ",
            _                                   => "???"
        };
        public string PayMethod { get; } = origin.PayMethod switch
        {
            Enums.PayMethod.FullPrepaid => "ПРЕДОПЛАТА 100%",
            Enums.PayMethod.PartPrepaid => "ПРЕДОПЛАТА",
            Enums.PayMethod.Advance     => "АВАНС",
            Enums.PayMethod.FullPayment => "ПОЛНЫЙ РАСЧЕТ",
            Enums.PayMethod.PartPayment => "ЧАСТИЧНЫЙ РАСЧЕТ И КРЕДИТ",
            Enums.PayMethod.ToCredit    => "ПЕРЕДАЧА В КРЕДИТ",
            Enums.PayMethod.Postpaid    => "ОПЛАТА КРЕДИТА",
            _                           => "???",
        };
        public string TaxRate { get; } = origin.TaxRate switch
        {
            Enums.TaxRate.Calc5     => "НДС 5/105",
            Enums.TaxRate.Calc7     => "НДС 7/107",
            Enums.TaxRate.Calc10    => "НДС 10/110",
            Enums.TaxRate.Calc20    => "НДС 20/120",
            Enums.TaxRate.Without   => "-",
            Enums.TaxRate.Tax0      => "НДС 0%",
            Enums.TaxRate.Tax5      => "НДС 5%",
            Enums.TaxRate.Tax7      => "НДС 7%",
            Enums.TaxRate.Tax10     => "НДС 10%",
            Enums.TaxRate.Tax20     => "НДС 20%",
            _                       => "???",
        };
        public string Price { get; } = $"{(origin.Price * 0.01):C2}";
        public string Quantity { get; } = $"{(origin.Quantity * 0.001):F3}{origin.MeasureUnit switch
        {
            null                => "",
            MeasureUnit.Default => " шт.",
            MeasureUnit.g       => " г",
            MeasureUnit.kg      => " кг",
            MeasureUnit.t       => " т",
            MeasureUnit.cm      => " см",
            MeasureUnit.dm      => " дм",
            MeasureUnit.m       => " м",
            MeasureUnit.cm2     => " кв. см",
            MeasureUnit.dm2     => " кв. дм",
            MeasureUnit.m2      => " кв. м",
            MeasureUnit.mL      => " мл",
            MeasureUnit.L       => " д",
            MeasureUnit.m3      => " куб. м",
            MeasureUnit.kW_h    => " кВт * ч",
            MeasureUnit.GCal    => " Гкал",
            MeasureUnit.Day     => " д",
            MeasureUnit.h       => " ч",
            MeasureUnit.min     => " мин",
            MeasureUnit.sec     => " с",
            MeasureUnit.kByte   => " Кбайт",
            MeasureUnit.MByte   => " Мбайт",
            MeasureUnit.GByte   => " Гбайт",
            MeasureUnit.TByte   => " Тбайт",
            _                   => " ???"
        }}";
        public string Cost { get; } = $"{(origin.Cost * 0.01):C2}";
    }
}
