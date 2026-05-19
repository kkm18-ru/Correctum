using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Enums;
using Correctum.Core.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Correctum.Actions;

//[ActionInfo("Смена меры измерения")]
[CheckBox("Старое значение отсутствует", nameof(IsNullOldValue))]
[CheckBox("Значение будет отсутствовать", nameof(IsNullNewValue))]
[LocalizedComboBox("Найти", nameof(OldValue), nameof(Utils.EnumLocales.PositionUnits))]
[LocalizedComboBox("Заменить на", nameof(NewValue), nameof(Utils.EnumLocales.PositionUnits))]
public sealed class UnitOfMeasureChanger : BasePositionChanger
{
    public bool IsNullOldValue { get; set; }
    public bool IsNullNewValue { get; set; }
    public UnitOfMeasure OldValue { get; set; }
    public UnitOfMeasure NewValue { get; set; }

    public override Expression<Func<Position, bool>> Filter =>
        position => position.Unit == (IsNullOldValue ? null : OldValue);

    public override void Update(UpdateSettersBuilder<Position> builder) =>
        builder.SetProperty(p => p.Unit, NewValue);
}
