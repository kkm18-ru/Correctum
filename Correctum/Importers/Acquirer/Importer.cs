using Correctum.Core.Models;
using Correctum.Core.Enums;
using Correctum.Utils;
using System.IO;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace Correctum.Importers;

[Obsolete("#74")]
internal sealed partial class Acquirer : ImporterBase<Acquirer, Acquirer.View>
{
    public override string Name => "Генерация по эквайрингу";

    public override async IAsyncEnumerable<Receipt> Import
        (BoundedProgress<uint> progress, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var lines = await File.ReadAllLinesAsync(FilePath, cancellationToken);
        for (var i = (HasHeaders ? 1u : 0); i < lines.Length; i++)
        {
            var columns = lines[i].Split(Separator);
            var sum = (ulong)(double.Parse(columns[TotalReceipt]) * 100);
            var operation = columns[OperationReceipt].Trim('"');
            ReceiptType? type = null;
            if (operation == IncomePattern) type = ReceiptType.Income;
            else if (operation == RefundPattern) type = ReceiptType.RefundIncome;
            yield return new Receipt
            {
                DocInSession = 0,
                RegId = 0.ToString("D16"),
                StorageId = 0.ToString("D16"),
                SessionId = 0,
                DocNumber = i,
                Created = DateTime.ParseExact(columns[DateReceipt].Trim('"'), DateFormat, CultureInfo.InvariantCulture),
                TotalSum = sum,
                BuyerContact = columns[3],
                Type = type,
                Items = [
                    new Position
                    {
                        Name = ItemName,
                        Price = sum,
                        Quantity = 1,
                        Cost = sum,
                        TaxRate = ItemVat,
                        Unit = UnitOfMeasure.Piece,
                        CalcMethod = CalculationMethod.FullPayment,
                        Type = PositionType.Product
                    }
                ],
                Payment = new Payment { NonCashSum = sum },
                TaxationSystem = 0,
                DocSign = null
            };
        }
        yield break;
    }
}