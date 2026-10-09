using Correctum.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Correctum.Abstractions;

public abstract class BaseChanger : IAction
{
    public abstract Expression<Func<Receipt, bool>> Filter { get; }
    public async Task DoAsync(DbSet<Receipt> receipts) =>
        await receipts.Where(r => !r.IsReadOnly).Where(Filter).ExecuteUpdateAsync(Update);

    public abstract void Update(UpdateSettersBuilder<Receipt> builder);
}
