using Correctum.Core.ValueObjects;

namespace Correctum.Core.Models
{
    public struct Payment
    {
        public Currency Cash { get; set; }
        public Currency ECash { get; set; }
        public Currency Pre { get; set; }
        public Currency Post { get; set; }
        public Currency Provision { get; set; }
    }
}
