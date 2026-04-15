using System.Collections.ObjectModel;
using Correctum.Core.Enums;

namespace RetailCorrector.Utils;

public record EnumLocale<TEnum>(TEnum Value, string Locale) where TEnum : Enum;

public static class EnumLocales
{
    public static readonly ReadOnlyCollection<EnumLocale<TaxationSystems>> TaxationSystems =
    [
        new(Correctum.Core.Enums.TaxationSystems.General, "ОСН"),
        new(Correctum.Core.Enums.TaxationSystems.SimplifiedIncome, "УСН доход"),
        new(Correctum.Core.Enums.TaxationSystems.SimplifiedIncomeMinusExpense, "УСН доход-расход"),
        new(Correctum.Core.Enums.TaxationSystems.ImputedIncome, "ЕНВД"),
        new(Correctum.Core.Enums.TaxationSystems.Agricultural, "ЕСХН"),
        new(Correctum.Core.Enums.TaxationSystems.Patent, "Патент"),
    ];

    public static readonly ReadOnlyCollection<EnumLocale<ReceiptType>> ReceiptTypes =
    [
        new(ReceiptType.Income, "Приход"),
        new(ReceiptType.Outcome, "Расход"),
        new(ReceiptType.RefundIncome, "Возврат прихода"),
        new(ReceiptType.RefundOutcome, "Возврат расхода"),
    ];

    public static readonly ReadOnlyCollection<EnumLocale<UnitOfMeasure>> PositionUnits =
    [
        new(UnitOfMeasure.Piece, "шт."),
        new(UnitOfMeasure.Gram, "гр."),
        new(UnitOfMeasure.Kilogram, "кг"),
        new(UnitOfMeasure.Ton, "т"),
        new(UnitOfMeasure.Centimeter, "см"),
        new(UnitOfMeasure.Decimeter, "дм"),
        new(UnitOfMeasure.Meter, "м"),
        new(UnitOfMeasure.SquareCentimeter, "кв. см"),
        new(UnitOfMeasure.SquareDecimeter, "кв. дм"),
        new(UnitOfMeasure.SquareMeter, "кв. м"),
        new(UnitOfMeasure.Milliliter, "мл"),
        new(UnitOfMeasure.Liter, "л"),
        new(UnitOfMeasure.CubicMeter, "куб. м"),
        new(UnitOfMeasure.KilowattHour, "кВт·ч"),
        new(UnitOfMeasure.Gigacalories, "ГКал"),
        new(UnitOfMeasure.Days, "дн."),
        new(UnitOfMeasure.Hours, "ч"),
        new(UnitOfMeasure.Minutes, "мин."),
        new(UnitOfMeasure.Seconds, "сек."),
        new(UnitOfMeasure.Kilobyte, "КБ"),
        new(UnitOfMeasure.Megabyte, "МБ"),
        new(UnitOfMeasure.Gigabyte, "ГБ"),
        new(UnitOfMeasure.Terabyte, "ТБ"),
        new(UnitOfMeasure.Other, "др."),
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

    public static readonly ReadOnlyCollection<EnumLocale<CalculationMethod>> PositionPayments =
    [
        new(CalculationMethod.FullPrepaid, "Полная предоплата"),
        new(CalculationMethod.PartPrepaid, "Частичная предоплата"),
        new(CalculationMethod.Advance, "Аванс"),
        new(CalculationMethod.FullPayment, "Полная оплата"),
        new(CalculationMethod.PartPayment, "Частичная оплата"),
        new(CalculationMethod.ToCredit, "Передача в кредит"),
        new(CalculationMethod.Postpaid, "Оплата кредита"),
    ];

    public static string GetLocale<TEnum>(this ReadOnlyCollection<EnumLocale<TEnum>> collection, TEnum? value,
        string defaultLocale = "???") where TEnum : struct, Enum => value is null
        ? string.Empty
        : collection.FirstOrDefault(i => i.Value.Equals(value))?.Locale ?? defaultLocale;
}