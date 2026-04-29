using PuppeteerSharp;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RetailCorrector.Utils.Integrations.Clients;

internal sealed class AstralOfdClient(string host = "https://ofd.astralnalog.ru") : ApiClient<AstralOfdClient>(host)
{
    public async Task AuthByEmailAsync(string email, string password, string host = "ofd.astralnalog.ru", CancellationToken cancellationToken = default)
    {
        var options = new LaunchOptions
        {
            Headless = true,
            ExecutablePath = Configuration.Instance.HttpClient.HeadlessBrowserPath
        };
        cancellationToken.ThrowIfCancellationRequested();
        using var browser = await Puppeteer.LaunchAsync(options);
        cancellationToken.ThrowIfCancellationRequested();
        using var page = await browser.NewPageAsync();
        cancellationToken.ThrowIfCancellationRequested();
        await page.GoToAsync($"http://{host}/lk/");
        cancellationToken.ThrowIfCancellationRequested();
        await page.ClickAsync("a[data-test='authLoginTypeToggle']");
        cancellationToken.ThrowIfCancellationRequested();
        await page.TypeAsync("input[name='email']", email);
        cancellationToken.ThrowIfCancellationRequested();
        await page.TypeAsync("input[name='password']", password);
        cancellationToken.ThrowIfCancellationRequested();
        await page.ClickAsync("button[type='submit']");
        cancellationToken.ThrowIfCancellationRequested();
        await page.WaitForResponseAsync(resp => resp.Url.EndsWith("users.getMe"));
        cancellationToken.ThrowIfCancellationRequested();
        Cookies.Add((await page.GetCookiesAsync()).First(c => c.Name == "connect.sid").ToCookie());
    }

    public async Task<IEnumerable<AstralOfdOrganization>> GetOrganizationIdAsync(CancellationToken cancellationToken = default)
    {
        using var resp = await Client.GetAsync("/lk/api/v4.2/users.getMe", cancellationToken).ConfigureAwait(false);
        await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        if (!resp.IsSuccessStatusCode)
            throw new HttpRequestException(null, null, resp.StatusCode);
        var content = await JsonSerializer.DeserializeAsync<AstralOfdGetMeResponse>
            (stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (!content.Success)
        {
            var builder = new StringBuilder();
            throw new HttpRequestException(builder.ToString());
        }
        return content.Data.Organizations;
    }

    public async IAsyncEnumerable<AstralOfdKkt> GetKktsAsync
        (string orgId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;
        var kkts = await GetKktsByPageAsync(orgId, page, Configuration.Instance.HttpClient.PageSize, cancellationToken);
        var pageCount = (int)Math.Ceiling(kkts.TotalCount / (decimal)Configuration.Instance.HttpClient.PageSize);
        do
        {
            page++;
            foreach (var kkt in kkts.Kkts)
                yield return kkt;
            if (page > pageCount) yield break;
            kkts = await GetKktsByPageAsync(orgId, page, Configuration.Instance.HttpClient.PageSize, cancellationToken);
        } while (true);
    }

    internal async Task<AstralOfdKktSearchResponseItem> GetKktsByPageAsync
        (string organizationId, int page, int count, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(organizationId, out _))
            throw new ArgumentException("Входной параметр не является целочисленным", nameof(organizationId));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(page);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);
        using var req = new HttpRequestMessage(HttpMethod.Post, "/lk/api/v4.2/kkt.search")
        {
            Content = JsonContent.Create(new { organizationId, page, count })
        };
        using var resp = await Client.SendAsync(req, cancellationToken).ConfigureAwait(false);
        await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        if (!resp.IsSuccessStatusCode)
            throw new HttpRequestException(null, null, resp.StatusCode);
        var content = await JsonSerializer.DeserializeAsync<AstralOfdKktSearchResponse>
            (stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (!content.Success)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Сервер сообщил об ошибках запроса:");
            foreach(var property in content.Errors!.Value.EnumerateObject())
                builder.AppendLine($"- {property.Name}: {property.Value}");
            throw new HttpRequestException(builder.ToString());
        }
        return content.Data;
    }
}

file interface IAstralOfdResponse<out T>
{
    public bool Success { get; }
    public T? Data { get; }
    public JsonElement? Errors { get; }
}

[method: JsonConstructor]
internal readonly struct AstralOfdKktSearchResponse(bool ok, AstralOfdKktSearchResponseItem result, JsonElement? description) 
    : IAstralOfdResponse<AstralOfdKktSearchResponseItem>
{
    public bool Success { get; } = ok;
    public AstralOfdKktSearchResponseItem Data { get; } = ok ? result : default;
    public JsonElement? Errors { get; } = description;
}

[method: JsonConstructor]
internal readonly struct AstralOfdKktSearchResponseItem(AstralOfdKkt[] kkts, string totalCount)
{
    public AstralOfdKkt[] Kkts { get; } = kkts;
    public int TotalCount => int.Parse(totalCount);
}

[method: JsonConstructor]
internal readonly struct AstralOfdKkt(string id, string kktRegID)
{
    public string Id { get; } = id;
    public override string ToString() => kktRegID;
}

[method: JsonConstructor]
internal readonly struct AstralOfdGetMeResponse(bool ok, AstralOfdGetMeResponseItem result, JsonElement? description) 
    : IAstralOfdResponse<AstralOfdGetMeResponseItem>
{
    public bool Success { get; } = ok;
    public AstralOfdGetMeResponseItem Data { get; } = ok ? result : default;
    public JsonElement? Errors { get; } = description;
}

[method: JsonConstructor]
internal readonly struct AstralOfdGetMeResponseItem(AstralOfdOrganization[] organization)
{
    public AstralOfdOrganization[] Organizations { get; } = organization;
}

[method: JsonConstructor]
public readonly struct AstralOfdOrganization(string id, string name)
{
    public string Id { get; } = id;
    public override string ToString() => name;
}
