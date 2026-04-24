using Microsoft.Win32;
using RetailCorrector.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RetailCorrector.ViewModels.Reports;

public abstract class BaseReportViewModel : BaseViewModel
{
    public abstract string ReportName { get; }
    public abstract Dictionary<string, string> ColumnDisplayNames { get; }

    public ObservableCollection<object> Items { get; } = [];
    public bool IsBusy { get; set => SetField(ref field, value); } = false;

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public ICommand ExportCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand AutoGeneratingColumnCommand { get; }

    protected BaseReportViewModel()
    {
        ExportCommand = new RelayCommand(_ => Export(), _ => Items.Count > 0 && !IsBusy);
        RefreshCommand = new RelayCommand(_ => Refresh(), _ => !IsBusy);
        AutoGeneratingColumnCommand = new RelayCommand(a =>
        {
            var e = (DataGridAutoGeneratingColumnEventArgs)a!;
            if (ColumnDisplayNames.TryGetValue(e.PropertyName, out var header))
                e.Column.Header = header;
        });
    }
    protected abstract Task<List<object>> InternalRefresh();
    protected abstract void InternalExport(string filepath);

    public async void Refresh()
    {
        try
        {
            IsBusy = true;

            Items.Clear();
            foreach (var item in await InternalRefresh())
                Items.Add(item);

            Logger.Info("Отчет \"{ReportName}\" успешно составлен.", ReportName);
            IsBusy = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось составить отчет.", "", 0, MessageBoxImage.Error);
            Logger.Error(ex, "Не удалось составить отчет \"{ReportName}\"", ReportName);
        }
    }

    public void Export()
    {
        IsBusy = true;
        var saveDialog = new SaveFileDialog
        {
            Filter = "CSV-таблица (*.csv)|*.csv",
            FileName = "report.csv",
            OverwritePrompt = true,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            AddToRecent = true
        };
        if (saveDialog.ShowDialog() ?? false)
        {
            InternalExport(saveDialog.FileName);
            MessageBox.Show("Отчет успешно экспортирован!");
            Logger.Info("Отчет \"{ReportName}\" успешно экспортирован.", ReportName);
        }
        IsBusy = false;
    }
}
