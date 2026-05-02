using System.Text;
using Bogus;

namespace Correctum.Tests.Utils;

public class SecretHelper
{
    private static IEnumerable<TestCaseData<string>> Passwords()
    {
        for (var i = 0; i < 250; i++)
            yield return new TestCaseData<string>(new Faker().Internet.Password(Random.Shared.Next(4, 32)));
    }

    [TestCaseSource(nameof(Passwords))]
    public void CheckEncodeAndDecodeUnicode(string password) => 
        CheckEncodeAndDecode(password, Encoding.Unicode);

    [TestCaseSource(nameof(Passwords))]
    public void CheckEncodeAndDecodeUtf8(string password) => 
        CheckEncodeAndDecode(password, Encoding.UTF8);

    [TestCaseSource(nameof(Passwords))]
    public void CheckEncodeAndDecodeUtf32(string password) => 
        CheckEncodeAndDecode(password, Encoding.UTF32);

    [TestCaseSource(nameof(Passwords))]
    public void CheckEncodeAndDecodeAscii(string password) => 
        CheckEncodeAndDecode(password, Encoding.ASCII);

    [TestCaseSource(nameof(Passwords))]
    public void CheckEncodeAndDecodeBigUnicode(string password) => 
        CheckEncodeAndDecode(password, Encoding.BigEndianUnicode);

    [TestCaseSource(nameof(Passwords))]
    public void CheckEncodeAndDecodeLatin1(string password) => 
        CheckEncodeAndDecode(password, Encoding.Latin1);
    
    [Test] public void CheckEncodeAndDecodeEmpty()
    {
        var encoded = Correctum.Utils.SecretHelper.EncodeSecret("");
        Assert.That(encoded, Is.Empty);
        
        var decoded = Correctum.Utils.SecretHelper.DecodeSecret("");
        Assert.That(decoded, Is.Empty);
    }
    [Test] public void CheckEncodeAndDecodeSpaces() 
    {
        var encoded = Correctum.Utils.SecretHelper.EncodeSecret("         ");
        Assert.That(encoded, Is.Empty);
        
        var decoded = Correctum.Utils.SecretHelper.DecodeSecret("           ");
        Assert.That(decoded, Is.Empty);
    }

    private static void CheckEncodeAndDecode(string password, Encoding encoding)
    {
        var encoded = Correctum.Utils.SecretHelper.EncodeSecret(password, encoding);
        Assert.That(encoded, Is.Not.Empty);
        Assert.That(encoded, Is.Not.EqualTo(password));
        
        var decoded = Correctum.Utils.SecretHelper.DecodeSecret(encoded, encoding);
        Assert.That(decoded, Is.Not.Empty);
        Assert.That(decoded, Is.EqualTo(password));
    }
}