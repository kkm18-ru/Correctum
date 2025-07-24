namespace RetailCorrector.Enums
{
    /// <summary>
    /// Ставки НДС
    /// </summary>
    public enum TaxRate : byte
    {
        /// <summary>
        /// Ставка 20%
        /// </summary>
        Tax20 = 1,
        /// <summary>
        /// Ставка 10%
        /// </summary>
        Tax10 = 2,
        /// <summary>
        /// Расчитываемая ставка 20/120%
        /// </summary>
        Calc20 = 3,
        /// <summary>
        /// Расчитываемая ставка 10/110%
        /// </summary>
        Calc10 = 4,
        /// <summary>
        /// Ставка 0%
        /// </summary>
        Tax0 = 5,
        /// <summary>
        /// Не облагается НДС
        /// </summary>
        Without = 6,
        /// <summary>
        /// Ставка 5%
        /// </summary>
        Tax5 = 7,
        /// <summary>
        /// Ставка 7%
        /// </summary>
        Tax7 = 8,
        /// <summary>
        /// Расчитываемая ставка 5/105%
        /// </summary>
        Calc5 = 9,
        /// <summary>
        /// Расчитываемая ставка 7/107%
        /// </summary>
        Calc7 = 10,
    }
}