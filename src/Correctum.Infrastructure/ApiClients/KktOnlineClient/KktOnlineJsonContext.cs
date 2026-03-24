using System.Text.Json.Serialization;

namespace Correctum.ApiClients.KktOnlineClient;

[JsonSerializable(typeof(KktOnlineResponse))]
public partial class KktOnlineJsonContext : JsonSerializerContext;