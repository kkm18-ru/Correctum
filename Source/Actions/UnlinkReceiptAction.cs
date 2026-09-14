using Correctum.Abstractions;
using Correctum.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Correctum.Actions;

internal sealed class UnlinkReceiptAction() : IReceiptAction<UnlinkReceiptAction>
{
    public static string Name => "Отвязка от оригиналов";

    public static async Task InternalExecuteAsync(DbSet<Receipt> set, UnlinkReceiptAction args)
    {
        foreach (var receipt in set)
        {
            receipt.RegId = null;
            receipt.StorageId = null;
            receipt.DocNumber = null;
            receipt.SessionId = null;
            receipt.DocInSession = null;
            receipt.DocSign = null;
        }
    }
}
