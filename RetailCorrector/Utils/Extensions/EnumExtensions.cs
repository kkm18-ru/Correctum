using Correctum.Core.Enums;

namespace RetailCorrector.Utils.Extensions;

internal static class EnumExtensions
{
    extension(Operation origin)
    {
        public uint? ToCorrection(uint? ffd) => (ffd, origin) switch
        {
            (null, _) => null,
            ( <= 105, Operation.Income or Operation.RefundOutcome) => 1,
            ( <= 105, Operation.Outcome or Operation.RefundIncome) => 4,
            ( >= 110, Operation.Income) => 7,
            ( >= 110, Operation.Outcome) => 9,
            ( >= 110, Operation.RefundOutcome) => 10,
            ( >= 110, Operation.RefundIncome) => 8,
            _ => throw new NotImplementedException()
        };
    }

    extension(TaxRate rate)
    {
        public byte ToFns() => rate switch
        {
            TaxRate.Tax20 => 1,
            TaxRate.Tax10 => 2,
            TaxRate.Calc20 => 3,
            TaxRate.Calc10 => 4,
            TaxRate.Tax0 => 5,
            TaxRate.Without => 6,
            TaxRate.Tax5 => 7,
            TaxRate.Tax7 => 8,
            TaxRate.Calc5 => 9,
            TaxRate.Calc7 => 10,
            TaxRate.Tax22 => 11,
            TaxRate.Calc22 => 12,
            _ => 0
        };
    }
}
