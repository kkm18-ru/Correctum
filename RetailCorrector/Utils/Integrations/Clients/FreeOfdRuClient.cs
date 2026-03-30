using RetailCorrector.Data;
using RetailCorrector.Parsers;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace RetailCorrector.Utils.Integrations.Clients;

public sealed class FreeOfdRuClient(string host = "https://lk.ofd.ru") : ApiClient<FreeOfdRuClient>(host)
{
    private Guid OfdAgreementId = Guid.Empty;
    private string AccountId = string.Empty;

    public async Task AuthAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        string pac = string.Empty;
        using var req = new HttpRequestMessage(HttpMethod.Post, "/api/Authorization/Authorize");
        req.Content = JsonContent.Create(new AuthorizeRequest(login, password));
        using var resp = await Client.SendAsync(req, cancellationToken).ConfigureAwait(false);
        foreach(var cookie in resp.Headers.GetValues("Set-Cookie"))
        {
            var valueEndIndex = cookie.IndexOf(';');
            var keyEndIndex = cookie.IndexOf('=');
            switch (cookie[..keyEndIndex])
            {
                case "PAC":
                    if(pac.Length == 0)
                        pac = cookie[(keyEndIndex + 1)..valueEndIndex];
                    break;
                case "AccountId":
                    if (AccountId.Length == 0)
                        AccountId = cookie[(keyEndIndex + 1)..valueEndIndex].ToString();
                    break;
            }
        }
        await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var content = await JsonSerializer.DeserializeAsync<AuthorizeResponse>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        foreach (var account in content.Data.AvailableAccounts)
        {
            if(account.Type == "Client")
            {
                OfdAgreementId = account.ObjectId;
                break;
            }
        }
        if (OfdAgreementId == Guid.Empty) 
            throw new Exception("Идентификатор соглашения с ОФД не найден.");
        Client.DefaultRequestHeaders.Add("Cookie", $"PAC={pac}; AccountId={AccountId}");
    }

    public async IAsyncEnumerable<FreeOfdRuDevice> FetchDevicesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int PageCount = 0;
        using (var req = new HttpRequestMessage(HttpMethod.Post, "/api/rpc/v1/GroupFilter")) { 
            req.Content = JsonContent.Create(new GroupFilterRequest(OfdAgreementId));
            using var resp = await Client.SendAsync(req, cancellationToken).ConfigureAwait(false);
            await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var content = await JsonSerializer.DeserializeAsync<GroupFilterResponse>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
            PageCount = (int)Math.Ceiling((double)content.Data.KktTotal / Configuration.Instance.HttpClient.PageSize);
            foreach (var dev in content.Data.List) yield return dev;
        }
        if (PageCount == 1) yield break;
        for(var pageId = 1; pageId < PageCount; pageId++)
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, "/api/rpc/v1/GroupFilter");
            req.Content = JsonContent.Create(new GroupFilterRequest(OfdAgreementId, pageId));
            using var resp = await Client.SendAsync(req, cancellationToken).ConfigureAwait(false);
            await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var content = await JsonSerializer.DeserializeAsync<GroupFilterResponse>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
            foreach (var dev in content.Data.List) yield return dev;
        }
    }

    public async Task<string[]> FetchStoragesByDeviceAsync(FreeOfdRuDevice dev, CancellationToken cancellationToken = default)
    {
        if (dev.FnNumber is null) return [];
        using var req = new HttpRequestMessage(HttpMethod.Post, $"/api/rpc/v1/getFNInfo4KKTTab");
        req.Content = JsonContent.Create(new StorageRequest(dev, OfdAgreementId));
        using var resp = await Client.SendAsync(req, cancellationToken).ConfigureAwait(false);
        await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var content = await JsonSerializer.DeserializeAsync<StorageResponse>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        return [dev.FnNumber, .. content.Data.listFNS ?? []];
    }

    public async IAsyncEnumerable<int> FetchDocsByStorageAsync(string devId, string storageId, DateTime begin, DateTime end, [EnumeratorCancellation] CancellationToken cancellationToken= default)
    {
        const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fff";
        int PageCount = 0;
        var textBegin = begin.ToString(DateTimeFormat);
        var textEnd = end.ToString(DateTimeFormat);
        var url = $"/api/document/v2/GetKktsDocuments?AccountId={AccountId}&PageCount={Configuration.Instance.HttpClient.PageSize}&OfdAgreementId={OfdAgreementId:D}&KKTRegNumber={devId}&fNNumber={storageId}&docType%5B%5D=cash_receipt&docType%5B%5D=cash_receipt_correction&dateFrom={textBegin}&dateTo={textEnd}";

        using (var resp = await Client.GetAsync($"{url}&Page=0", cancellationToken).ConfigureAwait(false)) { 
            await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var content = await JsonSerializer.DeserializeAsync<KktsDocumentsResponse>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
            PageCount = (int)Math.Ceiling((double)content.Data.TotalCount / Configuration.Instance.HttpClient.PageSize);
            foreach (var dev in content.Data.List) yield return dev.DocNumber;
        }
        if (PageCount == 1) yield break;
        for (var pageId = 1; pageId < PageCount; pageId++)
        {
            using var resp = await Client.GetAsync($"{url}&Page={pageId}", cancellationToken).ConfigureAwait(false);
            await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var content = await JsonSerializer.DeserializeAsync<KktsDocumentsResponse>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
            foreach (var dev in content.Data.List) yield return dev.DocNumber;
        }
    }
    public async Task<Receipt> FetchDetailDocumentAsync(Guid parseId, Guid DevAgreeId, int DocNumber, string storageId, CancellationToken cancellationToken = default)
    {
        var receipt = new Receipt { ParseId = parseId };
        using var resp = await Client.GetAsync($"/api/Customer/GetJsonDoc?KktAgreementId={DevAgreeId:D}&DocNumber={DocNumber}&CustomFnNumber={storageId}", cancellationToken).ConfigureAwait(false);
        await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var content = await JsonSerializer.DeserializeAsync<JsonDocResponse>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        OfdRuJson.Instance.Parse(content.Data.Container.ToString(), ref receipt);
        return receipt;
    }


}

