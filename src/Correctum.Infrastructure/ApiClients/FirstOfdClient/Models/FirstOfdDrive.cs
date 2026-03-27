using System.Text.Json.Serialization;

namespace Correctum.ApiClients.FirstOfdClient.Models;

[method: JsonConstructor]
internal readonly struct FirstOfdDrive(string number)
{
    public string SerialNumber { get; } = number;
}
