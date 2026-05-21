using Correctum.Core.Models;
using Correctum.Deserializers;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Correctum.Utils.Integrations.Clients;

public sealed class AccountOfdRuClient(string host) : 
    ApiClient<AccountOfdRuClient>(host)
{
    public AccountOfdRuClient() : this("https://lk.ofd.ru/") { }

    public async Task AuthAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        var reqContent = new HtmlOfdRuAuthorizeRequest(login, password);
        using var req = new HttpRequestMessage(HttpMethod.Post, "/api/Authorization/Authorize")
        {
            Content = JsonContent.Create(reqContent, HtmlOfdRuSerializerContext.Default.HtmlOfdRuAuthorizeRequest)
        };
        await Client.SendAsync(req, cancellationToken).ConfigureAwait(false);
    }

    public async Task<HtmlOfdRuUserIds> GetUserIdsAsync(CancellationToken cancellationToken = default)
    {
        using var resp = await Client.GetAsync("/api/UserAccounts/GetUserInfo", cancellationToken).ConfigureAwait(false); 
        await using var respStream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var respContent = await JsonSerializer.DeserializeAsync
            (respStream, HtmlOfdRuSerializerContext.Default.HtmlOfdRuGetUserInfoResponse, cancellationToken)
            .ConfigureAwait(false);
        return new(respContent.UserInfo.Id, respContent.AvailableAccounts.First(a => a.Type == "Client").ObjectId);
    }

    public async IAsyncEnumerable<HtmlOfdRuKkt> GetKktsAsync
        (Guid ofdAgreementId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var pageCount = 1;
        for (var page = 0u; page < pageCount; page++)
        {
            var resp = await GetKktsAsync(ofdAgreementId, page, cancellationToken);
            if (page == 0) pageCount = (int)Math.Ceiling((double)resp.KktTotal / Configuration.Current.Http.PageSize);
            foreach (var item in resp.List) yield return item;
        }
    }

    internal async Task<HtmlOfdRuKkts> GetKktsAsync(Guid ofdAgreementId, uint page, CancellationToken cancellationToken)
    {
        var reqContent = new HtmlOfdRuGroupFilterRequest(ofdAgreementId, page);
        using var req = new HttpRequestMessage(HttpMethod.Post, "/api/rpc/v1/GroupFilter")
        {
            Content = JsonContent.Create(reqContent, HtmlOfdRuSerializerContext.Default.HtmlOfdRuGroupFilterRequest)
        };
        using var resp = await Client.SendAsync(req, cancellationToken).ConfigureAwait(false);
        await using var respStream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var respContent = await JsonSerializer.DeserializeAsync
            (respStream, HtmlOfdRuSerializerContext.Default.HtmlOfdRuRpcResponseHtmlOfdRuKkts, cancellationToken)
            .ConfigureAwait(false);
        return respContent.Data;
    }

    public async Task<string[]> GetDrivesAsync(Guid ofdAgreementId, HtmlOfdRuKkt kkt, CancellationToken cancellationToken = default)
    {
        var reqContent = new HtmlOfdRuGetFNInfoRequest(ofdAgreementId, kkt);
        using var req = new HttpRequestMessage(HttpMethod.Post, "/api/rpc/v1/getFNInfo4KKTTab")
        {
            Content = JsonContent.Create(reqContent, HtmlOfdRuSerializerContext.Default.HtmlOfdRuGetFNInfoRequest)
        };
        using var resp = await Client.SendAsync(req, cancellationToken).ConfigureAwait(false);
        await using var respStream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var respContent = await JsonSerializer.DeserializeAsync
            (respStream, HtmlOfdRuSerializerContext.Default.HtmlOfdRuRpcResponseHtmlOfdRuRpcDataHtmlOfdRuGetFNInfo, cancellationToken)
            .ConfigureAwait(false);
        return respContent.Data.DataSet.Drives;
    }

    public async IAsyncEnumerable<HtmlOfdRuGetKktsDocumentsItem> GetDocsInfoAsync(
        Guid accountId, Guid ofdAgreementId,
        string regNumber, string fnNumber,
        DateTime fromDate, DateTime toDate, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var template = new HtmlOfdRuGetKktsDocumentRequest(accountId, ofdAgreementId, regNumber, fnNumber, fromDate, toDate);
        var pageCount = 1;
        for(var page = 0u; page < pageCount; page++)
        {
            var resp = await GetDocsInfoAsync(template, page, cancellationToken);
            if (page == 0) pageCount = (int)Math.Ceiling((double)resp.TotalCount / Configuration.Current.Http.PageSize);
            foreach (var item in resp.List.Where(i => i.Tag == 3 || i.Tag == 31)) yield return item;
        }
    }

    internal async Task<HtmlOfdRuGetKktsDocumentsData> GetDocsInfoAsync
        (HtmlOfdRuGetKktsDocumentRequest request, uint page, CancellationToken cancellationToken)
    {
        var url = $"/api/document/v2/GetKktsDocuments?{request.ToQuery(page)}";
        using var resp = await Client.GetAsync(url.ToString(), cancellationToken).ConfigureAwait(false);
        await using var respStream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var respContent = await JsonSerializer.DeserializeAsync
            (respStream, HtmlOfdRuSerializerContext.Default.HtmlOfdRuResponseHtmlOfdRuGetKktsDocumentsData, cancellationToken)
            .ConfigureAwait(false);
        return respContent.Data;
    }

    public async Task<Receipt> GetReceiptAsync(Guid kktAgreementId, HtmlOfdRuGetKktsDocumentsItem docInfo, CancellationToken cancellationToken = default)
    {
        var url = $"/api/Customer/GetJsonDoc?KktAgreementId={kktAgreementId:D}&DocNumber={docInfo.DocNumber}&CustomFnNumber={docInfo.FnNumber}";
        using var resp = await Client.GetAsync(url, cancellationToken).ConfigureAwait(false);
        await using var respStream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var respContent = await JsonSerializer.DeserializeAsync
            (respStream, HtmlOfdRuSerializerContext.Default.HtmlOfdRuResponseHtmlOfdRuGetJsonDocData, cancellationToken)
            .ConfigureAwait(false);
        var rawJson = respContent.Data.TlvDictionary.ToString();
        return OfdRuTlvDeserializer.Deserialize(rawJson);
    }
}

