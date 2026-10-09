using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Models;
using Microsoft.EntityFrameworkCore;

namespace Correctum.Actions.Combo;

[ActionInfo("Удаление связи с оригинальным чеком")]
internal sealed class UnlinkReceiptsAction : IAction
{
    public Task DoAsync(DbSet<Receipt> receipts)
    {
        foreach(var receipt in receipts)
        {
            receipt.DocSign = null;
            receipt.DocNumber = null;
            receipt.RegId = null;
            receipt.StorageId = null;
            receipt.SessionId = null;
            receipt.DocInSession = null;
        }
        return Task.CompletedTask;
    }
}
