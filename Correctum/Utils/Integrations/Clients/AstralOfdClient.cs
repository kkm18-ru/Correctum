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

[JsonSerializable(typeof(AstralOfdOrganizationRequest))]
[JsonSerializable(typeof(AstralOfdResponse<AstralOfdOrganizations>))]
internal partial class AstralOfdSerializerContext : JsonSerializerContext;
