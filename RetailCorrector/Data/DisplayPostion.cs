using RetailCorrector.Enums;
using System.Diagnostics;

namespace RetailCorrector.Data
{
    [DebuggerDisplay("{Price}  {Name}")]
    internal readonly struct DisplayPostion(Position origin, DisplayReceipt receipt)
    {
        public string Receipt { get; } = receipt.ToString();
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
            PositionType.TradeFee               => "ТОРГОВЫЙ СБОР",
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
            Enums.TaxRate.Calc18    => "НДС 18/118",
            Enums.TaxRate.Calc20    => "НДС 20/120",
            Enums.TaxRate.Calc22    => "НДС 22/122",
            Enums.TaxRate.Without   => "-",
            Enums.TaxRate.Tax0      => "НДС 0%",
            Enums.TaxRate.Tax5      => "НДС 5%",
            Enums.TaxRate.Tax7      => "НДС 7%",
            Enums.TaxRate.Tax10     => "НДС 10%",
            Enums.TaxRate.Tax18     => "НДС 18%",
            Enums.TaxRate.Tax20     => "НДС 20%",
            Enums.TaxRate.Tax22     => "НДС 22%",
            _                       => "???",
        };
        public string Price { get; } = $"{(origin.Price * 0.01):F2}";

        public string Quantity { get; } = $"{(origin.Quantity * 0.001):F3}{origin.Unit switch
        {
            null => "",
            PositionUnit.Piece => " шт.",
            PositionUnit.Gram => " г",
            PositionUnit.Kilogram => " кг",
            PositionUnit.Ton => " т",
            PositionUnit.Centimeter => " см",
            PositionUnit.Decimeter => " дм",
            PositionUnit.Meter => " м",
            PositionUnit.SquareCentimeter => " кв. см",
            PositionUnit.SquareDecimeter => " кв. дм",
            PositionUnit.SquareMeter => " кв. м",
            PositionUnit.Milliliter => " мл",
            PositionUnit.Liter => " л",
            PositionUnit.CubicMeter => " куб. м",
            PositionUnit.KilowattHour => " кВт * ч",
            PositionUnit.Gigacalories => " Гкал",
            PositionUnit.Days => " д",
            PositionUnit.Hours => " ч",
            PositionUnit.Minutes => " мин",
            PositionUnit.Seconds => " с",
            PositionUnit.Kilobyte => " Кбайт",
            PositionUnit.Megabyte => " Мбайт",
            PositionUnit.Gigabyte => " Гбайт",
            PositionUnit.Terabyte => " Тбайт",
            _ => " ???"
        }}";
        public string Cost { get; } = $"{(origin.Cost * 0.01):F2}";
    }
}
