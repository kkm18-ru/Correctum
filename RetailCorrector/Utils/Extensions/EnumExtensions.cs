using RetailCorrector.Enums;

namespace RetailCorrector.Utils.Extensions;

internal static class EnumExtensions
{
    extension(TaxRate taxRate)
    {
        public uint ToFptr() => taxRate switch
        {
            TaxRate.Tax10 => 2,
            TaxRate.Calc10 => 4,
            TaxRate.Tax0 => 5,
            TaxRate.Without => 6,
            TaxRate.Tax20 => 7,
            TaxRate.Calc20 => 8,
            TaxRate.Tax5 => 9,
            TaxRate.Tax7 => 10,
            TaxRate.Calc5 => 11,
            TaxRate.Calc7 => 12,
            _ => throw new NotImplementedException()
        };
    }

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
}
