using RetailCorrector.Data;
using RetailCorrector.Enums;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RetailCorrector.Utils.Integrations.Clients;

/// <summary>Обертка API-клиента OfdRu.</summary>
internal sealed class OfdRuClient : APIClient
{
    private const string Host = "https://ofd.ru/api/integration/v2/";
    private readonly HttpClient _client = new(Handler, false) { BaseAddress = new Uri(Host) };
    private string _token = "";

    /// <summary>Установка аутентификационного токена.</summary>
    /// <param name="token">Аутентификационный токен API.</param>
    /// <param name="cancel">Токен, позволяющий отменить проверку токена.</param>
    /// <exception cref="ArgumentException">Вызывается, если токен не валиден.</exception>
    public async Task SetAuthTokenAsync(string token, CancellationToken cancel = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        using var req = new HttpRequestMessage(HttpMethod.Get, $"kktgroup/list?AuthToken={token}");
        using var resp = await _client.SendAsync(req, cancel).ConfigureAwait(false);
        if (!resp.IsSuccessStatusCode)
            throw new ArgumentException("Некорректный аутентификационный токен."); 
        _token = token;
    }

    /// <summary>Получение списка регистрационных номеров ККТ.</summary>
    /// <param name="cancel">Токен, позволяющий отменить получение данных о ККТ.</param>
    /// <returns>Список регистрационных номеров ККТ.</returns>
    public async Task<OfdRuDevice[]> FetchDevicesAsync(CancellationToken cancel = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, $"kkts?AuthToken={_token}");
        using var resp = await _client.SendAsync(req, cancel).ConfigureAwait(false);
        var stream = await resp.Content.ReadAsStreamAsync(cancel).ConfigureAwait(false);
        var json = await JsonSerializer.DeserializeAsync<Response<OfdRuDevice>>(stream, cancellationToken: cancel).ConfigureAwait(false);
        if (json.Status != 0) throw json.ToException();
        return json.Data;
    }

    /// <summary>Получение списка чеков.</summary>
    /// <param name="parseId">Идентификтор сессии парсера.</param>
    /// <param name="device">Контрольно-кассовая машина.</param>
    /// <param name="fromDate">Начальная дата периода для формирования отчета.</param>
    /// <param name="toDate">Конечная дата периода для формирования отчета.</param>
    /// <param name="cancel">Токен, позволяющий отменить получение данных о ККТ.</param>
    /// <returns>Список чеков.</returns>
    public async Task<Receipt[]> FetchReceiptsAsync
        (Guid parseId, OfdRuDevice device, DateTime fromDate, DateTime toDate, CancellationToken cancel = default)
    {
        var uri = $"inn/{device.INN}/kkt/{device.KktRegId}/receipts-info?AuthToken={_token}" +
            $"&dateFrom={fromDate:yyyy-MM-ddTHH:mm:ss}" +
            $"&dateTo={toDate:yyyy-MM-ddTHH:mm:ss}";
        using var req = new HttpRequestMessage(HttpMethod.Get, uri);
        using var resp = await _client.SendAsync(req, cancel).ConfigureAwait(false);
        var stream = await resp.Content.ReadAsStreamAsync(cancel).ConfigureAwait(false);
        var json = await JsonSerializer.DeserializeAsync<Response<Document>>(stream, cancellationToken: cancel).ConfigureAwait(false);
        if (json.Status != 0) throw json.ToException();
        return [.. json.Data.Select(d => d.ToReceipt(parseId))];
    }

    ~OfdRuClient()
    {
        _client.Dispose();
    }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
file enum ResponseStatus { Success, Failed }

file struct Response<T>
{
    public ResponseStatus Status { get; set; }
    public T[] Data { get; set; }
    public string[] Errors { get; set; }

    public Exception ToException()
    {
        var builder = new StringBuilder();
        builder.AppendLine("Запрос выполнился со следующими ошибками:");
        foreach (var error in Errors)
            builder.AppendLine($" - {error}");
        return new Exception(builder.ToString());
    }
}

