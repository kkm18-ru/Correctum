using Correctum.Configurations;
using Correctum.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;

namespace Correctum.ApiClients.FirstOfdClient;

public sealed class FirstOfdClient(string host = "https://org.1-ofd.ru") : BaseApiClient(host, true)
{
    public async Task AuthAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        const string uri = "/api/cp-core/user/login";
        if (Cookies["PLAY_SEESION"] is { } sessionToken) Cookies.Remove(sessionToken);
        var reqContent = new FirstOfdAuthRequest(login, password);
        using var resp = await Client.PostAsJsonAsync(uri, reqContent, JsonAuthData, cancellationToken).ConfigureAwait(false);
        if (!resp.IsSuccessStatusCode) 
            throw await resp.Content.ReadFromJsonAsync(JsonException, cancellationToken).ConfigureAwait(false) ?? new Exception("Неизвестная ошибка");
        var data = await resp.Content.ReadFromJsonAsync(JsonAuthToken, cancellationToken).ConfigureAwait(false);
        Cookies.Add(new Cookie("PLAY_SEESION", data.Token));
    }

    public async IAsyncEnumerable<FirstOfdPlace> EnumeratePlacesAsync(CancellationToken cancellationToken = default)
    {
        const string uri = "/api/cp-ofd/retail-places/without-group";
        using var resp = await Client.GetAsync(uri, cancellationToken).ConfigureAwait(false);
        if (!resp.IsSuccessStatusCode) 
            throw await resp.Content.ReadFromJsonAsync(JsonException, cancellationToken).ConfigureAwait(false) ?? new Exception("Неизвестная ошибка");
        await foreach (var place in resp.Content.ReadFromJsonAsAsyncEnumerable(JsonPlace, cancellationToken).ConfigureAwait(false))
            yield return place;
    }

    public async IAsyncEnumerable<FirstOfdKkt> EnumerateKktAsync(FirstOfdPlace place, CancellationToken cancellationToken = default)
    {
        var uri = $"/api/cp-ofd/retail-places/filter-retail-place/{place.Id}";
        using var resp = await Client.PostAsJsonAsync(uri, new(), JsonKktRequest, cancellationToken).ConfigureAwait(false);
        if (!resp.IsSuccessStatusCode)
            throw await resp.Content.ReadFromJsonAsync(JsonException, cancellationToken).ConfigureAwait(false) ?? new Exception("Неизвестная ошибка");
        await foreach (var kkt in resp.Content.ReadFromJsonAsAsyncEnumerable(JsonKkt, cancellationToken).ConfigureAwait(false))
            yield return kkt;
    }
    
    public async IAsyncEnumerable<string> EnumerateDriveAsync(FirstOfdKkt kkt, CancellationToken cancellationToken = default)
    {
        var uri = $"/api/cp-ofd/kkms/{kkt.Id}/fiscal-drives";
        using var resp = await Client.GetAsync(uri, cancellationToken).ConfigureAwait(false);
        if (!resp.IsSuccessStatusCode)
            throw await resp.Content.ReadFromJsonAsync(JsonException, cancellationToken).ConfigureAwait(false) ?? new Exception("Неизвестная ошибка");
        await foreach (var drive in resp.Content.ReadFromJsonAsAsyncEnumerable(JsonDrive, cancellationToken).ConfigureAwait(false))
            yield return drive.SerialNumber;

    }
    
    public async IAsyncEnumerable<FirstOfdTransaction> EnumerateTransactionsAsync
        (int kktId, string fn, DateTime date, CancellationToken cancellationToken = default)
    {
        var fromDate = new DateTimeOffset(date.Date, TimeSpan.Zero).ToUnixTimeMilliseconds();
        var toDate = fromDate + ((23 * 60 + 59) * 60 + 59) * 1000 + 999;
        var numberTransactions = await GetCountTransactions(kktId, fn, fromDate, toDate, cancellationToken);
        var pageSize = Configuration.Current.Importers.FirstOfdTransactionSize;
        var countPage = Math.Ceiling((double)numberTransactions / pageSize);
        for (var page = 1; page <= countPage; page++)
        {
            var uri = $"/api/cp-ofd/kkms/{kktId}/transactions?" +
                $"fromDate={fromDate}&toDate={toDate}&fiscalDriveNumber={fn}&" +
                $"page={page}&pageSize={pageSize}";
            using var resp = await Client.GetAsync(uri, cancellationToken).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
                throw await resp.Content.ReadFromJsonAsync(JsonException, cancellationToken).ConfigureAwait(false) ?? new Exception("Неизвестная ошибка");
            var transactions = await resp.Content.ReadFromJsonAsync(JsonTransactions, cancellationToken).ConfigureAwait(false);
            foreach (var transaction in transactions.Transactions) yield return transaction;
        }
    }

    private async Task<int> GetCountTransactions
        (int kktId, string fn, long fromDate, long toDate, CancellationToken cancellationToken)
    {
        var uri = $"/api/cp-ofd/kkms/{kktId}/transactions?pageSize=0&" +
            $"fromDate={fromDate}&toDate={toDate}&fiscalDriveNumber={fn}";
        using var resp = await Client.GetAsync(uri, cancellationToken).ConfigureAwait(false);
        if (!resp.IsSuccessStatusCode)
            throw await resp.Content.ReadFromJsonAsync(JsonException, cancellationToken).ConfigureAwait(false) ?? new Exception("Неизвестная ошибка");
        var transactions = await resp.Content.ReadFromJsonAsync(JsonTransactions, cancellationToken).ConfigureAwait(false);
        return transactions.Pagination.Total;
    }
    
    public async Task<Receipt?> FetchReceiptAsync(FirstOfdTransaction tr, CancellationToken cancellationToken = default)
    {
        if (tr.Type != "TICKET") return null;
        var uri = $"/api/cp-ofd/ticket/{tr.Id}";
        using var resp = await Client.GetAsync(uri, cancellationToken).ConfigureAwait(false);
        if (!resp.IsSuccessStatusCode)
            throw await resp.Content.ReadFromJsonAsync(JsonException, cancellationToken).ConfigureAwait(false) ?? new Exception("Неизвестная ошибка");
        var ticket = await resp.Content.ReadFromJsonAsync(JsonTicket, cancellationToken).ConfigureAwait(false);
        return ticket.Ticket.ToReceipt();
    }

    private static JsonTypeInfo<FirstOfdTransactionResponse> JsonTransactions => FirstOfdJsonContext.Default.FirstOfdTransactionResponse;
    private static JsonTypeInfo<FirstOfdTicketResponse> JsonTicket => FirstOfdJsonContext.Default.FirstOfdTicketResponse;
    private static JsonTypeInfo<FirstOfdAuthResponse> JsonAuthToken => FirstOfdJsonContext.Default.FirstOfdAuthResponse;
    private static JsonTypeInfo<FirstOfdAuthRequest> JsonAuthData => FirstOfdJsonContext.Default.FirstOfdAuthRequest;
    private static JsonTypeInfo<FirstOfdKktRequest> JsonKktRequest => FirstOfdJsonContext.Default.FirstOfdKktRequest;
    private static JsonTypeInfo<FirstOfdException> JsonException => FirstOfdJsonContext.Default.FirstOfdException;
    private static JsonTypeInfo<FirstOfdDrive> JsonDrive => FirstOfdJsonContext.Default.FirstOfdDrive;
    private static JsonTypeInfo<FirstOfdPlace> JsonPlace => FirstOfdJsonContext.Default.FirstOfdPlace;
    private static JsonTypeInfo<FirstOfdKkt> JsonKkt => FirstOfdJsonContext.Default.FirstOfdKkt;
}
