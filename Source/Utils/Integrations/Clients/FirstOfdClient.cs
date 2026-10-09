using Correctum.Enums;
using Correctum.Models;
using Correctum.ValueObjects;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Correctum.Utils.Integrations.Clients;

public sealed class FirstOfdClient(string host) : ApiClient<FirstOfdClient>(host)
{
    public FirstOfdClient() : this("https://org.1-ofd.ru") { }
    public async Task AuthAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        using var req = new HttpRequestMessage(HttpMethod.Post, "/api/cp-core/user/login")
        {
            Content = JsonContent.Create(new FirstOfdAuthRequest(login, password))
        };
        using var resp = await Client.SendAsync(req, cancellationToken);
        await ThrowIfFailedStatusCodeAsync(resp);
    }

    public async Task<int[]> GetRetailPlaceIdsAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        using var resp = await Client.GetAsync("/api/cp-ofd/retail-places/without-group", cancellationToken);
        await ThrowIfFailedStatusCodeAsync(resp);
        await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken);
        var content = JsonSerializer.DeserializeAsyncEnumerable<FirstOfdRetailPlace>
            (stream, cancellationToken: cancellationToken);
        return await content.Select(c => c.id).ToArrayAsync(cancellationToken);
    }

    public async Task<FirstOfdKkt[]> GetKktsAsync(int placeId, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        using var req = new HttpRequestMessage(HttpMethod.Post, $"/api/cp-ofd/retail-places/filter-retail-place/{placeId}")
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        };
        using var resp = await Client.SendAsync(req, cancellationToken);
        await ThrowIfFailedStatusCodeAsync(resp);
        await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken);
        var content = await JsonSerializer.DeserializeAsync<FirstOfdRetailPlaceDetails>
            (stream, cancellationToken: cancellationToken);
        return content.kkms;
    }

    public async Task<FirstOfdFiscalDrive[]> GetFiscalDrivesAsync(FirstOfdKkt kkt, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        using var resp = await Client.GetAsync($"/api/cp-ofd/kkms/{kkt.id}/fiscal-drives", cancellationToken);
        await ThrowIfFailedStatusCodeAsync(resp);
        await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken);
        var content = JsonSerializer.DeserializeAsyncEnumerable<FirstOfdFiscalDrive>
            (stream, cancellationToken: cancellationToken);
        return await content.ToArrayAsync(cancellationToken);
    }

    public async IAsyncEnumerable<FirstOfdTransaction> GetTransactionsAsync
        (FirstOfdKkt kkt, DateTime fromDate, DateTime toDate, FirstOfdFiscalDrive drive, int size, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var builder = new StringBuilder();
        builder.AppendFormat("/api/cp-ofd/kkms/{0}/transactions", kkt.id);
        builder.AppendFormat("?fromDate={0}", fromDate.UnixTimestampMilliseconds);
        builder.AppendFormat("&toDate={0}", toDate.UnixTimestampMilliseconds);
        builder.AppendFormat("&fiscalDriveNumber={0}", drive.number);
        builder.Append("&transactionTypes=TICKET,RECEIPT_CORRECTION");
        builder.AppendFormat("&pageSize={0}", size);
        var uri = builder.ToString();
        int currPage = 1, countPages = 1;
        bool needCalcPages = true;
        while(currPage <= countPages)
        {
            using var resp = await Client.GetAsync($"{uri}&page={currPage}", cancellationToken);
            await ThrowIfFailedStatusCodeAsync(resp);
            await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken);
            var data = await JsonSerializer.DeserializeAsync<FirstOfdTransactionInformation>
                (stream, cancellationToken: cancellationToken);
            foreach (var tr in data.transactions) yield return tr;
            if (needCalcPages)
            {
                countPages = (int)Math.Ceiling(data.pagination.totalItems / (decimal)size);
                needCalcPages = false;
            }
            currPage++;
        }
    }

    public Task<Receipt> GetReceiptAsync(FirstOfdTransaction tr, CancellationToken cancellationToken = default) =>
        GetReceiptAsync(tr.kkmId, tr.fiscalDriveNumber, tr.fiscalDocumentNumber, cancellationToken);

    public async Task<Receipt> GetReceiptAsync(string kktRegId, string fnId, uint docId, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var path = string.Format("/api/cp-ofd/ticket/{0}_{1}_{2}", kktRegId, fnId, docId);
        using var resp = await Client.GetAsync(path, cancellationToken);
        await ThrowIfFailedStatusCodeAsync(resp);
        await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken);
        var data = await JsonSerializer.DeserializeAsync<FirstOfdTicketInformation>
            (stream, cancellationToken: cancellationToken);
        return FirstOfdHelper.ToReceipt(data.ticket);
    }

    private static async Task ThrowIfFailedStatusCodeAsync(HttpResponseMessage resp)
    {
        if (resp.IsSuccessStatusCode) return;
        await using var stream = await resp.Content.ReadAsStreamAsync(CancellationToken.None);
        var exc = await JsonSerializer.DeserializeAsync<FirstOfdError?>(stream, cancellationToken: CancellationToken.None);
        throw exc?.ToException() ?? new HttpRequestException("Неизвестная ошибка сервера.", null, resp.StatusCode);
    }
}

#pragma warning disable IDE1006

[method: JsonConstructor]
public readonly record struct FirstOfdFiscalDrive(string number, DateTime? activationDate, DateTime? expireDate, DateTime? closeArchiveDate)
{
    public override string ToString() => number.ToString();
}

