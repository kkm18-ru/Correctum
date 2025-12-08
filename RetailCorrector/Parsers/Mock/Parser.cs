#pragma warning disable CS1998
using RetailCorrector.Data;
using RetailCorrector.Enums;
using System.Runtime.CompilerServices;
using Bogus;

namespace RetailCorrector.Parsers
{
    internal partial class Mock : ParserBase<Mock, Mock.View>
    {
        public override string Name => "Mock [для ОТЛАДКИ]";

        public override ParseInfo GenerateInfo() =>
            parseInfo = new ParseInfo("Mock");

        public override async IAsyncEnumerable<Receipt> Parse(IProgress<long> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
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
                yield return new()
                {
                    Items = pos,
                    Created = faker.Date.Past(),
                    DocSign = faker.Random.ULong(10000000, 99999999999),
                    Type = faker.Random.Enum<Operation>(),
                    PaymentSums = [new PaymentSum(faker.Random.Enum<PaymentType>(), sum)],
                    TotalSum = sum,
                    ParseId = parseInfo.Id,
                    TaxationSystem = faker.Random.Enum<TaxationSystem>()
                };
            }
        }
    }
}
