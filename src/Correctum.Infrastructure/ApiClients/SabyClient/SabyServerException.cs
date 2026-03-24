using System.Text.Json.Serialization;

namespace Correctum.ApiClients.SabyClient;

public sealed class SabyServerException : Exception
{
    [JsonConstructor] internal SabyServerException(SabyInternalServerErrorInfo error): base(error.Message) { }
}

[method: JsonConstructor]
internal readonly struct SabyInternalServerErrorInfo(string message)
{
    public string Message { get; } = message;
}