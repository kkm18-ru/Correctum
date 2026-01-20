using System.Globalization;
using RetailCorrector.Data;
using RetailCorrector.Enums;

namespace RetailCorrector.Utils;

public static class Constants
{
    public static readonly DateTime OfdBeginDate = new(2016, 6, 1, new GregorianCalendar());

    public const string DocsUrl = "https://retailcorrector.readthedocs.io/";
    public const string RoadmapUrl = "https://gitlab.com/ornaras/retailcorrector/-/milestones";
    public const string ChangelogUrl = "https://gitlab.com/ornaras/retailcorrector/-/releases";

    public static readonly EnumLocale<TaxRate>[] TaxRateLocales = [
        new(TaxRate.Tax0, "0%"),
        new(TaxRate.Without, "Не облагается"),
        new(TaxRate.Tax5, "5%"),
        new(TaxRate.Calc5, "5/105"),
        new(TaxRate.Tax7, "7%"),
        new(TaxRate.Calc7, "7/107"),
        new(TaxRate.Tax10, "10%"),
        new(TaxRate.Calc10, "10/110"),
        new(TaxRate.Tax18, "18%"),
        new(TaxRate.Calc18, "18/118"),
        new(TaxRate.Tax20, "20%"),
        new(TaxRate.Calc20, "20/120"),
        new(TaxRate.Tax22, "22%"),
        new(TaxRate.Calc22, "22/122"),
    ];

    public static readonly EnumLocale<PositionType>[] PositionTypeLocales = [
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
        new(PositionType.TraceFee, "ТОРГОВЫЙ СБОР"),
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
        new(PositionType.ExciseNonMark, "АТНИ"),
        new(PositionType.ExciseMark, "АТМ"),
        new(PositionType.NonMark, "ТНМ"),
        new(PositionType.Mark, "ТМ"),
    ];

    public static readonly EnumLocale<TaxationSystem>[] TaxationSystemLocales = [
        new(TaxationSystem.OSN, "ОСН"),
        new(TaxationSystem.USNIncome, "УСН доход"),
        new(TaxationSystem.USNIncomeExpenditure, "УСН доход - расход"),
        new(TaxationSystem.ENVD, "ЕНВД"),
        new(TaxationSystem.ESN, "ЕСН"),
        new(TaxationSystem.Patent, "Патент"),
    ];

    public static readonly EnumLocale<Operation>[] ReceiptTypeLocales = [
        new(Operation.Income, "Приход"),
        new(Operation.RefundIncome, "Возврат прихода"),
        new(Operation.Outcome, "Расход"),
        new(Operation.RefundOutcome, "Возврат расхода"),
    ];

    public static readonly EnumLocale<MeasureUnit>[] PositionUnitLocales = [
        new(MeasureUnit.Default, "шт."),
        new(MeasureUnit.g, "гр."),
        new(MeasureUnit.kg, "кг"),
        new(MeasureUnit.t, "т"),
        new(MeasureUnit.cm, "см"),
        new(MeasureUnit.dm, "дм"),
        new(MeasureUnit.m, "м"),
        new(MeasureUnit.cm2, "см2"),
        new(MeasureUnit.dm2, "дм2"),
        new(MeasureUnit.m2, "м2"),
        new(MeasureUnit.mL, "мл"),
        new(MeasureUnit.L, "л"),
        new(MeasureUnit.m3, "м3"),
        new(MeasureUnit.kW_h, "кВт/ч"),
        new(MeasureUnit.GCal, "Гкал"),
        new(MeasureUnit.Day, "дн."),
        new(MeasureUnit.h, "ч"),
        new(MeasureUnit.min, "мин."),
        new(MeasureUnit.sec, "сек."),
        new(MeasureUnit.kByte, "КБ"),
        new(MeasureUnit.MByte, "МБ"),
        new(MeasureUnit.GByte, "ГБ"),
        new(MeasureUnit.TByte, "КБ"),
        new(MeasureUnit.Other, "..."),
    ];

    public static readonly EnumLocale<PayMethod>[] PaymentMethodLocales = [
        new(PayMethod.FullPrepaid, "ПРЕДОПЛАТА 100%"),
        new(PayMethod.PartPrepaid, "ПРЕДОПЛАТА"),
        new(PayMethod.Advance, "АВАНС"),
        new(PayMethod.FullPayment, "ПОЛНЫЙ РАСЧЕТ"),
        new(PayMethod.PartPayment, "ЧАСТИЧНЫЙ РАСЧЕТ И КРЕДИТ"),
        new(PayMethod.ToCredit, "ПЕРЕДАЧА В КРЕДИТ"),
        new(PayMethod.Postpaid, "ОПЛАТА КРЕДИТА"),
    ];
}