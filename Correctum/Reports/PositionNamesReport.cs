using Correctum.Attrubutes;
using Correctum.Utils;
using System.Data;

namespace Correctum.Reports;

[ReportInfo("Наименования позиций")]
public class PositionNamesReport : IReport
{
    public static DataTable GenerateReport()
    {
        var table = new DataTable();
        table.Columns.AddRange([new("Наименование"), new("Тип"), new("Количество чеков")]);

        var filtered = Workspace.Instance.Receipts.Where(r => r.Items is not null);
        var casted = filtered.SelectMany(r => r.Items!.Select(i => new { i.Name, i.Type, r }));
        var grouped = casted.GroupBy(x => (x.Name, x.Type));
        var ordered = grouped.OrderBy(g => g.Key.Name);
        var rows = ordered.Select(g => new object?[] { g.Key.Name, EnumLocales.PositionTypes.GetLocale(g.Key.Type), g.Select(x => x.r).Distinct().Count() });
        rows.ForEach(row => table.Rows.Add(row));
        
        return table;
    }
}
