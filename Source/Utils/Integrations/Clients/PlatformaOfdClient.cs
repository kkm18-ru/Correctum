using Correctum.Core.Models;
using Correctum.Deserializers;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Correctum.Utils.Integrations.Clients;

/// <summary>Обертка API-клиента ПлатформаОФД.</summary>
public sealed partial class PlatformaOfdClient() : ApiClient<PlatformaOfdClient>(Host)
{
    private const string Host = "https://lk.platformaofd.ru";
    private string JSessionId = "";

    /// <summary>Установка аутентификационных данных.</summary>
    /// <param name="sessionId">Идентификатор сессии</param>
    /// <param name="accessToken">Токен доступа</param>
    /// <param name="refreshToken">Обновляющий токен</param>
    public void Auth(string sessionId, string accessToken, string refreshToken)
    {
        ThrowIfDisposed();
        JSessionId = sessionId;
        Auth(accessToken, refreshToken);
    }

    private void Auth(string accessToken, string refreshToken)
    {
        Client.DefaultRequestHeaders.Remove("Cookie");
        Client.DefaultRequestHeaders.Add("Cookie", $"JSESSIONID={JSessionId}; ofd_access_token={accessToken}; refresh_token={refreshToken}");
    }

    private void Auth(HttpResponseHeaders headers)
    {
        if (!headers.Contains("Set-Cookie")) return;
        string access = "", refresh = "";
        var header = headers.GetValues("Set-Cookie");
        foreach(var value in header)
        {
            var @params = value.Split("; ");
            var kv = @params[0].Split("=");
            if (kv[0] == "access_token") access = kv[1];
            if (kv[0] == "refresh_token") refresh = kv[1];
        }
        if (!string.IsNullOrWhiteSpace(access) && 
            !string.IsNullOrWhiteSpace(refresh))
            Auth(access, refresh);
    }

    /// <summary>Получение списка идентификаторов ККТ.</summary>
    /// <param name="cancel">Токен, позволяющий отменить получение данных о ККТ.</param>
    /// <returns>Список идентификаторов ККТ.</returns>
    public async Task<PlatformaOfdDevice[]> FetchDevicesAsync(CancellationToken cancel = default)
    {
        ThrowIfDisposed();

        using var req = new HttpRequestMessage(HttpMethod.Get, "/web/auth/terminals/monitoring");
        using var resp = await Client.SendAsync(req, cancel);
        Auth(resp.Headers);
        var html = await resp.Content.ReadAsStringAsync(cancel);

        var deviceIds = DeviceIdRegex().Matches(html);
        var numbers = RegIdRegex().Matches(html);
        var devices = new PlatformaOfdDevice[deviceIds.Count];

        for (var i = 0; i < devices.Length; i++)
            devices[i] = new(ulong.Parse(deviceIds[i].Groups["id"].Value), numbers[i * 2].Groups["numb"].Value);
        return devices;
    }

    /// <summary>Получение списка ссылок на чеки.</summary>
    /// <param name="device">Контрольно-кассовая машина.</param>
    /// <param name="date">Дата для формирования отчета.</param>
    /// <param name="cancel">Токен, позволяющий отменить получение ссылок на чеки.</param>
    /// <returns>Список ссылок на чеки.</returns>
    public async Task<PlatformaOfdReceiptReference[]> FetchReceiptsAsync(DateOnly date, PlatformaOfdDevice device, CancellationToken cancel = default)
    {
        ThrowIfDisposed();

        var textDeviceId = device is { Id: > 0 } id ? id.Id.ToString() :
            "%D0%92%D1%81%D0%B5%20%D1%82%D0%B5%D1%80%D0%BC%D0%B8%D0%BD%D0%B0%D0%BB%D1%8B";
        var textDate = date.ToString("dd.MM.yyyy");
        var url = new StringBuilder("/web/auth/cheques/search");
        url.Append($"?deviceId={textDeviceId}&start={textDate}%2000%3A00&end={textDate}%2023%3A59");
        url.Append("&shiftNumber=&requestNumber=&sumFrom=&sumTo=&cashiers=%5B%5D&tag=&tagSearchField=&_fnsFdSentStatus=on&_fnsFdSentStatus=on&_fnsFdSentStatus=on&_fnsFdSentStatus=on&_fnsFdSentStatus=on&_paymentTypes=on&_paymentTypes=on&_paymentTypes=on&_paymentTypes=on&_paymentTypes=on&_paymentTypes=on&_operTypes=on&_operTypes=on&_operTypes=on&_operTypes=on&fragments=cheques-search-content%2Ccheques-search-common-content-block%2Ccom-messages&ajaxSource=cheques_filter_form_id");

        using var req = new HttpRequestMessage(HttpMethod.Get, url.ToString());
        using var resp = await Client.SendAsync(req, cancel);
        Auth(resp.Headers);
        var html = await resp.Content.ReadAsStringAsync(cancel);
        var matches = DocumentIdRegex().Matches(html);
        var arr = new PlatformaOfdReceiptReference[matches.Count];
        for (var i = 0; i < arr.Length; i++)
            arr[i] = new(matches[i]);
        return arr;
    }

