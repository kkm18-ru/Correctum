#pragma warning disable CS1998
#if DEBUG
using RetailCorrector.Data;
using RetailCorrector.Enums;
using System.Runtime.CompilerServices;
using Bogus;

namespace RetailCorrector.Importers;

internal partial class Mock : ImporterBase<Mock, Mock.View>
{
    public override string Name => "Mock [для ОТЛАДКИ]";

    public override ImportSession GenerateInfo() =>
        parseInfo = new ImportSession("Mock");

    public override async IAsyncEnumerable<Receipt> Import(IProgress<long> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var faker = new Faker();
        var count = faker.Random.Int(MinCountDocs, MaxCountDocs);
        for(var i = 0; i < count; i++)
        {
            var posCount = faker.Random.UInt(1, 10);
            var pos = new Position[posCount];
            ulong sum = 0;
            for(var j = 0; j < pos.Length; j++)
            {
                pos[j] = new()
                {
                    Name = faker.Commerce.ProductName(),
                    Price = faker.Random.ULong(100,9999999),
                    Quantity = 1000 * faker.Random.UInt(1,10),
                    MeasureUnit = faker.Random.Enum<MeasureUnit>(),
                    PayMethod = faker.Random.Enum<PayMethod>(),
                    TaxRate = faker.Random.Enum<TaxRate>(),
                    Type = faker.Random.Enum<PositionType>(),
                };
                pos[j].Cost = pos[j].Price * (pos[j].Quantity / 1000);
                sum += pos[j].Cost;
            }
            var payment = new Payment();
            switch(faker.Random.Byte(0, 4))
            {
                case 0:
                    payment.Cash = sum;
                    break;
                case 1:
                    payment.ECash = sum;
                    break;
                case 2:
                    payment.Pre = sum;
                    break;
                case 3:
                    payment.Post = sum;
                    break;
                case 4:
                    payment.Provision = sum;
                    break;
            }
            yield return new()
            {
                Items = pos,
                RegId = faker.Random.ULong(0, 9999_9999_9999_9999).ToString("D16"),
                StorageId = faker.Random.ULong(0, 9999_9999_9999_9999).ToString("D16"),
                DocNumber = faker.Random.UInt(1,100000),
                DocInSession = faker.Random.UInt(1,1000),
                SessionId = faker.Random.UInt(1,365),
                Created = faker.Date.Past(),
                DocSign = faker.Random.UInt(),
                Type = faker.Random.Enum<Operation>(),
                Payment = payment,
                TotalSum = sum,
                ParseId = parseInfo.Id,
                TaxationSystem = faker.Random.Enum<TaxationSystem>()
            };
        }
    }
}
#endif
