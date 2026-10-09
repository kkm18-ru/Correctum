using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Enums;
using Correctum.Models;
using Correctum.Utils;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Correctum.Actions.Combo;

[ActionInfo("Смена способа расчета")]
[CheckComboBox("Старые значения", nameof(OldValues), nameof(CalculationMethods))]
[DateRange("Период изменения", nameof(DateRange))]
[LocalizedComboBox("Новое значение", nameof(NewValue), nameof(EnumLocales.PositionPayments))]
public sealed class CalculationMethodComboAction : IAction
{
    public ObservableCollection<CalculationMethod?> OldValues { get; } = [];
    public CalculationMethod NewValue { get; set; }
    public DateRange DateRange { get; } = new();

    public ImmutableArray<CalculationMethod?> CalculationMethods { get; } = [null, .. Enum.GetValues<CalculationMethod>()];
    public async Task DoAsync(DbSet<Receipt> receipts)
    {
        // Шаг 1. Удаление чеков вне периода
        await receipts
            .Where(r => r.Created > DateRange.EndDateTime || r.Created < DateRange.BeginDateTime)
            .ExecuteDeleteAsync();
        // Шаг 2. Удаление чеков без позиций с одной из ошибочных способов расчета
        await receipts.Where(r => r.Items.All(i => !OldValues.Contains(i.CalcMethod))).ExecuteDeleteAsync();
        // Шаг 3. Генерация аннулирующих ЧК
        await new CancelReceiptCloner().DoAsync(receipts);
        // Шаг 4. Исправление
        await receipts
            .Where(r => !r.IsReadOnly)
            .SelectMany(r => r.Items)
            .Where(i => OldValues.Contains(i.CalcMethod))
            .ExecuteUpdateAsync(b => b.SetProperty(i => i.CalcMethod, NewValue));
    }
}
