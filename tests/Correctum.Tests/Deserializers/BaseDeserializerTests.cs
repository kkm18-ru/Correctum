using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Deserializers;
using Correctum.Tests.Utils.Extensions;
using static Correctum.Tests.Utils.BogusHelper;

namespace Correctum.Tests.Deserializers;

public abstract class BaseDeserializerTests<T>
    where T : IDeserializer
{
    protected abstract string GenerateText(Receipt receipt);

    [Test]
    public void Deserialize_Should_ReturnReceipt()
    {
        for (var i = 0; i < 100; i++)
        {
            var exceptReceipt = GenerateSourceReceipt();
            var text = GenerateText(exceptReceipt);
            var receipt = T.Deserialize(text);
            CheckReceipt(receipt, exceptReceipt);
        }
    }

    protected static Receipt GenerateSourceReceipt() => new()
    {
        RegId = SharedFaker.Random.ULong(0, 9999999999999999).ToString("D16"),
        StorageId = SharedFaker.Random.ULong(0, 9999999999999999).ToString("D16"),
        BuyerContact = SharedFaker.Internet.Email(),
        Created = SharedFaker.Date.Past().Truncate(TimeSpan.TicksPerSecond),
        DocNumber = SharedFaker.Random.UInt(),
        SessionId = SharedFaker.Random.UInt(),
        DocInSession = SharedFaker.Random.UInt(),
        DocSign = SharedFaker.Random.UInt(),
        Payment = new()
        {
            CashSum = SharedFaker.Random.ULong(),
            NonCashSum = SharedFaker.Random.ULong(),
            AdvanceSum = SharedFaker.Random.ULong(),
            CreditSum = SharedFaker.Random.ULong(),
            ProvisionSum = SharedFaker.Random.ULong(),
        },
        TotalSum = SharedFaker.Random.ULong(),
        TaxationSystem = SharedFaker.Random.Enum<TaxationSystems>(),
        Type = SharedFaker.Random.Enum<ReceiptType>(),
        Items = GenerateSourcePositions()
    };

    protected static Position[] GenerateSourcePositions()
    {
        var arr = new Position[SharedFaker.Random.Int(1,5)];
        for (var i = 0; i < arr.Length; i++)
            arr[i] = GenerateSourcePosition();
        return arr;
    }
    protected static Position GenerateSourcePosition() => new()
    {
        Name = SharedFaker.Commerce.ProductName(),
        Price = SharedFaker.Random.ULong(),
        Quantity = SharedFaker.Random.ULong(),
        Cost = SharedFaker.Random.ULong(),
        CalcMethod = SharedFaker.Random.Enum<CalculationMethod>(),
        Unit = SharedFaker.Random.Enum<UnitOfMeasure>(),
        Type = SharedFaker.Random.Enum<PositionType>(),
        TaxRate = SharedFaker.Random.Enum(NDSRate.NDS118, NDSRate.NDS18, NDSRate.NDS20, NDSRate.NDS120),
    };

    protected static void CheckReceipt(Receipt actual, Receipt excepted)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.RegId, Is.EqualTo(excepted.RegId));
            Assert.That(actual.StorageId, Is.EqualTo(excepted.StorageId));
            Assert.That(actual.DocNumber, Is.EqualTo(excepted.DocNumber));
            Assert.That(actual.SessionId, Is.EqualTo(excepted.SessionId));
            Assert.That(actual.DocInSession, Is.EqualTo(excepted.DocInSession));
            Assert.That(actual.Created, Is.EqualTo(excepted.Created));
            Assert.That(actual.DocSign, Is.EqualTo(excepted.DocSign));
            Assert.That(actual.Type, Is.EqualTo(excepted.Type));
            Assert.That(actual.Payment, Is.EqualTo(excepted.Payment));
            Assert.That(actual.TotalSum, Is.EqualTo(excepted.TotalSum));
            Assert.That(actual.BuyerContact, Is.EqualTo(excepted.BuyerContact));
            Assert.That(actual.TaxationSystem, Is.EqualTo(excepted.TaxationSystem));
            Assert.That(actual.Items, Is.Not.Null);
            Assert.That(actual.Items!, Has.Length.EqualTo(excepted.Items!.Length));
            for (var p = 0; p < actual.Items!.Length; p++)
                CheckPosition(actual.Items[p], excepted.Items![p]);
        }
    }

    protected static void CheckPosition(Position actual, Position excepted)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Name, Is.EqualTo(excepted.Name));
            Assert.That(actual.Type, Is.EqualTo(excepted.Type));
            Assert.That(actual.CalcMethod, Is.EqualTo(excepted.CalcMethod));
            Assert.That(actual.TaxRate, Is.EqualTo(excepted.TaxRate));
            Assert.That(actual.Unit, Is.EqualTo(excepted.Unit));
            Assert.That(actual.Price, Is.EqualTo(excepted.Price));
            Assert.That(actual.Quantity, Is.EqualTo(excepted.Quantity));
            Assert.That(actual.Cost, Is.EqualTo(excepted.Cost));
        }
    }
}
