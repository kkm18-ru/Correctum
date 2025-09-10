#pragma warning disable CS1998
using RetailCorrector.Data;
using RetailCorrector.Enums;
using System.Runtime.CompilerServices;
using Bogus;
using RetailCorrector.Attributes;

namespace RetailCorrector.Parsers
{
    [RegisterParser("Рандомизатор чеков", true)]
    internal partial class MockParser : ParserBase<MockParser>
    {
        public override ParseInfo GenerateInfo() =>
            parseInfo = new ParseInfo("Mock");

        public override async IAsyncEnumerable<Receipt> Parse(IProgress<long> progress, CancellationToken cancellationToken)
        {
            progress.Report(-1);            
            var faker = new Faker();
            var count = faker.Random.UInt(1, 1000);
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
                    Created = faker.Date.Past(),
                    DocSign = faker.Random.ULong(10000000, 99999999999),
                    Type = faker.Random.Enum<Operation>(),
                    Payment = payment,
                    TotalSum = sum,
                    ParseId = parseInfo.Id
                };
            }
            progress.Report(1);
        }
    }
}
