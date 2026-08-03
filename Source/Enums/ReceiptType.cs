namespace Correctum.Core.Enums
{
    /// <summary>
    /// Определяет признак расчета
    /// </summary>
    public enum ReceiptType : byte
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

    public static class ReceiptTypeExtensions
    {
        extension(ReceiptType type)
        {
            public ReceiptType ToCancellation() => type switch
            {
                ReceiptType.Income => ReceiptType.RefundIncome,
                ReceiptType.RefundIncome => ReceiptType.Income,
                ReceiptType.Outcome => ReceiptType.RefundOutcome,
                ReceiptType.RefundOutcome => ReceiptType.Outcome,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}