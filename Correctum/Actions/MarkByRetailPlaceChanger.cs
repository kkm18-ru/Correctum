using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Attributes.ModuleViewProperty;
using Correctum.Core.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Correctum.Actions;

//[ActionInfo("Пометить по месту расчета")]
[CheckBox("Место расчета отсутствует", nameof(IsNullRetailPlace))]
[TextBox("Место расчета", nameof(RetailPlace))]
public sealed class MarkByRetailPlaceChanger : BaseChanger
{
    public bool IsNullRetailPlace { get; set; }
    public string RetailPlace { get; set; } = string.Empty;

    public override Expression<Func<Receipt, bool>> Filter =>
        r => r.RetailPlace == RetailPlace;

    public override void Update(UpdateSettersBuilder<Receipt> builder) =>
        builder.SetProperty(r => r.Marked, true);
}
