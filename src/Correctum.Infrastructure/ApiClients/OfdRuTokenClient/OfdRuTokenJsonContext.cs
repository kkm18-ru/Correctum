using Correctum.ApiClients.OfdRuTokenClient.Models;
using System.Text.Json.Serialization;

namespace Correctum.ApiClients.OfdRuTokenClient;

[JsonSerializable(typeof(OfdRuTokenResponse<OfdRuTokenKkt>))]
[JsonSerializable(typeof(OfdRuTokenResponse<OfdRuTokenReceipt>))]
[JsonSerializable(typeof(OfdRuTokenException))]
internal partial class OfdRuTokenJsonContext : JsonSerializerContext { }
