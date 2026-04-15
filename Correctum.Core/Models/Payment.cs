using Correctum.Core.ValueObjects;

namespace Correctum.Core.Models
{
    /// <summary>
    /// Представляет информацию об оплате кассового чека
    /// </summary>
    public struct Payment
    {
        /// <summary>
        /// Сумма оплаты наличным способом
        /// </summary>
        public Currency CashSum { get; set; }
        /// <summary>
        /// Сумма оплаты безналичным способом
        /// </summary>
        public Currency NonCashSum { get; set; }
        /// <summary>
        /// Сумма аванса
        /// </summary>
        public Currency AdvanceSum { get; set; }
        /// <summary>
        /// Сумма оплаты кредита
        /// </summary>
        public Currency CreditSum { get; set; }
        /// <summary>
        /// Встречное предоставление
        /// </summary>
        public Currency ProvisionSum { get; set; }
    }
}
