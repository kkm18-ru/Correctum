using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Correctum.Exceptions.Saby;
using Correctum.Core.Models;
using Correctum.Core.Enums;
using Correctum.Core.ValueObjects;

namespace Correctum.Utils.Integrations.Clients;

/// <summary>Обертка API-клиента SabyOFD.</summary>
internal sealed class SabyClient() : ApiClient<SabyClient>(BaseHost)
{
    private const string BaseHost = "https://api.sbis.ru";

    /// <summary>Аутентификация в API SabyOFD.</summary>
    /// <param name="login">Логин пользователя от личного кабинета Saby.</param>
    /// <param name="password">Пароль пользователя от личного кабинета Saby.</param>
    /// <param name="asFranchise">true, если аутентификация выполняется от имени арендодателя или франшизы, иначе false.</param>
    /// <param name="cancel">Токен, позволяющий отменить аутентификацию.</param>
    public async Task AuthAsync(string login, string password, bool asFranchise = false, CancellationToken cancel = default)
    {
        ThrowIfDisposed();
        Client.DefaultRequestHeaders.Remove("Cookie");
        var body = new AuthRequest(login, password, asFranchise);
        using var req = new HttpRequestMessage(HttpMethod.Post, "/oauth/service/");
        req.Content = JsonContent.Create(body);
        using var resp = await Client.SendAsync(req, cancel).ConfigureAwait(false);
        await using var stream = await resp.Content.ReadAsStreamAsync(cancel).ConfigureAwait(false);
        if(!resp.IsSuccessStatusCode)
        {
            var node = await JsonNode.ParseAsync(stream, cancellationToken: cancel).ConfigureAwait(false);
            throw new SabyAuthException(node?["error"]?.GetValue<string>() ?? "");
        }
        var json = await JsonSerializer.DeserializeAsync<AuthResponse>(stream, cancellationToken: cancel).ConfigureAwait(false);
        Client.DefaultRequestHeaders.Add("Cookie", $"sid={json.SessionId}");
    }
    
    /// <summary>Получение списка зарегистрированных ККТ.</summary>
    /// <param name="inn">ИНН организации-владельца ККТ.</param>
    /// <param name="cancel">Токен, позволяющий отменить выгрузку ККТ.</param>
    /// <returns>Асинхронное перечисление зарегистрированных ККТ.</returns>
    public async IAsyncEnumerable<string> EnumerateDevicesAsync
        (Inn inn, [EnumeratorCancellation] CancellationToken cancel = default)
    {
        ThrowIfDisposed();
        using var req = new HttpRequestMessage(HttpMethod.Get, $"/ofd/v1/orgs/{inn}/kkts");
        using var resp = await Client.SendAsync(req, cancel).ConfigureAwait(false);
        await using var stream = await resp.Content.ReadAsStreamAsync(cancel).ConfigureAwait(false);
        if (!resp.IsSuccessStatusCode) throw new SabyException(stream);
        var enumerable = JsonSerializer.DeserializeAsyncEnumerable<KktResponse>(stream, cancellationToken: cancel)
            .ConfigureAwait(false);
        await foreach (var kkt in enumerable)
            yield return kkt.Id;
    }

    /// <summary>Получение списка фискальных накопителей по ККТ.</summary>
    /// <param name="inn">ИНН организации-владельца ККТ.</param>
    /// <param name="regId">Регистрационный номер ККТ, выданный ФНС.</param>
    /// <param name="dateFrom">Время начала периода сканирования.</param>
    /// <param name="dateTo">Время окончания периода сканирования.</param>
    /// <param name="cancel">Токен, позволяющий отменить выгрузку фискальных накопителей.</param>
    /// <returns>Массив фискальных накопителей по ККТ</returns>
    public async Task<SabyDriveResponse[]> FetchDrivesAsync
        (Inn inn, string regId, DateTime dateFrom, DateTime dateTo, CancellationToken cancel = default)
    {
        ThrowIfDisposed();
        using var req = new HttpRequestMessage(HttpMethod.Get, $"/ofd/v1/orgs/{inn}/kkts/{regId}/storages");
        using var resp = await Client.SendAsync(req, cancel).ConfigureAwait(false);
        await using var stream = await resp.Content.ReadAsStreamAsync(cancel).ConfigureAwait(false);
        if (!resp.IsSuccessStatusCode) throw new SabyException(stream);
        var json = await JsonSerializer.DeserializeAsync<SabyDriveResponse[]>(stream, cancellationToken: cancel) ?? [];
        return [..json.Where(i => !(i.DateStart > dateTo || (i.DateEnd is not null && i.DateEnd < dateFrom)))];
    }

