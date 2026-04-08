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
    public event Action<string, string, MessageBoxButton, MessageBoxImage>? NotifyCreated;

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    protected BaseExportViewModel()
    {
        ExportCommand = new RelayCommand(async _ => await Export());
    }

    protected void ShowNotification(string message, string caption = "", MessageBoxImage image = 0, MessageBoxButton button = 0) =>
        NotifyCreated?.Invoke(message, caption, button, image);

    protected abstract Task<bool> InternalExport();

    public async Task Export()
    {
        try
        {
            IsBusy = true;
            var forcedFinish = await InternalExport();
            if(!forcedFinish) ShowNotification("Экспорт завершен успешно");
        }
        catch(Exception ex)
        {
            Logger.Error(ex, "Не удалось экспортировать ФД");
            ShowNotification(ex.Message, "Не удалось экспортировать ФД.", MessageBoxImage.Error);
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
