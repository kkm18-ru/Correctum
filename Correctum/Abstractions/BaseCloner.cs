using Correctum.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Linq.Expressions;

namespace Correctum.Abstractions;

public abstract class BaseCloner : IAction
{
    public abstract void Change(Receipt receipt);
    public abstract Expression<Func<Receipt, bool>> Filter { get; }
    public async Task DoAsync(DbSet<Receipt> receipts)
    {
        var _receipts = receipts.Where(Filter).ToImmutableArray();
        foreach (var receipt in _receipts)
        {
            var currReceipt = receipt.Clone();
            Change(currReceipt);
            await receipts.AddAsync(currReceipt);
        }
    }
}
