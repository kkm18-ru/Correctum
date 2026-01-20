using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Controls;
using RetailCorrector.Data;

namespace RetailCorrector.UI.Controls;

public partial class ConditionItem : ListBoxItem, INotifyPropertyChanged
{
    public required ConditionSubjectType SubjectType
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, SubjectTypeChangedArgs);
            Subject = ConditionSubject.None;
            UpdateLocaleSubjects();
        }
    }

    public ConditionSubject Subject
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, SubjectChangedArgs);
            Prop = ConditionProp.None;
            UpdateLocaleProps();
        }
    } = ConditionSubject.None;
    
    public ConditionProp Prop
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, PropChangedArgs);
            Operator = ConditionOperator.None;
            UpdateLocaleOperators();
        }
    } = ConditionProp.None;
    
    public ConditionOperator Operator
    {
        get;
        set
        {
            if(field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, OperatorChangedArgs);
        }
    } = ConditionOperator.None;
    
    private ConditionValueType ValueType => Prop switch
    {
        ConditionProp.DeviceId or ConditionProp.DriveId or ConditionProp.Buyer or ConditionProp.Name =>
            ConditionValueType.Text,
        ConditionProp.Session => ConditionValueType.Integer,
        ConditionProp.Created => ConditionValueType.DateTime,
        ConditionProp.Taxation => ConditionValueType.Flag,
        ConditionProp.ReceiptType or ConditionProp.PaymentMethod or ConditionProp.Unit
            or ConditionProp.PositionType => ConditionValueType.Enum,
        ConditionProp.IsReadOnly => ConditionValueType.Boolean,
        ConditionProp.CashSum or ConditionProp.EcashSum or ConditionProp.PreSum or ConditionProp.PostSum
            or ConditionProp.OtherSum or ConditionProp.TotalSum or ConditionProp.Price or ConditionProp.Quantity
            or ConditionProp.Cost => ConditionValueType.Float,
        _ => ConditionValueType.None
    };

    public ObservableCollection<EnumLocale<ConditionSubject>> LocaleSubjects { get; } = [];
    public ObservableCollection<EnumLocale<ConditionProp>> LocaleProps { get; } = [];
    public ObservableCollection<EnumLocale<ConditionOperator>> LocaleOperators { get; } = [];
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private static readonly PropertyChangedEventArgs SubjectTypeChangedArgs = new(nameof(SubjectType));
    private static readonly PropertyChangedEventArgs SubjectChangedArgs = new(nameof(Subject));
    private static readonly PropertyChangedEventArgs PropChangedArgs = new(nameof(Prop));
    private static readonly PropertyChangedEventArgs OperatorChangedArgs = new(nameof(Operator));
    
    public ConditionItem()
    {
        InitializeComponent();
        UpdateLocaleSubjects();
    }

    private void UpdateLocaleSubjects()
    {
        LocaleSubjects.Clear();
        var locales = SubjectType switch
        {
            ConditionSubjectType.Receipt => Constants.ReceiptSubjects,
            ConditionSubjectType.Position => Constants.PositionSubjects,
            _ => []
        };
        foreach (var locale in locales)
            LocaleSubjects.Add(locale);
    }

    private void UpdateLocaleProps()
    {
        LocaleProps.Clear();
        var locales = SubjectType == ConditionSubjectType.Position || Subject > 0
            ? Constants.PositionProperties
            : Constants.ReceiptProperties;
        foreach (var locale in locales)
            LocaleProps.Add(locale);
    }

    private void UpdateLocaleOperators()
    {
        LocaleOperators.Clear();
        var locales = ValueType switch
        {
            ConditionValueType.Flag => Constants.FlagsOperators,
            ConditionValueType.Enum => Constants.EnumsOperators,
            ConditionValueType.Text => Constants.TextOperators,
            ConditionValueType.Boolean => Constants.BooleanOperators,
            ConditionValueType.Float or ConditionValueType.DateTime or ConditionValueType.Integer => Constants.ComparableOperators,
            _ => []
        };
        foreach (var locale in locales)
            LocaleOperators.Add(locale);
    }

    internal Expression<Predicate<object>> ToExpression()
    {
        throw new NotImplementedException();
    }
}

