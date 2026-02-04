using System.Collections.ObjectModel;
using RetailCorrector.Enums;

namespace RetailCorrector.Utils;

public record EnumLocale<TEnum>(TEnum Value, string Locale) where TEnum : Enum;

public static class EnumLocales
{
    public static readonly ReadOnlyCollection<EnumLocale<TaxationSystem>> TaxationSystems =
    [
        new(TaxationSystem.OSN, "ОСН"),
        new(TaxationSystem.USNIncome, "УСН доход"),
        new(TaxationSystem.USNIncomeExpenditure, "УСН доход-расход"),
        new(TaxationSystem.ENVD, "ЕНВД"),
        new(TaxationSystem.ESHN, "ЕСХН"),
        new(TaxationSystem.Patent, "Патент"),
    ];

    public static readonly ReadOnlyCollection<EnumLocale<Operation>> ReceiptTypes =
    [
        new(Operation.Income, "Приход"),
        new(Operation.Outcome, "Расход"),
        new(Operation.RefundIncome, "Возврат прихода"),
        new(Operation.RefundOutcome, "Возврат расхода"),
    ];

    public static readonly ReadOnlyCollection<EnumLocale<PositionUnit>> PositionUnits =
    [
        new(PositionUnit.Piece, "шт."),
        new(PositionUnit.Gram, "гр."),
        new(PositionUnit.Kilogram, "кг"),
        new(PositionUnit.Ton, "т"),
        new(PositionUnit.Centimeter, "см"),
        new(PositionUnit.Decimeter, "дм"),
        new(PositionUnit.Meter, "м"),
        new(PositionUnit.SquareCentimeter, "кв. см"),
        new(PositionUnit.SquareDecimeter, "кв. дм"),
        new(PositionUnit.SquareMeter, "кв. м"),
        new(PositionUnit.Milliliter, "мл"),
        new(PositionUnit.Liter, "л"),
        new(PositionUnit.CubicMeter, "куб. м"),
        new(PositionUnit.KilowattHour, "кВт·ч"),
        new(PositionUnit.Gigacalories, "ГКал"),
        new(PositionUnit.Days, "дн."),
        new(PositionUnit.Hours, "ч"),
        new(PositionUnit.Minutes, "мин."),
        new(PositionUnit.Seconds, "сек."),
        new(PositionUnit.Kilobyte, "КБ"),
        new(PositionUnit.Megabyte, "МБ"),
        new(PositionUnit.Gigabyte, "ГБ"),
        new(PositionUnit.Terabyte, "ТБ"),
        new(PositionUnit.Other, "др."),
    ];
    
    public static readonly ReadOnlyCollection<EnumLocale<TaxRate>> TaxRates =
    [
        new(TaxRate.Tax18, "НДС 18%"),
        new(TaxRate.Tax10, "НДС 10%"),
        new(TaxRate.Calc18, "НДС 18/118"),
        new(TaxRate.Calc10, "НДС 10/110"),
        new(TaxRate.Tax0, "НДС 0%"),
        new(TaxRate.Without, "Не облагается"),
        new(TaxRate.Tax20, "НДС 20%"),
        new(TaxRate.Calc20, "НДС 20/120"),
        new(TaxRate.Tax5, "НДС 5%"),
        new(TaxRate.Tax7, "НДС 7%"),
        new(TaxRate.Calc5, "НДС 5/105"),
        new(TaxRate.Calc7, "НДС 7/107"),
        new(TaxRate.Tax22, "НДС 22%"),
        new(TaxRate.Calc22, "НДС 22/122"),
    ];

    public static readonly ReadOnlyCollection<EnumLocale<PositionType>> PositionTypes =
    [
        new(PositionType.Product, "Т"),
        new(PositionType.Excise, "АТ"),
        new(PositionType.Work, "Р"),
        new(PositionType.Service, "У"),
        new(PositionType.Gambling, "СА"),
        new(PositionType.GamblingWin, "ВА"),
        new(PositionType.Lottery, "СЛ"),
        new(PositionType.LotteryWin, "ВЛ"),
        new(PositionType.IntellectualProperty, "РИД"),
        new(PositionType.Payment, "П"),
        new(PositionType.Agent, "АВ"),
        new(PositionType.Forfeit, "В"),
        new(PositionType.Other, "ИПР"),
        new(PositionType.PropertyLaw, "ИМУЩЕСТВЕННОЕ ПРАВО"),
        new(PositionType.NonOperatingIncome, "ВНЕРЕАЛИЗАЦИОННЫЙ ДОХОД"),
        new(PositionType.OtherPayment, "ИНЫЕ ПЛАТЕЖИ И ВЗНОСЫ"),
        new(PositionType.TradeFee, "ТОРГОВЫЙ СБОР"),
        new(PositionType.ResortFee, "КУРОРТНЫЙ СБОР"),
        new(PositionType.Deposit, "ЗАЛОГ"),
        new(PositionType.Outcome, "РАСХОД"),
        new(PositionType.PensionInsuranceIE, "ВЗНОСЫ НА ОПС ИП"),
        new(PositionType.PensionInsurance, "ВЗНОСЫ НА ОПС"),
        new(PositionType.MedicalInsuranceIE, "ВЗНОСЫ НА ОМС ИП"),
        new(PositionType.MedicalInsurance, "ВЗНОСЫ НА ОМС"),
        new(PositionType.SocialInsurance, "ВЗНОСЫ НА ОСС"),
        new(PositionType.Casino, "ПК"),
        new(PositionType.FromBankAgent, "ВЫДАЧА ДС"),
        new(PositionType.ExciseNonMark, "АТНМ"),
        new(PositionType.ExciseMark, "АТМ"),
        new(PositionType.NonMark, "ТНМ"),
        new(PositionType.Mark, "ТМ"),
    ];

    public static readonly ReadOnlyCollection<EnumLocale<PayMethod>> PositionPayments =
    [
        new(PayMethod.FullPrepaid, "Полная предоплата"),
        new(PayMethod.PartPrepaid, "Частичная предоплата"),
        new(PayMethod.Advance, "Аванс"),
        new(PayMethod.FullPayment, "Полная оплата"),
        new(PayMethod.PartPayment, "Частичная оплата"),
        new(PayMethod.ToCredit, "Передача в кредит"),
        new(PayMethod.Postpaid, "Оплата кредита"),
    ];

    public static string GetLocale<TEnum>(this ReadOnlyCollection<EnumLocale<TEnum>> collection, TEnum? value,
        string defaultLocale = "???") where TEnum : struct, Enum => value is null
        ? string.Empty
        : collection.FirstOrDefault(i => i.Value.Equals(value))?.Locale ?? defaultLocale;
}