using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Correctum.ValueObjects;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Correctum.Utils.Integrations.Clients;

public sealed class AstralOfdClient(string host) : ApiClient<AstralOfdClient>(host)
{
    private string SessionId = string.Empty;
    public AstralOfdClient() : this("https://ofd.astralnalog.ru") { }

    public void SetSessionId(string sid) => SessionId = sid;

    public async IAsyncEnumerable<AstralOfdOrganization> EnumOrganizationAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var pageCount = 1u;
        for (var pageNumber = 1; pageNumber <= pageCount; pageNumber++)
        {
            var page = await GetOrganizationPageAsync(pageNumber, cancellationToken);
            if (pageNumber == 1) pageCount = (uint)Math.Ceiling(page.Data.TotalCount / (double)Configuration.Current.Http.PageSize);
            foreach (var organization in page.Data.Organizations) yield return organization;
        }
    }

    private async Task<AstralOfdResponse<AstralOfdOrganizations>> GetOrganizationPageAsync(int page, CancellationToken cancellationToken)
    {
        var ctxRequest = AstralOfdSerializerContext.Default.AstralOfdOrganizationRequest;
        var ctxResponse = AstralOfdSerializerContext.Default.AstralOfdResponseAstralOfdOrganizations;

        var req = new HttpRequestMessage(HttpMethod.Post, "/lk/api/v4.2/organization.list");
        req.Headers.Add("Cookie", $"connect.sid={SessionId}");
        var jsonRequestContent = JsonSerializer.Serialize(new(page), ctxRequest);
        req.Content = new StringContent(jsonRequestContent, new MediaTypeHeaderValue("application/json"));

        var resp = await Client.SendAsync(req, cancellationToken).ConfigureAwait(false);
        await using var respBody = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        return await JsonSerializer.DeserializeAsync(respBody, ctxResponse, cancellationToken).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<AstralOfdTicket> EnumTicketInfoAsync
        (AstralOfdOrganization org, DateTime date, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var pageCount = 1u;
        for(var pageNumber = 1; pageNumber <= pageCount; pageNumber++)
        {
            var page = await GetTicketPageAsync(org.Id, date, pageNumber, cancellationToken);
            if (pageNumber == 1) pageCount = (uint)Math.Ceiling(page.Data.TotalCount / (double)Configuration.Current.Http.PageSize);
            foreach (var ticket in page.Data.Tickets) yield return ticket;
        }
    }

    private async Task<AstralOfdResponse<AstralOfdTickets>> GetTicketPageAsync(string orgId, DateTime date, int page, CancellationToken cancellationToken)
    {
        var ctxRequest = AstralOfdSerializerContext.Default.AstralOfdTicketRequest;
        var ctxResponse = AstralOfdSerializerContext.Default.AstralOfdResponseAstralOfdTickets;

        var req = new HttpRequestMessage(HttpMethod.Post, "/lk/api/v4.2/documents.tickets");
        req.Headers.Add("Cookie", $"connect.sid={SessionId}");
        var jsonRequestContent = JsonSerializer.Serialize(new(orgId, date, page, Configuration.Current.Http.PageSize), ctxRequest);
        req.Content = new StringContent(jsonRequestContent, new MediaTypeHeaderValue("application/json"));

        var resp = await Client.SendAsync(req, cancellationToken).ConfigureAwait(false);
        await using var respBody = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        return await JsonSerializer.DeserializeAsync(respBody, ctxResponse, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Receipt> GetReceiptAsync(AstralOfdTicket ticket, CancellationToken cancellationToken = default)
    {
        var ctxRequest = AstralOfdSerializerContext.Default.AstralOfdReceiptRequest;
        var ctxResponse = AstralOfdSerializerContext.Default.AstralOfdResponseAstralOfdReceipt;

        var req = new HttpRequestMessage(HttpMethod.Post, "/lk/api/v4.2/documents.receiptNew");
        req.Headers.Add("Cookie", $"connect.sid={SessionId}");
        var jsonRequestContent = JsonSerializer.Serialize(ticket.ToRequest(), ctxRequest);
        req.Content = new StringContent(jsonRequestContent, new MediaTypeHeaderValue("application/json"));

        var resp = await Client.SendAsync(req, cancellationToken).ConfigureAwait(false);
        await using var respBody = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var respContent = await JsonSerializer.DeserializeAsync(respBody, ctxResponse, cancellationToken).ConfigureAwait(false);
        return respContent.Data.ToModel();
    }
}

internal readonly struct AstralOfdOrganizationRequest(int page)
{
    [JsonPropertyName("count")] public int PageSize { get; init; } = 10;
    [JsonPropertyName("page")] public int PageNumber { get; init; } = page;
    [JsonPropertyName("firstLoad")] public bool FisrtLoad { get; init; } = true;
}

internal readonly struct AstralOfdResponse<T> where T : struct
{
    [JsonPropertyName("ok")] public bool Success { get; init; }
    [JsonPropertyName("result")] public T Data { get; init; }
}

internal readonly struct AstralOfdReceiptRequest(int docNumber, string driveSerial, uint sign)
{
    [JsonPropertyName("fiscalDocumentNumber")] public int DocumentNumber { get; } = docNumber;
    [JsonPropertyName("fiscalDriveNumber")] public string DriveSerial { get; } = driveSerial;
    [JsonPropertyName("fiscalSign")] public uint DocumentSign { get; } = sign;
}

internal readonly struct AstralOfdTicketRequest(string orgId, DateTime date, int page, int size)
{
    [JsonPropertyName("organizationId")] public string OrganizationId { get; } = orgId;
    [JsonPropertyName("beginDate")] public long BeginDate { get; } = date.Date.UnixTimestampSeconds;
    [JsonPropertyName("endDate")] public long EndDate { get; } = date.Date.UnixTimestampSeconds + 86399;
    [JsonPropertyName("order")] public string Order { get; } = "desc";
    [JsonPropertyName("orderBy")] public string OrderBy { get; } = "dateTime";
    [JsonPropertyName("pageNumber")] public int PageNumber { get; } = page;
    [JsonPropertyName("count")] public int Count { get; } = size;
}

internal readonly struct AstralOfdReceipt
{
    [JsonPropertyName("documentJson")] public AstralOfdReceiptDto Document { get; init; }

    public Receipt ToModel() => Document.ToModel();
}

internal readonly struct AstralOfdReceiptItemDto
{
    [JsonPropertyName("name")] public string? Name { get; init; }
    [JsonPropertyName("productType")] public int? Type { get; init; }
    [JsonPropertyName("nds")] public int? NdsRate { get; init; }
    [JsonPropertyName("paymentType")] public int? CalcMethod { get; init; }
    [JsonPropertyName("itemsQuantityMeasure")] public int? Unit { get; init; }
    [JsonPropertyName("price")] public decimal? Price { get; init; }
    [JsonPropertyName("quantity")] public decimal? Quantity { get; init; }
    [JsonPropertyName("sum")] public decimal? Cost { get; init; }

    public Position ToModel() => new()
    {
        Name = Name,
        CalcMethod = (CalculationMethod?)CalcMethod,
        Cost = Cost,
        Price = Price,
        Quantity = Quantity,
        TaxRate = NdsRate is null ? null : NDSRate.FromFns(NdsRate),
        Type = (PositionType?)Type,
        Unit = (UnitOfMeasure?)Unit,
    };
}

internal readonly struct AstralOfdReceiptDto
{
    [JsonIgnore] public decimal? AdvancePaymentSum { get; init; }
    [JsonPropertyName("buyerPhoneOrAddress")] public string? BuyerContact { get; init; }
    [JsonPropertyName("totalSum")] public decimal? TotalSum { get; init; }
    [JsonPropertyName("cashTotalSum")] public decimal? CashPaymentSum { get; init; }
    [JsonPropertyName("ecashTotalSum")] public decimal? NonCashPaymentSum { get; init; }
    [JsonIgnore] public decimal? CreditPaymentSum { get; init; }
    [JsonIgnore] public decimal? ProvisionPaymentSum { get; init; }
    [JsonPropertyName("dateTime")] public string? Created { get; init; }
    [JsonPropertyName("requestNumber")] public uint? DocInSession { get; init; }
    [JsonPropertyName("fiscalDocumentNumber")] public uint? DocNumber { get; init; }
    [JsonPropertyName("fiscalSign")] public string? DocSign { get; init; }
    [JsonPropertyName("kktRegId")] public string? RegId { get; init; }
    [JsonPropertyName("shiftNumber")] public uint? SessionId { get; init; }
    [JsonPropertyName("fiscalDriveNumber")] public string? StorageId { get; init; }
    [JsonPropertyName("sellerAddress")] public string? SenderAddress { get; init; }
    [JsonPropertyName("retailPlace")] public string? RetailPlace { get; init; }
    [JsonPropertyName("retailAddress")] public string? RetailAddress { get; init; }
    [JsonPropertyName("operator")] public string? OperatorName { get; init; }
    [JsonPropertyName("operatorInn")] public string? OperatorInn { get; init; }
    [JsonPropertyName("taxationType")] public int? TaxationSystem { get; init; }
    [JsonPropertyName("operationType")] public int? Type { get; init; }
    [JsonPropertyName("items")] public AstralOfdReceiptItemDto[]? Items { get; init; }

    public Receipt ToModel() => new()
    {
        AdvanceSum = AdvancePaymentSum,
        BuyerContact = BuyerContact,
        CashSum = CashPaymentSum,
        Created = Created is null ? null : 
            DateTime.ParseExact(Created, "dd'.'MM'.'yyyy' 'HH':'mm", CultureInfo.InvariantCulture),
        CreditSum = CreditPaymentSum,
        DocInSession = DocInSession,
        DocNumber = DocNumber,
        DocSign = DocSign is null ? null : FiscalSign.Parse(DocSign),
        NonCashSum = NonCashPaymentSum,
        ProvisionSum = ProvisionPaymentSum,
        RegId = RegId,
        SessionId = SessionId,
        StorageId = StorageId,
        TaxationSystem = (TaxationSystems?)TaxationSystem,
        TotalSum = TotalSum,
        Type = (ReceiptType?)Type,
        Items = Items?.Select(i => i.ToModel()).ToList() ?? [],
        RetailPlace = RetailPlace,
        RetailAddress = RetailAddress,
        SenderAddress = SenderAddress,
        OperatorName = OperatorName,
        OperatorInn = OperatorInn is null ? null : Inn.Parse(OperatorInn)
    };
}

public readonly struct AstralOfdTicket
{
    [JsonPropertyName("checkNumber")] public int DocNumber { get; init; }
    [JsonPropertyName("fiscalDriveNumber")] public string DriveSerial { get; init; }
    [JsonPropertyName("documentType")] public string DocumentType { get; init; }
    [JsonPropertyName("fiscalSign")] public uint DocSign { get; init; }

    internal AstralOfdReceiptRequest ToRequest() => new(DocNumber, DriveSerial, DocSign);
    public override string ToString() => DocNumber.ToString();
}

internal readonly struct AstralOfdTickets
{
    [JsonPropertyName("documents")] public AstralOfdTicket[] Tickets { get; init; }
    [JsonPropertyName("totalCount")] public int TotalCount { get; init; }
}

public readonly struct AstralOfdOrganization
{
    [JsonPropertyName("fullTitle")] public string Name { get; init; }
    [JsonPropertyName("id")] public string Id { get; init; }

    public override string ToString() => Name;
}

internal readonly struct AstralOfdOrganizations
{
    [JsonPropertyName("organizations")] public AstralOfdOrganization[] Organizations { get; init; }
    [JsonPropertyName("totalCount")] public int TotalCount { get; init; }
}

[JsonSerializable(typeof(AstralOfdTicketRequest))]
[JsonSerializable(typeof(AstralOfdResponse<AstralOfdTickets>))]
[JsonSerializable(typeof(AstralOfdReceiptRequest))]
[JsonSerializable(typeof(AstralOfdResponse<AstralOfdReceipt>))]
[JsonSerializable(typeof(AstralOfdOrganizationRequest))]
[JsonSerializable(typeof(AstralOfdResponse<AstralOfdOrganizations>))]
internal partial class AstralOfdSerializerContext : JsonSerializerContext;