[method: JsonConstructor]
internal readonly record struct FirstOfdRetailPlace(int id);

[method: JsonConstructor]
internal readonly record struct FirstOfdRetailPlaceDetails(FirstOfdKkt[] kkms);

[method: JsonConstructor]
public readonly record struct FirstOfdKkt(int id, string fnsKkmId)
{
    public override string ToString() => fnsKkmId;
}

internal readonly record struct FirstOfdAuthRequest(string login, string password);

[method: JsonConstructor]
internal readonly record struct FirstOfdError(string message, string localizedMessage, FirstOfdErrorPayload? payload = null)
{
    public FirstOfdException ToException() => new(message, localizedMessage, payload);
}

public class FirstOfdException : HttpRequestException
{
    internal string MessageId { get; init; }
    internal string LocalizedMessage { get; init; }
    internal FirstOfdErrorPayload? Payload { get; init; }

    internal FirstOfdException(string message, string localizedMessage, FirstOfdErrorPayload? payload)
    {
        Payload = payload;
        MessageId = message;
        LocalizedMessage = localizedMessage;
    }

    public override string ToString() =>
        string.IsNullOrWhiteSpace(LocalizedMessage)
        ? MessageId
        : LocalizedMessage;
}

[method: JsonConstructor]
internal readonly record struct FirstOfdTransactionInformation(FirstOfdTransactionPagination pagination, FirstOfdTransaction[] transactions);

[method: JsonConstructor]
internal readonly record struct FirstOfdTransactionPagination(uint totalItems);

[method: JsonConstructor]
public readonly record struct FirstOfdTransaction(string kkmId, string fiscalDriveNumber, uint fiscalDocumentNumber)
{
    public override string ToString() => fiscalDocumentNumber.ToString();
}

[method: JsonConstructor]
internal readonly record struct FirstOfdTicketInformation(FirstOfdTicket ticket);

[method: JsonConstructor]
internal readonly record struct FirstOfdTicket(
    FirstOfdTicketItem[]? items,
    int? taxationType,
    int? requestNumber,
    string? transactionDate,
    int? fiscalDocumentNumber,
    string? kktRegId,
    string? fiscalDriveNumber,
    int? shiftNumber,
    string? fiscalId,
    decimal? totalSum,
    decimal? cashTotalSum,
    decimal? ecashTotalSum,
    decimal? prepaymentSum,
    decimal? postpaymentSum,
    decimal? counterSubmissionSum,
    JsonElement? options);

[method: JsonConstructor]
internal readonly record struct FirstOfdTicketItem(
    string? name,
    int? calculationSubjectSign,
    int? calculationTypeSign,
    int? ndsRate,
    int? quantityMeasure,
    long? price,
    decimal? quantity,
    long? sum);

internal static class FirstOfdHelper
{
    public static Receipt ToReceipt(FirstOfdTicket ticket) => new()
    {
        Type =
(            ticket.options?.TryGetProperty("operationType", out var operationElement) ?? false) &&
            operationElement.GetString() is { } strOperation ?
            (ReceiptType)int.Parse(strOperation) : null,
        Created = ticket.transactionDate is null 
            ? throw new ArgumentNullException(null, "ticket.transactionDate")
            : DateTime.ParseExact(ticket.transactionDate, 
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture),
        StorageId = ticket.fiscalDriveNumber?.Trim(),
        RegId = ticket.kktRegId?.Trim(),
        DocInSession = (uint?)ticket.requestNumber,
        TotalSum = ticket.totalSum ?? Currency.Zero,
        CashSum = ticket.cashTotalSum ?? Currency.Zero,
        NonCashSum = ticket.ecashTotalSum ?? Currency.Zero,
        AdvanceSum = ticket.prepaymentSum ?? Currency.Zero,
        CreditSum = ticket.postpaymentSum ?? Currency.Zero,
        ProvisionSum = ticket.counterSubmissionSum ?? Currency.Zero,
        DocNumber = (uint?)ticket.fiscalDocumentNumber,
        SessionId = (uint?)ticket.shiftNumber,
        Items = ticket.items?.Select(ToPosition).ToList() ?? [],
        DocSign = FiscalSign.Parse(ticket.fiscalId),
        TaxationSystem = (TaxationSystems?)ticket.taxationType,
        //todo BuyerContact
    };

    public static Position ToPosition(FirstOfdTicketItem item)
    {
        if (item.quantity < 0)
            throw new ArgumentOutOfRangeException
                ($"{nameof(item)}.{nameof(FirstOfdTicketItem.quantity)}");
        return new()
        {
            Name = item.name,
            Type = (PositionType?)item.calculationSubjectSign,
            CalcMethod = (CalculationMethod?)item.calculationTypeSign,
            TaxRate = item.ndsRate is null ? null : NDSRate.FromFns(item.ndsRate),
            Unit = (UnitOfMeasure?)item.quantityMeasure,
            Price = item.price,
            Quantity = item.quantity,
            Cost = item.sum,
        };
    }
}

[method: JsonConstructor]
internal readonly record struct FirstOfdErrorPayload(FirstOfdFieldError[] fieldErrors);
[method: JsonConstructor]
internal readonly record struct FirstOfdFieldError(string field, string value, string message, string localizedMessage);

#pragma warning restore IDE1006