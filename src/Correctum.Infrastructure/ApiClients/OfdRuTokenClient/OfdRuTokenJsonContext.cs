using System.Text.Json.Serialization;

namespace Correctum.ApiClients.OfdRuTokenClient;

[JsonSerializable(typeof(OfdRuTokenResponse<OfdRuTokenKkt>))]
[JsonSerializable(typeof(OfdRuTokenResponse<OfdRuTokenReceipt>))]
internal partial class OfdRuTokenJsonContext : JsonSerializerContext { }
