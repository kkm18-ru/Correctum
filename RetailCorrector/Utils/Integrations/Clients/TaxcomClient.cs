using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using RetailCorrector.Data;
using RetailCorrector.Enums;
using RetailCorrector.Exceptions;

namespace RetailCorrector.Utils.Integrations.Clients;

/// <summary>API-клиент ОФД Такском-Касса</summary>
/// <seealso href="https://lk-ofd.taxcom.ru/ApiHelp/">Документация интегратора</seealso>
public class TaxcomClient(string host = "https://api-lk-ofd.taxcom.ru") : APIClient
{
    private readonly HttpClient _http = new(Handler, false) { BaseAddress = new Uri(host) };
    
    #if DEBUG
    private const string SessionHeaderName = "Session-Token";
    public string SessionToken
    {
        get => _http.DefaultRequestHeaders.GetValues(SessionHeaderName).FirstOrDefault("");
        set
        {
            _http.DefaultRequestHeaders.Remove(SessionHeaderName);
            if(!string.IsNullOrWhiteSpace(value))
                _http.DefaultRequestHeaders.Add(SessionHeaderName, value);
        }
    }

    public string IntegratorId
    {
        get => Configuration.Instance.Importers.TaxcomIntegratorId;
        set => Configuration.Instance.Importers.TaxcomIntegratorId = value;
    }
    #endif