file static class Constants
{
    public static readonly EnumLocale<ConditionSubject>[] ReceiptSubjects = [
        new(ConditionSubject.Default, "Чек"),
        new(ConditionSubject.AnyPosition, "В любой позиции"),
        new(ConditionSubject.AllPositions, "Во всех позициях")
    ];
    public static readonly EnumLocale<ConditionSubject>[] PositionSubjects = [
        new(ConditionSubject.Default, "Позиция")
    ];

    public static readonly EnumLocale<ConditionProp>[] ReceiptProperties = [
        new(ConditionProp.DeviceId, "ККТ"),
        new(ConditionProp.DriveId, "ФН"),
        new(ConditionProp.Session, "смена"),
        new(ConditionProp.Created, "дата/время"),
        new(ConditionProp.Buyer, "покупатель"),
        new(ConditionProp.Taxation, "СНО"),
        new(ConditionProp.ReceiptType, "операция"),
        new(ConditionProp.CashSum, "сумма нал. оплаты"),
        new(ConditionProp.EcashSum, "сумма б/н оплаты"),
        new(ConditionProp.PreSum, "сумма преоплаты"),
        new(ConditionProp.PostSum, "сумма постоплаты"),
        new(ConditionProp.OtherSum, "сумма иной оплаты"),
        new(ConditionProp.TotalSum, "итоговая сумма"),
        new(ConditionProp.IsReadOnly, "только чтение"),
    ];
    public static readonly EnumLocale<ConditionProp>[] PositionProperties = [
        new(ConditionProp.Name, "наименование"),
        new(ConditionProp.PaymentMethod, "метод оплаты"),
        new(ConditionProp.PositionType, "тип"),
        new(ConditionProp.TaxRate, "ставка НДС"),
        new(ConditionProp.Price, "цена"),
        new(ConditionProp.Quantity, "количество"),
        new(ConditionProp.Unit, "единица измерения"),
        new(ConditionProp.Cost, "стоимость"),
    ];

    public static readonly EnumLocale<ConditionOperator>[] TextOperators = [
        new(ConditionOperator.StartsWith, "начинается с"),
        new(ConditionOperator.StartsWithIgnoreCase, "начинается с (без учета регистра)"),
        new(ConditionOperator.Contains, "содержит"),
        new(ConditionOperator.ContainsIgnoreCase, "содержит (без учета регистра)"),
        new(ConditionOperator.EndsWith, "заканчивается на"),
        new(ConditionOperator.EndsWithIgnoreCase, "заканчивается на (без учета регистра)"),
    ];
    public static readonly EnumLocale<ConditionOperator>[] ComparableOperators = [
        new(ConditionOperator.Equal, "=="),
        new(ConditionOperator.NotEqual, "!="),
        new(ConditionOperator.LessThan, "<"),
        new(ConditionOperator.EqualOrLessThan, "<="),
        new(ConditionOperator.MoreThan, ">"),
        new(ConditionOperator.EqualOrMoreThan, ">="),
    ];
    public static readonly EnumLocale<ConditionOperator>[] EnumsOperators = [
        new(ConditionOperator.Equal, "=="),
        new(ConditionOperator.NotEqual, "!="),
    ];
    public static readonly EnumLocale<ConditionOperator>[] FlagsOperators = [
        new(ConditionOperator.Contains, "содержит"),
        new(ConditionOperator.Equal, "=="),
        new(ConditionOperator.NotEqual, "!="),
    ];
    public static readonly EnumLocale<ConditionOperator>[] BooleanOperators = [
        new(ConditionOperator.IsTrue, "является True"),
        new(ConditionOperator.IsFalse, "является False"),
    ];
}

