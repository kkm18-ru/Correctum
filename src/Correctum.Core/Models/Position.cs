using Correctum.Enums;

namespace Correctum.Models;

/// <summary>
/// Описывает позицию кассового чека
/// </summary>
public struct Position
{
    /// <summary>
    /// Возвращает наименование позиции
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Возвращает цену позиции (с учетом скидок и надбавок)
    /// </summary>
    /// <remarks>Рассчитывается в копейках</remarks>
    public ulong Price { get; set; }
    /// <summary>
    /// Возвращает количество позиции
    /// </summary>
    /// <remarks>Рассчитывается в тысячных единиц (1/1000)</remarks>
    public ulong Quantity { get; set; }
    /// <summary>
    /// Возвращает стоимость позиции (с учетом скидок и надбавок)
    /// </summary>
    /// <remarks>Рассчитывается в копейках</remarks>
    public ulong Cost { get; set; }
    /// <summary>
    /// Возвращает единицу измерения позиции
    /// </summary>
    public PositionUnit Unit { get; set; }
    /// <summary>
    /// Возвращает тип позиции
    /// </summary>
    public PositionType Type { get; set; }
    /// <summary>
    /// Возвращает ставку НДС позиции
    /// </summary>
    public TaxRate TaxRate { get; set; }
    /// <summary>
    /// Возвращает способ оплаты позиции
    /// </summary>
    public PositionPayment PaymentMethod { get; set; }
}
