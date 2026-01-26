namespace RetailCorrector.Enums.Condition;

/// <summary>Реквизит условия</summary>
public enum ConditionProp
{
    None = -1,
    
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
}