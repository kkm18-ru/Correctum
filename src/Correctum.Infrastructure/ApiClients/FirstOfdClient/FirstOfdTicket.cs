using Correctum.Enums;
using Correctum.Models;
using System.Text.Json.Serialization;

namespace Correctum.ApiClients.FirstOfdClient;

[method: JsonConstructor]
internal readonly struct FirstOfdTicket(
    string kktRegId, 
    string fiscalDriveNumber,
    uint shiftNumber,
    decimal totalSum,
    decimal cashTotalSum,
    decimal ecashTotalSum,
    decimal prepaymentSum,
    decimal postpaymentSum,
    decimal counterSubmissionSum,
    string buyerAddress,
    int taxationType,
    DateTime transactionDate,
    int operationType,
    uint requestNumber,
    string fiscalId,
    uint fiscalDocumentNumber,
    FirstOfdTicketItem[] items)
{
    public string RegNumber { get; } = kktRegId;
    public string FnSerial { get; } = fiscalDriveNumber;
    public uint SessionId { get; } = shiftNumber;
    public decimal TotalSum { get; } = totalSum;
    public decimal CashSum { get; } = cashTotalSum;
    public decimal NonCashSum { get; } = ecashTotalSum;
    public decimal PreSum { get; } = prepaymentSum;
    public decimal PostSum { get; } = postpaymentSum;
    public decimal OtherSum { get; } = counterSubmissionSum;
    public string? Buyer { get; } = buyerAddress;
    public int TaxationSystem { get; } = taxationType;
    public DateTime DateTime { get; } = transactionDate;
    public int Type { get; } = operationType;
    public uint NumberInSession { get; } = requestNumber;
    public string Sign { get; } = fiscalId;
    public uint Number { get; } = fiscalDocumentNumber;
    public FirstOfdTicketItem[] Items { get; } = items;

    public Receipt ToReceipt() => new()
    {
        RegNumber = RegNumber,
        DriveSerial = FnSerial,
        SessionNumber = SessionId,
        TotalSum = (ulong)Math.Round(TotalSum * 100),
        CashSum = (ulong)Math.Round(CashSum * 100),
        NonCashSum = (ulong)Math.Round(NonCashSum * 100),
        PrepaidSum = (ulong)Math.Round(PreSum * 100),
        CreditSum = (ulong)Math.Round(PostSum * 100),
        OtherSum = (ulong)Math.Round(OtherSum * 100),
        Buyer = Buyer,
        Taxation = (TaxationSystem)TaxationSystem,
        DateTime = DateTime,
        NumberInSession = NumberInSession,
        Number = Number,
        FiscalSign = uint.Parse(Sign),
        Type = Type switch
        {
            2 => ReceiptType.Income,
            3 => ReceiptType.RefundIncome,
            _ => throw new NotImplementedException($"{RegNumber}/{FnSerial}/{Number}")
        },
        Items = [.. Items.Select(i => i.ToPosition())]
    };
}
