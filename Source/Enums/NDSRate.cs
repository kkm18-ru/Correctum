#pragma warning disable S2342
namespace Correctum.Core.Enums
{
    /// <summary>
    /// Указывает ставку НДС
    /// </summary>
    public enum NDSRate : byte
    {
        /// <summary>
        /// Ставка 18%
        /// </summary>
        /// <remarks>
        /// Устарел: С 1 января 2019 г. заменен на НДС 20%
        /// </remarks>
        NDS18 = 1,
        /// <summary>
        /// Ставка 10%
        /// </summary>
        NDS10 = 2,
        /// <summary>
        /// Расчитываемая ставка 18/118
        /// </summary>
        /// <remarks>
        /// Устарел: С 1 января 2019 г. заменен на НДС 20/120
        /// </remarks>
        NDS118 = 3,
        /// <summary>
        /// Расчитываемая ставка 10/110
        /// </summary>
        NDS110 = 4,
        /// <summary>
        /// Ставка 0%
        /// </summary>
        NDS0 = 5,
        /// <summary>
        /// Не облагается НДС
        /// </summary>
        NoNDS = 6,
        /// <summary>
        /// Ставка 20%
        /// </summary>
        /// <remarks>
        /// Устарел: С 1 января 2026 г. заменен на НДС 22%
        /// </remarks>
        NDS20 = 7,
        /// <summary>
        /// Расчитываемая ставка 20/120
        /// </summary>
        /// <remarks>
        /// Устарел: С 1 января 2026 г. заменен на НДС 22/122
        /// </remarks>
        NDS120 = 8,
        /// <summary>
        /// Ставка 5%
        /// </summary>
        NDS5 = 9,
        /// <summary>
        /// Ставка 7%
        /// </summary>
        NDS7 = 10,
        /// <summary>
        /// Расчитываемая ставка 5/105
        /// </summary>
        NDS105 = 11,
        /// <summary>
        /// Расчитываемая ставка 7/107
        /// </summary>
        NDS107 = 12,
        /// <summary>
        /// Ставка 22%
        /// </summary>
        NDS22 = 13,
        /// <summary>
        /// Расчитываемая ставка 22/122
        /// </summary>
        NDS122 = 14
    }
}