using System.Text.Json.Serialization;

namespace Correctum.ApiClients.FirstOfdClient.Models;

[method: JsonConstructor]
internal readonly struct FirstOfdPagination(int totalItems)
{
    public int Total { get; } = totalItems;
}
