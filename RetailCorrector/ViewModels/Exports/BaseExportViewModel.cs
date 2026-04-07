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

    protected BaseExportViewModel()
    {
        ExportCommand = new RelayCommand(async _ => await Export());
    }

    protected abstract Task<bool> InternalExport();

    public async Task Export()
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

public abstract class FileExportViewModel : BaseExportViewModel
{
    public virtual string Filter => string.Empty;
    protected override async Task<bool> InternalExport()
    {
        var dialog = new SaveFileDialog
        {
            Filter = Filter,
            ShowHiddenItems = true,
            DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            AddExtension = true,
            AddToRecent = true,
            CheckFileExists = false,
            CheckPathExists = true,
            OverwritePrompt = true,
        };
        if (!dialog.ShowDialog() ?? false) 
            return true;
        await InternalExport(dialog.FileName).AsTask();
        return false;
    }

    protected abstract ValueTask InternalExport(string filepath);
}
