using Correctum.ApiClients.OfdRuTokenClient.Models;
using Correctum.Models;

namespace Correctum.ApiClients.OfdRuTokenClient;

public sealed class OfdRuTokenClient(string host = "https://ofd.ru/") : BaseApiClient(host)
{
    public async Task<OfdRuTokenKkt[]> GetKktsAsync
        (string authToken, CancellationToken cancellationToken = default)
    {
        var address = $"/api/integration/v2/kkts?AuthToken={authToken}";
        var ctxException = OfdRuTokenJsonContext.Default.Exception;
        var ctxResponse = OfdRuTokenJsonContext.Default.OfdRuTokenResponseOfdRuTokenKkt;
        return (await GetAsync(address, ctxException, ctxResponse, cancellationToken)).Data;
    }

    public async Task<Receipt[]> GetReceiptsAsync(OfdRuTokenKkt kkt, string authToken, 
        DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
        var textFromDate = fromDate.ToString(DateTimeFormat);
        var textToDate = toDate.ToString(DateTimeFormat);
        var address = $"/api/integration/v2/inn/{kkt.INN}/kkt/{kkt.KktRegId}/receipts-info?" +
            $"dateFrom={textFromDate}&dateTo={textToDate}&AuthToken={authToken}";
        var ctxException = OfdRuTokenJsonContext.Default.Exception;
        var ctxResponse = OfdRuTokenJsonContext.Default.OfdRuTokenResponseOfdRuTokenReceipt;
        var resp = await GetAsync(address, ctxException, ctxResponse, cancellationToken);
        return [.. resp.Data.Select(x => x.ToReceipt())];
    }
}