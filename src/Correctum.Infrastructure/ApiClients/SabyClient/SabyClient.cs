using Correctum.Models;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Correctum.ApiClients.SabyClient;

public sealed class SabyClient(string host = "https://api.sbis.ru") : BaseApiClient(host, true)
{
    private const string AuthUrl = "/oauth/service/"; // Обязательно заканчивается на слеш (иначе 302)
    private const string KktsTemplate = "/ofd/v1/orgs/{0}/kkts";
    private const string StoragesTemplate = "/ofd/v1/orgs/{0}/kkts/{1}/storages";
    private const string ReceiptsTemplate = "/ofd/v1/orgs/{0}/kkts/{1}/storages/{2}/docs?dateFrom={3}&dateTo={4}&startId={5}&limit={6}";
    private const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

    public async Task AuthAsync(string login, string password, bool asFranchise, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(m_disposed, this);
        if (Cookies["sid"] is { } cookie) Cookies.Remove(cookie);
        var reqContent = new SabyAuthRequest(login, password, asFranchise);
        using var req = new HttpRequestMessage(HttpMethod.Post, AuthUrl);
        req.Content = JsonContent.Create(reqContent, SabyJsonContext.Default.SabyAuthRequest);
        using var resp = await Client.SendAsync(req, cancellationToken).ConfigureAwait(false);
        await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        if (resp.IsSuccessStatusCode)
            throw await GetErrorAsync(stream, SabyJsonContext.Default.SabyAuthException, cancellationToken);
        var respContent = await JsonSerializer.DeserializeAsync(stream, SabyJsonContext.Default.SabyAuthResponse, cancellationToken).ConfigureAwait(false);
        Cookies.Add(new Cookie("sid", respContent.SessionId));
    }
    
    public async IAsyncEnumerable<string> EnumerateRegNumbersAsync(string inn, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(m_disposed, this);
        var uri = string.Format(KktsTemplate, inn);
        using var req = new HttpRequestMessage(HttpMethod.Get, uri);
        await foreach (var kkt in await GetEnumerableAsync(req, SabyJsonContext.Default.SabyKkt, cancellationToken)) 
            yield return kkt.RegNumber;
    }

    public async IAsyncEnumerable<SabyFiscalDrive> EnumerateFiscalDrivesAsync
        (string inn, string regNumber, DateTime dateFrom, DateTime dateTo, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(m_disposed, this);
        var uri = string.Format(StoragesTemplate, inn, regNumber);
        using var req = new HttpRequestMessage(HttpMethod.Get, uri);
        await foreach (var item in await GetEnumerableAsync(req, SabyJsonContext.Default.SabyFiscalDrive, cancellationToken)) 
            if(item.ActivatedDate > dateTo || (item.ClosedDate is not null && item.ClosedDate < dateFrom))
                yield return item;
    }

    public async IAsyncEnumerable<Receipt> EnumerateReceiptAsync
        (string inn, string regNumber, string fiscalDrive, DateTime dateFrom, DateTime dateTo,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(m_disposed, this);
        var uri = string.Format(ReceiptsTemplate, inn, regNumber, fiscalDrive, 
            dateFrom.ToString(DateTimeFormat), dateTo.ToString(DateTimeFormat));
        using var req = new HttpRequestMessage(HttpMethod.Get, uri);
        await foreach (var item in await GetEnumerableAsync(req, SabyJsonContext.Default.SabyDocument, cancellationToken))
            if (item.Receipt is SabyReceipt receipt)
                yield return receipt.ToReceipt();
    }

    private async Task<ConfiguredCancelableAsyncEnumerable<TResponse?>> GetEnumerableAsync<TResponse>
        (HttpRequestMessage req, JsonTypeInfo<TResponse> ctx, CancellationToken cancellationToken)
    {
        using var resp = await Client.SendAsync(req, cancellationToken).ConfigureAwait(false);
        await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        if (resp.IsSuccessStatusCode)
            throw resp.StatusCode switch
            {
                HttpStatusCode.InternalServerError => await GetErrorAsync(stream, SabyJsonContext.Default.SabyServerException, cancellationToken),
                HttpStatusCode.Unauthorized => new UnauthorizedAccessException("Нужно произвести авторизацию в ЛК Saby"),
                _ => new Exception("Непредвиденная ошибка")
            };
        return JsonSerializer.DeserializeAsyncEnumerable(stream, ctx, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Exception> GetErrorAsync<TException>(Stream stream, JsonTypeInfo<TException> ctx, CancellationToken cancellationToken) 
        where TException: Exception =>
        await JsonSerializer.DeserializeAsync(stream, ctx, cancellationToken).ConfigureAwait(false) ?? new Exception("Неизвестная ошибка");
}
