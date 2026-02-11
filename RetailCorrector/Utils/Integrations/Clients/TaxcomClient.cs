using System.Globalization;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using RetailCorrector.Data;
using RetailCorrector.Enums;

namespace RetailCorrector.Utils.Integrations.Clients;

/// <summary>API-клиент ОФД Такском-Касса</summary>
/// <seealso href="https://lk-ofd.taxcom.ru/ApiHelp/">Документация интегратора</seealso>
internal class TaxcomClient(string host = "https://api-lk-ofd.taxcom.ru") : APIClient
{
    private readonly HttpClient _http = new(Handler, false) { BaseAddress = new Uri(host) };

    private async Task<T> GetAsync<T>(Uri uri, CancellationToken cancellationToken)
    {
        using var response = await _http.GetAsync(uri, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new HttpRequestException(message, null, response.StatusCode);
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        return (await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }

    // todo 3.1. Аутентификация и авторизация 
    // todo 3.4.1. Список торговых точек (Номер страницы результатов; Кол-во результатов на странице)
    // todo 3.5.1. Список ККТ по торговой точке (ИД торговой точки)
    // todo 3.5.2. Список фискальных накопителей по ККТ (Регистрационный номер ККТ)
    
    /// <summary>Перечисление номер смен по дате</summary>
    /// <param name="fn">Номер фискального накопителя</param>
    /// <param name="date">Дата</param>
    /// <param name="pn">Номер страницы результатов</param>
    /// <param name="ps">Кол-во результатов на странице</param>
    /// <param name="cancellationToken">Токен, позволяющий отменить выполнение операции</param>
    /// <returns><see cref="T:System.Collections.Generic.IAsyncEnumerable`1" />, представляющий массив номеров смен.</returns>
    /// <exception cref="HttpRequestException">Вызывается, если статус-код является не успешным</exception>
    public IAsyncEnumerable<uint> FetchSessionIdsByDateAsync(string fn, DateOnly date, uint pn, uint ps,
        CancellationToken cancellationToken = default) =>
        FetchSessionIdsByPeriodAsync(fn, date, date, pn, ps, cancellationToken);

    /// <summary>Перечисление номер смен по периоду</summary>
    /// <param name="fn">Номер фискального накопителя</param>
    /// <param name="begin">Начало периода</param>
    /// <param name="end">Окончание периода</param>
    /// <param name="pn">Номер страницы результатов</param>
    /// <param name="ps">Кол-во результатов на странице</param>
    /// <param name="cancellationToken">Токен, позволяющий отменить выполнение операции</param>
    /// <returns><see cref="T:System.Collections.Generic.IAsyncEnumerable`1" />, представляющий массив номеров смен.</returns>
    /// <exception cref="HttpRequestException">Вызывается, если статус-код является не успешным</exception>
    public async IAsyncEnumerable<uint> FetchSessionIdsByPeriodAsync(string fn, DateOnly begin, DateOnly end, uint pn,
        uint ps, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var uri = new Uri(
            $"/API/v2/ShiftList?fn={fn}&begin={begin:yyyy-MM-dd}T00:00:00&end={end:yyyy-MM-dd}T23:59:59&pn={pn}&ps={ps}");
        var responseContent =
            await GetAsync<TaxcomResponse<TaxcomSession>>(uri, cancellationToken).ConfigureAwait(false);
        foreach (var document in responseContent.Items) yield return document.Number;
    }
    
    /// <summary>Перечисление номер ФД чеков по смене</summary>
    /// <param name="fn">Номер фискального накопителя</param>
    /// <param name="shift">Номер смены</param>
    /// <param name="pn">Номер страницы результатов</param>
    /// <param name="ps">Кол-во результатов на странице</param>
    /// <param name="cancellationToken">Токен, позволяющий отменить выполнение операции</param>
    /// <returns><see cref="T:System.Collections.Generic.IAsyncEnumerable`1" />, представляющий массив номеров ФД.</returns>
    /// <exception cref="HttpRequestException">Вызывается, если статус-код является не успешным</exception>
    public async IAsyncEnumerable<uint> FetchDocumentNumbersBySessionAsync(string fn, uint shift, uint pn, uint ps,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var uri = new Uri($"/API/v2/DocumentList?fn={fn}&shift={shift}&pn={pn}&ps={ps}&type=3&type=31");
        var responseContent =
            await GetAsync<TaxcomResponse<TaxcomDocument>>(uri, cancellationToken).ConfigureAwait(false);
        foreach (var document in responseContent.Items) yield return document.Number;
    }

    /// <summary>Получение чека по реквизитам</summary>
    /// <param name="fn">Номер фискального накопителя</param>
    /// <param name="fd">Номер фискального документа</param>
    /// <param name="cancellationToken">Токен, позволяющий отменить выполнение операции</param>
    /// <returns>Объект задачи, представляющий асинхронную операцию.</returns>
    /// <exception cref="HttpRequestException">Вызывается, если статус-код является не успешным</exception>
    public async Task<Receipt?> FetchReceiptAsync(string fn, uint fd, CancellationToken cancellationToken = default)
    {
        var uri = new Uri($"/API/v2/DocumentInfo?fn={fn}&fd={fd}");
        var document = await GetAsync<TaxcomDocumentResponse>(uri, cancellationToken).ConfigureAwait(false);
        if (!document.DocType.Contains('3')) return null;
        var receipt = new Receipt
        {
            BuyerContact = document.Details["1008"]?.GetValue<string>() ?? "",
            ParseId = Guid.Empty,
            StorageId = document.Details["1041"]?.GetValue<string>().Trim('\0', ' ', '\t') ?? 0.ToString("D16"),
            RegId = document.Details["1037"]?.GetValue<string>().Trim('\0', ' ', '\t') ?? 0.ToString("D16"),
            DocNumber = document.Details["1040"]?.GetValue<uint>() ?? 0,
            SessionId = document.Details["1038"]?.GetValue<uint>() ?? 0,
            DocInSession = document.Details["1042"]?.GetValue<uint>() ?? 0,
            Items = new Position[document.Details["1059"]?.AsArray().Count ?? 0],
            Type = (Operation?)document.Details["1054"]?.GetValue<int>() ?? 0,
            DocSign = FiscalHelper.ToDecimalSign(document.Details["1077"]?.GetValue<string>()),
            Payment = new Payment
            {
                Cash = document.Details["1031"]?.GetValue<ulong>() ?? 0,
                ECash = document.Details["1081"]?.GetValue<ulong>() ?? 0,
                Pre = document.Details["1215"]?.GetValue<ulong>() ?? 0,
                Post = document.Details["1216"]?.GetValue<ulong>() ?? 0,
                Provision = document.Details["1217"]?.GetValue<ulong>() ?? 0,
            },
            TotalSum = document.Details["1020"]?.GetValue<ulong>() ?? 0,
            TaxationSystem = (TaxationSystem?)document.Details["1020"]?.AsArray().Sum
                (node => node?.GetValue<int>() ?? 0) ?? 0
        };
        if (document.Details["1012"]?.GetValue<string>() is { Length: 19 } rawDate)
            receipt.Created = DateTime.ParseExact(rawDate, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, 0);
        for (var i = 0; i < receipt.Items.Length; i++)
        {
            var position = document.Details["1008"]?[i];
            if (position is null) continue;
            receipt.Items[i] = new Position
            {
                Name = position["1030"]?.GetValue<string>() ?? "",
                Quantity = (ulong)Math.Round(double.Parse(position["1023"]?.GetValue<string>() ?? "0") * 1000),
                Price = position["1079"]?.GetValue<ulong>() ?? 0,
                Cost = position["1043"]?.GetValue<ulong>() ?? 0,
                Type = (PositionType?)position["1212"]?.GetValue<int>() ?? 0,
                PayMethod = (PayMethod?)position["1214"]?.GetValue<int>() ?? 0,
                TaxRate = (TaxRate?)position["1199"]?.GetValue<int>() ?? 0,
                Unit = (PositionUnit?)position["2108"]?.GetValue<int>()
            };
        }

        return receipt;
    }

    ~TaxcomClient() => _http.Dispose();
}

file struct TaxcomResponse<T>
{
    [JsonPropertyName("records")]
    public T[] Items { get; init; }
}

file struct TaxcomDocument
{
    [JsonPropertyName("fdNumber")]
    public uint Number { get; init; }
}

file struct TaxcomSession
{
    [JsonPropertyName("shiftNumber")]
    public uint Number { get; init; }
}

file struct TaxcomDocumentResponse
{
    [JsonPropertyName("documentType")]
    public string DocType { get; init; }
    
    [JsonPropertyName("document")]
    public JsonNode Details { get; init; }
}