public struct OfdRuDevice
{
    public string KktRegId { get; set; }
    public string INN { get; set; }
}

file struct Document
{
    public DateTime DocDateTime { get; set; }
    public string DecimalFiscalSign { get; set; }
    public ulong TotalSumm { get; set; }
    public string OperationType { get; set; }
    public TaxationSystem TaxationType { get; set; }
    public DocumentItem[] Items { get; set; }

    public ulong CashSumm { get; set; }
    public ulong ECashSumm { get; set; }
    public ulong PrepaidSumm { get; set; }
    public ulong CreditSumm { get; set; }
    public ulong ProvisionSumm { get; set; }

    public ulong Tax5Summ { get; set; }
    public ulong Tax7Summ { get; set; }
    public ulong Tax10Summ { get; set; }
    public ulong Tax18Summ { get; set; }
    public ulong Tax110Summ { get; set; }
    public ulong Tax105Summ { get; set; }
    public ulong Tax107Summ { get; set; }
    public ulong Tax118Summ { get; set; }
    public ulong TaxNaSumm { get; set; }
    public ulong Tax0Summ { get; set; }


    public Receipt ToReceipt(Guid ParseId) => new()
    {
        ParseId = ParseId,
        Created = DocDateTime,
        DocSign = ulong.Parse(DecimalFiscalSign),
        Type = OperationType.ToLowerInvariant() switch
        {
            "income" => Operation.Income,
            "outcome" => Operation.Outcome,
            "refund income" => Operation.RefundIncome,
            "refund outcome" => Operation.RefundOutcome,
            _ => throw new NotSupportedException($"Тип операции '{OperationType}' не поддерживается.")
        },
        Items = [.. Items.Select(i => i.ToPosition())],
        PaymentSums = [.. new PaymentSum[]
        {
            new(PaymentType.Cash, CashSumm),
            new(PaymentType.NonCash, ECashSumm),
            new(PaymentType.Advance, PrepaidSumm),
            new(PaymentType.Credit, CreditSumm),
            new(PaymentType.Other, ProvisionSumm)
        }.Where(p => p.Sum > 0)],
        TaxSums = [.. new TaxSum[] {
            new(TaxRate.Tax0, Tax0Summ),
            new(TaxRate.Tax5, Tax5Summ),
            new(TaxRate.Tax7, Tax7Summ),
            new(TaxRate.Tax10, Tax10Summ),
            new(TaxRate.Tax20, Tax18Summ),
            new(TaxRate.Without, TaxNaSumm),
            new(TaxRate.Calc5, Tax105Summ),
            new(TaxRate.Calc7, Tax107Summ),
            new(TaxRate.Calc10, Tax110Summ),
            new(TaxRate.Calc20, Tax118Summ),
        }.Where(t => t.Sum > 0)],
        TotalSum = TotalSumm,
        TaxationSystem = TaxationType
    };
}
file struct DocumentItem
{
    public string Name { get; set; }
    public ulong Price { get; set; }
    public double Quantity { get; set; }
    public ulong Total { get; set; }
    public PayMethod CalculationMethod { get; set; }
    public string? ProductUnitOfMeasure { get; set; }
    public TaxRate NDS_Rate { get; set; }
    public PositionType SubjectType { get; set; }

    public Position ToPosition()
    {
        MeasureUnit? mu = null;
        if (ProductUnitOfMeasure is not null)
            mu = (MeasureUnit)int.Parse(ProductUnitOfMeasure);

        return new Position
        {
            Name = Name,
            Price = Price,
            Quantity = (ulong)(Quantity * 1000),
            Cost = Total,
            PayMethod = CalculationMethod,
            MeasureUnit = mu,
            TaxRate = NDS_Rate,
            Type = SubjectType
        };
    }
}