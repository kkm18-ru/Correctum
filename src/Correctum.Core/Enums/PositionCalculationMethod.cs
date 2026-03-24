namespace Correctum.Enums
{
    /// <summary>
    /// Указывает метод расчета позиции
    /// </summary>
    public enum PositionCalculationMethod : byte
    {
        Unknown = 0,
        /// <summary>
        /// Полная предварительная оплата
        /// </summary>
        FullPrepaid = 1,
        /// <summary>
        /// Частичная предварительная оплата
        /// </summary>
        PartPrepaid = 2,
        /// <summary>
        /// Аванс
        /// </summary>
        Advance = 3,
        /// <summary>
        /// Полная оплата
        /// </summary>
        FullPayment = 4,
        /// <summary>
        /// Частичная оплата с последующей оплатой в кредит
        /// </summary>
        PartPayment = 5,
        /// <summary>
        /// Передача предмета расчета без его оплаты с последующей оплатой в кредит
        /// </summary>
        ToCredit = 6,
        /// <summary>
        /// Оплата предмета расчета после его передачи (оплата кредита)
        /// </summary>
        Postpaid = 7,
    }
}