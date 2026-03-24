using System.Text.Json;
using Correctum.Enums;

namespace Correctum.ApiClients.KktOnlineClient;

public sealed class KktOnlineClient(string host = "https://kkt-online.nalog.ru/") : BaseApiClient(host)
{
    public async Task<KktFnStatusCode> CheckKktModelAsync
        (string serial, KktOnlineKktModel model, CancellationToken cancellationToken =default) =>
        (await CheckAsync("/kkt/model/check", serial, (int)model, cancellationToken)).GetStatusCode();
    
    public async Task<KktFnStatusCode> CheckFnModelAsync
        (string serial, KktOnlineFnModel model, CancellationToken cancellationToken = default) =>
        (await CheckAsync("/fn/model/check", serial, (int)model, cancellationToken)).GetStatusCode();

    private async Task<KktOnlineResponse> CheckAsync
        (string query, string factoryNumber, int modelCode, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(m_disposed, this);
        var uri = $"lkip.html?query={query}&factory_number={factoryNumber}&model_code={modelCode:D4}";
        using var req = new HttpRequestMessage(HttpMethod.Get, uri);
        using var resp = await Client.SendAsync(req, cancellationToken).ConfigureAwait(false);
        await using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var ctx = KktOnlineJsonContext.Default.KktOnlineResponse;
        return await JsonSerializer.DeserializeAsync(stream, ctx, cancellationToken).ConfigureAwait(false);
    }
}