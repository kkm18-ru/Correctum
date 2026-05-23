using Correctum.Abstractions;
using Correctum.Core.Models;
using Correctum.Utils;
using Correctum.ViewModels.Controls;
using System.Windows;
using System.Windows.Input;

namespace Correctum.ViewModels.Windows;

public sealed class ImportWindowViewModel : BindableBase
{
    public string ImportName { get; }
    public ICommand StartCommand { get; }
    public ICommand CancelCommand { get; }
    public BaseImport Import { get; }

    public event Action? Imported;

    private CancellationTokenSource cts = null!;
    private readonly static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private MultiProgress progress;

    internal ImportWindowViewModel(string importName, BaseImport import, MultiProgress progress)
    {
        StartCommand = new RelayCommand(Start, () => cts?.IsCancellationRequested ?? true);
        CancelCommand = new RelayCommand(Cancel, () => !cts?.IsCancellationRequested ?? false);

        ImportName = importName;
        this.progress = progress;
        Import = import;
    }

#if DEBUG
    /// <summary>
    /// Инициализация тестового экземпляра <see cref="ImportWindowViewModel"/>
    /// </summary>
    public ImportWindowViewModel() : this("Mock", null!, null!) { }
#endif

    private async void Start()
    {
        var buffer = new List<Receipt>();
        cts = new CancellationTokenSource();
        try
        {
            progress.RemoveAllStages();
            await foreach (var receipt in Import.ImportAsync(progress, cts.Token))
                buffer.Add(receipt);
            cts.Dispose();
            using (var ctx = new StorageContext())
            {
                await ctx.Receipts.AddRangeAsync(buffer.OrderBy(r => r.Created));
                await ctx.SaveChangesAsync();
            }
            Imported?.Invoke();
        }
        catch (OperationCanceledException e)
        {
            Logger.Warn(e, "Импортирование чеков была отменена пользователем");
            MessageBox.Show("Выгрузка чеков была отменена пользователем", "", 0, MessageBoxImage.Hand);
        }
        catch (Exception ex)
        {
            Logger.Warn(ex, "Неизвестная ошибка");
            MessageBox.Show(ex.Message, "Ошибка выгрузки чеков", 0, MessageBoxImage.Error);
            Cancel();
        }
    }

    private void Cancel() => cts?.Cancel();

    public void OnClosing() => Import.OnClosing();
}
