using System.Text.Json.Serialization;

namespace Correctum.ApiClients.SabyClient;

public sealed class SabyAuthException : Exception
{
    [JsonConstructor] internal SabyAuthException(string error) : base(error) { }
    private SabyAuthException() { }
}
