#pragma warning disable CS1998
#if DEBUG
using Bogus;
using Correctum.Core.Models;
using Correctum.Core.Enums;
using RetailCorrector.Utils;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Importers;

internal partial class Mock : ImporterBase<Mock, Mock.View>
{
    public override string Name => "Mock [для ОТЛАДКИ]";

    public override async IAsyncEnumerable<Receipt> Import(BoundedProgress<uint> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var faker = new Faker();
        var count = faker.Random.UInt((uint)MinCountDocs, (uint)MaxCountDocs);
        progress.ReportMaximum(count);
        for(var i = 0; i < count; i++)
        {
            var posCount = faker.Random.UInt(1, 10);
            var pos = new Position[posCount];
            ulong sum = 0;
            for(var j = 0; j < pos.Length; j++)
            {
                var quantity = Math.Round(faker.Random.Decimal(1, 10), 3);
                var price = faker.Random.ULong(100, 9999999);
                pos[j] = new()
                {
                    Name = faker.Commerce.ProductName(),
                    Price = price,
                    Quantity = quantity,
                    Cost = price * quantity,
                    Unit = faker.Random.Enum<UnitOfMeasure>(),
                    CalcMethod = faker.Random.Enum<CalculationMethod>(),
                    TaxRate = faker.Random.Enum<NDSRate>(),
                    Type = faker.Random.Enum<PositionType>(),
                };
                sum += (ulong)pos[j].Cost;
            }
            var payType = faker.Random.Byte(0, 4);
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
                Type = faker.Random.Enum<ReceiptType>(),
                CashSum = payType == 0 ? sum : 0,
                NonCashSum = payType == 1 ? sum : 0,
                AdvanceSum = payType == 2 ? sum : 0,
                CreditSum = payType == 3 ? sum : 0,
                ProvisionSum = payType == 4 ? sum : 0,
                TotalSum = sum,
                TaxationSystem = faker.Random.Enum<TaxationSystems>()
            };
            await Task.Delay(50, cancellationToken);
            progress.ReportIncrement();
        }
    }
}
#endif
