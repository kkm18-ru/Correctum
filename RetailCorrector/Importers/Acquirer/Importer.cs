using RetailCorrector.Data;
using RetailCorrector.Enums;
using System.IO;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Importers;

internal sealed partial class Acquirer : ImporterBase<Acquirer, Acquirer.View>
{
    public override string Name => "Генерация по эквайрингу";

    public override ImportSession GenerateInfo() =>
        parseInfo = new ImportSession(Name);

    public override async IAsyncEnumerable<Receipt> Import
        (IProgress<long> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var lines = File.ReadAllLines(FilePath);
        for (var i = (HasHeaders ? 1u : 0); i < lines.Length; i++)
        {
            var columns = lines[i].Split(Separator);
            var sum = (ulong)(double.Parse(columns[TotalReceipt]) * 100);
            var operation = columns[OperationReceipt].Trim('"');
            var receipt = new Receipt
            {
                ParseId = parseInfo.Id,
                DocInSession = 0,
                RegId = 0.ToString("D16"),
                StorageId = 0.ToString("D16"),
                SessionId = 0,
                EditsCount = 0,
                DocNumber = i,
                Created = DateTime.ParseExact(columns[DateReceipt].Trim('"'), DateFormat, null),
                TotalSum = sum,
                BuyerContact = columns[3],
                Type = operation == IncomePattern ? Operation.Income :
                    operation == RefundPattern ? Operation.RefundIncome : 0,
                Payment = new Payment { ECash = sum },
                TaxationSystem = 0,
                DocSign = 0
            };
            receipt.Items =
            [
                new Position
                {
                    Name = ItemName,
                    Price = sum,
                    Quantity = 1000,
                    Cost = sum,
                    TaxRate = ItemVat,
                    MeasureUnit = MeasureUnit.Default,
                    PayMethod = PayMethod.FullPayment,
                    Type = PositionType.Product,
                    Receipt = receipt 
                }
            ];
            yield return receipt;
        }
        yield break;
    }
}