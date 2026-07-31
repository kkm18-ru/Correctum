using System.Net;
using System.Net.Http;

namespace Correctum.Exceptions;

public sealed class TaxcomException(string? message, int apiCode, string summary, int httpCode)
    : HttpRequestException(message, null, (HttpStatusCode)httpCode)
{
    public int ApiCode { get; } = apiCode;
    public string Summary { get; } = summary;

    public override string ToString() => $"{Summary}{(string.IsNullOrWhiteSpace(Message) ? "" : ": " + Message)}";
}