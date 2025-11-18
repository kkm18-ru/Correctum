using RetailCorrector.Enums;

namespace RetailCorrector.Utils
{
    internal static class FptrConverter
    {
        public static uint ToAtol(this TaxRate origin) => origin switch
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
}
