using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Core.Models;
using System.Linq.Expressions;

namespace Correctum.Actions;

//[ActionInfo("Удалить непомеченное")]
public sealed class NotMarkedRemover : BaseRemover
{
    public override Expression<Func<Receipt, bool>> Filter =>
        receipt => !receipt.Marked;
}
