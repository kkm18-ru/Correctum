using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Correctum.Utils;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace Correctum.Actions.Combo;

[ActionInfo("Смена метода оплаты")]
[ComboBox("Старое значение", nameof(OldValue), nameof(PaymentMethods))]
[DateRange("Период изменения", nameof(DateRange))]
[CheckBox("Учитывать отсутствие даты, как вне периода изменения", nameof(MissingDateAsOutsidePeriod))]
[ComboBox("Новое значение", nameof(NewValue), nameof(PaymentMethods))]
public sealed class SimplePaymentComboAction : IAction
{
    public string OldValue { get; set; } = PaymentMethods[0];
    public string NewValue { get; set; } = PaymentMethods[0];
    public DateRange DateRange { get; } = new();
    public bool MissingDateAsOutsidePeriod { get; set; } = true;

    public static ImmutableArray<string> PaymentMethods { get; } = [
        "Наличная оплата",
        "Безналичная оплата",
        "Оплата аванса",
        "Оплата кредита",
        "Встречное предоставление",
    ];

    public async Task DoAsync(DbSet<Receipt> receipts)
    {
        // Шаг 1. Удаление чеков вне периода
        await receipts
            .Where(r =>
                (!r.Created.HasValue && MissingDateAsOutsidePeriod) ||
                (r.Created.HasValue &&
                    (r.Created > DateRange.EndDateTime ||
                    r.Created < DateRange.BeginDateTime)))
            .ExecuteDeleteAsync();
        // Шаг 2. Удаление лишних чеков
        await receipts.Where(GetDeleteConditionExpression()).ExecuteDeleteAsync();
        // Шаг 3. Генерация аннулирующих ЧК
        await new CancelReceiptCloner().DoAsync(receipts);
        // Шаг 4. Исправление
        await receipts
            .Where(r => !r.IsReadOnly)
            .ExecuteUpdateAsync(b =>
            {
                b.SetProperty(GetPropertyExpression(NewValue), GetPropertyExpression(OldValue));
                b.SetProperty(GetPropertyExpression(OldValue), Currency.Zero);
            });
    }

    private Expression<Func<Receipt, bool>> GetDeleteConditionExpression() => OldValue switch
    {
        "Наличная оплата" => r => r.CashSum == null || (ulong)r.CashSum.Value == 0,
        "Безналичная оплата" => r => r.NonCashSum == null || (ulong)r.NonCashSum.Value == 0,
        "Оплата аванса" => r => r.AdvanceSum == null || (ulong)r.AdvanceSum.Value == 0,
        "Оплата кредита" => r => r.CreditSum == null || (ulong)r.CreditSum.Value == 0,
        "Встречное предоставление" => r => r.ProvisionSum == null || (ulong)r.ProvisionSum.Value == 0,
        _ => throw new NotImplementedException()
    };

    private static Expression<Func<Receipt, Currency?>> GetPropertyExpression(string value) => value switch
    {
        "Наличная оплата" => r => r.CashSum,
        "Безналичная оплата" => r => r.NonCashSum,
        "Оплата аванса" => r => r.AdvanceSum,
        "Оплата кредита" => r => r.CreditSum,
        "Встречное предоставление" => r => r.ProvisionSum,
        _ => throw new NotImplementedException()
    };
}