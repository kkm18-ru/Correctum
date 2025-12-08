using RetailCorrector.Enums;

namespace RetailCorrector.Data;

internal struct TaxSum
{
    public TaxRate Rate { get; set; }
    public ulong Sum { get; set; }

    public TaxSum(TaxRate rate, ulong sum)
    {
        Rate = rate;
        Sum = sum;
    }
}
