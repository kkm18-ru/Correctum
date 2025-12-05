using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace RetailCorrector.Utils.Integrations;

internal class FeedbackClient : APIWrapper
{
    private readonly HttpClient _client = new(Handler, false);
    private const string project = "retailcorrector";
    private const string UnknownError = "Неизвестная ошибка";
    private const string Host = "https://hook.eu2.make.com/piq3tak50r15gaoflsp8iu7upk7irf33";

    public async Task SendReportAsync(string title, string description)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, Host);
        var data = new { title, description, project };
        req.Content = JsonContent.Create(data);
        using var resp = await _client.SendAsync(req).ConfigureAwait(false);
        if (resp.IsSuccessStatusCode) return;
        var body = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var json = await JsonNode.ParseAsync(body).ConfigureAwait(false);
        throw new Exception(json?["message"]?.GetValue<string>() ?? UnknownError);
    }

    ~FeedbackClient() { _client.Dispose(); }
}
