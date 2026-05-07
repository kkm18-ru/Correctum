using Correctum.Utils;
using Microsoft.Win32;
using NanoXLSX;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Correctum.ViewModels.Windows;

public sealed class ReportWindowViewModel : BaseViewModel
{
    public string ReportName { get; }
    public Func<DataTable> Func { get; }
    public bool IsBusy { get; set => SetField(ref field, value); } = false;
    public DataTable Table { get; set => SetField(ref field, value); }

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    public ICommand ExportCommand { get; }
    public ICommand RefreshCommand { get; }

    public ReportWindowViewModel(string reportName, Func<DataTable> function)
    {
        ReportName = reportName;
        Func = function;
        Table = null!;
        ExportCommand = new RelayCommand(Export, () => Table is not null && Table.Rows.Count > 0 && !IsBusy);
        RefreshCommand = new RelayCommand(Refresh, () => !IsBusy);
    }

#if DEBUG
    public ReportWindowViewModel() 
        : this("Тест", () => new DataTable()) { }
#endif

    private async void Refresh()
    {
        try
        {
            IsBusy = true;

            Table = await Task.Run(Func);

            Logger.Info("Отчет \"{ReportName}\" успешно составлен.", ReportName);
            IsBusy = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось составить отчет.", "", 0, MessageBoxImage.Error);
            Logger.Error(ex, "Не удалось составить отчет \"{ReportName}\"", ReportName);
        }
    }

    private void Export()
    {
        IsBusy = true;
        try
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "CSV UTF-8 (Разделитель - точка с запятой)|*.csv|Excel|*.xlsx",
                OverwritePrompt = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                AddExtension = true,
                AddToRecent = true
            };
            if (!saveDialog.ShowDialog() ?? true) return;
            switch (saveDialog.FilterIndex)
            {
                case 1: ExportAsCsv(saveDialog.FileName); break;
                case 2: ExportAsXlsx(saveDialog.FileName); break;
                default: throw new NotImplementedException();
            }
            MessageBox.Show("Отчет успешно экспортирован!");
            Logger.Info("Отчет \"{ReportName}\" успешно экспортирован.", ReportName);
        }
        catch(Exception ex)
        {
            Logger.Error(ex, "Необработаная ошибка при экспорте отчета.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ExportAsCsv(string filepath)
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

        File.WriteAllText(filepath, builder.ToString(), Encoding.UTF8);
    }

    private void ExportAsXlsx(string filepath)
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
        workbook.Save();
    }
}
