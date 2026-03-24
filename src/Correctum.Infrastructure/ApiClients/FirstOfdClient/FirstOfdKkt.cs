using System.Text.Json.Serialization;

namespace Correctum.ApiClients.FirstOfdClient;

public readonly struct FirstOfdKkt
{
    public int Id { get; }
    public string RegNumber { get; }

    [JsonConstructor] 
    internal FirstOfdKkt(int id, string fnsKkmId) => 
        (Id, RegNumber) = (id, fnsKkmId);
}
