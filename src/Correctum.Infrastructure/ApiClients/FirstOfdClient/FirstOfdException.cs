using System.Text.Json.Serialization;

namespace Correctum.ApiClients.FirstOfdClient;

public sealed class FirstOfdException : Exception
{
    [JsonConstructor] internal FirstOfdException(string localizedMessage) : base(localizedMessage) { }
}
