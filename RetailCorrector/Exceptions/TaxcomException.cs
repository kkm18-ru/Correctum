using System.Net;
using System.Net.Http;

namespace RetailCorrector.Exceptions;

public sealed class TaxcomException(string? message, int apiCode, string summary, int httpCode)
    : HttpRequestException(message, null, (HttpStatusCode)httpCode)
{
    public int ApiCode { get; } = apiCode;
    public string Summary { get; } = summary;
}