using Correctum.Models;
using Microsoft.EntityFrameworkCore;

namespace Correctum.Abstractions;

public interface IAction
{
    Task DoAsync(DbSet<Receipt> receipts);
}
