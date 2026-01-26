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
            var receipt = new Receipt
            {
                Items = new Position[faker.Random.UInt(1, 10)],
                RegId = faker.Random.ULong(0, 9999_9999_9999_9999).ToString("D16"),
                StorageId = faker.Random.ULong(0, 9999_9999_9999_9999).ToString("D16"),
                DocNumber = faker.Random.UInt(1,100000),
                DocInSession = faker.Random.UInt(1,1000),
                SessionId = faker.Random.UInt(1,365),
                Created = faker.Date.Past(),
                DocSign = faker.Random.UInt(),
                Type = faker.Random.Enum<Operation>(),
                ParseId = parseInfo.Id,
                TaxationSystem = faker.Random.Enum<TaxationSystem>()
            };
            for(var j = 0; j < receipt.Items.Length; j++)
            {
                var price = faker.Random.ULong(100, 9999999);
                var quantity = 1000 * faker.Random.UInt(1, 10);
                receipt.Items[j] = new Position
                {
                    Receipt = receipt,
                    Name = faker.Commerce.ProductName(),
                    Price = price,
                    Quantity = quantity,
                    Cost = price * quantity / 1000,
                    MeasureUnit = faker.Random.Enum<MeasureUnit>(),
                    PayMethod = faker.Random.Enum<PayMethod>(),
                    TaxRate = faker.Random.Enum<TaxRate>(),
                    Type = faker.Random.Enum<PositionType>(),
                };
                receipt.TotalSum += price * quantity / 1000;
            }
            var payment = new Payment();
            switch(faker.Random.Byte(0, 4))
            {
                case 0:
                    payment.Cash = receipt.TotalSum;
                    break;
                case 1:
                    payment.ECash = receipt.TotalSum;
                    break;
                case 2:
                    payment.Pre = receipt.TotalSum;
                    break;
                case 3:
                    payment.Post = receipt.TotalSum;
                    break;
                case 4:
                    payment.Provision = receipt.TotalSum;
                    break;
            }
            yield return receipt;
        }
    }
}
#endif
