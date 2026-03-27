using Correctum.ApiClients.KktOnlineClient.Enums;
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

    private Task<KktOnlineResponse> CheckAsync
        (string query, string factoryNumber, int modelCode, CancellationToken cancellationToken)
    {
        var address = $"lkip.html?query={query}&factory_number={factoryNumber}&model_code={modelCode:D4}";
        //var ctxException = KktOnlineJsonContext.Default.E todo
        var ctxResponse = KktOnlineJsonContext.Default.KktOnlineResponse;
        return GetAsync(address, null, ctxResponse, cancellationToken);
    }
}