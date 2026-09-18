using Microsoft.EntityFrameworkCore;
using Correctum.Core.Models;

namespace Correctum.Abstractions;

internal interface IReceiptAction<TSelf>
    where TSelf : class, IReceiptAction<TSelf>, new()
{
    public abstract static string Name { get; }
    internal abstract static Task InternalExecuteAsync(DbSet<Receipt> set, TSelf args);
}
