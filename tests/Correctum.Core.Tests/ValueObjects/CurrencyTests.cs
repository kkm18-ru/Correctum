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
}
