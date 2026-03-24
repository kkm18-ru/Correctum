using System.Text;

namespace Correctum.SecretEncoders;

public abstract class SecretEncoder
{
    private static SecretEncoder m_current;

    static SecretEncoder()
    {
        if (OperatingSystem.IsWindows()) m_current = new WindowsSecretEncoder();
        else throw new PlatformNotSupportedException();
    }
    
    protected abstract string InnerEncode(string raw, Encoding encoding);
    protected abstract string InnerDecode(string encoded, Encoding encoding);

    public static string Encode(string raw, Encoding? encoding = null) =>
        string.IsNullOrWhiteSpace(raw) ? string.Empty : m_current.InnerEncode(raw, encoding ?? Encoding.UTF8);

    public static string Decode(string encoded, Encoding? encoding = null) =>
        string.IsNullOrWhiteSpace(encoded) ? string.Empty : m_current.InnerDecode(encoded, encoding ?? Encoding.UTF8);
}