    /// <summary>Получение списка чеков.</summary>
    /// <param name="args">Основные параметры запроса.</param>
    /// <param name="offset">Номер фискального документа, от которого идет отсчет фискальных документов в списке.</param>
    /// <param name="cancel">Токен, позволяющий отменить выгрузку чеков.</param>
    /// <returns>Массив чеков, если API ответил непустым массивом документов, иначе null.</returns>
    public async Task<Receipt[]?> FetchReceiptsAsync
        (SabyReceiptsRequest args, uint offset = 0, CancellationToken cancel = default)
    {
        ThrowIfDisposed();
        var builder = new StringBuilder(args.ToUri());
        builder.Append($"&startId={offset}&limit={Configuration.Current.Http.PageSize}");
        using var resp = await Client.GetAsync(builder.ToString(), cancel);
        await using var stream = await resp.Content.ReadAsStreamAsync(cancel);
        if (!resp.IsSuccessStatusCode) throw new SabyException(stream);
        var json = await JsonSerializer.DeserializeAsync<DocumentResponse[]>(stream, cancellationToken: cancel) ?? [];
        return json.Length == 0 ? null :
            [..json.Where(i => i.GenericReceipt is not null)
                .Select(i => i.GenericReceipt!.Value.ToReceipt())];
    }
}

/// <summary>
/// Основные параметры <see cref="SabyClient.FetchReceiptsAsync"/>.
/// </summary>
/// <param name="parseId">Идентификатор сессии парсера.</param>
/// <param name="inn">ИНН организации-владельца ККТ.</param>
/// <param name="regId">Регистрационный номер ККТ, выданный ФНС.</param>
/// <param name="driveId">Номер фискального накопителя.</param>
public readonly struct SabyReceiptsRequest (Inn inn, string regId, string driveId)
{   
    /// <summary>ИНН организации-владельца ККТ.</summary>
    private Inn Inn { get; } = inn;
    
    /// <summary>Регистрационный номер ККТ, выданный ФНС.</summary>
    private string RegId { get; } = regId;
    
    /// <summary>Номер фискального накопителя.</summary>
    private string DriveId { get; } = driveId;
    
    /// <summary>Время начала периода запрашиваемых документов.</summary>
    /// <remarks>Период между <see cref="DateFrom"/> и <see cref="DateTo"/> не должен быть больше 7 дней</remarks>
    public DateTime DateFrom { private get; init; }
    
    /// <summary>Время окончания периода запрашиваемых документов.</summary>
    /// <remarks>Период между <see cref="DateFrom"/> и <see cref="DateTo"/> не должен быть больше 7 дней</remarks>
    public DateTime DateTo { private get; init; }

    public string ToUri() =>
        $"/ofd/v1/orgs/{Inn}/kkts/{RegId}/storages/{DriveId}/docs" +
        $"?dateFrom={DateFrom:yyyy-MM-ddTHH:mm:ss}&dateTo={DateTo:yyyy-MM-ddTHH:mm:ss}";
}

file readonly struct AuthRequest(string login,string password,bool asFranchise)
{
    [JsonPropertyName("appClient_id")]
    public string AppClientId { get; } = asFranchise ? "2248309793600149" : "1025293145607151";
    
    [JsonPropertyName("login")]
    public string Login { get; } = login;
    
    [JsonPropertyName("password")]
    public string Password { get; } = password; 
}

file struct KktResponse
{
    [JsonPropertyName("regId")]
    public string Id { get; set; }
    
    [JsonPropertyName("firstShiftdate")]
    public DateTime DateStart { get; set;  }
    
    [JsonPropertyName("fsFinishDate")]
    public DateTime DateEnd { get; set;  }
}

public struct SabyDriveResponse
{
    [JsonPropertyName("storageId")]
    public string Id { get; set; }
    
