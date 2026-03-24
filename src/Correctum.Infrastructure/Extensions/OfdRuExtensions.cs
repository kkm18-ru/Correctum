using Correctum.Enums;

namespace Correctum.Extensions;

public static class OfdRuExtensions
{
    extension(string receiptType)
    {
        public ReceiptType FromOfdRuToReceiptType()
        {
            receiptType = receiptType.ToLowerInvariant();
            var opertionId = 0;
            if (receiptType.EndsWith("outcome")) opertionId = 2;
            opertionId += receiptType.StartsWith("refund") ? 2 : 1;
            return (ReceiptType)opertionId;
        }
    }
}
