using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Models;
using Correctum.Utils;
using Correctum.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace Correctum.Actions.Combo;

[ActionInfo("Смена метода оплаты")]
[ComboBox("Старое значение", nameof(OldValue), nameof(PaymentMethods))]
[DateRange("Период изменения", nameof(DateRange))]
[ComboBox("Новое значение", nameof(NewValue), nameof(PaymentMethods))]
public sealed class SimplePaymentComboAction : IAction
{
    public string OldValue { get; set; } = PaymentMethods[0];
    public string NewValue { get; set; } = PaymentMethods[0];
    public DateRange DateRange { get; } = new();

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
            .Where(r => r.Created > DateRange.EndDateTime || r.Created < DateRange.BeginDateTime)
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
        "Наличная оплата" => r => r.CashSum == Currency.Zero,
        "Безналичная оплата" => r => r.NonCashSum == Currency.Zero,
        "Оплата аванса" => r => r.AdvanceSum == Currency.Zero,
        "Оплата кредита" => r => r.CreditSum == Currency.Zero,
        "Встречное предоставление" => r => r.ProvisionSum == Currency.Zero,
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