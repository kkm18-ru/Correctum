using Microsoft.Win32;
using RetailCorrector.Utils;
using System.Windows;
using System.Windows.Input;

namespace RetailCorrector.ViewModels.Exports;

public abstract class BaseExportViewModel : BaseViewModel
{
    public abstract string ExportName { get; }
    public bool IsBusy { get; set => SetField(ref field, value); }
    public ICommand ExportCommand { get; }

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public BaseExportViewModel()
    {
        ExportCommand = new RelayCommand(_ => Export());
    }

    protected abstract Task<bool> InternalExport();

    public async void Export()
    {
        try
        {
            IsBusy = true;
            var forcedFinish = await InternalExport();
            if(!forcedFinish) MessageBox.Show("Экспорт завершен успешно");
        }
        catch(Exception ex)
        {
            Logger.Error(ex, "Не удалось экспортировать ФД");
            MessageBox.Show(ex.Message, "Не удалось экспортировать ФД.", 0, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
