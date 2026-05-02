using Correctum.Core.Enums;

namespace Correctum.Utils.Extensions;

internal static class EnumExtensions
{
    extension(ReceiptType origin)
    {
        public uint? ToCorrection(uint? ffd) => (ffd, origin) switch
        {
            (null, _) => null,
            ( <= 105, ReceiptType.Income or ReceiptType.RefundOutcome) => 1,
            ( <= 105, ReceiptType.Outcome or ReceiptType.RefundIncome) => 4,
            ( >= 110, ReceiptType.Income) => 7,
            ( >= 110, ReceiptType.Outcome) => 9,
            ( >= 110, ReceiptType.RefundOutcome) => 10,
            ( >= 110, ReceiptType.RefundIncome) => 8,
            _ => throw new NotImplementedException()
        };
    }

    extension(NDSRate rate)
    {
        public byte ToFns() => rate switch
        {
            NDSRate.NDS20 => 1,
            NDSRate.NDS10 => 2,
            NDSRate.NDS120 => 3,
            NDSRate.NDS110 => 4,
            NDSRate.NDS0 => 5,
            NDSRate.NoNDS => 6,
            NDSRate.NDS5 => 7,
            NDSRate.NDS7 => 8,
            NDSRate.NDS105 => 9,
            NDSRate.NDS107 => 10,
            NDSRate.NDS22 => 11,
            NDSRate.NDS122 => 12,
            _ => 0
        };
    }
}
