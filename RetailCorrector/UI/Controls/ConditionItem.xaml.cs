using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Controls;

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

    public ObservableCollection<string> LocaleSubjects { get; } = [];
    public ObservableCollection<string> LocaleProps { get; } = [];
    public ObservableCollection<string> LocaleOperators { get; } = [];
    
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
        switch (SubjectType)
        {
            case ConditionSubjectType.Receipt:
                LocaleSubjects.Add("В чеке");
                LocaleSubjects.Add("В любой позиции");
                LocaleSubjects.Add("Во всех позициях");
                break;
            case ConditionSubjectType.Position:
                LocaleSubjects.Add("В позиции");
                break;
        }
    }

    private void UpdateLocaleProps()
    {
        LocaleProps.Clear();
        if (SubjectType == ConditionSubjectType.Position || Subject > 0)
        {
            LocaleProps.Add("наименование");
            LocaleProps.Add("метод оплаты");
            LocaleProps.Add("тип");
            LocaleProps.Add("ставка НДС");
            LocaleProps.Add("цена");
            LocaleProps.Add("количество");
            LocaleProps.Add("ЕИ");
            LocaleProps.Add("стоимость");
        }
        else
        {
            LocaleProps.Add("ККТ");
            LocaleProps.Add("ФН");
            LocaleProps.Add("смена");
            LocaleProps.Add("дата/время");
            LocaleProps.Add("покупатель");
            LocaleProps.Add("СНО");
            LocaleProps.Add("операция");
            LocaleProps.Add("сумма нал. оплаты");
            LocaleProps.Add("сумма б/н оплаты");
            LocaleProps.Add("сумма преоплаты");
            LocaleProps.Add("сумма постоплаты");
            LocaleProps.Add("сумма иной оплаты");
            LocaleProps.Add("итоговая сумма");
            LocaleProps.Add("только чтение");
        }
    }

    private void UpdateLocaleOperators()
    {
        LocaleOperators.Clear();
        
        var isReceipt = SubjectType == ConditionSubjectType.Receipt && Subject == ConditionSubject.Default;
        
        var isString = Prop == 0 || isReceipt && Prop is ConditionProp.DriveId or ConditionProp.Buyer;
        var isFlag = isReceipt && Prop == ConditionProp.Taxation;
        var isBool = isReceipt && Prop == ConditionProp.IsReadOnly;

        if (isString)
        {
            LocaleOperators.Add("начинается с");
            LocaleOperators.Add("начинается с (без учета регистра)");
            LocaleOperators.Add("содержит");
            LocaleOperators.Add("содержит (без учета регистра)");
            LocaleOperators.Add("заканчивается на");
            LocaleOperators.Add("заканчивается на (без учета регистра)");
        }
        else if (isFlag)
        {
            LocaleOperators.Add("равно");
            LocaleOperators.Add("не равно");
            LocaleOperators.Add("содержит");
        }
        else if (isBool)
        {
            LocaleOperators.Add("является True");
            LocaleOperators.Add("является False");
        }
        else
        {
            LocaleOperators.Add("равно");
            LocaleOperators.Add("не равно");
            LocaleOperators.Add("меньше");
            LocaleOperators.Add("меньше или равно");
            LocaleOperators.Add("больше");
            LocaleOperators.Add("больше или равно");
        }
    }

    internal Expression<Predicate<object>> ToExpression()
    {
        throw new NotImplementedException();
    }
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
    Name = 0,           // Text
    /// <summary>Метод оплаты позиции</summary>
    PaymentMethod = 1,  // Enum
    /// <summary>Тип позиции</summary>
    PositionType = 2,   // Enum
    /// <summary>Ставка НДС</summary>
    TaxRate = 3,        // Enum
    /// <summary>Цена</summary>
    Price = 4,          // Double
    /// <summary>Количество</summary>
    Quantity = 5,       // Double
    /// <summary>Единица измерения</summary>
    Unit = 6,           // Enum
    /// <summary>Стоимость</summary>
    Cost = 7,           // Double

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
    
    Equal = 0,
    NotEqual = 1,
    LessThan = 2,
    EqualOrLessThan = 3,
    MoreThan = 4,
    EqualOrMoreThan = 5,

    #endregion

    #region Операторы логических реквизитов

    IsTrue = 0,
    IsFalse = 1

    #endregion
    
    #pragma warning restore CA1069
}