internal readonly struct HtmlOfdRuAuthorizeRequest (string login, string password)
{
    public string Login { get; } = login;
    public string Password { get; } = password;
    public bool V2Auth { get; } = true;
    [JsonPropertyName("version")] public int Version { get; } = 1;
}

internal readonly struct HtmlOfdRuGetFNInfoRequest(Guid ofdAgreementId, HtmlOfdRuKkt kkt)
{
    [JsonPropertyName("fn")] public string FnNumber { get; } = kkt.FnNumber;
    [JsonPropertyName("kktAgreementId")] public string KktAgreementId { get; } = kkt.Id.ToString("D");
    [JsonPropertyName("ofdAgreementId")] public string OfdAgreementId { get; } = ofdAgreementId.ToString("D");
}

internal readonly struct HtmlOfdRuRpcResponse<T>
{
    public bool Success { get; init; }
    public T Data { get; init; }
    public string Description { get; init; }
}

internal readonly struct HtmlOfdRuRpcData<T>
{
    public string Status { get; init; }
    public T DataSet { get; init; }
}

internal readonly struct HtmlOfdRuGetFNInfo
{
    [JsonPropertyName("listFNS")] public string[] Drives { get; init; }
}

internal readonly struct HtmlOfdRuResponse<T>
{
    public string Status { get; init; }
    public TimeSpan Elapsed { get; init; }
    public T Data { get; init; }
}

internal readonly struct HtmlOfdRuGetKktsDocumentRequest(
    Guid accountId,
    Guid ofdAgreementId,
    string regNumber,
    string fnNumber,
    DateTime fromDate,
    DateTime toDate)
{
    public string ToQuery(uint page) =>
        $"AccountId={accountId:D}&" +
        $"OfdAgreementId={ofdAgreementId:D}&" +
        $"KKTRegNumber={regNumber}&" +
        $"fNNumber={fnNumber}&" +
        $"docType%5B%5D=cash_receipt&" +
        $"docType%5B%5D=cash_receipt_correction&" +
        $"dateFrom={fromDate:yyyy'-'MM'-'dd'T00:00:00.000'}&" +
        $"dateTo={toDate:yyyy'-'MM'-'dd'T23:59:59.000'}&" +
        $"Page={page}&" +
        $"PageCount={Configuration.Current.Http.PageSize}";
}

internal readonly struct HtmlOfdRuGetKktsDocumentsData
{
    public HtmlOfdRuGetKktsDocumentsItem[] List { get; init; }
    public uint TotalCount { get; init; }
}

public readonly struct HtmlOfdRuGetKktsDocumentsItem
{
    public string FnNumber { get; init; }
    public int Tag { get; init; }
    public uint DocNumber { get; init; }
}

internal readonly struct HtmlOfdRuGetJsonDocData
{
    public JsonElement TlvDictionary { get; init; }
}

internal readonly struct HtmlOfdRuKkts
{
    public uint KktTotal { get; init; }
    public HtmlOfdRuKkt[] List { get; init; }
}

public readonly struct HtmlOfdRuKkt
{
    public Guid Id { get; init; }
    public string FnNumber { get; init; }
    public string RegNumber { get; init; }

    public override string ToString() => RegNumber;
}

internal readonly struct HtmlOfdRuGroupFilterRequest(Guid ofdAgreementId, uint page)
{
    public string OfdAgreementId { get; } = ofdAgreementId.ToString("D");
    public uint Page { get; } = page;
    public int PageSize { get; } = Configuration.Current.Http.PageSize;
    [JsonPropertyName("version")] public int Version { get; } = 2;
}

public readonly struct HtmlOfdRuAccount
{
    public Guid ObjectId { get; init; }
    public string Type { get; init; }
}

internal readonly struct HtmlOfdRuUserInfo
{
    public Guid Id { get; init; }
}

internal readonly struct HtmlOfdRuGetUserInfoResponse
{
    public HtmlOfdRuAccount[] AvailableAccounts { get; init; }
    public HtmlOfdRuUserInfo UserInfo { get; init; }
}

public readonly struct HtmlOfdRuUserIds(Guid accountId, Guid agreementId)
{
    public Guid AccountId { get; } = accountId;
    public Guid OfdAgreementId { get; } = agreementId;
}

[JsonSerializable(typeof(HtmlOfdRuGetUserInfoResponse))]
[JsonSerializable(typeof(HtmlOfdRuAuthorizeRequest))]
[JsonSerializable(typeof(HtmlOfdRuGetFNInfoRequest))]
[JsonSerializable(typeof(HtmlOfdRuGroupFilterRequest))]
[JsonSerializable(typeof(HtmlOfdRuResponse<HtmlOfdRuGetKktsDocumentsData>))]
[JsonSerializable(typeof(HtmlOfdRuResponse<HtmlOfdRuGetJsonDocData>))]
[JsonSerializable(typeof(HtmlOfdRuRpcResponse<HtmlOfdRuRpcData<HtmlOfdRuGetFNInfo>>))]
[JsonSerializable(typeof(HtmlOfdRuRpcResponse<HtmlOfdRuKkts>))]
internal partial class HtmlOfdRuSerializerContext : JsonSerializerContext;
