using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Enums;
using Correctum.Core.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Correctum.Actions;

//[ActionInfo("Смена ставки НДС")]
[CheckBox("Старое значение отсутствует", nameof(IsNullOldValue))]
[CheckBox("Значение будет отсутствовать", nameof(IsNullNewValue))]
[LocalizedComboBox("Найти", nameof(OldValue), nameof(Utils.EnumLocales.TaxRates))]
[LocalizedComboBox("Заменить на", nameof(NewValue), nameof(Utils.EnumLocales.TaxRates))]
public sealed class TaxRateChanger : BasePositionChanger
{
    public bool IsNullOldValue { get; set; }
    public bool IsNullNewValue { get; set; }
    public NDSRate OldValue { get; set; }
    public NDSRate NewValue { get; set; }
    public NDSRate[] TaxRates { get; } = Enum.GetValues<NDSRate>();

    public override Expression<Func<Position, bool>> Filter =>
        position => position.TaxRate == (IsNullOldValue ? null : OldValue);

    public override void Update(UpdateSettersBuilder<Position> builder) =>
        builder.SetProperty(p => p.TaxRate, NewValue);
}
