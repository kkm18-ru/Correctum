using Correctum.ApiClients.FirstOfdClient.Models;
using System.Text.Json.Serialization;

namespace Correctum.ApiClients.FirstOfdClient;

[JsonSerializable(typeof(FirstOfdTransactionResponse))]
[JsonSerializable(typeof(FirstOfdKktSearchRequest))]
[JsonSerializable(typeof(FirstOfdTicketResponse))]
[JsonSerializable(typeof(FirstOfdAuthResponse))]
[JsonSerializable(typeof(FirstOfdAuthRequest))]
[JsonSerializable(typeof(FirstOfdException))]
[JsonSerializable(typeof(FirstOfdPlace[]))]
[JsonSerializable(typeof(FirstOfdDrive[]))]
[JsonSerializable(typeof(FirstOfdKkt[]))]
internal sealed partial class FirstOfdJsonContext : JsonSerializerContext;
