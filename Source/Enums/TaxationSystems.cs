namespace Correctum.Enums
{
    /// <summary>
    /// Определяет применяемую систему налогообложения
    /// </summary>    
    [Flags] public enum TaxationSystems
    {
        /// <summary>
        /// Общая система налогообложения
        /// </summary>
        General = 1 << 0,
        /// <summary>
        /// Упрощенная система налогообложения (доход)
        /// </summary>
        SimplifiedIncome = 1 << 1,
        /// <summary>
        /// Упрощенная система налогообложения (доход минус расход)
        /// </summary>
        SimplifiedIncomeMinusExpense = 1 << 2,
        /// <summary>
        /// Налог на вмененный доход
        /// </summary>
        /// <remarks>
        /// Устарел: Утратил силу с 1 сентября 2025
        /// </remarks>
        ImputedIncome = 1 << 3,
        /// <summary>
        /// Единый сельскохозяйственный налог
        /// </summary>
        Agricultural = 1 << 4,
        /// <summary>
        /// Патентная система налогообложения
        /// </summary>
        Patent = 1 << 5,
    }
}
