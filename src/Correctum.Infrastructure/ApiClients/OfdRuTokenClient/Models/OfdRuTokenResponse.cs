using System.Text.Json.Serialization;

namespace Correctum.ApiClients.OfdRuTokenClient.Models;

[JsonConverter(typeof(JsonStringEnumConverter<ResponseStatus>))] 
internal enum ResponseStatus { Success, Failed }
internal interface IOfdRuTokenModel;

internal readonly struct OfdRuTokenResponse<T> where T : struct, IOfdRuTokenModel
{
    public ResponseStatus Status { get; init; }
    public T[] Data { get; init; }
    public string[] Errors { get; init; }

    public readonly void ThrowIfFailed()
    {
        if (Status == ResponseStatus.Success) return;
        throw new OfdRuTokenException(Errors);
    }
}

