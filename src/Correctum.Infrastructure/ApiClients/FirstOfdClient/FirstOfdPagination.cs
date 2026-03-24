using System.Text.Json.Serialization;

namespace Correctum.ApiClients.FirstOfdClient;

[method: JsonConstructor]
internal readonly struct FirstOfdPagination(int totalItems)
{
    public int Total { get; } = totalItems;
}
