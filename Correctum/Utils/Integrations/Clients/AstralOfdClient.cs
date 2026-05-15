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

public readonly struct AstralOfdTicket
{
    [JsonPropertyName("checkNumber")] public int DocNumber { get; init; }
    [JsonPropertyName("fiscalDriveNumber")] public string DriveSerial { get; init; }
    [JsonPropertyName("documentType")] public string DocumentType { get; init; }
    [JsonPropertyName("fiscalSign")] public uint DocSign { get; init; }
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
[JsonSerializable(typeof(AstralOfdOrganizationRequest))]
[JsonSerializable(typeof(AstralOfdResponse<AstralOfdOrganizations>))]
internal partial class AstralOfdSerializerContext : JsonSerializerContext;
