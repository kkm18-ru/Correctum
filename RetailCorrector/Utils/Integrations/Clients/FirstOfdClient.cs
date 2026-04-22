using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RetailCorrector.Utils.Integrations.Clients;

public sealed class FirstOfdClient(string host = "https://org.1-ofd.ru") : ApiClient<FirstOfdClient>(host)
{
    public async Task AuthAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "/api/cp-core/user/login")
        {
            Content = JsonContent.Create(new FirstOfdAuthRequest(login, password))
        };
        using var resp = await Client.SendAsync(req, cancellationToken);
        await ThrowIfFailedStatusCodeAsync(resp);
    }

    public async Task<Receipt> GetReceiptAsync(string kktRegId, string fnId, uint docId, CancellationToken cancellationToken = default)
    {
        var path = string.Format("/api/cp-ofd/ticket/{0}_{1}_{2}", kktRegId, fnId, docId);
        using var resp = await Client.GetAsync(path, cancellationToken);
        await ThrowIfFailedStatusCodeAsync(resp);
        await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken);
        var data = await JsonSerializer.DeserializeAsync<TicketInformation>
            (stream, cancellationToken: cancellationToken);
        return FirstOfdHelper.ToReceipt(data.ticket);
    }

    private static async Task ThrowIfFailedStatusCodeAsync(HttpResponseMessage resp)
    {
        if (resp.IsSuccessStatusCode) return;
        await using var stream = await resp.Content.ReadAsStreamAsync(CancellationToken.None);
        var exc = await JsonSerializer.DeserializeAsync<FirstOfdException>(stream, cancellationToken: CancellationToken.None);
        throw exc ?? new HttpRequestException("Неизвестная ошибка сервера.", null, resp.StatusCode);
    }
}

internal readonly record struct FirstOfdAuthRequest(string login, string password);

public class FirstOfdException : HttpRequestException
{
    [JsonPropertyName("message")] public string MessageId { get; set; }
    [JsonPropertyName("localizedMessage")] public string LocalizedMessage { get; set; }
    [JsonPropertyName("payload")] public object? Payload { get; set; }

    public override string ToString() => 
        string.IsNullOrWhiteSpace(LocalizedMessage) 
        ? MessageId 
        : LocalizedMessage;
}

[method: JsonConstructor]
internal readonly record struct TicketInformation(Ticket ticket, string ticketDescription, string orgTitle, string orgId, string retailPlaceAddress, string kkmSerialNumber, string kkmFnsId, object kkmExtraInfo, string fnsFlcStatus);

[method: JsonConstructor]
internal readonly record struct Ticket(
    TicketItem[] items,
    int taxationType,
    int requestNumber,
    //int operationType, todo использовать его когда будут лучше известны значение (2 - приход)
    string transactionDate,
    int fiscalDocumentNumber,
    string kktRegId,
    string fiscalDriveNumber,
    int shiftNumber,
    string fiscalId,
    decimal totalSum,
    decimal cashTotalSum,
    decimal ecashTotalSum,
    decimal prepaymentSum,
    decimal postpaymentSum,
    decimal counterSubmissionSum,
    JsonElement options);

[method: JsonConstructor]
internal readonly record struct TicketItem(
    string name,
    int calculationSubjectSign,
    int calculationTypeSign,
    int ndsRate,
    int quantityMeasure,
    long price,
    decimal quantity,
    long sum);

internal static class FirstOfdHelper
{
    public static Receipt ToReceipt(Ticket ticket) => new()
    {
        /* todo
        Type = ticket.operationType switch
        {
            2 => ReceiptType.Income,
            _ => throw new ArgumentOutOfRangeException
                ($"{nameof(ticket)}.{nameof(Ticket.operationType)}")
        },*/
        Type = (ReceiptType)int.Parse(ticket.options.GetProperty("operationType").GetString()),
        Created = DateTime.ParseExact(ticket.transactionDate, "yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture),
        StorageId = ticket.fiscalDriveNumber.Trim(),
        RegId = ticket.kktRegId.Trim(),
        DocInSession = (uint)ticket.requestNumber,
        TotalSum = ticket.totalSum,
        Payment = new Payment
        {
            CashSum = ticket.cashTotalSum,
            NonCashSum = ticket.ecashTotalSum,
            AdvanceSum = ticket.prepaymentSum,
            CreditSum = ticket.postpaymentSum,
            ProvisionSum = ticket.counterSubmissionSum,
        },
        DocNumber = (uint)ticket.fiscalDocumentNumber,
        SessionId = (uint)ticket.shiftNumber,
        Items = [..ticket.items.Select(ToPosition)],
        DocSign = FiscalSign.Parse(ticket.fiscalId),
        TaxationSystem = (TaxationSystems)ticket.taxationType,
        BuyerContact = ""
    };

    public static Position ToPosition(TicketItem item)
    {
        if (item.quantity < 0)
            throw new ArgumentOutOfRangeException
                ($"{nameof(item)}.{nameof(TicketItem.quantity)}");
        return new()
        {
            Name = item.name,
            Type = (PositionType)item.calculationSubjectSign,
            CalcMethod = (CalculationMethod)item.calculationTypeSign,
            TaxRate = (NDSRate)item.ndsRate,
            Unit = (UnitOfMeasure)item.quantityMeasure,
            Price = item.price,
            Quantity = (ulong)Math.Round(item.quantity * 1000),
            Cost = item.sum,
        };
    }
}