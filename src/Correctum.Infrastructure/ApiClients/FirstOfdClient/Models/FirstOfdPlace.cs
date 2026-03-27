using System.Text.Json.Serialization;

namespace Correctum.ApiClients.FirstOfdClient.Models;

public readonly struct FirstOfdPlace
{
    public int Id { get; }
    public string Title { get; }

    [JsonConstructor]
    internal FirstOfdPlace(int id, string title) =>
        (Id, Title) = (id, title);
}
