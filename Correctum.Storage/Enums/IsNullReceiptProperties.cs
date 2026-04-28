
namespace Correctum.Storage.Enums
{
    [Flags] internal enum IsNullReceiptProperties : uint
    {
        RegId = 1 << 0,
        StorageId = 1 << 1,
        DocNumber = 1 << 2,
        SessionId = 1 << 3,
        DocInSession = 1 << 4,
        CreatedAt = 1 << 5,
        FiscalSign = 1 << 6,
        Type = 1 << 7,
        CashSum = 1 << 8,
        NonCashSum = 1 << 9,
        AdvanceSum = 1 << 10,
        CreditSum = 1 << 11,
        ProvisionSum = 1 << 12,
        TotalSum = 1 << 13,
        BuyerContact = 1 << 14,
        TaxationSystem = 1 << 15,
        Positions = 1 << 16,
    }
}
