using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Enums;
using Correctum.Models;
using System.Linq.Expressions;

namespace Correctum.Actions;

//[ActionInfo("Генерация аннулирующих чеков")]
public sealed class CancelReceiptCloner : BaseCloner
{
    public override Expression<Func<Receipt, bool>> Filter => _ => true;

    public override void Change(Receipt receipt)
    {
        receipt.IsReadOnly = true;
        receipt.Type = receipt.Type switch
        {
            ReceiptType.Income => ReceiptType.RefundIncome,
            ReceiptType.RefundIncome => ReceiptType.Income,
            ReceiptType.Outcome => ReceiptType.RefundOutcome,
            ReceiptType.RefundOutcome => ReceiptType.Outcome,
            ReceiptType type => throw new InvalidOperationException
                ($"Неожиданное значение типа операции чека [{type}]"),
            _ => null
        };
    }
}
