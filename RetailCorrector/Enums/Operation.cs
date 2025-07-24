namespace RetailCorrector.Enums
{
    /// <summary>
    /// Операция чека
    /// </summary>
    public enum Operation : byte
    {
        /// <summary>
        /// Приход
        /// </summary>
        Income = 1,
        /// <summary>
        /// Возврат прихода
        /// </summary>
        RefundIncome = 2,
        /// <summary>
        /// Расход
        /// </summary>
        Outcome = 3,
        /// <summary>
        /// Возврат расхода
        /// </summary>
        RefundOutcome = 4,
    }
}