using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Correctum.Exceptions;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Correctum.Utils.Integrations.Clients;

/// <summary>API-клиент ОФД Такском-Касса</summary>
/// <seealso href="https://lk-ofd.taxcom.ru/ApiHelp/">Документация интегратора</seealso>
public class TaxcomClient(string host) : ApiClient<TaxcomClient>(host)
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    
    private const string SessionHeaderName = "Session-Token";

    public TaxcomClient() : this("https://api-lk-ofd.taxcom.ru") { }

    #if DEBUG
    public static string IntegratorId
    {
        get => Configuration.Current.Import.Taxcom.IntegratorId;
        set => Configuration.Current.Import.Taxcom.IntegratorId = value;
    }
    public
#endif
    string SessionToken
    {
        get => Client.DefaultRequestHeaders.GetValues(SessionHeaderName).FirstOrDefault("");
        set
        {
            Client.DefaultRequestHeaders.Remove(SessionHeaderName);
            if (!string.IsNullOrWhiteSpace(value))
                Client.DefaultRequestHeaders.Add(SessionHeaderName, value);
        }
    }

    private async Task<T> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Logger.Trace("Отправка запроса: {Method} {Uri}", request.Method, request.RequestUri);
        using var response = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            Logger.Trace("Прочтение контента ответа в режиме текста");
            var message = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            Logger.Trace("Десериализация ошибки...");
            throw JsonSerializer.Deserialize<TaxcomError>(message) is { Message: not null } error ? error.ToException()
                : new HttpRequestException(message, null, response.StatusCode);
        }

        Logger.Trace("Прочтение контента ответа в режиме потока");
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        Logger.Trace("Десериализация контента...");
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
        ThrowIfDisposed();
        using var request = new HttpRequestMessage(HttpMethod.Post, "/API/v2/Login");
        request.Headers.Add("Integrator-ID", Configuration.Current.Import.Taxcom.IntegratorId);
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
        ThrowIfDisposed();
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
        ThrowIfDisposed();
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
        ThrowIfDisposed();
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
        (string fn, DateOnly begin, DateOnly end, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
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
        ThrowIfDisposed();
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
        (string fn, uint fd, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/API/v2/DocumentInfo?fn={fn}&fd={fd}");
        var document = await SendAsync<TaxcomDocumentResponse>(request, cancellationToken).ConfigureAwait(false);
        return document.ParseReceipt();
    }
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

    public Receipt? ParseReceipt()
    {
        if (DocType is not ("3" or "31")) return null;
        DateTime? dt = Details["1012"]?.GetValue<string>() is { Length: 19 } s ?
            DateTime.Parse(s, CultureInfo.InvariantCulture, 0) : null;


        return new Receipt
        {
            BuyerContact = Details["1008"]?.GetValue<string>(),
            StorageId = Details["1041"]?.GetValue<string>().Trim('\0', ' ', '\t'),
            RegId = Details["1037"]?.GetValue<string>().Trim('\0', ' ', '\t'),
            DocNumber = Details["1040"]?.GetValue<uint>(),
            SessionId = Details["1038"]?.GetValue<uint>(),
            DocInSession = Details["1042"]?.GetValue<uint>(),
            Items = Details["1059"]?.AsArray().Select(ParsePosition).Where(p => p is not null).Cast<Position>().ToList() ?? [],
            Type = (ReceiptType?)Details["1054"]?.GetValue<int>(),
            DocSign = FiscalSign.TryParse(Details["1077"]?.GetValue<string>(), out var sign) ? sign : null,
            CashSum = Details["1031"]?.GetValue<ulong>(),
            NonCashSum = Details["1081"]?.GetValue<ulong>(),
            AdvanceSum = Details["1215"]?.GetValue<ulong>(),
            CreditSum = Details["1216"]?.GetValue<ulong>(),
            ProvisionSum = Details["1217"]?.GetValue<ulong>(),
            TotalSum = Details["1020"]?.GetValue<ulong>(),
            TaxationSystem = (TaxationSystems?)Details["1055"]?.GetValue<int>(),
            Created = dt
        };
    }

    private static Position? ParsePosition(JsonNode? node)
    {
        if (node is null) return null;
        var nds = node["1199"]?.GetValue<int>();
        return new()
        {
            Name = node["1030"]?.GetValue<string>(),
            Quantity = decimal.Parse(node["1023"]?.GetValue<string>() ?? "0",
            NumberStyles.Float, CultureInfo.InvariantCulture),
            Price = node["1079"]?.GetValue<ulong>(),
            Cost = node["1043"]?.GetValue<ulong>(),
            Type = (PositionType?)node["1212"]?.GetValue<int>(),
            CalcMethod = (CalculationMethod?)node["1214"]?.GetValue<int>(),
            TaxRate = nds is null ? null : NDSRate.FromFns(nds),
            Unit = (UnitOfMeasure?)node["2108"]?.GetValue<int>()
        };
    }
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