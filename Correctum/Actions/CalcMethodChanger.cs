using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Enums;
using Correctum.Core.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Correctum.Actions;

//[ActionInfo("Смена метода расчета")]
[CheckBox("Старое значение отсутствует", nameof(IsNullOldValue))]
[CheckBox("Значение будет отсутствовать", nameof(IsNullNewValue))]
[LocalizedComboBox("Найти", nameof(OldValue), nameof(Utils.EnumLocales.PositionPayments))]
[LocalizedComboBox("Заменить на", nameof(NewValue), nameof(Utils.EnumLocales.PositionPayments))]
public sealed class CalcMethodChanger : BasePositionChanger
{
    public bool IsNullOldValue { get; set; }
    public bool IsNullNewValue { get; set; }
    public CalculationMethod OldValue { get; set; }
    public CalculationMethod NewValue { get; set; }
    public CalculationMethod[] CalculationMethods { get; } = Enum.GetValues<CalculationMethod>();

    public override Expression<Func<Position, bool>> Filter =>
        position => position.CalcMethod == (IsNullOldValue ? null : OldValue);

    public override void Update(UpdateSettersBuilder<Position> builder) =>
        builder.SetProperty(p => p.CalcMethod, NewValue);
}