file struct JsonDocResponse
{
    public string Status { get; set; }
    public JsonDocResponseData Data { get; set; }
    public string Elapsed { get; set; }
}

file struct JsonDocResponseData
{
    public JsonNode Container { get; set; }
}

file readonly struct StorageRequest(FreeOfdRuDevice dev, Guid ofdAgreeId)
{
    [JsonPropertyName("kktAgreementId")]
    public string DevAgreementId { get; } = dev.AgreementId.ToString("D");
    [JsonPropertyName("ofdAgreementId")]
    public string OfdAgreementId { get; } = ofdAgreeId.ToString("D");
    [JsonPropertyName("fn")]
    public string Fn { get; } = dev.FnNumber;
}

file readonly struct GroupFilterRequest(Guid ofdAgreeId, int page = 0)
{
    [JsonPropertyName("version")]
    public int Version { get; } = 2;
    public int Page { get; } = page;
    public int PageSize { get; } = Configuration.Instance.HttpClient.PageSize;
    public string OfdAgreementId { get; } = ofdAgreeId.ToString("D");
}

public struct FreeOfdRuDevice
{
    [JsonPropertyName("Id")]
    public Guid AgreementId { get; set; }
    public string RegNumber { get; set; }
    public string FnNumber { get; set; }
}

file struct GroupFilterResponse
{
    public GroupFilterResponseData Data { get; set; }
    public bool Success { get; set; }
    public string Description { get; set; }
}

file struct GroupFilterResponseData
{
    public ICollection<FreeOfdRuDevice> List { get; set; }
    public int KktTotal { get; set; }
}

file readonly struct AuthorizeRequest(string login, string password)
{
    [JsonPropertyName("version")]
    public int Version { get; } = 1;
    public bool V2Auth { get; } = true;
    public string Login { get; } = login;
    public string Password { get; } = password;
}

file struct AuthorizeResponse
{
    public AuthorizeResponseData Data { get; set; }
    public bool Success { get; set; }
    public string HttpStatusCode { get; set; }
}

file struct AuthorizeResponseData
{
    public ICollection<AvailableAccount> AvailableAccounts { get; set; }
    public bool Success { get; set; }
    public string Description { get; set; }
}

file struct AvailableAccount
{
    public string OrgName { get; set; }
    public Guid ObjectId { get; set; }
    public string Type { get; set; }
}

file struct StorageResponse
{
    public StorageResponseData Data { get; set; }
    public bool Success { get; set; }
}

file struct StorageResponseData
{
    public ICollection<string> listFNS { get; set; }
}

file struct KktsDocumentsResponse
{
    public KktsDocumentsResponseData Data { get; set; }
    public bool Success { get; set; }
}

file struct KktsDocumentsResponseData
{
    public ICollection<KktDocument> List { get; set; }
    public int TotalCount { get; set; }
}

file struct KktDocument
{
    public int DocNumber { get; set; }
}