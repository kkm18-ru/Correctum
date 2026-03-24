using Correctum.Enums;
using Correctum.Extensions;
using Correctum.Models;

namespace Correctum.ApiClients.OfdRuTokenClient;

internal readonly struct OfdRuTokenReceipt : IOfdRuTokenModel
{
    public DateTime DocDateTime { get; init; }
    public string DecimalFiscalSign { get; init; }
    public string KktRegNumber { get; init; }
    public string FnNumber { get; init; }
    public uint DocNumber { get; init; }
    public uint DocShiftNumber { get; init; }
    public string ReceiptNumber { get; init; }
    public ulong TotalSumm { get; init; }
    public string OperationType { get; init; }
    public TaxationSystem TaxationType { get; init; }
    public IEnumerable<OfdRuTokenReceiptItem> Items { get; init; }
    public ulong CashSumm { get; init; }
    public ulong ECashSumm { get; init; }
    public ulong PrepaidSumm { get; init; }
    public ulong CreditSumm { get; init; }
    public ulong ProvisionSumm { get; init; }

    public readonly Receipt ToReceipt() => new()
    {
        RegNumber = KktRegNumber,
        DriveSerial = FnNumber,
        Number = DocNumber,
        FiscalSign = uint.Parse(DecimalFiscalSign),
        DateTime = DocDateTime,
        CashSum = CashSumm,
        NonCashSum = ECashSumm,
        CreditSum = CreditSumm,
        PrepaidSum = PrepaidSumm,
        OtherSum = ProvisionSumm,
        NumberInSession = uint.Parse(ReceiptNumber),
        Taxation = TaxationType,
        Type = OperationType.FromOfdRuToReceiptType(),
        SessionNumber = DocShiftNumber,
        TotalSum = TotalSumm,
        Buyer = null,
        Items = [..Items.Select(i => i.ToPosition())]
    };
}
