using Correctum.Enums;

namespace Correctum.Models;

/// <summary>
/// Описывает кассовый чек
/// </summary>
public struct Receipt
{
    /// <summary>
    /// Возвращает регистрационный номер контрольно-кассовой машины
    /// </summary>
    public string RegNumber { get; set; }
    /// <summary>
    /// Возвращает заводской номер фискального накопителя
    /// </summary>
    public string DriveSerial { get; set; }
    /// <summary>
    /// Возвращает номер фискального документа
    /// </summary>
    public uint Number { get; set; }
    /// <summary>
    /// Возвращает номер смены
    /// </summary>
    public uint SessionNumber { get; set; }
    /// <summary>
    /// Возвращает номер фискального документа в смене
    /// </summary>
    public uint NumberInSession { get; set; }
    /// <summary>
    /// Возвращает дату и время проведения документа
    /// </summary>
    public DateTime DateTime { get; set; }
    /// <summary>
    /// Возвращает фискальный признак документа
    /// </summary>
    public uint FiscalSign { get; set; }
    /// <summary>
    /// Возвращает тип кассового чека
    /// </summary>
    public ReceiptType Type { get; set; }
    /// <summary>
    /// Возвращает применяемую систему налогооблажения
    /// </summary>
    public TaxationSystem Taxation { get; set; }
    /// <summary>
    /// Возвращает адрес электронной почты или номер телефона покупателя
    /// </summary>
    public string? Buyer { get; set; }
    /// <summary>
    /// Возвращает массив позиций кассового чека
    /// </summary>
    public Position[] Items { get; set; }
    /// <summary>
    /// Возвращает сумму наличной оплаты
    /// </summary>
    /// <remarks>Рассчитывается в копейках</remarks>
    public ulong CashSum { get; set; }
    /// <summary>
    /// Возвращает сумму безналичной оплаты
    /// </summary>
    /// <remarks>Рассчитывается в копейках</remarks>
    public ulong NonCashSum { get; set; }
    /// <summary>
    /// Возвращает сумму оплаты аванса (предоплата)
    /// </summary>
    /// <remarks>Рассчитывается в копейках</remarks>
    public ulong PrepaidSum { get; set; }
    /// <summary>
    /// Возвращает сумму оплаты кредита (постоплаты)
    /// </summary>
    /// <remarks>Рассчитывается в копейках</remarks>
    public ulong CreditSum { get; set; }
    /// <summary>
    /// Возвращает сумму встречнного представления
    /// </summary>
    /// <remarks>Рассчитывается в копейках</remarks>
    public ulong OtherSum { get; set; }
    /// <summary>
    /// Возвращает итоговую сумму
    /// </summary>
    /// <remarks>Рассчитывается в копейках</remarks>
    public ulong TotalSum { get; set; }
}
