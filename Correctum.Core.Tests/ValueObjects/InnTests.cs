using Correctum.Core.ValueObjects;

namespace Correctum.Core.Tests.ValueObjects;

public sealed class InnTests
{
    private const string ValidedInnCompany = "0123456788";
    private const string ValidedInnPerson = "012345678943";

    [Test]
    public void Parse_Should_ThrowException_OutOfRangeLength()
    {
        var ex = Assert.Throws<FormatException>(() => Inn.Parse("123"));
        Assert.That(ex.Message, Is.EqualTo("Длина ввода отлична от 10 и 12 символов."));
    }

    [Test]
    public void Parse_Should_ThrowException_IsNotDigitCode()
    {
        var ex = Assert.Throws<FormatException>(() => Inn.Parse("qwertyuiop"));
        Assert.That(ex.Message, Is.EqualTo("Ввод не является числовым кодом."));
    }

    [Test]
    public void Parse_Should_ThrowException_InvalidHashSum()
    {
        var ex = Assert.Throws<FormatException>(() => Inn.Parse("0123456789"));
        Assert.That(ex.Message, Is.EqualTo("Контрольное значение ИНН некорректно."));
    }

    [Test] public void Parse() => Assert.DoesNotThrow(() => Inn.Parse(ValidedInnCompany));
    [Test] public void TryParse_Should_ReturnFalse() => Assert.That(Inn.TryParse("012345678941", out _), Is.False);
    [Test] public void TryParse_Should_ReturnTrue() => Assert.That(Inn.TryParse(ValidedInnPerson, out _), Is.True);
    [Test] public void ToString_Should_ReturnEmpty() => Assert.That(new Inn().ToString(), Is.Empty);
    [Test] public void ToString_Company() => Assert.That(Inn.Parse(ValidedInnCompany).ToString(), Is.EqualTo(ValidedInnCompany));
    [Test] public void ToString_Person() => Assert.That(Inn.Parse(ValidedInnPerson).ToString(), Is.EqualTo(ValidedInnPerson));
#pragma warning disable NUnit2010
    [Test] public void EqualsInns() => Assert.That(Inn.Parse(ValidedInnPerson).Equals(Inn.Parse(ValidedInnCompany)), Is.False);
    [Test] public void EqualsObjects() => Assert.That(Inn.Parse(ValidedInnPerson).Equals(0), Is.False);
#pragma warning restore NUnit2010
}
