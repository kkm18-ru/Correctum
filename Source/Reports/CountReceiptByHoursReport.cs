using Correctum.Abstractions;
using Correctum.Attributes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Data;

namespace Correctum.Reports;

[ReportInfo("Количество чеков по часам")]
public sealed class CountReceiptByHoursReport : IReport
{
    public static (DataTable, PlotModel?) GenerateReport()
    {
        var table = new DataTable();
        table.Columns.AddRange([new("Дата"), new("Количество")]);

        var model = new PlotModel();
        var series = new LineSeries();
        var xAxis = new DateTimeAxis
        {
            Position = AxisPosition.Bottom,
            Title = "Дата",
            StringFormat = "yyyy-MM-dd",
            IntervalType = DateTimeIntervalType.Hours,
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot
        };
        var yAxis = new LinearAxis
        {
            Position = AxisPosition.Left,
            Title = "Количество",
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot
        };
        model.Axes.Add(xAxis);
        model.Axes.Add(yAxis);
        model.Series.Add(series);

        using var ctx = new StorageContext();
        var receipts = ctx.Receipts.ToArray();
        var filtered = receipts.Where(r => r.Created is not null);
        var aggregated = filtered.CountBy(r => r.Created!.Value.Round(TimeSpan.TicksPerHour));
        var dict = aggregated.ToDictionary(a => a.Key, a => a.Value);
        var minDate = filtered.Min(r => r.Created!.Value).Round(TimeSpan.TicksPerHour);
        var maxDate = filtered.Max(r => r.Created!.Value).Round(TimeSpan.TicksPerHour);

        for(var date = minDate; date <= maxDate; date = date.AddHours(1))
        {
            if(dict.TryGetValue(date, out var value))
                table.Rows.Add(date, value);
            series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(date), value));
        }


        return (table, model);
    }
}
