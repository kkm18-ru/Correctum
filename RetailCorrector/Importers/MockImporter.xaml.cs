using System.Runtime.CompilerServices;
using Bogus;
using RetailCorrector.Attributes;
using RetailCorrector.Data;
using RetailCorrector.Enums;
using RetailCorrector.UI.Controls;
using RetailCorrector.Utils;

namespace RetailCorrector.Importers;

[ImporterName("Mock")]
public sealed partial class MockImporter : ImporterBase
{
    public int Minimum { get; set => SetField(ref field, value); } = 1;
    public int Maximum { get; set => SetField(ref field, value); } = 1000;

    public override bool CanStartClick => true;
    
    public MockImporter()
    {
        InitializeComponent();
    }

    public override async IAsyncEnumerable<Receipt> ImportAsync(VisualProgress progress, Guid sessionId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var faker = new Faker();
        var count = faker.Random.Int(Minimum, Maximum);
        progress.ReportMaxValue(count);
        var i = 0;
        while (i < count)
        {
            await Task.Delay(faker.Random.Int(1, 1000), cancellationToken);
            var posCount = faker.Random.UInt(1, 10);
            var pos = new Position[posCount];
            ulong sum = 0;
            for (var j = 0; j < pos.Length; j++)
            {
                pos[j] = GeneratePosition(faker);
                sum += pos[j].Cost;
            }
            yield return new()
            {
                Items = pos,
                RegId = faker.Random.ULong(0, 9999_9999_9999_9999).ToString("D16"),
                StorageId = faker.Random.ULong(0, 9999_9999_9999_9999).ToString("D16"),
                DocNumber = faker.Random.UInt(1, 100000),
                DocInSession = faker.Random.UInt(1, 1000),
                SessionId = faker.Random.UInt(1, 365),
                Created = faker.Date.Past(),
                DocSign = faker.Random.UInt(),
                Type = faker.Random.Enum<Operation>(),
                Payment = GeneratePayment(faker, sum),
                TotalSum = sum,
                ImportSessionId = sessionId,
                TaxationSystem = faker.Random.Enum<TaxationSystem>()
            };
            progress.ReportCurrValue(++i);
        }
    }

    private static Position GeneratePosition(Faker faker)
    {
        var position = new Position
        {
            Name = faker.Commerce.ProductName(),
            Price = faker.Random.ULong(100, 9999999),
            Quantity = 1000 * faker.Random.UInt(1, 10),
            Unit = faker.Random.Enum<PositionUnit>(),
            PayMethod = faker.Random.Enum<PayMethod>(),
            TaxRate = faker.Random.Enum<TaxRate>(),
            Type = faker.Random.Enum<PositionType>(),
        };
        position.Cost = position.Price * (position.Quantity / 1000);
        return position;
    }

    private static Payment GeneratePayment(Faker faker, ulong total)
    {
        var payment = new Payment();
        switch (faker.Random.Byte(0, 4))
        {
            case 0:
                payment.Cash = total;
                break;
            case 1:
                payment.ECash = total;
                break;
            case 2:
                payment.Pre = total;
                break;
            case 3:
                payment.Post = total;
                break;
            case 4:
                payment.Provision = total;
                break;
        }

        return payment;
    }
}