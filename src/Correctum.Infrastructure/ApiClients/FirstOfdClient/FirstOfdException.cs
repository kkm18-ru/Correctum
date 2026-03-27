using System.Text.Json;
using System.Text.Json.Serialization;

namespace Correctum.ApiClients.FirstOfdClient;

public sealed class FirstOfdException : Exception
{
    [JsonConstructor] internal FirstOfdException(string localizedMessage) : base(localizedMessage) { }

    public static void ThrowIfError(HttpResponseMessage msg)
    {
        if (msg.IsSuccessStatusCode) return;
        var stream = msg.Content.ReadAsStream();
        var exc = JsonSerializer.Deserialize(stream, FirstOfdJsonContext.Default.FirstOfdException);
        throw exc ?? new FirstOfdException("Неизвестная ошибка");                
    }
}
