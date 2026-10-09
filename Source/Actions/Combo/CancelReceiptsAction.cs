using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Enums;
using Correctum.Models;
using Microsoft.EntityFrameworkCore;

namespace Correctum.Actions.Combo;

[ActionInfo("Аннулировать")]
internal class CancelReceiptsAction : IAction
{
    public async Task DoAsync(DbSet<Receipt> receipts)
    {
        foreach(var r in receipts)
        {
            r.Type = r.Type switch
            {
                ReceiptType.RefundOutcome => ReceiptType.Outcome,
                ReceiptType.Outcome => ReceiptType.RefundOutcome,
                ReceiptType.RefundIncome => ReceiptType.Income,
                ReceiptType.Income => ReceiptType.RefundIncome,
                _ => throw new InvalidOperationException("Неизвестный тип чека")
            };
        }
    }
}
