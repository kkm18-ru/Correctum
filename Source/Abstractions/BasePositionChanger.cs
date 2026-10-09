using Correctum.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Correctum.Abstractions;

public abstract class BasePositionChanger : IAction
{
    public abstract Expression<Func<Position, bool>> Filter { get; }
    public async Task DoAsync(DbSet<Receipt> receipts) =>
        await receipts.Where(r => !r.IsReadOnly).SelectMany(r => r.Items).Where(Filter).ExecuteUpdateAsync(Update);

    public abstract void Update(UpdateSettersBuilder<Position> builder);
}
