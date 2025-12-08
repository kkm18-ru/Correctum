using RetailCorrector.Enums;

namespace RetailCorrector.Data;

internal struct PaymentSum
{
    public PaymentType Type { get; set; }
    public ulong Sum { get; set; }

    public PaymentSum(PaymentType type, ulong sum)
    {
        Type = type;
        Sum = sum;
    }
}
