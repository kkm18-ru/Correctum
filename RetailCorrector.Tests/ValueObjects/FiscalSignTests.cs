using RetailCorrector.ValueObjects;

namespace RetailCorrector.Tests.ValueObjects;

public sealed class FiscalSignTests
{
    [Test] public void Parse_Should_ThrowException()
    {
        var ex = Assert.Throws<FormatException>(() => FiscalSign.Parse((string?)null));
        Assert.That(ex.Message, Is.EqualTo("Ввод не является, ни TLV-, ни целочисленным представлением ФПД."));
    }

    [Test] public void TryParse_Digits_Should_ReturnTrue()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(FiscalSign.TryParse("1", out var sign), Is.True);
            Assert.That(sign, Is.EqualTo(new FiscalSign(1)));
        }
    }

    [Test] public void TryParse_Tlv_Should_ReturnTrue()
    {
        using (Assert.EnterMultipleScope())
        {
            foreach (var s in TlvFiscalSigns())
            {
                Assert.That(FiscalSign.TryParse(s.Item1, out var sign), Is.True);
                Assert.That(sign, Is.EqualTo(new FiscalSign(s.Item2)));
            }
        }
    }

    [Test] public void TryParse_8Characters_Should_ReturnFalse() =>
        Assert.That(FiscalSign.TryParse("(123456)", out _), Is.False);

    [Test] public void TryParse_7Characters_Should_ReturnFalse() =>
        Assert.That(FiscalSign.TryParse("(12345)", out _), Is.False);

    private static IEnumerable<(string, uint)> TlvFiscalSigns()
    {
        yield return ("MQRK2fVD", 1255798083);
        yield return ("MQQNRjQj", 222704675);
        yield return ("MQRrYFLO", 1801474766);
        yield return ("MQQps4Vo", 699630952);
        yield return ("MQTVuyvD", 3585813443);
        yield return ("MQRV8ptE", 1441962820);
        yield return ("MQRKTM7H", 1246547655);
        yield return ("MQT3vKkf", 4156336415);
        yield return ("MQSWphCp", 2527465641);
        yield return ("MQR2+bjq", 1996077290);
        yield return ("MQTIHncn", 3357439783);
        yield return ("MQSm5f1X", 2800090455);
        yield return ("MQR1K0rm", 1965771494);
        yield return ("MQRr4BmD", 1809848707);
        yield return ("MQQiRBSj", 574887075);
    }
}