    private async Task<T> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw JsonSerializer.Deserialize<TaxcomError>(message) is { Message: not null } error ? error.ToException()
                : new HttpRequestException(message, null, response.StatusCode);
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        return (await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken)
            .ConfigureAwait(false))!;
    }

    /// <summary>Аутентификация и авторизация по логину и паролю</summary>
    /// <param name="login">Электронная почта сотрудника</param>
    /// <param name="password">Пароль сотрудника</param>
    /// <param name="agreement">Номер договора</param>
    /// <param name="cancellationToken">Токен, позволяющий отменить выполнение операции</param>
    /// <returns>Объект задачи, представляющий асинхронную операцию.</returns>
    /// <exception cref="HttpRequestException">Вызывается, если статус-код является не успешным</exception>
    public async Task AuthAsync(string login, string password, string? agreement = null, 
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/API/v2/Login");
        request.Headers.Add("Integrator-ID", Configuration.Instance.Importers.TaxcomIntegratorId);
        request.Content = JsonContent.Create(new TaxcomAuthRequest(login, password, agreement));
        var response = await SendAsync<TaxcomAuthResponse>(request, cancellationToken).ConfigureAwait(false);
        SessionToken = response.Token;
    }
    
    /// <summary>Перечисление номеров торговых точек</summary>
    /// <param name="cancellationToken">Токен, позволяющий отменить выполнение операции</param>
    /// <returns><see cref="T:System.Collections.Generic.IAsyncEnumerable`1" />, представляющий массив номеров торговых точек.</returns>
    /// <exception cref="HttpRequestException">Вызывается, если статус-код является не успешным</exception>
    public async IAsyncEnumerable<string> FetchOutletIdsAsync
        ([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        for (var i = 1U; i < uint.MaxValue; i++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"/API/v2/OutletList?&pn={i}&ps=100");
            var response = await SendAsync<TaxcomResponse>(request, cancellationToken).ConfigureAwait(false);
            if(response.Items.Length == 0) break;
            foreach (var outlet in response.Items) yield return outlet.Outlet;
        }
    }

    /// <summary>Перечисление ККТ по торговой точке</summary>
    /// <param name="id">ИД торговой точки</param>
    /// <param name="cancellationToken">Токен, позволяющий отменить выполнение операции</param>
    /// <returns><see cref="T:System.Collections.Generic.IAsyncEnumerable`1" />, представляющий массив ККТ.</returns>
    /// <exception cref="HttpRequestException">Вызывается, если статус-код является не успешным</exception>
    public async IAsyncEnumerable<string> FetchDevicesAsync
        (string id, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        for (var i = 1U; i < uint.MaxValue; i++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"/API/v2/KKTList?id={id}&pn={i}&ps=100");
            var response = await SendAsync<TaxcomResponse>(request, cancellationToken).ConfigureAwait(false);
            if(response.Items.Length == 0) break;
            foreach (var device in response.Items) yield return device.Device;
        }
    }

    /// <summary>Перечисление номеров ФН по ККТ</summary>
    /// <param name="kktRegNumber">Регистрационный номер ККТ</param>
    /// <param name="cancellationToken">Токен, позволяющий отменить выполнение операции</param>
    /// <returns><see cref="T:System.Collections.Generic.IAsyncEnumerable`1" />, представляющий массив номеров ФН.</returns>
    /// <exception cref="HttpRequestException">Вызывается, если статус-код является не успешным</exception>
    public async IAsyncEnumerable<string> FetchDriveIdsAsync
        (string kktRegNumber, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, 
            $"/API/v2/FnHistory?kktRegNumber={kktRegNumber}");
        var response = await SendAsync<TaxcomResponse>(request, cancellationToken).ConfigureAwait(false);
        foreach (var drive in response.Items) yield return drive.Drive;
    }

    /// <summary>Перечисление номеров смен по периоду</summary>
    /// <param name="fn">Номер фискального накопителя</param>
    /// <param name="begin">Начало периода</param>
    /// <param name="end">Окончание периода</param>
    /// <param name="cancellationToken">Токен, позволяющий отменить выполнение операции</param>
    /// <returns>Объект задачи, представляющий асинхронную операцию.</returns>
    /// <exception cref="HttpRequestException">Вызывается, если статус-код является не успешным</exception>
    public async Task<List<uint>> FetchSessionIdsByPeriodAsync
        (string fn, DateTime begin, DateTime end, CancellationToken cancellationToken = default)
    {
        var textBegin = $"begin={begin:yyyy-MM-dd}T00:00:00";
        var textEnd = $"end={end:yyyy-MM-dd}T23:59:59";
        var sessionIds = new List<uint>();
        for (var i = 1U; i < uint.MaxValue; i++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, 
                $"/API/v2/ShiftList?fn={fn}&{textBegin}&{textEnd}&pn={i}&ps=100");
            var response = await SendAsync<TaxcomResponse>(request, cancellationToken).ConfigureAwait(false);
            if(response.Items.Length == 0) break;
            sessionIds.AddRange(response.Items.Select(number => number.Session.GetUInt32()));
        }

        return sessionIds;
    }

    /// <summary>Перечисление номеров ФД чеков по смене</summary>
    /// <param name="fn">Номер фискального накопителя</param>
    /// <param name="shift">Номер смены</param>
    /// <param name="cancellationToken">Токен, позволяющий отменить выполнение операции</param>
    /// <returns>Объект задачи, представляющий асинхронную операцию.</returns>
    /// <exception cref="HttpRequestException">Вызывается, если статус-код является не успешным</exception>
    public async Task<List<uint>> FetchDocumentNumbersBySessionAsync
        (string fn, uint shift, CancellationToken cancellationToken = default)
    {
        var documentIds = new List<uint>();
        for (var i = 1U; i < uint.MaxValue; i++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"/API/v2/DocumentList?fn={fn}&shift={shift}&pn={i}&ps=100");
            var response = await SendAsync<TaxcomResponse>(request, cancellationToken).ConfigureAwait(false);
            if(response.Items.Length == 0) break;
            documentIds.AddRange(response.Items.Select(number => number.Document));
        }
        documentIds.Sort();
        return documentIds;
    }

    /// <summary>Получение чека по реквизитам</summary>
    /// <param name="fn">Номер фискального накопителя</param>
    /// <param name="fd">Номер фискального документа</param>
    /// <param name="importId">Идентификатор сессии импорта</param>
    /// <param name="cancellationToken">Токен, позволяющий отменить выполнение операции</param>
    /// <returns>Объект задачи, представляющий асинхронную операцию.</returns>
    /// <exception cref="HttpRequestException">Вызывается, если статус-код является не успешным</exception>
    public async Task<Receipt?> FetchReceiptAsync
        (string fn, uint fd, Guid importId, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/API/v2/DocumentInfo?fn={fn}&fd={fd}");
        var document = await SendAsync<TaxcomDocumentResponse>(request, cancellationToken).ConfigureAwait(false);
        return document.ParseReceipt(importId);
    }

    ~TaxcomClient() => _http.Dispose();
}

file readonly struct TaxcomResponse
{
    [JsonPropertyName("records")] public TaxcomNumber[] Items { get; init; }
}

