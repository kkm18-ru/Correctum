using Correctum.Abstractions;
using Correctum.Utils;
using Microsoft.Win32;
using NanoXLSX;
using OxyPlot;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Correctum.ViewModels.Windows;

public sealed class ReportWindowViewModel : BindableBase
{
    public string ReportName { get; }
    public Func<(DataTable, PlotModel?)> Func { get; }
    public bool Exporting { get; set => SetField(ref field, value); } = false;
    public DataTable Table { get; set => SetField(ref field, value); }
    public PlotModel? Plot { get; set => SetField(ref field, value); }

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    public ICommand ExportCommand { get; }

    public ReportWindowViewModel(string reportName, Func<(DataTable, PlotModel?)> function)
    {
        ReportName = reportName;
        Func = function;
        Table = null!;
        ExportCommand = new RelayCommand(Export, () => Table is not null && Table.Rows.Count > 0 && !Exporting);
        RefreshAsync().GetAwaiter().GetResult();
    }

#if DEBUG
    public ReportWindowViewModel() 
        : this("Тест", () => (new DataTable(), null)) { }
#endif

    private async Task RefreshAsync()
    {
        try
        {
            (Table, Plot) = await Task.Run(Func).ConfigureAwait(false);
            Logger.Info("Отчет \"{ReportName}\" успешно составлен.", ReportName);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось составить отчет.", "", 0, MessageBoxImage.Error);
            Logger.Error(ex, "Не удалось составить отчет \"{ReportName}\"", ReportName);
        }
    }

    private async void Export()
    {
        try
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "CSV UTF-8|*.csv|Excel-таблица|*.xlsx",
                OverwritePrompt = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                AddExtension = true,
                AddToRecent = true
            };
            if (!saveDialog.ShowDialog() ?? true) return;
            Exporting = true;
            Func<string, Task> exporter = saveDialog.FilterIndex switch
            {
                1 => ExportAsCsv,
                2 => ExportAsXlsx,
                _ => throw new NotImplementedException()
            };
            await exporter(saveDialog.FileName).ConfigureAwait(false);
            MessageBox.Show("Отчет успешно экспортирован!");
            Logger.Info("Отчет \"{ReportName}\" успешно экспортирован.", ReportName);
        }
        catch(Exception ex)
        {
            Logger.Error(ex, "Необработаная ошибка при экспорте отчета.");
        }
        finally
        {
            Exporting = false;
        }
    }

    private async Task ExportAsCsv(string filepath)
    {
        var builder = new StringBuilder();
        for (var i = 0; i < Table.Columns.Count; i++)
        {
            var str = Table.Columns[i].ColumnName;
            if (str.Contains(';')) str = $"\"{str}\"";
            builder.Append(str);
            if (i == Table.Columns.Count - 1) builder.AppendLine();
            else builder.Append(';');
        }
        for (var r = 0; r < Table.Rows.Count; r++)
            builder.AppendLine(string.Join(';', Table.Rows[r].ItemArray.Select(o =>
            {
                var str = string.Empty;

                if (o is string s) str = s;
                else if (o is IFormattable f) 
                    str = f.ToString(null, CultureInfo.InvariantCulture);
                else str = o?.ToString() ?? string.Empty;

                if (str.Contains(';')) 
                    str = $"\"{str}\"";

                return str;
            })));

        await File.WriteAllTextAsync(filepath, builder.ToString(), Encoding.UTF8);
    }

    private async Task ExportAsXlsx(string filepath)
    {
        var workbook = new Workbook(filepath, "Отчет");
        var sheet = workbook.CurrentWorksheet;
        for (var i = 0; i < Table.Columns.Count; i++)
            sheet.AddNextCell(Table.Columns[i].ColumnName);
        for(var i = 0; i < Table.Rows.Count; i++)
        {
            sheet.GoToNextRow();
            var row = Table.Rows[i];
            foreach(var cell in row.ItemArray)
                sheet.AddNextCell(cell);
        }
        await workbook.SaveAsync();
    }
}
