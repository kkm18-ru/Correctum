using RetailCorrector.Data;
using RetailCorrector.Enums;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RetailCorrector.Utils.Integrations.Clients;

/// <summary>Обертка API-клиента OfdRu.</summary>
internal sealed class OfdRuClient() : ApiClient<OfdRuClient>(Host)
{
    private const string Host = "https://ofd.ru/api/integration/v2/";
    private string _token = "";

    /// <summary>Установка аутентификационного токена.</summary>
    /// <param name="token">Аутентификационный токен API.</param>
    /// <param name="cancel">Токен, позволяющий отменить проверку токена.</param>
    /// <exception cref="ArgumentException">Вызывается, если токен не валиден.</exception>
    public async Task SetAuthTokenAsync(string token, CancellationToken cancel = default)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        using var req = new HttpRequestMessage(HttpMethod.Get, $"kktgroup/list?AuthToken={token}");
        using var resp = await Client.SendAsync(req, cancel).ConfigureAwait(false);
        if (!resp.IsSuccessStatusCode)
            throw new ArgumentException("Некорректный аутентификационный токен."); 
        _token = token;
    }

    /// <summary>Получение списка регистрационных номеров ККТ.</summary>
    /// <param name="cancel">Токен, позволяющий отменить получение данных о ККТ.</param>
    /// <returns>Список регистрационных номеров ККТ.</returns>
    public async Task<OfdRuDevice[]> FetchDevicesAsync(CancellationToken cancel = default)
    {
        ThrowIfDisposed();
        using var req = new HttpRequestMessage(HttpMethod.Get, $"kkts?AuthToken={_token}");
        using var resp = await Client.SendAsync(req, cancel).ConfigureAwait(false);
        var stream = await resp.Content.ReadAsStreamAsync(cancel).ConfigureAwait(false);
        var json = await JsonSerializer.DeserializeAsync<Response<OfdRuDevice>>(stream, cancellationToken: cancel).ConfigureAwait(false);
        if (json.Status != 0) throw json.ToException();
        return json.Data;
    }

    /// <summary>Получение списка чеков.</summary>
    /// <param name="parseId">Идентификтор сессии парсера.</param>
    /// <param name="device">Контрольно-кассовая машина.</param>
    /// <param name="date">Дата для формирования отчета.</param>
    /// <param name="cancel">Токен, позволяющий отменить получение данных о ККТ.</param>
    /// <returns>Список чеков.</returns>
    public async Task<Receipt[]> FetchReceiptsAsync
        (Guid parseId, OfdRuDevice device, DateOnly date, CancellationToken cancel = default)
    {
        ThrowIfDisposed();
        var uri = $"inn/{device.INN}/kkt/{device.KktRegId}/receipts-info?AuthToken={_token}" +
            $"&dateFrom={date:yyyy-MM-dd}T00:00:00&dateTo={date:yyyy-MM-dd}T23:59:59";
        using var req = new HttpRequestMessage(HttpMethod.Get, uri);
        using var resp = await Client.SendAsync(req, cancel).ConfigureAwait(false);
        var stream = await resp.Content.ReadAsStreamAsync(cancel).ConfigureAwait(false);
        var json = await JsonSerializer.DeserializeAsync<Response<Document>>(stream, cancellationToken: cancel).ConfigureAwait(false);
        if (json.Status != 0) throw json.ToException();
        return [.. json.Data.Select(d => d.ToReceipt(parseId))];
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
    public string KktRegNumber { get; set; }
    public string FnNumber { get; set; }
    public uint DocNumber { get; set; }
    public uint DocShiftNumber { get; set; }
    public string ReceiptNumber { get; set; }
    public ulong TotalSumm { get; set; }
    public string OperationType { get; set; }
    public TaxationSystem TaxationType { get; set; }
    public DocumentItem[] Items { get; set; }
    public ulong CashSumm { get; set; }
    public ulong ECashSumm { get; set; }
    public ulong PrepaidSumm { get; set; }
    public ulong CreditSumm { get; set; }
    public ulong ProvisionSumm { get; set; }

    public Receipt ToReceipt(Guid ParseId) => new()
    {
        ParseId = ParseId,
        RegId = KktRegNumber,
        StorageId = FnNumber,
        DocNumber = DocNumber,
        SessionId = DocShiftNumber,
        DocInSession = uint.Parse(ReceiptNumber),
        Created = DocDateTime,
        DocSign = uint.Parse(DecimalFiscalSign),
        Type = OperationType.ToLowerInvariant() switch
        {
            "income" => Operation.Income,
            "outcome" => Operation.Outcome,
            "refund income" => Operation.RefundIncome,
            "refund outcome" => Operation.RefundOutcome,
            _ => throw new NotSupportedException($"Тип операции '{OperationType}' не поддерживается.")
        },
        Items = [.. Items.Select(i => i.ToPosition())],
        Payment = new Payment
        {
            Cash = CashSumm,
            ECash = ECashSumm,
            Pre = PrepaidSumm,
            Post = CreditSumm,
            Provision = ProvisionSumm
        },
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
    public byte NDS_Rate { get; set; }
    public PositionType SubjectType { get; set; }

    public Position ToPosition()
    {
        PositionUnit? mu = null;
        if (ProductUnitOfMeasure is not null)
            mu = (PositionUnit)int.Parse(ProductUnitOfMeasure);

        return new Position
        {
            Name = Name,
            Price = Price,
            Quantity = (ulong)(Quantity * 1000),
            Cost = Total,
            PayMethod = CalculationMethod,
            Unit = mu,
            TaxRate = EnumUtils.ToTaxRate(NDS_Rate),
            Type = SubjectType
        };
    }
}