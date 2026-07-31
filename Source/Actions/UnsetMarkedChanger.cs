using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Core.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Correctum.Actions;

//[ActionInfo("Снять метку")]
public sealed class UnsetMarkedChanger : BaseChanger
{
    public override Expression<Func<Receipt, bool>> Filter =>
        receipt => receipt.Marked;

    public override void Update(UpdateSettersBuilder<Receipt> builder) =>
        builder.SetProperty(r => r.Marked, false);
}