    [JsonPropertyName("effectiveFrom")]
    public DateTime DateStart { get; set;  }
    
    [JsonPropertyName("effectiveTo")]
    public DateTime? DateEnd { get; set;  }
}

file struct AuthResponse
{
    [JsonPropertyName("sid")]
    public string SessionId { get; set; }
    
    [JsonPropertyName("token")]
    public string Token { get; set;  }
}

file struct DocumentResponse
{
    [JsonPropertyName("receipt")]
    public ReceiptResponse? Receipt { get; set; }
    
    [JsonPropertyName("receiptCorrection")]
    public ReceiptResponse? ReceiptCorrection { get; set; }

    [JsonIgnore] public ReceiptResponse? GenericReceipt => Receipt ?? ReceiptCorrection;
}

file struct ReceiptResponse
{
    [JsonPropertyName("kktRegId")]
    public string RegId { get; set; }
    
    [JsonPropertyName("fiscalDriveNumber")]
    public string StorageId { get; set; }
    
    [JsonPropertyName("shiftNumber")]
    public uint SessionId { get; set; }
    
    [JsonPropertyName("requestNumber")]
    public uint DocInSession { get; set; }

    [JsonPropertyName("buyerPhoneOrAddress")]
    public string Buyer { get; set; }
    
    [JsonPropertyName("dateTime")]
    public long DateTimeUnix { get; set; }
    
    [JsonPropertyName("totalSum")]
    public ulong Total { get; set; }
    
    [JsonPropertyName("cashTotalSum")]
    public ulong Cash { get; set; }
    
    [JsonPropertyName("operationType")]
    public ReceiptType Operation { get; set; }

    [JsonPropertyName("items")]
    public ItemResponse[] Items { get; set; }

    [JsonPropertyName("fiscalSign")]
    public uint Sign { get; set; }
    
    [JsonPropertyName("ecashTotalSum")]
    public ulong Ecash { get; set; }
    
    [JsonPropertyName("prepaidSum")]
    public ulong Prepaid { get; set; }
    
    [JsonPropertyName("creditSum")]
    public ulong Credit { get; set; }
    
    [JsonPropertyName("taxationType")]
    public TaxationSystems Taxation { get; set; }
    
    [JsonPropertyName("provisionSum")]
    public ulong Provision { get; set; }
    
    [JsonPropertyName("fiscalDocumentNumber")]
    public uint Number { get; set; }

    public Receipt ToReceipt() => new()
    {
        RegId = RegId,
        StorageId = StorageId,
        DocInSession = DocInSession,
        SessionId = SessionId,
        Items = Items.Select(ToPosition).ToArray(),
        Payment = new Payment
        {
            CashSum = Cash,
            NonCashSum = Ecash,
            AdvanceSum = Prepaid,
            CreditSum = Credit,
            ProvisionSum = Provision
        },
        Type = Operation,
        BuyerContact = Buyer,
        DocSign = Sign,
        TotalSum = Total,
        TaxationSystem = Taxation,
        DocNumber = Number,
        Created = DateTimeOffset.FromUnixTimeSeconds(DateTimeUnix).DateTime
    };
    
    private static Position ToPosition(ItemResponse item) => new()
    {
        Name = item.Name,
        Cost = item.Sum,
        Price = item.Price,
        Quantity = (ulong)item.Quantity * 1000,
        Unit = item.Measure,
        TaxRate = NDSRate.FromFns(item.Vat),
        Type = item.Type ?? 0,
        CalcMethod = item.Payment ?? 0
    };
}

file struct ItemResponse
{
    [JsonPropertyName("quantity")]
    public double Quantity { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("sum")]
    public ulong Sum { get; set; }
    
    [JsonPropertyName("price")]
    public ulong Price { get; set; }
    
    [JsonPropertyName("itemsQuantityMeasure")]
    public UnitOfMeasure? Measure { get; set; }
    
    [JsonPropertyName("nds")]
    public byte? Vat { get; set; }

    [JsonPropertyName("productType")]
    public PositionType? Type { get; set; }

    [JsonPropertyName("paymentType")]
    public CalculationMethod? Payment { get; set; }

}

