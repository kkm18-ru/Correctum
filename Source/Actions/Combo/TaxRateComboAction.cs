using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Utils;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Correctum.Actions.Combo;

[ActionInfo("Смена ставки НДС")]
[CheckComboBox("Старые значения", nameof(OldValues), nameof(TaxRates))]
[DateRange("Период изменения", nameof(DateRange))]
[CheckBox("Учитывать отсутствие даты, как вне периода изменения", nameof(MissingDateAsOutsidePeriod))]
[LocalizedComboBox("Новое значение", nameof(NewValue), nameof(EnumLocales.TaxRates))]
public sealed class TaxRateComboAction : IAction
{
    public ObservableCollection<NDSRate?> OldValues { get; } = [];
    public NDSRate NewValue { get; set; }
    public DateRange DateRange { get; } = new();
    public bool MissingDateAsOutsidePeriod { get; set; } = true;

    public ImmutableArray<NDSRate?> TaxRates { get; } = [null, ..Enum.GetValues<NDSRate>()];

    private bool IsOutPeriod(Receipt r) => (!r.Created.HasValue && MissingDateAsOutsidePeriod) ||
        (r.Created.HasValue && (r.Created > DateRange.EndDateTime || r.Created < DateRange.BeginDateTime));
    private bool HasNotError(Receipt r) => r.Items.All(i => !OldValues.Contains(i.TaxRate));
    private bool IsToCorrection(Receipt r) => !IsOutPeriod(r) && !HasNotError(r);

    public async Task DoAsync(DbSet<Receipt> receipts)
    {
        // Шаг 1. Удаление чеков вне периода
        foreach (var receipt in receipts.Where(IsOutPeriod))
            receipts.Remove(receipt);
        // Шаг 2. Удаление чеков без позиций с одной из ошибочных ставок НДС
        foreach (var receipt in receipts.Where(HasNotError))
            receipts.Remove(receipt);
        // Шаг 3. Генерация аннулирующих ЧК
        foreach(var r in receipts.Where(IsToCorrection))
            receipts.Add(r.CloneAsReversed());
        // Шаг 4. Исправление
        foreach(var receipt in receipts.Where(r => !r.IsReadOnly))
            foreach(var item in receipt.Items.Where(i => OldValues.Contains(i.TaxRate)))
                item.TaxRate = NewValue;
    }
}
