using NLog;
using System.Security.Cryptography;
using System.Text;
using Tomlyn.Serialization;

namespace Correctum.Converters.Toml;

internal sealed class SecretTomlConverter : TomlConverter<string>
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private static Encoding LocalEncoding => Encoding.UTF8;
    private static DataProtectionScope ProtectionScope => DataProtectionScope.CurrentUser;

    public override string? Read(TomlReader reader)
    {
        var str = reader.GetString();
        if (string.IsNullOrWhiteSpace(str)) return string.Empty;
        try
        {
            var protectedByte = Convert.FromBase64String(str);
            var bytes = ProtectedData.Unprotect(protectedByte, [], ProtectionScope);
            return LocalEncoding.GetString(bytes);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Не удалось расшифровать секрет.");
            return null;
        }
    }

    public override void Write(TomlWriter writer, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            writer.WriteStringValue(string.Empty);
            return;
        }

        try
        {
            var bytes = LocalEncoding.GetBytes(value);
            var protectedByte = ProtectedData.Protect(bytes, [], ProtectionScope);
            writer.WriteStringValue(Convert.ToBase64String(protectedByte));
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Не удалось зашифровать секрет.");
            return;
        }
    }
}
