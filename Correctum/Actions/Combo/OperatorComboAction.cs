using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Correctum.Utils;
using Microsoft.EntityFrameworkCore;

namespace Correctum.Actions.Combo;

[ActionInfo("Смена кассира")]
[CheckBox("Старое значение отсутствует?", nameof(IsNullOldName))]
[TextBox("Найти кассира", nameof(OldName))]
[TextBox("Найти ИНН кассира", nameof(OldInn), "innConverter")]
[DateRange("Период изменения", nameof(DateRange))]
[CheckBox("Учитывать отсутствие даты, как вне периода изменения", nameof(MissingDateAsOutsidePeriod))]
[TextBox("Кассира заменить на", nameof(NewName))]
[TextBox("ИНН кассира заменить на", nameof(NewInn), "innConverter")]
public sealed class OperatorComboAction : IAction
{
    public bool IsNullOldName { get; set; }
    public string OldName { get; set; } = string.Empty;
    public Inn? OldInn { get; set; }
    
    public string NewName { get; set; } = string.Empty;
    public Inn? NewInn { get; set; }

    public DateRange DateRange { get; } = new();
    public bool MissingDateAsOutsidePeriod { get; set; } = true;

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
        // Шаг 2. Удаление чеков без ошибочного кассира
        await receipts
            .Where(r => 
                r.OperatorName != OldName && 
                r.OperatorInn.ToString() != OldInn.ToString())
            .ExecuteDeleteAsync();
        // Шаг 3. Генерация аннулирующих ЧК
        await new CancelReceiptCloner().DoAsync(receipts);
        // Шаг 4. Исправление
        await receipts
            .Where(r => !r.IsReadOnly)
            .ExecuteUpdateAsync(b =>
            {
                b.SetProperty(r => r.OperatorName, NewName);
                b.SetProperty(r => r.OperatorInn, NewInn);
            });
    }
}