file struct TaxcomNumber
{
    [JsonPropertyName("id")] public string Outlet { get; init; }
    [JsonPropertyName("kktRegNumber")] public string Device { get; init; }
    [JsonPropertyName("fn")] public string Drive { get; init; }
    [JsonPropertyName("shiftNumber")] public JsonElement Session { get; init; }
    [JsonPropertyName("fdNumber")] public uint Document { get; init; }
}

file readonly struct TaxcomDocumentResponse
{
    [JsonPropertyName("documentType")] public string DocType { get; init; }
    [JsonPropertyName("document")] public JsonNode Details { get; init; }

    public Receipt? ParseReceipt(Guid importId)
    {
        if (DocType is not ("3" or "31")) return null;
        var receipt = new Receipt
        {
            BuyerContact = Details["1008"]?.GetValue<string>() ?? "",
            ParseId = importId,
            StorageId = Details["1041"]?.GetValue<string>().Trim('\0', ' ', '\t') ?? 0.ToString("D16"),
            RegId = Details["1037"]?.GetValue<string>().Trim('\0', ' ', '\t') ?? 0.ToString("D16"),
            DocNumber = Details["1040"]?.GetValue<uint>() ?? 0,
            SessionId = Details["1038"]?.GetValue<uint>() ?? 0,
            DocInSession = Details["1042"]?.GetValue<uint>() ?? 0,
            Items = new Position[Details["1059"]?.AsArray().Count ?? 0],
            Type = (Operation?)Details["1054"]?.GetValue<int>() ?? 0,
            DocSign = FiscalHelper.ToDecimalSign(Details["1077"]?.GetValue<string>()),
            Payment = ParsePayment(),
            TotalSum = Details["1020"]?.GetValue<ulong>() ?? 0,
            TaxationSystem = (TaxationSystem?)Details["1055"]?.GetValue<int>()?? 0
        };
        if (Details["1012"]?.GetValue<string>() is { Length: 19 } s)
            receipt.Created = DateTime.Parse(s, CultureInfo.InvariantCulture, 0);
        for (var i = 0; i < receipt.Items.Length; i++) receipt.Items[i] = ParsePosition(i);
        return receipt;
    }

    private Position ParsePosition(int index) => new()
    {
        Name = Details["1059"]?[index]?["1030"]?.GetValue<string>() ?? "",
        Quantity = (ulong)Math.Round(double.Parse(Details["1059"]?[index]?["1023"]?.GetValue<string>() ?? "0",
            NumberStyles.Float, CultureInfo.InvariantCulture) * 1000),
        Price = Details["1059"]?[index]?["1079"]?.GetValue<ulong>() ?? 0,
        Cost = Details["1059"]?[index]?["1043"]?.GetValue<ulong>() ?? 0,
        Type = (PositionType?)Details["1059"]?[index]?["1212"]?.GetValue<int>() ?? 0,
        PayMethod = (PayMethod?)Details["1059"]?[index]?["1214"]?.GetValue<int>() ?? 0,
        TaxRate = (TaxRate?)Details["1059"]?[index]?["1199"]?.GetValue<int>() ?? 0,
        Unit = (PositionUnit?)Details["1059"]?[index]?["2108"]?.GetValue<int>()
    };

    private Payment ParsePayment() => new()
    {
        Cash = Details["1031"]?.GetValue<ulong>() ?? 0,
        ECash = Details["1081"]?.GetValue<ulong>() ?? 0,
        Pre = Details["1215"]?.GetValue<ulong>() ?? 0,
        Post = Details["1216"]?.GetValue<ulong>() ?? 0,
        Provision = Details["1217"]?.GetValue<ulong>() ?? 0,
    };
}

file readonly struct TaxcomAuthRequest(string login, string password, string? agreement = null)
{
    [JsonPropertyName("login")] public string Login { get; init; } = login;
    [JsonPropertyName("password")] public string Password { get; init; } = password;
    [JsonPropertyName("agreementNumber")] public string? Agreement { get; init; } = agreement;
}

file struct TaxcomAuthResponse
{
    [JsonPropertyName("sessionToken")] public string Token { get; init; }
}

file struct TaxcomError
{
    [JsonPropertyName("details")] public string? Message { get; init; }
    [JsonPropertyName("apiErrorCode")] public ushort ApiCode { get; init; }
    [JsonPropertyName("commonDescription")] public string Summary { get; init; }
    [JsonPropertyName("httpErrorCode")] public ushort StatusCode { get; init; }

    public TaxcomException ToException() => new(Message, ApiCode, Summary, StatusCode);
}