#pragma warning disable S1133
namespace Correctum.Enums
{
    /// <summary>
    /// Указывает ставку НДС
    /// </summary>
    public enum TaxRate : byte
    {
        /// <summary>
        /// Не определено
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Ставка 18%
        /// </summary>
        /// <remarks>С 1 января 2019 г. заменен на НДС 20%</remarks>
        Tax18 = 1,
        /// <summary>
        /// Ставка 10%
        /// </summary>
        Tax10 = 2,
        /// <summary>
        /// Расчитываемая ставка 18/118
        /// </summary>
        /// <remarks>С 1 января 2019 г. заменен на НДС 20/120</remarks>
        Calc18 = 3,
        /// <summary>
        /// Расчитываемая ставка 10/110
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
        /// Ставка 20%
        /// </summary>
        /// <remarks>С 1 января 2026 г. заменен на НДС 22%</remarks>
        Tax20 = 7,
        /// <summary>
        /// Расчитываемая ставка 20/120
        /// </summary>
        /// <remarks>С 1 января 2026 г. заменен на НДС 22/122</remarks>
        Calc20 = 8,
        /// <summary>
        /// Ставка 5%
        /// </summary>
        Tax5 = 9,
        /// <summary>
        /// Ставка 7%
        /// </summary>
        Tax7 = 10,
        /// <summary>
        /// Расчитываемая ставка 5/105
        /// </summary>
        Calc5 = 11,
        /// <summary>
        /// Расчитываемая ставка 7/107
        /// </summary>
        Calc7 = 12,
        /// <summary>
        /// Ставка 22%
        /// </summary>
        Tax22 = 13,
        /// <summary>
        /// Расчитываемая ставка 22/122
        /// </summary>
        Calc22 = 14
    }
}