using System.Text.Json.Serialization;

namespace Correctum.ApiClients.FirstOfdClient.Models;

[method: JsonConstructor]
internal readonly struct FirstOfdTransactionResponse
    (FirstOfdTransaction[] transactions, FirstOfdPagination pagination)
{
    public FirstOfdTransaction[] Transactions { get; } = transactions;
    public FirstOfdPagination Pagination { get; } = pagination;
}
