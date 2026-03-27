using Correctum.ApiClients.FirstOfdClient.Models;
using Correctum.Configurations;
using Correctum.Models;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization.Metadata;

namespace Correctum.ApiClients.FirstOfdClient;

public sealed class FirstOfdClient(string host = "https://org.1-ofd.ru") : BaseApiClient(host)
{
    public Task AuthAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        const string address = "/api/cp-core/user/login";
        var content = new FirstOfdAuthRequest(login, password);
        var ctxException = FirstOfdJsonContext.Default.FirstOfdException;
        var ctxRequest = FirstOfdJsonContext.Default.FirstOfdAuthRequest;
        return PostAsync(address, content, ctxRequest, ctxException, cancellationToken);
    }

    public async Task<int[]> GetPlaceIdsAsync(CancellationToken cancellationToken = default)
    {
        const string address = "/api/cp-ofd/retail-places/without-group";
        var ctxException = FirstOfdJsonContext.Default.FirstOfdException;
        var ctxResponse = FirstOfdJsonContext.Default.FirstOfdPlaceArray;
        var places = await GetAsync(address, ctxException, ctxResponse, cancellationToken);
        return [.. places?.Select(x => x.Id) ?? []];
    }

    public async Task<FirstOfdKkt[]> GetKktsAsync(int placeId, CancellationToken cancellationToken = default)
    {
        var address = $"/api/cp-ofd/retail-places/filter-retail-place/{placeId}";
        var content = new FirstOfdKktSearchRequest();
        var ctxRequest = FirstOfdJsonContext.Default.FirstOfdKktSearchRequest;
        var ctxException = FirstOfdJsonContext.Default.FirstOfdException;
        var ctxResponse = FirstOfdJsonContext.Default.FirstOfdKktArray;
        var kkts = await PostAsync(address, content, ctxRequest, ctxException, ctxResponse, cancellationToken);
        return kkts ?? [];
    }

    public async Task<string[]> GetDriveAsync(FirstOfdKkt kkt, CancellationToken cancellationToken = default)
    {
        var address = $"/api/cp-ofd/kkms/{kkt.Id}/fiscal-drives";
        var ctxException = FirstOfdJsonContext.Default.FirstOfdException;
        var ctxResponse = FirstOfdJsonContext.Default.FirstOfdDriveArray;
        var drives = await GetAsync(address, ctxException, ctxResponse, cancellationToken);
        return [.. drives?.Select(x => x.SerialNumber) ?? []];
    }
        
    public async IAsyncEnumerable<string> GetTicketIdsAsync
        (int kktId, string fn, DateTime date, CancellationToken cancellationToken = default)
    {
        var pageSize = Configuration.Current.Importers.FirstOfdTransactionSize;
        var endDate = date.Date.AddDays(1);
        var trResponse = await GetTransactionResponseInternal(kktId, fn, date.Date, endDate, 1, pageSize, cancellationToken);
        var numPages = Math.Ceiling((double)trResponse.Pagination.Total / pageSize);
        foreach (var tr in trResponse.Transactions)
            if (tr.Type.Equals("TICKET", StringComparison.InvariantCultureIgnoreCase))
                yield return tr.Id;
        for (var page = 1; page <= numPages; page++)
        {
            trResponse = await GetTransactionResponseInternal(kktId, fn, date.Date, endDate, page, pageSize, cancellationToken);
            foreach (var tr in trResponse.Transactions)
                if (tr.Type.Equals("TICKET", StringComparison.InvariantCultureIgnoreCase))
                    yield return tr.Id;
        }
    }

    public async Task<Receipt?> GetReceiptAsync(string ticketId, CancellationToken cancellationToken = default)
    {
        var address = $"/api/cp-ofd/ticket/{ticketId}";
        var ctxException = FirstOfdJsonContext.Default.FirstOfdException;
        var ctxResponse = FirstOfdJsonContext.Default.FirstOfdTicketResponse;
        var ticket = await GetAsync(address, ctxException, ctxResponse, cancellationToken);
        return ticket.Ticket.ToReceipt();
    }


    private async Task<FirstOfdTransactionResponse> GetTransactionResponseInternal
        (int kktId, string fiscalDriveNumber, DateTime fromDate, DateTime toDate, int page, int pageSize, CancellationToken cancellationToken)
    {
        var unixEpoch = new DateTime(1, 1, 1970);
        var unixFromDate = (fromDate - unixEpoch).TotalMilliseconds;
        var unixToDate = (toDate - unixEpoch).TotalMilliseconds;
        var uri = $"/api/cp-ofd/kkms/{kktId}/transactions?" +
                $"fromDate={unixFromDate}&toDate={unixToDate}&pageSize={pageSize}&" +
                $"fiscalDriveNumber={fiscalDriveNumber}&page={page}";

        using var respMessage = await Client
            .GetAsync(uri, cancellationToken)
            .ConfigureAwait(false);
        FirstOfdException.ThrowIfError(respMessage);
        return await respMessage.Content
            .ReadFromJsonAsync(JsonTransactions, cancellationToken)
            .ConfigureAwait(false);
    }
}
