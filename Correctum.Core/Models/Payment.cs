using Correctum.Core.ValueObjects;

namespace Correctum.Core.Models
{
    /// <summary>
    /// Представляет информацию об оплате кассового чека
    /// </summary>
    public readonly struct Payment
    {
        /// <summary>
        /// Сумма оплаты наличным способом
        /// </summary>
        public Currency CashSum { get; init; }
        /// <summary>
        /// Сумма оплаты безналичным способом
        /// </summary>
        public Currency NonCashSum { get; init; }
        /// <summary>
        /// Сумма аванса
        /// </summary>
        public Currency AdvanceSum { get; init; }
        /// <summary>
        /// Сумма оплаты кредита
        /// </summary>
        public Currency CreditSum { get; init; }
        /// <summary>
        /// Встречное предоставление
        /// </summary>
        public Currency ProvisionSum { get; init; }
    }
}
