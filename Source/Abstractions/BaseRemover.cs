using Correctum.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Correctum.Abstractions;

public abstract class BaseRemover : IAction
{
    public abstract Expression<Func<Receipt, bool>> Filter { get; }
    public async Task DoAsync(DbSet<Receipt> receipts) =>
        await receipts.Where(Filter).ExecuteDeleteAsync();
}