/// <summary>Тип субъекта условия</summary>
public enum ConditionSubjectType
{
    /// <summary>Чек</summary>
    Receipt,
    /// <summary>Позиция</summary>
    Position
}

/// <summary>Субъект условия</summary>
public enum ConditionSubject
{
    None = -1,
    /// <summary>Реквизит чека/позиции</summary>
    Default,
    /// <summary>Реквизит хотя бы одного из позиций</summary>
    AnyPosition,
    /// <summary>Реквизит всех позиций</summary>
    AllPositions
}

/// <summary>Реквизит условия</summary>
public enum ConditionProp
{
    #pragma warning disable CA1069
    None = -1,

    #region Реквизиты чека

    /// <summary>Регистрационный номер ККТ</summary>
    DeviceId = 0,       // Text
    /// <summary>Заводской номер ФН</summary>
    DriveId = 1,        // Text
    /// <summary>Номер смены</summary>
    Session = 2,        // Integer
    /// <summary>Дата/время чека</summary>
    Created = 3,        // DateTime
    /// <summary>Контакт покупателя</summary>
    Buyer = 4,          // Text
    /// <summary>Система налогооблажения</summary>
    Taxation = 5,       // Flags
    /// <summary>Операция чека</summary>
    ReceiptType = 6,    // Enum
    /// <summary>Сумма наличной оплаты</summary>
    CashSum = 7,        // Double
    /// <summary>Сумма безналичной оплаты</summary>
    EcashSum = 8,       // Double
    /// <summary>Сумма аванса</summary>
    PreSum = 9,         // Double
    /// <summary>Сумма кредита</summary>
    PostSum = 10,       // Double
    /// <summary>Сумма иной оплаты</summary>
    OtherSum = 11,      // Double
    /// <summary>Итоговая сумма</summary>
    TotalSum = 12,      // Double
    /// <summary>Чек в режиме "только чтение"?</summary>
    IsReadOnly = 13,    // Bool

    #endregion
    
    #region Реквизиты позиции

    /// <summary>Наименование позиции</summary>
    Name = 14,           // Text
    /// <summary>Метод оплаты позиции</summary>
    PaymentMethod = 15,  // Enum
    /// <summary>Тип позиции</summary>
    PositionType = 16,   // Enum
    /// <summary>Ставка НДС</summary>
    TaxRate = 17,        // Enum
    /// <summary>Цена</summary>
    Price = 18,          // Double
    /// <summary>Количество</summary>
    Quantity = 19,       // Double
    /// <summary>Единица измерения</summary>
    Unit = 20,           // Enum
    /// <summary>Стоимость</summary>
    Cost = 21,           // Double

    #endregion
    #pragma warning restore CA1069
}

public enum ConditionOperator
{
    /* - Строка             [04]    Операторы текстовых реквизитов
     * - Целое число        [01]    Операторы сравнения
     * - Дробное чимло      [09]    Операторы сравнения
     * - Дата/время         [01]    Операторы сравнения
     * - Булевое значение   [01]    Операторы логических реквизитов
     * - Перечисление       [05]    Операторы сравнения
     * - Флаги              [01]    Equal, NotEqual, Contains
     */
    #pragma warning disable CA1069
    None = -1,

    #region Операторы текстовых реквизитов

    StartsWith = 0,
    StartsWithIgnoreCase = 1,
    Contains = 2,
    ContainsIgnoreCase = 3,
    EndsWith = 4,
    EndsWithIgnoreCase = 5,

    #endregion

    #region Операторы сравнения
    
    Equal = 6,
    NotEqual = 7,
    LessThan = 8,
    EqualOrLessThan = 9,
    MoreThan = 10,
    EqualOrMoreThan = 11,

    #endregion

    #region Операторы логических реквизитов

    IsTrue = 12,
    IsFalse = 13

    #endregion
    
    #pragma warning restore CA1069
}

public enum ConditionValueType { None, Text, Integer, Float, DateTime, Boolean, Enum, Flag }
