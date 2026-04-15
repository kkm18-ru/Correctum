using Correctum.Core.Enums;
using RetailCorrector.Utils;
using System.IO;
using System.Text;

namespace RetailCorrector.ViewModels.Reports;

public class PositionNamesReportViewModel : BaseReportViewModel
{
    public override string ReportName => "Наименования позиций";

    public override Dictionary<string, string> ColumnDisplayNames => new()
    {
        { nameof(ReportItem.Name), "Наименование"  },
        { nameof(ReportItem.TypeName), "Тип" },
        { nameof(ReportItem.CountReceipts), "Количество чеков" },
    };

    protected override void InternalExport(string filepath)
    {
        var lines = Items.Cast<ReportItem>().Select(i => $"\"{i.Name}\";\"{i.TypeName}\";{i.CountReceipts:F2}");
        File.WriteAllText(filepath, string.Join("\r\n", ["\"Наименование\";\"Тип\";\"Количество чеков\"", .. lines]), Encoding.UTF8);
    }

    protected override Task<List<object>> InternalRefresh() =>
        Task.Run(() => Workspace.Instance.Receipts
            .SelectMany(r => r.Items.Select(i => new { i.Name, i.Type, r, }))
            .GroupBy(x => (x.Name, x.Type))
            .Select(n => new ReportItem(n.Key.Name, n.Key.Type, n
                .Select(x => x.r)
                .Distinct()
                .Count()))
            .OrderBy(i => i.Name)
            .Cast<object>()
            .ToList());

    public readonly struct ReportItem(string? name, PositionType? type, int numReceipts)
    {
        public string Name { get; } = name ?? "";
        public string TypeName { get; } = EnumLocales.PositionTypes.GetLocale(type, "");
        public int CountReceipts { get; } = numReceipts;
    }
}
