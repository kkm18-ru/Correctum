using System.Text.Json.Serialization;

namespace Correctum.ApiClients.SabyClient;

[JsonSerializable(typeof(SabyAuthException))]
[JsonSerializable(typeof(SabyServerException))]
[JsonSerializable(typeof(SabyAuthRequest))]
[JsonSerializable(typeof(SabyAuthResponse))]
[JsonSerializable(typeof(SabyKkt))]
[JsonSerializable(typeof(SabyFiscalDrive))]
[JsonSerializable(typeof(SabyDocument))]
internal partial class SabyJsonContext : JsonSerializerContext;
