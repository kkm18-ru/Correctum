namespace Correctum.Configurations;

public sealed class HttpClientConfigurationSection
{
    /// <summary>
    /// Возвращает время ожидания ответа на HTTP-запрос в секундах.
    /// </summary>
    public uint Timeout { get; set; }

    public static HttpClientConfigurationSection Default => new()
    {
        Timeout = 30
    };
}
