using Microsoft.Win32;
using Correctum.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Correctum.ViewModels.Reports;

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
        ExportCommand = new RelayCommand(Export, () => Items.Count > 0 && !IsBusy);
        RefreshCommand = new RelayCommand(Refresh, () => !IsBusy);
        AutoGeneratingColumnCommand = new RelayCommand<DataGridAutoGeneratingColumnEventArgs>(args =>
        {
            if (ColumnDisplayNames.TryGetValue(args.PropertyName, out var header))
                args.Column.Header = header;
        });
    }
    protected abstract Task<List<object>> InternalRefresh();
    protected abstract void InternalExport(string filepath);
    
    private async void Refresh()
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

    private void Export()
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
