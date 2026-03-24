using System.Text.Json.Serialization;

namespace Correctum.ApiClients.FirstOfdClient;

public readonly struct FirstOfdTransaction
{
    public string Id { get; }
    public string Type { get; }
    
    [JsonConstructor]
    internal FirstOfdTransaction(string id, string transactionType) =>
        (Id, Type) = (id, transactionType);
}
