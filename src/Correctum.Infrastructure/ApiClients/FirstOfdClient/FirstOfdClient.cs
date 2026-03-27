using Correctum.ApiClients.FirstOfdClient.Models;
using Correctum.Configurations;
using Correctum.Models;
using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;

namespace Correctum.ApiClients.FirstOfdClient;

public sealed class FirstOfdClient(string host = "https://org.1-ofd.ru") : BaseApiClient(host, true)
{
    public Task AuthAsync(string login, string password, CancellationToken cancellationToken = default) =>
        AuthInternal(new FirstOfdAuthRequest(login, password), cancellationToken);

    public async Task<int[]> GetPlaceIdsAsync(CancellationToken cancellationToken = default) =>
        [.. (await GetPlacesInternal(cancellationToken)).Select(x => x.Id)];

    public Task<FirstOfdKkt[]> GetKktsAsync(int placeId, CancellationToken cancellationToken = default) =>
        GetKktsInternal(placeId, cancellationToken);

    public async Task<string[]> GetDriveAsync(FirstOfdKkt kkt, CancellationToken cancellationToken = default) =>
        [.. (await GetDrivesInternal(kkt.Id, cancellationToken)).Select(x => x.SerialNumber)];
        
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

    public async Task<Receipt?> GetReceiptAsync(string trId, CancellationToken cancellationToken = default) =>
        (await GetTicketInternal(trId, cancellationToken)).Ticket.ToReceipt();


    private async Task AuthInternal(FirstOfdAuthRequest request, CancellationToken cancellationToken)
    {
        const string uri = "/api/cp-core/user/login";
        using var respMessage = await Client
            .PostAsJsonAsync(uri, request, JsonAuthData, cancellationToken)
            .ConfigureAwait(false);
        FirstOfdException.ThrowIfError(respMessage);
    }

    private async Task<FirstOfdPlace[]> GetPlacesInternal(CancellationToken cancellationToken)
    {
        const string uri = "/api/cp-ofd/retail-places/without-group";
        using var respMessage = await Client
            .GetAsync(uri, cancellationToken)
            .ConfigureAwait(false);
        FirstOfdException.ThrowIfError(respMessage);
        return await respMessage.Content
            .ReadFromJsonAsync(JsonPlaces, cancellationToken)
            .ConfigureAwait(false) ?? [];
    }

    private async Task<FirstOfdKkt[]> GetKktsInternal(int placeId, CancellationToken cancellationToken)
    {
        var uri = $"/api/cp-ofd/retail-places/filter-retail-place/{placeId}";
        using var respMessage = await Client
            .GetAsync(uri, cancellationToken)
            .ConfigureAwait(false);
        FirstOfdException.ThrowIfError(respMessage);
        return await respMessage.Content
            .ReadFromJsonAsync(JsonKkts, cancellationToken)
            .ConfigureAwait(false) ?? [];
    }

    private async Task<FirstOfdDrive[]> GetDrivesInternal(int kktId, CancellationToken cancellationToken)
    {
        var uri = $"/api/cp-ofd/kkms/{kktId}/fiscal-drives";
        using var respMessage = await Client
            .GetAsync(uri, cancellationToken)
            .ConfigureAwait(false);
        FirstOfdException.ThrowIfError(respMessage);
        return await respMessage.Content
            .ReadFromJsonAsync(JsonDrives, cancellationToken)
            .ConfigureAwait(false) ?? [];
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
    
    private async Task<FirstOfdTicketResponse> GetTicketInternal(string trId, CancellationToken cancellationToken)
    {
        var uri = $"/api/cp-ofd/ticket/{trId}";
        using var respMessage = await Client
            .GetAsync(uri, cancellationToken)
            .ConfigureAwait(false);
        FirstOfdException.ThrowIfError(respMessage);
        return await respMessage.Content
            .ReadFromJsonAsync(JsonTicket, cancellationToken)
            .ConfigureAwait(false);
    }


    private static JsonTypeInfo<FirstOfdTransactionResponse> JsonTransactions => FirstOfdJsonContext.Default.FirstOfdTransactionResponse;
    private static JsonTypeInfo<FirstOfdKktSearchRequest> JsonKktRequest => FirstOfdJsonContext.Default.FirstOfdKktSearchRequest;
    private static JsonTypeInfo<FirstOfdTicketResponse> JsonTicket => FirstOfdJsonContext.Default.FirstOfdTicketResponse;
    private static JsonTypeInfo<FirstOfdAuthResponse> JsonAuthToken => FirstOfdJsonContext.Default.FirstOfdAuthResponse;
    private static JsonTypeInfo<FirstOfdAuthRequest> JsonAuthData => FirstOfdJsonContext.Default.FirstOfdAuthRequest;
    private static JsonTypeInfo<FirstOfdDrive[]> JsonDrives => FirstOfdJsonContext.Default.FirstOfdDriveArray;
    private static JsonTypeInfo<FirstOfdPlace[]> JsonPlaces => FirstOfdJsonContext.Default.FirstOfdPlaceArray;
    private static JsonTypeInfo<FirstOfdKkt[]> JsonKkts => FirstOfdJsonContext.Default.FirstOfdKktArray;
}
