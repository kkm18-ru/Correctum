using Correctum.Core.ValueObjects;

namespace Correctum.Core.Tests.ValueObjects;

public sealed class CurrencyTests
{
    [Test] public void FromLong_Should_ConvertFromLong() =>
        Assert.That((Currency)100L, Is.EqualTo((Currency)100UL));

    [Test] public void FromLong_Should_ThrowArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => _ = (Currency)(-100L));
        Assert.That(ex.ParamName, Is.EqualTo("value"));
    }
    [Test] public void FromDecimal_Should_ConvertFromDecimal() =>
        Assert.That((Currency)1M, Is.EqualTo((Currency)100UL));

    [Test] public void FromDecimal_Should_ThrowArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => _ = (Currency)(-100M));
        Assert.That(ex.ParamName, Is.EqualTo("value"));
    }
    [Test] public void FromDouble_Should_ConvertFromDouble() =>
        Assert.That((Currency)1D, Is.EqualTo((Currency)100UL));

    [Test] public void FromDouble_Should_ThrowArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => _ = (Currency)(-100D));
        Assert.That(ex.ParamName, Is.EqualTo("value"));
    }

    [Test] public void ToULong_Should_ConvertToULong() =>
        Assert.That((ulong)(Currency)100UL, Is.EqualTo(100UL));

    [Test] public void ToDouble_Should_ConvertToDouble() =>
        Assert.That((double)(Currency)100UL, Is.EqualTo(1D));

    [Test] public void ToDecimal_Should_ConvertToDecimal() =>
        Assert.That((decimal)(Currency)100UL, Is.EqualTo(1M));

    [Test] public void PlusCurrencyAndCurrency() =>
        Assert.That((Currency)100UL+ (Currency)100UL, Is.EqualTo((Currency)200UL));

    [Test] public void MinusCurrencyAndCurrency() =>
        Assert.That((Currency)200UL - (Currency)100UL, Is.EqualTo((Currency)100UL));

    [Test]
    public void PlusCurrencyAndULong() =>
        Assert.That((Currency)100UL + 100, Is.EqualTo(200));

    [Test]
    public void PlusULongAndCurrency() =>
        Assert.That(100+(Currency)100UL, Is.EqualTo(200));

    [Test]
    public void MinusCurrencyAndULong() =>
        Assert.That((Currency)200UL - 100, Is.EqualTo(100));

    [Test]
    public void MinusULongAndCurrency() =>
        Assert.That(200-(Currency)100UL, Is.EqualTo(100));

    [Test]
    public void MultipleCurrencyAndULong() =>
        Assert.That((Currency)100UL*2, Is.EqualTo(200));

    [Test]
    public void MultipleULongAndCurrency() =>
        Assert.That(2*(Currency)100UL, Is.EqualTo(200));

    [Test]
    public void DivileCurrencyAndULong() =>
        Assert.That((Currency)100UL / 10UL, Is.EqualTo(10));

    [Test]
    public void DivileULongAndCurrency() =>
        Assert.That(1000UL / (Currency)100UL, Is.EqualTo(10));

}
