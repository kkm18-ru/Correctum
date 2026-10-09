using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Correctum.Actions;

//[ActionInfo("Смена места расчета")]
[CheckBox("Старое значение отсутствует", nameof(IsNullOldValue))]
[CheckBox("Значение будет отсутствовать", nameof(IsNullNewValue))]
[TextBox("Найти", nameof(OldValue))]
[TextBox("Заменить на", nameof(NewValue))]
public sealed class RetailPlaceChanger : BaseChanger
{
    public bool IsNullOldValue { get; set; }
    public bool IsNullNewValue { get; set; }
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;

    public override Expression<Func<Receipt, bool>> Filter => 
        receipt => receipt.RetailPlace == (IsNullOldValue ? null : OldValue);        

    public override void Update(UpdateSettersBuilder<Receipt> builder) =>
        builder.SetProperty(r => r.RetailPlace, IsNullNewValue ? null : NewValue);
}