namespace Correctum.Enums
{
    /// <summary>
    /// Указывает систему налогооблажения
    /// </summary>
    public enum TaxationSystem
    {
        /// <summary>
        /// Основная система налогооблажения
        /// </summary>
        General = 1 << 0,
        /// <summary>
        /// Упрощенная система налогооблажения (доход)
        /// </summary>
        Simplified = 1 << 1,
        /// <summary>
        /// Упрощенная система налогооблажения (доход минус расходы)
        /// </summary>
        SimplifiedMinusExpenditure = 1 << 2,
        /// <summary>
        /// Налог на вмененный доход
        /// </summary>
        /// <remarks>Утратил силу с 1 сентября 2025 г.</remarks>
        ImputedIncome = 1 << 3, 
        /// <summary>
        /// Единая сельскохозяйственная система налогооблажения
        /// </summary>
        Agricultural = 1 << 4,
        /// <summary>
        /// Патентная система налогооблажения
        /// </summary>
        Patent = 1 << 5,
    }
}
