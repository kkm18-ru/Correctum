namespace Correctum.Configurations;

public sealed class LokiConfigurationSection
{
    public bool Enabled { get; set; }
    public string Endpoint { get; set; }
    public string HostName { get; set; }
    public bool AuthEnabled { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public static LokiConfigurationSection Default => new()
    {
        Enabled = false,
        Endpoint = "http://localhost:3100",
        HostName = "MainHost",
        AuthEnabled = false,
        Username = "admin",
        Password = string.Empty,
    };
}