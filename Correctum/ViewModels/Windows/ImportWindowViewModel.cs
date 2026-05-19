using Correctum.Abstractions;
using Correctum.Core.Models;
using Correctum.Utils;
using System.Windows;
using System.Windows.Input;

namespace Correctum.ViewModels.Windows;

public sealed class ImportWindowViewModel : BindableBase
{
    public string ImportName { get; }
    public ICommand StartCommand { get; }
    public ICommand CancelCommand { get; }
    public BaseImport Import { get; }

    public uint CurrentProgress { get; set => SetField(ref field, value); }
    public uint MaximumProgress { get; set => SetField(ref field, value); }

    public event Action? Imported;

    private CancellationTokenSource cts = null!;
    private readonly static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly BoundedProgress<uint> progress;

    internal ImportWindowViewModel(string importName, BaseImport import)
    {
        ImportName = importName;
        StartCommand = new RelayCommand(Start, () => cts?.IsCancellationRequested ?? true);
        CancelCommand = new RelayCommand(Cancel, () => !cts?.IsCancellationRequested ?? false);

        progress = new BoundedProgress<uint>();
        progress.ValueCleared += () => CurrentProgress = 0;
        progress.MaximumChanged += max => MaximumProgress = max;
        progress.ValueIncremented += () => CurrentProgress++;
        Import = import;
    }

#if DEBUG
    /// <summary>
    /// Инициализация тестового экземпляра <see cref="ImportWindowViewModel"/>
    /// </summary>
    public ImportWindowViewModel() : this("Mock", null!)
    {
        CurrentProgress = 10;
        MaximumProgress = 100;
    }
#endif

    private async void Start()
    {
        var buffer = new List<Receipt>();
        cts = new CancellationTokenSource();
        try
        {
            await foreach (var receipt in Import.ImportAsync(progress, cts.Token))
                buffer.Add(receipt);
            cts.Dispose();
            using (var ctx = new StorageContext())
            {
                await ctx.Receipts.AddRangeAsync(buffer);
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
