using Correctum.ApiClients.OfdRuTokenClient.Models;
using Correctum.Models;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Correctum.ApiClients.OfdRuTokenClient;

public sealed class OfdRuTokenClient(string host = "https://ofd.ru/") : BaseApiClient(host)
{
    private const string TemplateKkts = "/api/integration/v2/kkts?AuthToken={0}";
    private const string TemplateReceipts = "/api/integration/v2/inn/{0}/kkt/{1}/receipts-info?dateFrom={2}&dateTo={3}&AuthToken={4}";

    public async IAsyncEnumerable<OfdRuTokenKkt> EnumerateKktsAsync(string authToken, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(m_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(authToken);
        var ctx = OfdRuTokenJsonContext.Default.OfdRuTokenResponseOfdRuTokenKkt;
        var url = string.Format(TemplateKkts, authToken);
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        using var resp = await Client.SendAsync(req, cancellationToken).ConfigureAwait(false);
        await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var json = await JsonSerializer.DeserializeAsync(stream, ctx, cancellationToken).ConfigureAwait(false);
        json.ThrowIfFailed();
        foreach(var item in json.Data) yield return item;
    }

    public async IAsyncEnumerable<Receipt> EnumerateReceiptAsync(OfdRuTokenKkt kkt, string authToken, 
        DateTime fromDate, DateTime toDate, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
        var strFromDate = fromDate.ToString(DateTimeFormat);
        var strToDate = toDate.ToString(DateTimeFormat);
        ObjectDisposedException.ThrowIf(m_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(authToken);
        var ctx = OfdRuTokenJsonContext.Default.OfdRuTokenResponseOfdRuTokenReceipt;
        var url = string.Format(TemplateReceipts, kkt.INN, kkt.KktRegId, strFromDate, strToDate, authToken);
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        using var resp = await Client.SendAsync(req, cancellationToken).ConfigureAwait(false);
        await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var json = await JsonSerializer.DeserializeAsync(stream, ctx, cancellationToken).ConfigureAwait(false);
        json.ThrowIfFailed();
        foreach (var item in json.Data) yield return item.ToReceipt();
    }
}