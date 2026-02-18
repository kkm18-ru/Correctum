namespace RetailCorrector.Tests.Utils;

public class FiscalHelper
{
    private static IEnumerable<TestCaseData<string, uint>> FiscalSigns()
    {
        yield return new TestCaseData<string, uint>("MQRK2fVD", 1255798083);
        yield return new TestCaseData<string, uint>("MQQNRjQj", 222704675);
        yield return new TestCaseData<string, uint>("MQRrYFLO", 1801474766);
        yield return new TestCaseData<string, uint>("MQQps4Vo", 699630952);
        yield return new TestCaseData<string, uint>("MQTVuyvD", 3585813443);
        yield return new TestCaseData<string, uint>("MQRV8ptE", 1441962820);
        yield return new TestCaseData<string, uint>("MQRKTM7H", 1246547655);
        yield return new TestCaseData<string, uint>("MQT3vKkf", 4156336415);
        yield return new TestCaseData<string, uint>("MQSWphCp", 2527465641);
        yield return new TestCaseData<string, uint>("MQR2+bjq", 1996077290);
        yield return new TestCaseData<string, uint>("MQTIHncn", 3357439783);
        yield return new TestCaseData<string, uint>("MQSm5f1X", 2800090455);
        yield return new TestCaseData<string, uint>("MQR1K0rm", 1965771494);
        yield return new TestCaseData<string, uint>("MQRr4BmD", 1809848707);
        yield return new TestCaseData<string, uint>("MQQiRBSj", 574887075);
    }

    [TestCaseSource(nameof(FiscalSigns))]
    public void ToDigitSign(string sign, uint expected) =>
        Assert.That(RetailCorrector.Utils.FiscalHelper.ToDecimalSign(sign), Is.EqualTo(expected));

    [TestCaseSource(nameof(FiscalSigns))]
    public void ToTextSign(string expected, uint sign) =>
        Assert.That(RetailCorrector.Utils.FiscalHelper.ToStringSign(sign), Is.EqualTo(expected));
}