    /// <summary>Получение списка ссылок на чеки коррекции.</summary>
    /// <param name="device">Контрольно-кассовая машина.</param>
    /// <param name="date">Дата для формирования отчета.</param>
    /// <param name="cancel">Токен, позволяющий отменить получение ссылок на чеки.</param>
    /// <returns>Список ссылок на чеки.</returns>
    public async Task<PlatformaOfdReceiptReference[]> FetchCorrectionReceiptsAsync(DateOnly date, PlatformaOfdDevice device, CancellationToken cancel = default)
    {
        ThrowIfDisposed();
        var textDeviceId = device is { Id: > 0 } id ? id.Id.ToString() :
            "%D0%92%D1%81%D0%B5%20%D1%82%D0%B5%D1%80%D0%BC%D0%B8%D0%BD%D0%B0%D0%BB%D1%8B";
        var textDate = date.ToString("dd.MM.yyyy");
        var url = new StringBuilder("/web/auth/cheques/search/correction");
        url.Append($"?deviceId={textDeviceId}&start={textDate}%2000%3A00&end={textDate}%2023%3A59");
        url.Append("&shiftNumber=&requestNumber=&sumFrom=&sumTo=&cashiers=%5B%5D&tag=&tagSearchField=&_fnsFdSentStatus=on&_fnsFdSentStatus=on&_fnsFdSentStatus=on&_fnsFdSentStatus=on&_fnsFdSentStatus=on&_paymentTypes=on&_paymentTypes=on&_paymentTypes=on&_paymentTypes=on&_paymentTypes=on&_paymentTypes=on&_operTypes=on&_operTypes=on&fragments=cheques-search-content%2Ccheques-search-common-content-block%2Ccom-messages&ajaxSource=cheques_filter_form_id");

        using var req = new HttpRequestMessage(HttpMethod.Get, url.ToString());
        using var resp = await Client.SendAsync(req, cancel);
        Auth(resp.Headers);
        var html = await resp.Content.ReadAsStringAsync(cancel);
        var matches = CorrectionDocumentIdRegex().Matches(html);
        var arr = new PlatformaOfdReceiptReference[matches.Count];
        for (var i = 0; i < arr.Length; i++)
            arr[i] = new(matches[i]);
        return arr;
    }

    /// <summary>Получение чека.</summary>
    /// <param name="reference">Ссылка на чек.</param>
    /// <param name="parseId">Идентификтор сессии парсера.</param>
    /// <param name="cancel">Токен, позволяющий отменить получение информации о чеке.</param>
    /// <returns>Информация о чеке.</returns>
    public async Task<Receipt?> FetchReceiptAsync(PlatformaOfdReceiptReference reference, CancellationToken cancel = default)
    {
        ThrowIfDisposed();
        using var req = new HttpRequestMessage(HttpMethod.Get, reference.ToString());
        using var resp = await Client.SendAsync(req, cancel);
        Auth(resp.Headers);
        var html = await resp.Content.ReadAsStringAsync(cancel);
        var raw = HttpUtility.HtmlDecode(html.Split(@"<pre>")[1].Split(@"</pre>")[0]);
        return PlatformOfdDeserializer.Deserialize(raw);
    }

    [GeneratedRegex(@"terminal_(?<id>\d+)_id", 0, 250)]
    private static partial Regex DeviceIdRegex();
    [GeneratedRegex(@"<td>(?<numb>\d{16})</td>", 0, 250)]
    private static partial Regex RegIdRegex();
    [GeneratedRegex(@"id=(?<id>\d+)&date=(?<ts>\d+)&fp=(?<sign>\d+)", 0, 250)]
    private static partial Regex CorrectionDocumentIdRegex();
    [GeneratedRegex(@"(?<id>\d+)\/(?<ts>\d+)\/(?<sign>\d+)", 0, 250)]
    private static partial Regex DocumentIdRegex();
}

public readonly struct PlatformaOfdReceiptReference(Match match)
{
    public ulong Timestamp { get; } = ulong.Parse(match.Groups["ts"].Value);
    public uint FiscalSign { get; } = uint.Parse(match.Groups["sign"].Value);
    public ulong DocId { get; } = ulong.Parse(match.Groups["id"].Value);

    public readonly override string ToString() =>
        $"/web/auth/cheques/requisites?id={DocId}&date={Timestamp}&fp={FiscalSign}";
}

public readonly struct PlatformaOfdDevice(ulong devId, string regId)
{
    public ulong Id { get; } = devId;
    public readonly override string ToString() => regId;
}