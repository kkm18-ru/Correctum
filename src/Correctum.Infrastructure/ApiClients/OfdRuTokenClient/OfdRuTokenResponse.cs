using System.Text.Json.Serialization;

namespace Correctum.ApiClients.OfdRuTokenClient;

[JsonConverter(typeof(JsonStringEnumConverter))] internal enum ResponseStatus { Success, Failed }
internal interface IOfdRuTokenModel;

internal readonly struct OfdRuTokenResponse<T> where T : struct, IOfdRuTokenModel
{
    public ResponseStatus Status { get; init; }
    public IEnumerable<T> Data { get; init; }
    public IEnumerable<string> Errors { get; init; }

    public readonly void ThrowIfFailed()
    {
        if (Status == ResponseStatus.Success) return;
        throw new OfdRuTokenException(Errors);
    }
}

