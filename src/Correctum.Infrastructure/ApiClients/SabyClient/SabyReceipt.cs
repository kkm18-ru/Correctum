using Correctum.Enums;
using Correctum.Extensions;
using Correctum.Models;
using System.Text.Json.Serialization;

namespace Correctum.ApiClients.SabyClient;

[method: JsonConstructor]
internal readonly struct SabyReceipt(
    string kktRegId,
    string fiscalDriveNumber,
    uint shiftNumber,
    uint requestNumber,
    long dateTime,
    string buyerPhoneOrAddress,
    ulong totalSum,
    ulong cashTotalSum,
    uint operationType,
    uint fiscalSign,
    ulong ecashTotalSum,
    ulong prepaidSum,
    ulong creditSum,
    uint taxationType,
    ulong provisionSum,
    uint fiscalDocumentNumber,
    SabyReceiptItem[] items
    )
{
    public string RegNumber { get; } = kktRegId;
    public string DriveSerial { get; } = fiscalDriveNumber;
    public uint SessionId { get; } = shiftNumber;
    public uint NumberInSession { get; } = requestNumber;
    public long UnixTimestamp { get; } = dateTime;
    public string Buyer { get; } = buyerPhoneOrAddress;
    public ulong Total { get; } = totalSum;
    public ulong Cash { get; } = cashTotalSum;
    public uint ReceiptOperation { get; } = operationType;
    public uint FiscalSign { get; } = fiscalSign;
    public ulong NonCash { get; } = ecashTotalSum;
    public ulong Prepaid { get; } = prepaidSum;
    public ulong Credit { get; } = creditSum;
    public uint Taxation { get; } = taxationType;
    public ulong Provision { get; } = provisionSum;
    public uint DocNumber { get; } = fiscalDocumentNumber;
    public SabyReceiptItem[] Items { get; } = items;

    public Receipt ToReceipt() => new()
    {
        Items = [.. Items.Select(i => i.ToPosition())],
        Buyer = Buyer,
        CashSum = Cash,
        CreditSum = Credit,
        PrepaidSum = Prepaid,
        NonCashSum = NonCash,
        OtherSum = Provision,
        DateTime = DateTime.FromUnixTimeSeconds(UnixTimestamp),
        DriveSerial = DriveSerial,
        FiscalSign = FiscalSign,
        Number = DocNumber,
        NumberInSession = NumberInSession,
        RegNumber = RegNumber,
        SessionNumber = SessionId,
        Taxation = (TaxationSystem)Taxation,
        TotalSum = Total,
        Type = (ReceiptType)ReceiptOperation
    };
}