using Correctum.Abstractions;
using Correctum.Core.Enums;
using Correctum.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Correctum.Actions;

internal sealed class SetCalculationMethodReceiptAction : BindableBase,
    IReceiptAction<SetCalculationMethodReceiptAction>
{
    public static string Name => "Установка способа расчета";

    public ObservableHashSet<CalculationMethod?> Filter { get; } = [];
    public CalculationMethod Value { get; set => SetField(ref field, value); }

    public static Task InternalExecuteAsync(DbSet<Receipt> set, SetCalculationMethodReceiptAction args)
    {
        if(args.Filter.Count == 0) return Task.CompletedTask;
        foreach(var receipt in set.Include(r => r.Items))
        {
            if (receipt.IsReadOnly) continue;
            receipt.Items
                .Where(i => args.Filter.Contains(i.CalcMethod))
                .ForEach(i => i.CalcMethod = args.Value);
        }
        return Task.CompletedTask;
    }
}
