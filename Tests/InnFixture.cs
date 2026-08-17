using Correctum.ValueObjects;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Correctum.Tests;

[TestFixture]
public class InnFixture
{
    [TestCase("0000000000")]
    [TestCase("1234567894")]
    [TestCase("9999999998")]
    [TestCase("000000000000")]
    [TestCase("123456789047")]
    [TestCase("999999999950")]
    [TestCase("001597523403")]
    public void Validate_NotThrows(string inn) => Inn.Validate(inn);

    [TestCase("0000000000")]
    [TestCase("000000000000")]
    [TestCase("999999999950")]
    [TestCase("001597523403")]
    public void ParseAndToString_NotThrows(string inn)
    {
        var _inn = Inn.Parse(inn, CultureInfo.InvariantCulture);
        Assert.That(inn, Is.EqualTo(_inn.ToString()));
    }

    [Test] public void Sizeof_ReturnSix() => 
        Assert.That(Unsafe.SizeOf<Inn>(), Is.EqualTo(6));
}
