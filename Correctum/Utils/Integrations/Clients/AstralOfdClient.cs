using System.Text.Json;
﻿using System.Text.Json.Serialization;

namespace Correctum.Utils.Integrations.Clients;

public sealed class AstralOfdClient(string host) : ApiClient<AstralOfdClient>(host)
{
    private string SessionId = string.Empty;
    public AstralOfdClient() : this("https://ofd.astralnalog.ru") { }

    public void SetSessionId(string sid) => SessionId = sid;
}

internal readonly struct AstralOfdResponse<T> where T : struct
{
    [JsonPropertyName("ok")] public bool Success { get; init; }
    [JsonPropertyName("result")] public T Data { get; init; }
}

internal partial class AstralOfdSerializerContext : JsonSerializerContext;
