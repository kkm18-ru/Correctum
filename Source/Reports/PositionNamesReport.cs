using Correctum.Abstractions;
using Correctum.Attributes;
using Correctum.Utils;
using Microsoft.EntityFrameworkCore;
using OxyPlot;
using System.Data;

namespace Correctum.Reports;

[ReportInfo("Наименования позиций")]
public class PositionNamesReport : IReport
{
    public static (DataTable, PlotModel?) GenerateReport()
    {
        var table = new DataTable();
        table.Columns.AddRange([new("Наименование"), new("Тип"), new("Количество чеков")]);

        using var ctx = new StorageContext();
        ctx.Positions.Load();
        var receipts = ctx.Receipts.ToArray();
        var casted = receipts.SelectMany(r => r.Items.Select(i => new { i.Name, i.Type, r }));
        var grouped = casted.GroupBy(x => (x.Name, x.Type));
        var ordered = grouped.OrderBy(g => g.Key.Name);
        var rows = ordered.Select(g => new object?[] { g.Key.Name, EnumLocales.PositionTypes.GetLocale(g.Key.Type), g.Select(x => x.r).Distinct().Count() });
        rows.ForEach(row => table.Rows.Add(row));
        
        return (table, null);
    }
}
